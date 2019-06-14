using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Imaging;
using System.Diagnostics;

namespace SolutionBook
{
    [DebuggerDisplay("{{Type}:{Header}}")]
    public class BookItem : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isEditing;
        private string _header;

        public BookItem(BookItem parent)
        {
            Items = new ObservableCollection<BookItem>();
            Parent = parent;
        }

        public BookItem(BookItem parent, BookItem source)
        {
            Items = source.Items;
            Parent = parent;
            Type = source.Type;
            Header = source.Header;
            Path = source.Path;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BookItem Parent { get; }

        public BookItemType Type { get; set; }

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public string Path { get; set; }

        public ObservableCollection<BookItem> Items { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
                OnPropertyChanged(nameof(Icon));
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }

        public ImageMoniker Icon 
        {
            get
            {
                switch (Type)
                {
                    case BookItemType.Folder:
                    case BookItemType.Recents:
                        return _isExpanded ? KnownMonikers.FolderOpened : KnownMonikers.FolderClosed;
                    case BookItemType.Recent:
                    case BookItemType.Solution:
                        return KnownMonikers.Solution;
                    default:
                        return KnownMonikers.Blank;
                }
            }
        }

        public FontWeight Weight
        {
            get
            {
                return Parent == null ? FontWeights.Bold : FontWeights.Normal;
            }
        }
    }
}