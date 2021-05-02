using DotsCore;
using NUnit.Framework;

namespace CoreTest
{
    public class CellPosTests
    {

        [Test]
        public void Coords()
        {
            var pos = new CellPos(1, 2);
            
            Assert.AreEqual(1, pos.Row);
            Assert.AreEqual(2, pos.Col);
        }
        
        [Test]
        public void Cross()
        {
            var top = new CellPos(1, 0);
            var right = new CellPos(0, 1);
            
            Assert.True(CellPos.Cross(top, right) < 0);
            Assert.True(CellPos.Cross(right, top) > 0);
        }
    }
}