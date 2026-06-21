
// https://github.com/AutoItConsulting/text-encoding-detect
//
// Copyright 2015-2016 Jonathan Bennett <jon@autoitscript.com>
//
// https://www.autoitscript.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Threading;

namespace FileEncoding.Helper
{
    public class TextEncodingDetect
    {
        private readonly byte[] _utf16BeBom = { 0xFE, 0xFF };
        private readonly byte[] _utf16LeBom = { 0xFF, 0xFE };
        private readonly byte[] _utf8Bom = { 0xEF, 0xBB, 0xBF };

        private bool _nullSuggestsBinary = true;
        private double _utf16ExpectedNullPercent = 70;
        private double _utf16UnexpectedNullPercent = 10;

        public enum Encoding
        {
            /// <summary>Unknown or binary</summary>
            None,
            /// <summary>0-255</summary>
            Ansi,
            /// <summary>0-127</summary>
            Ascii,
            /// <summary>UTF8 with BOM</summary>
            Utf8Bom,
            /// <summary>UTF8 without BOM</summary>
            Utf8Nobom,
            /// <summary>UTF16 LE with BOM</summary>
            Utf16LeBom,
            /// <summary>UTF16 LE without BOM</summary>
            Utf16LeNoBom,
            /// <summary>UTF16 BE with BOM</summary>
            Utf16BeBom,
            /// <summary>UTF16 BE without BOM</summary>
            Utf16BeNoBom
        }

        public bool NullSuggestsBinary { set => _nullSuggestsBinary = value; }

        public double Utf16ExpectedNullPercent
        {
            set { if (value > 0 && value < 100) _utf16ExpectedNullPercent = value; }
        }

        public double Utf16UnexpectedNullPercent
        {
            set { if (value > 0 && value < 100) _utf16UnexpectedNullPercent = value; }
        }

        public static int GetBomLengthFromEncodingMode(Encoding encoding)
        {
            switch (encoding)
            {
                case Encoding.Utf16BeBom:
                case Encoding.Utf16LeBom:
                    return 2;
                case Encoding.Utf8Bom:
                    return 3;
                default:
                    return 0;
            }
        }

        public Encoding CheckBom(byte[] buffer, int size)
        {
            if (size >= 2 && buffer[0] == _utf16LeBom[0] && buffer[1] == _utf16LeBom[1])
                return Encoding.Utf16LeBom;
            if (size >= 2 && buffer[0] == _utf16BeBom[0] && buffer[1] == _utf16BeBom[1])
                return Encoding.Utf16BeBom;
            if (size >= 3 && buffer[0] == _utf8Bom[0] && buffer[1] == _utf8Bom[1] && buffer[2] == _utf8Bom[2])
                return Encoding.Utf8Bom;
            return Encoding.None;
        }

        public Encoding DetectEncoding(byte[] buffer, int size)
        {
            var encoding = CheckBom(buffer, size);
            if (encoding != Encoding.None) return encoding;

            encoding = CheckUtf8(buffer, size);
            if (encoding != Encoding.None) return encoding;

            encoding = CheckUtf16NewlineChars(buffer, size);
            if (encoding != Encoding.None) return encoding;

            encoding = CheckUtf16Ascii(buffer, size);
            if (encoding != Encoding.None) return encoding;

            if (!DoesContainNulls(buffer, size))
                return Encoding.Ansi;

            return _nullSuggestsBinary ? Encoding.None : Encoding.Ansi;
        }

        private static Encoding CheckUtf16NewlineChars(byte[] buffer, int size)
        {
            if (size < 2) return Encoding.None;

            size -= 1;
            int leControlChars = 0;
            int beControlChars = 0;
            int pos = 0;

            while (pos < size)
            {
                byte ch1 = buffer[Interlocked.Increment(ref pos) - 1];
                byte ch2 = buffer[Interlocked.Increment(ref pos) - 1];

                if (ch1 == 0)
                {
                    if (ch2 == 0x0A || ch2 == 0x0D)
                        Interlocked.Increment(ref beControlChars);
                }
                else if (ch2 == 0)
                {
                    if (ch1 == 0x0A || ch1 == 0x0D)
                        Interlocked.Increment(ref leControlChars);
                }

                if (leControlChars > 0 && beControlChars > 0)
                    return Encoding.None;
            }

            if (leControlChars > 0) return Encoding.Utf16LeNoBom;
            return beControlChars > 0 ? Encoding.Utf16BeNoBom : Encoding.None;
        }

        private static bool DoesContainNulls(byte[] buffer, int size)
        {
            int pos = 0;
            while (pos < size)
            {
                if (buffer[Interlocked.Increment(ref pos) - 1] == 0)
                    return true;
            }
            return false;
        }

        private Encoding CheckUtf16Ascii(byte[] buffer, int size)
        {
            int numEvenNulls = 0;
            int numOddNulls = 0;

            int pos = 0;
            while (pos < size)
            {
                if (buffer[pos] == 0) numEvenNulls++;
                pos += 2;
            }

            pos = 1;
            while (pos < size)
            {
                if (buffer[pos] == 0) numOddNulls++;
                pos += 2;
            }

            double evenNullThreshold = numEvenNulls * 2.0 / size;
            double oddNullThreshold = numOddNulls * 2.0 / size;
            double expectedNullThreshold = _utf16ExpectedNullPercent / 100.0;
            double unexpectedNullThreshold = _utf16UnexpectedNullPercent / 100.0;

            if (evenNullThreshold < unexpectedNullThreshold && oddNullThreshold > expectedNullThreshold)
                return Encoding.Utf16LeNoBom;
            if (oddNullThreshold < unexpectedNullThreshold && evenNullThreshold > expectedNullThreshold)
                return Encoding.Utf16BeNoBom;

            return Encoding.None;
        }

        private Encoding CheckUtf8(byte[] buffer, int size)
        {
            bool onlySawAsciiRange = true;
            int pos = 0;

            while (pos < size)
            {
                byte ch = buffer[Interlocked.Increment(ref pos) - 1];

                if (ch == 0 && _nullSuggestsBinary)
                    return Encoding.None;

                int moreChars;
                if (ch <= 127)
                    moreChars = 0;
                else if (ch >= 194 && ch <= 223)
                    moreChars = 1;
                else if (ch >= 224 && ch <= 239)
                    moreChars = 2;
                else if (ch >= 240 && ch <= 244)
                    moreChars = 3;
                else
                    return Encoding.None;

                while (moreChars > 0 && pos < size)
                {
                    onlySawAsciiRange = false;
                    ch = buffer[Interlocked.Increment(ref pos) - 1];
                    if (ch < 128 || ch > 191)
                        return Encoding.None;
                    Interlocked.Decrement(ref moreChars);
                }
            }

            return onlySawAsciiRange ? Encoding.Ascii : Encoding.Utf8Nobom;
        }
    }
}