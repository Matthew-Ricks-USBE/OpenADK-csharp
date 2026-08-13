using System;
using OpenADK.Library;
using Xunit;
using Library.UnitTesting.Framework;
using OpenADK.Library.us;

namespace Library.Nunit.Core
{
    
    public class SifVersionTests : AdkTest
    {
        [Fact]
        public void testAllOfficiallySupportedVersions()
        {
            SifVersion[] versions = Runtime.SupportedSIFVersions;

            assertSIFVersion(SifVersion.Parse("1.1"), 1, 1, 0);
            Assert.Equal(SifVersion.SIF11, SifVersion.Parse("1.1"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF11));
            Assert.Equal(0, Array.BinarySearch(versions, SifVersion.SIF11));


            assertSIFVersion(SifVersion.Parse("1.5r1"), 1, 5, 1);
            Assert.Equal(SifVersion.SIF15r1, SifVersion.Parse("1.5r1"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF15r1));
            Assert.Equal(1, Array.BinarySearch(versions, SifVersion.SIF15r1));

            assertSIFVersion(SifVersion.Parse("2.0"), 2, 0, 0);
            Assert.Equal(SifVersion.SIF20, SifVersion.Parse("2.0"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF20));
            Assert.Equal(2, Array.BinarySearch(versions, SifVersion.SIF20));

            assertSIFVersion(SifVersion.Parse("2.0r1"), 2, 0, 1);
            Assert.Equal(SifVersion.SIF20r1, SifVersion.Parse("2.0r1"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF20r1));
            Assert.Equal(3, Array.BinarySearch(versions, SifVersion.SIF20r1));

            assertSIFVersion(SifVersion.Parse("2.1"), 2, 1, 0);
            Assert.Equal(SifVersion.SIF21, SifVersion.Parse("2.1"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF21));
            Assert.Equal(4, Array.BinarySearch(versions, SifVersion.SIF21));

            assertSIFVersion(SifVersion.Parse("2.2"), 2, 2, 0);
            Assert.Equal(SifVersion.SIF22, SifVersion.Parse("2.2"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF22));
            Assert.Equal(5, Array.BinarySearch(versions, SifVersion.SIF22));

            assertSIFVersion(SifVersion.Parse("2.3"), 2, 3, 0);
            Assert.Equal(SifVersion.SIF23, SifVersion.Parse("2.3"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF23));
            Assert.Equal(6, Array.BinarySearch(versions, SifVersion.SIF23));

            assertSIFVersion(SifVersion.Parse("2.4"), 2, 4, 0);
            Assert.Equal(SifVersion.SIF24, SifVersion.Parse("2.4"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF24));
            Assert.Equal(7, Array.BinarySearch(versions, SifVersion.SIF24));

            assertSIFVersion(SifVersion.Parse("2.5"), 2, 5, 0);
            Assert.Equal(SifVersion.SIF25, SifVersion.Parse("2.5"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF25));
            Assert.Equal(8, Array.BinarySearch(versions, SifVersion.SIF25));

            assertSIFVersion(SifVersion.Parse("2.6"), 2, 6, 0);
            Assert.Equal(SifVersion.SIF26, SifVersion.Parse("2.6"));
            Assert.True(Runtime.IsSIFVersionSupported(SifVersion.SIF26));
            Assert.Equal(9, Array.BinarySearch(versions, SifVersion.SIF26));

            Assert.Equal( SifVersion.LATEST, SifVersion.Parse( "2.6" ) );
            Assert.True(SifVersion.Parse("2.6").Equals( SifVersion.LATEST ));
        }

