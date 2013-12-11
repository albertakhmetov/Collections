/* Copyright © 2013, Albert Akhmetov (albert.akhmetov@hotmail.com) */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Linq;

namespace Collections
{
    [TestClass]
    public class SortedCollectionTests
    {
        [TestMethod]
        public void AddTest()
        {
            var sortedCollection = new SortedCollection();

            AddItem(sortedCollection, 10, null, 1);
        }

        [TestMethod]
        public void AddTwiceTest()
        {
            var sortedCollection = new SortedCollection();

            AddItem(sortedCollection, 3, null, 1);
            AddItem(sortedCollection, 2, 0, 2);

            var en = sortedCollection.GetEnumerator();

            en.MoveNext();
            Assert.AreEqual(2, en.Current);
            en.MoveNext();
            Assert.AreEqual(3, en.Current);
        }

        [TestMethod]
        public void Add3xTest()
        {
            var sortedCollection = new SortedCollection();

            AddItem(sortedCollection, 5, null, 1);
            AddItem(sortedCollection, 2, 0, 2);
            AddItem(sortedCollection, 3, 1, 3);

            var en = sortedCollection.GetEnumerator();

            en.MoveNext();
            Assert.AreEqual(2, en.Current);
            en.MoveNext();
            Assert.AreEqual(3, en.Current);
            en.MoveNext();
            Assert.AreEqual(5, en.Current);
        }

        [TestMethod]
        public void Add5xTest()
        {
            var sortedCollection = new SortedCollection();

            AddItem(sortedCollection, 5, null, 1);
            AddItem(sortedCollection, 2, 0, 2);
            AddItem(sortedCollection, 4, 1, 3);
            AddItem(sortedCollection, 3, 1, 4);
            AddItem(sortedCollection, 10, 4, 5);

            var en = sortedCollection.GetEnumerator();

            en.MoveNext();
            Assert.AreEqual(2, en.Current);
            en.MoveNext();
            Assert.AreEqual(3, en.Current);
            en.MoveNext();
            Assert.AreEqual(4, en.Current);
            en.MoveNext();
            Assert.AreEqual(5, en.Current);
            en.MoveNext();
            Assert.AreEqual(10, en.Current);
        }


        private static void AddItem(SortedCollection sortedCollection, int item, int? position, int totalCount)
        {
            var wasInvoke = false;

            NotifyCollectionChangedEventHandler eventHandler = delegate(object x, NotifyCollectionChangedEventArgs e)
            {
                wasInvoke = true;

                if (position.HasValue)
                    Assert.AreEqual(position.Value, e.NewStartingIndex);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
                Assert.AreEqual(1, e.NewItems.Count);
                Assert.AreEqual(item, e.NewItems[0]);
            };

            sortedCollection.CollectionChanged += eventHandler;
            sortedCollection.Add(item);

            Assert.AreEqual(totalCount, sortedCollection.Count);
            Assert.IsTrue(wasInvoke);

            sortedCollection.CollectionChanged -= eventHandler;
        }
    }
}
