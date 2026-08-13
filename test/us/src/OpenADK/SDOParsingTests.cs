using System;
using System.IO;
using System.Text;
using OpenADK.Library;
using OpenADK.Library.Infra;
using OpenADK.Library.us.Instr;
using OpenADK.Library.us.Student;
using Xunit;
using Library.Nunit.US;
using Library.UnitTesting.Framework;

namespace Library.Nunit.US
{
   /// <summary>
   /// Summary description for SDOParsingTests.
   /// </summary>
   
   public class SDOParsingTests : UsAdkTest
   {
       [Fact]
       public void ParseLibraryPatronStatus()
       {
           // This test attempts to parse LibraryPatronStatus, which had a problem with parsing
           // Child elements that were SIFTime
           //	  Parse the object from the file
           Console.WriteLine("Parsing from file...");
           SifParser p = new SifParser(Runtime);
           SIF_Response msg = null;
           using (Stream inStream = GetResourceStream("LibraryPatronStatus.xml"))
           {
               msg = (SIF_Response)p.Parse(inStream, null);
               inStream.Close();
           }

           Assert.NotNull(msg);
           AdkObjectParseHelper.runParsingTest((SifDataObject)msg.SIF_ObjectData.GetChildList()[0], SifVersion.SIF15r1);
       }


       [Fact]
       public void ParseSIF_LogEntry()
       {
           // This test attempts to parse SIF_LogEntry,
           Console.WriteLine("Parsing from file...");
           SifParser p = new SifParser(Runtime);
           SIF_LogEntry logMsg = null;
           using (Stream inStream = GetResourceStream("SIF_LogEntry.xml"))
           {
               logMsg = (SIF_LogEntry)p.Parse(inStream, null, SifParserFlags.None, SifVersion.SIF15r1 );
               inStream.Close();
           }

           Assert.NotNull(logMsg);
           AdkObjectParseHelper.runParsingTest(logMsg, SifVersion.LATEST);
       }

       [Fact]
       public void ParseSIF_LogEntry2()
       {
           // This test attempts to parse SIF_LogEntry,
           Console.WriteLine("Parsing from file...");
           SifParser p = new SifParser(Runtime);
           SIF_Ack logMsg = null;
           using (Stream inStream = GetResourceStream("SIF_LogEntry2.xml"))
           {
               logMsg = (SIF_Ack)p.Parse(inStream, null, SifParserFlags.ExpectInnerEnvelope, SifVersion.SIF20r1);
               inStream.Close();
           }

           Assert.NotNull(logMsg);
           //AdkObjectParseHelper.runParsingTest( logMsg );
       }

       [Fact]
       public void ParseSectionInfoWithOverride()
       {

            //  Parse the object from the file
           Console.WriteLine("Parsing from file...");
           SifParser p = new SifParser(Runtime);
           SIF_Event sifEvent = null;
           using (Stream inStream = GetResourceStream( "SectionInfo_SchoolCourseInfoOverride.xml" ) )
           {
               sifEvent = (SIF_Event)p.Parse(inStream, null, SifParserFlags.None, SifVersion.SIF15r1);
               inStream.Close();
           }

           Assert.NotNull(sifEvent);
           AdkObjectParseHelper.runParsingTest( (SifDataObject)sifEvent.SIF_ObjectData.SIF_EventObject.GetChildList()[0], SifVersion.SIF15r1);

       }

       private void ParseSingleSDOObjectFromFile( string fileName )
       {
           //  Parse the object from the file
           Console.WriteLine("Parsing from file...");
           SifParser p = new SifParser(Runtime);
           SifDataObject sifObject = null;
           using (Stream inStream = GetResourceStream( fileName ) )
           {
               sifObject = (SifDataObject)p.Parse(inStream, null, SifParserFlags.None, SifVersion.SIF15r1);
               inStream.Close();
           }

           Assert.NotNull(sifObject);
           AdkObjectParseHelper.runParsingTest( sifObject, SifVersion.SIF15r1);
       }

       [Fact]
       	public void ParseActivity() {
		    ParseSingleSDOObjectFromFile("Activity.xml");
	    }

       [Fact]
       public void ParseBusRouteInfo()
       {
           ParseSingleSDOObjectFromFile("BusRouteDetail.xml");       }

