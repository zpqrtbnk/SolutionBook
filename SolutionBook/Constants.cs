using System;

namespace SolutionBook
{
    public static class Constants
    {
        public const string Version = "1.2.3.1"; // also sync with VSIX
        public const string Copyright = "Copyright © Pilotine / Stéphane Gay / ZpqrtBnk 2021";

        public const string PackageGuidString = "408c3e79-bd20-415c-887c-1e69842467a3";
        public const string ProviderMenuResourceId = "Menus.ctmenu";
        public const short ProviderMenuResourceVersion = 1;
        public const string CommandSetGuidString = "68844eaa-105e-49f4-8116-1a114f1dee03";
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSetGuid = Guid.Parse(CommandSetGuidString);
    }
}
