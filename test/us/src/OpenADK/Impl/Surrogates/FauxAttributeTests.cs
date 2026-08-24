using System.Xml.XPath;
using OpenADK.Library;
using OpenADK.Library.Impl.Surrogates;
using OpenADK.Library.us.Student;
using OpenADK.Library.Tools.XPath;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.xUnit.US.Library.Impl.Surrogates
{
    
    public class FauxAttributeTests : AdkTest
    {
        [Fact]
        public void testFauxAttribute010()
        {

            SifElementPointer sep = new SifElementPointer(null, new StudentPersonal(), SifVersion.SIF15r1);
            FauxAttribute fa = new FauxAttribute(sep, "Type", "Projected");


            // Assert base functionality
            Assert.Equal( "Type", fa.Name );
            Assert.Equal("Projected", fa.Value);
            Assert.Equal(sep, fa.Parent );

            // Assert XPath functionality
            Assert.Equal( XPathNodeType.Attribute, fa.NodeType );


        }
    }
}
