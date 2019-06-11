namespace VSIXProject1
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using VSIXProject1.Services;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Imaging;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        public static readonly Microsoft.VisualStudio.Imaging.Interop.ImageMoniker MenuIconMoniker = Microsoft.VisualStudio.Imaging.KnownMonikers.Solution;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control(ToolWindowState state)
        {
            this.InitializeComponent();

            /*
            //System.Windows.Media.ImageSource icon1, icon2;
            System.Windows.Media.Imaging.BitmapSource icon1, icon2;
            icon1 = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("")); // reuse VS icons?!
            icon2 = new System.Windows.Media.Imaging.BitmapImage(new System.Uri("")); // reuse VS icons?!
            var z = new System.Windows.Media.Imaging.CachedBitmap(icon1, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
            */
            //Microsoft.VisualStudio.Imaging.CrispImage crisp = new Microsoft.VisualStudio.Imaging.CrispImage();
            //crisp.Moniker = Microsoft.VisualStudio.Imaging.KnownMonikers.FolderClosed;

            var icon1 = Microsoft.VisualStudio.Imaging.KnownMonikers.FolderClosed;
            var icon2 = Microsoft.VisualStudio.Imaging.KnownMonikers.Solution;

            // FolderClosed
            // FolderOpened
            // Solution
            // SolutionNoColor
            // StatusWarning, StatusWarningNoColor

            // try to hide the root by binding to its items
            // {Binding Items} becomes {Binding Items[0].Items}
            // https://stackoverflow.com/questions/17005910/how-to-hide-treeview-root

            //MenuItem root = new MenuItem() { Title = "Menu" }; // fixme hide that one?!
            MenuItem childItem1 = new MenuItem(null) { Title = "Child item #1", Icon = icon1, Weight = FontWeights.Bold, SolutionType = SolutionType.Folder };
            childItem1.Items.Add(new MenuItem(childItem1) { Title = "Child item #1.1", Icon = icon2, SolutionType = SolutionType.Solution });
            childItem1.Items.Add(new MenuItem(childItem1) { Title = "Child item #1.2", Icon = icon2, SolutionType = SolutionType.Solution });
            solutions.Items.Add(childItem1);
            solutions.Items.Add(new MenuItem(null) { Title = "Child item #2", Icon = icon2, SolutionType = SolutionType.Solution, ContextMenuStyle = solutions.Resources["ContextMenuStyle"] });

            //solutions.Items.Add(root);

            //title.FontWeight = FontWeights.Bold;

            // all this should be async but.. also on UI thread?!
            // or maybe this should be done above and just the items are fed to the control?
            //var fileMenuRecents = new FileMenuRecents(serviceProvider);
            var recentItems = new MenuItem(null) { Title = "Recent", Icon = icon1, Weight = FontWeights.Bold, SolutionType = SolutionType.Recents };
            solutions.Items.Insert(0, recentItems);
            var projects = state.Projects;
            foreach (var recent in projects)
            {
                // fixme test availability?
                var path = System.IO.Path.GetDirectoryName(recent.Path);
                var sol = System.IO.Path.GetFileNameWithoutExtension(recent.Path);
                recentItems.Items.Add(new MenuItem(recentItems) { Title = path + " :: " + sol, Icon = icon2, SolutionType = SolutionType.Recent });
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "ToolWindow1");
        }

        public class MenuItem : System.ComponentModel.INotifyPropertyChanged
        {
            public MenuItem(MenuItem parent)
            {
                this.Items = new ObservableCollection<MenuItem>();
                Parent = parent;
            }

            public MenuItem Parent { get; }

            public string Title { get; set; }

            // fixme could derive from solutiontype
            //public System.Windows.Media.ImageSource Icon { get; set; }
            public Microsoft.VisualStudio.Imaging.Interop.ImageMoniker Icon { get; set; }

            public FontWeight Weight { get; set; } = FontWeights.Normal;

            public SolutionType SolutionType { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }

            //public string ContextMenuStyle => "ContextMenuStyle";
            public object ContextMenuStyle { get;set;}

            public event PropertyChangedEventHandler PropertyChanged;

            public void IconChanged()
            {
                // https://www.c-sharpcorner.com/article/explain-inotifypropertychanged-in-wpf-mvvm/
                PropertyChanged(this, new PropertyChangedEventArgs("Icon"));
            }
        }

        private void solutions_Expanded(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            if (item == null) return;

            var data = item.DataContext as MenuItem;
            data.Icon = Microsoft.VisualStudio.Imaging.KnownMonikers.FolderOpened;
            data.IconChanged();
        }

        private void solutions_Collapsed(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            if (item == null) return;

            var data = item.DataContext as MenuItem;
            data.Icon = Microsoft.VisualStudio.Imaging.KnownMonikers.FolderClosed;
            data.IconChanged();
        }

        private void Menu_Open_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OPEN");
        }

        private void Menu_Bah_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("BAH");
        }

        // context menu via styles = data context works
        // https://stackoverflow.com/questions/13420994/how-to-display-context-menu-for-treeview-item-in-a-hierarchial-data-template-in

        private void Menu_Test_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as System.Windows.Controls.MenuItem;
            var menuItem = control.DataContext as MenuItem;
            var solution = menuItem.Title; // that's what we would open
            System.Diagnostics.Debug.WriteLine("Menu 'Test' on " + solution + "(" + menuItem.SolutionType + ")");
        }

        private void Menu_AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as System.Windows.Controls.MenuItem;
            var menuItem = control.DataContext as MenuItem;
            var solution = menuItem.Title; // that's what we would open
            System.Diagnostics.Debug.WriteLine("Menu 'AddFolder' on " + solution + "(" + menuItem.SolutionType + ")");
            menuItem.Items.Add(new MenuItem(menuItem) { SolutionType=SolutionType.Folder, Title="(new folder)", Icon = Microsoft.VisualStudio.Imaging.KnownMonikers.FolderClosed });
        }

        private void Menu_AddSolution_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as System.Windows.Controls.MenuItem;
            var menuItem = control.DataContext as MenuItem;
            var solution = menuItem.Title; // that's what we would open
            System.Diagnostics.Debug.WriteLine("Menu 'AddSolution' on " + solution + "(" + menuItem.SolutionType + ")");
            menuItem.Items.Add(new MenuItem(menuItem) { SolutionType = SolutionType.Solution, Title = "(new solution)", Icon = Microsoft.VisualStudio.Imaging.KnownMonikers.Solution });
        }

        private void Menu_RemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as System.Windows.Controls.MenuItem;
            var menuItem = control.DataContext as MenuItem;
            var solution = menuItem.Title; // that's what we would open
            System.Diagnostics.Debug.WriteLine("Menu 'RemoveFolder' on " + solution + "(" + menuItem.SolutionType + ")");

            var parentItems = menuItem.Parent;
            if (parentItems == null) return; // ?!
            parentItems.Items.Remove(menuItem);
        }

        private void Menu_RemoveSolution_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as System.Windows.Controls.MenuItem;
            var menuItem = control.DataContext as MenuItem;
            var solution = menuItem.Title; // that's what we would open
            System.Diagnostics.Debug.WriteLine("Menu 'RemoveFolder' on " + solution + "(" + menuItem.SolutionType + ")");

            var parentItems = menuItem.Parent;
            if (parentItems == null) return; // ?!
            parentItems.Items.Remove(menuItem);
        }

        private void Solutions_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var control = sender as TreeViewItem;
            var menuItem = control.DataContext as MenuItem;
            //var menuItem = e.OriginalSource.DataContext 
            System.Diagnostics.Debug.WriteLine("DoubleClick! " + menuItem.Title);
        }

        private void Solutions_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount <= 1)
                return;


            MenuItem item;
            switch (e.OriginalSource)
            {
                case StackPanel panel:
                    item = panel.DataContext as MenuItem;
                    break;
                case TextBlock text:
                    item = ((StackPanel)text.Parent).DataContext as MenuItem;
                    break;
                case CrispImage img:
                    item = ((StackPanel)img.Parent).DataContext as MenuItem;
                    break;
                default:
                    return;
            }

            switch (item.SolutionType)
            {
                case SolutionType.Folder:
                case SolutionType.Recents:
                    return;
                default:
                    e.Handled = true;
                    break;
            }

            System.Diagnostics.Debug.WriteLine("DoubleClick! " + item.Title);
        }

        private void Solutions_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MenuItem item;
            switch (e.OriginalSource)
            {
                case StackPanel panel:
                    item = panel.DataContext as MenuItem;
                    break;
                case TextBlock text:
                    item = ((StackPanel) text.Parent).DataContext as MenuItem;
                    break;
                case CrispImage img:
                    item = ((StackPanel) img.Parent).DataContext as MenuItem;
                    break;
                default:
                    return;
            }

            e.Handled = true;
            System.Diagnostics.Debug.WriteLine("DoubleClick! " + item.Title);
        }

        /*
        // https://stackoverflow.com/questions/1398943/context-menu-for-xaml-treeviewitem-distinguished-by-different-attributes
        // https://stackoverflow.com/questions/592373/select-treeview-node-on-right-click-before-displaying-contextmenu
        private void SolutionTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //TreeViewItem SelectedItem = solutions.SelectedItem as TreeViewItem;
            var item = solutions.SelectedItem as MenuItem;
            switch (item.SolutionType)
            {
                case SolutionType.Solution:
                    solutions.ContextMenu = solutions.Resources["SolutionContext"] as System.Windows.Controls.ContextMenu;
                    break;
                case SolutionType.Folder:
                    solutions.ContextMenu = solutions.Resources["FolderContext"] as System.Windows.Controls.ContextMenu;
                    break;
                case SolutionType.Recent:
                    solutions.ContextMenu = solutions.Resources["RecentContext"] as System.Windows.Controls.ContextMenu;
                    break;
                default:
                    solutions.ContextMenu = null;
                    break;
            }
        }
        */
    }
}