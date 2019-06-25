using System;
using System.Windows;

namespace SolutionBook
{
    public static class TryCatch
    {
        /// <summary>
        /// Executes an action, catching and reporting exceptions.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void Action(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                var text = $"The SolutionBook exception has thrown an Exception!\n{e.GetType().Name}: {e.Message}\n{e.StackTrace}";
                MessageBox.Show(text, "SolutionBook", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
