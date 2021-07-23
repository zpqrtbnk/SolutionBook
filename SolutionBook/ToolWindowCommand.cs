// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace SolutionBook
{
    internal sealed class ToolWindowCommand
    {
        private readonly AsyncPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file).
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        public ToolWindowCommand(AsyncPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
        }

        public void RegisterWith(OleMenuCommandService commandService)
        {
            var menuCommandId = new CommandID(Constants.CommandSetGuid, Constants.CommandId);
            var menuItem = new MenuCommand((s, e) => Execute(), menuCommandId);

            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        private void Execute()
        {
            // first one fires the task in
            // second one synchronously blocks the calling thread

            //var _ = _package.JoinableTaskFactory.RunAsync(async () =>
            _package.JoinableTaskFactory.Run(async () =>
            {
                var window = await _package.ShowToolWindowAsync(typeof(ToolWindow), 0, true, _package.DisposalToken);
                if (window?.Frame == null) throw new NotSupportedException("Cannot create tool window");
            });
        }
    }
}
