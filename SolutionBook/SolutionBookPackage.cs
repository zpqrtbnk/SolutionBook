using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SolutionBook.Services;
using Task = System.Threading.Tasks.Task;

namespace SolutionBook
{
    /// <summary>
    /// Represents the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindow))]
    public sealed class SolutionBookPackage : AsyncPackage
    {
        /// <summary>
        /// Gets the package Guid string.
        /// </summary>
        public const string PackageGuidString = "408c3e79-bd20-415c-887c-1e69842467a3";

        /// <summary>
        /// Initializes the package.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        /// <remarks>
        /// <para>This method is called right after the package is sited, and is the place where to put
        /// all the initialization code that relies on services provided by Visual Studio.
        /// </para>
        /// </remarks>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ToolWindowCommand.InitializeAsync(this);
        }

        /// <inheritdoc />
        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            return toolWindowType.Equals(Guid.Parse(ToolWindow.WindowGuidString)) ? this : null;
        }

        /// <inheritdoc />
        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            return toolWindowType == typeof(ToolWindow) ? ToolWindow.Title : base.GetToolWindowTitle(toolWindowType, id);
        }

        /// <inheritdoc />
        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken cancellationToken)
        {
            // see https://github.com/microsoft/VSSDK-Extensibility-Samples/tree/master/AsyncToolWindow

            var dataSourceFactory = await GetServiceAsync(typeof(SVsDataSourceFactory)) as IVsDataSourceFactory;
            var dte = await GetServiceAsync(typeof(DTE)) as DTE;

            var items = await SolutionBookSettings.LoadAsync();
            var recents = new RecentSource(dataSourceFactory);
            return new ToolWindowState { RecentSource = recents, ItemSource = new SolutionBookSettings(), Items = items, DTE = dte };
        }
    }
}
