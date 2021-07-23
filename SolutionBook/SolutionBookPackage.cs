// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SolutionBook.Models;
using SolutionBook.Services;
using Task = System.Threading.Tasks.Task;

namespace SolutionBook
{
    /// <summary>
    /// Represents the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(Constants.PackageGuidString)]
    [ProvideMenuResource(Constants.ProviderMenuResourceId, Constants.ProviderMenuResourceVersion)]
    [ProvideToolWindow(typeof(ToolWindow))]
    public sealed class SolutionBookPackage : AsyncPackage
    {
        private ToolWindowCommand _toolWindowCommand;

        // see https://github.com/microsoft/VSSDK-Extensibility-Samples/tree/master/AsyncToolWindow

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
            // when initialized asynchronously, the current thread may be a background thread at this point.
            // do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // create the singleton tool window
            _toolWindowCommand = new ToolWindowCommand(this);

            // register the command with the command service
            var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            _toolWindowCommand.RegisterWith(commandService);
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
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread - we're not invoking them here
            var dataSourceFactory = await GetServiceAsync(typeof(SVsDataSourceFactory)) as IVsDataSourceFactory;
            var dte = await GetServiceAsync(typeof(DTE)) as DTE;
#pragma warning restore VSTHRD010

            return new ToolWindowState
            {
                RecentSource = new RecentSource(dataSourceFactory),
                ItemSource = new SolutionBookSettings(),
                DTE = dte
            };
        }
    }
}
