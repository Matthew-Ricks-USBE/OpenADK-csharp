using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using OpenADK.Library;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Library.Tools.Mapping;
using NUnit.Framework;

namespace Library.Nunit.US.Library.Tools.Mapping
{
    [TestFixture]
    public class MappingsObjectTests
    {
        private AgentConfig fCfg;

        [SetUp]
        public virtual void setUp()
        {
            Adk.Initialize();
            fCfg = new AgentConfig();
            fCfg.Read( "..\\..\\OpenADK\\Tools\\Mapping\\SIF1.5.agent.cfg",
                       false );
        }

        /**
	 * This test "builds up" a set of mappings programmatically, writes it to
	 * disk and the reads it again. The assertions assert that all of the
	 * attributes of each associated part of the mappings hierarchy is
	 * preserved.
	 */

        [Test]
        public void testReadAndWriteMappings()
        {
            String FILE_NAME = "tmp.cfg";

            // TODO: Right now we don't have a way to build up a Mappings hierarchy
            // without reading it from a file. This should be fixed and this
            // test should be updated as a result.
            AgentConfig cfg = createMappings();

            debug( cfg.Document );

            // save the mappings to a file
            FileInfo f = new FileInfo( FILE_NAME );
            if ( f.Exists )
            {
                f.Delete();
            }

            using ( StreamWriter fs = new StreamWriter( FILE_NAME ) )
            {
                try
                {
                    cfg.Save( fs );
                }
                finally
                {
                    fs.Close();
                }
            }

            // Read the new mappings
            cfg = new AgentConfig();
            cfg.Read( FILE_NAME, false );

            Mappings reparsed = cfg.Mappings;
            assertMappings( reparsed );
        }

        /**
	 * Returns a new mappings root, with the passed-in Mappings copied into it
	 * as a child, obtained by using the Mappings.copy() method
	 * 
	 * @param mappingSet
	 *            The set of mappings to be copied into the new root
	 * @return
	 */

        private Mappings getCopy( Mappings mappingSet )
        {
            XmlDocument dom = new XmlDocument();
            dom.LoadXml( "<mappings/>" );
            Mappings newRoot = new Mappings();
            newRoot.XmlElement = dom.DocumentElement;
            mappingSet.Copy( newRoot );

            debug( dom );

            return newRoot;
        }

        /**
	 * Writes the DOM to System.out
	 * 
	 * @param dom
	 */

        private void debug( XmlDocument dom )
        {
            Console.WriteLine( dom.DocumentElement.OuterXml );
        }

        /**
	 * Builds up a mappings hierarchy, and then calls the "copy" method to copy
	 * the hierarchy. The copy is then asserted to ensure it has the proper
	 * behavior.
	 * 
	 * @throws AdkException
	 */

        [Test]
        public void testCopyMappings()
        {
            AgentConfig cfg = createMappings();
            Mappings m = cfg.Mappings;
            Mappings newRoot = getCopy( m.GetMappings( "Test" ) );
            assertMappings( newRoot );
        }

        [Test]
        public void testCreateMappings()
        {
            // Create the root instance
            Mappings root = new Mappings();
            // Create the default set of universal mappings
            Mappings defaults = new Mappings( root, "Default" );
            root.AddChild( defaults );

            // Create an ObjectMapping for StudentPersonal
            ObjectMapping studentMappings = new ObjectMapping( "StudentPersonal" );
            defaults.AddRules( studentMappings );
            // Add field rules
            studentMappings.AddRule( new FieldMapping( "FIRSTNAME",
                                                       "Name[@Type='04']/FirstName" ) );
            studentMappings.AddRule( new FieldMapping( "LASTNAME",
                                                       "Name[@Type='04']/LastName" ) );

            // Create a set of mappings for the state of Wisconsin
            Mappings wisconsin = new Mappings( defaults, "Wisconsin" );
            defaults.AddChild( wisconsin );
            // Create a set of mappings for the Neillsville School District
            Mappings neillsville = new Mappings( wisconsin, "Neillsville" );

            wisconsin.AddChild( neillsville );

            XmlDocument doc = new XmlDocument( );
            doc.LoadXml( "<agent/>");

            XmlElement n = root.ToDom( doc );
            debug( doc );
        }

