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
        [TestInitialize()]
        public void SetUp()
        {
            _sortedCollection = new SortedCollection(false);
        }

        private SortedCollection _sortedCollection;

        [TestMethod]
        public void AddTest()
        {
            AddItem(_sortedCollection, 10, null, 1);
        }

        [TestMethod]
        public void AddTwiceTest()
        {
            AddItem(_sortedCollection, 3, null, 1);
            AddItem(_sortedCollection, 2, 0, 2);

            CheckCollectionOrder(_sortedCollection);
        }

        [TestMethod]
        public void Add3xTest()
        {
            AddItem(_sortedCollection, 5, null, 1);
            AddItem(_sortedCollection, 2, 0, 2);
            AddItem(_sortedCollection, 3, 1, 3);

            CheckCollectionOrder(_sortedCollection);
        }

        [TestMethod]
        public void Add5xTest()
        {
            AddItem(_sortedCollection, 5, null, 1);
            AddItem(_sortedCollection, 2, 0, 2);
            AddItem(_sortedCollection, 4, 1, 3);
            AddItem(_sortedCollection, 3, 1, 4);
            AddItem(_sortedCollection, 10, 4, 5);

            CheckCollectionOrder(_sortedCollection);
        }

        [TestMethod]
        public void AddExistsTest()
        {
            _sortedCollection.Add(1);
            _sortedCollection.Add(0);

            var wasInvoke = false;
            _sortedCollection.CollectionChanged += (x, y) => { wasInvoke = true; };
            _sortedCollection.Add(1);

            Assert.IsTrue(wasInvoke);
            Assert.AreEqual(3, _sortedCollection.Count);
        }

        [TestMethod]
        public void AddExists_IsUniqueTest()
        {
            _sortedCollection = new SortedCollection(true);
            _sortedCollection.Add(1);
            _sortedCollection.Add(0);

            var wasInvoke = false;
            _sortedCollection.CollectionChanged += (x, y) => { wasInvoke = true; };
            _sortedCollection.Add(1);

            Assert.IsFalse(wasInvoke);
            Assert.AreEqual(2, _sortedCollection.Count);
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

            var invokeCount = 0;
            _sortedCollection.CollectionChanged += (x, y) =>
            {
                invokeCount++;

                Assert.AreEqual(NotifyCollectionChangedAction.Add, y.Action);
                Assert.AreEqual(items.Length, y.NewItems.Count);
                Assert.AreEqual(0, y.NewStartingIndex);
            };

            _sortedCollection.AddRange(items);

            Assert.AreEqual(1, invokeCount);
            Assert.AreEqual(items.Length, _sortedCollection.Count);
            CheckCollectionOrder(_sortedCollection);
        }

        [TestMethod]
        public void AddRangeToNotEmptyTest()
        {
            var items = new int[] { 3, 4, 5, 6, 7, 8, 9 };

            _sortedCollection.Add(1);
            _sortedCollection.Add(2);

            const int sortedCollectionInitSize = 2;

            var invokeCount = 0;
            _sortedCollection.CollectionChanged += (x, y) =>
            {
                invokeCount++;

                Assert.AreEqual(NotifyCollectionChangedAction.Add, y.Action);
                Assert.AreEqual(items.Length, y.NewItems.Count);
                Assert.AreEqual(sortedCollectionInitSize, y.NewStartingIndex);
            };

            _sortedCollection.AddRange(items);

            Assert.AreEqual(1, invokeCount);
            Assert.AreEqual(items.Length + sortedCollectionInitSize, _sortedCollection.Count);
            CheckCollectionOrder(_sortedCollection);
        }

        [TestMethod]
        public void AddRangeToNotEmptyBigTest()
        {
            var items = new int[] { 3, 4, 5, 8, 19 };

            _sortedCollection.Add(1);
            _sortedCollection.Add(2);
            _sortedCollection.Add(6);
            _sortedCollection.Add(9);
            _sortedCollection.Add(10);

            const int sortedCollectionInitSize = 5;

            var invokeCount = 0;
            var expectedInvokeLen = new int[] { 3, 1, 1 };
            var expectedInvokeIndex = new int[] { 2, 6, 9 };

            _sortedCollection.CollectionChanged += (x, y) =>
            {
                Assert.IsTrue(invokeCount < expectedInvokeLen.Length);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, y.Action);
                Assert.AreEqual(expectedInvokeLen[invokeCount], y.NewItems.Count);
                Assert.AreEqual(expectedInvokeIndex[invokeCount], y.NewStartingIndex);

                invokeCount++;
            };

            _sortedCollection.AddRange(items);

            Assert.AreEqual(expectedInvokeLen.Length, invokeCount);
            Assert.AreEqual(items.Length + sortedCollectionInitSize, _sortedCollection.Count);

            CheckCollectionOrder(_sortedCollection);
        }

        [TestMethod]
        public void AddRangeWithUnsortedItemsTest()
        {
            var items = new int[] { 8, 7, 5, 34, 19 };

            _sortedCollection.Add(1);
            _sortedCollection.Add(2);
            _sortedCollection.Add(6);
            _sortedCollection.Add(9);
            _sortedCollection.Add(10);

            _sortedCollection.AddRange(items);

            CheckCollectionOrder(_sortedCollection);
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
            _sortedCollection.AddRange(null);
        }

        [TestMethod]
        public void AddRangeWithEmptyItemsTest()
        {
            _sortedCollection.AddRange(new int[0]);
        }

        [TestMethod]
        public void AddNotUniqueRangeItemsTest()
        {
            var items = new int[] { 1, 3, 4, 5, 5, 4, 6, 8, };

            _sortedCollection.Add(1);
            _sortedCollection.Add(3);
            _sortedCollection.Add(5);
            _sortedCollection.Add(7);

            _sortedCollection.AddRange(items);

            Assert.AreEqual(12, _sortedCollection.Count);
            CheckCollectionOrder(_sortedCollection);
        }
    }
}
