using System.Linq;
using DotsCore;
using NUnit.Framework;

namespace CoreTest
{
    [TestFixture]
    public class BoardStateTests
    {
        [Test]
        public void Parse()
        {
            var board = TestUtils.ParseBoardState(@"
R . .
. B .
            ");
            
            Assert.AreEqual(2, board.Rows);
            Assert.AreEqual(3, board.Cols);
            
            Assert.True(board.Get(1, 0).IsPlaced);
            Assert.AreEqual(Player.Red, board.Get(1, 0).Player);
            
            Assert.False(board.Get(0, 0).IsPlaced);
            
            Assert.True(board.Get(0, 1).IsPlaced);
            Assert.AreEqual(Player.Blue, board.Get(0, 1).Player);
            
            Assert.AreEqual(0, board.Captures.Count);
        }

        [TestFixture]
        public class FindCycles
        {
            [Test]
            public void Diamond()
            {
                var board = TestUtils.ParseBoardState(@"
. R .
R B .
. R .
                ");
                
                board.PlaceByPlayer(new CellPos(1, 2), Player.Red, false);
                var cycles = board.GetCycles(new CellPos(1, 2), Player.Red);

                Assert.AreEqual(1, cycles.Count());
            }
        }
        
        [TestFixture]
        public class GetPointInside
        {
            [Test]
            public void Diamond()
            {
                var board = TestUtils.ParseBoardState(@"
. R .
R . R
. R .
                ");
                var cycle = new Cycle(TestUtils.ParsePoints(@"
. 2 .
1 . 3
. 0 .
                "));
                cycle.Normalize();

                var inside = board.GetPointInside(cycle);

                Assert.AreEqual(new CellPos(1, 1), inside);
            }
        }

        [TestFixture]
        public class Captures
        {

            [Test]
            public void NonCaptureMove()
            {
                var board = TestUtils.ParseBoardState(@"
R . .
. B .
            ");

                board.PlaceByPlayer(new CellPos(1, 2), Player.Red);

                Assert.AreEqual(0, board.Captures.Count);
            }

            [Test]
            public void DiamondCapture()
            {
                var board = TestUtils.ParseBoardState(@"
. R .
R B .
. R .
                ");

                board.PlaceByPlayer(new CellPos(1, 2), Player.Red);

                Assert.AreEqual(1, board.Captures.Count);
            }
            
            [Test]
            public void EmptyDiamondWithPointOnOutside()
            {
                var board = TestUtils.ParseBoardState(@"
. B .
. R .
R . .
. R .
                ");

                board.PlaceByPlayer(new CellPos(1, 2), Player.Red);

                Assert.AreEqual(0, board.Captures.Count);
            }
            
            [Test]
            public void EmptyDiamondWithPointOnCorner()
            {
                var board = TestUtils.ParseBoardState(@"
B R .
R . .
. R .
                ");

                board.PlaceByPlayer(new CellPos(1, 2), Player.Red);

                Assert.AreEqual(0, board.Captures.Count);
            }
            
            [Test]
            public void WideDiamondCapture()
            {
                var board = TestUtils.ParseBoardState(@"
. R R .
R B . .
. R R .
                ");

                board.PlaceByPlayer(new CellPos(1, 3), Player.Red);

                Assert.AreEqual(1, board.Captures.Count);
            }
        }
    }
}