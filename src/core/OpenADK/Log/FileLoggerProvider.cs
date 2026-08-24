using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace OpenADK.Library.Log
{
    internal sealed class FileLoggerProvider : ILoggerProvider
    {
        private readonly object _sync = new object();
        private readonly StreamWriter _writer;

        public FileLoggerProvider(string path)
        {
            string fullPath = Path.GetFullPath(path);
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _writer = new StreamWriter(fullPath, true) { AutoFlush = true };
        }

        public ILogger CreateLogger(string categoryName) => new FileLogger(this, categoryName);

        public void Dispose()
        {
            lock (_sync)
            {
                _writer.Dispose();
            }
        }

        private void Write(string category, Microsoft.Extensions.Logging.LogLevel level, EventId eventId,
            string message, Exception exception)
        {
            lock (_sync)
            {
                _writer.Write(DateTimeOffset.Now.ToString("O"));
                _writer.Write(' ');
                _writer.Write(level);
                _writer.Write(" [");
                _writer.Write(category);
                _writer.Write("] ");
                _writer.Write(message);
                if (eventId.Id != 0)
                {
                    _writer.Write(" (EventId: ");
                    _writer.Write(eventId.Id);
                    _writer.Write(')');
                }

                _writer.WriteLine();
                if (exception != null)
                {
                    _writer.WriteLine(exception);
                }
            }
        }

        private sealed class FileLogger : ILogger
        {
            private readonly FileLoggerProvider _provider;
            private readonly string _category;

            public FileLogger(FileLoggerProvider provider, string category)
            {
                _provider = provider;
                _category = category;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) =>
                logLevel != Microsoft.Extensions.Logging.LogLevel.None;

            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state,
                Exception exception, Func<TState, Exception, string> formatter)
            {
                if (IsEnabled(logLevel))
                {
                    _provider.Write(_category, logLevel, eventId, formatter(state, exception), exception);
                }
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();
            public void Dispose() { }
        }
    }
}
