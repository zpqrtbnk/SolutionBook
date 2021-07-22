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
