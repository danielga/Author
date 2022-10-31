using Author.Utility;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

namespace Author.OTP
{
    public class Secret : INotifyPropertyChanged, IEquatable<Secret>
    {
        private bool _dirtySecret = true;
        private IBaseGenerator _generator;
        private long _nextUpdate = long.MinValue;

        private Guid _identifier = Guid.NewGuid();
        public Guid Identifier
        {
            get => _identifier;

            set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Invalid identifier");

                if (value != _identifier)
                {
                    _identifier = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _name = null;
        public string? Name
        {
            get => _name;

            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private Type _type = Type.Time;
        public Type Type
        {
            get => _type;

            set
            {
                if (value != _type)
                {
                    _type = value;
                    _dirtySecret = true;
                    OnPropertyChanged();
                }
            }
        }

        private string? _issuer = null;
        public string? Issuer
        {
            get => _issuer;

            set
            {
                if (value != _issuer)
                {
                    _issuer = value;
                    OnPropertyChanged();
                }
            }
        }

        private HashAlgorithmName _algorithm = HashBased.DefaultAlgorithm;
        public HashAlgorithmName Algorithm
        {
            get => _algorithm;

            set
            {
                if (string.IsNullOrEmpty(value.Name))
                    throw new ArgumentException("Invalid algorithm");

                if (value != _algorithm)
                {
                    _algorithm = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _digits = 6;
        public byte Digits
        {
            get => _digits;

            set
            {
                if (value < 4 || value > 8)
                    throw new ArgumentException("Invalid number of digits (must be between 4 and 8)");

                if (value != _digits)
                {
                    _digits = value;
                    OnPropertyChanged();
                }
            }
        }

        private long _counter = 0;
        public long Counter
        {
            get => _counter;

            set
            {
                if (value != _counter)
                {
                    _counter = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte _period = 30;
        public byte Period
        {
            get => _period;

            set
            {
                if (value < 5 || value > 60)
                    throw new ArgumentException("Invalid period (must be between 5 and 60)");

                if (value != _period)
                {
                    _period = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _data;
        public string Data
        {
            get => _data;

            set
            {
                if (value == null)
                    throw new ArgumentException("Invalid secret data");

                if (value != _data)
                {
                    _data = value;
                    _dirtySecret = true;
                    OnPropertyChanged();
                }
            }
        }

        private string? _code = null;
        public string? Code
        {
            get
            {
                UpdateCode(Time.GetCurrent());
                return _code;
            }

            private set
            {
                if (value != _code)
                {
                    _code = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Secret(string data = "")
        {
            _data = data;
            _generator = Factory.CreateGenerator(Type, _data, Algorithm);
        }

        public string GetCode(long timestamp)
        {
            if (_dirtySecret)
            {
                _dirtySecret = false;
                _generator = Factory.CreateGenerator(Type, Data, Algorithm);
            }

            return _generator.GetCode(timestamp, Digits, Period);
        }

        public bool UpdateCode(long timestamp, bool force = false)
        {
            if (!force && timestamp < _nextUpdate)
                return false;

            _nextUpdate = timestamp + Period - timestamp % Period;
            Code = GetCode(timestamp);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static readonly Regex LabelRegex = new("^/(.+): *(.+)$");

        public static Secret Parse(string? input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var uri = new Uri(input);
            if (uri.Scheme != "otpauth")
                throw new ArgumentException("Invalid scheme", nameof(input));

            var kvs = HttpUtility.ParseQueryString(uri.Query);

            var kvsSecrets = kvs.GetValues("secret");
            var kvsSecret = kvsSecrets?[0];
            if (kvsSecret == null)
                throw new ArgumentException("Invalid secret", nameof(input));

            var secret = new Secret(kvsSecret)
            {
                Type = TypeExtensions.Parse(uri.Host)
            };

            Match match = LabelRegex.Match(HttpUtility.UrlDecode(uri.AbsolutePath));
            if (match.Success && match.Groups.Count == 3)
            {
                secret.Issuer = match.Groups[1].Value;
                secret.Name = match.Groups[2].Value;
            }
            else
                secret.Name = HttpUtility.UrlDecode(uri.AbsolutePath[1..]);

            var kvsIssuers = kvs.GetValues("issuer");
            var kvsIssuer = kvsIssuers?[0];
            if (!string.IsNullOrEmpty(kvsIssuer))
            {
                if (secret.Issuer != null && kvsIssuer != secret.Issuer)
                    throw new ArgumentException("Different issuers between label and parameter", nameof(input));

                secret.Issuer = kvsIssuer;
            }

            var kvsAlgorithms = kvs.GetValues("algorithm");
            var kvsAlgorithm = kvsAlgorithms?[0];
            if (!string.IsNullOrEmpty(kvsAlgorithm))
                secret.Algorithm = kvsAlgorithm switch
                {
                    "SHA1" => HashAlgorithmName.SHA1,
                    "SHA256" => HashAlgorithmName.SHA256,
                    "SHA512" => HashAlgorithmName.SHA512,
                    _ => throw new ArgumentException("Invalid algorithm", nameof(input)),
                };

            var kvsDigitsArray = kvs.GetValues("digits");
            var kvsDigits = kvsDigitsArray?[0];
            if (!string.IsNullOrEmpty(kvsDigits))
                secret.Digits = byte.Parse(kvsDigits);

            var kvsCounters = kvs.GetValues("counter");
            var kvsCounter = kvsCounters?[0];
            if (!string.IsNullOrEmpty(kvsCounter))
                secret.Counter = long.Parse(kvsCounter);
            else if (secret.Type == Type.Hash)
                throw new ArgumentException("Invalid counter", nameof(input));

            var kvsPeriods = kvs.GetValues("period");
            var kvsPeriod = kvsPeriods?[0];
            if (!string.IsNullOrEmpty(kvsPeriod))
                secret.Period = byte.Parse(kvsPeriod);

            var kvsUuids = kvs.GetValues("uuid");
            var kvsUuid = kvsUuids?[0];
            if (!string.IsNullOrEmpty(kvsUuid))
                secret.Identifier = Guid.Parse(kvsUuid);

            return secret;
        }

        public override string ToString()
        {
            NameValueCollection kvs = HttpUtility.ParseQueryString("");

            if (!string.IsNullOrEmpty(Data))
                kvs.Set("secret", Data);

            if (!string.IsNullOrEmpty(Issuer))
                kvs.Set("issuer", Issuer);

            if (!string.IsNullOrEmpty(Algorithm.Name))
                kvs.Set("algorithm", Algorithm.Name);

            if (Digits != 0)
                kvs.Set("digits", Digits.ToString());

            if (Identifier != Guid.Empty)
                kvs.Set("uuid", Identifier.ToString());

            if (Counter != 0)
                kvs.Set("counter", Counter.ToString());

            if (Period != 0)
                kvs.Set("period", Period.ToString());

            var builder = new UriBuilder
            {
                Scheme = "otpauth",
                Host = TypeExtensions.ToString(Type),
                Path = Issuer != null ? $"{Issuer}:{Name}" : Name,
                Query = HttpUtility.UrlDecode(kvs.ToString())
            };
            return builder.Uri.AbsoluteUri;
        }

        public bool Equals(Secret? other)
        {
            if (other == null)
                return false;

            return Identifier == other.Identifier &&
                   Algorithm == other.Algorithm &&
                   Counter == other.Counter &&
                   Data == other.Data &&
                   Digits == other.Digits &&
                   Issuer == other.Issuer &&
                   Name == other.Name &&
                   Period == other.Period &&
                   Type == other.Type;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Secret);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}
