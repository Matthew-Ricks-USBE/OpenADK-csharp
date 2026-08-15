using System;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using Xunit;
using Library.UnitTesting.Framework;
using OpenADK.Library.us;

namespace Library.Nunit.US.Library.Impl.Surrogates
{
    
    public class GradYearSurrogateTests : AdkTest
    {
        private SifVersion _testOriginalVersion;

        
        public GradYearSurrogateTests()
        {
            
            _testOriginalVersion = Runtime.SifVersion;
            Runtime.SifVersion = SifVersion.SIF15r1;
        }

        
        public override void Dispose()
        {
            Runtime.SifVersion = _testOriginalVersion;
            base.Dispose();
        }

        [Fact]
        public void testParseOnTimeGradYear()
        {
            String sXML = "<StudentPersonal RefId='12345678901234567890'>"
                          + " <GradYear Type='Original'>1971</GradYear>"
                          + "</StudentPersonal>";

            StudentPersonal sp = (StudentPersonal) parseSIF15r1XML( sXML );
            sp = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF11 );
            Assert.NotNull( sp );
            Assert.NotNull(sp.OnTimeGraduationYear);
            Assert.Equal(1971, (int) sp.OnTimeGraduationYear);

            sp = Objects.Create<StudentPersonal>();
            sp.SetElementOrAttribute( "GradYear[@Type='Original']", "8877" );
            Assert.NotNull(sp.OnTimeGraduationYear);
            Assert.Equal(8877, (int) sp.OnTimeGraduationYear);

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Original']" );
            Assert.NotNull( gradValue);
            int gradYear = (int) gradValue.SifValue.RawValue;
            Assert.Equal(8877, gradYear);
        }

        [Fact]
        public void testParseProjectedGradYear()
        {
            String sXML = "<StudentPersonal RefId='12345678901234567890'>"
                          + " <GradYear Type='Projected'>2012</GradYear>"
                          + "</StudentPersonal>";

            StudentPersonal sp = (StudentPersonal) parseSIF15r1XML( sXML );
            sp = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF11 );
            Assert.NotNull( sp );
            Assert.NotNull( sp.ProjectedGraduationYear);
            Assert.Equal( 2012, (int) sp.ProjectedGraduationYear);

            sp = Objects.Create<StudentPersonal>();
            sp.SetElementOrAttribute( "GradYear[@Type='Projected']", "2089" );
            Assert.NotNull( sp.ProjectedGraduationYear);
            Assert.Equal( 2089, (int) sp.ProjectedGraduationYear);

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Projected']" );
            Assert.NotNull( gradValue);
            int gradYear = (int) gradValue.SifValue.RawValue;
            Assert.Equal( 2089, gradYear);
        }

        [Fact]
        public void testParseGraduationDate()
        {
            String sXML = "<StudentPersonal RefId='12345678901234567890'>"
                          + " <GradYear Type='Actual'>2005</GradYear>"
                          + "</StudentPersonal>";

            StudentPersonal sp = (StudentPersonal) parseSIF15r1XML( sXML );
            sp = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF11 );
            Assert.NotNull( sp );
            PartialDateType gd = sp.GraduationDate;
            Assert.NotNull( gd);
            Assert.Equal( 2005, (int) gd.Year);

            sp = Objects.Create<StudentPersonal>();
            sp.SetElementOrAttribute( "GradYear[@Type='Actual']", "2054" );
            gd = sp.GraduationDate;
            Assert.NotNull( gd);
            Assert.NotNull(gd.Year);
            Assert.Equal( 2054, gd.Year.Value);

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Actual']" );
            Assert.NotNull( gradValue);
            PartialDateType pdt = (PartialDateType) gradValue;
            Assert.Equal( 2054, pdt.Year.Value);
        }

        [Fact]
        public void testParseOnTimeGradYearSS()
        {
            String sXML = "<StudentSnapshot StudentPersonalRefId='12345678901234567890'>"
                          + " <GradYear Type='Original'>1971</GradYear>"
                          + "</StudentSnapshot>";

            StudentSnapshot sp = (StudentSnapshot) parseSIF15r1XML( sXML );
            sp = (StudentSnapshot) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF15r1 );
            Assert.NotNull( sp );
            Assert.NotNull( sp.OnTimeGraduationYear);
            Assert.Equal( 1971, (int) sp.OnTimeGraduationYear);

            sp = Objects.Create<StudentSnapshot>();
            sp.SetElementOrAttribute( "GradYear[@Type='Original']", "8877" );
            Assert.NotNull( sp.OnTimeGraduationYear);
            Assert.Equal( 8877, (int) sp.OnTimeGraduationYear);

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Original']" );
            Assert.NotNull( gradValue);
            SifInt intValue = (SifInt) gradValue.SifValue;
            Assert.Equal( 8877, intValue.RawValue);
        }

        [Fact]
        public void testParseProjectedGradYearSS()
        {
            String sXML = "<StudentSnapshot StudentPersonalRefId='12345678901234567890'>"
                          + " <GradYear Type='Projected'>2012</GradYear>"
                          + "</StudentSnapshot>";

            StudentSnapshot sp = (StudentSnapshot) parseSIF15r1XML( sXML );
            sp = (StudentSnapshot) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF15r1 );
            Assert.NotNull( sp );
            Assert.NotNull( sp.ProjectedGraduationYear);
            Assert.Equal( 2012, (int) sp.ProjectedGraduationYear);

            sp = Objects.Create<StudentSnapshot>();
            sp.SetElementOrAttribute( "GradYear[@Type='Projected']", "2089" );
            Assert.NotNull( sp.ProjectedGraduationYear);
            Assert.Equal( 2089, (int) sp.ProjectedGraduationYear);

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Projected']" );
            Assert.NotNull( gradValue);
            SifInt intValue = (SifInt) gradValue.SifValue;
            Assert.Equal( 2089, intValue.Value.Value);
        }

        [Fact]
        public void testParseGraduationDateSS()
        {
            String sXML = "<StudentSnapshot StudentPersonalRefId='12345678901234567890'>"
                          + " <GradYear Type='Actual'>2005</GradYear>"
                          + "</StudentSnapshot>";

            StudentSnapshot sp = (StudentSnapshot) parseSIF15r1XML( sXML );
            sp = (StudentSnapshot) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF15r1 );
            Assert.NotNull( sp );
            PartialDateType gd = sp.GraduationDate;
            Assert.NotNull( gd);
            Assert.Equal( 2005, (int) gd.Year);

            sp = Objects.Create<StudentSnapshot>();
            sp.SetElementOrAttribute( "GradYear[@Type='Actual']", "2054" );
            gd = sp.GraduationDate;
            Assert.NotNull( gd);
            Assert.Equal( 2054, (int) gd.Year);

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Actual']" );
            Assert.NotNull( gradValue);
            Assert.True( gradValue is PartialDateType);
            PartialDateType gradYear = (PartialDateType) gradValue;
            Assert.Equal( 2054, gradYear.Year.Value);
        }

        private SifElement parseSIF15r1XML( String xml )
        {
            SifParser parser = new SifParser(Runtime);
            return parser.Parse( xml, null, 0, SifVersion.SIF15r1 );
        }
    }
}
