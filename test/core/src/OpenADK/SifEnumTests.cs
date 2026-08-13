using System;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;
using Xunit;

namespace Library.Nunit.Core
{
    /// <summary>
    /// Summary description for EnumTests.
    /// </summary>
    
    public class SifEnumTests
    {
        /**
	 * Test that overriding equals works as expected
	 */

        public void testEqualsOverride()
        {
            String assertedValue = "FOO";

            AddressType testEnum = AddressType.Wrap(assertedValue);
            // objects should be reflexively equal to themselves
            Assert.True(testEnum.Equals(testEnum));

            // test with null comparison
            Assert.False(testEnum.Equals(null));

            AddressType testEnum2 = AddressType.Wrap(assertedValue);
            Assert.True(testEnum.Equals(testEnum2));
            Assert.True(testEnum2.Equals(testEnum));

            // Test with a different enum value
            AddressType testEnum3 = AddressType.MAILING;
            Assert.False(testEnum3.Equals(testEnum));
            Assert.False(testEnum.Equals(testEnum3));

            // Test with a different enum type, but same value
            EmailType differentEnum = EmailType.Wrap(assertedValue);
            Assert.False(differentEnum.Equals(testEnum));
            Assert.False(testEnum.Equals(differentEnum));

            // Test with two null values
            AddressType nullEnum = AddressType.Wrap(null);
            AddressType nullEnum2 = AddressType.Wrap(null);
            Assert.True(nullEnum.Equals(nullEnum2));
            Assert.False(nullEnum.Equals(testEnum));
            Assert.False(testEnum.Equals(null));
        }

        /**
		 *  Test that overriding hashCode works as expected
		 */

        public void testHashCodeOverride()
        {
            String assertedValue = "FOO";

            AddressType testEnum = AddressType.Wrap(assertedValue);
            AddressType testEnum2 = AddressType.Wrap(assertedValue);
            Assert.Equal(testEnum.GetHashCode(), testEnum2.GetHashCode());
        }
    }
}