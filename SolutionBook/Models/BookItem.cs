// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace SolutionBook.Models
{
    /// <summary>
    /// Represents a solution book item.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class BookItem : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isEditing;
        private string _header;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookItem"/> class with a parent item.
        /// </summary>
        /// <param name="parent">A parent item, or <c>null</c> for root items.</param>
        /// <param name="type">The book item type.</param>
        /// <param name="path">The path of the solution.</param>
        /// <param name="items">The child items.</param>
        private BookItem(BookItem parent, BookItemType type, string path, ObservableCollection<BookItem> items)
        {
            Items = items;
            Parent = parent;

            Type = type;
            Path = path;

            Exists = File.Exists(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookItem"/> class with a parent item.
        /// </summary>
        /// <param name="parent">A parent item, or <c>null</c> for root items.</param>
        /// <param name="type">The book item type.</param>
        /// <param name="path">The path of the solution.</param>
        public BookItem(BookItem parent, BookItemType type, string path = null)
            : this(parent, type, path, new ObservableCollection<BookItem>())
        { }

        private string DebuggerDisplay
        {
            get
            {
                switch (Type)
                {
                    case BookItemType.RecentFolder:
                        return "{RecentFolder}";
                    case BookItemType.Folder:
                        return "{Folder:" + Header + "}";
                    case BookItemType.RecentSolution:
                        return "{RecentSolution:" + Header + "}";
                    case BookItemType.Solution:
                        return "{Solution:" + Header + "}";
                    default:
                        return "{?}";
                }
            }
        }

        /// <summary>
        /// Clone a book item under a new parent and with a new type.
        /// </summary>
        /// <param name="newParent">The new parent item.</param>
        /// <param name="newType">The new type.</param>
        /// <returns>The cloned book item.</returns>
        /// <remarks>
        /// <para>The cloned item shares its <see cref="Items"/> collection with the source item.</para>
        /// <para>The cloned item is *not* added to the new parent.</para>
        /// </remarks>
        public BookItem Clone(BookItem newParent, BookItemType newType)
            => new BookItem(newParent, newType, Path, Items) { Header = Header };

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Gets the parent of this item.
        /// </summary>
        public BookItem Parent { get; }

        /// <summary>
        /// Gets or sets the type of this item.
        /// </summary>
        public BookItemType Type { get; }

        /// <summary>
        /// Gets or sets the header of this item.
        /// </summary>
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        /// <summary>
        /// Gets or sets the path of this item.
        /// </summary>
        /// <remarks>
        /// <para>Only solutions have paths.</para>
        /// </remarks>
        public string Path { get; }

        // whether the file exists at the time the item was created
        public bool Exists { get; }

        /// <summary>
        /// Gets or sets the collection of child items of this item.
        /// </summary>
        public ObservableCollection<BookItem> Items { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is expanded.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is being edited.
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }

        /// <summary>
        /// Gets the icon image moniker for this item.
        /// </summary>
        public ImageMoniker Icon
        {
            get
            {
                switch (Type)
                {
                    case BookItemType.Folder:
                    case BookItemType.RecentFolder:
                        return _isExpanded ? KnownMonikers.FolderOpened : KnownMonikers.FolderClosed;
                    case BookItemType.RecentSolution:
                    case BookItemType.Solution:
                        return KnownMonikers.Solution;
                    default:
                        return KnownMonikers.Blank;
                }
            }
        }

        /// <summary>
        /// Gets the font weight for this item.
        /// </summary>
        public FontWeight Weight => Parent == null ? FontWeights.Bold : FontWeights.Normal;
    }
}