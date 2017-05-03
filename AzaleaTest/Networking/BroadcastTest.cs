using System;
using Xunit;
using Azalea.Networking;

namespace AzaleaTest.Networking
{
    public class BroadcastTest
    {
        private Broadcast broadcast;

        public BroadcastTest()
        {
            broadcast = Broadcast.Instance;
        }

        [Fact]
        public void Test() {
            var dummyServer = new ServerDetail("Dummy", "001122AABBCC", "127.0.0.1", 2333);
            broadcast.StartBroadcast(dummyServer);

            var task = broadcast.GetServer();
            task.Wait();
            var receivedServer = task.Result;

            Assert.Equal(dummyServer, receivedServer);
        }
    }
}
