using System;
using System.Collections;
using System.Data;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using OpenADK.Library.Tools.Queries;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.Nunit.US.Tools.Queries
{
    /// <summary>
    /// Summary description for SQLQueryFormatterTests.
    /// </summary>
    
    public class SQLQueryFormatterTests : AdkTest
    {
        
        public void Setup()
        {
            Runtime.Initialize();
        }

        [Fact]
        public void testSQLQueryFormatter010()
        {
            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition(CommonDTD.NAME_FIRSTNAME, ComparisonOperators.EQ, "Johnny");

            IDictionary fields = new Hashtable();
            fields[CommonDTD.NAME_FIRSTNAME] = new SQLField("vchFirstName", DbType.String);

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(q, fields);

            Assert.Equal("( vchFirstName = 'Johnny' )", sql);
        }

        [Fact]
        public void testSQLQueryFormatter020()
        {
           Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition(CommonDTD.NAME_FIRSTNAME, ComparisonOperators.EQ, "Johnny");

            IDictionary fields = new Hashtable();

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            try
            {
                String sql = formatter.Format(q, fields);
            }
            catch (QueryFormatterException)
            {
                // Expected because the map doesn't have an entry for the query condition
                return;
            }

            throw new Xunit.Sdk.XunitException("QueryFormatterException should have been thrown");
        }


        [Fact]
        public void testSQLQueryFormatter030()
        {
           Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition(CommonDTD.NAME_FIRSTNAME, ComparisonOperators.EQ, "Johnny");

            IDictionary fields = new Hashtable();

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(q, fields, false);
            Assert.Equal("( 1=1 )", sql);
        }

        [Fact]
        public void testSQLQueryFormatter050()
        {
           Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition("Demographics/RaceList/Race/Code", ComparisonOperators.EQ, "1002");

            // Convert the query to XML and back
            Query reparsed = QueryTests.SaveToXMLAndReparse(q, SifVersion.LATEST, Runtime);

            IDictionary fields = new Hashtable();
            fields["Demographics/RaceList/Race/Code"] =
                new SQLField("Users.vchFirstName{0998=I;0999=A;1000=B;1001=H;1002=W}",
                             DbType.String);

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(reparsed, fields);

            Assert.Equal( "( Users.vchFirstName = 'W' )", sql);
        }


        [Fact]
        public void testSQLQueryFormatter060()
        {
            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition("Name/FirstName", ComparisonOperators.GT, "Sally");

            // Convert the query to XML and back
            Query reparsed = QueryTests.SaveToXMLAndReparse(q, SifVersion.LATEST, Runtime);

            IDictionary fields = new Hashtable();
            fields["Name/FirstName"] =
                new SQLField("Users.FName", DbType.String);

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(reparsed, fields);

            Assert.Equal("( Users.FName > 'Sally' )", sql);
        }

        [Fact]
        public void testSQLQueryFormatter070()
        {
            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition("Name/FirstName", ComparisonOperators.LT, "Sally");

            // Convert the query to XML and back
            Query reparsed = QueryTests.SaveToXMLAndReparse(q, SifVersion.LATEST, Runtime);

            IDictionary fields = new Hashtable();
            fields["Name/FirstName"] =
                new SQLField("Users.FName", DbType.String);

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(reparsed, fields);

            Assert.Equal("( Users.FName < 'Sally' )", sql);
        }

        [Fact]
        public void testSQLQueryFormatter080()
        {
            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition("Name/FirstName", ComparisonOperators.NE, "Sally");

            // Convert the query to XML and back
            Query reparsed = QueryTests.SaveToXMLAndReparse(q, SifVersion.LATEST, Runtime);

            IDictionary fields = new Hashtable();
            fields["Name/FirstName"] =
                new SQLField("Users.FName", DbType.String);

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(reparsed, fields);

            Assert.Equal("( Users.FName != 'Sally' )", sql);
        }

        [Fact]
        public void testSQLQueryFormatter090()
        {
            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition("Name/FirstName", ComparisonOperators.GE, "Sally");

            // Convert the query to XML and back
            Query reparsed = QueryTests.SaveToXMLAndReparse(q, SifVersion.LATEST, Runtime);

            IDictionary fields = new Hashtable();
            fields["Name/FirstName"] =
                new SQLField("Users.FName", DbType.String);

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(reparsed, fields);

            Assert.Equal("( Users.FName >= 'Sally' )", sql);
        }

        [Fact]
        public void testSQLQueryFormatter100()
        {
            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            q.AddCondition("Name/FirstName", ComparisonOperators.LE, "Sally");

            // Convert the query to XML and back
            Query reparsed = QueryTests.SaveToXMLAndReparse(q, SifVersion.LATEST, Runtime);

            IDictionary fields = new Hashtable();
            fields["Name/FirstName"] =
                new SQLField("Users.FName", DbType.String);

            SQLQueryFormatter formatter = new SQLQueryFormatter();
            String sql = formatter.Format(reparsed, fields);

            Assert.Equal("( Users.FName <= 'Sally' )", sql);
        }




    }
}
