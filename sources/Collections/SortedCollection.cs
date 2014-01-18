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
    public class SortedCollection<T> : INotifyCollectionChanged, IEnumerable<T>
    {
        public SortedCollection(bool isUnique)
        {
            _items = new List<T>();
            _isUnique = isUnique;
            _comparer = null;
        }

        private List<T> _items;
        private readonly bool _isUnique;
        private Comparer<T> _comparer;

        public Comparer<T> Comparer
        {
            get { return _comparer ?? Comparer<T>.Default; }
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
                else if (i == _items.Count || Comparer.Compare(item, _items[i]) == -1)
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

            var itemsEnumerator = PrepareItemsToAdd(items).GetEnumerator();
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
                else if (i == _items.Count || Comparer.Compare(itemsEnumerator.Current,_items[i]) == -1)
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

        private IEnumerable<T> PrepareItemsToAdd(IEnumerable<T> items)
        {
            if (IsUnique)
                return items.OrderBy(i => i, Comparer).Distinct();
            else
                return items.OrderBy(i => i, Comparer);
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
    }
}
