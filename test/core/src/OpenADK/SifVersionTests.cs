using System;
using OpenADK.Library;
using NUnit.Framework;
using Library.UnitTesting.Framework;
using OpenADK.Library.us;

namespace Library.Nunit.Core
{
    [TestFixture]
    public class SifVersionTests : AdkTest
    {
        [Test]
        public void testAllOfficiallySupportedVersions()
        {
            SifVersion[] versions = Runtime.SupportedSIFVersions;

            assertSIFVersion(SifVersion.Parse("1.1"), 1, 1, 0, "1.1");
            Assert.AreEqual(SifVersion.SIF11, SifVersion.Parse("1.1"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF11));
            Assert.AreEqual(0, Array.BinarySearch(versions, SifVersion.SIF11), "1.1");


            assertSIFVersion(SifVersion.Parse("1.5r1"), 1, 5, 1, "1.5r1");
            Assert.AreEqual(SifVersion.SIF15r1, SifVersion.Parse("1.5r1"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF15r1));
            Assert.AreEqual(1, Array.BinarySearch(versions, SifVersion.SIF15r1), "1.5r1");

            assertSIFVersion(SifVersion.Parse("2.0"), 2, 0, 0, "2.0");
            Assert.AreEqual(SifVersion.SIF20, SifVersion.Parse("2.0"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF20));
            Assert.AreEqual(2, Array.BinarySearch(versions, SifVersion.SIF20), "2.0");

            assertSIFVersion(SifVersion.Parse("2.0r1"), 2, 0, 1, "2.0r1");
            Assert.AreEqual(SifVersion.SIF20r1, SifVersion.Parse("2.0r1"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF20r1));
            Assert.AreEqual(3, Array.BinarySearch(versions, SifVersion.SIF20r1), "2.0r1");

            assertSIFVersion(SifVersion.Parse("2.1"), 2, 1, 0, "2.1");
            Assert.AreEqual(SifVersion.SIF21, SifVersion.Parse("2.1"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF21));
            Assert.AreEqual(4, Array.BinarySearch(versions, SifVersion.SIF21), "2.1");

            assertSIFVersion(SifVersion.Parse("2.2"), 2, 2, 0, "2.2");
            Assert.AreEqual(SifVersion.SIF22, SifVersion.Parse("2.2"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF22));
            Assert.AreEqual(5, Array.BinarySearch(versions, SifVersion.SIF22), "2.2");

            assertSIFVersion(SifVersion.Parse("2.3"), 2, 3, 0, "2.3");
            Assert.AreEqual(SifVersion.SIF23, SifVersion.Parse("2.3"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF23));
            Assert.AreEqual(6, Array.BinarySearch(versions, SifVersion.SIF23), "2.3");

            assertSIFVersion(SifVersion.Parse("2.4"), 2, 4, 0, "2.4");
            Assert.AreEqual(SifVersion.SIF24, SifVersion.Parse("2.4"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF24));
            Assert.AreEqual(7, Array.BinarySearch(versions, SifVersion.SIF24), "2.4");

            assertSIFVersion(SifVersion.Parse("2.5"), 2, 5, 0, "2.5");
            Assert.AreEqual(SifVersion.SIF25, SifVersion.Parse("2.5"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF25));
            Assert.AreEqual(8, Array.BinarySearch(versions, SifVersion.SIF25), "2.5");

            assertSIFVersion(SifVersion.Parse("2.6"), 2, 6, 0, "2.6");
            Assert.AreEqual(SifVersion.SIF26, SifVersion.Parse("2.6"));
            Assert.IsTrue(Runtime.IsSIFVersionSupported(SifVersion.SIF26));
            Assert.AreEqual(9, Array.BinarySearch(versions, SifVersion.SIF26), "2.6");

            Assert.AreEqual( SifVersion.LATEST, SifVersion.Parse( "2.6" ) );
            Assert.IsTrue(SifVersion.Parse("2.6").Equals( SifVersion.LATEST ), "Latest");
        }

