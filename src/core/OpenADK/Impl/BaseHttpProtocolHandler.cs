//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using OpenADK.Library.Infra;
using OpenADK.Util;

namespace OpenADK.Library.Impl
{
    /// <summary>
    /// Summary description for BaseHttpProtocolHandler.
    /// </summary>
    internal abstract class BaseHttpProtocolHandler : IProtocolHandler
    {

        #region Private Members

        private string fHttpUserAgent;
        private bool fKeepAliveOnSend;
        private Uri fZoneUrl;
        private ZoneImpl fZone;
        protected readonly HttpTransport fTransport;

        private IHttpClientFactory _clientFactory;
        private HttpClient _secureClient;
        private readonly ReaderWriterLockSlim _secureClientLock = new ReaderWriterLockSlim();


        protected BaseHttpProtocolHandler(HttpTransport transport)
        {
            fTransport = transport;
        }


        #endregion


        #region IProtocolHandler Members

        public virtual string Name
        {
            get { return fZone.Agent.Id + "@" + fZone.ZoneId + ".HttpProtocolHandler"; }
        }


        /// <summary>  Initialize the protocol handler for a zone</summary>
        public virtual void Open( ZoneImpl zone )
        {
            fZone = zone;

            fKeepAliveOnSend = ((HttpProperties) fTransport.Properties).KeepAliveOnSend;

            try {
                //  Ensure the ZIS URL is http/https
                fZoneUrl = fZone.ZoneUrl;
                string check = fZoneUrl.Scheme.ToLower();
                if ( !check.Equals( "http" ) && !check.Equals( "https" ) ) {
                    throw new AdkException
                        ( "HttpProtocolHandler cannot handle URL: " + fZone.ZoneUrl, fZone );
                }

                //  Prepare headers later used to send messages
                fHttpUserAgent = fZone.Agent.Id + " (Adk/" + fZone.Agent.Runtime.AdkVersion + ")";
            }
            catch ( Exception thr ) {
                throw new AdkException
                    ( "HttpProtocolHandler could not parse URL \"" + fZone.ZoneUrl + "\": " + thr,
                      fZone );
            }

            _clientFactory = fZone.Agent.Runtime.HttpClientFactory;

            if ( fTransport.Secure )
            {
                _secureClient = BuildSecureClient();
            }
        }


        public virtual void Close( IZone zone )
        {
            DisposeSecureClient();
        }

        public virtual void Shutdown()
        {
            DisposeSecureClient();
        }

        public abstract void Start();

        /// <summary>  Sends a SIF infrastructure message and returns the response.</summary>
        /// <remarks>
        /// The message content should consist of a complete &lt;SIF_Message&gt; element.
        /// This method sends whatever content is passed to it without any checking
        /// or validation of any kind.
        /// </remarks>
        /// <param name="msg">The message content</param>
        /// <returns> The response from the ZIS (expected to be a &lt;SIF_Ack&gt; message)
        /// </returns>
        /// <exception cref="AdkMessagingException"> is thrown if there is an error sending
        /// the message to the Zone Integration Server
        /// </exception>
        public IMessageInputStream Send( IMessageOutputStream msg )
        {
            try {
                return TrySend( msg );
            }
            catch ( AdkException ) {
                throw;
            }
            catch ( Exception ex ) {
                throw new AdkMessagingException
                    ( "HttpProtocolHandler: Unexpected error sending message: " + ex, fZone, ex );
            }
        }

        /// <summary>
        /// Returns true if the protocol and underlying transport are currently active
        /// for this zone
        /// </summary>
        /// <param name="zone"></param>
        /// <returns>True if the protocol handler and transport are active</returns>
        public abstract bool IsActive( ZoneImpl zone );

        /// <summary>
        /// Creates the SIF_Protocol object that will be included with a SIF_Register
        /// message sent to the zone associated with this Transport.</Summary>
        /// <remarks>
        /// The base class implementation creates an empty SIF_Protocol with zero
        /// or more SIF_Property elements according to the parameters that have been
        /// defined by the client via setParameter. Derived classes should therefore
        /// call the superclass implementation first, then add to the resulting
        /// SIF_Protocol element as needed.
        /// </remarks>
        /// <param name="zone"></param>
        /// <returns></returns>
        public abstract SIF_Protocol MakeSIF_Protocol( IZone zone );


        private IMessageInputStream TrySend( IMessageOutputStream msg )
        {
            MessageStreamImpl returnStream;
            using HttpRequestMessage request = CreateRequestMessage( fZoneUrl, msg );
            TimeSpan requestTimeout = ((HttpProperties)fTransport.Properties).RequestTimeout;

            try {
                if ( (fZone.Agent.Runtime.Debug & AdkDebugFlags.Transport) != 0 ) {
                    fZone.Log.Debug( "Sending message (" + msg.Length + " bytes)" );
                }
                if ( (fZone.Agent.Runtime.Debug & AdkDebugFlags.Message_Content) != 0 ) {
                    fZone.Log.Debug( msg.Decode() );
                }

                try {
                    returnStream = fTransport.Secure
                        ? SendSecure( request, requestTimeout )
                        : SendPlain( request, requestTimeout );
                }
                catch ( Exception thr ) {
                    throw new AdkTransportException
                        ( "An unexpected error occurred while receiving data from the ZIS: " + thr,
                          fZone, thr );
                }
            }
            catch ( AdkException ) {
                // rethrow anything that's already wrapped in an AdkException
                throw;
            }
            catch ( Exception thr ) {
                throw new AdkMessagingException
                    ( "HttpProtocolHandler: Error receiving response to sent message: " + thr, fZone );
            }

            return returnStream;
        }

