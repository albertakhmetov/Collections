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

            CheckCollectionOrder(sortedCollection);
        }

        [TestMethod]
        public void Add3xTest()
        {
            var sortedCollection = new SortedCollection();

            AddItem(sortedCollection, 5, null, 1);
            AddItem(sortedCollection, 2, 0, 2);
            AddItem(sortedCollection, 3, 1, 3);

            CheckCollectionOrder(sortedCollection);
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

            CheckCollectionOrder(sortedCollection);
        }

        [TestMethod]
        public void AddExistsTest()
        {
            var sortedCollection = new SortedCollection();

            sortedCollection.Add(1);
            sortedCollection.Add(0);

            var wasInvoke = false;
            sortedCollection.CollectionChanged += (x, y) => { wasInvoke = true; };
            sortedCollection.Add(1);

            Assert.IsFalse(wasInvoke);
            Assert.AreEqual(2, sortedCollection.Count);
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

        [TestMethod]
        public void AddRangeTest()
        {
            var items = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var sortedCollection = new SortedCollection();

            var invokeCount = 0;
            sortedCollection.CollectionChanged += (x, y) => {
                invokeCount++;

                Assert.AreEqual(NotifyCollectionChangedAction.Add, y.Action);
                Assert.AreEqual(items.Length, y.NewItems.Count);
                Assert.AreEqual(0, y.NewStartingIndex);
            };

            sortedCollection.AddRange(items);

            Assert.AreEqual(1, invokeCount);
            Assert.AreEqual(items.Length, sortedCollection.Count);
            CheckCollectionOrder(sortedCollection);
        }

        [TestMethod]
        public void AddRangeToNotEmptyTest()
        {
            var items = new int[] { 3, 4, 5, 6, 7, 8, 9 };

            var sortedCollection = new SortedCollection();
            sortedCollection.Add(1);
            sortedCollection.Add(2);

            const int sortedCollectionInitSize = 2;

            var invokeCount = 0;
            sortedCollection.CollectionChanged += (x, y) =>
            {
                invokeCount++;

                Assert.AreEqual(NotifyCollectionChangedAction.Add, y.Action);
                Assert.AreEqual(items.Length, y.NewItems.Count);
                Assert.AreEqual(sortedCollectionInitSize, y.NewStartingIndex);
            };

            sortedCollection.AddRange(items);

            Assert.AreEqual(1, invokeCount);
            Assert.AreEqual(items.Length + sortedCollectionInitSize, sortedCollection.Count);
            CheckCollectionOrder(sortedCollection);
        }

        [TestMethod]
        public void AddRangeToNotEmptyBigTest()
        {
            var items = new int[] { 3, 4, 5, 8, 19 };

            var sortedCollection = new SortedCollection();
            sortedCollection.Add(1);
            sortedCollection.Add(2);
            sortedCollection.Add(6);
            sortedCollection.Add(9);
            sortedCollection.Add(10);

            const int sortedCollectionInitSize = 5;

            var invokeCount = 0;
            var expectedInvokeLen = new int[] { 3, 1, 1 };
            var expectedInvokeIndex = new int[] { 2, 6, 9 };

            sortedCollection.CollectionChanged += (x, y) =>
            {             
                Assert.IsTrue(invokeCount < expectedInvokeLen.Length);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, y.Action);
                Assert.AreEqual(expectedInvokeLen[invokeCount], y.NewItems.Count);
                Assert.AreEqual(expectedInvokeIndex[invokeCount], y.NewStartingIndex); 
                
                invokeCount++;
            };

            sortedCollection.AddRange(items);

            Assert.AreEqual(expectedInvokeLen.Length, invokeCount);
            Assert.AreEqual(items.Length + sortedCollectionInitSize, sortedCollection.Count);

            CheckCollectionOrder(sortedCollection);
        }

        private static void CheckCollectionOrder(SortedCollection sortedCollection)
        {
            var expected = sortedCollection.OrderBy(i => i).ToArray();

            for (var i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], sortedCollection[i]);
        }

        [TestMethod]
        public void AddRangeWithNullItemsTest()
        {
            var sortedCollection = new SortedCollection();
            sortedCollection.AddRange(null);
        }

        [TestMethod]
        public void AddRangeWithEmptyItemsTest()
        {
            var sortedCollection = new SortedCollection();
            sortedCollection.AddRange(new int[0]);
        }
    }
}