        [Test]
        public void TestOperatorOverloads()
        {
            // > operator
            Assert.IsTrue(SifVersion.SIF22 > SifVersion.SIF21);
            
            // < operator
            Assert.IsTrue(SifVersion.SIF21 < SifVersion.SIF22);

            // >= operator
            Assert.IsTrue(SifVersion.SIF22 >= SifVersion.SIF21);
            Assert.IsTrue(SifVersion.SIF22 >= SifVersion.SIF21);
            Assert.IsFalse(SifVersion.SIF21 >= SifVersion.SIF22);

            // <= operator
            Assert.IsTrue(SifVersion.SIF21 <= SifVersion.SIF22);
            Assert.IsTrue(SifVersion.SIF21 <= SifVersion.SIF22);
            Assert.IsFalse(SifVersion.SIF22 <= SifVersion.SIF21);

            // == operator
            Assert.IsTrue(SifVersion.Parse("2.2") == SifVersion.SIF22, "==");
            Assert.IsFalse(SifVersion.Parse("2.1") == SifVersion.SIF22, "==");

        }

        [Test]
        public void ADKIntegrityTest()
        {
            SifVersion[] versions = Runtime.SupportedSIFVersions;

            foreach (SifVersion version in versions)
            {
                Assert.IsTrue(Runtime.IsSIFVersionSupported(version), version.ToString());
                Assert.AreEqual(version, SifVersion.Parse(version.ToString()), version.ToString());
            }
        }


        /**
         *  Test basic parsing capabilities
         */
        [Test]
        public void testParse()
        {
            assertSIFVersion(SifVersion.Parse("1.0r1"), 1, 0, 1, "1.0r1");
            assertSIFVersion(SifVersion.Parse("1.0r2"), 1, 0, 2, "1.0r2");
            assertSIFVersion(SifVersion.Parse("1.1r3"), 1, 1, 3, "1.1r3");
            assertSIFVersion(SifVersion.Parse("1.1"), 1, 1, 0, "1.1");
            assertSIFVersion(SifVersion.Parse("1.5"), 1, 5, 0, "1.5");
            assertSIFVersion(SifVersion.Parse("1.5r1"), 1, 5, 1, "1.5r1");
            assertSIFVersion(SifVersion.Parse("2.0"), 2, 0, 0, "2.0");
            assertSIFVersion(SifVersion.Parse("2.0r1"), 2, 0, 1, "2.0r1");

            assertSIFVersion(SifVersion.Parse("3.0"), 3, 0, 0, "3.0");
            assertSIFVersion(SifVersion.Parse("5.0r77"), 5, 0, 77, "5.0r77");
        }

        [Test]
        public void test2DotStar()
        {
            Assert.AreEqual( SifVersion.GetLatest( 2 ), SifVersion.Parse( "2.*"));
        }

        [Test]
        public void test2DotStarWithAgentVersion()
        {
            // Agent configured for SIF 2.4 → returns SIF 2.4
            Assert.AreEqual(SifVersion.SIF24, SifVersion.Parse("2.*", SifVersion.SIF24));

            // Agent configured for SIF 2.6 (LATEST) → returns SIF 2.6
            Assert.AreEqual(SifVersion.LATEST, SifVersion.Parse("2.*", SifVersion.LATEST));

            // Agent configured for a SIF 1.x version (below SIF20r1 floor) → returns SIF20r1
            Assert.AreEqual(SifVersion.SIF20r1, SifVersion.Parse("2.*", SifVersion.SIF15r1));

            // Null agentVersion → falls back to SIF20r1 floor
            Assert.AreEqual(SifVersion.SIF20r1, SifVersion.Parse("2.*", null));
        }

        private void assertSIFVersion(SifVersion assertVersion, int major, int minor, int revision, String tag)
        {
            Assert.AreEqual(major, assertVersion.Major, "Major");
            Assert.AreEqual(minor, assertVersion.Minor, "Minor");
            Assert.AreEqual(revision, assertVersion.Revision, "Revision");
            Assert.AreEqual(tag, assertVersion.ToString(), "Tag");
        }


