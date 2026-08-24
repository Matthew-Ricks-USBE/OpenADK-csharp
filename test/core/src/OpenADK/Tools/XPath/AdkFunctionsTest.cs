using OpenADK.Library.Tools.XPath;
using Xunit;

namespace Library.xUnit.Core.Tools.XPath
{
    
    public class AdkFunctionsTest
    {
        [Fact]
        public void testToUpper()
        {
            Assert.Equal("ABC DEFG", AdkFunctions.toUpperCase("abc defg"));
        }

        [Fact]
        public void testToLower()
        {
            Assert.Equal("abc defg", AdkFunctions.toLowerCase("aBc dEfg"));
        }

        [Fact]
        public void testPad()
        {
            Assert.Equal("Hello World", AdkFunctions.padEnd("Hello World", "*", 11));
            Assert.Equal("Hello World", AdkFunctions.padEnd("Hello World", "*", 5));
            Assert.Equal("Hello World****", AdkFunctions.padEnd("Hello World", "*", 15));

            Assert.Equal("Hello World", AdkFunctions.padBegin("Hello World", "*", 11));
            Assert.Equal("Hello World", AdkFunctions.padBegin("Hello World", "*", 5));
            Assert.Equal("****Hello World", AdkFunctions.padBegin("Hello World", "*", 15));
        }

        [Fact]
        public void testToProperCase()
        {
            Assert.Equal("Hello World", AdkFunctions.toProperCase("hello World"));
            Assert.Equal("Hello O'Reilly", AdkFunctions.toProperCase("HELLO o'reilly"));
            Assert.Equal("Old O'Leary", AdkFunctions.toProperCase("old o'leary"));
            Assert.Equal("Old\tO'Leary", AdkFunctions.toProperCase("old\to'LEARY"));
        }
    }
}

