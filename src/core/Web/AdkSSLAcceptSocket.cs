//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;

namespace OpenADK.Web
{
    /// <summary>
    /// Summary description for AdkSSLAcceptSocket.
    /// </summary>
    public class AdkSSLAcceptSocket : IAcceptSocket
    {
        private Socket fAcceptSocket;
        private X509Certificate2 fCertificate;
        private RemoteCertificateValidationCallback fClientCertificateValidator;

        public AdkSSLAcceptSocket(X509Certificate2 certificate, RemoteCertificateValidationCallback validator = null)
        {
            fCertificate = certificate;
            fClientCertificateValidator = validator;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Bind(IPEndPoint endPoint)
        {
            if (fAcceptSocket != null)
            {
                throw new InvalidOperationException("Socket is already bound");
            }
            fAcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            fAcceptSocket.Bind(endPoint);
            fAcceptSocket.Listen(10);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Close()
        {
            if (fAcceptSocket != null)
            {
                fAcceptSocket.Close();
                fAcceptSocket = null;
            }
        }

        public void BeginAccept(AsyncCallback callback,
                                 object state)
        {
            fAcceptSocket.BeginAccept(callback, state);
        }

        public IConnectedSocket EndAccept(IAsyncResult result)
        {
            Socket clientSocket = fAcceptSocket.EndAccept(result);
            SslStream sslStream = new SslStream(new NetworkStream(clientSocket, true), false, 
                fClientCertificateValidator);
            
            try
            {
                sslStream.AuthenticateAsServer(fCertificate, fClientCertificateValidator != null, System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13, false);
            }
            catch
            {
                sslStream?.Dispose();
                clientSocket?.Close();
                throw;
            }

            return new AdkSSLConnectedSocket(clientSocket, sslStream);
        }
    }
}
