using System;
using System.IO;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;
using OpenADK.Library.Infra;
using OpenADK.Library.us.Infrastructure;
using OpenADK.Library.us.Reporting;
using OpenADK.Library.us.Student;
using Xunit;
using Library.UnitTesting.Framework;

namespace Library.xUnit.US
{
    /// <summary>
    /// Summary description for QueryTests.
    /// </summary>
    
    public class QueryTests : AdkTest
    {
        [Fact]
        public void AddFieldRestriction()
        {
            Query q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddFieldRestriction( StudentDTD.STUDENTPERSONAL_NAME );
            q.AddFieldRestriction( StudentDTD.STUDENTPERSONAL_ADDRESSLIST );

            IElementDef[] restrictions = q.FieldRestrictions;
            Assert.Equal( 2, restrictions.Length);
            Assert.Equal( StudentDTD.STUDENTPERSONAL_NAME, restrictions[0]);
            Assert.Equal( StudentDTD.STUDENTPERSONAL_ADDRESSLIST, restrictions[1]);
        }

        [Fact]
        public void TestToXml()
        {
            // From the javadoc example ...
            // Query for student where the Last Name is Jones and the First Name is
            // Bob, and the graduation year is 2004, 2005, or 2006
            ConditionGroup root = new ConditionGroup( GroupOperator.And );
            ConditionGroup grp1 = new ConditionGroup( GroupOperator.And );
            ConditionGroup grp2 = new ConditionGroup( GroupOperator.Or );

            // For nested elements, you cannot reference a SifDtd constant. Instead, use
            // the lookupElementDefBySQL function to lookup an IElementDef constant
            // given a SIF Query Pattern (SQP)
            IElementDef lname = Runtime.Dtd.LookupElementDefBySQP(
                StudentDTD.STUDENTPERSONAL, "Name/LastName" );
            IElementDef fname = Runtime.Dtd.LookupElementDefBySQP(
                StudentDTD.STUDENTPERSONAL, "Name/FirstName" );
            grp1.AddCondition( lname, ComparisonOperators.EQ, "Jones" );
            grp1.AddCondition( fname, ComparisonOperators.EQ, "Bob" );

            grp2.AddCondition( StudentDTD.STUDENTPERSONAL_ONTIMEGRADUATIONYEAR, ComparisonOperators.EQ, "2004" );
            grp2.AddCondition( StudentDTD.STUDENTPERSONAL_ONTIMEGRADUATIONYEAR, ComparisonOperators.EQ, "2005" );
            grp2.AddCondition( StudentDTD.STUDENTPERSONAL_ONTIMEGRADUATIONYEAR, ComparisonOperators.EQ, "2006" );

            // Add condition groups to the root group
            root.AddGroup( grp1 );
            root.AddGroup( grp2 );

            //	Query for student with the conditions prepared above by passing the
            //	root ConditionGroup to the constructor
            Query query = new Query( StudentDTD.STUDENTPERSONAL, root );
            query.AddFieldRestriction( StudentDTD.STUDENTPERSONAL_NAME );

            // Now, call toXML() on the query object, reparse back into a Query object and assert all values

            String sifQueryXML = query.ToXml( SifVersion.LATEST );
            Console.WriteLine( sifQueryXML );

            SifParser parser = new SifParser(Runtime);
            SIF_Request sifR = (SIF_Request) parser.Parse( "<SIF_Request>" + sifQueryXML + "</SIF_Request>", null );

            Query reparsedQuery = new Query( sifR.SIF_Query );

            Assert.Equal( StudentDTD.STUDENTPERSONAL, reparsedQuery.ObjectType);
            Assert.Single( reparsedQuery.FieldRestrictions);
            Assert.Equal( StudentDTD.STUDENTPERSONAL_NAME, reparsedQuery.FieldRestrictions[0]);


            ConditionGroup newRoot = reparsedQuery.RootConditionGroup;
            Assert.Equal( StudentDTD.STUDENTPERSONAL, reparsedQuery.ObjectType);
            Assert.Equal( GroupOperator.And, newRoot.Operator);


            ConditionGroup[] groups = reparsedQuery.RootConditionGroup.Groups;
            Assert.Equal( 2, groups.Length);
            Assert.Equal( GroupOperator.And, groups[0].Operator);
            Assert.Equal( GroupOperator.Or, groups[1].Operator);

            // Assert the first group conditions
            Condition[] newGrp1Conditions = groups[0].Conditions;
            Assert.Equal( 2, newGrp1Conditions.Length);

            // Assert the first condition
            Assert.Equal( ComparisonOperators.EQ, newGrp1Conditions[1].Operators);
            Assert.Equal( lname, newGrp1Conditions[0].Field);
            Assert.Equal( "Jones", newGrp1Conditions[0].Value);

            // Assert the second condition
            Assert.Equal( ComparisonOperators.EQ, newGrp1Conditions[0].Operators);
            Assert.Equal( fname, newGrp1Conditions[1].Field);
            Assert.Equal( "Bob", newGrp1Conditions[1].Value);

            // Assert the second group conditions
            Condition[] newGrp2Conditions = groups[1].Conditions;
            Assert.Equal( 3, newGrp2Conditions.Length);

            // Assert the first condition
            Assert.Equal( ComparisonOperators.EQ, newGrp2Conditions[0].Operators);
            Assert.Equal( StudentDTD.STUDENTPERSONAL_ONTIMEGRADUATIONYEAR, newGrp2Conditions[0].Field);
            Assert.Equal( "2004", newGrp2Conditions[0].Value);

            // Assert the second condition
            Assert.Equal( ComparisonOperators.EQ, newGrp2Conditions[1].Operators);
            Assert.Equal( StudentDTD.STUDENTPERSONAL_ONTIMEGRADUATIONYEAR, newGrp2Conditions[1].Field);
            Assert.Equal( "2005", newGrp2Conditions[1].Value);

            // Assert the third condition
            Assert.Equal( ComparisonOperators.EQ, newGrp2Conditions[2].Operators);
            Assert.Equal( StudentDTD.STUDENTPERSONAL_ONTIMEGRADUATIONYEAR, newGrp2Conditions[2].Field);
            Assert.Equal( "2006", newGrp2Conditions[2].Value);
        }



