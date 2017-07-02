using System;
using System.Diagnostics;
using Author.Utility;
using PCLCrypto;

namespace Author.OTP
{
    public class HashBased : IBaseGenerator
    {
        public const MacAlgorithm DefaultAlgorithm = MacAlgorithm.HmacSha1;

        protected CryptographicHash Hash = null;
        protected byte[] SecretBytes = null;
        protected string SecretData = null;

        public HashBased(string secret, MacAlgorithm algorithm = DefaultAlgorithm)
        {
            SecretData = secret;
            SecretBytes = Base32.Decode(secret);
            IMacAlgorithmProvider provider =
                WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(algorithm);
            Hash = provider.CreateHash(SecretBytes);
        }

        public virtual string GetCode(long counter, byte digits, byte period)
        {
            byte[] ts = BitConverter.GetBytes(counter);
            ts[4] = ts[3];
            ts[5] = ts[2];
            ts[6] = ts[1];
            ts[7] = ts[0];
            ts[0] = 0;
            ts[1] = 0;
            ts[2] = 0;
            ts[3] = 0;

            Hash.Append(ts);
            byte[] hash = Hash.GetValueAndReset();

            int offset = hash[hash.Length - 1] & 0x0F;
            int binary = hash[offset + 0] << 24 |
                hash[offset + 1] << 16 |
                hash[offset + 2] << 8 |
                hash[offset + 3];
            int otp = (binary & 0x7FFFFFFF) % (int)Math.Pow(10, digits);
            return otp.ToString().PadLeft(digits, '0');
        }
    }
}
