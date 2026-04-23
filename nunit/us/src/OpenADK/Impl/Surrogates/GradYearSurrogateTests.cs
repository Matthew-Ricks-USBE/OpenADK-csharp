using System;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using NUnit.Framework;
using Library.UnitTesting.Framework;
using OpenADK.Library.us;

namespace Library.Nunit.US.Library.Impl.Surrogates
{
    [TestFixture]
    public class GradYearSurrogateTests
    {
        protected SifVersion fOriginalVersion;

        [SetUp]
        public void SetUp()
        {
            fOriginalVersion = Adk.SifVersion;
            Adk.Initialize(SifVersion.SIF15r1, SIFVariant.SIF_US, (int)SdoLibraryType.Student);
            Adk.SifVersion = SifVersion.SIF15r1;
        }

        [TearDown]
        public void TearDown()
        {
            Adk.SifVersion = fOriginalVersion;
        }

        [Test]
        public void testParseOnTimeGradYear()
        {
            String sXML = "<StudentPersonal RefId='12345678901234567890'>"
                          + " <GradYear Type='Original'>1971</GradYear>"
                          + "</StudentPersonal>";

            StudentPersonal sp = (StudentPersonal) parseSIF15r1XML( sXML );
            sp = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF11 );
            Assert.IsNotNull( sp );
            Assert.IsNotNull(sp.OnTimeGraduationYear, "On Time Grad Year");
            Assert.AreEqual(1971, (int) sp.OnTimeGraduationYear, "On Time Grad Year");

            sp = new StudentPersonal();
            sp.SetElementOrAttribute( "GradYear[@Type='Original']", "8877" );
            Assert.IsNotNull(sp.OnTimeGraduationYear, "On Time Grad Year");
            Assert.AreEqual(8877, (int) sp.OnTimeGraduationYear, "On Time Grad Year");

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Original']" );
            Assert.IsNotNull( gradValue, "On Time Grad Year" );
            int gradYear = (int) gradValue.SifValue.RawValue;
            Assert.IsNotNull(gradYear, "On Time Grad Year");
            Assert.AreEqual(8877, gradYear, "On Time Grad Year");
        }

        public void testParseProjectedGradYear()
        {
            String sXML = "<StudentPersonal RefId='12345678901234567890'>"
                          + " <GradYear Type='Projected'>2012</GradYear>"
                          + "</StudentPersonal>";

            StudentPersonal sp = (StudentPersonal) parseSIF15r1XML( sXML );
            sp = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF11 );
            Assert.IsNotNull( sp );
            Assert.IsNotNull( sp.ProjectedGraduationYear, "Projected Grad Year" );
            Assert.AreEqual( 2012, (int) sp.ProjectedGraduationYear, "Projected Grad Year" );

