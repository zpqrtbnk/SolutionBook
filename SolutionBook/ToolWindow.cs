// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using SolutionBook.Models;

namespace SolutionBook
{
    [Guid(WindowGuidString)]
    public sealed class ToolWindow : ToolWindowPane
    {
        public const string WindowGuidString = "db30685e-ef09-41a2-ae16-a33e67f1e802";
        public const string Title = "SolutionBook";
        private const string WindowObjectKind = "{" + WindowGuidString + "}";

        public ToolWindow(ToolWindowState state) // requires the UI thread
        {
            ThreadHelper.ThrowIfNotOnUIThread(); // DTE requires the UI thread

            Caption = "SolutionBook";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on the object
            // returned by the Content property.

            var content = new ToolWindowControl(state, this);
            Content = content;

            // icon
            BitmapImageMoniker = KnownMonikers.Solution;

            // focus
            var dteEvents = (EnvDTE80.Events2) state.DTE.Events;
            dteEvents.WindowVisibilityEvents.WindowShowing += window =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                // window here is a DTE thing, not directly our own ToolWindow
                // window.Object *is* our own ToolWindow
                if (window.ObjectKind.ToLowerInvariant() == WindowObjectKind &&
                    window.Object is ToolWindow toolWindow &&
                    toolWindow.Content is ToolWindowControl control)
                {
                    control.Show();
                }
            };
        }
    }
}