        [Fact]
        public void TestToXml010()
        {
            Query q = new Query(StudentDTD.SECTIONINFO);
            q.AddFieldRestriction(StudentDTD.SECTIONINFO_REFID);
            q.AddFieldRestriction( StudentDTD.SECTIONINFO_SCHOOLCOURSEINFOREFID );
            q.AddFieldRestriction( StudentDTD.SECTIONINFO_SCHOOLYEAR );
            q.AddFieldRestriction( StudentDTD.SECTIONINFO_LOCALID );
            q.AddFieldRestriction( "ScheduleInfoList/ScheduleInfo/@TermInfoRefId" );
            q.AddFieldRestriction( StudentDTD.SECTIONINFO_DESCRIPTION );
            q.AddFieldRestriction( StudentDTD.SECTIONINFO_LANGUAGEOFINSTRUCTION);
            q.AddFieldRestriction( StudentDTD.LANGUAGEOFINSTRUCTION_CODE );

            assertSectionInfoQueryXML( q );

        }

        [Fact]
        public void TestToXml020()
        {
            Query q = new Query(StudentDTD.SECTIONINFO);
            q.AddFieldRestriction( "@RefId");
            q.AddFieldRestriction( "@SchoolCourseInfoRefId" );
            q.AddFieldRestriction( "@SchoolYear" );
            q.AddFieldRestriction( "LocalId" );
            q.AddFieldRestriction("ScheduleInfoList/ScheduleInfo/@TermInfoRefId");
            q.AddFieldRestriction( "Description" );
            q.AddFieldRestriction( "LanguageOfInstruction" );
            q.AddFieldRestriction("LanguageOfInstruction");
            q.AddFieldRestriction("LanguageOfInstruction/Code");

            assertSectionInfoQueryXML(q);

        }

        [Fact]
        public void TestToXml030()
        {
            string queryStr =    @"<SIF_Query>
                                         <SIF_QueryObject ObjectName='SectionInfo'>
                                            <SIF_Element>@RefId</SIF_Element>
                                            <SIF_Element>@SchoolCourseInfoRefId</SIF_Element>
                                            <SIF_Element>@SchoolYear</SIF_Element>
                                            <SIF_Element>LocalId</SIF_Element>
                                            <SIF_Element>ScheduleInfoList/ScheduleInfo/@TermInfoRefId</SIF_Element>
                                            <SIF_Element>Description</SIF_Element>
                                            <SIF_Element>LanguageOfInstruction</SIF_Element>
                                            <SIF_Element>LanguageOfInstruction/Code</SIF_Element>
                                         </SIF_QueryObject>
                                      </SIF_Query>";

            SifParser parser = new SifParser(Runtime);
            SIF_Query sifQuery = (SIF_Query)parser.Parse( queryStr );
            Query q = new Query( sifQuery );
            assertSectionInfoQueryXML( q );


        }