       [Fact]
       public void ParseSectionInfo()
       {
           //  Parse the object from the file
           Console.WriteLine("Parsing from file...");
           SifParser p = new SifParser(Runtime);
           SectionInfo sectionInfo = null;
           using (Stream inStream = GetResourceStream("SectionInfo.xml"))
           {
               sectionInfo = (SectionInfo)p.Parse(inStream, null, SifParserFlags.None, SifVersion.SIF15r1);
               inStream.Close();
           }
           Assert.NotNull(sectionInfo);

           ScheduleInfoList schedules = sectionInfo.ScheduleInfoList;
           Assert.True(2 == schedules.Count, "Should have two ScheduleInfo elements");

           ScheduleInfo[] scheds = schedules.ToArray();

           // Assert the first ScheduleInfo
           Assert.True(2 == scheds[0].TeacherList.Count, "Should have two teachers");
           Assert.True(5 == scheds[0].MeetingTimeList.Count, "Should have 5 meeeting times");

           // Assert the second ScheduleInfo
           Assert.True(1 == scheds[1].TeacherList.Count, "Should have one teacher");
           Assert.True(5 == scheds[1].MeetingTimeList.Count, "Should have 5 meeeting times");

           // Assert that the SchoolCourseInfoOverride parsed correctly
           SchoolCourseInfoOverride cio = sectionInfo.SchoolCourseInfoOverride;
           Assert.NotNull(cio.CourseCredits);

           // NOTE: This will currently fail every time, due to a bug in 
           // CompareGraphTo
           AdkObjectParseHelper.runParsingTest(sectionInfo, SifVersion.SIF15r1);
       }

       [Fact(Skip = "Explicit - run manually")]
       public void ParseLearningStandardDocument()
       {
           // TODO: This test currently fails, due to deficiencies in parsing
           // SIF 1.5 XML
           //  Parse the object from the file
           //Console.WriteLine("Parsing from file...");
           //SifParser p = new SifParser(Runtime);
           //LearningStandardDocument lsd = null;
           //using (Stream inStream = GetResourceStream("LearningStandardDocument.xml"))
           //{
           //   lsd = (LearningStandardDocument)p.Parse(inStream, null);
           //   inStream.Close();
           //}
           //Assert.NotNull(lsd);

           // Assert.True("en" == lsd.Language, "xml:lang"); // lsd parse block commented out above
       }
 


       [Fact]
       public void SimpleObjectParse()
       {
           SifElement element = null;
           using (Stream aStream = GetResourceStream("SchoolInfo.xml"))
           {
               TextReader aReader = new StreamReader(aStream, Encoding.UTF8);
               SifParser parser = new SifParser(Runtime);
               element = parser.Parse(aReader, null, SifParserFlags.None, SifVersion.SIF11);
               aReader.Close();
               aStream.Close();
           }

           Assert.NotNull(element);
       }

       [Fact]
       public void SIFMessageParse()
       {
           SifElement element = null;
           using (Stream aStream = GetResourceStream("StudentPersonalResponse_AddForDelete.xml"))
           {
               TextReader aReader = new StreamReader(aStream);
               SifParser parser = new SifParser(Runtime);
               element = parser.Parse(aReader, null, SifParserFlags.None, SifVersion.SIF11);
               aReader.Close();
               aStream.Close();
           }

           Assert.NotNull(element);
       }

       [Fact]
       public void ParsingSIFTime()
       {
           SifElement element = null;
           using (Stream aStream = GetResourceStream("LibraryPatronStatus.xml"))
           {
               SifParser parser = new SifParser(Runtime);
               element = parser.Parse(aStream, null);
               aStream.Close();
           }

           Assert.NotNull(element);
       }


       [Fact]
       public void ParsingSectionInfo()
       {
           SifElement element = null;
           using (Stream aStream = GetResourceStream("SectionInfo_SchoolCourseInfoOverride.xml"))
           {
               SifParser parser = new SifParser(Runtime);
               element = parser.Parse(aStream, null);
               aStream.Close();
           }

           Assert.NotNull(element);
       }



      [Fact]
      public void TestParseComplexTypes()
      {
         Runtime.Initialize();
         Runtime.SifVersion = SifVersion.SIF15r1;
         SifParser p = new SifParser(Runtime);
         Activity activity = new Activity();
         activity = (Activity)p.Parse(
                        "<Activity RefId='0E3915409C3611DABE9FE16E3CD135F2' xml:lang='en'><Title>Activity 0E3915409C3611DABE9FE16E3CD135F2</Title><ActivityTime><CreationDate>20041016</CreationDate></ActivityTime></Activity>"
                        , null);
         Assert.NotNull(activity);
         Assert.True(activity.Title == "Activity 0E3915409C3611DABE9FE16E3CD135F2", "activity.Title is incorrect");
         Assert.True(DateTime.Parse(activity.ActivityTime.CreationDate.ToString()).ToString("yyyyMMdd") == "20041016", "activity.ActivityTime.CreationDate is incorrect");
      }//end TestParseComplexTypes
     


      }
}