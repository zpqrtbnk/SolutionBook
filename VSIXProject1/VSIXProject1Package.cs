using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSIXProject1.Services;
using Task = System.Threading.Tasks.Task;

namespace VSIXProject1
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(VSIXProject1Package.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindow1))]
    public sealed class VSIXProject1Package : AsyncPackage
    {
        /// <summary>
        /// VSIXProject1Package GUID string.
        /// </summary>
        public const string PackageGuidString = "408c3e79-bd20-415c-887c-1e69842467a3";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ToolWindow1Command.InitializeAsync(this);
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(Guid.Parse(ToolWindow1.WindowGuidString)) ? this : null;
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            return toolWindowType == typeof(ToolWindow1) ? ToolWindow1.Title : base.GetToolWindowTitle(toolWindowType, id);
        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            // see https://github.com/microsoft/VSSDK-Extensibility-Samples/tree/master/AsyncToolWindow

            //return Task.FromResult((object) new ToolWindowState { Projects = Enumerable.Empty<FileMenuRecents.RecentProject>() });

            //return base.InitializeToolWindowAsync(toolWindowType, id, cancellationToken);
            var dataSourceFactory = await GetServiceAsync(typeof(SVsDataSourceFactory)) as IVsDataSourceFactory;
            //return Enumerable.Empty<FileMenuRecents.RecentProject>();
            var recents = new FileMenuRecents(null);
            return new ToolWindowState { Projects = recents.GetRecentProjects(dataSourceFactory) };
        }

        //protected override WindowPane CreateToolWindow(Type toolWindowType, int id, object context)
        //{
        //    //return base.CreateToolWindow(toolWindowType, id, context);
        //    return new ToolWindow1((IEnumerable<FileMenuRecents.RecentProject>) context);
        //}

        #endregion
    }
}
