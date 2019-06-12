using System.Collections.Generic;
using EnvDTE;
using SolutionBook.Services;

namespace SolutionBook
{
    public class ToolWindowState
    {
        public IEnumerable<FileMenuRecents.RecentProject> RecentSolutions { get; set; }

        public DTE DTE { get; set; }
    }
}