        private void assertSectionInfoQueryXML( Query q )
        {
            String sif15Xml = q.ToXml( SifVersion.SIF15r1 );
            Console.WriteLine("SIF1.5 SectionInfo Query \r\n {0}", sif15Xml);
            String sif20Xml = q.ToXml( SifVersion.SIF21 );
            Console.WriteLine( "SIF2.0 SectionInfo Query \r\n {0}", sif20Xml );
            
            Assert.True( sif15Xml.IndexOf( "<SIF_QueryObject ObjectName=\"SectionInfo\">" ) > 0 );
            Assert.True(sif20Xml.IndexOf("<SIF_QueryObject ObjectName=\"SectionInfo\">") > 0);

            Assert.True(sif15Xml.IndexOf("<SIF_Element>@RefId</SIF_Element>") > 0);
            Assert.True(sif20Xml.IndexOf("<SIF_Element>@RefId</SIF_Element>") > 0);

            Assert.True(sif15Xml.IndexOf("<SIF_Element>@SchoolCourseInfoRefId</SIF_Element>") > 0);
            Assert.True(sif20Xml.IndexOf("<SIF_Element>@SchoolCourseInfoRefId</SIF_Element>") > 0);

            Assert.True(sif15Xml.IndexOf("<SIF_Element>@SchoolYear</SIF_Element>") == -1 );
            Assert.True(sif20Xml.IndexOf("<SIF_Element>@SchoolYear</SIF_Element>") > 0);

            Assert.True(sif15Xml.IndexOf("<SIF_Element>ScheduleInfo/@TermInfoRefId</SIF_Element>") > 0);
            Assert.True(sif20Xml.IndexOf("<SIF_Element>ScheduleInfoList/ScheduleInfo/@TermInfoRefId</SIF_Element>") > 0);

            Assert.True(sif15Xml.IndexOf("<SIF_Element>Description</SIF_Element>") == -1 );
            Assert.True(sif20Xml.IndexOf("<SIF_Element>Description</SIF_Element>") > 0);

            Assert.True(sif15Xml.IndexOf("<SIF_Element>LanguageOfInstruction</SIF_Element>") > 0);
            Assert.True(sif20Xml.IndexOf("<SIF_Element>LanguageOfInstruction</SIF_Element>") > 0);

            Assert.True(sif15Xml.IndexOf("<SIF_Element>LanguageOfInstruction/Code</SIF_Element>") == -1 );
            Assert.True(sif20Xml.IndexOf("<SIF_Element>LanguageOfInstruction/Code</SIF_Element>") > 0);


        }



        public static Query SaveToXMLAndReparse(Query query, SifVersion version, IAdkRuntime runtime)
        {
            String sifQueryXML = query.ToXml(version);
            Console.WriteLine(sifQueryXML);

            SifParser parser = new SifParser(runtime);
            SIF_Request sifR = (SIF_Request)parser.Parse("<SIF_Request>" + sifQueryXML + "</SIF_Request>", null);

            Query newQuery = new Query(sifR.SIF_Query);
            return newQuery;
        }


        [Fact]
        public void CustomSIFElementEncoding()
        {
            SIF_Query q = new SIF_Query();
            q.SIF_QueryObject = new SIF_QueryObject( StudentDTD.STUDENTPERSONAL.Name );
            SIF_Conditions conditions = new SIF_Conditions( ConditionType.NONE );
            conditions.AddSIF_Condition( "Name[@Type=\"05\"]/LastName", Operators.EQ, "Cookie" );
            q.SetSIF_ConditionGroup( ConditionType.NONE, conditions );

            string xml;
            using ( StringWriter w = new StringWriter() )
            {
                SifWriter writer = new SifWriter( w, Runtime );
                writer.Write( q );
                writer.Flush();
                writer.Close();
                xml = w.ToString();
            }

            Console.WriteLine( xml );
            // Mainly, just check to make sure that the single quotes didn't get encoded
            int index = xml.IndexOf( "&quot;" );
            Assert.Equal( -1, index);
        }


        private const String REFID_GUID = "1405050D1115164D461C7458ECB9FAAA";
        private const String SIFREFID_GUID = "895DDD1EB05D428084732CB635066585";

        private Authentication BuildAuthentication()
        {
            AuthSystem sys = (new AuthSystem( AuthSystemType.NETWORK, "Unit Tests" ));
            AuthenticationInfo inf = new AuthenticationInfo( sys );
            inf.Username = "braddock";
            Authentication auth = new Authentication( REFID_GUID, SIFREFID_GUID, AuthSifRefIdType.STUDENTPERSONAL );
            auth.AuthenticationInfo = inf;
            return auth;
        }

        [Fact]
        public void TestQueryCompare()
        {
            Query query = new Query(StudentDTD.STUDENTSCHOOLENROLLMENT, GroupOperator.Or);
            query.AddCondition(StudentDTD.STUDENTSCHOOLENROLLMENT_TIMEFRAME, ComparisonOperators.EQ, TimeFrame.CURRENT.Value);
            query.AddCondition(StudentDTD.STUDENTSCHOOLENROLLMENT_TIMEFRAME, ComparisonOperators.EQ, TimeFrame.FUTURE.Value);

            StudentSchoolEnrollment studentSchoolEnrollment = new StudentSchoolEnrollment();
            studentSchoolEnrollment.TimeFrame = TimeFrame.HISTORICAL.Value;
            Assert.False(query.Evaluate(studentSchoolEnrollment));
        }

        [Fact]
        public void testSimpleFilter()
        {
            Authentication auth = BuildAuthentication();
            Query q = new Query( StudentDTD.STUDENTPERSONAL );

            Assert.False( q.Evaluate( auth ) );

            q = new Query( InfrastructureDTD.AUTHENTICATION );
            Assert.True( q.Evaluate( auth ) );
        }

