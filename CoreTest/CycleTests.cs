using System;
using System.Collections.Generic;
using DotsCore;
using NUnit.Framework;

namespace CoreTest
{
    [TestFixture]
    public class CycleTests
    {

        [TestFixture]
        public class Normalize
        {
            [Test]
            public void Noop()
            {
                var cycle = new Cycle(TestUtils.ParseGrid(@"
 .  2  . 
 1  .  3 
 .  0  .
                "));

                Console.WriteLine(cycle);

                cycle.Normalize();

                Assert.AreEqual(new CellPos(0, 1), cycle.Points[0]);
                Assert.AreEqual(new CellPos(1, 0), cycle.Points[1]);
                Assert.AreEqual(new CellPos(2, 1), cycle.Points[2]);
                Assert.AreEqual(new CellPos(1, 2), cycle.Points[3]);
            }

            [Test]
            public void Reverse()
            {
                var cycle = new Cycle(TestUtils.ParseGrid(@"
 .  3  . 
 0  .  2 
 .  1  .
                "));

                cycle.Normalize();

                Assert.AreEqual(new CellPos(0, 1), cycle.Points[0]);
                Assert.AreEqual(new CellPos(1, 0), cycle.Points[1]);
                Assert.AreEqual(new CellPos(2, 1), cycle.Points[2]);
                Assert.AreEqual(new CellPos(1, 2), cycle.Points[3]);
            }
        }

        [TestFixture]
        public class IsSelfIntersecting
        {
            [Test]
            public void No()
            {
                var cycle = new Cycle(TestUtils.ParseGrid(@"
 .  2  . 
 1  .  3 
 .  0  .
                "));

                Assert.False(cycle.IsSelfIntersecting());
            }
            
            [Test]
            public void Yes()
            {
                var cycle = new Cycle(TestUtils.ParseGrid(@"
 .  2  3  4  .
 1  .  7  .  5
 .  0  .  6  .
                "));

                Assert.True(cycle.IsSelfIntersecting());
            }
        }
    }
}