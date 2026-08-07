using OpenADK.Library;
using OpenADK.Library.Impl;
using NUnit.Framework;
using OpenADK.Library.us;
using Microsoft.Extensions.DependencyInjection;
//import com.OpenADK.Library.ADK;
//import com.OpenADK.Library.impl.TransportPlugin;

namespace Library.UnitTesting.Framework
{
    public class InMemoryProtocolTest
    {
        protected Agent fAgent;
        protected TestZoneImpl fZone;
        protected SifVersion fOriginalVersion;
        protected ServiceProvider fServices;
        protected IAdkRuntime Runtime { get; private set; }

        protected const string TEST_URL = "http://localhost:7003?%20%34%"; 

        [SetUp]
        public virtual void Setup()
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
            //uses transportplugin interface , and factory method Createthat
            //returns new instance of class we're looking for 
            TransportPlugin tp = new InMemoryTransportPlugin();
            Runtime.Install( tp );
            fAgent = new TestAgent(Runtime,
                fServices.GetRequiredService<IAdkComponentFactory>());
            fAgent.Initialize();
            fAgent.Properties.TransportProtocol = tp.Protocol;

            //createzone added To ZoneFactoryImpl
            fZone =  (TestZoneImpl)fAgent.ZoneFactory.GetInstance( "test", TEST_URL );
        } //end method Setup


        [TearDown]
        public virtual void TearDown()
        {
            fAgent?.Shutdown();
            fServices?.Dispose();
        }


        public Agent Agent
        {
            get { return fAgent; }
        }

        public TestZoneImpl Zone
        {
            get { return fZone; }
        }
    } //end class
} //end namespace
