//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using OpenADK.Library;

namespace OpenADK.Web.Http
{
    /// <summary>
    /// Summary description for HttpHost.
    /// </summary>
    public class AdkHttpServer
    {
        private ILogger fLog;
        private string fServerName;
        private ArrayList fBindings = new ArrayList();
        private AdkHttpListener fListener;
        private bool fIsStarted = false;

        /// <summary/>
        protected AdkHttpServer(IAdkRuntime runtime)
        {
            Runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            Type type = this.GetType();
            fServerName = "OpenADK Library ADK(r); Version " +
                          type.Assembly.GetName().Version.ToString();
            // Default the log
            fLog = runtime.LoggerFactory.CreateLogger(this.GetType());
            fListener = new AdkHttpListener(this);
        }

        internal IAdkRuntime Runtime { get; }

        /// <summary/>
        public virtual string Name
        {
            get { return fServerName; }
        }

        /// <summary/>
        public virtual AdkHttpRequestContext CreateContext(AdkHttpConnection connection,
                                                            AdkHttpRequest request,
                                                            AdkHttpResponse response)
        {
            return new AdkHttpRequestContext(connection, request, response, this);
        }

        /// <summary>
        /// Adds a handler for a virtual directory
        /// </summary>
        /// <param name="hostName">The host name to use for this handler ( currently only "" is supported )</param>
        /// <param name="virtualPath">The virtual path to respond to requests on</param>
        /// <param name="contextHandler">The handler that should respond to requests in this path</param>
        /// <param name="force">True if an existing handler should be replaced by this one</param>
        public void AddHandlerContext(string hostName,
                                       string virtualPath,
                                       IAdkHttpHandlerFactory contextHandler,
                                       bool force)
        {
            fListener.AddHandlerContext(hostName, virtualPath, contextHandler, force);
        }

        /// <summary>
        /// Removes a context handler for the specified virtual path
        /// </summary>
        /// <param name="hostname">The host name to use for this handler ( currently only "" is supported )</param>
        /// <param name="virtualPath">The virtual path</param>
        public void RemoveHandlerContext(string hostname,
                                          string virtualPath)
        {
            fListener.RemoveHandlerContext(hostname, virtualPath);
        }

        /// <summary/>
        public virtual void AddListener(AdkSocketBinding binding)
        {
            lock (fBindings.SyncRoot)
            {
                if (GetListener(binding.Port) != null)
                {
                    throw new ArgumentException
                        (string.Format("Port {0} is already in use", binding.Port));
                }

                fListener.Attach(binding);
                fBindings.Add(binding);
            }
        }

        /// <summary/>
        public virtual AdkSocketBinding CreateHttpListener()
        {
            AdkSocketBinding binding =
                new AdkSocketBinding(new AdkDefaultAcceptSocket(), this.Log);
            return binding;
        }

        /// <summary>
        /// Creates an HTTPS socket binding with the specified certificate
        /// </summary>
        /// <param name="certificate">The server certificate</param>
        /// <param name="validator">Optional client certificate validator</param>
        /// <returns></returns>
        public virtual AdkSocketBinding CreateHttpsListener(X509Certificate2 certificate, RemoteCertificateValidationCallback validator = null)
        {
            AdkSSLAcceptSocket socket = new AdkSSLAcceptSocket(certificate, validator);
            AdkSocketBinding binding = new AdkSocketBinding(socket);
            return binding;
        }


        /// <summary/>
        public AdkSocketBinding GetListener(int port)
        {
            lock (fBindings.SyncRoot)
            {
                foreach (AdkSocketBinding binding in fBindings)
                {
                    if (binding.Port == port)
                    {
                        return binding;
                    }
                }
            }
            return null;
        }

        /// <summary/>
        protected void RemoveBinding(int port)
        {
            lock (fBindings.SyncRoot)
            {
                AdkSocketBinding server = GetListener(port);
                if (server != null)
                {
                    server.Stop();
                    fBindings.Remove(server);
                }
            }
        }

        /// <summary/>
        protected AdkHttpListener Listener
        {
            get { return fListener; }
        }


        /// <summary/>
        protected AdkSocketBinding[] GetPortBindings()
        {
            lock (fBindings.SyncRoot)
            {
                AdkSocketBinding[] bindings = new AdkSocketBinding[fBindings.Count];
                fBindings.CopyTo(bindings);
                return bindings;
            }
        }

        /// <summary/>
        protected void StartServer()
        {
            lock (fBindings.SyncRoot)
            {
                if (!IsStarted)
                {
                    foreach (AdkSocketBinding server in fBindings)
                    {
                        try
                        {
                            server.Start();
                        }
                        catch (Exception ex)
                        {
                            this.Error(ex.Message, ex);
                        }
                    }
                    fIsStarted = true;
                }
            }
        }

        /// <summary/>
        public bool IsStarted
        {
            get { return fIsStarted; }
        }

        /// <summary/>
        protected void StopServer(bool clearAllListeners)
        {
            lock (fBindings.SyncRoot)
            {
                if (IsStarted)
                {
                    foreach (AdkSocketBinding server in fBindings)
                    {
                        try
                        {
                            server.Stop();
                        }
                        catch (Exception ex)
                        {
                            this.Error(ex.Message, ex);
                        }
                    }
                    if (clearAllListeners)
                    {
                        fBindings.Clear();
                    }

                    fIsStarted = false;
                }
            }
        }

        /// <summary/>
        public ILogger Log
        {
            get { return fLog; }
            set { fLog = value; }
        }

        /// <summary/>
        public void Error(string message,
                           Exception ex)
        {
            if (fLog != null && fLog.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error))
            {
                fLog.Error(message, ex);
            }
        }
    }
}
