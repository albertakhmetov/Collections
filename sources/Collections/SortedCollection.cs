/* Copyright © 2013, Albert Akhmetov (albert.akhmetov@hotmail.com) */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    public class SortedCollection : INotifyCollectionChanged
    {
        public void Add(int item)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public int Count
        {
            get { return 1; }             
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
