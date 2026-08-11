using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenADK.Library.Impl;
using OpenADK.Library.Infra;

namespace OpenADK.Library.Nunit.Core
{
    [TestFixture]
    public class DependencyInjectionTests
    {
        [Test]
        public void ProvidersOwnIndependentRuntimes()
        {
            using ServiceProvider first = BuildProvider(SifVersion.SIF20);
            using ServiceProvider second = BuildProvider(SifVersion.SIF26);

            IAdkRuntime firstRuntime = first.GetRequiredService<IAdkRuntime>();
            IAdkRuntime secondRuntime = second.GetRequiredService<IAdkRuntime>();
            firstRuntime.Initialize();
            secondRuntime.Initialize();

            Assert.That(firstRuntime, Is.Not.SameAs(secondRuntime));
            Assert.That(firstRuntime.SifVersion, Is.EqualTo(SifVersion.SIF20));
            Assert.That(secondRuntime.SifVersion, Is.EqualTo(SifVersion.SIF26));

            firstRuntime.SifVersion = SifVersion.SIF15r1;
            Assert.That(secondRuntime.SifVersion, Is.EqualTo(SifVersion.SIF26));
        }

        [Test]
        public void ObjectFactoryAppliesRuntimeVersion()
        {
            using ServiceProvider services = BuildProvider(SifVersion.SIF20r1);
            ISifObjectFactory objects = services.GetRequiredService<IAdkRuntime>().Objects;

            SIF_Request request = objects.Create<SIF_Request>();
            Query query = objects.CreateQuery(InfraDTD.SIF_ZONESTATUS);

            Assert.That(request.SifVersion, Is.EqualTo(SifVersion.SIF20r1));
            Assert.That(query.EffectiveVersion, Is.EqualTo(SifVersion.SIF20r1));
        }

        [Test]
        public void AgentUsesConfiguredComponentFactoryDelegate()
        {
            StubRequestCache cache = new StubRequestCache();
            using ServiceProvider services = new ServiceCollection()
                .AddOpenAdk(options =>
                {
                    options.SdoLibraries = 0;
                    options.Components.RequestCache = _ => cache;
                })
                .BuildServiceProvider();

            Agent agent = new Agent(
                "test-agent",
                services.GetRequiredService<IAdkRuntime>(),
                services.GetRequiredService<IAdkComponentFactory>());

            Assert.That(agent.Requests, Is.SameAs(cache));
            Assert.That(cache.WasInitialized, Is.True);
        }

        [Test]
        public void RuntimeUsesRegisteredLoggerFactory()
        {
            RecordingLoggerProvider logs = new RecordingLoggerProvider();
            using ServiceProvider services = new ServiceCollection()
                .AddLogging(logging => logging.AddProvider(logs))
                .AddOpenAdk(options => options.SdoLibraries = 0)
                .BuildServiceProvider();

            IAdkRuntime runtime = services.GetRequiredService<IAdkRuntime>();
            runtime.Log.LogInformation("DI logging works");

            Assert.That(logs.Messages, Does.Contain("DI logging works"));
        }

        private static ServiceProvider BuildProvider(SifVersion version)
        {
            return new ServiceCollection()
                .AddOpenAdk(options =>
                {
                    options.SifVersion = version;
                    options.SdoLibraries = 0;
                })
                .BuildServiceProvider();
        }

        private sealed class StubRequestCache : RequestCache
        {
            public bool WasInitialized { get; private set; }

            protected override void Initialize(Agent agent)
            {
                WasInitialized = true;
            }

            public override int ActiveRequestCount => 0;

            public override IRequestInfo StoreRequestInfo(SIF_Request request, Query query, IZone zone) => null;

            public override IRequestInfo GetRequestInfo(string msgId, IZone zone) => null;

            public override IRequestInfo LookupRequestInfo(string msgId, IZone zone) => null;
        }

        private sealed class RecordingLoggerProvider : ILoggerProvider, ILogger
        {
            public IList<string> Messages { get; } = new List<string>();

            public ILogger CreateLogger(string categoryName) => this;

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;

            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId,
                TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Messages.Add(formatter(state, exception));
            }

            public void Dispose() { }
        }
    }
}
