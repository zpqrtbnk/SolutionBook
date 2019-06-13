using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.VisualStudio.Imaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System;
using System.Windows.Media;

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

            //BookItem childItem1 = new BookItem(null) { Header = "Child item #1", Type = BookItemType.Folder };
            //childItem1.Items.Add(new BookItem(childItem1) { Header = "Child item #1.1", Type = BookItemType.Solution, Path = "path1.1" });
            //childItem1.Items.Add(new BookItem(childItem1) { Header = "Child item #1.2", Type = BookItemType.Solution, Path = "path1.2" });
            //Book.Items.Add(childItem1);
            //Book.Items.Add(new BookItem(null) { Header = "Child item #2", Type = BookItemType.Solution, Path = "path2" });

            var recentItems = new BookItem(null) { Header = "Recent", Type = BookItemType.Recents };
            Book.Items.Add(recentItems);

            foreach (var recent in state.RecentSolutions)
            {
                // fixme test availability?
                var solutionName = Path.GetFileNameWithoutExtension(recent.Path);
                recentItems.Items.Add(new BookItem(recentItems) { Header = solutionName, Path = recent.Path, Type = BookItemType.Recent });
            }

            foreach (var element in state.Settings.Elements)
            {
                if (element is SolutionBookSettings.Solution solution)
                {
                    Book.Items.Add(new BookItem(null) { Header = solution.Name, Path = solution.Path, Type = BookItemType.Solution });
                }
                else if (element is SolutionBookSettings.Folder folder)
                {
                    var folderItems = new BookItem(null) { Header = folder.Name, Type = BookItemType.Folder };
                    Book.Items.Add(folderItems);
                    Populate(folderItems, folder.Elements);
                }
            }

            OpenEnabler.Initialize(_state.DTE);
        }

        void Populate(BookItem bookItems, List<SolutionBookSettings.Element> elements)
        {
            foreach (var element in elements)
            {
                if (element is SolutionBookSettings.Solution solution)
                {
                    bookItems.Items.Add(new BookItem(bookItems) { Header = solution.Name, Path = solution.Path, Type = BookItemType.Solution });
                }
                else if (element is SolutionBookSettings.Folder folder)
                {
                    var folderItems = new BookItem(bookItems) { Header = folder.Name, Type = BookItemType.Folder };
                    bookItems.Items.Add(folderItems);
                    Populate(folderItems, folder.Elements);
                }
            }
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

            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".sln",
                Filter = @"Solution (*.sln)|*.sln"
                         /*+ @"|All files (*.*)|*.*"*/,
                AddExtension = true,
                Multiselect = false,
                ValidateNames = true,
                Title = @"Browse for solution..."
            };

            var path = dialog.ShowDialog() == true
                ? dialog.FileName
                : null; 
            
            if (path != null)
                bookItem.Items.Add(new BookItem(bookItem) { Type = BookItemType.Solution, Header = Path.GetFileNameWithoutExtension(path), Path = path });
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

        private Point _lastMouseDown;
        private BookItem _sourceItem, _targetItem;

        private void Book_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                _lastMouseDown = e.GetPosition(Book);
        }

        private void Book_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            // Note: This should be based on some accessibility number and not just 2 pixels 
            const double trigger = 2.0;
            var currentPosition = e.GetPosition(Book);
            var dx = Math.Abs(currentPosition.X - _lastMouseDown.X);
            var dy = Math.Abs(currentPosition.Y - _lastMouseDown.Y);

            if (dx <= trigger || dy <= trigger)
                return;

            // todo: cannot drag 'Recents' folder, anything else is ok
            // todo: cannot drag folder to child?

            var selectedItem = (BookItem) Book.SelectedItem;
            if (selectedItem == null || !CanBeDragged(selectedItem))
                return;

            var container = GetContainerFromItem(selectedItem);
            if (container != null)
            {
                _sourceItem = selectedItem;
                var finalDropEffect = DragDrop.DoDragDrop(container, selectedItem, DragDropEffect);
                DoDrop(finalDropEffect);
                //if ((finalDropEffect == DragDropEffects.Move) && (_targetItem != null))
                //{
                //    // A Move drop was accepted 
                //    selectedItem.Parent.Items.Remove(selectedItem);
                //    _targetItem.Items.Add(selectedItem);
                //    _targetItem = null;
                //    _sourceItem = null;
                //}
            }
        }

        private void DoDrop(DragDropEffects effect)
        {
            if (_targetItem == null)
                return;

            if (effect == DragDropEffects.Move)
                _sourceItem.Parent.Items.Remove(_sourceItem);

            if (effect == DragDropEffects.Move || effect == DragDropEffects.Copy)
            {
                var newType = _sourceItem.Type == BookItemType.Recent ? BookItemType.Solution : _sourceItem.Type;
                var newItem = new BookItem(_targetItem, _sourceItem) { Type = newType };
                _targetItem.Items.Add(newItem);
            }

            _targetItem = null;
            _sourceItem = null;
        }

        private DragDropEffects DragDropEffect
        {
            get
            {
                if (_sourceItem == null)
                    return DragDropEffects.None;

                // recent items are copied, not moved
                return _sourceItem.Type == BookItemType.Recent ? DragDropEffects.Copy : DragDropEffects.Move;
            }
        }

        private void Book_ChkDrop(object sender, DragEventArgs e)
        {
            if (!IsValidDropTarget(e.OriginalSource as UIElement))
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void Book_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            // Verify that this is a valid drop and then store the drop target 
            var container = GetNearestContainer(e.OriginalSource as UIElement);
            if (container == null)
                return;

            var source = (BookItem) e.Data.GetData(typeof(BookItem));
            var target = (BookItem) container.Header;
            if (source == null || target == null)
                return;

            // assume checks have been performed
            // assume source is _sourceItem

            _targetItem = target;
            e.Effects = DragDropEffect;
        }

        private bool IsDescendant(BookItem item, BookItem parent)
        {
            while (item != null)
            {
                if (item.Parent == parent) return true;
                item = item.Parent;
            }
            return false;
        }

        private bool CanBeDragged(BookItem bookItem)
        {
            if (bookItem == null)
                return false;

            // that one does not move
            return bookItem.Type != BookItemType.Recents;
        }

        private bool IsValidDropTarget(UIElement target)
        {
            if (target == null) return false;

            var targetTreeViewItem = GetNearestContainer(target);
            if (targetTreeViewItem == null) return false;

            var targetBookItem = targetTreeViewItem.Header as BookItem;
            if (targetBookItem == null) return false;

            return IsValidDropTarget(targetBookItem, _sourceItem);
        }

        private bool IsValidDropTarget(BookItem target, BookItem dragging)
        {            
            // cannot drop onto recents
            if (target.Type == BookItemType.Recents)
                return false;

            // drop on folder only, for now
            if (target.Type != BookItemType.Folder)
                return false;

            // has to be ok folder
            if (target == dragging || target == dragging.Parent)
                return false;

            if (IsDescendant(target, dragging))
                return false;

            return true;
        }

        private TreeViewItem GetContainerFromItem(BookItem item)
        {
            var _stack = new Stack<BookItem>();
            _stack.Push(item);
            var parent = item.Parent;

            while (parent != null)
            {
                _stack.Push(parent);
                parent = parent.Parent;
            }

            ItemsControl container = Book;
            while ((_stack.Count > 0) && (container != null))
            {
                BookItem top = _stack.Pop();
                container = (ItemsControl)container.ItemContainerGenerator.ContainerFromItem(top);
            }

            return container as TreeViewItem;
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item. 
            var container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }

            return container;
        }
    }
}
