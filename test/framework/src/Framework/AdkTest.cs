using OpenADK.Library;
using OpenADK.Library.Impl;
using OpenADK.Library.us;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Library.UnitTesting.Framework
{
    /// <summary>
    /// Summary description for AdkTest.
    /// </summary>
    public class AdkTest : IDisposable
    {
        protected SifVersion fOriginalVersion;

        protected ServiceProvider Services { get; private set; }

        protected IAdkRuntime Runtime { get; private set; }

        protected IAdkComponentFactory Components { get; private set; }

        protected ISifObjectFactory Objects => Runtime?.Objects;


        public AdkTest()
        {
            Services = new ServiceCollection()
                .AddOpenAdk(ConfigureOptions)
                .BuildServiceProvider();

            Runtime = Services.GetRequiredService<IAdkRuntime>();
            Runtime.Initialize();
            Components = Services.GetRequiredService<IAdkComponentFactory>();
            fOriginalVersion = Runtime.SifVersion;
        }


        public virtual void Dispose()
        {
            Services?.Dispose();
            Runtime.SifVersion = fOriginalVersion;
            GC.SuppressFinalize(this);
        }

        protected virtual void ConfigureOptions(AdkOptions options)
        {
            options.SifVersion = SifVersion.LATEST;
            options.Variant = SIFVariant.SIF_US;
            options.SdoLibraries = (int)SdoLibraryType.All;
        }

        protected TestAgent CreateTestAgent()
        {
            return new TestAgent(Runtime, Components);
        }
    }
}
