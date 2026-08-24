using System;
using System.IO;
using System.Xml;
using OpenADK.Library;
using OpenADK.Library.Global;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using Xunit;
using Library.UnitTesting.Framework;
using OpenADK.Library.Infra;


namespace Library.xUnit.US
{
   /// <summary>
   /// Summary description for SifWriterTests.
   /// </summary>
   
   public class SifWriterTests : AdkTest
   {
      /// <summary>
      /// This method is not a true xUnit test, in that it doesn't do assertions.
      /// </summary>
      /// <remarks>
      /// However, it is useful when changing the behavior of the SIFWriter to see
      /// how changes affect the speed of writing.
      /// 
      /// All tests below run on the HP ZD7168CL laptop
      /// 
      /// .Net ADK Timings
      /// Ran in 7.093 - 7.39 seconds on 11/18/2004 ( XML Escaping is always on )
      /// Ran in 9.26  - 9.31 seconds on 12/20/2004 ( XML Escaping is always on )
      /// Ran in 8.75  - 9.0  seconds on 01/28/2005 ( Check XML Escaping on Elements, change to ElementDefImpl.FQClassName )
      /// Ran in 9.625        seconds on 02/28/2005 ( Check version before writing element )
      /// 
      /// Java ADK Timings:
      /// Ran in 14.953 - 15.062 seconds  on 11/18/2004 before XML escaping was added to SIFWriter
      /// Ran in 17.935 - 17.938 seconds  on 11/18/2004 after XML escaping was added to SIFWriter
      /// Ran in 15.053 - 15.127 seconds  on 11/18/2004 after XML escaping was added to SIFWriter with escaping turned off
      /// </remarks>
      [Fact(Skip = "Explicit")]
      public void WriteSpeedTest()
      {
         Runtime.Debug = AdkDebugFlags.None;

         StudentPersonal sp = ObjectCreator.CreateStudentPersonal();
         Address addr = sp.AddressLists[0][0];
         // Add in a few cases where escaping needs to be done
         addr.Street.Line1 = "ATTN: \"Miss Thompson\"";
         addr.Street.Line2 = "321 Dunn & Bradstreeet Lane";
         addr.Street.Line3 = "Weyer's Way, MO 32254";

         // Dump the object once to the console
         SifWriter writer = new SifWriter(Console.Out, Runtime);
         writer.Write(sp);
         writer.Flush();

         MemoryStream stream = new MemoryStream();
         writer = new SifWriter(stream, Runtime);

         for (int a = 0; a < 50000; a++)
         {
            writer.Write(sp);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
         }

         Console.WriteLine("Test Complete. Please See timings for details");
      }
      [Fact]
      public void FilterOutElementsFromDifferentVersion()
      {
         Runtime.SifVersion = SifVersion.SIF11;
         StudentPersonal sp = ObjectCreator.CreateStudentPersonal();
         sp.StateProvinceId = "55889";
         sp.LocalId = "987987987987987";

         StudentPersonal sp11 = (StudentPersonal)AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.SIF11);

          Assert.Null(sp11.LocalId);
          Assert.Null(sp11.StateProvinceId);

         StudentPersonal sp15 = (StudentPersonal)AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.SIF15r1);

