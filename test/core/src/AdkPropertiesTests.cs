using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using OpenADK.Library;

namespace Library.xUnit.Core
{
    
    public class AdkPropertiesTests
    {
        [Fact]
        public void TestReturnDefaultValue()
        {
            AdkProperties props = new AdkProperties( null );
            props["TEST1"] = "Value1";

            Assert.Equal( "Value1", props["TEST1"] );
            Assert.Equal("Value1", props.GetProperty( "TEST1" ));
            Assert.Equal("Value1", props.GetProperty( "TEST1", "foo" ));
            Assert.Equal("foo", props.GetProperty( "bar", "foo" ));

        }

        [Fact]
        public void TestReturnDefaultValueWithInheritance()
        {
            AdkProperties parent = new AdkProperties(null);
            parent["TEST1"] = "Value1";

            AdkProperties props = new AdkProperties( parent );
            props["TEST2"] = "Value2";

            Assert.Equal("Value1", props["TEST1"]);
            Assert.Equal("Value2", props["TEST2"]);
            Assert.Equal("Value1", props.GetProperty("TEST1"));
            Assert.Equal("Value1", props.GetProperty("TEST1", "foo"));
            Assert.Equal("foo", props.GetProperty("bar", "foo"));

            props["TEST1"] = "scooter";
            Assert.Equal("scooter", props["TEST1"]);

        }

    }
}
