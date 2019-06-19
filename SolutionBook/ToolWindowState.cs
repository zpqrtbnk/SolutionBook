using System.Collections.Generic;
using EnvDTE;
using SolutionBook.Services;

namespace SolutionBook
{
    public class ToolWindowState
    {
        public IEnumerable<RecentSource.Recent> Recents => RecentSource.GetRecents();

        public ICollection<BookItem> Items { get; set; }

        public DTE DTE { get; set; }

        public RecentSource RecentSource { get; set; }

        public SolutionBookSettings ItemSource { get; set;}
    }
}
