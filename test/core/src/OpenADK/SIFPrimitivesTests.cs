using System;
using OpenADK.Library;
using OpenADK.Library.Infra;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.NUnit.Core.Library
{
    
    public class SIFPrimitivesTests : InMemoryProtocolTest, ISubscriber
    {
        
        public override void Dispose()
        {
            base.Dispose();
            // Clean up any properties that have been set and reset the ADK version
            Agent.Properties.Clear();
        }

        [Fact]
        public void testRegisterSIF20()
        {
            Runtime.SifVersion = (SifVersion.SIF20);
            String iconURL = "http://acme.foo.bar/ico";
            AgentProperties props = Agent.Properties;
            props.AgentIconUrl = iconURL;

            Agent.Name = "acmeAgent";
            props.AgentVendor = "acmeVendor";
            props.AgentVersion = "2.6.5.8";

            props.ApplicationName = "acmeApp";
            props.ApplicationVendor = "acme<>AppVendor";
            props.ApplicationVersion = "10.2";

            Zone.Connect( ProvisioningFlags.Register );
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler) Zone.ProtocolHandler;

            SIF_Register sr = (SIF_Register) handler.readMsg();

            Assert.Equal( Agent.Id, sr.SourceId);
            Assert.Equal( "acmeAgent", sr.SIF_Name);
            Assert.Equal( "acmeVendor", sr.SIF_NodeVendor);
            Assert.Equal( "2.6.5.8", sr.SIF_NodeVersion);
            SIF_Application appInfo = sr.SIF_Application;
            Assert.NotNull( appInfo );
            Assert.Equal( "acmeApp", appInfo.SIF_Product);
            Assert.Equal( "acme<>AppVendor", appInfo.SIF_Vendor);
            Assert.Equal( "10.2", appInfo.SIF_Version);
            Assert.Equal( iconURL, sr.SIF_Icon);
        }

        [Fact]
        public void testRegisterOverrideZISVersion()
        {
            Runtime.SifVersion = (SifVersion.SIF20);
            AgentProperties props = Agent.Properties;
            props.OverrideSifVersions = "1.1, 2.5";
            
            Zone.Connect(ProvisioningFlags.Register);
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler)Zone.ProtocolHandler;

            SIF_Register sr = (SIF_Register)handler.readMsg();
            SIF_Version[] versions = sr.GetSIF_Versions();
            Assert.NotNull( versions );
            Assert.Equal( 2, versions.Length );
            Assert.Equal( "1.1", versions[0].Value );
            Assert.Equal("2.5", versions[1].Value);
        }


        [Fact]
        public void testRegisterSIF15r1()
        {
            Runtime.SifVersion = (SifVersion.SIF15r1);
            String iconURL = "http://acme.foo.bar/ico";
            AgentProperties props = Agent.Properties;
            props.AgentIconUrl = iconURL;

            Agent.Name = "acmeAgent";
            props.AgentVendor = "acmeVendor";
            props.AgentVersion = "2.6.5.8";
            props.ApplicationName = "acmeApp";
            props.ApplicationVendor = "acme<>AppVendor";
            props.ApplicationVersion = "10.2";


            Zone.Connect( ProvisioningFlags.Register );
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler) Zone.ProtocolHandler;

            SIF_Register sr = (SIF_Register) handler.readMsg();

            Assert.Equal( Agent.Id, sr.SourceId);
            Assert.Equal( "acmeAgent", sr.SIF_Name);
            Assert.Null( sr.SIF_NodeVendor);
            Assert.Null( sr.SIF_NodeVersion);
            SIF_Application appInfo = sr.SIF_Application;
            Assert.Null( appInfo );
            Assert.Null( sr.SIF_Icon);


            // Assert the versions in the message. If the ADK is initialized to
            // SIF 1.5r1, it should not be sending any versions that start with a
            // "2"
            SifVersion messageVersion = sr.SifVersion;
            Assert.Equal( SifVersion.SIF15r1, messageVersion);
            foreach ( SIF_Version version in sr.GetSIF_Versions() )
            {
                String versionString = version.TextValue;
                Assert.StartsWith("1", versionString);
            }
        }

        /**
	 * 
	 */
        [Fact]
        public void testSIFPingDifferentVersions()
        {
            Runtime.SifVersion = (SifVersion.LATEST);
            Zone.Connect( ProvisioningFlags.None );
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler) Zone.ProtocolHandler;
            handler.clear();
            Zone.SifPing();
            SIF_SystemControl ssc = (SIF_SystemControl) handler.readMsg();

            Assert.Equal( SifVersion.LATEST, ssc.SifVersion);
            Assert.Equal( SifVersion.LATEST.Xmlns, ssc.GetXmlns());

            foreach ( SifVersion version in Runtime.SupportedSIFVersions )
            {
                // This may seem strange, but the ADK sometimes has a SIF version in the list of 
                // supported versions that is not fully supported yet (e.g. preparing the ADK for 
                // the next version. Because of that, only test SIF_Ping with versions if they
                // are equal to or less than SifVersion.LATEST
                if ( version.CompareTo( SifVersion.LATEST ) <= 0 )
                {
                    testSIFPingWithZISVersion( handler, version );
                }
            }
        }

        /**
	 * 
	 */
        [Fact]
        public void testSynchronousGetZoneStatus()
        {
            Runtime.SifVersion = (SifVersion.LATEST);
            Zone.Connect( ProvisioningFlags.None );
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler) Zone.ProtocolHandler;
            Zone.Properties.UseZoneStatusSystemControl = true;
            Zone.Properties.ZisVersion = SifVersion.SIF15r1.ToString();
            handler.clear();

            SIF_ZoneStatus szs = Zone.GetZoneStatus();

            SIF_SystemControl ssc = (SIF_SystemControl) handler.readMsg();

            Assert.Equal( SifVersion.SIF15r1, ssc.SifVersion);
            Assert.Equal( SifVersion.SIF15r1.Xmlns, ssc.GetXmlns());
            SifElement element = ssc.SIF_SystemControlData.GetChildList()[0];
            Assert.NotNull( element);
            Assert.True( element is SIF_GetZoneStatus);
        }

        /**
	 * 
	 */
        [Fact]
        public void testAsynchronousGetZoneStatus()
        {
            Runtime.SifVersion = (SifVersion.LATEST);
            Zone.Connect( ProvisioningFlags.None );
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler) Zone.ProtocolHandler;
            Zone.Properties.UseZoneStatusSystemControl = false;
            Zone.Properties.ZisVersion = SifVersion.SIF15r1.ToString();
            handler.clear();

            try
            {
                // We expect a SIF XML Error exception in this case because
                // our handler doesn't return a valid response back to a pull message
                Zone.GetZoneStatus();
            }
            catch ( SifException sifEx )
            {
                Assert.Equal( SifErrorCategoryCode.Xml, sifEx.ErrorCategory );
            }

            SIF_Request sr = (SIF_Request) handler.readMsg();

            Assert.Equal( SifVersion.SIF15r1, sr.SifVersion);
            Assert.Equal( SifVersion.SIF15r1.Xmlns, sr.GetXmlns());
        }


        private void testSIFPingWithZISVersion( InMemoryProtocolHandler handler, SifVersion testVersion )
        {
            SIF_SystemControl ssc;
            Zone.Properties.ZisVersion = testVersion.ToString();
            Zone.SifPing();
            ssc = (SIF_SystemControl) handler.readMsg();

            Assert.Equal( testVersion, ssc.SifVersion);
            Assert.Equal( testVersion.Xmlns, ssc.GetXmlns());
        }


        /**
	 * Tests registering with the ADK version set to 2.0 or greater, but the 
	 * AgentProperties.getZISVersion() property set to 1.5r1. This should result 
	 * in the SIF_Register message being sent in 1.5r1
	 * @throws Exception
	 */
        [Fact]
        public void testSIFRegisterZISVersion15r1()
        {
            Runtime.SifVersion = (SifVersion.LATEST);
            String iconURL = "http://acme.foo.bar/ico";
            AgentProperties props = Agent.Properties;
            // Set the ZIS Version to 1.5r1
            props.ZisVersion = SifVersion.SIF15r1.ToString();

            props.AgentIconUrl = iconURL;

            Agent.Name = "acmeAgent";
            props.AgentVendor = "acmeVendor";
            props.AgentVersion = "2.6.5.8";
            props.ApplicationName = "acmeApp";
            props.ApplicationVendor = "acme<>AppVendor";
            props.ApplicationVersion = "10.2";


            Zone.Connect( ProvisioningFlags.Register );
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler) Zone.ProtocolHandler;

            SIF_Register sr = (SIF_Register) handler.readMsg();

            Assert.Equal( SifVersion.SIF15r1, sr.SifVersion);
            Assert.Equal( SifVersion.SIF15r1.Xmlns, sr.GetXmlns());

            Assert.Equal( Agent.Id, sr.SourceId);
            Assert.Equal( "acmeAgent", sr.SIF_Name);
            Assert.Null( sr.SIF_NodeVendor);
            Assert.Null( sr.SIF_NodeVersion);
            SIF_Application appInfo = sr.SIF_Application;
            Assert.Null( appInfo );
            Assert.Null( sr.SIF_Icon);


            // Assert the versions in the message. If the ADK is initialized to
            // SIF 1.5r1, it should not be sending any versions that start with a
            // "2"
            SifVersion messageVersion = sr.SifVersion;
            Assert.Equal( SifVersion.SIF15r1, messageVersion);
            foreach ( SIF_Version version in sr.GetSIF_Versions() )
            {
                String versionString = version.TextValue;
                Assert.StartsWith("1", versionString);
            }
        }

        /**
	 * 
	 */
        [Fact]
        public void testProvisioningSIF20()
        {
            String[] expectedMessages =
                new String[] {"SIF_Register", "SIF_SystemControl", "SIF_SystemControl", "SIF_Provision"};
            Runtime.SifVersion = (SifVersion.LATEST);
            Zone.SetSubscriber( this, InfraDTD.SIF_AGENTACL, null );
            assertMessagesInVersion( SifVersion.LATEST, expectedMessages );
        }

        /**
	 * 
	 */
        [Fact]
        public void testProvisioningSIF15r1()
        {
            String[] expectedMessages = new String[] {"SIF_Register", "SIF_SystemControl", "SIF_Subscribe"};
            Runtime.SifVersion = (SifVersion.SIF15r1);
            Zone.SetSubscriber(this, InfraDTD.SIF_AGENTACL, null);
            assertMessagesInVersion( SifVersion.SIF15r1, expectedMessages );
        }

        /**
	 * 
	 */
        [Fact]
        public void testProvisioningZIS15r1()
        {
            String[] expectedMessages = new String[] {"SIF_Register", "SIF_SystemControl", "SIF_Subscribe"};
            Runtime.SifVersion = (SifVersion.LATEST);
            Zone.Properties.ZisVersion = SifVersion.SIF15r1.ToString();
            Zone.SetSubscriber(this, InfraDTD.SIF_AGENTACL, null);
            assertMessagesInVersion( SifVersion.SIF15r1, expectedMessages );
        }


        private void assertMessagesInVersion( SifVersion version, String[] expectedMessages )
        {
            Zone.Connect( ProvisioningFlags.Register );
            InMemoryProtocolHandler handler = (InMemoryProtocolHandler) Zone.ProtocolHandler;

            for ( int a = 0; a < expectedMessages.Length; a++ )
            {
                SifMessagePayload smp = (SifMessagePayload) handler.readMsg();
                Assert.Equal( expectedMessages[a], smp.Tag);
                Assert.Equal( version, smp.SifVersion);
            }

            Assert.Null( handler.readMsg());
        }

        #region ISubscriber Members

        /// <summary>  Respond to a SIF_Event received from a zone.</summary>
        /// <param name="evnt">The event data</param>
        /// <param name="zone">The zone from which this event originated</param>
        /// <param name="info">Information about the SIF_Event message</param>
        void ISubscriber.OnEvent( Event evnt, IZone zone, IMessageInfo info )
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
