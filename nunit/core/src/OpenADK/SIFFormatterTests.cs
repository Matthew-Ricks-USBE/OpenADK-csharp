using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using OpenADK.Library;
using OpenADK.Library.Impl;
using OpenADK.Library.Infra;
using NUnit.Framework;

namespace Library.Nunit.Core
{
    [TestFixture]
    public class SifFormatterTests
    {
        	/**
	 * The SIF 1.x formatter supports things like empty strings and spaces
	 * as null values. This is primarily for backwards support for SASI
	 * @throws ADKParsingException
	 */
        [Test]
        public void testSIF1xEmtpyStringSupport()
        {
            SifFormatter SIF1x = new Sif1xFormatter();


            Assert.IsNull( SIF1x.ToBool( "" ), "Should be null" );
            Assert.IsNull( SIF1x.ToBool( "  " ), "Should be null" );
            Assert.IsNull( SIF1x.ToDecimal( "" ), "Should be null" );
            Assert.IsNull( SIF1x.ToDecimal( "  " ), "Should be null" );
            Assert.IsNull( SIF1x.ToInt( "" ), "Should be null" );
            Assert.IsNull( SIF1x.ToInt( "  " ), "Should be null" );
            Assert.IsNull( SIF1x.ToDateTime( "" ), "Should be null" );
            Assert.IsNull( SIF1x.ToDateTime( "  " ), "Should be null" );
        }

        [Test]
        public void testParsing()
        {
            SifFormatter SIF1x = new Sif1xFormatter();
            SifFormatter SIF2x = new Sif2xFormatter();
            Boolean BooleanValue = true;
            //mjn declare types as nullable
            Boolean? BooleanNull = null;
            int? intNull = null;
            assertBooleanParsing(SIF1x, "Yes", BooleanValue);
            Assert.AreEqual("", SIF1x.ToString(BooleanNull), "Null Bool Value"); //( (Boolean)null ));
            assertBooleanParsing(SIF2x, "true", BooleanValue);
            Assert.IsNull(SIF2x.ToString(BooleanNull), "Null Bool Value");

            bool? testValue = SIF2x.ToBool( "1");
            Assert.IsTrue(testValue.Value, "Boolean Value" );

            testValue = SIF2x.ToBool("0");
            Assert.IsFalse(testValue.Value, "Boolean Value" );

            float floatValue =99.34f;

            assertFloatParsing(SIF1x, "99.34", floatValue);
            Assert.IsNull( SIF1x.ToString((float?)null));
            assertFloatParsing(SIF2x, "99.34", floatValue);
            Assert.IsNull( SIF2x.ToString((float?)null));

            floatValue = 321651.09934f;
            assertFloatParsing(SIF1x, "321651.1", floatValue);
            assertFloatParsing(SIF2x, "321651.1", floatValue);


            //INF, -INF and NaN
            assertFloatParsing(SIF1x, "INF", float.PositiveInfinity );
            assertFloatParsing(SIF1x, "-INF", float.NegativeInfinity );

            float? nan = SIF1x.ToFloat( "NaN" );
            Assert.IsTrue( float.IsNaN( nan.Value ) );
            
            assertFloatParsing(SIF2x, "INF", float.PositiveInfinity );
            assertFloatParsing(SIF2x, "-INF", float.NegativeInfinity );
            
            nan = SIF2x.ToFloat("NaN");
            Assert.IsTrue(float.IsNaN(nan.Value));
            

            int intValue = 9998877; // new int(9998877);

            assertintParsing(SIF1x, "9998877", intValue);
            Assert.AreEqual("", SIF1x.ToString(intNull), "Null int Value");
            assertintParsing(SIF2x, "9998877", intValue);
            Assert.IsNull(SIF2x.ToString(intNull), "Null int Value");
            DateTime? sampleDate = new DateTime(1999, 10, 01);
            AssertDateParsing(SIF1x, "19991001", sampleDate);
            Assert.AreEqual("", SIF1x.ToDateString(null), "Null Value");
            AssertDateParsing(SIF2x, "1999-10-01" /* + tzOffset */, sampleDate);
            Assert.IsNull(SIF2x.ToDateString(null), "Null Date Value");
        }

