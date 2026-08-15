using System;
using OpenADK.Library.Tools.XPath.Compiler;
using Xunit;

namespace Library.Nunit.Core.Tools.XPath
{
    
    public class XPathParserTests
    {
        [Fact]
        public void TestXPath010()
        {
            AdkXPathStep[] steps = XPathParser.Parse("Foo");

            Assert.Single(steps);
            Assert.Equal("Foo", ((AdkNodeNameTest) steps[0].NodeTest).NodeName);

            steps = XPathParser.Parse("/Foo");
            Assert.Single(steps);
            Assert.Equal("Foo", ((AdkNodeNameTest) steps[0].NodeTest).NodeName);

            Console.WriteLine(steps[0]);
        }

        [Fact]
        public void TestXPath020()
        {
            AdkXPathStep[] steps = XPathParser.Parse("Foo/Bar/Win");

            Assert.Equal(3, steps.Length);
            Assert.Equal("Foo", ((AdkNodeNameTest) steps[0].NodeTest).NodeName);
            Assert.Equal("Bar", ((AdkNodeNameTest) steps[1].NodeTest).NodeName);
            Assert.Equal("Win", ((AdkNodeNameTest) steps[2].NodeTest).NodeName);

            steps = XPathParser.Parse("/Foo/Bar/Win");
            Assert.Equal(3, steps.Length);
            Assert.Equal("Foo", ((AdkNodeNameTest) steps[0].NodeTest).NodeName);
            Assert.Equal("Bar", ((AdkNodeNameTest) steps[1].NodeTest).NodeName);
            Assert.Equal("Win", ((AdkNodeNameTest) steps[2].NodeTest).NodeName);
        }

        [Fact]
        public void TestXPath030()
        {
            String path = "Foo[@Win='2']/Bar[Type=5]/Win[Zone=\"yada\"]";
            AdkXPathStep[] steps = XPathParser.Parse(path);

            Assert.Equal(3, steps.Length);
            AssertStep(steps[0], "Foo", "Win", "2");
            AssertStep(steps[1], "Bar", "Type", 5);
            AssertStep(steps[2], "Win", "Zone", "yada");

            Console.WriteLine(steps[0] + "/" + steps[1] + "/" + steps[2]);


            steps = XPathParser.Parse("/" + path);
            Assert.Equal(3, steps.Length);
            AssertStep(steps[0], "Foo", "Win", "2");
            AssertStep(steps[1], "Bar", "Type", 5);
            AssertStep(steps[2], "Win", "Zone", "yada");
        }


        [Fact]
        public void TestXPath040()
        {
            String path = "Foo[@Win='2']/Bar";
            AdkXPathStep[] steps = XPathParser.Parse(path);

            Assert.Equal(2, steps.Length);
            AssertStep(steps[0], "Foo", "Win", "2");
            Assert.Equal("Bar", ((AdkNodeNameTest) steps[1].NodeTest).NodeName);


            steps = XPathParser.Parse("/" + path);
            Assert.Equal(2, steps.Length);
            AssertStep(steps[0], "Foo", "Win", "2");
            Assert.Equal("Bar", ((AdkNodeNameTest) steps[1].NodeTest).NodeName);
        }

        private void AssertStep(AdkXPathStep step, String name, String singePredicateName, object singlePredicateValue)
        {
            Assert.Equal(name, ((AdkNodeNameTest) step.NodeTest).NodeName);
            Assert.NotNull(step.Predicates);
            Assert.Single(step.Predicates);
            Assert.IsType<AdkEqualOperation>(step.Predicates[0]);

            AdkExpression[] components = ((AdkEqualOperation) step.Predicates[0]).Arguments;
            Assert.Equal(2, components.Length);
            Assert.IsType<AdkLocPath>(components[0]);
            AdkLocPath lp = (AdkLocPath) components[0];

            AdkNodeNameTest attrName = (AdkNodeNameTest) lp.Steps[0].NodeTest;
            Assert.Equal(singePredicateName, attrName.NodeName);

            object value = components[1].ComputeValue(null);
            Assert.Equal(singlePredicateValue?.ToString(), value?.ToString());
        }
    }
}