        [Fact]
        public void TestOperatorOverloads()
        {
            // > operator
            Assert.True(SifVersion.SIF22 > SifVersion.SIF21);
            
            // < operator
            Assert.True(SifVersion.SIF21 < SifVersion.SIF22);

            // >= operator
            Assert.True(SifVersion.SIF22 >= SifVersion.SIF21);
            Assert.True(SifVersion.SIF22 >= SifVersion.SIF21);
            Assert.False(SifVersion.SIF21 >= SifVersion.SIF22);

            // <= operator
            Assert.True(SifVersion.SIF21 <= SifVersion.SIF22);
            Assert.True(SifVersion.SIF21 <= SifVersion.SIF22);
            Assert.False(SifVersion.SIF22 <= SifVersion.SIF21);

            // == operator
            Assert.True(SifVersion.Parse("2.2") == SifVersion.SIF22);
            Assert.False(SifVersion.Parse("2.1") == SifVersion.SIF22);

        }

        [Fact]
        public void ADKIntegrityTest()
        {
            SifVersion[] versions = Runtime.SupportedSIFVersions;

            foreach (SifVersion version in versions)
            {
                Assert.True(Runtime.IsSIFVersionSupported(version));
                Assert.Equal(version, SifVersion.Parse(version.ToString()));
            }
        }


        /**
         *  Test basic parsing capabilities
         */
        [Fact]
        public void testParse()
        {
            assertSIFVersion(SifVersion.Parse("1.0r1"), 1, 0, 1, "1.0r1");
            assertSIFVersion(SifVersion.Parse("1.0r2"), 1, 0, 2, "1.0r2");
            assertSIFVersion(SifVersion.Parse("1.1r3"), 1, 1, 3, "1.1r3");
            assertSIFVersion(SifVersion.Parse("1.1"), 1, 1, 0);
            assertSIFVersion(SifVersion.Parse("1.5"), 1, 5, 0, "1.5");
            assertSIFVersion(SifVersion.Parse("1.5r1"), 1, 5, 1);
            assertSIFVersion(SifVersion.Parse("2.0"), 2, 0, 0);
            assertSIFVersion(SifVersion.Parse("2.0r1"), 2, 0, 1);

            assertSIFVersion(SifVersion.Parse("3.0"), 3, 0, 0, "3.0");
            assertSIFVersion(SifVersion.Parse("5.0r77"), 5, 0, 77, "5.0r77");
        }

        [Fact]
        public void test2DotStar()
        {
            Assert.Equal( SifVersion.GetLatest( 2 ), SifVersion.Parse( "2.*"));
        }

        [Fact]
        public void test2DotStarWithAgentVersion()
        {
            // Agent configured for SIF 2.4 → returns SIF 2.4
            Assert.Equal(SifVersion.SIF24, SifVersion.Parse("2.*", SifVersion.SIF24));

            // Agent configured for SIF 2.6 (LATEST) → returns SIF 2.6
            Assert.Equal(SifVersion.LATEST, SifVersion.Parse("2.*", SifVersion.LATEST));

            // Agent configured for a SIF 1.x version (below SIF20r1 floor) → returns SIF20r1
            Assert.Equal(SifVersion.SIF20r1, SifVersion.Parse("2.*", SifVersion.SIF15r1));

            // Null agentVersion → falls back to SIF20r1 floor
            Assert.Equal(SifVersion.SIF20r1, SifVersion.Parse("2.*", null));
        }

        private void assertSIFVersion(SifVersion assertVersion, int major, int minor, int revision, String tag = null)
        {
            Assert.Equal(major, assertVersion.Major);
            Assert.Equal(minor, assertVersion.Minor);
            Assert.Equal(revision, assertVersion.Revision);
            Assert.Equal(tag ?? (revision == 0 ? $"{major}.{minor}" : $"{major}.{minor}r{revision}"), assertVersion.ToString());
        }


