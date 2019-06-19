using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Imaging;
using System.Collections.Generic;
using Microsoft.Win32;
using System;
using System.Windows.Media;
using System.Windows.Documents;
using SolutionBook.Services;
using System.Threading.Tasks;

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
        public ToolWindowControl(ToolWindowState state, ToolWindow window)
        {
            InitializeComponent();

            _state = state;
            Solutions.Initialize(_state.DTE);
        }

        public void Populate(IEnumerable<BookItem> items)
        {
            foreach (var item in items)
                Book.Items.Add(item);

            LoadingPanel.Visibility = Visibility.Hidden;
            ToolPanel.Visibility = Visibility.Visible;
        }

        private void PopulateRecents(IEnumerable<RecentSource.Recent> recents)
        {
            var items = Book.Items[0] as BookItem;
            items.Items.Clear();

            foreach (var item in recents)
            {
                // fixme test availability?
                var solutionName = Path.GetFileNameWithoutExtension(item.Path);
                items.Items.Add(new BookItem(items, BookItemType.Recent, item.Path) { Header = solutionName });
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

            if (!string.IsNullOrWhiteSpace(value))
                Save();
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

            PopulateRecents(_state.RecentSource.GetRecents());
        }

        //private void Menu_Properties(object sender, RoutedEventArgs e)
        //{
        //    var treeItem = sender as MenuItem;
        //    var bookItem = treeItem.DataContext as BookItem;

        //    System.Diagnostics.Debug.WriteLine($"Open solution {bookItem.Header} properties");
        //}

        private void Save()
        {
            // FIXME: might want some delay, not Wait, async, locking, queing, etc etc etc
            _state.ItemSource.SaveAsync(Book.Items.Cast<BookItem>().Skip(1)).Wait();
        }

        private void Menu_AddFolder(object sender, RoutedEventArgs e)
        {
            var treeItem = sender as MenuItem;
            var bookItem = treeItem.DataContext as BookItem;

            bookItem.Items.Add(new BookItem(bookItem, BookItemType.Folder) { Header = "(new folder)" });
            Save();
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
            {
                bookItem.Items.Add(new BookItem(bookItem, BookItemType.Solution, path) { Header = Path.GetFileNameWithoutExtension(path) });
                Save();
            }
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

            Save();
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

            Save();
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

            Solutions.Open(bookItem.Path);
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
        private int _targetRelative;

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
                ClearAdorner();
                DoDrop(finalDropEffect);
            }
        }

        private void DoDrop(DragDropEffects effect)
        {
            if (_targetItem == null)
                return;

            if (effect == DragDropEffects.Move)
            {
                if (_sourceItem.Parent == null)
                    Book.Items.Remove(_sourceItem);
                else
                    _sourceItem.Parent.Items.Remove(_sourceItem);
            }

            int index = _targetItem.Parent == null
                ? Book.Items.IndexOf(_targetItem)
                : _targetItem.Parent.Items.IndexOf(_targetItem);

            if (effect == DragDropEffects.Move || effect == DragDropEffects.Copy)
            {
                var newType = _sourceItem.Type == BookItemType.Recent ? BookItemType.Solution : _sourceItem.Type;
                var newItem = _sourceItem.Clone(_targetItem, newType);

                if (_targetRelative == 0)
                {
                    _targetItem.Items.Add(newItem);
                }
                else
                {
                    if (_targetRelative > 0) index += 1;
                    if (_targetItem.Parent == null)
                        Book.Items.Insert(index, newItem);
                    else
                        _targetItem.Parent.Items.Insert(index, newItem);
                }
            }

            _targetItem = null;
            _targetRelative = 0;
            _sourceItem = null;

            Save();
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

        private AdornerLayer _adornerLayer;
        private DragDropAdorner _adorner;
        private TreeViewItem _adorned;

        private void ClearAdorner()
        {
            if (_adorner != null)
            {
                _adornerLayer.Remove(_adorner);
                _adornerLayer = null;
                _adorner = null;
            }

            if (_adorned != null)
            {
                _adorned.Background = DragDropAdorner.Transparent;
                _adorned = null;
            }
        }

        private int GetRelative(TreeViewItem treeViewItem, DragEventArgs e)
        {
            var pos = e.GetPosition(treeViewItem).Y;
            var presenter = GetPresenter(treeViewItem);
            //var height = treeViewItem.ActualHeight;
            var height = presenter.ActualHeight;

            var targetBookItem = treeViewItem.Header as BookItem;
            if (targetBookItem == null) return 0;

            var a = 0;
            switch (targetBookItem.Type)
            {
                case BookItemType.Folder:
                    if (pos < height / 3) a = -1;
                    if (pos >= 2 * height / 3) a = +1;
                    break;
                case BookItemType.Solution:
                    if (pos < height / 2) a = -1;
                    if (pos >= height / 2) a = +1;
                    break;

                    // cannot drop on Recents nor Recent
            }

            return a;
        }

        private void Book_ChkDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            var target = e.OriginalSource as UIElement;
            var targetTreeViewItem = target == null ? null : GetNearestContainer(target);
            var targetBookItem = targetTreeViewItem == null ? null : targetTreeViewItem.Header as BookItem;

            //var tvi2 = target == null ? null : target.VisualUpwardSearch<TreeViewItem>();
            var relative = targetTreeViewItem == null ? 0 : GetRelative(targetTreeViewItem, e);

            if (targetBookItem == null || targetBookItem == _sourceItem || !IsValidDropTarget(targetBookItem, relative))
            {
                //System.Diagnostics.Debug.WriteLine("XX: " + e.OriginalSource + " " + tvi2);
                e.Effects = DragDropEffects.None;
                ClearAdorner();
                return;
            }

            //System.Diagnostics.Debug.WriteLine("AA: " + e.OriginalSource + " " + pos + " " + height + " " + a);

            if (targetTreeViewItem == _adorned && _adorner != null)
            { 
                if (relative == 0)
                {
                    ClearAdorner();
                    targetTreeViewItem.Background = DragDropAdorner.Brush;
                }
                else
                {
                    _adorner.UpdatePosition(relative, GetWidth(targetBookItem, targetTreeViewItem, relative));
                    _adornerLayer.Update();
                }
            }
            else
            {
                ClearAdorner();
                _adorned = targetTreeViewItem;

                if (relative != 0)
                {
                    _adornerLayer = AdornerLayer.GetAdornerLayer(_adorned);
                    _adorner = new DragDropAdorner(_adorned, relative, GetWidth(targetBookItem, targetTreeViewItem, relative));
                    _adornerLayer.Add(_adorner);
                }
                else
                {
                    targetTreeViewItem.Background = DragDropAdorner.Brush;
                }
            }
        }

        private double GetWidth(BookItem target, TreeViewItem targetViewItem, int relative)
        {
            var index = target.Parent == null 
                ? Book.Items.IndexOf(target)
                : target.Parent.Items.IndexOf(target);

            if (index < 0)
                throw new Exception("panic");

            index += relative;
            if (index < 0) index++;
            var count = target.Parent == null ? Book.Items.Count : target.Parent.Items.Count;
            if (index == count) index--;

            if (index < 0)
                throw new Exception("panic");

            var other = target.Parent == null
                ? Book.Items[index] as BookItem
                : target.Parent.Items[index];

            //Debug.WriteLine("W: " + target.Header + " - " + other.Header);

            var targetPresenter = GetPresenter(targetViewItem);
            var otherViewItem = GetContainerFromItem(other) as TreeViewItem;
            var otherPresenter = GetPresenter(otherViewItem);

            var offset = targetPresenter.TranslatePoint(new Point(0, 0), targetViewItem).X;

            return offset + Math.Max(otherPresenter.DesiredSize.Width, targetPresenter.DesiredSize.Width);
        }

        private ContentPresenter GetPresenter(TreeViewItem treeViewItem)
        {
            var grid = VisualTreeHelper.GetChild(treeViewItem, 0);
            var border = VisualTreeHelper.GetChild(grid, 1);
            var contentPresenter = VisualTreeHelper.GetChild(border, 0);

            return contentPresenter as ContentPresenter;
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

            _targetRelative = GetRelative(container, e);
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

        //private bool IsValidDropTarget(UIElement target)
        //{
        //    if (target == null) return false;

        //    var targetTreeViewItem = GetNearestContainer(target);
        //    if (targetTreeViewItem == null) return false;

        //    var targetBookItem = targetTreeViewItem.Header as BookItem;
        //    if (targetBookItem == null) return false;

        //    return IsValidDropTarget(targetBookItem, _sourceItem);
        //}

        private bool IsValidDropTarget(BookItem target, int relative/*, BookItem dragging*/)
        {
            var dragging = _sourceItem;

            switch (target.Type)
            {
                // cannot drop onto recents
                case BookItemType.Recents:
                case BookItemType.Recent:
                    return false;

                case BookItemType.Solution:
                    // always, whatever the value of relative
                    return target != dragging && !IsDescendant(target, dragging) && !IsCloseSibling(target, dragging, relative);

                case BookItemType.Folder:
                    if (target == dragging || IsDescendant(target, dragging)) return false;
                    if (target == dragging.Parent) return relative < 0; // not ON parent but before
                    return true;
                    //return target != dragging && target != dragging.Parent && !IsDescendant(target, dragging);
            }

            return false;
        }

        private bool IsCloseSibling(BookItem target, BookItem dragging, int relative)
        {
            if (relative == 0) return false;
            if (target.Parent != dragging.Parent) return false;

            var targetIndex = GetItemIndex(target);
            var draggingIndex = GetItemIndex(dragging);
            return draggingIndex == targetIndex + relative;
        }

        private int GetItemIndex(BookItem item)
        {
            return item.Parent == null ? Book.Items.IndexOf(item) : item.Parent.Items.IndexOf(item);
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

        private void ContentPresenter_MouseEnter(object sender, MouseEventArgs e)
        {
            // disabled for now

            //var presenter = sender as ContentPresenter;
            //var bookItem = presenter.Content as BookItem;

            //switch (bookItem.Type)
            //{
            //    case BookItemType.Folder:
            //    case BookItemType.Recents:
            //        Infos.Text = "";
            //        break;
            //    case BookItemType.Solution:
            //    case BookItemType.Recent:
            //        Infos.Text = bookItem.Path;
            //        break;
            //}
        }

        private void ContentPresenter_MouseLeave(object sender, MouseEventArgs e)
        {
            // disabled for now

            Infos.Text = "";
        }

        private void AddRootFolder_Click(object sender, RoutedEventArgs e)
        {
            Book.Items.Add(new BookItem(null, BookItemType.Folder) { Header = "(new folder)" });
            Save();
        }

        private void AddRootSolution_Click(object sender, RoutedEventArgs e)
        {
            // fixme DRY

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
            {
                Book.Items.Add(new BookItem(null, BookItemType.Solution, path) { Header = Path.GetFileNameWithoutExtension(path) });
                Save();
            }
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
