// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using EnvDTE;
using SolutionBook.Services;

namespace SolutionBook.Models
{
    public class ToolWindowState
    {
        // ReSharper disable once InconsistentNaming
        public DTE DTE { get; set; }

        public RecentSource RecentSource { get; set; }

        public SolutionBookSettings ItemSource { get; set; }
    }
}