        private MessageStreamImpl SendPlain( HttpRequestMessage request, TimeSpan timeout )
        {
            HttpClient client = _clientFactory.CreateClient( AdkServiceCollectionExtensions.SifHttpClientName );
            return ExecuteSend( client, request, timeout );
        }

        /// <summary>
        /// Sends using the long-lived secure client, holding a read lock for the entire duration
        /// of the send so that <see cref="DisposeSecureClient"/> cannot dispose the client
        /// while a send is in progress.
        /// </summary>
        private MessageStreamImpl SendSecure( HttpRequestMessage request, TimeSpan timeout )
        {
            _secureClientLock.EnterReadLock();
            try
            {
                if ( _secureClient == null )
                {
                    throw new AdkException(
                        "HTTPS client is not available; Open() must be called before Send(), or Close()/Shutdown() has already been called.",
                        fZone);
                }
                return ExecuteSend( _secureClient, request, timeout );
            }
            finally
            {
                _secureClientLock.ExitReadLock();
            }
        }

        private MessageStreamImpl ExecuteSend( HttpClient client, HttpRequestMessage request, TimeSpan timeout )
        {
            using CancellationTokenSource cts = new CancellationTokenSource( timeout );
            using HttpResponseMessage response =
                client.Send( request, HttpCompletionOption.ResponseHeadersRead, cts.Token );
            response.EnsureSuccessStatusCode();

            if ( (fZone.Agent.Runtime.Debug & AdkDebugFlags.Transport) != 0 ) {
                fZone.Log.Debug
                    ( "Expecting reply (" + response.Content.Headers.ContentLength + " bytes)" );
            }

            MessageStreamImpl returnStream = new MessageStreamImpl( response.Content.ReadAsStream() );

            if ( (fZone.Agent.Runtime.Debug & AdkDebugFlags.Transport) != 0 ) {
                fZone.Log.Debug( "Received reply (" + returnStream.Length + " bytes)" );
            }
            if ( (fZone.Agent.Runtime.Debug & AdkDebugFlags.Message_Content) != 0 ) {
                fZone.Log.Debug( returnStream.Decode() );
            }

            return returnStream;
        }

        #endregion

        #region Protected Members

        protected internal ZoneImpl Zone
        {
            get { return fZone; }
        }


        /// <summary>  Get an outbound connection to the ZIS</summary>
        protected HttpRequestMessage CreateRequestMessage( Uri uri, IMessageOutputStream msg )
        {
            try {
                MemoryStream requestStream = new MemoryStream();
                msg.CopyTo( requestStream );
                requestStream.Seek( 0, SeekOrigin.Begin );

                HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Post, uri );
                request.Headers.TryAddWithoutValidation( "User-Agent", fHttpUserAgent );
                request.Headers.ConnectionClose = !fKeepAliveOnSend;

                StreamContent content = new StreamContent( requestStream );
                content.Headers.ContentType = MediaTypeHeaderValue.Parse( SifIOFormatter.CONTENTTYPE );
                content.Headers.ContentLength = msg.Length;
                request.Content = content;

                return request;
            }
            catch ( Exception webEx ) {
                throw new AdkTransportException
                    ( "Failed to create HTTP request " + fZoneUrl.AbsoluteUri + ": " + webEx, fZone,
                      webEx );
            }
        }

        protected SifParser CreateParser()
        {
            return new SifParser(fZone.Agent.Runtime);
        }

        #endregion

        #region Private helpers

        /// <summary>
        /// Builds a long-lived <see cref="HttpClient"/> for HTTPS connections, configured with
        /// the client certificate (if any) and server-certificate validation callback.
        /// Called once from <see cref="Open"/> — no lazy-init race.
        /// </summary>
        private HttpClient BuildSecureClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;

            handler.ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) =>
            {
                if (errors == System.Net.Security.SslPolicyErrors.None)
                {
                    return true;
                }
                if ((fZone.Agent.Runtime.Debug & AdkDebugFlags.Messaging_Detailed) != 0)
                {
                    fTransport.DebugTransport(
                        "Certificate validation failed with errors: " + errors.ToString(),
                        new object[0]);
                }
                return false;
            };

            X509Certificate2 cert = fTransport.GetClientAuthenticationCertificate();
            if (cert == null)
            {
                fTransport.DebugTransport("No certificate found for client authentication", new object[0]);
            }
            else
            {
                handler.ClientCertificates.Add(cert);
            }

            HttpClient client = new HttpClient(handler, disposeHandler: true);
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan; // timeout governed per-request via CancellationTokenSource
            return client;
        }

        private void DisposeSecureClient()
        {
            _secureClientLock.EnterWriteLock();
            try
            {
                _secureClient?.Dispose();
                _secureClient = null;
            }
            finally
            {
                _secureClientLock.ExitWriteLock();
            }
        }

        #endregion

    }
}

// Synchronized with Library-ADK-1.5.0.Version_5.SIFPrimitives.java