        /**
         * Test the fact that parse() should always return the same instance
         */
        [Fact]
        public void testSameInstance()
        {
            SifVersion v1 = SifVersion.Parse("2.0");
            Assert.True(v1 == SifVersion.SIF20);
            Assert.True(v1.Equals(SifVersion.SIF20));

            v1 = SifVersion.Parse("2.0r1");
            Assert.True(v1 == SifVersion.SIF20r1);
            Assert.True(v1.Equals(SifVersion.SIF20r1));

            v1 = SifVersion.Parse("2.1");
            Assert.True(v1 == SifVersion.SIF21);
            Assert.True(v1.Equals(SifVersion.SIF21));

            v1 = SifVersion.Parse("1.1");
            Assert.True(v1 == SifVersion.SIF11);
            Assert.True(v1.Equals(SifVersion.SIF11));

            v1 = SifVersion.Parse("1.5r1");
            Assert.True(v1 == SifVersion.SIF15r1);
            Assert.True(v1.Equals(SifVersion.SIF15r1));

            v1 = SifVersion.Parse("3.69r55");
            SifVersion v2 = SifVersion.Parse("3.69r55");
            Assert.True(v1 == v2);
            Assert.True(v1.Equals(v2));
        }

        /**
         * Test comparision of SIFVersions
         */
        [Fact]
        public void testComparison()
        {
            Assert.Equal(0, SifVersion.SIF20.CompareTo(SifVersion.SIF20));
            Assert.Equal(0, SifVersion.SIF20r1.CompareTo(SifVersion.SIF20r1));
            Assert.Equal(0, SifVersion.SIF21.CompareTo(SifVersion.SIF21));

            Assert.Equal(-1, SifVersion.SIF15r1.CompareTo(SifVersion.SIF20));
            SifVersion custom = SifVersion.Parse("1.69r55");
            Assert.Equal(-1, custom.CompareTo(SifVersion.SIF20));
            Assert.Equal(1, custom.CompareTo(SifVersion.SIF15r1));

            custom = SifVersion.Parse("1.5r1");
            Assert.Equal(0, custom.CompareTo(SifVersion.SIF15r1));

            custom = SifVersion.Parse("1.5r2");
            Assert.Equal(1, custom.CompareTo(SifVersion.SIF15r1));

            custom = SifVersion.Parse("1.5r0");
            Assert.Equal(-1, custom.CompareTo(SifVersion.SIF15r1));

        }


        /**
         * Test generation of xmlns 
         */
        [Fact]
        public void testxmlnsWrite()
        {
            String xmlns = SifVersion.SIF15r1.Xmlns;
// JEN           Assert.Equal(SifDtd.XMLNS_BASE + "/1.x", xmlns);
            Assert.Equal(Runtime.Dtd.BaseNamespace + "/1.x", xmlns);

            xmlns = SifVersion.SIF20.Xmlns;
// JEN            Assert.Equal(SifDtd.XMLNS_BASE + "/2.x", xmlns);
            Assert.Equal(Runtime.Dtd.BaseNamespace + "/2.x", xmlns);
        }

        /**
         * Test parsing of xmlns 
         */
        [Fact]
        public void testxmlnsParse()
        {
            SifVersion testedVersion = SifVersion.ParseXmlns(null);
            Assert.Null(testedVersion);

            testedVersion = SifVersion.ParseXmlns("");
            Assert.Null(testedVersion);

// JEN           testedVersion = SifVersion.ParseXmlns(SifDtd.XMLNS_BASE + "/1.x");
            testedVersion = SifVersion.ParseXmlns(Runtime.Dtd.BaseNamespace + "/1.x");
            Assert.Equal(SifVersion.SIF15r1, testedVersion);

// JEN           testedVersion = SifVersion.ParseXmlns(SifDtd.XMLNS_BASE + "/2.x");
            testedVersion = SifVersion.ParseXmlns(Runtime.Dtd.BaseNamespace + "/2.x");
            Assert.Equal(SifVersion.SIF26, testedVersion);

// JEN          testedVersion = SifVersion.ParseXmlns(SifDtd.XMLNS_BASE + "/9.x");
            testedVersion = SifVersion.ParseXmlns(Runtime.Dtd.BaseNamespace + "/9.x");
            Assert.Null(testedVersion);
        }
    }
}
