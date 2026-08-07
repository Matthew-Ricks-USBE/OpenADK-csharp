using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenADK.Library.Tools;
using OpenADK.Library.Impl;
using OpenADK.Library.Tools.Metadata;

namespace OpenADK.Library
{
    /// <summary>Dependency-injection registrations for OpenADK.</summary>
    public static class AdkServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenAdk(
            this IServiceCollection services,
            Action<AdkOptions> configure = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            AdkOptions options = new();
            configure?.Invoke(options);

            services.AddLogging();
            services.TryAddSingleton(options);
            services.TryAddSingleton<IAdkRuntime, AdkRuntime>();
            services.TryAddSingleton<IAdkComponentFactory, AdkComponentFactory>();
            services.TryAddSingleton<ISifObjectFactory>(
                provider => provider.GetRequiredService<IAdkRuntime>().Objects);
            services.TryAddSingleton<IDtd>(provider =>
            {
                IAdkRuntime runtime = provider.GetRequiredService<IAdkRuntime>();
                if (!runtime.Initialized) runtime.Initialize();
                return runtime.Dtd;
            });
            services.TryAddSingleton<AdkMetadata>();
            services.TryAddSingleton<VariantAssisstant>();
            services.TryAddSingleton<SifParser>();
            return services;
        }
    }
}
