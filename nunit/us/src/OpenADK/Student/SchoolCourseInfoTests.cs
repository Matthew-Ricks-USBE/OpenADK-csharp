using System;
using System.IO;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using OpenADK.Library.Tools.XPath;
using NUnit.Framework;

namespace Library.Nunit.US.Library.Student
{
    [TestFixture]
    public class SchoolCourseInfoTests : UsAdkTest
    {
        [Test]
        public void testCourseCodeSIF15r1()
        {
            Adk.SifVersion = SifVersion.SIF15r1;
            SchoolCourseInfo sci = new SchoolCourseInfo();
            sci.SetCourseCredits( CreditType.C0108_0585, 2 );

            SifXPathContext spc = SifXPathContext.NewSIFContext( sci );
            Element value = (Element) spc.GetValue( "CourseCredits[@Code='0585']" );

            SifSimpleType elementValue = value.SifValue;
            Assert.IsNotNull(elementValue,  "Value by XPath");
            Assert.AreEqual(2, elementValue.RawValue, "Value By XPath");
        }

        [Test]
        public void testSubjectAreaSIF15r1()
        {
            Adk.SifVersion = SifVersion.SIF15r1;
            SchoolCourseInfo sci = new SchoolCourseInfo();
            SubjectAreaList lst = new SubjectAreaList();
            sci.SubjectAreaList = lst;

            SubjectArea sa = new SubjectArea( "13" );
            sa.TextValue = "Graphic Arts"; // for SIF 1.x ???
            OtherCodeList ocl = new OtherCodeList();
            ocl.Add( new OtherCode( Codeset.TEXT, "Graphic Arts" ) );
            sa.OtherCodeList = ocl;
            lst.Add( sa );

            StringWriter sw = new StringWriter();
            SifWriter sifw = new SifWriter( sw );
            sifw.Write( sci );
            sifw.Flush();
            sifw.Close();

            String xml = sw.ToString();
            Console.WriteLine( xml );

            int found = xml.IndexOf( ">Graphic Arts</SubjectArea>" );
            Assert.True( found > -1 );
        }

        [Test]
        public void testCourseCodeSIF20()
        {
            Adk.SifVersion = SifVersion.SIF20;
            SchoolCourseInfo sci = new SchoolCourseInfo();
            sci.SetCourseCredits( CreditType.C0108_0585, 2 );

            SifXPathContext spc = SifXPathContext.NewSIFContext( sci );
            Element value = (Element) spc.GetValue( "CourseCredits[@Type='0585']" );
            Assert.IsNotNull(value, "Value by XPath");

            SifSimpleType elementValue = value.SifValue;
            Assert.IsNotNull( elementValue, "Value by XPath" );
            Assert.AreEqual( 2, elementValue.RawValue, "Value By XPath" );
        }
    }
}
