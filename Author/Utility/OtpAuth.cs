using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Author.Utility
{
    public struct OtpAuth
    {
        public byte Type;
        public string Name;
        public string Secret;
        public string Issuer;
        public string Algorithm;
        public byte Digits;
        public long Counter;
        public byte Period;

        private static readonly Regex LabelRegex = new Regex("^/(.+): *(.+)$");

        public static OtpAuth ParseString(Uri uri)
        {
            if (uri.Scheme != "otpauth")
                throw new ArgumentException("Invalid scheme", nameof(uri));

            var kvs = HttpUtility.ParseQueryString(uri.Query);
            var auth = new OtpAuth
            {
                Type = OTP.Type.Time,
                Algorithm = "SHA1",
                Digits = 6,
                Period = 30
            };

            if (!OTP.Type.FromName.TryGetValue(uri.Authority, out auth.Type))
                throw new ArgumentException("Invalid type", nameof(uri));

            var path = Uri.UnescapeDataString(uri.AbsolutePath);
            var match = LabelRegex.Match(path);
            if (match.Success && match.Groups.Count == 3)
            {
                auth.Issuer = match.Groups[1].Value;
                auth.Name = match.Groups[2].Value;
            }
            else
            {
                auth.Name = path.Substring(1);
            }

            var secrets = kvs.GetValues("secret");
            if (secrets == null || secrets.Length == 0)
                throw new ArgumentException("Invalid secret", nameof(uri));

            auth.Secret = secrets[0];

            var issuers = kvs.GetValues("issuer");
            if (issuers != null && issuers.Length > 0)
            {
                var issuer = issuers[0];
                if (auth.Issuer != null && issuer != auth.Issuer)
                    throw new ArgumentException("Different issuers between label and parameter", nameof(uri));

                auth.Issuer = issuer;
            }

            var algorithms = kvs.GetValues("algorithm");
            if (algorithms != null && algorithms.Length > 0)
            {
                var algorithm = algorithms[0];
                if (algorithm != "SHA1" && algorithm != "SHA256" && algorithm != "SHA512")
                    throw new ArgumentException("Invalid algorithm", nameof(uri));

                auth.Algorithm = algorithm;
            }

            var digits = kvs.GetValues("digits");
            if (digits != null && digits.Length > 0)
            {
                var digit = byte.Parse(digits[0]);
                if (digit < 4 || digit > 8)
                    throw new ArgumentException("Invalid number of digits", nameof(uri));

                auth.Digits = digit;
            }

            var counters = kvs.GetValues("counter");
            if (counters != null && counters.Length > 0)
                auth.Counter = long.Parse(counters[0]);
            else if (auth.Type == OTP.Type.Hash)
                throw new ArgumentException("Invalid counter", nameof(uri));

            var periods = kvs.GetValues("period");
            if (periods == null || periods.Length <= 0)
                return auth;

            var period = byte.Parse(periods[0]);
            if (period < 5 || period > 60)
                throw new ArgumentException("Invalid period", nameof(uri));

            auth.Period = period;

            return auth;
        }
    }
}
