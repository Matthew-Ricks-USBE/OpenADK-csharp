using OpenADK.Library;
using OpenADK.Library.us.Common;
using Xunit;
using OpenADK.Library.us;
using Library.UnitTesting.Framework;

namespace Library.Nunit.Core
{
    
    public class SifListTests : AdkTest
    {
        [Fact]
        public void testList010()
        {
            EmailList el = new EmailList();

            // Using the generic "Wrap" API so that we can use this
            // test against any internationalized version of the ADK
            Email email1 = new Email(EmailType.Wrap("foo"), "email1@OpenADK.com");
            Email email2 = new Email(EmailType.Wrap("foo"), "email2@OpenADK.com");

            el.Add(email1);
            Assert.Equal(1, el.ChildCount);

            el.Add(email2);
            Assert.Equal(2, el.ChildCount);

            Email[] children = el.ToArray();
            Assert.Equal(2, children.Length);

            el.RemoveChild(email2);
            Assert.Equal(1, el.ChildCount);

            el.RemoveChild(email1);
            Assert.Equal(0, el.ChildCount);

            children = el.ToArray();
            Assert.Empty(children);
        }


        [Fact]
        public void testList020()
        {
            EmailList el = new EmailList();

            // Using the generic "Wrap" API so that we can use this
            // test against any internationalized version of the ADK
            Email email1 = new Email(EmailType.Wrap("foo"), "email1@OpenADK.com");
            Email email2 = new Email(EmailType.Wrap("foo"), "email2@OpenADK.com");


            el.Add(email1);
            el.Add(email2);

            Assert.NotNull(email1.Parent);
            Assert.NotNull(email2.Parent);

            el.Clear();
            Assert.Equal(0, el.ChildCount);
            Assert.Null(email1.Parent);
            Assert.Null(email2.Parent);
        }

        [Fact]
        public void testList030()
        {
            EmailList el = new EmailList();

            // Using the generic "Wrap" API so that we can use this
            // test against any internationalized version of the ADK
            Email email1 = new Email(EmailType.Wrap("foo"), "email1@OpenADK.com");
            Email email2 = new Email(EmailType.Wrap("bar"), "email2@OpenADK.com");

            el.Add(email1);
            el.Add(email2);

            // test the iterator
            int count = 0;
            foreach (Email e in el)
            {
                Assert.NotNull(e);
                Assert.Equal(18, e.TextValue.Length);
                count++;
            }

            Assert.Equal(2, count);
        }
    }
}
