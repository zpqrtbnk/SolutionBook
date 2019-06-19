using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SolutionBook.Services
{
    // inspired: https://github.com/GrzegorzKozub/ClearRecent

    public class RecentSource
    {
        //private readonly IServiceProvider _serviceProvider;
        private readonly IVsDataSourceFactory _dataSourceFactory;
        private readonly Types _types;
        //private readonly Files files;

        private RecentSource()
        {
            _types = new Types();
            //files = new Files();
        }

        internal RecentSource(IVsDataSourceFactory dataSourceFactory)
            : this()
        {
            _dataSourceFactory = dataSourceFactory;
        }

        //internal FileMenuRecents(IServiceProvider serviceProvider)
        //    : this()
        //{
        //    this.serviceProvider = serviceProvider;
        //}

        //internal bool FilesFound() => Found(Kind.File);
        //internal bool ProjectsFound() => Found(Kind.Project);

        //internal void ClearAllFiles() => Clear(Kind.File, _ => true);
        //internal void ClearAllProjects() => Clear(Kind.Project, _ => true);

        public IEnumerable<Recent> GetRecents() => GetRecents(_dataSourceFactory);

        private IEnumerable<Recent> GetRecents(IVsDataSourceFactory factory)
        {
            // this can give... strange results including duplicates
            // not exactly what the menu gives - how come?!
            //
            // see also ~/AppData/Local/Microsoft/VisualStudio/.../ApplicationPrivateSettings.xml
            // which 'seems' to contain some recent stuff (and better than using the bin thing there?


            var kind = Kind.Project;
            factory.GetDataSource(
                new Guid("9099ad98-3136-4aca-a9ac-7eeeaee51dca"),
                (uint)kind,
                out IVsUIDataSource dataSource);

            var recents = GetRecents(dataSource, kind);
            if (recents.Count == 0) return Enumerable.Empty<Recent>();
            var output = new List<Recent>();
            foreach (var recent in recents)
                output.Add(new Recent { Path = GetPath(recent) });
            return output;
        }

        public class Recent
        {
            public string Path { get; set;}
        }

        //internal void ClearMissingFiles() =>
        //    Clear(Kind.File, FileMissing);

        //internal void ClearMissingProjects() =>
        //    Clear(Kind.Project, FileMissing);

        //private bool Found(Kind kind) =>
        //    GetCount(GetDataSource(kind), kind) > 0;

        //private void Clear(Kind kind, Func<string, bool> shouldDelete)
        //{
        //    var dataSource = GetDataSource(kind);
        //    var recents = GetRecents(dataSource, kind);

        //    if (recents.Count == 0) { return; }

        //    var remove = types.GetRemoveItemAtMethod(kind);

        //    for (var i = recents.Count - 1; i > -1; i--)
        //    {
        //        if (shouldDelete(GetPath(recents[i])))
        //        { remove.Invoke(dataSource, new object[] { i }); }
        //    }
        //}

        //private IVsUIDataSource GetDataSource(Kind kind)
        //{
        //    var factory = _serviceProvider.GetService(typeof(SVsDataSourceFactory)) as IVsDataSourceFactory;

        //    factory.GetDataSource(
        //        new Guid("9099ad98-3136-4aca-a9ac-7eeeaee51dca"),
        //        (uint)kind,
        //        out IVsUIDataSource dataSource);

        //    return dataSource;
        //}

        //private int GetCount(IVsUIDataSource dataSource, Kind kind) =>
        //    (int)types
        //        .GetCountProp(kind)
        //        .GetValue(dataSource, index: null);

        private IList GetRecents(IVsUIDataSource dataSource, Kind kind) =>
            _types
                .GetItemsProp(kind)
                .GetValue(dataSource, index: null) as IList;

        private string GetPath(object recent) =>
            _types
                .GetPathProp()
                .GetValue(recent, index: null) as string;

        //private static bool FileMissing(string path) =>
        //    !File.Exists(path) && !Directory.Exists(path);
    }
}

