using System;
using OpenADK.Library;
using Xunit;
using Library.UnitTesting.Framework;

namespace OpenADK.Utils
{
    
    public class UUIDTests : AdkTest
    {
        [Fact]
        public void testAssertSIFGUIDFormat()
        {
            String refId = Runtime.MakeGuid();
            Console.WriteLine(refId);
            assertRefId(refId);
        }

        [Fact]
        public void testConvertUUIDToRefId()
        {
            String str = "f81d4fae-7dec-11d0-a765-00a0c91e6bf6";
            Guid guid = new Guid(str);
            String adkGuid = SifFormatter.GuidToSifRefID(guid);
            assertRefId(adkGuid);
            Assert.Equal("F81D4FAE7DEC11D0A76500A0C91E6BF6", adkGuid);
        }

        [Fact]
        public void testConvertRefIdtoUUID()
        {
            String adkGuid = "F81D4FAE7DEC11D0A76500A0C91E6BF6";
            Guid? guid = SifFormatter.SifRefIDToGuid(adkGuid);
            Assert.Equal(new Guid("f81d4fae-7dec-11d0-a765-00a0c91e6bf6"), guid.Value);
        }

        /// <summary>
        /// Asserts that the refId is in the proper format
        /// </summary>
        /// <param name="refId"></param>
        private void assertRefId(String refId)
        {
            Assert.Equal(32, refId.Length);

            int pos = refId.IndexOf("-");
            Assert.Equal(-1, pos);

            // Assert case
            Assert.Equal(refId, refId.ToUpperInvariant());
        }
    }
}
