using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SolutionBook
{
    /// <summary>
    /// Provides Extension Methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Search the visual tree upward for an item of a given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The starting point.</param>
        /// <returns>The item, or <c>null</c>.</returns>
        public static T VisualUpwardSearch<T>(this object source)
            where T : DependencyObject
            => (source as DependencyObject).VisualUpwardSearch<T>();

        /// <summary>
        /// Search the visual tree upward for an item of a given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The starting point.</param>
        /// <returns>The item, or <c>null</c>.</returns>
        public static T VisualUpwardSearch<T>(this DependencyObject source)
            where T : DependencyObject
        {
            // see; https://stackoverflow.com/questions/592373/select-treeview-node-on-right-click-before-displaying-contextmenu

            DependencyObject returnVal = source;

            while (returnVal != null && !(returnVal is T))
            {
                DependencyObject tempReturnVal = null;
                if (returnVal is Visual /*|| returnVal is Visual3D*/)
                    tempReturnVal = VisualTreeHelper.GetParent(returnVal);

                returnVal = tempReturnVal ?? LogicalTreeHelper.GetParent(returnVal);

                //if (tempReturnVal == null)
                //    returnVal = LogicalTreeHelper.GetParent(returnVal);
                //else returnVal = tempReturnVal;
            }

            return returnVal as T;
        }

        /// <summary>
        /// Search the visual tree downward for an item of a give type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="source">The starting point.</param>
        /// <returns>The item, or <c>null</c>.</returns>
        public static T VisualDownwardSearch<T>(this DependencyObject obj) 
            where T : DependencyObject
        {
            // see: https://stackoverflow.com/questions/26827916/how-to-get-the-child-control-inside-a-treeviewitem

            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T childOfTypeT) return childOfTypeT;

                    var childItem = VisualDownwardSearch<T>(child);
                    if (childItem != null) return childItem;
                }
            }

            return null;
        }

        public static IEnumerable<TResult> Select<TResult>(this IList source, Func<object, TResult> selector)
        {
            foreach (var o in source) yield return selector(o);
        }
    }
}
