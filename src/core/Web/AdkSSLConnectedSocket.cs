//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace OpenADK.Web
{
    /// <summary>
    /// Summary description for AdkSSLConnectedSocket.
    /// </summary>
    public class AdkSSLConnectedSocket : IConnectedSocket
    {
        private SslStream fStream;
        private Socket fSocket;

        /// <summary/>
        public AdkSSLConnectedSocket(Socket socket, SslStream sslStream)
        {
            fSocket = socket;
            fStream = sslStream;
        }

        /// <summary/>
        public bool Connected
        {
            get { return fSocket.Connected; }
        }

        /// <summary/>
        public void SetSocketOption(SocketOptionLevel level,
                                     SocketOptionName name,
                                     int val)
        {
            fSocket.SetSocketOption(level, name, val);
        }

        /// <summary/>
        public void Close()
        {
            fStream?.Close();
            fSocket?.Close();
        }

        /// <summary/>
        public void Shutdown(SocketShutdown shutDownType)
        {
            fSocket.Shutdown(shutDownType);
        }

        /// <summary/>
        public EndPoint LocalEndPoint
        {
            get { return fSocket.LocalEndPoint; }
        }


        /// <summary/>
        public EndPoint RemoteEndPoint
        {
            get { return fSocket.RemoteEndPoint; }
        }

        /// <summary/>
        public Stream CreateStream(FileAccess access,
                                    bool ownsSocket)
        {
            return fStream;
        }
    }
}
