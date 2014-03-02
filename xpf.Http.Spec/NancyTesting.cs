using Microsoft.Owin.Testing;
using Owin;

namespace xpf.Http.Spec
{
    public class NancyTesting
    {
        public TestServer Server;

        public class Startup
        {
            public void Configuration(IAppBuilder appBuilder)
            {
                appBuilder.UseNancy();
            }
        }

        public void Start()
        {
            Server = TestServer.Create<Startup>();
        }

        // Use ClassCleanup to run code after all tests in a class have run

        public void Stop()
        {
            Server.Dispose();
        }
    }
}