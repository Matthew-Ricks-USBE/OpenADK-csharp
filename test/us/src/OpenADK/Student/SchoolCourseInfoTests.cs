using System;
using System.IO;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using OpenADK.Library.Tools.XPath;
using Xunit;

namespace Library.Nunit.US.Library.Student
{
    
    public class SchoolCourseInfoTests : UsAdkTest
    {
        [Fact]
        public void testCourseCodeSIF15r1()
        {
            Runtime.SifVersion = SifVersion.SIF15r1;
            SchoolCourseInfo sci = new SchoolCourseInfo();
            sci.SifVersion = Runtime.SifVersion;
            sci.SetCourseCredits( CreditType.C0108_0585, 2 );

            SifXPathContext spc = SifXPathContext.NewSIFContext( sci, Runtime.SifVersion );
            Element value = (Element) spc.GetValue( "CourseCredits[@Code='0585']" );

            SifSimpleType elementValue = value.SifValue;
            Assert.NotNull(elementValue);
            Assert.Equal("2", elementValue.RawValue?.ToString());
        }

        [Fact]
        public void testSubjectAreaSIF15r1()
        {
            Runtime.SifVersion = SifVersion.SIF15r1;
            SchoolCourseInfo sci = new SchoolCourseInfo();
            sci.SifVersion = Runtime.SifVersion;
            SubjectAreaList lst = new SubjectAreaList();
            sci.SubjectAreaList = lst;

            SubjectArea sa = new SubjectArea( "13" );
            sa.TextValue = "Graphic Arts"; // for SIF 1.x ???
            OtherCodeList ocl = new OtherCodeList();
            ocl.Add( new OtherCode( Codeset.TEXT, "Graphic Arts" ) );
            sa.OtherCodeList = ocl;
            lst.Add( sa );

            StringWriter sw = new StringWriter();
            SifWriter sifw = new SifWriter( sw, Runtime );
            sifw.Write( sci );
            sifw.Flush();
            sifw.Close();

            String xml = sw.ToString();
            Console.WriteLine( xml );

            int found = xml.IndexOf( ">Graphic Arts</SubjectArea>" );
            Assert.True( found > -1 );
        }

        [Fact]
        public void testCourseCodeSIF20()
        {
            Runtime.SifVersion = SifVersion.SIF20;
            SchoolCourseInfo sci = new SchoolCourseInfo();
            sci.SifVersion = Runtime.SifVersion;
            sci.SetCourseCredits( CreditType.C0108_0585, 2 );

            SifXPathContext spc = SifXPathContext.NewSIFContext( sci, Runtime.SifVersion );
            Element value = (Element) spc.GetValue( "CourseCredits[@Type='0585']" );
            Assert.NotNull(value);

            SifSimpleType elementValue = value.SifValue;
            Assert.NotNull( elementValue);
            Assert.Equal("2", elementValue.RawValue?.ToString());
        }
    }
}
