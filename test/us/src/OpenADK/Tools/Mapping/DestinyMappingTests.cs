using System;
using System.Collections;
using System.Collections.Specialized;
using OpenADK.Library;
using OpenADK.Library.Infra;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Library.Tools.Mapping;
using Xunit;

namespace Library.Nunit.US.Library.Tools.Mapping
{
    
    public class DestinyMappingTests : UsAdkTest
    {
        private AgentConfig fCfg;

        
        public DestinyMappingTests()
        {
            Runtime.SifVersion = SifVersion.SIF15r1;
            fCfg = new AgentConfig();
            fCfg.Read("..\\..\\OpenADK\\Tools\\Mapping\\Destiny2.0.cfg", false);
        }

        [Fact]
        public void testSchoolInfo010()
        {
            String schoolInfoResp = "	<SIF_Message  xmlns=\"http://www.sifinfo.org/infrastructure/1.x\" Version=\"1.5r1\">"
                                    + "	  <SIF_Response>"
                                    + "		<SIF_Header>"
                                    + "		  <SIF_MsgId>B329CA6E5BC342339F135880CEE0578E</SIF_MsgId>"
                                    + "		  <SIF_Date>20070813</SIF_Date>"
                                    + "		  <SIF_Time Zone=\"UTC-06:00\">13:55:57</SIF_Time>"
                                    + "		  <SIF_Security>"
                                    + "			<SIF_SecureChannel>"
                                    + "			  <SIF_AuthenticationLevel>0</SIF_AuthenticationLevel>"
                                    + "			  <SIF_EncryptionLevel>0</SIF_EncryptionLevel>"
                                    + "			</SIF_SecureChannel>"
                                    + "		  </SIF_Security>"
                                    + "		  <SIF_SourceId>SASIxp</SIF_SourceId>"
                                    + "		  <SIF_DestinationId>Destiny</SIF_DestinationId>"
                                    + "		</SIF_Header>"
                                    + "		<SIF_RequestMsgId>88BE30BA0B8746809598854A1F58C8CC</SIF_RequestMsgId>"
                                    + "		<SIF_PacketNumber>1</SIF_PacketNumber>"
                                    + "		<SIF_MorePackets>No</SIF_MorePackets>"
                                    + "		<SIF_ObjectData>"
                                    + "		 <SchoolInfo RefId=\"B3E73AC5E8C7392044015E25A454AC6F\">"
                                    + "			<LocalId>997</LocalId>"
                                    + "			<SchoolName>Junior High Demo</SchoolName>"
                                    + "			<PrincipalInfo>"
                                    + "			  <ContactName>Mrs. Pleasant</ContactName>"
                                    + "			</PrincipalInfo>\r\n"
                                    +
                                    /* " <PhoneNumber Format=\"NA\" Type=\"TE\">(949) 888-7655</PhoneNumber>" + */
                                    "			<PhoneNumber Format=\"NA\" Type=\"FE\">888-9877</PhoneNumber>\r\n"
                                    + "			<Address Type=\"SS\">"
                                    + "			  <Street>"
                                    + "				<Line1>Grand Avenue</Line1>"
                                    + "			  </Street>"
                                    + "			  <City>Plesantville</City>"
                                    + "			  <StatePr Code=\"CA\" />"
                                    + "			  <Country Code=\"US\" />"
                                    + "			  <PostalCode>12345</PostalCode>"
                                    + "			</Address>"
                                    + "			<IdentificationInfo Code=\"76\">997</IdentificationInfo>"
                                    + "		  </SchoolInfo>"
                                    + "		</SIF_ObjectData>"
                                    + "	  </SIF_Response>" + "	</SIF_Message>";

            SifParser parser = new SifParser(Runtime);
            SifMessagePayload smi = (SifMessagePayload) parser.Parse(
                                                            schoolInfoResp, null );

            // Verify that it is parsing the correct version
            Assert.Equal(SifVersion.SIF15r1, smi.SifVersion);
            Assert.Equal("1.5r1", smi.VersionAttribute);

            SifDataObject sdo = (SifDataObject) ((SIF_Response) smi)
                                                    .SIF_ObjectData.GetChildList()[0];

            // Now attempt an inbound mapping
            IDictionary fields = new ListDictionary();
            StringMapAdaptor sma = new StringMapAdaptor( fields );
            Mappings m = fCfg.Mappings.GetMappings( "Default" ).Select( "asdf","SASIxp", smi.SifVersion );

            m.MapInbound( sdo, sma, smi.SifVersion );
            Assert.True(fields.Count > 0);
            Assert.Equal( "888-9877", fields["FAX"]);
        }
    }
}