        [Fact]
        public void testSimpleRefidFilter()
        {
            Authentication auth = BuildAuthentication();
            Query q = new Query( InfrastructureDTD.AUTHENTICATION );
            q.AddCondition( InfrastructureDTD.AUTHENTICATION_REFID, ComparisonOperators.EQ, REFID_GUID );
            Assert.True( q.Evaluate( auth ) );
        }

        [Fact]
        public void testSimpleGTFilter()
        {
            StudentPersonal sp = new StudentPersonal( Runtime.MakeGuid(), new Name( NameType.BIRTH, "E", "Sally" ) );

            Query q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.GT, "D" );
            Assert.True( q.Evaluate( sp ) );

            q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.GT, "E" );
            Assert.False( q.Evaluate( sp ) );
        }

        [Fact]
        public void testConditionWithNullValue()
        {
            StudentPersonal sp = new StudentPersonal( Runtime.MakeGuid(), new Name( NameType.BIRTH, "E", "Sally" ) );

            Query q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.GT, null );
            Assert.False( q.Evaluate( sp ) );
        }

        [Fact]
        public void testElementWithNullValue()
        {
            StudentPersonal sp = new StudentPersonal( Runtime.MakeGuid(), new Name( NameType.BIRTH, null, "Sally" ) );

            Query q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.GT, "E" );
            Assert.False( q.Evaluate( sp ) );

            q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.EQ, "E" );
            Assert.False( q.Evaluate( sp ) );

            q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.NE, "E" );
            Assert.True( q.Evaluate( sp ) );
        }

        [Fact]
        public void testSimpleLTFilter()
        {
            StudentPersonal sp = new StudentPersonal( Runtime.MakeGuid(), new Name( NameType.BIRTH, "E", "Sally" ) );

            Query q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.LT, "G" );
            Assert.True( q.Evaluate( sp ) );

            q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( CommonDTD.NAME_LASTNAME, ComparisonOperators.LT, "E" );
            Assert.False( q.Evaluate( sp ) );
        }

        [Fact]
        public void testSimpleRefidFilterFail()
        {
            Authentication auth = BuildAuthentication();
            Query q = new Query( InfrastructureDTD.AUTHENTICATION );
            q.AddCondition( InfrastructureDTD.AUTHENTICATION_REFID, ComparisonOperators.EQ, "FAIL" );
            Assert.False( q.Evaluate( auth ) );
        }

        [Fact]
        public void testSimpleOrFilter()
        {
            Authentication auth = BuildAuthentication();
            Query q = new Query( InfrastructureDTD.AUTHENTICATION, GroupOperator.Or );
            q.AddCondition( InfrastructureDTD.AUTHENTICATIONINFO_DISTINGUISHEDNAME, ComparisonOperators.EQ, "foo" );
            q.AddCondition( InfrastructureDTD.AUTHENTICATION_REFID, ComparisonOperators.EQ, REFID_GUID );
            Assert.True( q.Evaluate( auth ) );
        }

        [Fact]
        public void testComplexOrWithGTFilter()
        {
            ConditionGroup fail = new ConditionGroup( GroupOperator.None );
            // This condition should fail
            fail.AddCondition( InfrastructureDTD.AUTHENTICATION_REFID, ComparisonOperators.NE, REFID_GUID );

            ConditionGroup pass = new ConditionGroup( GroupOperator.Or );
            // This condition should fail
            pass.AddCondition( InfrastructureDTD.AUTHENTICATIONINFO_USERNAME, ComparisonOperators.EQ, "FAIL" );
            pass.AddCondition( InfrastructureDTD.AUTHENTICATIONINFO_USERNAME, ComparisonOperators.GT, "a" );

            ConditionGroup root = new ConditionGroup( GroupOperator.Or );
            root.AddGroup( fail );
            root.AddGroup( pass );

            Query q = new Query( InfrastructureDTD.AUTHENTICATION, root );

            Authentication auth = BuildAuthentication();
            Assert.True( q.Evaluate( auth ) );
        }



        [Fact]
        public void testCreateWithSIF_Query()
        {
            SIF_Query q = new SIF_Query( new SIF_QueryObject(
                                             ReportingDTD.STUDENTLOCATOR.Name ) );
            SIF_ConditionGroup scg = new SIF_ConditionGroup();
            scg.SetType( ConditionType.NONE );

            SIF_Condition sifCondition = new SIF_Condition(
                "RequestingAgencyId[@Type=\"School\"]", Operators.EQ, "2001" );
            SIF_Conditions conds = new SIF_Conditions( ConditionType.NONE );
            conds.AddChild( sifCondition );
            scg.AddSIF_Conditions( conds );
            q.SIF_ConditionGroup = scg;

            Query query = new Query( q );

            Assert.True( query.HasConditions );
            ConditionGroup[] conditions = query.Conditions;
            Assert.Single( conditions);
            Assert.Equal( GroupOperator.None, conditions[0].Operator);
            Condition condition = conditions[0].Conditions[0];
            Assert.Equal( "2001", condition.Value);
            Assert.Equal( ComparisonOperators.EQ, condition.Operators);
            Assert.Equal( ReportingDTD.STUDENTLOCATOR_REQUESTINGAGENCYID, condition.Field);
        }

        [Fact]
        public void testComplexAndQuery()
        {
            String sifQuery = "<SIF_Request><SIF_Query>" +
                              "    <SIF_QueryObject ObjectName=\"StudentSchoolEnrollment\"/>" +
                              "    <SIF_ConditionGroup Type=\"None\">" +
                              "           <SIF_Conditions Type=\"And\">" +
                              "             <SIF_Condition>" +
                              "                 <SIF_Element>@MembershipType</SIF_Element>" +
                              "                 <SIF_Operator>EQ</SIF_Operator>" +
                              "                 <SIF_Value>Home</SIF_Value>" +
                              "              </SIF_Condition>" +
                              "              <SIF_Condition>" +
                              "                 <SIF_Element>@RefId</SIF_Element>" +
                              "                 <SIF_Operator>EQ</SIF_Operator>" +
                              "                 <SIF_Value>49B02D134D6D445DA7B5C76160BF3902</SIF_Value>" +
                              "              </SIF_Condition>" +
                              "              <SIF_Condition>" +
                              "                 <SIF_Element>@SchoolInfoRefId</SIF_Element>" +
                              "                 <SIF_Operator>EQ</SIF_Operator>" +
                              "                 <SIF_Value>EF8850D522E54688B036B08F9C4C1312</SIF_Value>" +
                              "              </SIF_Condition>" +
                              "              <SIF_Condition>" +
                              "                 <SIF_Element>@SchoolYear</SIF_Element>" +
                              "                 <SIF_Operator>EQ</SIF_Operator>" +
                              "                 <SIF_Value>2006</SIF_Value>" +
                              "              </SIF_Condition>" +
                              "              <SIF_Condition>" +
                              "                 <SIF_Element>@StudentPersonalRefId</SIF_Element>" +
                              "                 <SIF_Operator>EQ</SIF_Operator>" +
                              "                 <SIF_Value>1AA295D3BC5146FA9058BB62FB6CC602</SIF_Value>" +
                              "              </SIF_Condition>" +
                              "              <SIF_Condition>" +
                              "                 <SIF_Element>@TimeFrame</SIF_Element>" +
                              "                 <SIF_Operator>EQ</SIF_Operator>" +
                              "                 <SIF_Value>Historical</SIF_Value>" +
                              "              </SIF_Condition>" +
                              "              <SIF_Condition>" +
                              "                <SIF_Element>EntryDate</SIF_Element>" +
                              "                 <SIF_Operator>EQ</SIF_Operator>" +
                              "                 <SIF_Value>2005-08-28</SIF_Value>" +
                              "              </SIF_Condition>" +
                              "           </SIF_Conditions>" +
                              "        </SIF_ConditionGroup>" +
                              "     </SIF_Query></SIF_Request>";


            SifParser parser = new SifParser(Runtime);
            SIF_Request sifR = (SIF_Request) parser.Parse( sifQuery, null, 0, SifVersion.LATEST );
            Query query = new Query( sifR.SIF_Query );

            String sse =
                "<StudentSchoolEnrollment RefId=\"49B02D134D6D445DA7B5C76160BF3902\" StudentPersonalRefId=\"1AA295D3BC5146FA9058BB62FB6CC602\" SchoolInfoRefId=\"EF8850D522E54688B036B08F9C4C1312\" MembershipType=\"Home\" TimeFrame=\"Historical\" SchoolYear=\"2006\">" +
                "<EntryDate>2005-08-28</EntryDate>" +
                "</StudentSchoolEnrollment>";

            SifDataObject sdo = (SifDataObject) parser.Parse( sse, null, 0, SifVersion.SIF20r1 );

            Assert.True( query.Evaluate( sdo ) );
        }

        [Fact]
        public void testParseFieldRestrictions()
        {
            String filteredRequest = "<SIF_Request>" +
                                     " <SIF_Header>" +
                                     "	 <SIF_MsgId>D59CE61000D011DDB8D4E57E5F08151C</SIF_MsgId>" +
                                     "	 <SIF_Timestamp>2008-04-02T12:21:22.801-05:00</SIF_Timestamp>" +
                                     "	 <SIF_SourceId>318D14F000D011DDB8D4AE35C22E84E0</SIF_SourceId>" +
                                     "	 <SIF_DestinationId>SASIxp</SIF_DestinationId>" +
                                     "  </SIF_Header>" +
                                     "  <SIF_Version>2.*</SIF_Version>" +
                                     "  <SIF_MaxBufferSize>16384</SIF_MaxBufferSize>" +
                                     "  <SIF_Query>" +
                                     "	 <SIF_QueryObject ObjectName=\"StudentSnapshot\">" +
                                     "		<SIF_Element>@SchoolYear</SIF_Element>" +
                                     "		<SIF_Element>@SnapDate</SIF_Element>" +
                                     "		<SIF_Element>@StudentPersonalRefId</SIF_Element>" +
                                     "		<SIF_Element>HomeEnrollment/GradeLevel/Code</SIF_Element>" +
                                     "		<SIF_Element>HomeEnrollment/Status</SIF_Element>" +
                                     "		<SIF_Element>LocalId</SIF_Element>" +
                                     "		<SIF_Element>Address</SIF_Element>" +
                                     "	 </SIF_QueryObject>" +
                                     "  </SIF_Query>" +
                                     "</SIF_Request>";

            SifParser parser = new SifParser(Runtime);
            SIF_Request request = (SIF_Request) parser.Parse( filteredRequest, null, 0, SifVersion.LATEST );
            Query query = new Query( request.SIF_Query );

            // Assert things about the query
            Assert.Equal( StudentDTD.STUDENTSNAPSHOT, query.ObjectType );
            Assert.Equal( "StudentSnapshot", query.ObjectTag );
            IElementDef[] elements = query.FieldRestrictions;

            Assert.NotNull( elements );
            Assert.Equal( 7, elements.Length );

            Assert.Equal( StudentDTD.STUDENTSNAPSHOT_SCHOOLYEAR, elements[0] );
            Assert.Equal( StudentDTD.STUDENTSNAPSHOT_SNAPDATE, elements[1] );
            Assert.Equal( StudentDTD.STUDENTSNAPSHOT_STUDENTPERSONALREFID, elements[2] );
            Assert.Equal( CommonDTD.GRADELEVEL_CODE, elements[3] );
            Assert.Equal( StudentDTD.HOMEENROLLMENT_STATUS, elements[4] );
            Assert.Equal( StudentDTD.STUDENTSNAPSHOT_LOCALID, elements[5] );
            Assert.Equal( StudentDTD.STUDENTSNAPSHOT_ADDRESS, elements[6] );
        }



        [Fact]
        public void testParseFieldRestrictions020()
        {
            String filteredRequest =
                "<SIF_Message xmlns=\"http://www.sifinfo.org/infrastructure/2.x\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Version=\"2.0r1\">" +
                "<SIF_Request>" +
                "   <SIF_Header>" +
                "      <SIF_MsgId>EC357ED020F811DD9039FE9AB6D9D924</SIF_MsgId>" +
                "      <SIF_Timestamp>2008-05-13T10:28:57.789-05:00</SIF_Timestamp>" +
                "      <SIF_SourceId>DDD7112020E211DD9039DFD0562B2BED</SIF_SourceId>" +
                "      <SIF_DestinationId>Classroll Agent</SIF_DestinationId>" +
                "   </SIF_Header>" +
                "   <SIF_Version>2.*</SIF_Version>" +
                "   <SIF_MaxBufferSize>11984</SIF_MaxBufferSize>" +
                "   <SIF_Query>" +
                "      <SIF_QueryObject ObjectName=\"SectionInfo\">" +
                "         <SIF_Element>@RefId</SIF_Element>" +
                "         <SIF_Element>@SchoolCourseInfoRefId</SIF_Element>" +
                "         <SIF_Element>@SchoolYear</SIF_Element>" +
                "         <SIF_Element>LocalId</SIF_Element>" +
                "         <SIF_Element>ScheduleInfoList/ScheduleInfo/@TermInfoRefId</SIF_Element>" +
                "         <SIF_Element>Description</SIF_Element>" +
                "         <SIF_Element>LanguageOfInstruction</SIF_Element>" +
                "         <SIF_Element>LanguageOfInstruction/Code</SIF_Element>" +
                "      </SIF_QueryObject>" +
                "   </SIF_Query>" +
                "</SIF_Request>" +
                "</SIF_Message>";


            SifParser parser = new SifParser(Runtime);
            SIF_Request request = (SIF_Request) parser.Parse( filteredRequest, null, 0, SifVersion.LATEST );
            Query query = new Query( request.SIF_Query );

            // Assert things about the query
            Assert.Equal(StudentDTD.SECTIONINFO, query.ObjectType);
            Assert.Equal( "SectionInfo", query.ObjectTag );
            IElementDef[] elements = query.FieldRestrictions;

            Assert.NotNull( elements );
            Assert.Equal( 8, elements.Length );

            // Attempt reparsing and then re-asserting:
            query = SaveToXMLAndReparse( query, SifVersion.LATEST, Runtime );
            // Assert things about the query
            Assert.Equal(StudentDTD.SECTIONINFO, query.ObjectType);
            Assert.Equal("SectionInfo", query.ObjectTag);
            elements = query.FieldRestrictions;

            Assert.NotNull(elements);
            Assert.Equal(8, elements.Length);

        }


        [Fact]
        public void testSQP1x010()
        {
            // Test using all 1.x versions of SIF
            testSQP( StudentDTD.STUDENTPERSONAL, CommonDTD.NAME_FIRSTNAME,
                     "Name/FirstName", SifVersion.SIF11 );
            testSQP( StudentDTD.STUDENTPERSONAL, CommonDTD.NAME_FIRSTNAME,
                     "Name/FirstName", SifVersion.SIF15r1 );
        }

        [Fact]
        public void testSQP2x010()
        {
            // Test using all 2.x versions of SIF
            testSQP( StudentDTD.STUDENTPERSONAL, CommonDTD.NAME_FIRSTNAME,
                     "Name/FirstName", SifVersion.SIF11 );
            testSQP( StudentDTD.STUDENTPERSONAL, CommonDTD.NAME_FIRSTNAME,
                     "Name/FirstName", SifVersion.SIF15r1 );
        }

        [Fact]
        public void testSQP1x020()
        {
            // Test using all 1.x versions of SIF
            Query q = testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                                        "Demographics/Ethnicity", SifVersion.SIF11,
                                        CommonDTD.RACELIST_RACE );
            Condition cond = q.HasCondition( CommonDTD.RACELIST_RACE );
            Assert.NotNull( cond);
            Assert.True("Demographics/RaceList/Race" == cond .GetXPath( q, SifVersion.SIF20 ), "Translate XPath");

            q = testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                                  "Demographics/Ethnicity", SifVersion.SIF15r1,
                                  CommonDTD.RACELIST_RACE );
            cond = q.HasCondition( CommonDTD.RACELIST_RACE );
            Assert.NotNull( cond);
            Assert.True("Demographics/RaceList/Race" == cond .GetXPath( q, SifVersion.SIF20 ), "Translate XPath");
        }

        [Fact]
        public void testSQP2x020()
        {
            // Test using all 2.xversions of SIF
            Query q = testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                                        "Demographics/RaceList/Race", SifVersion.SIF20,
                                        CommonDTD.RACELIST_RACE );
            Condition cond = q.HasCondition( CommonDTD.RACELIST_RACE );
            Assert.NotNull( cond);
            Assert.True("Demographics/Ethnicity" == cond .GetXPath( q, SifVersion.SIF15r1 ), "Translate XPath");

            q = testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                                  "Demographics/RaceList/Race", SifVersion.SIF20r1,
                                  CommonDTD.RACELIST_RACE );
            cond = q.HasCondition( CommonDTD.RACELIST_RACE );
            Assert.NotNull( cond);
            Assert.True("Demographics/Ethnicity" == cond .GetXPath( q, SifVersion.SIF15r1 ), "Translate XPath");

            // testSQP( StudentDTD.STUDENTPERSONAL, "Demographics/RaceList/Race",
            // SifVersion.SIF21 );
        }

        [Fact]
        public void testSQP1x030()
        {
            // Test using all 1.x versions of SIF
            testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                              "SIF_ExtendedElements/SIF_ExtendedElement[@Name='eyecolor']",
                              SifVersion.SIF11, GlobalDTD.SIF_EXTENDEDELEMENT );
            testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                              "SIF_ExtendedElements/SIF_ExtendedElement[@Name='eyecolor']",
                              SifVersion.SIF15r1, GlobalDTD.SIF_EXTENDEDELEMENT );
        }

        [Fact]
        public void testSQP2x030()
        {
            // Test using all 2.xversions of SIF
            testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                              "SIF_ExtendedElements/SIF_ExtendedElement[@Name='eyecolor']",
                              SifVersion.SIF20, GlobalDTD.SIF_EXTENDEDELEMENT );
            testResolveBySQP( StudentDTD.STUDENTPERSONAL,
                              "SIF_ExtendedElements/SIF_ExtendedElement[@Name='eyecolor']",
                              SifVersion.SIF20r1, GlobalDTD.SIF_EXTENDEDELEMENT );
            // testSQP( StudentDTD.STUDENTPERSONAL, "Demographics/RaceList/Race",
            // SifVersion.SIF21 );
        }

        [Fact]
        public void testSQP040()
        {
            Query q = new Query( StudentDTD.STUDENTSCHOOLENROLLMENT );
            q.AddCondition( StudentDTD.STUDENTSCHOOLENROLLMENT_SCHOOLYEAR,
                            ComparisonOperators.EQ, "2001" );

            Condition c = q
                .HasCondition( StudentDTD.STUDENTSCHOOLENROLLMENT_SCHOOLYEAR );
            Assert.NotNull( c );
            String xPath = c.GetXPath( q, SifVersion.SIF15r1 );
            Assert.Equal( "SchoolYear", xPath);

            xPath = c.GetXPath( q, SifVersion.SIF20 );
            Assert.Equal( "@SchoolYear", xPath);
        }

        [Fact]
        public void testSQP050()
        {
            Query q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( "OtherIdList/OtherId[@Type='ZZ']",
                            ComparisonOperators.EQ, "SCHOOL:997" );

            Condition c = q.HasCondition( CommonDTD.OTHERID );
            Assert.NotNull( c );
            String xPath = c.GetXPath( q, SifVersion.SIF15r1 );
            Assert.Equal( "OtherId[@Type='ZZ']", xPath);

            xPath = c.GetXPath( q, SifVersion.SIF20 );
            Assert.Equal( "OtherIdList/OtherId[@Type='ZZ']", xPath);
        }

        [Fact]
        public void testSQP060()
        {
            // TT 217 Presumptive Query Syntax support
            Query q = new Query( ReportingDTD.STUDENTLOCATOR );
            q.AddCondition( "RequestingAgencyId/@Type", ComparisonOperators.EQ,
                            "LEA" );
            q.AddCondition( "RequestingAgencyId", ComparisonOperators.EQ, "0001" );

            q = SaveToXMLAndReparse( q, SifVersion.LATEST, Runtime );

            Condition c = q.HasCondition( ReportingDTD.REQUESTINGAGENCYID_TYPE );
            Assert.NotNull( c );
            String xPath = c.GetXPath( q, SifVersion.LATEST );
            Assert.Equal( "RequestingAgencyId/@Type", xPath);

            c = q.HasCondition( ReportingDTD.REQUESTINGAGENCYID );
            Assert.NotNull( c );
            xPath = c.GetXPath( q, SifVersion.LATEST );
            Assert.Equal( "RequestingAgencyId", xPath);
        }





        private Query testResolveBySQP( IElementDef objectDef, String sqp,
                                        SifVersion version, IElementDef resolvedNestedElement )
        {
            Runtime.SifVersion = version;

            Query q = Objects.CreateQuery( objectDef );
            q.AddCondition( sqp, ComparisonOperators.EQ, "foo" );
            String sifQueryXML = q.ToXml();
            Console.WriteLine( sifQueryXML );

            String searchFor = "<SIF_Element>" + sqp + "</SIF_Element>";
            // The .Net ADK doesn't encode apostrophes when they are in 
            // element content, so the following line is different than
            // the java test
            //searchFor = searchFor.Replace( "'", "&apos;" );
            Assert.Contains( searchFor, sifQueryXML );

            SifParser parser = new SifParser(Runtime);
            SIF_Request sifR = (SIF_Request) parser.Parse( "<SIF_Request>"
                                                           + sifQueryXML + "</SIF_Request>", null );

            Query newQuery = new Query( sifR.SIF_Query );

            Condition cond = newQuery.HasCondition( sqp );
            Assert.NotNull( cond);
            Assert.Equal( sqp, cond.GetXPath());
            Assert.True(sqp == cond.GetXPath( newQuery, version ), "Version-Specific SQP");

            return newQuery;
        }

        /**
	 * Test SIF Query Pattern support in the ADK
	 * 
	 * @param objectDef
	 *            The IElementDef representing the root SIF Object
	 * @param def
	 *            The IElementDef representing the field being queried (e.g.
	 *            CommonDTD.NAME_FIRSTNAME)
	 * @param sqp
	 *            The expected SIF Query Pattern (e.g. "Name/FirstName") for the
	 *            above field def
	 * @param version
	 *            The version of SIF to test
	 */

        private Query testSQP( IElementDef objectDef, IElementDef def, String sqp,
                               SifVersion version )
        {
            Runtime.SifVersion = version;
            IElementDef lookedUp = Runtime.Dtd.LookupElementDefBySQP( objectDef, sqp );
            Assert.Equal( def.Name, lookedUp.Name);
            testResolveBySQP( objectDef, sqp, version, def );

            Query q = Objects.CreateQuery( objectDef );
            q.AddCondition( def, ComparisonOperators.EQ, "foo" );

            String sifQueryXML = q.ToXml();
            Console.WriteLine( sifQueryXML );

            String searchFor = "<SIF_Element>" + sqp + "</SIF_Element>";
            Assert.Contains( searchFor, sifQueryXML );

            SifParser parser = new SifParser(Runtime);
            SIF_Request sifR = (SIF_Request) parser.Parse( "<SIF_Request>"
                                                           + sifQueryXML + "</SIF_Request>", null );

            Query newQuery = new Query( sifR.SIF_Query );

            Condition cond = newQuery.HasCondition( sqp );
            Assert.NotNull( cond);
            Assert.Equal( sqp, cond.GetXPath());
            Assert.Equal( def, cond.Field);

            return newQuery;
        }

        [Fact]
        public void testQuery010()
        {
            Query q = new Query( StudentDTD.STUDENTPERSONAL );
            q.AddCondition( "Demographics/Ethnicity", ComparisonOperators.EQ, "W" );
            Console.WriteLine( q.ToXml() );

            q = SaveToXMLAndReparse( q, SifVersion.SIF15r1, Runtime );

            Condition c = q.HasCondition( "Demographics/Ethnicity" );
            Assert.NotNull( c);
        }
    }
}

