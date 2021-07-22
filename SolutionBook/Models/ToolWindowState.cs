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
