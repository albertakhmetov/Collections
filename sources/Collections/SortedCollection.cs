﻿/* Copyright © 2013, Albert Akhmetov (albert.akhmetov@hotmail.com) */

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

    public class SortedCollection<T> : INotifyCollectionChanged, IEnumerable<T>
    {
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

        public bool IsUnique
        {
            get { return _isUnique; }
        }

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

        public int Count
        {
            get { return _items.Count; }
        }

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

        public event EventHandler ComparerChanged;
        public event EventHandler SortDirectionChanged;
        public event EventHandler CountChanged;
    }
}
