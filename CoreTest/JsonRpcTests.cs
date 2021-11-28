using System;
using System.Threading.Tasks;
using DotsCore;
using NUnit.Framework;

namespace CoreTest
{
    [TestFixture]
    public class JsonRpcTests
    {
        [Test]
        public async Task Success()
        {
            JsonRpc bob = null;
            var alice = new JsonRpc(msg => bob.HandleMessageFromTransport(msg));
            bob = new JsonRpc(msg => alice.HandleMessageFromTransport(msg));

            alice.Handle("Double", (int number) => number * 2);

            int doubled = await bob.Call<int, int>("Double", 6);
            
            Assert.AreEqual(12, doubled);
        }
        
        [Test]
        public void Error()
        {
            JsonRpc bob = null;
            var alice = new JsonRpc(msg => bob.HandleMessageFromTransport(msg));
            bob = new JsonRpc(msg => alice.HandleMessageFromTransport(msg));

            alice.Handle<int, int>("Double", (number) => throw new Exception("Test exception"));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await bob.Call<int, int>("Double", 6);
            });
        }
    }
}