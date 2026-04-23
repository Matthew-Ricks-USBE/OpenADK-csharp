using OpenADK.Library.Tools.XPath;
using NUnit.Framework;

namespace Library.Nunit.Core.Tools.XPath
{
    [TestFixture]
    public class AdkFunctionsTest
    {
        [Test]
        public void testToUpper()
        {
            Assert.AreEqual("ABC DEFG", AdkFunctions.toUpperCase("abc defg"));
        }

        [Test]
        public void testToLower()
        {
            Assert.AreEqual("abc defg", AdkFunctions.toLowerCase("aBc dEfg"));
        }

        [Test]
        public void testPad()
        {
            Assert.AreEqual("Hello World", AdkFunctions.padEnd("Hello World", "*", 11));
            Assert.AreEqual("Hello World", AdkFunctions.padEnd("Hello World", "*", 5));
            Assert.AreEqual("Hello World****", AdkFunctions.padEnd("Hello World", "*", 15));

            Assert.AreEqual("Hello World", AdkFunctions.padBegin("Hello World", "*", 11));
            Assert.AreEqual("Hello World", AdkFunctions.padBegin("Hello World", "*", 5));
            Assert.AreEqual("****Hello World", AdkFunctions.padBegin("Hello World", "*", 15));
        }

        [Test]
        public void testToProperCase()
        {
            Assert.AreEqual("Hello World", AdkFunctions.toProperCase("hello World"));
            Assert.AreEqual("Hello O'Reilly", AdkFunctions.toProperCase("HELLO o'reilly"));
            Assert.AreEqual("Old O'Leary", AdkFunctions.toProperCase("old o'leary"));
            Assert.AreEqual("Old\tO'Leary", AdkFunctions.toProperCase("old\to'LEARY"));
        }
    }
}

