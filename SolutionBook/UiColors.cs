using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace SolutionBook
{
    public class UiColors
    {
        public static ThemeResourceKey ToolWindowBackground => EnvironmentColors.ToolWindowBackgroundBrushKey;

        public static ThemeResourceKey ToolWindowText => EnvironmentColors.ToolWindowTextBrushKey;

        public static object ToolBarBackground => VsBrushes.CommandBarGradientKey;

        public static object ToolBarHover => VsBrushes.CommandBarHoverKey;
    }
}
