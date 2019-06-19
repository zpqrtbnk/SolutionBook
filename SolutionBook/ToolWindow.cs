using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Imaging;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SolutionBook
{
    [Guid(WindowGuidString)]
    public class ToolWindow : ToolWindowPane
    {
        public const string WindowGuidString = "db30685e-ef09-41a2-ae16-a33e67f1e802";
        public const string Title = "SolutionBook";

        private const string WindowObjectKind = "{" + WindowGuidString + "}";

        private ToolWindowState _state;
        //private ToolWindowControl _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1"/> class.
        /// </summary>
        public ToolWindow(ToolWindowState state) 
            : base()
        {
            _state = state;

            Caption = "SolutionBook - Initializing";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            var content = new ToolWindowControl(state, this);
            Content = content;

            Task.Run(() => state.GetAll())
                .ContinueWith(x => 
                {
                    content.Populate(x.Result); 
                    Caption = "SolutionBook"; 
                }, TaskScheduler.FromCurrentSynchronizationContext());

            // icon
            BitmapImageMoniker = KnownMonikers.Solution;

            //// focus
            EnvDTE80.Events2 events2 = (EnvDTE80.Events2) state.DTE.Events;
            var wve = events2.WindowVisibilityEvents;
            wve.WindowShowing += (window) => { if (window.ObjectKind.ToLowerInvariant() == WindowObjectKind) content.Show(); };
            //wve.WindowHiding += (window) => { };
        }
    }
}
