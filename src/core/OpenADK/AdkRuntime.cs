using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenADK.Library.Impl;
using OpenADK.Library.Log;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OpenADK.Library
{
    /// <summary>
    /// Default instance-based implementation of the ADK runtime.
    /// </summary>
    public class AdkRuntime : IAdkRuntime
    {
        public const string LogIdentifier = "ADK";

        private static readonly SifVersion[] SupportedVersions = SifVersion.SupportedVersions;

        private readonly object _initializationLock = new object();
        private readonly AdkOptions _options;
        private readonly IDictionary<string, TransportPlugin> _transports =
            new Dictionary<string, TransportPlugin>(StringComparer.OrdinalIgnoreCase);
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _log;
        private readonly ServerLog _serverLog;
        private readonly SifObjectFactory _objects;
        private IDtd _dtd;
        private SifVersion _sifVersion;

        public AdkRuntime(AdkOptions options, ILoggerFactory loggerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _log = _loggerFactory.CreateLogger(LogIdentifier);
            _serverLog = new Library.Log.ServerLog(LogIdentifier, null);
            _objects = new SifObjectFactory(this);
            Debug = options.Debug;
        }

        public bool Initialized { get; private set; }

        public SifVersion[] SupportedSIFVersions => (SifVersion[])SupportedVersions.Clone();

        public virtual SifVersion SifVersion
        {
            get
            {
                CheckInitialized();
                return _sifVersion;
            }
            set
            {
                CheckInitialized();
                if (value == null || !IsSIFVersionSupported(value))
                {
                    throw new AdkNotSupportedException("SIF " + value + " is not supported by the ADK", null);
                }

                _sifVersion = value;
                if (Debug != AdkDebugFlags.None)
                {
                    Log.DebugFormat("Using SIF {0}", _sifVersion);
                }
            }
        }

        public IDtd Dtd
        {
            get
            {
                CheckInitialized();
                return _dtd;
            }
        }

        public ILogger Log => _log;

        public ILoggerFactory LoggerFactory => _loggerFactory;

        public ServerLog ServerLog => _serverLog;

        public AdkDebugFlags Debug { get; set; }

        public Version AdkVersion => typeof(AdkRuntime).Assembly.GetName().Version;

        public ISifObjectFactory Objects => _objects;

        public string[] TransportProtocols
        {
            get
            {
                CheckInitialized();
                return _transports.Values.Where(plugin => !plugin.Internal)
                    .Select(plugin => plugin.Protocol).ToArray();
            }
        }

        public void Initialize()
        {
            Initialize(_options.SifVersion, _options.Variant, _options.SdoLibraries);
        }

        public void Initialize(SIFVariant variant)
        {
            Initialize(_options.SifVersion, variant, _options.SdoLibraries);
        }

        public virtual void Initialize(SifVersion version, SIFVariant variant, int sdoLibraries)
        {
            lock (_initializationLock)
            {
                if (Initialized) return;
                ArgumentNullException.ThrowIfNull(version, nameof(version));

                if (!IsSIFVersionSupported(version))
                {
                    throw new AdkNotSupportedException("SIF " + version + " is not supported by the ADK", null);
                }

                _dtd = _options.DtdFactory(variant) ??
                    throw new AdkException("The configured DTD factory returned null", null);
                if (_dtd is DTDInternals internals)
                {
                    internals.Logger = _loggerFactory.CreateLogger(internals.GetType());
                    internals.LoadLibraries(sdoLibraries);
                }

                _sifVersion = version;
                _serverLog?.AddLogger(new DefaultServerLogModule());

                InstallCoreTransport(new HttpTransportPlugin());
                InstallCoreTransport(new HttpsTransportPlugin());

                if (Debug != AdkDebugFlags.None)
                {
                    _log.Debug("Using Adk " + AdkVersion);
                }

                Initialized = true;
            }
        }

        public SifVersion GetLatestSupportedVersion(SifVersion[] candidates)
        {
            CheckInitialized();
            if (candidates == null || candidates.Length == 0)
            {
                return SifVersion;
            }

            SifVersion latest = null;
            foreach (SifVersion candidate in candidates)
            {
                if (latest == null || candidate.CompareTo(latest) > 0)
                {
                    latest = candidate;
                }
            }

            return latest ?? SifVersion;
        }

        public bool IsSIFVersionSupported(SifVersion version)
        {
            return version != null && Array.BinarySearch(SupportedVersions, version) > -1;
        }

        public void Install(TransportPlugin transportPlugin)
        {
            if (transportPlugin == null)
            {
                throw new ArgumentNullException(nameof(transportPlugin));
            }

            CheckInitialized();
            InstallCoreTransport(transportPlugin);
        }

        public TransportPlugin GetTransportProtocol(string protocol)
        {
            if (protocol == null)
            {
                throw new ArgumentNullException(nameof(protocol));
            }

            CheckInitialized();
            _transports.TryGetValue(protocol, out TransportPlugin plugin);
            return plugin;
        }

        public string MakeGuid()
        {
            return SifFormatter.GuidToSifRefID(Guid.NewGuid());
        }

        public void SetLogFile(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                throw new ArgumentException("A log file path is required.", nameof(logFilePath));
            }

            _loggerFactory.AddProvider(new FileLoggerProvider(logFilePath));
        }

        private void InstallCoreTransport(TransportPlugin transportPlugin)
        {
            _transports[transportPlugin.Protocol] = transportPlugin;
        }

        private void CheckInitialized()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("The ADK runtime is not initialized.");
            }
        }

    }
}
