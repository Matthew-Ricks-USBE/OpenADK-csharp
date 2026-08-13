using System;
using Microsoft.Extensions.DependencyInjection;
using OpenADK.Library;
using OpenADK.Library.Impl;
using OpenADK.Library.us;

namespace Library.UnitTesting.Framework
{
    public class InMemoryProtocolTest : IDisposable
    {
        protected Agent fAgent;
        protected TestZoneImpl fZone;
        protected SifVersion fOriginalVersion;
        protected ServiceProvider fServices;
        protected IAdkRuntime Runtime { get; private set; }

        protected const string TEST_URL = "http://localhost:7003?%20%34%";

        public InMemoryProtocolTest()
        {
            fServices = new ServiceCollection()
                .AddOpenAdk(options =>
                {
                    options.SifVersion = SifVersion.LATEST;
                    options.Variant = SIFVariant.SIF_US;
                    options.SdoLibraries = (int)SdoLibraryType.All;
                })
                .BuildServiceProvider();
            Runtime = fServices.GetRequiredService<IAdkRuntime>();
            Runtime.Initialize();
            fOriginalVersion = Runtime.SifVersion;
            TransportPlugin tp = new InMemoryTransportPlugin();
            Runtime.Install(tp);
            fAgent = new TestAgent(Runtime, fServices.GetRequiredService<IAdkComponentFactory>());
            fAgent.Initialize();
            fAgent.Properties.TransportProtocol = tp.Protocol;
            fZone = (TestZoneImpl)fAgent.ZoneFactory.GetInstance("test", TEST_URL);
        }

        public virtual void Dispose()
        {
            fAgent?.Shutdown();
            fServices?.Dispose();
        }

        public Agent Agent => fAgent;

        public TestZoneImpl Zone => fZone;
    }
}
