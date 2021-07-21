﻿using System.Collections.Generic;
using System.IO;
using EnvDTE;
using SolutionBook.Services;

namespace SolutionBook
{
    public class ToolWindowState
    {
        // ReSharper disable once InconsistentNaming
        public DTE DTE { get; set; }

        public RecentSource RecentSource { get; set; }

        public SolutionBookSettings ItemSource { get; set;}

        public IEnumerable<BookItem> GetAll()
        {
            var recentItems = RecentSource.GetRecents();
            var settingItems = ItemSource.Load();
            var root = new List<BookItem>();

            var recentFolder = new BookItem(null, BookItemType.Recents) { Header = "Recent" };
            root.Add(recentFolder);

            foreach (var item in recentItems)
            {
                // fixme test availability?
                var solutionName = Path.GetFileNameWithoutExtension(item.Path);
                recentFolder.Items.Add(new BookItem(recentFolder, BookItemType.Recent, item.Path) { Header = solutionName });
            }

            foreach (var item in settingItems)
                root.Add(item);

            return root;
        }
    }
}
