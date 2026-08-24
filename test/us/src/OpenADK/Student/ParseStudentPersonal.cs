using System;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.xUnit.US.Student
{
    /// <summary>
    /// Summary description for ParseStudentPersonal.
    /// </summary>
    
    public class ParseStudentPersonal : AdkTest
    {
       
        [Fact]
        public void SDOParse() {
		// Create a StudentPersonal
		StudentPersonal sp = ObjectCreator.CreateStudentPersonal();
		// Test changing the name
		sp.Name = new Name(NameType.BIRTH, "STUDENT", "JOE");

		sp = AdkObjectParseHelper.runParsingTest(sp, SifVersion.SIF15r1);

		// Test to ensure that Email is not a child of StudentPersonal
		Assert.Empty(sp.GetChildList( CommonDTD.EMAIL));
		Assert.NotNull(sp.EmailList);
        Assert.True(sp.EmailList.ChildCount > 0);

		sp = AdkObjectParseHelper.runParsingTest(sp, SifVersion.SIF20);

		// Test to ensure that Email is not a child of StudentPersonal
		Assert.Empty(sp.GetChildList(CommonDTD.EMAIL));
        Assert.NotNull(sp.EmailList);
        Assert.True(sp.EmailList.ChildCount > 0);
		sp = AdkObjectParseHelper.runParsingTest(sp, SifVersion.SIF11);

		// Test to ensure that Email is not a child of StudentPersonal
		Assert.Empty(sp.GetChildList(CommonDTD.EMAIL));
        Assert.NotNull(sp.EmailList);
        Assert.True(sp.EmailList.ChildCount > 0);

		sp = AdkObjectParseHelper.runParsingTest(sp, SifVersion.SIF22);

		// Test to ensure that Email is not a child of StudentPersonal
		Assert.Empty(sp.GetChildList(CommonDTD.EMAIL));
        Assert.NotNull(sp.EmailList);
        Assert.True(sp.EmailList.ChildCount > 0);

	}

    }
}


