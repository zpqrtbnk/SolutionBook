using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSIXProject1.Services;

namespace VSIXProject1
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid(WindowGuidString)]
    public class ToolWindow1 : ToolWindowPane
    {
        public const string WindowGuidString = "db30685e-ef09-41a2-ae16-a33e67f1e802";
        public const string Title = "SolutionBook";

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1"/> class.
        /// </summary>
        public ToolWindow1(ToolWindowState state) : base()
        {
            // is this ok?
            Caption = "SolutionBook - Initializing";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = new ToolWindow1Control(state);

            // would need to change after window has been initialized
            Caption = "SolutionBook";

            // icon
            BitmapImageMoniker = Microsoft.VisualStudio.Imaging.KnownMonikers.Solution;
        }
    }

    public class ToolWindowState
    {
        public IEnumerable<FileMenuRecents.RecentProject> Projects { get; set; }
    }
}