        /**
         * Test the fact that parse() should always return the same instance
         */
        [Test]
        public void testSameInstance()
        {
            SifVersion v1 = SifVersion.Parse("2.0");
            Assert.IsTrue(v1 == SifVersion.SIF20, "2.0");
            Assert.IsTrue(v1.Equals(SifVersion.SIF20), "2.0");

            v1 = SifVersion.Parse("2.0r1");
            Assert.IsTrue(v1 == SifVersion.SIF20r1, "2.0r1");
            Assert.IsTrue(v1.Equals(SifVersion.SIF20r1), "2.0r1");

            v1 = SifVersion.Parse("2.1");
            Assert.IsTrue(v1 == SifVersion.SIF21, "2.1");
            Assert.IsTrue(v1.Equals(SifVersion.SIF21), "2.1");

            v1 = SifVersion.Parse("1.1");
            Assert.IsTrue(v1 == SifVersion.SIF11, "1.1");
            Assert.IsTrue(v1.Equals(SifVersion.SIF11), "1.1");

            v1 = SifVersion.Parse("1.5r1");
            Assert.IsTrue(v1 == SifVersion.SIF15r1, "1.5r1");
            Assert.IsTrue(v1.Equals(SifVersion.SIF15r1), "1.5r1");

            v1 = SifVersion.Parse("3.69r55");
            SifVersion v2 = SifVersion.Parse("3.69r55");
            Assert.IsTrue(v1 == v2, "3.69r55");
            Assert.IsTrue(v1.Equals(v2));
        }

        /**
         * Test comparision of SIFVersions
         */
        [Test]
        public void testComparison()
        {
            Assert.AreEqual(0, SifVersion.SIF20.CompareTo(SifVersion.SIF20), "2.0<-->2.0");
            Assert.AreEqual(0, SifVersion.SIF20r1.CompareTo(SifVersion.SIF20r1), "2.0r1<-->2.0r1");
            Assert.AreEqual(0, SifVersion.SIF21.CompareTo(SifVersion.SIF21), "2.1<-->2.1");

            Assert.AreEqual(-1, SifVersion.SIF15r1.CompareTo(SifVersion.SIF20), "1.5r1<-->2.0");
            SifVersion custom = SifVersion.Parse("1.69r55");
            Assert.AreEqual(-1, custom.CompareTo(SifVersion.SIF20), "1.69r55<-->2.0");
            Assert.AreEqual(1, custom.CompareTo(SifVersion.SIF15r1), "1.69r55<-->1.5r1");

            custom = SifVersion.Parse("1.5r1");
            Assert.AreEqual(0, custom.CompareTo(SifVersion.SIF15r1), "1.5r1<-->1.5r1");

            custom = SifVersion.Parse("1.5r2");
            Assert.AreEqual(1, custom.CompareTo(SifVersion.SIF15r1), "1.5rr<-->1.5r1");

            custom = SifVersion.Parse("1.5r0");
            Assert.AreEqual(-1, custom.CompareTo(SifVersion.SIF15r1), "1.5r0<-->1.5r1");

        }


        /**
         * Test generation of xmlns 
         */
        [Test]
        public void testxmlnsWrite()
        {
            String xmlns = SifVersion.SIF15r1.Xmlns;
// JEN           Assert.AreEqual(SifDtd.XMLNS_BASE + "/1.x", xmlns, "xmlns15");
            Assert.AreEqual(Runtime.Dtd.BaseNamespace + "/1.x", xmlns, "xmlns15");

            xmlns = SifVersion.SIF20.Xmlns;
// JEN            Assert.AreEqual(SifDtd.XMLNS_BASE + "/2.x", xmlns, "xmlns20");
            Assert.AreEqual(Runtime.Dtd.BaseNamespace + "/2.x", xmlns, "xmlns20");
        }

        /**
         * Test parsing of xmlns 
         */
        [Test]
        public void testxmlnsParse()
        {
            SifVersion testedVersion = SifVersion.ParseXmlns(null);
            Assert.IsNull(testedVersion, "NULL");

            testedVersion = SifVersion.ParseXmlns("");
            Assert.IsNull(testedVersion, "empty");

// JEN           testedVersion = SifVersion.ParseXmlns(SifDtd.XMLNS_BASE + "/1.x");
            testedVersion = SifVersion.ParseXmlns(Runtime.Dtd.BaseNamespace + "/1.x");
            Assert.AreEqual(SifVersion.SIF15r1, testedVersion, "SIF15r1");

// JEN           testedVersion = SifVersion.ParseXmlns(SifDtd.XMLNS_BASE + "/2.x");
            testedVersion = SifVersion.ParseXmlns(Runtime.Dtd.BaseNamespace + "/2.x");
            Assert.AreEqual(SifVersion.SIF26, testedVersion, "LATEST");

// JEN          testedVersion = SifVersion.ParseXmlns(SifDtd.XMLNS_BASE + "/9.x");
            testedVersion = SifVersion.ParseXmlns(Runtime.Dtd.BaseNamespace + "/9.x");
            Assert.IsNull(testedVersion, "9.x");
        }
    }
}
