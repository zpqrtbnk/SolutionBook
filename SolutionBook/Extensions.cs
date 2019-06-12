using System.Windows;
using System.Windows.Media;

namespace SolutionBook
{
    public static class Extensions
    {
        public static T VisualUpwardSearch<T>(this object source)
            where T : DependencyObject
            => (source as DependencyObject).VisualUpwardSearch<T>();

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

                returnVal = tempReturnVal != null
                    ? tempReturnVal
                    : LogicalTreeHelper.GetParent(returnVal);
                //if (tempReturnVal == null)
                //    returnVal = LogicalTreeHelper.GetParent(returnVal);
                //else returnVal = tempReturnVal;
            }

            return returnVal as T;
        }

        public static T FindVisualChild<T>(this DependencyObject obj) 
            where T : DependencyObject
        {
            // see: https://stackoverflow.com/questions/26827916/how-to-get-the-child-control-inside-a-treeviewitem

            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T) return (T)child;

                    var childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }

            return null;
        }
    }
}
