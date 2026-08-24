using System;
using System.Collections;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using OpenADK.Library.Tools.Mapping;
using Xunit;
using Library.xUnit.US.Tools.Mapping;

namespace Library.xUnit.US.Library.Tools.Mapping
{
    
    public class AttributeMappingTests : BaseMappingsTest
    {
        [Fact]
        public void testCountryCodeStudentPersonal()
        {
            Runtime.SifVersion = SifVersion.SIF15r1;

            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='StudentPersonal'>"
                                    +
                                    "       <field name='COUNTRY' sifVersion='+2.0'>AddressList/Address[@Type='0769']/Country=US</field>"
                                    +
                                    "       <field name='COUNTRY' sifVersion='-1.5r1'>StudentAddress[@PickupOrDropoff='NA',@DayOfWeek='NA']/Address[@Type='M']/Country[@Code='UK']</field>"
                                    +
                                    "       <field name='RESCOUNTRY' sifVersion='+2.0'>AddressList/Address[@Type='0765']/Country=US</field>"
                                    +
                                    "       <field name='RESCOUNTRY' sifVersion='-1.5r1'>StudentAddress[@PickupOrDropoff='NA',@DayOfWeek='NA']/Address[@Type='P']/Country[@Code='US']</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "RESCOUNTRY", "" );
            map.Add( "COUNTRY", "" );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            StudentPersonal sp = new StudentPersonal();
            doOutboundMapping( sma, sp, customMappings, null );

            StudentAddressList sal = sp.AddressList;
            Assert.NotNull(sal);
            Assert.Equal(2, sal.ChildCount);

            assertCountry( (Address) sp.GetElementOrAttribute( "StudentAddress/Address[@Type='P']" ), "US" );
            assertCountry( (Address) sp.GetElementOrAttribute( "StudentAddress/Address[@Type='M']" ), "UK" );
        }

        [Fact]
        public void testCountryCodeLEAInfo()
        {
            Runtime.SifVersion = SifVersion.SIF15r1;

            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='LEAInfo'>"
                                    +
                                    "		<field name='DISTRICT_COUNTRY' sifVersion='+2.0'>AddressList/Address[@Type='0123']/Country=US</field>"
                                    +
                                    "		<field name='DISTRICT_COUNTRY' sifVersion='-1.5r1'>Address[@Type='07']/Country[@Code='US']</field>"
                                    +
                                    "		<field name='CONTACT_PHONE' sifVersion='+2.0'>LEAContactList/LEAContact/ContactInfo/PhonenumberList/PhoneNumber[@Type='0096']/Number</field>"
                                    +
                                    "     <field name='CONTACT_PHONE' sifVersion='-1.5r1'>LEAContact/ContactInfo/PhoneNumber[@Format='NA',@Type='TE']</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "DISTRICT_COUNTRY", null );
            map.Add( "CONTACT_PHONE", "801.550.2796" );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            LEAInfo obj = new LEAInfo();
            doOutboundMapping( sma, obj, customMappings, null );

            Assert.Null( obj.AddressList);

            ContactInfo ci = (ContactInfo) obj
                                               .GetElementOrAttribute( "LEAContact/ContactInfo" );
            PhoneNumber phone = ci.PhoneNumberList.ItemAt( 0 );
            Assert.NotNull( phone);
            Assert.Equal("NA", phone.Format);
            Assert.Equal("TE", phone.Type);
            Assert.Equal("801.550.2796", phone.Number);
        }

        [Fact]
        public void testCountryCodeSchoolInfo()
        {
            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='SchoolInfo'>"
                                    +
                                    "		<field name='COUNTRY' sifVersion='+2.0'>AddressList/Address[@Type='0123']/Country=US</field>"
                                    +
                                    "     <field name='COUNTRY' sifVersion='-1.5r1'>Address[@Type='SS']/Country[@Code='US']</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "COUNTRY", "" );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            SchoolInfo obj = new SchoolInfo();
            doOutboundMapping( sma, obj, customMappings, null );

            assertAddressWithCountry( obj.AddressList, "US" );
        }

        [Fact]
        public void testCountryCodeStaffPersonal()
        {
            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='StaffPersonal'>"
                                    +
                                    "		<field name='ASTF.COUNTRY' sifVersion='+2.0'>AddressList/Address[@Type='0123']/Country=US</field>"
                                    +
                                    "		<field name='ASTF.COUNTRY' sifVersion='-1.5r1'>Address[@Type='M']/Country[@Code='US']</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "ASTF.COUNTRY", "" );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            StaffPersonal obj = new StaffPersonal();
            doOutboundMapping( sma, obj, customMappings, null );

            assertAddressWithCountry( obj.AddressList, "US" );
        }

        [Fact]
        public void testCountryCodeStudentContact()
        {
            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='StudentContact'>"
                                    +
                                    "		<field name='APRN.COUNTRY' sifVersion='+2.0'>AddressList/Address[@Type='0123']/Country=US</field>"
                                    +
                                    "		<field name='APRN.COUNTRY' sifVersion='-1.5r1'>Address[@Type='M']/Country[@Code='US']</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "APRN.COUNTRY", null );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            StudentContact obj = new StudentContact();
            doOutboundMapping( sma, obj, customMappings, null );

            Assert.Null( obj.AddressList);
        }

        private void assertAddressWithCountry( AddressList list,
                                               String expectedCountryCode )
        {
            Assert.NotNull(list);
            Assert.Equal(1, list.Count);
            assertCountry( list.ItemAt( 0 ), expectedCountryCode );
        }

