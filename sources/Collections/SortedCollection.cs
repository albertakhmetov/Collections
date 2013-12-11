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
    public class SortedCollection : INotifyCollectionChanged, IEnumerable<int>
    {
        private List<int> _items = new List<int>();

        public void Add(int item)
        {
            var i = 0;

            while (i <= _items.Count)
                if (i < _items.Count && item == _items[i])
                    break;
                else if (i == _items.Count || item < _items[i])
                {
                    _items.Insert(i, item);

                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, i));
                    break;
                }
                else
                    i++;
        }

        public void AddRange(IEnumerable<int> items)
        {
            var i = 0;
            var notifyList = new List<int>();

            var itemsEnumerator = items.GetEnumerator();
            itemsEnumerator.MoveNext();

            while (i <= _items.Count)
            {
                if (i == _items.Count || itemsEnumerator.Current < _items[i])
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

        public int Count
        {
            get { return _items.Count; }
        }

        public int this[int index]
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

        public IEnumerator<int> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
