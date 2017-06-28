using System;
using System.Diagnostics;
using Author.Utility;
using PCLCrypto;

namespace Author.OTP
{
    public class HashBased : IBaseGenerator
    {
        public const MacAlgorithm DefaultAlgorithm = MacAlgorithm.HmacSha1;

        public string Type { get; set; }

        public string Name { get; set; }

        public byte Digits { get; set; }

        public byte Period { get; set; }

        public string SecretData
        {
            get
            {
                return _secretData;
            }

            set
            {
                _secretData = value;
                SecretBytes = Base32.Decode(value);
                IMacAlgorithmProvider provider =
                    WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(Algorithm);
                Hash = provider.CreateHash(SecretBytes);
            }
        }

        protected MacAlgorithm Algorithm = DefaultAlgorithm;
        protected CryptographicHash Hash;
        protected byte[] SecretBytes;
        string _secretData = null;

        public HashBased(Secret secret, MacAlgorithm algorithm = DefaultAlgorithm)
        {
            Debug.Assert(!string.IsNullOrEmpty(secret.Data), "Secret is not a valid string!");

            Name = secret.Name;
            Type = secret.Type;
            Digits = secret.Digits;
            Period = 0;
            Algorithm = algorithm;
            SecretData = secret.Data;
        }

        public virtual string GetCode(long counter)
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
            int otp = (binary & 0x7FFFFFFF) % (int)Math.Pow(10, Digits);
            return otp.ToString().PadLeft(Digits, '0');
        }
    }
}