        [Test]
        public void testThrowsFormatException1x()
        {
            assertThrowsFormatException(new Sif1xFormatter());
        }

        [Test]
        public void testThrowsFormatException2x()
        {
            assertThrowsFormatException(new Sif2xFormatter());
        }

        private void assertThrowsFormatException(SifFormatter formatter)
        {
            Assert.Throws<FormatException>(() => formatter.ToBool("asdf"), "ToBool() should throw FormatException");
            Assert.Throws<FormatException>(() => formatter.ToInt("asdf"), "ToInt() should throw FormatException");
            Assert.Throws<FormatException>(() => formatter.ToDecimal("asdf"), "ToDecimal() should throw FormatException");
            Assert.Throws<FormatException>(() => formatter.ToDate("asdf"), "ToDate() should throw FormatException");
            Assert.Throws<FormatException>(() => formatter.ToTime("asdf"), "ToTime() should throw FormatException");

            // DateTime and Duration are not supported by the SIF1xFormatter
            if (formatter is Sif1xFormatter) return;
            Assert.Throws<FormatException>(() => formatter.ToDateTime("asdf"), "ToDateTime() should throw FormatException");
            Assert.Throws<FormatException>(() => formatter.ToTimeSpan("asdf"), "ToTimeSpan() should throw FormatException");
        }

        private void AssertDateParsing(SifFormatter formatter, String stringValue, DateTime? value)
        {
            Console.WriteLine("Testing Date parse of '" + stringValue + "' using " + formatter.ToString());
            //Calendar testValue = formatter.ToDate(stringValue);
            DateTime? testValue = formatter.ToDate(stringValue);
            Assert.AreEqual(value.Value, testValue.Value, "Date Value");
            Assert.AreEqual(stringValue, (String) formatter.ToDateString(testValue), "String Value");

            testValue = (DateTime?) formatter.ToDate(null);
            Assert.IsNull(testValue, "Date value should be null");
        }

        private void AssertDateParsing(SifFormatter formatter, String stringValue, Calendar value)
        {
            Console.WriteLine("Testing Date parse of '" + stringValue + "' using " + formatter.ToString());
            //Calendar testValue = formatter.ToDate(stringValue);
            DateTime testValue = (DateTime) formatter.ToDate(stringValue);
            Assert.AreEqual(value, testValue, "Date Value");
            Assert.AreEqual(stringValue, (String) formatter.ToDateString(testValue), "String Value");

            testValue = (DateTime) formatter.ToDate(null);
            Assert.IsNull(testValue, "Date value should be null");
        }

        [Test]
        public void testSIF2xDateTimeParsing()
        {
            SifFormatter formatter = new Sif2xFormatter();
            String testValue = formatter.ToDateTimeString(null);
            Assert.IsNull(testValue, "Date value should be null" );

            DateTime assertedDate = new DateTime(1999, 9, 1, 22, 2, 4 );
            TimeSpan utcOffset = TimeZoneInfo.Local.GetUtcOffset(assertedDate);
            Console.Write( "UTC Offset: ");
            Console.WriteLine( utcOffset );
            Console.WriteLine();

            Nullable<DateTime> assertedDateTime = new Nullable<DateTime>(assertedDate);
            String sifString = formatter.ToDateTimeString(assertedDateTime);
            AssertDateTimeParsing(assertedDateTime, formatter, sifString, "Re-Parse of original");


            String test1 = "1999-09-01T22:02:04";
            AssertDateTimeParsing(assertedDateTime, formatter, test1, "Date Time");

            DateTime UTcTime = assertedDate.ToUniversalTime();
            String test2 = UTcTime.ToString("s") + "Z";
            AssertDateTimeParsing(assertedDateTime, formatter, test2, "UTC Time (Z)");

//            test2 = UTcTime.ToString("s") + "+0:00";
//            AssertDateTimeParsing(assertedDateTime, formatter, test2, "UTC Time (+00:00)");

            // Test the time, using Eastern time
            DateTime easternTime = UTcTime.AddHours(-5);
            String test3 = easternTime.ToString("s") + "-05:00";
            AssertDateTimeParsing(assertedDateTime, formatter, test3, "Eastern Time");


            // Test the time, using european time
            DateTime europeanTime = UTcTime.AddHours( 3 );
            String test4 = europeanTime.ToString( "s" ) + "+03:00";
            AssertDateTimeParsing(assertedDateTime, formatter, test4, "European Time");
        }