        /**
	 * Builds up a mappings hierarchy, and then calls the toDOM() method to copy
	 * the hierarchy. The copy is then read back into a new Mappings object and
	 * asserted to ensure it has the proper behavior.
	 * 
	 * @throws AdkException
	 */

        [Test]
        public void testCopyMappingsThroughDOM()
        {
            AgentConfig cfg = createMappings();
            Mappings m = cfg.Mappings;
            Mappings newRoot = getCopyFromDOM( m.GetMappings( "Test" ) );
            assertMappings( newRoot );
        }

        [Test]
        public void testCopyMappingsThroughCopyThenDOM()
        {
            AgentConfig cfg = createMappings();
            Mappings m = cfg.Mappings;
            Mappings newRoot = getCopy( m.GetMappings( "Test" ) );
            newRoot= getCopyFromDOM( newRoot.GetMappings( "Test") );
            assertMappings(newRoot);
        }



        /**
	 * Returns a new mappings root, with the passed-in Mappings copied into it
	 * as a child, obtained by using the Mappings.copy() method
	 * 
	 * @param root
	 *            The set of mappings to be copied into the new root
	 * @return
	 */

        private Mappings getCopyFromDOM( Mappings mappingSet )
        {
            XmlDocument dom = new XmlDocument();
            dom.LoadXml( "<agent/>" );
            XmlNode mappingsNode = dom.ImportNode( mappingSet.XmlElement, true );
            dom.DocumentElement.AppendChild( mappingsNode );

            Mappings newRoot = new Mappings();
            newRoot.Populate( dom, (XmlElement) mappingsNode );
            return newRoot;
        }

        /**
	 * Creates a set of mappings that operations can be applied to, such as
	 * saving to a DOM or Agent.cfg. The results can be asserted by calling
	 * {@see #assertMappings(Mappings)}.
	 * 
	 * NOTE: This method returns an AgentConfig instance instead of a mappings
	 * instance because there is no way set the Mappings instance on
	 * AgentConfig. This might change in the future
	 * 
	 * @return
	 */

        private AgentConfig createMappings()
        {
            Mappings root = fCfg.Mappings;
            // Remove the mappings being used
            root.RemoveChild( root.GetMappings( "Default" ) );
            root.RemoveChild( root.GetMappings( "TestID" ) );

            Mappings newMappings = root.CreateChild( "Test" );

            // Add an object mapping
            ObjectMapping objMap = new ObjectMapping( "StudentPersonal" );
            // Currently, the Adk code requires that an Object Mapping be added
            // to it's parent before fields are added.
            // We should re-examine this and perhaps fix it, if possible
            newMappings.AddRules( objMap );

            objMap.AddRule( new FieldMapping( "FIELD1", "Name/FirstName" ) );

            // Field 2
            FieldMapping field2 = new FieldMapping( "FIELD2", "Name/LastName" );
            field2.ValueSetID = "VS1";
            field2.Alias = "ALIAS1";
            field2.DefaultValue = "DEFAULT1";
            MappingsFilter mf = new MappingsFilter();
            mf.Direction = MappingDirection.Inbound;
            mf.SifVersion = SifVersion.SIF11.ToString();
            field2.Filter = mf;
            objMap.AddRule( field2 );

            // Field 3 test setting the XML values after it's been added to the
            // parent object (the code paths are different)
            FieldMapping field3 = new FieldMapping( "FIELD3", "Name/MiddleName" );
            objMap.AddRule( field3 );
            field3.ValueSetID = "VS2";
            field3.Alias = "ALIAS2";
            field3.DefaultValue = "DEFAULT2";
            MappingsFilter mf2 = new MappingsFilter();
            mf2.Direction = MappingDirection.Outbound;
            mf2.SifVersion = SifVersion.SIF15r1.ToString();
            field3.Filter = mf2;
            field3.NullBehavior = MappingBehavior.IfNullDefault;

            OtherIdMapping oim = new OtherIdMapping( "ZZ", "BUSROUTE" );
            FieldMapping field4 = new FieldMapping( "FIELD4", oim );
            objMap.AddRule( field4 );
            field4.DefaultValue = "Default";
            field4.ValueSetID = "vs";
            field4.Alias = "alias";
            field4.DefaultValue = null;
            field4.ValueSetID = null;
            field4.Alias = null;
            field4.NullBehavior = MappingBehavior.IfNullSuppress;

            // Field4 tests the new datatype attribute
            FieldMapping field5 = new FieldMapping( "FIELD5",
                                                    "Demographics/BirthDate" );
            objMap.AddRule( field5 );
            field5.DataType = SifDataType.Date;

            // Add a valueset translation
            ValueSet vs = new ValueSet( "VS1" );
            newMappings.AddValueSet( vs );
            // Add a few definitions
            for ( int a = 0; a < 10; a++ )
            {
                vs.Define( "Value" + a, "SifValue" + a, "Title" + a );
            }

            vs.Define( "AppDefault", "0000", "Default App Value" );
            vs.SetAppDefault( "AppDefault", true );

            vs.Define( "0000", "SifDefault", "Default Sif Value" );
            vs.SetSifDefault( "SifDefault", false );

            // Add a valueset translation
            vs = new ValueSet( "VS2" );
            newMappings.AddValueSet( vs );
            // Add a few definitions
            for ( int a = 0; a < 3; a++ )
            {
                vs.Define( "q" + a, "w" + a, "t" + a );
            }

            vs.Define( "AppDefault", "0000", "Default Value" );
            vs.SetAppDefault( "AppDefault", true );
            vs.SetSifDefault( "0000", true );

            return fCfg;
        }