         Assert.NotNull(sp15.LocalId);
         Assert.NotNull(sp15.StateProvinceId);
      }


      [Fact]
      public void TestEncodingHighAsciiChars()
      {
         StudentPersonal sp = new StudentPersonal();
         sp.RefId = Runtime.MakeGuid();
         sp.StateProvinceId = "\u06DE55889";
         sp.LocalId = "987987987987987";

         StudentPersonal copy = (StudentPersonal)AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST);

         Assert.Equal("\u06DE55889", copy.StateProvinceId);
      }



       [Fact]
       public void TestWriteXSINill()
       {
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.StateProvinceId = "\u06DE55889";
           sp.LocalId = "987987987987987";
           Name name = new Name(NameType.LEGAL, "Johnson", "Steve");
           sp.Name = name;
           name.SetField( CommonDTD.NAME_TYPE, new SifString( null ) );
           name.SetField(CommonDTD.NAME_MIDDLENAME, new SifString(null));

           SIF_ExtendedElement see = new SIF_ExtendedElement("FOO", null );
           see.SetField(GlobalDTD.SIF_EXTENDEDELEMENT, new SifString(null));
           see.XsiType = "Integer";
           sp.SIFExtendedElementsContainer.Add(see);

           sp.SetField( StudentDTD.STUDENTPERSONAL_LOCALID, new SifString( null ) );


           Console.WriteLine(sp.ToXml());
           StudentPersonal copy = (StudentPersonal)AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST, null, true);
           Console.WriteLine(copy.ToXml());

           name = copy.Name;
           Assert.Null(name.Type);
           Assert.Null(name.MiddleName);
           Assert.NotNull(name.FirstName);
           Assert.NotNull(name.LastName);

           // Attributes cannot be represented using xs nil
           SimpleField field = name.GetField(CommonDTD.NAME_TYPE);
           Assert.Null(field);


           field = name.GetField(CommonDTD.NAME_MIDDLENAME);
           Assert.NotNull(field);
           Assert.Null(field.Value);

           see = copy.GetSIFExtendedElement("FOO");
           field = see.GetField(GlobalDTD.SIF_EXTENDEDELEMENT);
           Assert.NotNull(field);
           Assert.Null(field.Value);

           field = copy.GetField(StudentDTD.STUDENTPERSONAL_LOCALID);
           Assert.NotNull(field);
           Assert.Null(field.Value);

           

       }

       [Fact]
       public void TestWriteXSINillMultiple()
       {
           SIF_Data data = new SIF_Data();

           for (int a = 0; a < 3; a++)
           {
               StudentPersonal sp = new StudentPersonal();
               sp.RefId = Runtime.MakeGuid();
               sp.StateProvinceId = "\u06DE55889";
               sp.LocalId = "987987987987987";
               Name name = new Name( NameType.LEGAL, "Johnson", "Steve" );
               sp.Name = name;
               name.SetField( CommonDTD.NAME_TYPE, new SifString( null ) );
               name.SetField( CommonDTD.NAME_MIDDLENAME, new SifString( null ) );

               SIF_ExtendedElement see = new SIF_ExtendedElement( "FOO", null );
               see.SetField( GlobalDTD.SIF_EXTENDEDELEMENT, new SifString( null ) );
               see.XsiType = "Integer";
               sp.SIFExtendedElementsContainer.Add( see );

               sp.SetField( StudentDTD.STUDENTPERSONAL_LOCALID, new SifString( null ) );
               data.AddChild( sp );
           }


           
           SIF_Data data2 = (SIF_Data)AdkObjectParseHelper.WriteParseAndReturn(data, SifVersion.LATEST, null, true);

           foreach (SifElement child in data2.GetChildList())
           {
               StudentPersonal copy = (StudentPersonal) child;
               Name name = copy.Name;
               Assert.Null( name.Type );
               Assert.Null( name.MiddleName );
               Assert.NotNull( name.FirstName );
               Assert.NotNull( name.LastName );

               // Attributes cannot be represented using xs nil
               SimpleField field = name.GetField( CommonDTD.NAME_TYPE );
               Assert.Null( field );


               field = name.GetField( CommonDTD.NAME_MIDDLENAME );
               Assert.NotNull( field );
               Assert.Null( field.Value );

               SIF_ExtendedElement see = copy.GetSIFExtendedElement( "FOO" );
               field = see.GetField( GlobalDTD.SIF_EXTENDEDELEMENT );
               Assert.NotNull( field );
               Assert.Null( field.Value );

               field = copy.GetField( StudentDTD.STUDENTPERSONAL_LOCALID );
               Assert.NotNull( field );
               Assert.Null( field.Value );
           }
       }


       [Fact]
       public void TestWriteXSIType()
       {
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.StateProvinceId = "\u06DE55889";
           sp.LocalId = "987987987987987";


           SIF_ExtendedElement see = new SIF_ExtendedElement( "FOO", "BAR" );
           see.XsiType = "Integer";
           sp.SIFExtendedElementsContainer.Add( see );

           Console.WriteLine( sp.ToXml() );

           StudentPersonal copy =
               (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn( sp, SifVersion.LATEST, null, true );

           see = copy.SIFExtendedElements[0];

           Assert.NotNull( see );
           Assert.Equal( "Integer", see.XsiType );


       }

       [Fact]
       public void TestSIFExtendedElementPlainText()
       {
           // SIF specification sample 1: plain text content
           // <SIF_ExtendedElement Name="ApplicationSubmissionStatus">4</SIF_ExtendedElement>
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.LocalId = "P00001";
           sp.Name = new Name(NameType.LEGAL, "Student", "Joe");

           SIF_ExtendedElement see = new SIF_ExtendedElement("ApplicationSubmissionStatus", "4");
           sp.SIFExtendedElementsContainer.Add(see);

           StudentPersonal copy = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST, null, true);

           see = copy.GetSIFExtendedElement("ApplicationSubmissionStatus");
           Assert.NotNull(see);
           Assert.NotNull(see.XmlFragment);
           Assert.Equal(1, see.XmlFragment.ChildNodes.Count);
           Assert.Equal(XmlNodeType.Text, see.XmlFragment.ChildNodes[0].NodeType);
           Assert.Equal("4", see.XmlFragment.ChildNodes[0].Value);
       }

       [Fact]
       public void TestSIFExtendedElementXmlContent()
       {
           // SIF specification sample 2: arbitrary XML element as content
           // <SIF_ExtendedElement Name="DynamicXml">
           //   <Parent xmlns="http://myapplication.com">
           //     <Child n="1">one</Child><Child n="2"/><Child n="3">three</Child>
           //   </Parent>
           // </SIF_ExtendedElement>
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.LocalId = "P00002";
           sp.Name = new Name(NameType.LEGAL, "Student", "Jane");

           SIF_ExtendedElement see = new SIF_ExtendedElement();
           see.Name = "DynamicXml";
           XmlDocument doc = new XmlDocument();
           doc.LoadXml("<Parent xmlns=\"http://myapplication.com\">" +
                       "<Child n=\"1\">one</Child><Child n=\"2\"/><Child n=\"3\">three</Child>" +
                       "</Parent>");
           XmlDocumentFragment frag = doc.CreateDocumentFragment();
           frag.AppendChild(doc.DocumentElement);
           see.XmlFragment = frag;
           sp.SIFExtendedElementsContainer.Add(see);

           StudentPersonal copy = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST, null, true);

           see = copy.GetSIFExtendedElement("DynamicXml");
           Assert.NotNull(see);
           Assert.NotNull(see.XmlFragment);
           Assert.Equal(1, see.XmlFragment.ChildNodes.Count);
           XmlElement root = (XmlElement) see.XmlFragment.ChildNodes[0];
           Assert.Equal("Parent", root.LocalName);
           Assert.Equal("http://myapplication.com", root.NamespaceURI);
           Assert.Equal(3, root.ChildNodes.Count);
           Assert.Equal("one", root.ChildNodes[0].InnerText);
       }

       [Fact]
       public void TestSIFExtendedElementMixedContent_XmlThenText()
       {
           // <SIF_ExtendedElement Name="Note">
           //   <xhtml:strong xmlns:xhtml="http://www.w3.org/1999/xhtml">Double</xhtml:strong>-check submission status.
           // </SIF_ExtendedElement>
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.LocalId = "P00003";
           sp.Name = new Name(NameType.LEGAL, "Student", "Bob");

           SIF_ExtendedElement see = new SIF_ExtendedElement();
           see.Name = "Note";
           XmlDocument doc = new XmlDocument();
           XmlDocumentFragment frag = doc.CreateDocumentFragment();
           frag.AppendChild(doc.CreateElement("xhtml", "strong", "http://www.w3.org/1999/xhtml")).InnerText = "Double";
           frag.AppendChild(doc.CreateTextNode("-check submission status."));
           see.XmlFragment = frag;
           sp.SIFExtendedElementsContainer.Add(see);

           StudentPersonal copy = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST, null, true);

           see = copy.GetSIFExtendedElement("Note");
           Assert.NotNull(see);
           Assert.NotNull(see.XmlFragment);
           Assert.Equal(2, see.XmlFragment.ChildNodes.Count);
           Assert.Equal(XmlNodeType.Element, see.XmlFragment.ChildNodes[0].NodeType);
           Assert.Equal("strong", see.XmlFragment.ChildNodes[0].LocalName);
           Assert.Equal("Double", see.XmlFragment.ChildNodes[0].InnerText);
           Assert.Equal(XmlNodeType.Text, see.XmlFragment.ChildNodes[1].NodeType);
           Assert.Equal("-check submission status.", see.XmlFragment.ChildNodes[1].Value);
       }

       [Fact]
       public void TestSIFExtendedElementMixedContent_TextThenXml()
       {
           // <SIF_ExtendedElement Name="Greeting">Hello <em>world</em></SIF_ExtendedElement>
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.LocalId = "P00004";
           sp.Name = new Name(NameType.LEGAL, "Student", "Ann");

           SIF_ExtendedElement see = new SIF_ExtendedElement();
           see.Name = "Greeting";
           XmlDocument doc = new XmlDocument();
           XmlDocumentFragment frag = doc.CreateDocumentFragment();
           frag.AppendChild(doc.CreateTextNode("Hello "));
           frag.AppendChild(doc.CreateElement("em")).InnerText = "world";
           see.XmlFragment = frag;
           sp.SIFExtendedElementsContainer.Add(see);

           StudentPersonal copy = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST, null, true);

           see = copy.GetSIFExtendedElement("Greeting");
           Assert.NotNull(see);
           Assert.NotNull(see.XmlFragment);
           Assert.Equal(2, see.XmlFragment.ChildNodes.Count);
           Assert.Equal(XmlNodeType.Text, see.XmlFragment.ChildNodes[0].NodeType);
           Assert.Equal("Hello ", see.XmlFragment.ChildNodes[0].Value);
           Assert.Equal(XmlNodeType.Element, see.XmlFragment.ChildNodes[1].NodeType);
           Assert.Equal("em", see.XmlFragment.ChildNodes[1].LocalName);
           Assert.Equal("world", see.XmlFragment.ChildNodes[1].InnerText);
       }

       [Fact]
       public void TestSIFExtendedElementMixedContent_TextBetweenXml()
       {
           // <SIF_ExtendedElement Name="Rich">Start <b>bold</b> end</SIF_ExtendedElement>
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.LocalId = "P00005";
           sp.Name = new Name(NameType.LEGAL, "Student", "Carl");

           SIF_ExtendedElement see = new SIF_ExtendedElement();
           see.Name = "Rich";
           XmlDocument doc = new XmlDocument();
           XmlDocumentFragment frag = doc.CreateDocumentFragment();
           frag.AppendChild(doc.CreateTextNode("Start "));
           frag.AppendChild(doc.CreateElement("b")).InnerText = "bold";
           frag.AppendChild(doc.CreateTextNode(" end"));
           see.XmlFragment = frag;
           sp.SIFExtendedElementsContainer.Add(see);

           StudentPersonal copy = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST, null, true);

           see = copy.GetSIFExtendedElement("Rich");
           Assert.NotNull(see);
           Assert.NotNull(see.XmlFragment);
           Assert.Equal(3, see.XmlFragment.ChildNodes.Count);
           Assert.Equal(XmlNodeType.Text, see.XmlFragment.ChildNodes[0].NodeType);
           Assert.Equal("Start ", see.XmlFragment.ChildNodes[0].Value);
           Assert.Equal(XmlNodeType.Element, see.XmlFragment.ChildNodes[1].NodeType);
           Assert.Equal("b", see.XmlFragment.ChildNodes[1].LocalName);
           Assert.Equal("bold", see.XmlFragment.ChildNodes[1].InnerText);
           Assert.Equal(XmlNodeType.Text, see.XmlFragment.ChildNodes[2].NodeType);
           Assert.Equal(" end", see.XmlFragment.ChildNodes[2].Value);
       }

       [Fact]
       public void TestSIFExtendedElementMixedContent_MultipleElements()
       {
           // <SIF_ExtendedElement Name="Multi"><a/><b/></SIF_ExtendedElement>
           StudentPersonal sp = new StudentPersonal();
           sp.RefId = Runtime.MakeGuid();
           sp.LocalId = "P00006";
           sp.Name = new Name(NameType.LEGAL, "Student", "Dana");

           SIF_ExtendedElement see = new SIF_ExtendedElement();
           see.Name = "Multi";
           XmlDocument doc = new XmlDocument();
           XmlDocumentFragment frag = doc.CreateDocumentFragment();
           frag.AppendChild(doc.CreateElement("a"));
           frag.AppendChild(doc.CreateElement("b"));
           see.XmlFragment = frag;
           sp.SIFExtendedElementsContainer.Add(see);

           StudentPersonal copy = (StudentPersonal) AdkObjectParseHelper.WriteParseAndReturn(sp, SifVersion.LATEST, null, true);

           see = copy.GetSIFExtendedElement("Multi");
           Assert.NotNull(see);
           Assert.NotNull(see.XmlFragment);
           Assert.Equal(2, see.XmlFragment.ChildNodes.Count);
           Assert.Equal(XmlNodeType.Element, see.XmlFragment.ChildNodes[0].NodeType);
           Assert.Equal("a", see.XmlFragment.ChildNodes[0].LocalName);
           Assert.Equal(XmlNodeType.Element, see.XmlFragment.ChildNodes[1].NodeType);
           Assert.Equal("b", see.XmlFragment.ChildNodes[1].LocalName);
       }

   }
}