        private void AssertDateTimeParsing(DateTime? assertedDateTime, SifFormatter formatter, string test, String statement)
        {
            DateTime? parsedValue;
            parsedValue = formatter.ToDateTime(test);
            Console.WriteLine( statement );
            Console.WriteLine( "parsing: " + test);
            Console.WriteLine( "Asserted: {0} : {1}", assertedDateTime.Value, assertedDateTime.Value.ToFileTimeUtc() );
            Console.WriteLine("Parsed: {0} : {1}", parsedValue.Value, parsedValue.Value.ToFileTimeUtc() );
            Console.WriteLine();
            AssertDateTime( statement, assertedDateTime, parsedValue);
        }


        private void AssertTimeParsing(DateTime? assertedDateTime, SifFormatter formatter, string test, String statement)
        {
            DateTime? parsedValue;
            parsedValue = formatter.ToTime(test);
            Console.WriteLine(statement);
            Console.WriteLine("parsing: " + test);
            Console.WriteLine("Asserted: {0} : {1}", assertedDateTime.Value, assertedDateTime.Value.ToFileTimeUtc());
            Console.WriteLine("Parsed: {0} : {1}", parsedValue.Value, parsedValue.Value.ToFileTimeUtc());
            Console.WriteLine();
            assertTimes( statement, assertedDateTime, parsedValue);
        }


        [Test]
        public void testSIF2xTimeParsing()
        {
            SifFormatter formatter = new Sif2xFormatter();
            String testValue = formatter.ToTimeString(null);
            Assert.IsNull(testValue, "Date value should be null" );


            DateTime now = DateTime.Now;
            DateTime assertedDate = new DateTime( now.Year, now.Month, now.Day, 21, 0, 59);
            Nullable<DateTime> assertedDateTime = new Nullable<DateTime>(assertedDate);


            String sifString = formatter.ToDateTimeString(assertedDateTime);
            AssertTimeParsing(assertedDateTime, formatter, sifString, "Re-Parse of original");


            String test1 = "21:00:59";
            AssertTimeParsing(assertedDateTime, formatter, test1, "Date Time");
            
            DateTime UTcTime = assertedDate.ToUniversalTime();
            String test2 = UTcTime.ToString("HH:mm:ss") + "Z";
            AssertTimeParsing(assertedDateTime, formatter, test2, "UTC Time (Z)");

//            test2 = UTcTime.ToString("HH:mm:ss") + "+0:00";
//            AssertTimeParsing(assertedDateTime, formatter, test2, "UTC Time (+00:00)");

            // Test the time, using Eastern time
            DateTime easternTime = UTcTime.AddHours(-5);
            String test3 = easternTime.ToString("HH:mm:ss") + "-05:00";
            AssertTimeParsing(assertedDateTime, formatter, test3, "Eastern Time");


            // Test the time, using european time
            DateTime europeanTime = UTcTime.AddHours(3);
            String test4 = europeanTime.ToString("HH:mm:s.s") + "+03:00";
            AssertTimeParsing(assertedDateTime, formatter, test4, "European Time");


            test4 = "2008-04-21T14:16:00.3651098Z";
            DateTime t = formatter.ToDateTime( test4 ).Value;

            Console.WriteLine( t );



        }

        private void AssertDateTime(String text, DateTime? expectedValue, DateTime? testValue)
        {
            Assert.AreEqual(expectedValue.Value, testValue.Value, text);
        }

