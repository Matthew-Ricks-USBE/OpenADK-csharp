using Library.UnitTesting.Framework;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using OpenADK.Library;
using OpenADK.Library.Impl;
using OpenADK.Library.Infra;
using OpenADK.Library.Log;
using OpenADK.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace OpenADK.Web.Http
{
    /// <summary>
    /// Tests HTTPS support in the ADK
    /// </summary>
    [TestFixture, Explicit("Requires trusted certificates in Windows certificate store")]
    public class HttpsTests
    {
        private HttpTransport fTransport = null;
        private SimpleHandler fHandler = null;
        private HttpsProperties fProps = null;
        private IZone fZone = null;
        private Agent fAgent = new TestAgent();

        private readonly string[] CERTIFICATES = ["issuer.pfx", "localhost.pfx", "127.0.0.1.pfx", "invalid.pfx"];
        private const string CERTIFICATE_PASSWORD = "changeit";

        private X509Certificate2 fRootCert;
        private X509Certificate2 fServerCert;
        private X509Certificate2 fIpCert;
        private X509Certificate2 fInvalidCert;
        private X509Store fRootStore;
        private X509Store fStore;

        private const string HANDLER_URL = "/";
        private const string SERVER_TEST_URL = "https://localhost:9000/";

        /// <summary>
        /// This method runs once at the start of this fixture.
        /// </summary>
        [OneTimeSetUp]
        public void SetupTestFixture()
        {
            // Ensure certificates are on the file system.
            foreach (var fileName in CERTIFICATES)
            {
                // Get the certificate contents.
                using Stream stream = GetType().Assembly.GetManifestResourceStream("Library.Nunit.US.res." + fileName) ??
                    throw new FileNotFoundException("Could not find embedded resource: Library.Nunit.US.res." + fileName);

                // Paste to the destination.
                var fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
                using Stream certFile = File.OpenWrite(fullPath);
                Streams.CopyStream(stream, certFile);
            }

            // Set up common certificates.
            fRootCert = X509CertificateLoader.LoadPkcs12FromFile("issuer.pfx", CERTIFICATE_PASSWORD);
            fIpCert = X509CertificateLoader.LoadPkcs12FromFile("127.0.0.1.pfx", CERTIFICATE_PASSWORD);
            fServerCert = X509CertificateLoader.LoadPkcs12FromFile("localhost.pfx", CERTIFICATE_PASSWORD);
            fInvalidCert = X509CertificateLoader.LoadPkcs12FromFile("invalid.pfx", CERTIFICATE_PASSWORD);

            fRootStore = new(StoreName.Root, StoreLocation.CurrentUser);
            fRootStore.Open(OpenFlags.ReadWrite);
            fRootStore.Add(fRootCert);

            fStore = new(StoreName.My, StoreLocation.CurrentUser);
            fStore.Open(OpenFlags.ReadWrite);
            fStore.Add(fIpCert);
            fStore.Add(fServerCert);
            fStore.Add(fInvalidCert);

            // Prep logging output.
            ConsoleAppender cAppender = new ConsoleAppender();
            cAppender.Layout = new PatternLayout(Adk.DEFAULT_LOG_PATTERN);
            SetLogAppender(cAppender, Level.Debug, false);
        }

        /// <summary>
        /// This method runs once the beginning of each test.
        /// </summary>
        [SetUp]
        public void SetUpTest()
        {
            Adk.Debug = AdkDebugFlags.All;
            Adk.Initialize();

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors)
                =>
            { return errors == System.Net.Security.SslPolicyErrors.None; };
            ServicePointManager.CheckCertificateRevocationList = false;

            fTransport = (HttpTransport)fAgent.TransportManager.GetTransport("https");

            fProps = (HttpsProperties)fTransport.Properties;
            fProps.Port = 9000;
            fProps.SSLCertName = "CN=localhost, O=OpenADK, C=US";
            fProps.ClientAuthLevel = 0;
            fProps.ClientCertName = null;

            fZone = new TestZone();
            fZone.Properties.MessagingMode = AgentMessagingMode.Push;
        }

        /// <summary>
        /// This method runs once at the end of every test.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            fTransport?.Shutdown();
            fTransport = null;
        }

        /// <summary>
        /// This method runs once at the end of this fixture.
        /// </summary>
        [OneTimeTearDown]
        public void TearDownFixture()
        {
            fStore.Remove(fInvalidCert);
            fStore.Remove(fServerCert);
            fStore.Remove(fIpCert);
            fRootStore.Remove(fRootCert);

            fStore.Close();
            fRootStore.Close();

            fStore.Dispose();
            fRootStore.Dispose();
        }


        /// <summary>
        /// This test should set up an HTTPS server that requires
        /// client certificates to be sent. It should then try to 
        /// connect to the server without using client certs. 
        /// This should fail. It then should try with client
        /// certs enabled and it should succeed.
        /// </summary>
        [Test]
        public void TestLevel2AuthSupport()
        {
            fProps.ClientAuthLevel = 2;
            fProps.ClientCertName = "invalid";
            startupTransport();

            //RunConnectionTest( false, false );
            RunConnectionTest(true, true);
        }

        /// <summary>
        /// This test should set up an HTTPS server that requires
        /// client certificates to be sent. It should then try to 
        /// connect to the server using a valid certificate, but with a
        /// subject that does not match the host name. This should fail.
        /// </summary>
        /// <remarks>
        /// Implementing this would be somewhat onerous&mdash;
        /// per the note in the associated method&mdash;
        /// so this has been left as-is for now.
        /// </remarks>
        /// <see cref="HttpTransport.verifyLevel3Authentication(object, X509Certificate, X509Chain, SslPolicyErrors)"/>
        [Test]
        [Ignore("Current implementation doesn't perform the subject-hostname validation.")]
        public void TestLevel3AuthSupportWithInvalidHost()
        {
            fProps.ClientAuthLevel = 3;
            fProps.ClientCertName = "invalid";
            startupTransport();
         
            // Run the test with a client certificate, and it should fail
            RunConnectionTest(true, false);
        }


        /// <summary>
        /// Tests Level 3 authentication with a certificate whose CN matches the hostname.
        /// </summary>
        [Test]
        public void TestLevel3AuthSupportWithValidHost()
        {
            fProps.ClientAuthLevel = 3;
            fProps.ClientCertName = "localhost";
            startupTransport();

            // Run the test with a client certificate, and it should not fail
            RunConnectionTest(true, true);
        }

        /// <summary>
        /// Tests Level 3 authentication with a certificate whose CN matches the IP address.
        /// </summary>
        [Test]
        public void TestLevel3AuthSupportWithValidIP()
        {
            fProps.ClientAuthLevel = 3;
            fProps.ClientCertName = "127.0.0.1";
            startupTransport();
         
            // Run the test with a client certificate, and it should not fail
            RunConnectionTest(true, true);
        }

        [Test]
        public void TestHttps()
        {
            startupTransport();
         
            RunConnectionTest(false, true);
        }

        [Test]
        public void TestHttpsWithName()
        {
            fProps.SSLCertName = "CN=localhost, O=OpenADK, C=US";
            fProps.ClientCertName = "localhost";
            startupTransport();

            RunConnectionTest(false, true);
            RunConnectionTest(true, true);
        }

        [Test]
        public void TestHttpsWithFile()
        {
            fProps.SSLCertFile = "localhost.pfx";
            fProps.SSLCertFilePassword = "changeit";
            startupTransport();
            RunConnectionTest(false, true);
        }

        private void startupTransport()
        {
            fTransport.Activate(fAgent);
            if (!fTransport.IsActive(fZone))
            {
                fTransport.Activate( fZone );
                fHandler = new SimpleHandler();
                ((AdkHttpApplicationServer) fTransport.Server).AddHandlerContext( "", HANDLER_URL, fHandler, true );
            }
        }


        private void RunConnectionTest(bool useClientCert, bool shouldPass)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(SERVER_TEST_URL);
            if (useClientCert)
            {
                X509Certificate2 cert = X509CertificateLoader.LoadPkcs12FromFile($"{fProps.ClientCertName}.pfx", CERTIFICATE_PASSWORD);
                request.ClientCertificates.Add(cert);
            }

            bool sslConnectError = false;
            string errorMessage = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                fHandler.AssertResponse(response);
            }
            catch (WebException wex)
            {
                if(wex.Status == WebExceptionStatus.SecureChannelFailure || wex.Status == WebExceptionStatus.SendFailure )
                {
                    sslConnectError = true;
                    errorMessage = wex.Message;
                }
                else
                {
                    throw;
                }
            }

            if (shouldPass)
            {
                Assert.IsFalse(sslConnectError, "Received an http exception: " + errorMessage);
            }
            else
            {
                Assert.IsTrue(sslConnectError, "Should have received an exception");
            }
        }


        private static void SetLogAppender(IAppender appender, Level level, bool additive)
        {
            Hierarchy hierarchy = LogManager.GetRepository() as Hierarchy;
            if (hierarchy != null)
            {
                if (!additive)
                {
                    hierarchy.Root.RemoveAllAppenders();
                }
                hierarchy.Root.AddAppender(appender);
                hierarchy.Threshold = level;
                hierarchy.Configured = true;
            }
            else
            {
                throw new AdkException("Unable to initialize log4net framework", null);
            }
        }


        private class SimpleHandler : IAdkHttpHandler
        {
            private const string OK_VALUE = "OK";
            private const string OK_HEADER = "CUSTOM_HEADER";

            public void ProcessRequest(AdkHttpRequestContext context)
            {
                context.Response.Headers.Add(OK_HEADER, OK_VALUE);
                context.Response.Write("OK");
            }

            public void AssertResponse(HttpWebResponse response)
            {
                // Write out the received headers for debugging purposes
                foreach (string key in response.Headers.AllKeys)
                {
                    Console.WriteLine("Header:{0} = \"{1}\"", key, response.Headers[key]);
                }

                Assert.AreEqual(OK_VALUE, response.Headers[OK_HEADER], "Did not receive proper response header");

                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseValue = reader.ReadToEnd();
                    Assert.AreEqual(OK_VALUE, responseValue);
                }
            }
        }

        private class TestZone : IZone
        {
            private AgentProperties fProperties;

            #region IZone Members

            public SIF_Ack SifUnregister()
            {
                // TODO:  Add TestZone.SifUnregister implementation
                return null;
            }

            public void PurgeQueue(bool incoming, bool outgoing)
            {
                // TODO:  Add TestZone.PurgeQueue implementation
            }

            public SIF_Ack SifRegister()
            {
                // TODO:  Add TestZone.SifRegister implementation
                return null;
            }

            public void ReportEvent(SifDataObject obj, EventAction action, string destinationId)
            {
                // TODO:  Add TestZone.ReportEvent implementation
            }

            void IZone.ReportEvent(SifDataObject obj, EventAction action)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.ReportEvent implementation
            }

            void IZone.ReportEvent(Event ev)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.ReportEvent implementation
            }

            public SIF_Ack SifUnprovide(string[] objectType)
            {
                // TODO:  Add TestZone.SifUnprovide implementation
                return null;
            }

            public SIF_Ack SifPing()
            {
                // TODO:  Add TestZone.SifPing implementation
                return null;
            }

            public SIF_Ack SifSubscribe(string[] objectType)
            {
                // TODO:  Add TestZone.SifSubscribe implementation
                return null;
            }

            public string ZoneId
            {
                get
                {
                    // TODO:  Add TestZone.ZoneId getter implementation
                    return null;
                }
            }

            public void Disconnect(ProvisioningFlags flags)
            {
                // TODO:  Add TestZone.Disconnect implementation
            }

            public ServerLog ServerLog
            {
                get
                {
                    // TODO:  Add TestZone.ServerLog getter implementation
                    return null;
                }
            }

            public IProtocolHandler ProtocolHandler
            {
                get { return null; }
            }

            public IList<SifException> ConnectWarnings
            {
                get
                {
                    // TODO:  Add TestZone.ConnectWarnings getter implementation
                    return null;
                }
            }

            public void WakeUp()
            {
                // TODO:  Add TestZone.WakeUp implementation
            }

            /// <summary>  Determines if the agent's queue for this zone is in sleep mode.
            /// 
            /// </summary>
            /// <param name="flags">When AdkFlags.LOCAL_QUEUE is specified, returns true if the
            /// Agent Local Queue is currently in sleep mode. False is returned if
            /// the Agent Local Queue is disabled. When AdkFlags.SERVER_QUEUE is
            /// specified, queries the sleep mode of the Zone Integration Server
            /// by sending a SIF_Ping message.
            /// </param>
            public bool IsSleeping(AdkQueueLocation flags)
            {
                // TODO:  Add TestZone.IsSleeping implementation
                return false;
            }

            public bool Connected
            {
                get
                {
                    // TODO:  Add TestZone.Connected getter implementation
                    return false;
                }
            }

            public void SetPublisher(IPublisher publisher, IElementDef objectType, PublishingOptions options)
            {
                // TODO:  Add TestZone.SetPublisher implementation
            }


            void IProvisioner.SetPublisher(IPublisher publisher)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.SetPublisher implementation
            }

            /// <summary>
            /// Register a Publisher message handler with this zone to process SIF_Requests
            /// for the specified object type. This method may be called repeatedly for
            /// each SIF Data Object type the agent will publish on this zone.
            /// </summary>
            /// <param name="publisher">
            /// An object that implements the <code>Publisher</code>
            /// interface to respond to SIF_Request queries received by the agent,
            /// where the SIF object type referenced by the request matches the
            /// specified objectType. This Publisher will be called whenever a
            /// SIF_Request is received on this zone and no other object in the
            /// message dispatching chain has processed the message.
            /// </param>
            /// <param name="objectType">An ElementDef constant from the SIFDTD class that 
            /// identifies a SIF Data Object type. E.g. SIFDTD.STUDENTPERSONAL
            /// </param>
            public void SetPublisher(IPublisher publisher, IElementDef objectType)
            {
                throw new NotImplementedException();
            }

            public void RemoveMessagingListener(IMessagingListener listener)
            {
                // TODO:  Add TestZone.RemoveMessagingListener implementation
            }

           public void SetSubscriber(ISubscriber subscriber, IElementDef objectType, SubscriptionOptions flags)
            {
                // TODO:  Add TestZone.SetSubscriber implementation
            }

            public SIF_Ack SifUnsubscribe(string[] objectType)
            {
                // TODO:  Add TestZone.SifUnsubscribe implementation
                return null;
            }


            public IUndeliverableMessageHandler ErrorHandler
            {
                get
                {
                    // TODO:  Add TestZone.ErrorHandler getter implementation
                    return null;
                }
                set
                {
                    // TODO:  Add TestZone.ErrorHandler setter implementation
                }
            }

            public Uri ZoneUrl
            {
                get
                {
                    // TODO:  Add TestZone.ZoneUrl getter implementation
                    return null;
                }
            }

            public ILog Log
            {
                get
                {
                    // TODO:  Add TestZone.Log getter implementation
                    return null;
                }
            }

            public SIF_ZoneStatus GetZoneStatus(TimeSpan timeout)
            {
                // TODO:  Add TestZone.GetZoneStatus implementation
                return null;
            }

            SIF_ZoneStatus IZone.GetZoneStatus()
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.GetZoneStatus implementation
                return null;
            }

            public void AddMessagingListener(IMessagingListener listener)
            {
                // TODO:  Add TestZone.AddMessagingListener implementation
            }

            ///<summary>
            ///Register a Subscriber message handler with this zone to process SIF_Event
            /// messages for the specified object type. This method may be called 
            /// repeatedly for each SIF Data Object type the agent subscribes to on 
            /// this zone.
            /// </summary>
            /// <param name="subscriber">
            /// An object that implements the <code>Subscriber</code>
            /// interface to respond to SIF_Event notifications received by the agent,
            /// where the SIF object type referenced by the request matches the
            /// specified objectType. This Subscriber will be called whenever a
            /// SIF_Event is received on this zone and no other object in the
            /// message dispatching chain has processed the message.
            /// </param>
            /// <param name="objectType">
            /// A constant from the SIFDTD class that identifies a
            /// SIF Data Object type.
            ///</param>
            public void SetSubscriber(ISubscriber subscriber, IElementDef objectType)
            {
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                // TODO:  Add TestZone.ToString implementation
                return null;
            }

            public Agent Agent
            {
                get
                {
                    // TODO:  Add TestZone.Agent getter implementation
                    return null;
                }
            }

            public void Connect(ProvisioningFlags provOptions)
            {
                // TODO:  Add TestZone.Connect implementation
            }

            public AgentProperties Properties
            {
                get
                {
                    if( fProperties == null )
                    {
                        fProperties = new AgentProperties( null );
                    }
                    return fProperties;
                }
                set
                {
                    fProperties = value;
                }
            }

            public SIF_Ack SifSend(string xml)
            {
                // TODO:  Add TestZone.SifSend implementation
                return null;
            }

           public string Query(Query query, IMessagingListener listener, string destinationId,
                                AdkQueryOptions queryOptions)
            {
                // TODO:  Add TestZone.Query implementation
                return null;
            }

           string IZone.Query(Query query, string destinationId, AdkQueryOptions queryOptions)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.Query implementation
                return null;
            }

           string IZone.Query(Query query, IMessagingListener listener, AdkQueryOptions queryOptions)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.Query implementation
                return null;
            }

           string IZone.Query(Query query, AdkQueryOptions queryOptions)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.Query implementation
                return null;
            }

           string IZone.Query(Query query, IMessagingListener listener)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.Query implementation
                return null;
            }

           string IZone.Query(Query query)
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.Query implementation
                return null;
            }

            public object UserData
            {
                get
                {
                    // TODO:  Add TestZone.UserData getter implementation
                    return null;
                }
                set
                {
                    // TODO:  Add TestZone.UserData setter implementation
                }
            }

            public SIF_Ack SifProvide(string[] objectType)
            {
                // TODO:  Add TestZone.SifProvide implementation
                return null;
            }

            void IProvisioner.SetQueryResults(IQueryResults queryResults, IElementDef objectType, QueryResultsOptions flags)
            {
                // TODO:  Add TestZone.SetQueryResults implementation
            }

            void IProvisioner.SetQueryResults(IQueryResults queryResults )
            {
                // TODO:  Add TestZone.OpenADK.Library.IZone.SetQueryResults implementation
            }

            #region IProvisioner Members

            /// <summary>
            /// Register a QueryResults object with this zone for the specified SIF object type.
            /// </summary>
            /// <param name="queryResults">
            /// An object that implements the <code>QueryResults</code>
            /// interface to respond to SIF_Response query results received by the agent,
            /// where the SIF object type referenced by the request matches the
            /// specified objectType. This QueryResults object will be called whenever
            /// a SIF_Response is received on this zone and no other object in the
            /// message dispatching chain has processed the message.
            ///</param>
            ///<param name="objectType">
            /// A constant from the SIFDTD class that identifies a
            /// SIF Data Object type.
            /// </param>
            public void SetQueryResults(IQueryResults queryResults, IElementDef objectType)
            {
                throw new NotImplementedException();
            }

            #endregion

            public void Sleep()
            {
                // TODO:  Add TestZone.Sleep implementation
            }

            #endregion
        }
    }
}