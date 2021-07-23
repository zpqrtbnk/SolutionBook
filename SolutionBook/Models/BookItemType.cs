// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

namespace SolutionBook.Models
{
    /// <summary>
    /// Defines the book item types.
    /// </summary>
    public enum BookItemType
    {
        /// <summary>
        /// The recent solutions folder.
        /// </summary>
        RecentFolder,

        /// <summary>
        /// A solution within the recent solutions folder.
        /// </summary>
        RecentSolution,

        /// <summary>
        /// A folder.
        /// </summary>
        Folder,

        /// <summary>
        /// A solution.
        /// </summary>
        Solution
    }
}