        private void assertTimes(String text, DateTime? expectedValue, DateTime? testValue)
        {
            Assert.AreEqual(expectedValue.Value.Hour, testValue.Value.Hour, text + " HOUR: ");
            Assert.AreEqual(expectedValue.Value.Minute, testValue.Value.Minute, text + " MINUTE: ");
            Assert.AreEqual(expectedValue.Value.Second, testValue.Value.Second, text + " SECOND: ");
        }

        private void assertBooleanParsing(SifFormatter formatter, String stringValue, Boolean value)
        {
            Console.WriteLine("Testing Boolean parse of '" + stringValue + "' using " + formatter.ToString());
            Boolean? testValue = formatter.ToBool(stringValue);
            Assert.AreEqual(value, testValue, "Boolean Value");
            Assert.AreEqual(stringValue, formatter.ToString(value), "String Value");

            testValue = formatter.ToBool(null);
            Assert.IsNull(testValue, "Boolean value should be null");
        }

        private void assertintParsing(SifFormatter formatter, String stringValue, int value)
        {
            Console.WriteLine("Testing int parse of '" + stringValue + "' using " + formatter.ToString());
            int? testValue = formatter.ToInt(stringValue);
            Assert.AreEqual(value, testValue, "int Value");
            Assert.AreEqual(stringValue, formatter.ToString(value), "String Value");

            testValue = formatter.ToInt(null);
            Assert.IsNull(testValue, "int value should be null");
        } 

        /// <summary>
        /// Asserts that the elements returned by GetContent() are returned using a stable sort.
        /// A stable sort is one in which the order of the elements remain the same if they compare 
        /// equally. There is a problem if the default .NET Sort algorithm is used, which is not stable.
        /// </summary>
        [Test]
        public void TestStableSortInGetContent()
        {
            if( !Adk.Initialized )
            {
                Adk.Initialize();
            }

            SIF_Register sr = new SIF_Register();

            sr.SIF_Icon = "test.ico";

            // Create the original list of SIF_Version strings
            string[] list = new string[5];
            list[0] = "1.1";
            list[1] = "2.*";
            list[2] = "2.0r1";
            list[3] = "2.5";
            list[4] = "1.0r1";

            for( int a = 0; a < 5; a++ )
            {
                sr.AddSIF_Version( new SIF_Version( list[a] ) );
            }

            sr.SIF_Name = "AgentName";
            sr.SIF_Mode = "Push";

            IList<Element> elements = Adk.Dtd.GetFormatter( SifVersion.SIF11 ).GetContent( sr, SifVersion.SIF11 );

            // We should have gotten back a list of elements like this:
            // SIF_Name
            // 5 SIF_Version elements
            // SIF_Mode
            // SIF_Icon (only if the version is 2.0 or greater)
            
            Assert.AreEqual( "AgentName", elements[0].TextValue );
            // Assert that the SIF_Version elements returned are still in the order they went in
            for (int a = 0; a < 5; a++)
            {
                Assert.AreEqual( list[a], elements[a+1].TextValue );
            }
            Assert.AreEqual("Push", elements[6].TextValue);
            // NOTE: SIF_Icon is not present in SIF 1.1
            
            elements = Adk.Dtd.GetFormatter(SifVersion.SIF21).GetContent(sr, SifVersion.SIF21);


            Assert.AreEqual("AgentName", elements[0].TextValue);
            // Assert that the SIF_Version elements returned are still in the order they went in
            for (int a = 0; a < 5; a++)
            {
                Assert.AreEqual(list[a], elements[a + 1].TextValue);
            }
            Assert.AreEqual("Push", elements[6].TextValue);
            Assert.AreEqual("test.ico", elements[7].TextValue);
        }


        private void assertFloatParsing(SifFormatter formatter, String stringValue, float value)
        {
            Console.WriteLine( "Testing Float parse of '" + stringValue + "' using " + formatter.ToString() );
            float? testValue = formatter.ToFloat( stringValue );
            Assert.AreEqual( value, testValue );
            Assert.AreEqual( stringValue, formatter.ToString( value ), "String Value" );

            testValue = formatter.ToFloat( null );
            Assert.IsFalse( testValue.HasValue, "Float value should be null" );
        }
    } 
} 