        private void assertCountry( Address address, String expectedCountryCode )
        {
            Assert.NotNull( address);
            Assert.Equal( expectedCountryCode, address.Country);
        }

        [Fact]
        public void testLEAInfoPhones()
        {
            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='LEAInfo'>"
                                    +
                                    "		<field name='DISTRICT_PHONE' sifVersion='-1.5r1'>PhoneNumber[@Format='NA',@Type='TE']</field>"
                                    +
                                    "		<field name='CONTACT_PHONE' sifVersion='-1.5r1'>LEAContact/ContactInfo/PhoneNumber[@Format='NA',@Type='TE']</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "DISTRICT_PHONE", "912-555-6658" );
            map.Add( "CONTACT_PHONE", "912-888-6658" );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            LEAInfo obj = new LEAInfo();
            doOutboundMapping( sma, obj, customMappings, null );

            PhoneNumberList pnl = obj.PhoneNumberList;
            Assert.NotNull( pnl);
            PhoneNumber phone = obj.PhoneNumberList.ItemAt( 0 );
            Assert.Equal( "NA", phone.Format);
            Assert.Equal( "TE", phone.Type);
            Assert.Equal( "912-555-6658", phone.Number);

            LEAContact contact = obj.LEAContactList.ItemAt( 0 );
            phone = contact.ContactInfo.PhoneNumberList.ItemAt( 0 );
            Assert.Equal("NA", phone.Format);
            Assert.Equal("TE", phone.Type);
            Assert.Equal("912-888-6658", phone.Number);
        }

        [Fact]
        public void testOtherIdsWithValues()
        {
            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='StudentPersonal'>"
                                    + "     <field name='PERMNUM' sifVersion='+2.0'>OtherIdList/OtherId[@Type='0593']</field>"
                                    + "     <field name='PERMNUM' sifVersion='-1.5r1'>OtherId[@Type='06']</field>"
                                    + "     <field name='PERMNUM' sifVersion='+1.5'>LocalId</field>"
                                    + "     <field name='SOCSECNUM' sifVersion='+2.0'>OtherIdList/OtherId[@Type='0004']</field>"
                                    + "     <field name='SOCSECNUM' sifVersion='-1.5r1'>OtherId[@Type='SY']</field>"
                                    + "     <field name='SCHOOLNUM' sifVersion='+2.0'>OtherIdList/OtherId[@Type='9999'+]=SCHOOL:$(SCHOOLNUM)</field>"
                                    + "     <field name='SCHOOLNUM' sifVersion='-1.5r1'>OtherId[@Type='ZZ'+]=SCHOOL:$(SCHOOLNUM)</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "PERMNUM", "123456" );
            map.Add( "SOCSECNUM", "111-555-9987" );
            map.Add( "SCHOOLNUM", "2" );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            StudentPersonal obj = new StudentPersonal();
            doOutboundMapping( sma, obj, customMappings, null );

            OtherIdList list = obj.OtherIdList;
            Assert.NotNull( list);
            OtherId oId = list[ "06" ];
            Assert.NotNull( oId);
            Assert.Equal( "123456", oId.Value);

            oId = list[ "SY"];
            Assert.NotNull( oId);
            Assert.Equal( "111-555-9987", oId.Value);

            oId = list[ "ZZ" ];
            Assert.NotNull( oId);
            Assert.Equal( "SCHOOL:2", oId.Value);
        }

        [Fact]
        public void testOtherIdsNullValues()
        {
            String customMappings = "<agent id='Repro' sifVersion='2.0'>"
                                    + "   <mappings id='Default'>"
                                    + "     <object object='StudentPersonal'>"
                                    +
                                    "     <field name='PERMNUM' sifVersion='+2.0'>OtherIdList/OtherId[@Type='0593']</field>"
                                    + "     <field name='PERMNUM' sifVersion='-1.5r1'>OtherId[@Type='06']</field>"
                                    + "     <field name='PERMNUM' sifVersion='+1.5'>LocalId</field>"
                                    +
                                    "     <field name='SOCSECNUM' sifVersion='+2.0'>OtherIdList/OtherId[@Type='0004']</field>"
                                    + "     <field name='SOCSECNUM' sifVersion='-1.5r1'>OtherId[@Type='SY']</field>"
                                    +
                                    "     <field name='SCHOOLNUM' sifVersion='+2.0'>OtherIdList/OtherId[@Type='9999'+]=SCHOOL:$(SCHOOLNUM)</field>"
                                    +
                                    "     <field name='SCHOOLNUM' sifVersion='-1.5r1'>OtherId[@Type='ZZ'+]=SCHOOL:$(SCHOOLNUM)</field>"
                                    + "</object></mappings></agent>";

            Runtime.SifVersion = SifVersion.SIF15r1;

            IDictionary map = new Hashtable();
            map.Add( "PERMNUM", "123456" );
            map.Add( "SOCSECNUM", null );
            map.Add( "SCHOOLNUM", "2" );
            StringMapAdaptor sma = new StringMapAdaptor( map );
            StudentPersonal obj = new StudentPersonal();
            doOutboundMapping( sma, obj, customMappings, null );

            OtherIdList list = obj.OtherIdList;
            Assert.NotNull( list);
            OtherId oId = list[ "06" ];
            Assert.NotNull( oId);
            Assert.Equal( "123456", oId.Value);

            // The SOCSECNUM was NULL, so it should not produce an element
            oId = list["SY"];
            Assert.Null( oId);

            oId = list[ "ZZ" ];
            Assert.NotNull( oId);
            Assert.Equal( "SCHOOL:2", oId.Value);
        }
    }
}
