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
        private ToolWindowState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowControl"/> class.
        /// </summary>
        public ToolWindowControl(ToolWindowState state)
        {
            InitializeComponent();

            _state = state;

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

            OpenEnabler.Initialize(_state.DTE);
        }

        public void Show()
        {
            var bookItem = Book.SelectedItem as BookItem;
            var treeViewItem = Book.ItemContainerGenerator.ContainerFromItem(bookItem) as TreeViewItem;
            
            if (treeViewItem == null) return;

            treeViewItem.Focus();
            Keyboard.Focus(treeViewItem);
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

            //System.Diagnostics.Debug.WriteLine($"Open solution {bookItem.Header} as {bookItem.Path}");

            if (_state.DTE.Solution.IsOpen)
                return;

            if (File.Exists(bookItem.Path))
                _state.DTE.ExecuteCommand("File.OpenProject", $"\"{bookItem.Path}\"");
        }

        private void Menu_Rename(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            BeginEdit(bookItem);

            //var treeViewItem = Book.ItemContainerGenerator.ContainerFromItem(bookItem) as TreeViewItem;
            //var textBox = treeViewItem.FindVisualChild<TextBox>();
        }

        private void Menu_Refresh(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            System.Diagnostics.Debug.WriteLine($"Refresh {bookItem.Header}");
        }

        //private void Menu_Properties(object sender, RoutedEventArgs e)
        //{
        //    var treeItem = sender as MenuItem;
        //    var bookItem = treeItem.DataContext as BookItem;

        //    System.Diagnostics.Debug.WriteLine($"Open solution {bookItem.Header} properties");
        //}

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

        private void Book_PreviewMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Focus: " + FocusManager.GetFocusedElement(Book));
            //System.Diagnostics.Debug.WriteLine("Focus: " + Keyboard.FocusedElement);

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

        private void Book_PreviewMouseRightDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = e.OriginalSource.VisualUpwardSearch<TreeViewItem>();
            //var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //var textBox = sender as TextBox;
            //var bookItem = textBox.DataContext as BookItem;

            EndEdit();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            //var bookItem = textBox.DataContext as BookItem;

            TreeViewItem treeViewItem = null;

            switch (e.Key)
            {
                case Key.Escape:
                    treeViewItem = textBox.VisualUpwardSearch<TreeViewItem>();
                    EndEdit();
                    break;
                case Key.Return: // also .Enter
                    treeViewItem = textBox.VisualUpwardSearch<TreeViewItem>();
                    EndEdit(textBox.Text);
                    break;
            }

            if (treeViewItem != null)
            {
                e.Handled = true;
                treeViewItem.Focus();
                Keyboard.Focus(treeViewItem);
            }
        }

        private void Book_KeyUp(object sender, KeyEventArgs e)
        {
            // do nothing but somehow, without this, arrows etc don't work in the textbox?

            if (e.Key == Key.F2)
            {
                var treeView = sender as TreeView;
                var bookItem = treeView.SelectedItem as BookItem;

                if (bookItem.Type == BookItemType.Folder || bookItem.Type == BookItemType.Solution)
                    BeginEdit(bookItem);
            }

            //System.Diagnostics.Debug.WriteLine("Key: " + e.Key);
            //System.Diagnostics.Debug.WriteLine("Focus: " + FocusManager.GetFocusedElement(Book));
            //System.Diagnostics.Debug.WriteLine("Focus: " + Keyboard.FocusedElement);
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            textBox.SelectAll();
            textBox.Focus();
        }

        // that one triggers, but is not enough: F2 is not handled by the treeview?!
        // so... what has focus exactly?!
        //private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var textBlock = sender as TextBlock;

        //    textBlock.Focus();
        //}
    }
}
