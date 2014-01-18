/* Copyright © 2013, Albert Akhmetov (albert.akhmetov@hotmail.com) */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public enum SortDirection
    {
        Asc,
        Desc,
    }

    /// <summary>
    /// Represents a collection of items that are sorted based on the associated <see cref="System.Collections.Generic.Comparer{T}"/> implementation
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    public class SortedCollection<T> : INotifyCollectionChanged, IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedCollection{T}"/> class that is empty
        /// </summary>
        /// <param name="isUnique">Value indicating whether a <see cref="SortedCollection{T}"/> object has only unique elements</param>
        public SortedCollection(bool isUnique)
        {
            _items = new List<T>();
            _isUnique = isUnique;
            _comparer = null;
            _sortDirection = SortDirection.Asc;
        }

        private List<T> _items;
        private readonly bool _isUnique;
        private Comparer<T> _comparer;
        private SortDirection _sortDirection;

        /// <summary>
        /// Gets or sets the <see cref="System.Collections.Generic.Comparer{T}"/> for the <see cref="SortedCollection{T}"/> 
        /// </summary>
        public Comparer<T> Comparer
        {
            get { return _comparer ?? Comparer<T>.Default; }
            set
            {
                if (_comparer != value)
                {
                    _comparer = value;
                    UpdateItems();
                    OnComparerChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SortDirection"/> for the <see cref="SortedCollection{T}"/> 
        /// </summary>
        public SortDirection SortDirection
        {
            get { return _sortDirection; }
            set
            {
                if (SortDirection != value)
                {
                    _sortDirection = value;
                    UpdateItems();
                    OnSortDirectionChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether a <see cref="SortedCollection{T}"/> object has unique elements
        /// </summary>
        public bool IsUnique
        {
            get { return _isUnique; }
        }

        /// <summary>
        /// Adds an object to the <see cref="SortedCollection"/>
        /// </summary>
        /// <param name="item">The object to be added to <see cref="SortedCollection{T}"/></param>
        public void Add(T item)
        {
            var i = 0;

            while (i <= _items.Count)
                if (IsUnique && i < _items.Count && Comparer.Compare(item, _items[i]) == 0)
                    break;
                else if (i == _items.Count || Compare(item, _items[i]))
                {
                    _items.Insert(i, item);

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, i));
                    break;
                }
                else
                    i++;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="SortedCollection{T}"/>.
        /// </summary>
        /// <param name="items">
        /// The collection whose elements should be added to the <see cref="SortedCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                return;

            var itemsEnumerator = PrepareItems(items).GetEnumerator();
            if (!itemsEnumerator.MoveNext())
                return;

            var i = 0;
            var notifyList = new List<T>();

            while (i <= _items.Count)
            {
                if (IsUnique && i < _items.Count && Comparer.Compare(itemsEnumerator.Current, _items[i]) == 0)
                {
                    if (!itemsEnumerator.MoveNext())
                        break;
                    else
                        continue;
                }
                else if (i == _items.Count || Compare(itemsEnumerator.Current, _items[i]))
                {
                    _items.Insert(i, itemsEnumerator.Current);
                    notifyList.Add(itemsEnumerator.Current);

                    if (i == _items.Count || !itemsEnumerator.MoveNext())
                        break;
                }
                else if (notifyList.Count > 0)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, notifyList, i - notifyList.Count));
                    notifyList.Clear();
                }

                i++;
            }
            if (notifyList.Count > 0)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, notifyList, i + 1 - notifyList.Count));
        }

        public void Remove(T item)
        {
            if (_items.Remove(item))
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Count)
                Remove(_items[index]);
        }

        public void Clear()
        {
            _items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private bool Compare(T x, T y)
        {
            return Comparer.Compare(x, y) == (SortDirection == Collections.SortDirection.Asc ? -1 : 1);
        }

        private void UpdateItems()
        {
            _items = PrepareItems(_items).ToList();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private IEnumerable<T> PrepareItems(IEnumerable<T> items)
        {
            IEnumerable<T> result = SortDirection == Collections.SortDirection.Asc ? items.OrderBy(i => i, Comparer) : items.OrderByDescending(i => i, Comparer);

            if (IsUnique)
                result = result.Distinct();

            return result;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="SortedCollection{T}"/>
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get { return _items[index]; }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);

            OnCountChanged();
        }

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        private void OnComparerChanged()
        {
            if (ComparerChanged != null)
                ComparerChanged(this, EventArgs.Empty);
        }

        private void OnSortDirectionChanged()
        {
            if (SortDirectionChanged != null)
                SortDirectionChanged(this, EventArgs.Empty);
        }

        private void OnCountChanged()
        {
            if (CountChanged != null)
                CountChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when a <see cref="Comparer"/> changes.
        /// </summary>
        public event EventHandler ComparerChanged;

        /// <summary>
        /// Occurs when a <see cref="SortDirection"/> changes.
        /// </summary>
        public event EventHandler SortDirectionChanged;

        /// <summary>
        /// Occurs when a <see cref="Count"/> changes.
        /// </summary>
        public event EventHandler CountChanged;
    }
}