            sp = new StudentPersonal();
            sp.SetElementOrAttribute( "GradYear[@Type='Projected']", "2089" );
            Assert.IsNotNull( sp.ProjectedGraduationYear, "Projected Grad Year" );
            Assert.AreEqual( 2089, (int) sp.ProjectedGraduationYear, "Projected Grad Year" );

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Projected']" );
            Assert.IsNotNull( gradValue, "Projected Grad Year" );
            int gradYear = (int) gradValue.SifValue.RawValue;
            Assert.IsNotNull( gradYear, "Projected Grad Year" );
            Assert.AreEqual( 2089, gradYear, "Projected Grad Year" );
        }

        public void testParseGraduationDate()
        {
            String sXML = "<StudentPersonal RefId='12345678901234567890'>"
                          + " <GradYear Type='Actual'>2005</GradYear>"
                          + "</StudentPersonal>";

            StudentPersonal sp = (StudentPersonal) parseSIF15r1XML( sXML );
            sp = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF11 );
            Assert.IsNotNull( sp );
            PartialDateType gd = sp.GraduationDate;
            Assert.IsNotNull( gd, "Actual Grad Year" );
            Assert.AreEqual( 2005, (int) gd.Year, "Actual Grad Year" );

            sp = new StudentPersonal();
            sp.SetElementOrAttribute( "GradYear[@Type='Actual']", "2054" );
            gd = sp.GraduationDate;
            Assert.IsNotNull( gd, "Actual Grad Year" );
            Assert.IsNotNull( gd.Year, "GraduationDate/getYear()" );
            Assert.AreEqual( 2054, gd.Year.Value, "Actual Grad Year" );

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Actual']" );
            Assert.IsNotNull( gradValue, "Actual Grad Year" );
            PartialDateType pdt = (PartialDateType) gradValue;
            Assert.AreEqual( 2054, pdt.Year.Value, "Actual Grad Year" );
        }

        public void testParseOnTimeGradYearSS()
        {
            String sXML = "<StudentSnapshot StudentPersonalRefId='12345678901234567890'>"
                          + " <GradYear Type='Original'>1971</GradYear>"
                          + "</StudentSnapshot>";

            StudentSnapshot sp = (StudentSnapshot) parseSIF15r1XML( sXML );
            sp = (StudentSnapshot) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF15r1 );
            Assert.IsNotNull( sp );
            Assert.IsNotNull( sp.OnTimeGraduationYear, "On Time Grad Year" );
            Assert.AreEqual( 1971, (int) sp.OnTimeGraduationYear, "On Time Grad Year" );

            sp = new StudentSnapshot();
            sp.SetElementOrAttribute( "GradYear[@Type='Original']", "8877" );
            Assert.IsNotNull( sp.OnTimeGraduationYear, "On Time Grad Year" );
            Assert.AreEqual( 8877, (int) sp.OnTimeGraduationYear, "On Time Grad Year" );

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Original']" );
            Assert.IsNotNull( gradValue, "On Time Grad Year is null" );
            SifInt intValue = (SifInt) gradValue.SifValue;
            Assert.AreEqual( 8877, intValue.RawValue, "On Time Grad Year" );
        }

        public void testParseProjectedGradYearSS()
        {
            String sXML = "<StudentSnapshot StudentPersonalRefId='12345678901234567890'>"
                          + " <GradYear Type='Projected'>2012</GradYear>"
                          + "</StudentSnapshot>";

            StudentSnapshot sp = (StudentSnapshot) parseSIF15r1XML( sXML );
            sp = (StudentSnapshot) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF15r1 );
            Assert.IsNotNull( sp );
            Assert.IsNotNull( sp.ProjectedGraduationYear, "Projected Grad Year" );
            Assert.AreEqual( 2012, (int) sp.ProjectedGraduationYear, "Projected Grad Year" );

            sp = new StudentSnapshot();
            sp.SetElementOrAttribute( "GradYear[@Type='Projected']", "2089" );
            Assert.IsNotNull( sp.ProjectedGraduationYear, "Projected Grad Year" );
            Assert.AreEqual( 2089, (int) sp.ProjectedGraduationYear, "Projected Grad Year" );

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Projected']" );
            Assert.IsNotNull( gradValue, "Projected Grad Year" );
            SifInt intValue = (SifInt) gradValue.SifValue;
            Assert.AreEqual( 2089, intValue.Value.Value, "Projected Grad Year" );
        }

        public void testParseGraduationDateSS()
        {
            String sXML = "<StudentSnapshot StudentPersonalRefId='12345678901234567890'>"
                          + " <GradYear Type='Actual'>2005</GradYear>"
                          + "</StudentSnapshot>";

            StudentSnapshot sp = (StudentSnapshot) parseSIF15r1XML( sXML );
            sp = (StudentSnapshot) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.SIF15r1 );
            Assert.IsNotNull( sp );
            PartialDateType gd = sp.GraduationDate;
            Assert.IsNotNull( gd, "Actual Grad Year" );
            Assert.AreEqual( 2005, (int) gd.Year, "Actual Grad Year" );

            sp = new StudentSnapshot();
            sp.SetElementOrAttribute( "GradYear[@Type='Actual']", "2054" );
            gd = sp.GraduationDate;
            Assert.IsNotNull( gd, "Actual Grad Year" );
            Assert.AreEqual( 2054, (int) gd.Year, "Actual Grad Year" );

            Element gradValue = sp.GetElementOrAttribute( "GradYear[@Type='Actual']" );
            Assert.IsNotNull( gradValue, "Actual Grad Year" );
            Assert.IsTrue( gradValue is PartialDateType, "Should be a partial date type" );
            PartialDateType gradYear = (PartialDateType) gradValue;
            Assert.AreEqual( 2054, gradYear.Year.Value, "Actual Grad Year" );
        }

        private SifElement parseSIF15r1XML( String xml )
        {
            SifParser parser = SifParser.NewInstance();
            return parser.Parse( xml, null, 0, SifVersion.SIF15r1 );
        }
    }
}
