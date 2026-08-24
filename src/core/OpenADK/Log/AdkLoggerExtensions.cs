using System;
using Microsoft.Extensions.Logging;

namespace OpenADK.Library
{
    /// <summary>Concise logging helpers used by the ADK implementation.</summary>
    public static class AdkLoggerExtensions
    {
        /// <summary/>
        public static void Debug(this ILogger logger, object message) =>
            logger?.LogDebug("{Message}", message);

        /// <summary/>
        public static void Debug(this ILogger logger, object message, Exception exception) =>
            logger?.LogDebug(exception, "{Message}", message);

        /// <summary/>
        public static void DebugFormat(this ILogger logger, string format, params object[] args) =>
            logger?.LogDebug(format, args);

        /// <summary/>
        public static void Info(this ILogger logger, object message) =>
            logger?.LogInformation("{Message}", message);

        /// <summary/>
        public static void Info(this ILogger logger, object message, Exception exception) =>
            logger?.LogInformation(exception, "{Message}", message);

        /// <summary/>
        public static void InfoFormat(this ILogger logger, string format, params object[] args) =>
            logger?.LogInformation(format, args);

        /// <summary/>
        public static void Warn(this ILogger logger, object message) =>
            logger?.LogWarning("{Message}", message);

        /// <summary/>
        public static void Warn(this ILogger logger, object message, Exception exception) =>
            logger?.LogWarning(exception, "{Message}", message);

        /// <summary/>
        public static void WarnFormat(this ILogger logger, string format, params object[] args) =>
            logger?.LogWarning(format, args);

        /// <summary/>
        public static void Error(this ILogger logger, object message) =>
            logger?.LogError("{Message}", message);

        /// <summary/>
        public static void Error(this ILogger logger, object message, Exception exception) =>
            logger?.LogError(exception, "{Message}", message);

        /// <summary/>
        public static void ErrorFormat(this ILogger logger, string format, params object[] args) =>
            logger?.LogError(format, args);

        /// <summary/>
        public static void Fatal(this ILogger logger, object message) =>
            logger?.LogCritical("{Message}", message);

        /// <summary/>
        public static void Fatal(this ILogger logger, object message, Exception exception) =>
            logger?.LogCritical(exception, "{Message}", message);
    }
}
