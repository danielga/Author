using System;
using System.Collections.Specialized;
using System.Web;

namespace Author.Utility
{
    public struct OTPAuth
    {
        public byte Type;
        public string Label;
        public string Secret;
        public string Issuer;
        public string Algorithm;
        public byte Digits;
        public long Counter;
        public byte Period;

        public static OTPAuth ParseString(string url)
        {
            Uri uri = new Uri(url);
            NameValueCollection blah = HttpUtility.ParseQueryString(uri.Query);
            foreach (string s in blah)
                foreach (string v in blah.GetValues(s))
                    System.Diagnostics.Debug.WriteLine("{0} {1}", s, v);
            return new OTPAuth();
        }
    }
}
