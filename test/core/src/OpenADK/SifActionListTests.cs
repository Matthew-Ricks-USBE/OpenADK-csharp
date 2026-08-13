using OpenADK.Library;
using OpenADK.Library.us.Common;
using Xunit;
using OpenADK.Library.us;
using Library.UnitTesting.Framework;

namespace Library.Nunit.Core
{
    
    public class SifActionListTests : AdkTest
    {
        [Fact]
        public void testList010()
        {
            EmailList el = new EmailList();

            // Using the generic "Wrap" API so that we can use this
            // test against any internationalized version of the ADK
            Email email1 = new Email(EmailType.Wrap("foo"), "email1@OpenADK.com");
            Email email2 = new Email(EmailType.Wrap("bar"), "email2@OpenADK.com");

            el.Add(email1);
            Assert.Equal(1, el.ChildCount);

            el.Add(email2);
            Assert.Equal(2, el.ChildCount);

            Email[] children = el.ToArray();
            Assert.Equal(2, children.Length);


            Assert.True( el.Remove( EmailType.Wrap("foo") ), "Should have removed the email") ;
            Assert.Equal(1, el.ChildCount);

            el.RemoveChild(CommonDTD.EMAILLIST_EMAIL, email2.Key);
            Assert.Equal(0, el.ChildCount);
        }

        [Fact]
        public void testList020()
        {
            EmailList el = new EmailList();

            // Using the generic "Wrap" API so that we can use this
            // test against any internationalized version of the ADK
            Email email1 = new Email( EmailType.Wrap( "asdfasdf" ), "email1@OpenADK.com" );
            Email email2 = new Email( EmailType.Wrap( "Primary" ), "email2@OpenADK.com" );

            

            el.Add(email1);
            Assert.Equal(1, el.ChildCount);

            el.Add(email2);
            Assert.Equal(2, el.ChildCount);


            Email email3 = new Email();
            email3.Type = "Alternate1";
            el.Add(email3);
            Assert.Equal(3, el.ChildCount);



            Email primary = el[EmailType.PRIMARY];
            Assert.NotNull( primary );

            primary = el["Primary"];
            Assert.NotNull(primary);


            Email secondary = el[EmailType.ALT1];
            Assert.NotNull(secondary);

            secondary = el["Alternate1"];
            Assert.NotNull(secondary);

        }
    }
}
