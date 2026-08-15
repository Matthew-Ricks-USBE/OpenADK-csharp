using System;
using System.Net.Http;
using OpenADK.Library.Impl;
using OpenADK.Library.Log;
using Microsoft.Extensions.Logging;

namespace OpenADK.Library
{
    /// <summary>
    /// Instance-scoped ADK services and configuration. Applications should depend on
    /// this interface rather than process-wide state.
    /// </summary>
    public interface IAdkRuntime
    {
        /// <summary/>
        bool Initialized { get; }

        /// <summary/>
        SifVersion[] SupportedSIFVersions { get; }

        /// <summary/>
        SifVersion SifVersion { get; set; }

        /// <summary/>
        IDtd Dtd { get; }

        /// <summary/>
        ILogger Log { get; }

        /// <summary/>
        ILoggerFactory LoggerFactory { get; }

        /// <summary/>
        ServerLog ServerLog { get; }

        /// <summary/>
        AdkDebugFlags Debug { get; set; }

        /// <summary/>
        Version AdkVersion { get; }

        /// <summary/>
        string[] TransportProtocols { get; }

        /// <summary/>
        IHttpClientFactory HttpClientFactory { get; }

        /// <summary/>
        void Initialize();

        /// <summary/>
        void Initialize(SIFVariant variant);

        /// <summary/>
        void Initialize(SifVersion version, SIFVariant variant, int sdoLibraries);

        /// <summary/>
        SifVersion GetLatestSupportedVersion(SifVersion[] candidates);

        /// <summary/>
        bool IsSIFVersionSupported(SifVersion version);

        /// <summary/>
        void Install(TransportPlugin transportPlugin);

        /// <summary/>
        TransportPlugin GetTransportProtocol(string protocol);

        /// <summary/>
        string MakeGuid();

        /// <summary/>
        ISifObjectFactory Objects { get; }

        /// <summary/>
        void SetLogFile(string logFilePath);
    }
}
