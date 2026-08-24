using System.IO;
using System.Text;
using OpenADK.Library;
using OpenADK.Library.us.Hrfin;
using OpenADK.Library.Infra;
using OpenADK.Library.us.Reporting;
using OpenADK.Library.us.Student;
using Xunit;
using Library.xUnit.US;

namespace Library.xUnit.US
{
    /// <summary>
    /// Summary description for SIFParserTests.
    /// </summary>
    
    public class SIFParserTests : UsAdkTest
    {
      
        #region Embedded Performance Test run the PerfTest target to run

#if PERFTEST

    /// <summary>
    /// Runs a perf test on the SIF Parser by parsing an in-memory stream 5000 times
    /// </summary>
    /// <remarks>
    /// The first time this test was run, the total time averaged 3.3 seconds
    /// After turning WhitespaceHandling to None in the SIFParser, the time went down to 3.2 seconds
    /// </remarks>
		[Fact]
		public void PerfTestParsing5000Times()
		{
			// Do one warmup parse . . .

			SifElement element = null;
			using( Stream aStream = this.GetType().Assembly.GetManifestResourceStream( AdkTest.RESOURCE_ROOT + "StudentPersonalResponse_AddForDelete.xml") )
			{
				SifParser parser = new SifParser();
				// Do one warmup parse . . .
				TextReader aReader = new StreamReader( aStream );
				element = parser.Parse( aReader, null, SifParserFlags.None, SifVersion.SIF11 );

				for( int a = 0; a< 5000; a++ )
				{
					aStream.Seek( 0, SeekOrigin.Begin );
					element = parser.Parse( aStream, null, SifParserFlags.None );
				}
				aStream.Close();
			}

			Assert.NotNull( element);
		}

#endif

        #endregion

        [Fact]
        public void EmbeddedSIFMessage()
        {
            SifElement element = null;
            using (Stream aStream = GetResourceStream("GetNextMessageResponse.xml"))
            {
                TextReader aReader = new StreamReader(aStream);
                SifParser parser = new SifParser(Runtime);
                element = parser.Parse(aReader, null, SifParserFlags.ExpectInnerEnvelope, SifVersion.SIF11);
                aReader.Close();
                aStream.Close();
            }

            Assert.NotNull(element);
            SIF_Ack ack = (SIF_Ack) element;
            SifElement messageElement = ack.SIF_Status.SIF_Data.GetChild("SIF_Message");
            SIF_Event aEvent = (SIF_Event) messageElement.GetChild("SIF_Event");
            SIF_EventObject eventObject = aEvent.SIF_ObjectData.SIF_EventObject;

            Assert.Equal("SchoolCourseInfo", eventObject.ObjectName);
            Assert.Equal("Change", eventObject.Action);
            Assert.True(eventObject.GetChildList()[0] is SchoolCourseInfo);
        }

        [Fact]
        public void UnexpectedEmbeddedSIFMessage()
        {
            // this test should throw an exception because we are not passing "ExpectInnerEnvelope" in the 
            // parser flags
            Assert.Throws<AdkParsingException>(() =>
            {
                using Stream aStream = GetResourceStream("GetNextMessageResponse.xml");
                using TextReader aReader = new StreamReader(aStream);
                SifParser parser = new SifParser(Runtime);
                SifElement element = parser.Parse(aReader, null, SifParserFlags.None, SifVersion.SIF11);
            });
        }

        [Fact]
        public void TestLooseXmlTypeParsing100()
        {
            string vendorInfo15r1 =
                            @"<VendorInfo RefId='F138FF5017DC11DBAC45A0329DB3F005'>
                               <VendorName>Kleinman &amp; Co.</VendorName>
                               <Send1099 Code='XXX' />
                             </VendorInfo>";

            SifParser parser = new SifParser(Runtime);
            VendorInfo vi = (VendorInfo) parser.Parse( vendorInfo15r1, null, SifParserFlags.None, SifVersion.SIF15r1 );

            Assert.NotNull( vi );
            Assert.False( vi.Send1099.HasValue );
        }

        [Fact]
        public void TestLooseXmlTypeParsing101()
        {
            string vendorInfo20r1 =
                            @"<VendorInfo RefId='F138FF5017DC11DBAC45A0329DB3F005'>
                               <VendorName>Kleinman &amp; Co.</VendorName>
                               <Send1099>xxx</Send1099>
                             </VendorInfo>";

            SifParser parser = new SifParser(Runtime);
            VendorInfo vi = (VendorInfo)parser.Parse(vendorInfo20r1, null, SifParserFlags.None, SifVersion.SIF20r1);

            Assert.NotNull(vi);
            Assert.False(vi.Send1099.HasValue);
        }


        [Fact]
        public void TestParseReportData()
        {
            SIF_ReportObject reportObject = null;
            using (Stream aStream = GetResourceStream("ReportData.xml"))
            {
                TextReader aReader = new StreamReader(aStream);
                SifParser parser = new SifParser(Runtime);
                reportObject = (SIF_ReportObject) parser.Parse(aReader, null, SifParserFlags.None, SifVersion.SIF20r1);
                aReader.Close();
                aStream.Close();
            }

            Assert.NotNull( reportObject );
            Assert.Equal( 1, reportObject.ChildCount );

            SifElement reportData = reportObject.GetChildList()[0];
            Assert.Equal(2060, reportData.ChildCount);


        }



    }
}