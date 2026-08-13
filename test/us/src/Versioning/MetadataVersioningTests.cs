using System;
using System.Collections.Generic;
using System.Text;
using OpenADK.Library;
using OpenADK.Library.us.Library;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.Nunit.US.Versioning
{
    public class MetadataVersioningTests : AdkTest
    {
        public MetadataVersioningTests()
        {
            Runtime.Initialize();
        }

        [Fact]
        public void testTagNameChanges()
        {
            // Create a Transaction element and show its version-dependent tag name
            Transaction trans = new Transaction();

            assertTransactionInfo(trans);

            TransactionList tl = new TransactionList();
            tl.Add(trans);
            assertTransactionInfo(trans);

        }

        private void assertTransactionInfo(Transaction trans)
        {
            Assert.Equal("Transaction", trans.ElementDef.Name);

            // assert the tag name for SIF 1.1: “CircTx”
            Assert.Equal("CircTx", trans.ElementDef.Tag(SifVersion.SIF11));

            // assert the tag name for SIF 1.5: “CircTx”
            Assert.Equal("CircTx", trans.ElementDef.Tag(SifVersion.Parse("1.5")));

            // assert the tag name for SIF 1.5r1: “CircTx”
            Assert.Equal("CircTx", trans.ElementDef.Tag(SifVersion.SIF15r1));

            // assert the tag name for SIF 2.0: “Transaction”
            Assert.Equal("Transaction", trans.ElementDef.Tag(SifVersion.SIF20));

            // assert the tag name for SIF 2.1: “Transaction”
            Assert.Equal("Transaction", trans.ElementDef.Tag(SifVersion.SIF21));

        }
    }
}
