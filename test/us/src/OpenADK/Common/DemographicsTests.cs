using System;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.Nunit.US.Common
{
    /// <summary>
    /// Summary description for DemographicsTests.
    /// </summary>
    
    public class DemographicsTests : AdkTest
    {
        /**
	 * Tests that accessors that accept a SIFDate can also accept a null value
	 */

        [Fact]
        public void testSettingNullCountryArrivalDate()
        {
            StudentPersonal sp = ObjectCreator.CreateStudentPersonal();
            Demographics d = sp.Demographics;
            d.CountryArrivalDate = new DateTime(1997, 5, 1);

            Assert.Equal(new DateTime(1997, 5, 1), d.CountryArrivalDate);
            d.CountryArrivalDate = null;
            Assert.Null(d.CountryArrivalDate);

            sp = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn(sp, Runtime.SifVersion);
            d = sp.Demographics;
            Assert.Null(d.CountryArrivalDate);
        }

        /**
		 * Asserts that ADKGen creates overloads to methods, such as setStatePr( string Code ) that
		 * also take the object. e.g. setStatePr( StatePr )
		 */

        [Fact]
        public void testSettingStatePrAndCountry()
        {
            StudentPersonal sp = ObjectCreator.CreateStudentPersonal();
            Demographics d = sp.Demographics;

            d.SetCountryOfBirth(CountryCode.US);
            d.SetStateOfBirth(StatePrCode.AR);


            Assert.Equal(CountryCode.US.Value, d.CountryOfBirth);
            Assert.Equal(StatePrCode.AR.Value, d.StateOfBirth);
        }
    }
}