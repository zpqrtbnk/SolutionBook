using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Imaging;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace SolutionBook
{
    [Guid(WindowGuidString)]
    public sealed class ToolWindow : ToolWindowPane
    {
        public const string WindowGuidString = "db30685e-ef09-41a2-ae16-a33e67f1e802";
        public const string Title = "SolutionBook";

        private const string WindowObjectKind = "{" + WindowGuidString + "}";

        //private ToolWindowState _state;

        public ToolWindow(ToolWindowState state)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //_state = state;

            Caption = "SolutionBook - Initializing";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on the object
            // returned by the Content property.

            var content = new ToolWindowControl(state, this);
            Content = content;

            var _ = Task.Run(state.GetAll)
                .ContinueWith(x =>
                {
                    content.Populate(x.Result);
                    Caption = "SolutionBook";
                }, TaskScheduler.FromCurrentSynchronizationContext());

            // icon
            BitmapImageMoniker = KnownMonikers.Solution;

            // focus
            var dteEvents = (EnvDTE80.Events2) state.DTE.Events;
            dteEvents.WindowVisibilityEvents.WindowShowing += window =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (window.ObjectKind.ToLowerInvariant() == WindowObjectKind) content.Show();
            };
        }
    }
}
