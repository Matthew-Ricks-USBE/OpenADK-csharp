using System;
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
        bool Initialized { get; }

        SifVersion[] SupportedSIFVersions { get; }

        SifVersion SifVersion { get; set; }

        IDtd Dtd { get; }

        ILogger Log { get; }

        ILoggerFactory LoggerFactory { get; }

        ServerLog ServerLog { get; }

        AdkDebugFlags Debug { get; set; }

        Version AdkVersion { get; }

        string[] TransportProtocols { get; }

        void Initialize();

        void Initialize(SIFVariant variant);

        void Initialize(SifVersion version, SIFVariant variant, int sdoLibraries);

        SifVersion GetLatestSupportedVersion(SifVersion[] candidates);

        bool IsSIFVersionSupported(SifVersion version);

        void Install(TransportPlugin transportPlugin);

        TransportPlugin GetTransportProtocol(string protocol);

        string MakeGuid();

        ISifObjectFactory Objects { get; }

        void SetLogFile(string logFilePath);
    }
}
