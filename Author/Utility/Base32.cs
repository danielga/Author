using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Author.Utility
{
    /*
     * Derived from https://github.com/google/google-authenticator-android/blob/master/AuthenticatorApp/src/main/java/com/google/android/apps/authenticator/Base32String.java
     * 
     * Copyright (C) 2016 BravoTango86
     *
     * Licensed under the Apache License, Version 2.0 (the "License");
     * you may not use this file except in compliance with the License.
     * You may obtain a copy of the License at
     *
     *      http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    public static class Base32
    {
        public const string ValidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        public static readonly char[] ValidChars = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', '2', '3', '4', '5', '6', '7'
        };
        const string Separator = "-";

        static readonly int _mask = 31;
        static readonly int _shift = 5;
        static readonly Dictionary<char, int> _charMap = new Dictionary<char, int>
        {
            { 'A',  0 }, { 'B',  1 }, { 'C',  2 }, { 'D',  3 },
            { 'E',  4 }, { 'F',  5 }, { 'G',  6 }, { 'H',  7 },
            { 'I',  8 }, { 'J',  9 }, { 'K', 10 }, { 'L', 11 },
            { 'M', 12 }, { 'N', 13 }, { 'O', 14 }, { 'P', 15 },
            { 'Q', 16 }, { 'R', 17 }, { 'S', 18 }, { 'T', 19 },
            { 'U', 20 }, { 'V', 21 }, { 'W', 22 }, { 'X', 23 },
            { 'Y', 24 }, { 'Z', 25 }, { '2', 26 }, { '3', 27 },
            { '4', 28 }, { '5', 29 }, { '6', 30 }, { '7', 31 }
        };

        public static byte[] Decode(string encoded)
        {
            // Remove whitespace and separators
            encoded = encoded.Trim().Replace(Separator, "");

            // Remove padding. Note: the padding is used as hint to determine how many
            // bits to decode from the last incomplete chunk (which is commented out
            // below, so this may have been wrong to start with).
            encoded = Regex.Replace(encoded, "[=]*$", "");

            // Canonicalize to all upper case
            encoded = encoded.ToUpper();
            if (encoded.Length == 0)
                return new byte[0];

            int encodedLength = encoded.Length;
            int outLength = encodedLength * _shift / 8;
            byte[] result = new byte[outLength];
            int buffer = 0;
            int next = 0;
            int bitsLeft = 0;
            foreach (char c in encoded.ToCharArray())
            {
                if (!_charMap.ContainsKey(c))
                    throw new DecodingException("Illegal character: " + c);

                buffer <<= _shift;
                buffer |= _charMap[c] & _mask;
                bitsLeft += _shift;
                if (bitsLeft >= 8)
                {
                    result[next++] = (byte)(buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }

            // We'll ignore leftover bits for now.
            //
            // if (next != outLength || bitsLeft >= SHIFT) {
            //  throw new DecodingException("Bits left: " + bitsLeft);
            // }
            return result;
        }


        public static string Encode(byte[] data, bool padOutput = false)
        {
            if (data.Length == 0)
                return "";

            // SHIFT is the number of bits per output character, so the length of the
            // output is the length of the input multiplied by 8/SHIFT, rounded up.
            if (data.Length >= (1 << 28))
                // The computation below will fail, so don't do it.
                throw new ArgumentOutOfRangeException("data");

            int outputLength = (data.Length * 8 + _shift - 1) / _shift;
            StringBuilder result = new StringBuilder(outputLength);

            int buffer = data[0];
            int next = 1;
            int bitsLeft = 8;
            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < _shift)
                {
                    if (next < data.Length)
                    {
                        buffer <<= 8;
                        buffer |= (data[next++] & 0xff);
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = _shift - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }

                int index = _mask & (buffer >> (bitsLeft - _shift));
                bitsLeft -= _shift;
                result.Append(ValidChars[index]);
            }

            if (padOutput)
            {
                int padding = 8 - (result.Length % 8);
                if (padding > 0)
                    result.Append(new string('=', padding == 8 ? 0 : padding));
            }

            return result.ToString();
        }

        class DecodingException : Exception
        {
            public DecodingException(string message) :
                base(message)
            { }
        }
    }
}
