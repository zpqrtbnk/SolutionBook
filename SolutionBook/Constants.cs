// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
//
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System;

namespace SolutionBook
{
    public static class Constants
    {
        public const string PackageGuidString = "408c3e79-bd20-415c-887c-1e69842467a3";
        public const string ProviderMenuResourceId = "Menus.ctmenu";
        public const short ProviderMenuResourceVersion = 1;
        public const string CommandSetGuidString = "68844eaa-105e-49f4-8116-1a114f1dee03";
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSetGuid = Guid.Parse(CommandSetGuidString);
    }
}
