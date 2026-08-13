using System;
using System.Collections;
using System.Collections.Specialized;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Food;
using OpenADK.Library.us.Programs;
using OpenADK.Library.us.Student;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Library.Tools.Mapping;
using OpenADK.Library.Tools.XPath;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.Nunit.US.Tools.Mapping
{
    /// <summary>
    /// Summary description for MappingTests.
    /// </summary>
    
    public abstract class MappingTests : BaseMappingsTest
    {
        private SifVersion fVersion;

        private String fFileName;

        private AgentConfig fCfg;

        protected MappingTests( SifVersion testedVersion, String configFileName )
        {
            fVersion = testedVersion;
            fFileName = configFileName;
            InitializeMappingTest();
        }

        
        protected void InitializeMappingTest()
        {
            
            Runtime.SifVersion = fVersion;
            fCfg = new AgentConfig();
            fCfg.Read( fFileName, false );
        }

        /**
	 * Asserts default value field mapping behavior
	 *
	 * @
	 */

        [Fact]
        public void testFieldMapping010()
        {
            StudentPersonal sp = new StudentPersonal();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = buildIDictionaryForStudentPersonalTest();

            // Add a "default" to the middle name rule and assert that it gets
            // created
            ObjectMapping om = mappings.GetObjectMapping( "StudentPersonal", false );
            FieldMapping middleNameRule = om.GetRule( 3 );
            middleNameRule.DefaultValue = "Jerry";
            map.Remove( "MIDDLE_NAME" );

            StringMapAdaptor adaptor = new StringMapAdaptor( map );

            mappings.MapOutbound( adaptor, sp , fVersion );
            SifWriter writer = new SifWriter( Console.Out, Runtime );
            writer.Write( sp );
            writer.Flush();

            // For the purposes of this test, all we care about is the Ethnicity
            // mapping.
            // It should have the default outbound value we specified, which is "7"
            Assert.Equal("Jerry", sp.Name.MiddleName);

            // Now, remap the student back into application fields
            IDictionary restoredData = new Hashtable();
            adaptor.Dictionary = restoredData;
            mappings.MapInbound( sp, adaptor , fVersion );

            Assert.Equal("Jerry", restoredData["MIDDLE_NAME"]);

            sp.Name.LastName = null;
            // Now, remap the student back into application fields
            restoredData = new Hashtable();
            adaptor.Dictionary = restoredData;
            mappings.MapInbound( sp, adaptor , fVersion );

            Object lastName = restoredData["LAST_NAME"];
            Console.WriteLine( sp.ToXml() );
            Assert.Null( lastName);
        }

        /**
	 * Assert that defaults work even with valueset mapping
	 *
	 * @
	 */

        [Fact]
        public void testFieldMapping020()
        {
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = buildIDictionaryForStudentPersonalTest();

            // Find the Ethnicity rule
            ObjectMapping om = mappings.GetObjectMapping( "StudentPersonal", false );
            FieldMapping inboundEthnicityRule = null;
            FieldMapping outboundEthnicityRule = null;
            for ( int a = 0; a < om.RuleCount; a++ )
            {
                FieldMapping fm = om.GetRule( a );
                if ( fm.FieldName.Equals( "ETHNICITY" ) )
                {
                    if ( fm.Filter.Direction == MappingDirection.Inbound )
                    {
                        inboundEthnicityRule = fm;
                    }
                    else
                    {
                        outboundEthnicityRule = fm;
                    }
                }
            }

            // Put a value into the map that won't match the valueset
            map[ "ETHNICITY" ] = "abcdefg" ;
            StudentPersonal sp = new StudentPersonal();
            MapOutbound( sp, mappings, map );

            // There's no default value defined, so we expect that what get's put in
            // is what we get out
            Assert.Equal( "abcdefg", sp.Demographics.RaceList.ItemAt( 0 ).Code);

            // Now, remap the student back into application fields
            IDictionary restoredData = new Hashtable();

            StringMapAdaptor adaptor = new StringMapAdaptor( restoredData );
            mappings.MapInbound( sp, adaptor , fVersion );

            // The value "abcdefg" does not have a match in the value set
            // It should be passed back through
            Assert.Equal( "abcdefg", restoredData["ETHNICITY"]);

            inboundEthnicityRule.DefaultValue = "11111";
            outboundEthnicityRule.DefaultValue = "99999";
            sp = new StudentPersonal();
            MapOutbound( sp, mappings, map );

            // It should have the default value we specified, which is "99999"
            Assert.Equal( "99999", sp.Demographics.RaceList.ItemAt( 0 ).Code);

            // Now, remap the student back into application fields
            restoredData = new Hashtable();
            adaptor.Dictionary = restoredData;
            mappings.MapInbound( sp, adaptor , fVersion );

            // The value "9999" does not have a match in the value set
            // we want it to take on the default value, which is "111111"
            Assert.Equal( "11111", restoredData["ETHNICITY"]);

            // Now, do the mapping again, this time with the Ethnicity value
            // completely missing
            sp = new StudentPersonal();
            map.Remove( "ETHNICITY" );
            MapOutbound( sp, mappings, map );

            // Ethnicity should be set to "99999" in this case because we didn't set
            // the "ifNull" behavior and the
            // default behavior is "NULL_DEFAULT"
            Assert.Equal("99999", sp.Demographics
                                                                       .RaceList.ItemAt( 0 ).Code);

            // Now, do the mapping again, this time with the NULL_DEFALT behavior.
            // The result should be the same
            outboundEthnicityRule.NullBehavior = MappingBehavior.IfNullDefault;
            sp = new StudentPersonal();
            MapOutbound( sp, mappings, map );

            // Ethnicity should be set to "99999" in this case because we set the
            // ifnull behavior to "NULL_DEFAULT"
            Assert.Equal( "99999", sp.Demographics.RaceList.ItemAt( 0 ).Code);

            outboundEthnicityRule.NullBehavior = MappingBehavior.IfNullSuppress;
            sp = new StudentPersonal();
            MapOutbound( sp, mappings, map );

            // Ethnicity should be null in this case because we told it to suppress
            // set the "ifNull" behavior
            Assert.Null( sp.Demographics.RaceList);
        }

        /**
	 * Asserts default value field mapping behavior
	 *
	 * @
	 */

        [Fact]
        public void testStudentContactMapping010()
        {
            StudentContact sc = new StudentContact();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IFieldAdaptor adaptor = createStudentContactFields();

            mappings.MapOutbound( adaptor, sc , fVersion );
            SifWriter writer = new SifWriter( Console.Out, Runtime );
            writer.Write( sc );
            writer.Flush();

            Assert.Equal("Yes", sc.ContactFlags.PickupRights);
        }

        /**
	 * @param sp
	 * @param mappings
	 * @param map
	 * @throws AdkMappingException
	 */

        private void MapOutbound( StudentPersonal sp, Mappings mappings, IDictionary map )
        {
            StringMapAdaptor adaptor = new StringMapAdaptor( map );
            mappings.MapOutbound( adaptor, sp , fVersion );

            SifWriter writer = new SifWriter( Console.Out, Runtime );
            writer.Write( sp );
            writer.Flush();
            writer.Close();
        }

        [Fact]
        public void testValueSetMapping010()
        {
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            ValueSet vs = mappings.GetValueSet( "Ethnicity", false );

            // Normal Translation
            Assert.Equal("A", vs.Translate( "1" ));
            Assert.Equal( "1", vs.TranslateReverse( "A" ));
            // Default Values
            Assert.True("ZZZ" == vs.Translate( "foo", "ZZZ" ), "Translate");
            Assert.True( "AAA" == vs.TranslateReverse( "foo", "AAA" ), "TranslateReverse" );

            // Test Null behavior
            Assert.Null( vs.Translate( null ));
            Assert.Null( vs.TranslateReverse( null ));

            // No Match (this time should return what we pass in)
            Assert.Equal("QQQQ", vs.Translate( "QQQQ" ));
            Assert.Equal("QQQQ", vs.TranslateReverse( "QQQQ" ));

            // //////////////////////////////////
            //
            // Add an app default
            //
            // //////////////////////////////////
            vs.SetAppDefault( "6", false );

            // Normal Translation
            Assert.Equal("A", vs.Translate( "1" ));
            Assert.Equal( "1", vs.TranslateReverse( "A" ));
            // Default Values
            Assert.True("ZZZ" == vs.Translate( "foo", "ZZZ" ), "Translate");
            Assert.True( "AAA" == vs.TranslateReverse( "foo", "AAA" ), "TranslateReverse" );

            // Test Null behavior
            Assert.Null( vs.Translate( null ));
            Assert.Null( vs.TranslateReverse( null ));

            // No Match (this time should return a default for app value)
            Assert.Equal("QQQQ", vs.Translate( "QQQQ" ));
            Assert.Equal( "6", vs.TranslateReverse( "QQQQ" ));

            // ////////////////////////////////
            //
            // Add a SIF default
            //
            // //////////////////////////////////
            vs.SetSifDefault( "H", false );

            // Normal Translation
            Assert.Equal("A", vs.Translate( "1" ));
            Assert.Equal( "1", vs.TranslateReverse( "A" ));
            // Default Values
            Assert.True("ZZZ" == vs.Translate( "foo", "ZZZ" ), "Translate");
            Assert.True( "AAA" == vs.TranslateReverse( "foo", "AAA" ), "TranslateReverse" );

            // Test Null behavior
            Assert.Null( vs.Translate( null ));
            Assert.Null( vs.TranslateReverse( null ));

            // No Match (this time should return a default for app value and sif
            // value)
            Assert.Equal( "H", vs.Translate( "QQQQ" ));
            Assert.Equal( "6", vs.TranslateReverse( "QQQQ" ));

            // //////////////////////////////////
            //
            // Change the App and SIF Value and set it to render if null
            //
            // //////////////////////////////////
            vs.SetSifDefault( "C", true );
            vs.SetAppDefault( "7", true );

            // Normal Translation
            Assert.Equal("A", vs.Translate( "1" ));
            Assert.Equal( "1", vs.TranslateReverse( "A" ));
            // Default Values
            Assert.True("ZZZ" == vs.Translate( "foo", "ZZZ" ), "Translate");
            Assert.True( "AAA" == vs.TranslateReverse( "foo", "AAA" ), "TranslateReverse" );

            // Test Null behavior
            Assert.Equal( "C", vs.Translate( null ));
            Assert.Equal( "7", vs.TranslateReverse( null ));

            // No Match (this time should return a default for app value and sif
            // value)
            Assert.Equal( "C", vs.Translate( "QQQQ" ));
            Assert.Equal( "7", vs.TranslateReverse( "QQQQ" ));
        }

        /**
	 * This test creates a StudentPersonal object and then builds it out using
	 * outbound mappings.
	 *
	 * Then, it uses the mappings again to rebuild a new Hashtable. The
	 * Hashtables are then compared to ensure that the resulting data is
	 * identical
	 *
	 * @
	 */

        [Fact]
        public void testStudentMappingAdk15Mappings()
        {
            StudentPersonal sp = new StudentPersonal();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = buildIDictionaryForStudentPersonalTest();

            StringMapAdaptor adaptor = new StringMapAdaptor( map );
            mappings.MapOutbound( adaptor, sp , fVersion );

            SifWriter writer = new SifWriter( Console.Out, Runtime );
            writer.Write( sp );
            writer.Flush();

            // Assert that the StudentPersonal object was mapped correctly
            assertStudentPersonal( sp );

            // Now, map the student personal back to a hashmap and assert it
            IDictionary restoredData = new Hashtable();
            adaptor.Dictionary = restoredData;
            mappings.MapInbound( sp, adaptor , fVersion );
            assertMapsAreEqual( map, restoredData, "ALT_PHONE_TYPE" );
        }

        /**
	 * This test creates a StudentPlacement object and then builds it out using
	 * outbound mappings.
	 *
	 * Then, it uses the mappings again to rebuild a new Hashtable. The
	 * hashtables are then compared.
	 *
	 * @
	 */

        [Fact]
        public void testStudentPlacementMapping()
        {
            StudentPlacement sp = new StudentPlacement();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = buildIDictionaryForStudentPlacementTest();
            StringMapAdaptor sma = new StringMapAdaptor( map );
            mappings.MapOutbound( sma, sp , fVersion );

            sp = (StudentPlacement) AdkObjectParseHelper.WriteParseAndReturn( sp, fVersion );

            // Assert that the StudentPlacement object was mapped correctly
            assertStudentPlacement( sp );

            // Now, map the StudentPlacement back to a hashmap and assert it
            IDictionary restoredData = new Hashtable();
            sma = new StringMapAdaptor( restoredData );
            mappings.MapInbound( sp, sma , fVersion );
            assertMapsAreEqual( map, restoredData );
        }

        /**
	 * This test creates a StudentMeal object and then builds it out using
	 * outbound mappings.
	 *
	 * Then, it uses the mappings again to rebuild a new Hashtable. The
	 * hashtables are then compared.
	 *
	 * @
	 */

        [Fact]
        public void testStudentMealMappings()
        {
            StudentMeal sm = new StudentMeal();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = new Hashtable();
            map.Add( "Balance", "10.55" );

            StringMapAdaptor sma = new StringMapAdaptor( map );
            mappings.MapOutbound( sma, sm , fVersion );

            sm = (StudentMeal) AdkObjectParseHelper.WriteParseAndReturn( sm, fVersion );

            // Assert that the object was mapped correctly
            FSAmounts amounts = sm.Amounts;
            Assert.NotNull( amounts );
            FSAmount amount = amounts.ItemAt( 0 );
            Assert.True( amount.Value.HasValue );
            Assert.Equal(10.55m, amount.Value.Value);


            // Now, map the object back to a hashmap and assert it
            IDictionary restoredData = new Hashtable();
            sma = new StringMapAdaptor( restoredData );
            mappings.MapInbound( sm, sma , fVersion );
            assertMapsAreEqual( map, restoredData );
        }

        /**
	 * This test creates a SectionInfo object and then builds it out using
	 * outbound mappings.
	 *
	 * Then, it uses the mappings again to rebuild a new Hashtable. The
	 * hashtables are then compared.
	 *
	 * @
	 */

        [Fact]
        public void testSectionInfoMappings()
        {
            SectionInfo si = new SectionInfo();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = new Hashtable();
            map.Add( "STAFF_REFID", "123456789ABCDEF" );

            StringMapAdaptor sma = new StringMapAdaptor( map );
            mappings.MapOutbound( sma, si , fVersion );

            si = (SectionInfo) AdkObjectParseHelper.WriteParseAndReturn( si, fVersion );

            // Assert that the object was mapped correctly
            ScheduleInfoList sil = si.ScheduleInfoList;
            Assert.NotNull( sil );
            ScheduleInfo schedule = sil.ItemAt( 0 );
            Assert.NotNull( schedule );
            TeacherList tl = schedule.TeacherList;
            Assert.NotNull( tl );
            StaffPersonalRefId refId = tl.ItemAt( 0 );
            Assert.NotNull( refId );
            Assert.Equal( "123456789ABCDEF", refId.Value );


            // Now, map the object back to a hashmap and assert it
            IDictionary restoredData = new Hashtable();
            sma = new StringMapAdaptor( restoredData );
            mappings.MapInbound( si, sma , fVersion );
            assertMapsAreEqual( map, restoredData );
        }


        protected abstract IDictionary buildIDictionaryForStudentPlacementTest();


        protected abstract void assertStudentPlacement( StudentPlacement sp );


        private void assertByXPath( SifXPathContext context, String xPath,
                                    String assertedValue )
        {
            Element e = (Element) context.GetValue( xPath );
            Assert.NotNull(e);
            SifSimpleType value = e.SifValue;
            Assert.NotNull(value);
            Assert.Equal(assertedValue, value.ToString());
        }

        protected StringMapAdaptor createStudentContactFields()
        {
            IDictionary values = new Hashtable();
            values.Add( "APRN.SOCSECNUM", "123456789" );
            values.Add( "APRN.SCHOOLNUM", "999" );
            values.Add( "APRN.SCHOOLNUM2", "999" );
            values.Add( "APRN.EMAILADDR", null );
            values.Add( "APRN.LASTNAME", "DOE" );
            values.Add( "APRN.FIRSTNAME", "JOHN" );
            values.Add( "APRN.MIDDLENAME", null );
            values.Add( "APRN.WRKADDR", null );
            values.Add( "APRN.WRKCITY", null );
            values.Add( "APRN.WRKSTATE", null );
            values.Add( "APRN.WRKCOUNTRY", null );
            values.Add( "APRN.WRKZIP", "54494" );
            values.Add( "APRN.ADDRESS", null );
            values.Add( "APRN.CITY", null );
            values.Add( "APRN.STATE", null );
            values.Add( "APRN.COUNTRY", null );
            values.Add( "APRN.ZIPCODE", null );
            values.Add( "APRN.TELEPHONE", "8014504555" );
            values.Add( "APRN.ALTTEL", "8014505555" );
            values.Add( "APRN.WRKTEL", null );
            values.Add( "APRN.WRKEXTN", null );
            values.Add( "APRN.RELATION", "01" );
            values.Add( "APRN.PICKUPRIGHTS", "Yes" );

            StringMapAdaptor sma = new StringMapAdaptor( values );
            return sma;
        }

        [Fact]
        public void StudentMapping()
        {
            StudentPersonal sp = new StudentPersonal();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = buildIDictionaryForStudentPersonalTest();
            StringMapAdaptor sma = new StringMapAdaptor( map );
            mappings.MapOutbound( sma, sp , fVersion );

            SifWriter writer = new SifWriter( Console.Out, Runtime );
            writer.Write( sp );
            writer.Flush();

            // Assert that the StudentPersonal object was mapped correctly
            assertStudentPersonal( sp );

            // Now, map the student personal back to a hashmap and assert it
            IDictionary restoredData = new HybridDictionary();
            StringMapAdaptor restorer = new StringMapAdaptor( restoredData );
            mappings.MapInbound( sp, restorer , fVersion );
            assertDictionariesAreEqual( restoredData, map );
        }

        [Fact]
        public void StudentPlacementMapping()
        {
            StudentPlacement sp = new StudentPlacement();
            Mappings mappings = fCfg.Mappings.GetMappings( "Default" );
            IDictionary map = buildIDictionaryForStudentPlacementTest();
            StringMapAdaptor sma = new StringMapAdaptor( map );
            mappings.MapOutbound( sma, sp , fVersion );

            SifWriter writer = new SifWriter( Console.Out, Runtime );
            writer.Write( sp );
            writer.Flush();

            // Assert that the StudentPersonal object was mapped correctly
            assertStudentParticipation( sp );

            // Now, map the student personal back to a hashmap and assert it
            IDictionary restoredData = new HybridDictionary();
            StringMapAdaptor restorer = new StringMapAdaptor( restoredData );
            mappings.MapInbound( sp, restorer , fVersion );
            assertDictionariesAreEqual( map, restoredData );
        }


        private IDictionary buildIDictionaryForStudentPersonalTest()
        {
            IDictionary data = new Hashtable();
            data.Add( "STUDENT_NUM", "998" );
            data.Add( "LAST_NAME", "Johnson" );
            data.Add( "MIDDLE_NAME", "George" );
            data.Add( "FIRST_NAME", "Betty" );
            data.Add( "BIRTHDATE", "19900101" );
            data.Add( "ETHNICITY", "4" );
            data.Add( "HOME_PHONE", "202-358-6687" );
            data.Add( "CELL_PHONE", "202-502-4856" );
            data.Add( "ALT_PHONE", "201-668-1245" );
            data.Add( "ALT_PHONE_TYPE", "TE" );

            data.Add( "ACTUALGRADYEAR", "2007" );
            data.Add( "ORIGINALGRADYEAR", "2005" );
            data.Add( "PROJECTEDGRADYEAR", "2007" );

            data.Add( "ADDR1", "321 Oak St" );
            data.Add( "ADDR2", "APT 11" );
            data.Add( "CITY", "Metropolis" );
            data.Add( "STATE", "IL" );
            data.Add( "COUNTRY", "US" );
            data.Add( "ZIPCODE", "321546" );

            return data;
        }


        private void assertStudentParticipation( StudentPlacement sp )
        {
            Assert.Equal( "0000000000000000", sp.RefId);
            Assert.Equal("0000000000000000", sp.StudentPersonalRefId);
            if (sp.SifVersion >= SifVersion.SIF20)
            {
                Assert.Equal("ZZZ99987", sp.Service.Code);
            }
            else
            {
                Assert.Equal("Local", sp.Service.CodeType);
                Assert.Equal("Related Service", sp.Service.Type);
                Assert.Equal("ZZZ99987", sp.Service.TextValue);
            }
        }


        private void assertStudentPersonal( StudentPersonal sp )
        {
            DateTime birthDate = new DateTime( 1990, 1, 1 );

            Assert.Equal( "Betty", sp.Name.FirstName);
            Assert.Equal( "George", sp.Name.MiddleName);
            Assert.Equal( "Johnson", sp.Name.LastName);
            Assert.Equal( "998", sp.OtherIdList.ItemAt( 0 ).TextValue);
            Assert.Equal( birthDate, sp.Demographics.BirthDate.Value);
            Assert.Equal( "H", sp.Demographics.RaceList.ItemAt( 0 ).Code);

            PhoneNumberList pnl = sp.PhoneNumberList;
            Assert.NotNull(pnl);

            PhoneNumber homePhone = pnl[PhoneNumberType.SIF1x_HOME_PHONE];
            Assert.NotNull(homePhone);
            Assert.Equal( "202-358-6687", homePhone.Number);

            PhoneNumber cellPhone = pnl
                [PhoneNumberType.SIF1x_PERSONAL_CELL];
            Assert.NotNull( cellPhone);
            Assert.Equal( "202-502-4856", cellPhone.Number);

            SifXPathContext xpathContext = SifXPathContext.NewSIFContext( sp, SifVersion.SIF20r1 );
            assertByXPath( xpathContext, "AddressList/Address/Street/Line1",
                           "321 Oak St" );
            assertByXPath( xpathContext, "AddressList/Address/Street/Line1",
                           "321 Oak St" );
            assertByXPath( xpathContext, "AddressList/Address/Street/Line2",
                           "APT 11" );
            assertByXPath( xpathContext, "AddressList/Address/City", "Metropolis" );
            assertByXPath( xpathContext, "AddressList/Address/StateProvince", "IL" );
            assertByXPath( xpathContext, "AddressList/Address/Country", "US" );
            assertByXPath( xpathContext, "AddressList/Address/PostalCode", "321546" );

            /*
             * These assertions are currently commented out because the Adk does not
             * currently support Repeatable elements that have wildcard attributes
             *
             * PhoneNumber number = sp.PhoneNumber( PhoneNumberType.PHONE );
             * Assert.NotNull(number);
             * Assert.Equal("201-668-1245", number.ToString());
             */

            Assert.Equal(2005, sp.OnTimeGraduationYear.Value);
            Assert.Equal(2007, sp.ProjectedGraduationYear.Value);
            Assert.NotNull( sp.GraduationDate.Value);
            Assert.Equal( 2007, sp.GraduationDate.Year);
        }


        private void assertDictionariesAreEqual( IDictionary map1, IDictionary map2 )
        {
            foreach ( Object key in map1.Keys )
            {
                Assert.True( map1[key].Equals(map2[key]), key.ToString() );
            }
        }
    }
}
