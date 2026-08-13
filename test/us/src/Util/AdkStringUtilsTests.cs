using System;
using Xunit;

namespace OpenADK.Util
{
    /// <summary>
    /// Summary description for AdkStringUtilsTests.
    /// </summary>
    
    public class AdkStringUtilsTests
    {
        [Fact]
        public void EncodeXML()
        {
            String xml = null;
            Assert.Null(AdkStringUtils.EncodeXml(xml));

            xml = "<Hello>";
            Assert.Equal("&lt;Hello&gt;", AdkStringUtils.EncodeXml(xml));
        }

        [Fact]
        public void UnencodeXML()
        {
            String xml = null;
            Assert.Null(AdkStringUtils.UnencodeXml(xml));

            xml = "&lt;Hello&gt;";
            Assert.Equal("<Hello>", AdkStringUtils.UnencodeXml(xml));
        }

        [Fact]
        public void ReplaceFirstTests()
        {
            AssertReplaceFirst("Select??", "?", " Hello", "Select Hello?");
            AssertReplaceFirst("Select[?]", "[?]", " Hello", "Select Hello");
            AssertReplaceFirst("Hel?lo", "?", "-", "Hel-lo");
            AssertReplaceFirst("Hel[?]lo", "[?]", "-", "Hel-lo");
            AssertReplaceFirst("?Hel?lo", "?", "-", "-Hel?lo");
            AssertReplaceFirst("[?]Hel[?]lo", "[?]", "-", "-Hel[?]lo");
            AssertReplaceFirst("Hello[?][?]", "[?]", "-", "Hello-[?]");
        }

        private void AssertReplaceFirst(string source, string search, string replace, string expectedResult)
        {
            string result = AdkStringUtils.ReplaceFirst(source, search, replace);
            Assert.Equal(expectedResult, result);
        }
    }
}