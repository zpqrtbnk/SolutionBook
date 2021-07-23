// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;

namespace SolutionBook.Services
{
    public class RecentSource
    {
        private static readonly Guid RecentSourceId = Guid.Parse("9099ad98-3136-4aca-a9ac-7eeeaee51dca");

        private readonly IVsDataSourceFactory _dataSourceFactory;
        private readonly Types _types;

        private RecentSource()
        {
            _types = new Types();
        }

        internal RecentSource(IVsDataSourceFactory dataSourceFactory)
            : this()
        {
            _dataSourceFactory = dataSourceFactory;
        }

        public IEnumerable<Recent> GetRecents() => GetRecents(_dataSourceFactory);

        private IEnumerable<Recent> GetRecents(IVsDataSourceFactory factory)
        {
            // this can give... strange results including duplicates
            // not exactly what the menu gives - how come?!
            //
            // see also ~/AppData/Local/Microsoft/VisualStudio/.../ApplicationPrivateSettings.xml
            // which 'seems' to contain some recent stuff (and better than using the bin thing there?

            //ThreadHelper.ThrowIfNotOnUIThread();

            factory.GetDataSource(RecentSourceId, (uint) Kind.Project, out var dataSource);

            var recents = GetRecents(dataSource, Kind.Project);
            if (recents.Count == 0) return Enumerable.Empty<Recent>();

            return recents.Select(x => new Recent { Path = GetPath(x) }).ToList();
        }

        public class Recent
        {
            public string Path { get; set;}
        }

        private IList GetRecents(IVsUIDataSource dataSource, Kind kind) =>
            _types
                .GetItemsProp(kind)
                .GetValue(dataSource, index: null) as IList;

        private string GetPath(object recent) =>
            _types
                .GetPathProp()
                .GetValue(recent, index: null) as string;
    }
}

