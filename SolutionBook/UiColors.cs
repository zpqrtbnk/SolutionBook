using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using System.Windows.Media;

namespace SolutionBook
{
    /// <summary>
    /// Defines the UI colors.
    /// </summary>
    public class UiColors
    {
        /// <summary>
        /// Gets the tool background color.
        /// </summary>
        public static ThemeResourceKey ToolWindowBackground => EnvironmentColors.ToolWindowBackgroundBrushKey;

        /// <summary>
        /// Gets the tool text color.
        /// </summary>
        public static ThemeResourceKey ToolWindowText => EnvironmentColors.ToolWindowTextBrushKey;

        /// <summary>
        /// Gets the toobar background color.
        /// </summary>
        public static object ToolBarBackground => VsBrushes.CommandBarGradientKey;

        /// <summary>
        /// Gets the toolbar button hover background color.
        /// </summary>
        public static object ToolBarHover => VsBrushes.CommandBarHoverKey;

        /// <summary>
        /// Gets the drag and drop color.
        /// </summary>
        public static Color DragDropColor => Colors.LightCoral;
    }
}
