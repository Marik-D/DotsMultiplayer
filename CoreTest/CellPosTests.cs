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
    }
}