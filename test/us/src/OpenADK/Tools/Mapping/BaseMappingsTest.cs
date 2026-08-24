using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OpenADK.Library;
using OpenADK.Library.us.Student;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Library.Tools.Mapping;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.xUnit.US.Tools.Mapping
{
    public abstract class BaseMappingsTest : AdkTest
    {
        private void writeConfig( string configFileText_in, String fileName )
        {
            using ( StreamWriter writer = new StreamWriter( fileName, false ) )
            {
                writer.Write( configFileText_in );
                writer.Close();
            }
        }

        protected StudentPersonal mapToStudentPersonal( IFieldAdaptor adaptor, String cfg, IValueBuilder vb )
        {
            StudentPersonal sp = new StudentPersonal();
            sp.SifVersion = Runtime.SifVersion;
            doOutboundMapping( adaptor, sp, cfg, vb );
            return sp;
        }

        protected void doOutboundMapping( IFieldAdaptor adaptor, SifDataObject sdo, String cfg, IValueBuilder vb )
        {
            AgentConfig config = createConfig( cfg );

            Mappings root = config.Mappings;
            Mappings defMap = root.GetMappings( "Default" );

            if ( vb != null )
            {
                defMap.MapOutbound( adaptor, sdo, vb, Runtime.SifVersion );
            }
            else
            {
                defMap.MapOutbound( adaptor, sdo, Runtime.SifVersion );
            }

            Console.WriteLine( sdo.ToXml() );
        }

        protected IDictionary doInboundMapping( String cfg, StudentPersonal sp )
        {
            AgentConfig config = createConfig( cfg );

            Mappings root = config.Mappings;
            Mappings defMap = root.GetMappings( "Default" );

            Dictionary<String, String> result = new Dictionary<string, string>();
            defMap.MapInbound( sp, new StringMapAdaptor( result ), Runtime.SifVersion );

            return result;
        }

        protected StudentPersonal doOutboundMappingSelect( IFieldAdaptor adaptor, String cfg, String zoneId,
                                                           String sourceId, SifVersion version )
        {
            version ??= Runtime.SifVersion;
            AgentConfig config = createConfig( cfg );

            Mappings root = config.Mappings;
            Mappings defMap = root.GetMappings( "Default" );
            Mappings selectedMap = defMap.Select( zoneId, sourceId, version );

            StudentPersonal sp = Objects.Create<StudentPersonal>();
            sp.SifVersion = version;
            selectedMap.MapOutbound( adaptor, sp, version );

            Console.WriteLine( sp.ToXml() );
            return sp;
        }

        private AgentConfig createConfig( String cfg )
        {
            String fileName = "AdvancedMappings.cfg";
            writeConfig( cfg, fileName );
            AgentConfig config = new AgentConfig();
            config.Read( fileName, false );
            return config;
        }

        /**
         * Asserts that the result map has the same values as the source map
         * @param expected
         * @param result
         */

        protected void assertMapsAreEqual( IDictionary expected, IDictionary result, params String[] excluded )
        {
            foreach ( DictionaryEntry entry in expected )
            {
                Object resultingValue = result[entry.Key];
                if ( resultingValue == null )
                {
                    bool wasExcluded = false;
                    if ( excluded != null )
                    {
                        foreach ( String s in excluded )
                        {
                            if ( s.Equals( entry.Key ) )
                            {
                                wasExcluded = true;
                                break;
                            }
                        }
                    }
                    if ( wasExcluded )
                    {
                        continue;
                    }
                }
                Assert.True(Equals(entry.Value, resultingValue), (string)entry.Key);
            }
        }
    }
}
