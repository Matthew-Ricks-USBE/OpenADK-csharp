using System;
using System.Collections.Generic;
using System.Text;
using OpenADK.Library;
using Xunit;
using Library.xUnit.US.Library.Tools;

namespace Library.xUnit.US.Library.Student
{
    
    public class SIF15r1StudentValidationTests : ValidationTest
    {
        
	public SIF15r1StudentValidationTests() : base( SifVersion.SIF15r1 ) {
	}

    [Fact]
	public void testStudentSchoolEnrollment010() {
		
		SifElement se = readElementFromFile( "data/SIF15r1/StudentSchoolEnrollment/StudentSchoolEnrollment.xml", SifVersion.SIF15r1 );
		testSchemaElement( se );
		
	}
    }
}
