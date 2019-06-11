using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Imaging;

namespace SolutionBook
{
    public class BookItem : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isEditing;

        public BookItem(BookItem parent)
        {
            Items = new ObservableCollection<BookItem>();
            Parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BookItem Parent { get; }

        public BookItemType Type { get; set; }

        public string Header { get; set; }

        public string Path { get; set; }

        public ObservableCollection<BookItem> Items { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
                PropertyChanged(this, new PropertyChangedEventArgs("Icon"));
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsEditing"));
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