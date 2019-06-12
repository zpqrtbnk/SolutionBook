using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.VisualStudio.Imaging;

namespace SolutionBook
{
    public partial class ToolWindowControl : UserControl
    {
        private string _editOrigin;
        private BookItem _editItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowControl"/> class.
        /// </summary>
        public ToolWindowControl(ToolWindowState state)
        {
            InitializeComponent();

            BookItem childItem1 = new BookItem(null) { Header = "Child item #1", Type = BookItemType.Folder };
            childItem1.Items.Add(new BookItem(childItem1) { Header = "Child item #1.1", Type = BookItemType.Solution, Path = "path1.1" });
            childItem1.Items.Add(new BookItem(childItem1) { Header = "Child item #1.2", Type = BookItemType.Solution, Path = "path1.2" });
            Book.Items.Add(childItem1);
            Book.Items.Add(new BookItem(null) { Header = "Child item #2", Type = BookItemType.Solution, Path = "path2" });


            var recentItems = new BookItem(null) { Header = "Recent", Type = BookItemType.Recents };
            Book.Items.Insert(0, recentItems);

            foreach (var recent in state.RecentSolutions)
            {
                // fixme test availability?
                var solutionName = Path.GetFileNameWithoutExtension(recent.Path);
                recentItems.Items.Add(new BookItem(recentItems) { Header = solutionName, Path = recent.Path, Type = BookItemType.Recent });
            }
        }

        private void BeginEdit(BookItem bookItem)
        {
            _editItem = bookItem;
            _editOrigin = bookItem.Header;
            bookItem.IsEditing = true;
        }

        private void EndEdit(string value = null)
        {
            if (_editItem == null)
                return;

            _editItem.Header = string.IsNullOrWhiteSpace(value) ? _editOrigin : value;

            _editItem.IsEditing = false;
            _editItem = null;
        }

        private void BookItem_Expanded(object sender, RoutedEventArgs e)
        {
            var treeItem = e.OriginalSource as TreeViewItem;
            var bookItem = treeItem.DataContext as BookItem;

            bookItem.IsExpanded = true;
        }

        private void BookItem_Collapsed(object sender, RoutedEventArgs e)
        {
            var treeItem = e.OriginalSource as TreeViewItem;
            var bookItem = treeItem.DataContext as BookItem;

            bookItem.IsExpanded = false;
        }

        private void Menu_Open(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            System.Diagnostics.Debug.WriteLine($"Open solution {bookItem.Header} as {bookItem.Path}");
        }

        private void Menu_Rename(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            // how can I select it?
            //var viewItem = Book.ItemContainerGenerator.ContainerFromItem(treeItem) as TreeViewItem;
            //viewItem.IsSelected = true;

            BeginEdit(bookItem);
        }

        private void Menu_Refresh(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            System.Diagnostics.Debug.WriteLine($"Refresh {bookItem.Header}");
        }

        private void Menu_Properties(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            System.Diagnostics.Debug.WriteLine($"Open solution {bookItem.Header} properties");
        }

        private void Menu_AddFolder(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            bookItem.Items.Add(new BookItem(bookItem) { Type = BookItemType.Folder, Header = "(new folder)" });
        }

        private void Menu_AddSolution(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            bookItem.Items.Add(new BookItem(bookItem) { Type = BookItemType.Solution, Header = "(new solution)" });
        }

        private void Menu_RemoveFolder(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            var parentItem = bookItem.Parent;
            if (parentItem == null)
                Book.Items.Remove(bookItem);
            else
                parentItem.Items.Remove(bookItem);
        }

        private void Menu_RemoveSolution(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            var parentItem = bookItem.Parent;
            if (parentItem == null)
                Book.Items.Remove(bookItem);
            else
                parentItem.Items.Remove(bookItem);
        }

        private void Book_Mouse(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount <= 1)
                return;

            BookItem bookItem;
            switch (e.OriginalSource)
            {
                case StackPanel panel:
                    bookItem = panel.DataContext as BookItem;
                    break;
                case TextBlock text:
                    bookItem = ((StackPanel)text.Parent).DataContext as BookItem;
                    break;
                case CrispImage img:
                    bookItem = ((StackPanel)img.Parent).DataContext as BookItem;
                    break;
                default:
                    return;
            }

            // do nothing, will expand/collapse
            if (bookItem.Type == BookItemType.Folder || bookItem.Type == BookItemType.Recents)
                return;

            // prevent double-click and expand/collapse
            e.Handled = true;

            System.Diagnostics.Debug.WriteLine($"Open solution {bookItem.Header} as {bookItem.Path}");
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            //var bookItem = textBox.DataContext as BookItem;

            //if (bookItem == null) return;

            //textBox.Text = _editOrigin;
            EndEdit();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            var bookItem = textBox.DataContext as BookItem;

            switch (e.Key)
            {
                case Key.Escape:
                    //textBox.Text = _editOrigin;
                    EndEdit();
                    e.Handled = true;
                    break;
                case Key.Return: // also .Enter
                    EndEdit(textBox.Text);
                    e.Handled = true;
                    break;
            }
        }
    }
}
