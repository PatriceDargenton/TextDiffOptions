
namespace TextDiffOptions
{
    internal static class Const
    {

#if DEBUG
        public const bool IsDebug = true;
        public const bool IsRelease = false;
#else
        public const bool IsDebug = false;
        public const bool IsRelease = true;
#endif

        public const string newline = "\n"; // Line Feed
        public const string newlineCRLF = "\r\n"; // Carriage Return + Line Feed: Environment.NewLine

        public const string ExtTxt = ".txt";
        public const string Fusion = "Fusion";
        public const string FilePrefix = "File";
        public const string PagePrefix = "Page";
        public const string OrigSuffix = "Orig";

        public const string TmpFileFilter = FilePrefix + "?_*" + ExtTxt;
        public const string FusionFileFilter = Fusion + "?" + ExtTxt;

        public const string SentenceSeparators = ".:?!;|¡¿";

        // Quote normalization
        public const int AsciiDoubleQuoteCode = 34;               // "
        public const int AsciiLeftGuillemetCode = 171;            // «
        public const int AsciiRightGuillemetCode = 187;           // »
        public const int AsciiLeftDoubleQuotationMarkCode = 147;  // "
        public const int AsciiRightDoubleQuotationMarkCode = 148; // "
        public const int AsciiSingleQuoteCode = 39;               // '
        public const int AsciiSingleQuote2Code = 27;
        public const int AsciiLeftSingleQuotationMarkCode = 145;  // '
        public const int AsciiRightSingleQuotationMarkCode = 146; // '
        public const int AsciiAcuteAccentCode = 180;              // ´

        public const int AsciiNonBreakingSpaceCode = 160;         // &nbsp;
        public const int Utf16NarrowNonBreakingSpaceCode = 8239;  // 0x202F

        public const int AsciiEnDashCode = 150;                   // –

        public const char Char3Dots = '…';
        public const string String3Dots = "…";

        public const int NullStringIndex = -1;
    }
}