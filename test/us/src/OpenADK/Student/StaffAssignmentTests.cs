using System;
using System.Collections.Generic;
using System.Text;
using OpenADK.Library;
using OpenADK.Library.us.Student;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.xUnit.US.Library.Student
{
    
    public class StaffAssignmentTests
    {
        [Fact]
        public void testReadWriteStaffAssignmentSIF1x()
		{
			StaffAssignment sa = ObjectCreator.CreateStaffAssignment();
			Console.WriteLine(sa.GetContent().Count );

			sa = (StaffAssignment) AdkObjectParseHelper.WriteParseAndReturn(sa,
					SifVersion.SIF15r1);
			Assert.Null(sa.PrimaryAssignment);
		}

		[Fact]
		public void testReadWriteStaffAssignmentSIF2x()
		{
			StaffAssignment sa = ObjectCreator.CreateStaffAssignment();
			sa = (StaffAssignment) AdkObjectParseHelper.WriteParseAndReturn(sa,
					SifVersion.SIF20r1);

			Assert.Equal("Yes", sa.PrimaryAssignment);

		}
    }
}

