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
            var item = 1;
            var wasInvoke = false;

            var sortedCollection = new SortedCollection();
            sortedCollection.CollectionChanged += (x, y) =>
                {
                    wasInvoke = true;

                    Assert.AreEqual(NotifyCollectionChangedAction.Add, y.Action);
                    Assert.AreEqual(1, y.NewItems.Count);
                    Assert.AreEqual(item, y.NewItems[0]);
                };

            sortedCollection.Add(item);

            Assert.AreEqual(1, sortedCollection.Count); 
            Assert.IsTrue(wasInvoke);           
        }
    }
}
