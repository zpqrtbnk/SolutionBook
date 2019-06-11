using System.Collections.Generic;
using SolutionBook.Services;

namespace SolutionBook
{
    public class ToolWindowState
    {
        public IEnumerable<FileMenuRecents.RecentProject> RecentSolutions { get; set; }
    }
}
