using System;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.Nunit.US.Common
{
    
    public class PartialDateTests : AdkTest
    {
        [Fact]
        public void testPartialDate005()
        {
            PartialDateType date = new PartialDateType("1999-12-25");
            assertPartialDate(date, 1999, 12, 25, "1999-12-25");
        }

        [Fact]
        public void testPartialDate006()
        {
            PartialDateType date = new PartialDateType("2006-06-01+13:00");
            assertPartialDate(date, 2006, 6, 1, "2006-06-01+13:00");
        }

        [Fact]
        public void testPartialDate007()
        {
            PartialDateType date = new PartialDateType("2006-06-01Z");
            assertPartialDate(date, 2006, 6, 1, "2006-06-01Z");
        }

        private void assertPartialDate(PartialDateType date, int year, int month, int day, String lexicalValue)
        {
            Assert.Equal(year, date.Year.Value);
            Assert.Equal(month, date.Month.Value);
            Assert.Equal(day, date.Day.Value);

            DateTime cal = date.Date;

            Assert.Equal(year, cal.Year);
            Assert.Equal(month, cal.Month);
            Assert.Equal(day, cal.Day);

            Assert.Equal(lexicalValue, date.Value);
            Assert.Equal(lexicalValue, date.TextValue);

            Assert.Equal(PartialDateType.DateType.Date, date.DataType);
        }

        [Fact]
        public void testPartialDate010()
        {
            PartialDateType date = new PartialDateType(1999);
            Assert.Equal(1999, (int) date.Year);
            Assert.Equal(PartialDateType.DateType.GYear, date.DataType);
        }

        [Fact]
        public void testPartialDate011()
        {
            PartialDateType date = new PartialDateType("1999");
            Assert.Equal(1999, (int) date.Year);
            Assert.Equal("1999", date.Value);
            Assert.Equal("1999", date.TextValue);
            Assert.Equal(PartialDateType.DateType.GYear, date.DataType);
        }

        [Fact]
        public void testPartialDate014()
        {
            PartialDateType date = new PartialDateType(1999);
            Assert.Equal(1999, (int) date.Year);
            Assert.Equal("1999", date.Value);
            Assert.Equal("1999", date.TextValue);
            Assert.Equal(PartialDateType.DateType.GYear, date.DataType);
        }

        [Fact]
        public void testPartialDate012()
        {
            PartialDateType date = new PartialDateType("1999Z");
            Assert.Equal(1999, date.Year);
            Assert.Equal("1999Z", date.Value);
            Assert.Equal("1999Z", date.TextValue);
            Assert.Equal(PartialDateType.DateType.GYear, date.DataType);
        }

        [Fact]
        public void testPartialDate013()
        {
            PartialDateType date = new PartialDateType("1999-06:00");
            Assert.Equal(1999, date.Year);
            Assert.Equal("1999-06:00", date.Value);
            Assert.Equal("1999-06:00", date.TextValue);
            Assert.Equal(PartialDateType.DateType.GYear, date.DataType);
        }

        [Fact]
        public void testPartialDate020()
        {
            PartialDateType date = new PartialDateType(1999, 12);
            Assert.Equal(1999, date.Year);
            Assert.Equal(12, date.Month);
            Assert.Equal("1999-12", date.Value);
            Assert.Equal("1999-12", date.TextValue);
            Assert.Equal(PartialDateType.DateType.GYearMonth, date.DataType);
        }

        [Fact]
        public void testPartialDate021()
        {
            PartialDateType date = new PartialDateType("1999-12Z");
            Assert.Equal(1999, (int) date.Year);
            Assert.Equal(12, (int) date.Month);
            Assert.Equal("1999-12Z", date.Value);
            Assert.Equal("1999-12Z", date.TextValue);
            Assert.Equal(PartialDateType.DateType.GYearMonth, date.DataType);
        }

        [Fact]
        public void testPartialDate022()
        {
            PartialDateType date = new PartialDateType("0010-12-06:00");
            Assert.Equal(10, (int) date.Year);
            Assert.Equal(12, (int) date.Month);
            Assert.Null(date.Day);
            Assert.Equal("0010-12-06:00", date.Value);
            Assert.Equal("0010-12-06:00", date.TextValue);
            Assert.Equal(PartialDateType.DateType.GYearMonth, date.DataType);
        }

        [Fact]
        public void testPartialDate030()
        {
            PartialDateType date = new PartialDateType(10, 12, 25);
            assertPartialDate(date, 10, 12, 25, "0010-12-25");
        }

        [Fact]
        public void testPartialDate050()
        {
            PartialDateType date = new PartialDateType(1999, 12, 25);
            assertPartialDate(date, 1999, 12, 25, "1999-12-25");

            date.TextValue = "1999-12-06:00";
            Assert.Equal(1999, (int) date.Year);
            Assert.Equal(12, (int) date.Month);
            Assert.Null(date.Day);
            Assert.Equal("1999-12-06:00", date.Value);
            Assert.Equal("1999-12-06:00", date.TextValue);

            date.TextValue = "2007-06-01";
            assertPartialDate(date, 2007, 06, 01, "2007-06-01");

            date.TextValue = "2020-06-05Z";
            assertPartialDate(date, 2020, 06, 05, "2020-06-05Z");
        }

        [Fact]
        public void testPartialDate040()
        {
            DateTime? c = Runtime.Dtd.GetFormatter(SifVersion.SIF20).ToDate("1999-12-25");
            PartialDateType date = new PartialDateType(c);
            assertPartialDate(date, 1999, 12, 25, "1999-12-25");
        }
    }
}
