using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace Author.OTP
{
    public struct Secret
    {
        public Guid Identifier;
        public string Name;
        public Type Type { get; set; }
        public string Issuer { get; set; }
        public HashAlgorithmName Algorithm { get; set; }
        public byte Digits { get; set; }
        public long Counter { get; set; }
        public byte Period { get; set; }
        public string Data { get; set; }

        private static readonly Regex LabelRegex = new Regex("^/(.+): *(.+)$");

        public static Secret Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            Uri uri = new Uri(input);
            if (uri.Scheme != "otpauth")
            {
                throw new ArgumentException("Invalid scheme", nameof(uri));
            }

            NameValueCollection kvs = HttpUtility.ParseQueryString(uri.Query);
            Secret auth = new Secret
            {
                Type = Type.Parse(uri.Host),
                Algorithm = HashAlgorithmName.SHA1,
                Digits = 6,
                Period = 30
            };

            Match match = LabelRegex.Match(HttpUtility.UrlDecode(uri.AbsolutePath));
            if (match.Success && match.Groups.Count == 3)
            {
                auth.Issuer = match.Groups[1].Value;
                auth.Name = match.Groups[2].Value;
            }
            else
            {
                auth.Name = uri.AbsolutePath.Substring(1);
            }

            string[] secrets = kvs.GetValues("secret");
            if (secrets == null || secrets.Length == 0)
            {
                throw new ArgumentException("Invalid secret", nameof(uri));
            }

            auth.Data = secrets[0];

            string[] issuers = kvs.GetValues("issuer");
            if (issuers != null && issuers.Length > 0)
            {
                string issuer = issuers[0];
                if (auth.Issuer != null && issuer != auth.Issuer)
                {
                    throw new ArgumentException("Different issuers between label and parameter", nameof(uri));
                }

                auth.Issuer = issuer;
            }

            string[] algorithms = kvs.GetValues("algorithm");
            if (algorithms != null && algorithms.Length > 0)
            {
                switch (algorithms[0])
                {
                    case "SHA1":
                        auth.Algorithm = HashAlgorithmName.SHA1;
                        break;

                    case "SHA256":
                        auth.Algorithm = HashAlgorithmName.SHA256;
                        break;

                    case "SHA512":
                        auth.Algorithm = HashAlgorithmName.SHA512;
                        break;

                    default:
                        throw new ArgumentException("Invalid algorithm", nameof(uri));
                }
            }

            string[] digits = kvs.GetValues("digits");
            if (digits != null && digits.Length > 0)
            {
                byte digit = byte.Parse(digits[0]);
                if (digit < 4 || digit > 8)
                {
                    throw new ArgumentException("Invalid number of digits", nameof(uri));
                }

                auth.Digits = digit;
            }

            string[] counters = kvs.GetValues("counter");
            if (counters != null && counters.Length > 0)
            {
                auth.Counter = long.Parse(counters[0]);
            }
            else if (auth.Type == Type.Hash)
            {
                throw new ArgumentException("Invalid counter", nameof(uri));
            }

            string[] periods = kvs.GetValues("period");
            if (periods != null && periods.Length > 0)
            {
                byte period = byte.Parse(periods[0]);
                if (period < 5 || period > 60)
                {
                    throw new ArgumentException("Invalid period", nameof(uri));
                }

                auth.Period = period;
            }

            string[] uuids = kvs.GetValues("uuid");
            if (uuids != null && uuids.Length > 0)
            {
                auth.Identifier = Guid.Parse(uuids[0]);
            }
            else
            {
                auth.Identifier = Guid.NewGuid();
            }

            return auth;
        }

        public override string ToString()
        {
            NameValueCollection kvs = HttpUtility.ParseQueryString("");
            kvs.Set("secret", Data);
            kvs.Set("issuer", Issuer);
            kvs.Set("algorithm", Algorithm.Name);
            kvs.Set("digits", Digits.ToString());
            kvs.Set("uuid", Identifier.ToString());

            if (Counter != 0)
            {
                kvs.Set("counter", Counter.ToString());
            }

            if (Period != 0)
            {
                kvs.Set("period", Period.ToString());
            }

            UriBuilder builder = new UriBuilder
            {
                Scheme = "otpauth",
                Host = Type.Name,
                Path = Issuer != null ? $"{Issuer}:{Name}" : Name,
                Query = HttpUtility.UrlDecode(kvs.ToString())
            };
            return builder.Uri.AbsoluteUri;
        }
    }
}