        /**
	 * Asserts that the provided set of Mappings matches the one that was
	 * created in createMappings();
	 */

        private void assertMappings( Mappings m )
        {
            Mappings test = m.GetMappings( "Test" );
            Assert.IsNotNull(test, "Test mappings is not present");
            Assert.AreEqual(1, test.GetObjectMappings().Length, "Should have a single Object Mapping");

            // TODO: Test the version and sourceId filters more carefully
            /*
		 * Assert.AreEqual(0, test.SifVersionFilter().Length,  "SifVersion attr should be empty");
         * Assert.AreEqual(0, test.SourceIdFilter().Length, "SourceId attr should be empty");
         * Assert.AreEqual(0, test.ZoneIdFilter().Length, "Zone attr should be empty");
		 */

            // assert the object mapping
            ObjectMapping om = test.GetObjectMapping( "StudentPersonal", false );
            Assert.IsNotNull(om, "StudentPersonal mappings is not present");
            Assert.AreEqual(5, om.RuleCount, "There should be five rules");
            IList<FieldMapping> rules = om.GetRulesList( false );

            // Field 1
            Assert.AreEqual("Name/FirstName", rules[0].GetRule().ToString(), "FIELD1 rule");
            Assert.AreEqual("FIELD1", rules[0].FieldName, "FIELD1 name");
            Assert.AreEqual(MappingBehavior.IfNullUnspecified, rules[0].NullBehavior, "FIELD1 ifNull");

            // Field 2
            Assert.AreEqual("FIELD2", rules[1].FieldName, "FIELD2 name");
            Assert.AreEqual("Name/LastName", rules[1].GetRule().ToString(), "FIELD2 rule");
            Assert.AreEqual("VS1", rules[1].ValueSetID, "FIELD2 valueset");
            Assert.AreEqual("ALIAS1", rules[1].Alias, "FIELD2 alias");
            Assert.AreEqual("DEFAULT1", rules[1].DefaultValue, "FIELD2 default");
            MappingsFilter filter = rules[1].Filter;
            Assert.IsNotNull(filter, "FIELD2 filter is null");
            Assert.AreEqual(MappingDirection.Inbound, filter.Direction, "filter direction");
            Assert.AreEqual("=" + SifVersion.SIF11.ToString(), filter.SifVersion, "filter sif version");

            // Field 3
            Assert.AreEqual("FIELD3", rules[2].FieldName, "FIELD3 name");
            Assert.AreEqual("Name/MiddleName", rules[2].GetRule().ToString(), "FIELD3 rule");
            Assert.AreEqual("VS2", rules[2].ValueSetID, "FIELD3 valueset");
            Assert.AreEqual("ALIAS2", rules[2].Alias, "FIELD3 alias");
            Assert.AreEqual("DEFAULT2", rules[2].DefaultValue, "FIELD3 default");
            Assert.AreEqual(MappingBehavior.IfNullDefault, rules[2].NullBehavior, "FIELD3 ifNull");
            MappingsFilter filter2 = rules[2].Filter;
            Assert.IsNotNull(filter2, "FIELD3 filter is null");
            Assert.AreEqual(MappingDirection.Outbound, filter2.Direction, "filter2 direction");
            Assert.AreEqual("=" + SifVersion.SIF15r1.ToString(), filter2.SifVersion, "filter2 sif version");

            // Field 4
            Assert.AreEqual("FIELD4", rules[3].FieldName, "FIELD4 name");
            Assert.IsNull( rules[3].ValueSetID, "FIELD4 valueset" );
            Assert.IsNull( rules[3].Alias, "FIELD4 alias" );
            Assert.IsNull( rules[3].DefaultValue, "FIELD4 default" );
            Assert.AreEqual( MappingBehavior.IfNullSuppress, rules[3].NullBehavior, "FIELD4 ifNull" );
            Rule r = rules[3].GetRule();
            Assert.True(r is OtherIdRule, "Rule should be OtherIdRule");

            Assert.AreEqual("FIELD5", rules[4].FieldName, "FIELD5 name");
            Assert.AreEqual(SifDataType.Date, rules[4].DataType, "FIELD5 datatype");

            // TODO: The OtherIdRule doesn't have an API to get at the
            // OtherIdMapping. For now, just
            // convert it to a string and assert the results
            String ruleStr = r.ToString();
            Assert.True(ruleStr.IndexOf( "prefix='BUSROUTE'" ) > 1, "prefix should be BUSROUTE");
            Assert.True(ruleStr.IndexOf( "type='ZZ'" ) > 1, "type should be ZZ");

            ValueSet vs = test.GetValueSet( "VS1", false );
            Assert.IsNotNull(vs, "ValueSet VS1 should not be null");
            Assert.AreEqual(12, vs.Entries.Length, "VS1 should have 12 entries");
            for ( int a = 0; a < 10; a++ )
            {
                Assert.AreEqual("SifValue" + a, vs.Translate( "Value" + a ), "Mapping by appvalue");
                Assert.AreEqual("Value" + a, vs.TranslateReverse( "SifValue" + a ), "Mapping by sifvalue");
            }
            // Test the default value entries
            Assert.AreEqual( "AppDefault", vs.TranslateReverse( "abcdefg" ), "Expecting app default value" );
            Assert.AreEqual( "AppDefault", vs.TranslateReverse( null ), "Expecting app default value" );
            Assert.AreEqual( "SifDefault", vs.Translate( "abcdefg" ), "Expecting sif default value" );
            Assert.IsNull( vs.Translate( null ), "Expecting NULL value" );

            vs = test.GetValueSet( "VS2", false );
            Assert.IsNotNull( vs, "ValueSet VS2 should not be null" );
            Assert.AreEqual( 4, vs.Entries.Length, "VS2 should have 4 entries" );
            for ( int a = 0; a < 3; a++ )
            {
                Assert.AreEqual( "w" + a, vs.Translate( "q" + a ), "Mapping by appvalue" );
                Assert.AreEqual( "q" + a, vs
                                                                            .TranslateReverse( "w" + a ), "Mapping by sifvalue" );
            }
            // Test the default value entries
            Assert.AreEqual( "AppDefault", vs
                                                                                     .TranslateReverse( "abcdefg" ), "Expecting app default value" );
            Assert.AreEqual( "AppDefault", vs
                                                                                     .TranslateReverse( null ), "Expecting app default value" );
            Assert.AreEqual( "0000", vs
                                                                               .Translate( "abcdefg" ), "Expecting sif default value" );
            Assert.AreEqual( "0000", vs.Translate( null ), "Expecting sif default value" );
        }
    }
}
