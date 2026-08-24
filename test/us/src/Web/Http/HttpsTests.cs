using Library.UnitTesting.Framework;
using Xunit;
using OpenADK.Library;
using OpenADK.Library.Impl;
using OpenADK.Util;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace OpenADK.Web.Http
{
    /// <summary>
    /// Tests HTTPS support in the ADK
    /// </summary>
    
    public class HttpsTestFixture : IDisposable
    {
        private static readonly string[] Certificates = ["issuer.pfx", "localhost.pfx", "127.0.0.1.pfx", "invalid.pfx"];
        public const string CertificatePassword = "changeit";

        public X509Certificate2 RootCert { get; }
        public X509Certificate2 ServerCert { get; }
        public X509Certificate2 IpCert { get; }
        public X509Certificate2 InvalidCert { get; }
        public X509Store RootStore { get; }
        public X509Store Store { get; }

        public HttpsTestFixture()
        {
            foreach (var fileName in Certificates)
            {
                using Stream stream = typeof(HttpsTests).Assembly.GetManifestResourceStream("Library.Nunit.US.res." + fileName) ??
                    throw new FileNotFoundException("Could not find embedded resource: Library.Nunit.US.res." + fileName);

                var fullPath = Path.Combine(Environment.CurrentDirectory, fileName);
                using Stream certFile = File.OpenWrite(fullPath);
                Streams.CopyStream(stream, certFile);
            }

            RootCert = X509CertificateLoader.LoadPkcs12FromFile("issuer.pfx", CertificatePassword);
            IpCert = X509CertificateLoader.LoadPkcs12FromFile("127.0.0.1.pfx", CertificatePassword);
            ServerCert = X509CertificateLoader.LoadPkcs12FromFile("localhost.pfx", CertificatePassword);
            InvalidCert = X509CertificateLoader.LoadPkcs12FromFile("invalid.pfx", CertificatePassword);

            RootStore = new(StoreName.Root, StoreLocation.CurrentUser);
            RootStore.Open(OpenFlags.ReadWrite);
            RootStore.Add(RootCert);

            Store = new(StoreName.My, StoreLocation.CurrentUser);
            Store.Open(OpenFlags.ReadWrite);
            Store.Add(IpCert);
            Store.Add(ServerCert);
            Store.Add(InvalidCert);
        }

        public void Dispose()
        {
            Store.Remove(InvalidCert);
            Store.Remove(ServerCert);
            Store.Remove(IpCert);
            RootStore.Remove(RootCert);

            Store.Close();
            RootStore.Close();

            Store.Dispose();
            RootStore.Dispose();
        }
    }

    public class HttpsTests : AdkTest, IClassFixture<HttpsTestFixture>
    {
        private HttpTransport fTransport = null;
        private SimpleHandler fHandler = null;
        private HttpsProperties fProps = null;
        private IZone fZone = null;
        private Agent fAgent;
        private const string HANDLER_URL = "/";
        private const string SERVER_TEST_URL = "https://localhost:9000/";

        public HttpsTests(HttpsTestFixture fixture)
        {
            Runtime.Debug = AdkDebugFlags.All;
            fAgent = CreateTestAgent();
            fAgent.Initialize();

            fTransport = (HttpTransport)fAgent.TransportManager.GetTransport("https");

            fProps = (HttpsProperties)fTransport.Properties;
            fProps.Port = 9000;
            fProps.SSLCertName = "CN=localhost, O=OpenADK, C=US";
            fProps.ClientAuthLevel = 0;
            fProps.ClientCertName = null;

            fZone = new TestZoneImpl("TestZone", SERVER_TEST_URL, fAgent, fAgent.Properties);
            fZone.Properties.MessagingMode = AgentMessagingMode.Push;
        }

        public override void Dispose()
        {
            fTransport?.Shutdown();
            fTransport = null;
            base.Dispose();
        }

        /// <summary>
        /// This test should set up an HTTPS server that requires
        /// client certificates to be sent. It should then try to 
        /// connect to the server without using client certs. 
        /// This should fail. It then should try with client
        /// certs enabled and it should succeed.
        /// </summary>
        [Fact(Explicit = true)]
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
        [Fact(Explicit = true, Skip = "Current implementation doesn't perform the subject-hostname validation.")]
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
        [Fact(Explicit = true)]
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
        [Fact(Explicit = true)]
        public void TestLevel3AuthSupportWithValidIP()
        {
            fProps.ClientAuthLevel = 3;
            fProps.ClientCertName = "127.0.0.1";
            startupTransport();
         
            // Run the test with a client certificate, and it should not fail
            RunConnectionTest(true, true);
        }

        [Fact(Explicit = true)]
        public void TestHttps()
        {
            startupTransport();
         
            RunConnectionTest(false, true);
        }

        [Fact(Explicit = true)]
        public void TestHttpsWithName()
        {
            fProps.SSLCertName = "CN=localhost, O=OpenADK, C=US";
            fProps.ClientCertName = "localhost";
            startupTransport();

            RunConnectionTest(false, true);
            RunConnectionTest(true, true);
        }

        [Fact(Explicit = true)]
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
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                errors == System.Net.Security.SslPolicyErrors.None;
            if (useClientCert)
            {
                X509Certificate2 cert = X509CertificateLoader.LoadPkcs12FromFile($"{fProps.ClientCertName}.pfx", HttpsTestFixture.CertificatePassword);
                handler.ClientCertificates.Add(cert);
            }

            bool sslConnectError = false;
            string errorMessage = string.Empty;
            try
            {
                using var httpClient = new HttpClient(handler);
                HttpResponseMessage response = httpClient.GetAsync(SERVER_TEST_URL).GetAwaiter().GetResult();
                fHandler.AssertResponse(response);
            }
            catch (HttpRequestException ex)
            {
                sslConnectError = true;
                errorMessage = ex.Message;
            }

            if (shouldPass)
            {
                Assert.False(sslConnectError);
            }
            else
            {
                Assert.True(sslConnectError);
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

            public void AssertResponse(HttpResponseMessage response)
            {
                // Write out the received headers for debugging purposes
                foreach (var header in response.Headers)
                {
                    Console.WriteLine("Header:{0} = \"{1}\"", header.Key, string.Join(",", header.Value));
                }

                Assert.Equal(OK_VALUE, response.Headers.GetValues(OK_HEADER).FirstOrDefault());

                string responseValue = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.Equal(OK_VALUE, responseValue);
            }
        }

    }
}
