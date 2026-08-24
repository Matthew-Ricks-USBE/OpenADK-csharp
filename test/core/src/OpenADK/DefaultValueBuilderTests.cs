using OpenADK.Library;
using Xunit;

namespace Library.xUnit.Core
{
    /// <summary>
    /// Summary description for DefaultValueBuilderTests.
    /// </summary>
    
    public class DefaultValueBuilderTests
    {
        [Fact]
        public void ParseResultsSimple()
        {
            string starter = "@random() @strip(\"(801) 323-1131\")";
            int pos = 10;

            DefaultValueBuilder.ParseResults results = DefaultValueBuilder.ParseResults.parse(starter, pos);

            Assert.Equal(34, results.Position);
            Assert.Equal("strip", results.MethodName);
            Assert.Equal(1, results.Parameters.TokenCount);
            Assert.Equal("(801) 323-1131", results.Parameters.GetToken(0));
        }

        [Fact]
        public void parseSyncMacro()
        {
            string starter = "@syncmatchname(first,middle,last)";
            int pos = 0;

            DefaultValueBuilder.ParseResults results = DefaultValueBuilder.ParseResults.parse(starter, pos);

            Assert.Equal("syncmatchname", results.MethodName);
            Assert.Equal(3, results.Parameters.TokenCount);
            Assert.Equal("first", results.Parameters.GetToken(0));
            Assert.Equal("middle", results.Parameters.GetToken(1));
            Assert.Equal("last", results.Parameters.GetToken(2));
        }
    }
}