using System;
using System.Collections.Generic;
using System.Text;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using Xunit;
using Library.xUnit.US.Library.Tools;

namespace Library.xUnit.US.Library.Student
{
    
    public class SIF20StudentValidationTests : ValidationTest
    {
        public SIF20StudentValidationTests()
            : base(SifVersion.SIF20r1 )
        {
		
	}

    [Fact]
	public void testStudentSchoolEnrollment010() {

		SifElement se = readElementFromFile( "data/SIF20r1/StudentSchoolEnrollment/SIF20StudentSchoolEnrollment.xml", SifVersion.SIF20r1 );
		testSchemaElement( se );

	}

    [Fact]
	public void testStudentSchoolEnrollment020() {

        StudentSchoolEnrollment sse = new StudentSchoolEnrollment(Runtime.MakeGuid(), Runtime.MakeGuid(), Runtime.MakeGuid(), MembershipType.HOME, TimeFrame.CURRENT);
		sse.SchoolYear = 2008;
		sse.SifVersion = SifVersion.SIF20r1;
	    DateTime entryDate = DateTime.Now;
		sse.EntryDate = entryDate;
		sse.computeTimeFrame( DateTime.Now );
		sse.Homeroom = new Homeroom( "RoomInfo", Runtime.MakeGuid() );
		sse.SetGradeLevel( GradeLevelCode.KG );
		testSchemaElement( sse );

	}

    [Fact]
	public void testStudentPersonal010() {

		SifElement se = readElementFromFile( "data/SIF20/StudentPersonal/SIF20StudentPersonal.xml", SifVersion.SIF20r1 );
		testSchemaElement( se );

	}

    }
}
