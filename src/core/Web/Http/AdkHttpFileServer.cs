//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace OpenADK.Web.Http
{
    /// <summary>
    /// Summary description for HttpSimpleHost.
    /// </summary>
    public class AdkHttpFileServer : AdkHttpServer
    {
        public AdkHttpFileServer(string physicalPath,
                                  string defaultFileName)
        {
            this.Listener.AddHandlerContext
                ("", "/", new AdkHttpDirectoryServer(physicalPath, "/", defaultFileName), false);
        }

        public void AddContext(string physicalPath,
                                string virtualPath,
                                string defaultFileName)
        {
            this.Listener.AddHandlerContext
                ("", virtualPath,
                  new AdkHttpDirectoryServer(physicalPath, virtualPath, defaultFileName), false);
        }

        private void ClientConnected(AdkSocketConnection connection)
        {
            IPEndPoint point = (IPEndPoint)connection.Socket.RemoteEndPoint;
            Console.WriteLine
                ("Client Connected: {0}:{1}. Total Connections:{2}", point.Address, point.Port,
                  connection.Binding.ConnectionCount);
        }

        private void ClientDisconnected(AdkSocketConnection connection)
        {
            Console.WriteLine
                ("Client Disconnected: Total Connections:{0}", connection.Binding.ConnectionCount);
        }

        private void Error(AdkSocketConnection connection,
                            Exception ex)
        {
            Console.WriteLine("ERROR: {0}:{1}", ex.Message, ex.StackTrace);
        }

        public override void AddListener(AdkSocketBinding binding)
        {
            base.AddListener(binding);
            binding.SocketAccepted += new AdkSocketMessageHandler(ClientConnected);
            binding.SocketClosed += new AdkSocketMessageHandler(ClientDisconnected);
            binding.SocketError += new AdkSocketErrorHandler(Error);
        }

        public void AddHttpBinding(IPAddress address,
                                    int port)
        {
            AdkSocketBinding listener = this.CreateHttpListener();
            listener.HostAddress = address;
            listener.Port = port;
            this.AddListener(listener);
        }

        public void AddHttpsBinding(IPAddress address,
                                     int port,
                                     X509Certificate2 certificate,
                                     RemoteCertificateValidationCallback validator = null)
        {
            AdkSocketBinding listener = this.CreateHttpsListener(certificate, validator);
            listener.HostAddress = address;
            listener.Port = port;
            this.AddListener(listener);
        }

        public void Start()
        {
            base.StartServer();
        }

        public void Stop()
        {
            base.StopServer(false);
        }
    }
}
