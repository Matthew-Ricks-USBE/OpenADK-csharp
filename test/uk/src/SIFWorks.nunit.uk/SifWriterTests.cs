using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenADK.Library.uk.Common;
using OpenADK.Library.Infra;
using OpenADK.Library.uk.Learner;
using OpenADK.Library.uk.School;
using Xunit;

namespace OpenADK.Library.Nunit.UK
{
    
    public class SifWriterTests : UkAdkTest
    {
        [Fact]
        public void TestxsiNill_SIFMessagePayload()
        {
            LearnerPersonal lp = new LearnerPersonal();

            // Add a null UPN
            SifString str= new SifString( null );
            lp.SetField( LearnerDTD.LEARNERPERSONAL_UPN, str );

            // Add a null AlertMsg
            AlertMsg msg = new AlertMsg( AlertMsgType.DISCIPLINE, null );
            lp.AlertMsgList = new AlertMsgList( msg );
            msg.SetField( CommonDTD.ALERTMSG, new SifString( null ) );



            SIF_Response sifMessage = new SIF_Response();
            sifMessage.AddChild( lp );


            //  Write the object to a file
            Console.WriteLine("Writing to file...");
            using (Stream fos = File.Open("SifWriterTest.Temp.xml", FileMode.Create, FileAccess.Write))
            {
                SifWriter writer = new SifWriter(fos, Runtime);
                sifMessage.SetChanged(true);
                writer.Write( sifMessage );
                writer.Flush();
                fos.Close();
            }

            //  Parse the object from the file
            Console.WriteLine("Parsing from file...");
            SifParser p = new SifParser(Runtime);
            using (Stream fis = File.OpenRead("SifWriterTest.Temp.xml"))
            {
                sifMessage = (SIF_Response)p.Parse(fis, null);
            }



            lp = (LearnerPersonal) sifMessage.GetChildList()[0];


            SimpleField upn = lp.GetField( LearnerDTD.LEARNERPERSONAL_UPN );
            Assert.NotNull( upn );

            SifString rawValue = (SifString)upn.SifValue;
            Assert.NotNull( rawValue );
            Assert.Null( rawValue.Value );
            Assert.Null( upn.Value );

            AlertMsgList alertMsgs = lp.AlertMsgList;
            Assert.NotNull( alertMsgs );
            Assert.True( alertMsgs.Count == 1 );
            msg = (AlertMsg)alertMsgs.GetChildList()[0];

            Assert.Null( msg.Value );
            SifSimpleType msgValue = msg.SifValue;
            Assert.NotNull( msgValue );
            Assert.Null( msgValue.RawValue );
        }

        [Fact]
        public void TestxsiNill_SDOObjectXML()
        {
            LearnerPersonal lp = new LearnerPersonal();

            // Add a null UPN
            SifString str = new SifString(null);
            lp.SetField(LearnerDTD.LEARNERPERSONAL_UPN, str);

            // Add a null AlertMsg
            AlertMsg msg = new AlertMsg(AlertMsgType.DISCIPLINE, null);
            lp.AlertMsgList = new AlertMsgList(msg);
            msg.SetField(CommonDTD.ALERTMSG, new SifString(null));



            //  Write the object to a file
            Console.WriteLine("Writing to file...");
            using (Stream fos = File.Open("SifWriterTest.Temp.xml", FileMode.Create, FileAccess.Write))
            {
                SifWriter writer = new SifWriter(fos, Runtime);
                lp.SetChanged(true);
                writer.Write(lp);
                writer.Flush();
                fos.Close();
            }

            //  Parse the object from the file
            Console.WriteLine("Parsing from file...");
            SifParser p = new SifParser(Runtime);
            using (Stream fis = File.OpenRead("SifWriterTest.Temp.xml"))
            {
                lp = (LearnerPersonal)p.Parse(fis, null);
            }


            SimpleField upn = lp.GetField(LearnerDTD.LEARNERPERSONAL_UPN);
            Assert.NotNull(upn);

            SifString rawValue = (SifString)upn.SifValue;
            Assert.NotNull(rawValue);
            Assert.Null(rawValue.Value);
            Assert.Null(upn.Value);

            AlertMsgList alertMsgs = lp.AlertMsgList;
            Assert.NotNull(alertMsgs);
            Assert.True(alertMsgs.Count == 1);
            msg = (AlertMsg)alertMsgs.GetChildList()[0];

            Assert.Null(msg.Value);
            SifSimpleType msgValue = msg.SifValue;
            Assert.NotNull(msgValue);
            Assert.Null(msgValue.RawValue);
        }

        [Fact]
        public void TestXsiNill_AllChildrenNil()
        {
            SchoolInfo si = new SchoolInfo();
            AddressableObjectName paon = new AddressableObjectName( );
            paon.Description = "The little white school house";
            paon.StartNumber = "321";
            Address addr = new Address( AddressType.CURRENT, paon );
            GridLocation gl = new GridLocation();
            gl.SetField( CommonDTD.GRIDLOCATION_PROPERTYEASTING, new SifDecimal( null ) );
            gl.SetField( CommonDTD.GRIDLOCATION_PROPERTYNORTHING, new SifDecimal( null ) );
            addr.GridLocation = gl;

            si.AddressList = new AddressList( addr );


            //  Write the object to a file
            Console.WriteLine("Writing to file...");
            using (Stream fos = File.Open("SifWriterTest.Temp.xml", FileMode.Create, FileAccess.Write))
            {
                SifWriter writer = new SifWriter(fos, Runtime);
                si.SetChanged(true);
                writer.Write(si);
                writer.Flush();
                fos.Close();
            }

            //  Parse the object from the file
            Console.WriteLine("Parsing from file...");
            SifParser p = new SifParser(Runtime);
            using (Stream fis = File.OpenRead("SifWriterTest.Temp.xml"))
            {
                si = (SchoolInfo)p.Parse(fis, null);
            }


            AddressList al = si.AddressList;
            Assert.NotNull( al );

            addr = al.ItemAt( 0 ); 
            Assert.NotNull( addr );

            gl = addr.GridLocation;
            Assert.NotNull( gl );

            Assert.Null( gl.PropertyEasting );
            Assert.Null(gl.PropertyNorthing );

            SimpleField sf = gl.GetField( CommonDTD.GRIDLOCATION_PROPERTYEASTING );
            Assert.NotNull( sf );
            Assert.Null( sf.Value );

            sf = gl.GetField(CommonDTD.GRIDLOCATION_PROPERTYNORTHING );
            Assert.NotNull(sf);
            Assert.Null(sf.Value);

        }

        [Fact]
        public void TestXsiNill_AllChildrenNilMultiple()
        {

            SIF_Data data = new SIF_Data();

            for (int a = 0; a < 3; a++)
            {
                SchoolInfo si = new SchoolInfo();
                AddressableObjectName paon = new AddressableObjectName();
                paon.Description = "The little white school house";
                paon.StartNumber = "321";
                Address addr = new Address( AddressType.CURRENT, paon );
                GridLocation gl = new GridLocation();
                gl.SetField( CommonDTD.GRIDLOCATION_PROPERTYEASTING, new SifDecimal( null ) );
                gl.SetField( CommonDTD.GRIDLOCATION_PROPERTYNORTHING, new SifDecimal( null ) );
                addr.GridLocation = gl;
                si.AddressList = new AddressList( addr );

                data.AddChild( si );
            }



            //  Write the object to a file
            Console.WriteLine("Writing to file...");
            using (Stream fos = File.Open("SifWriterTest.Temp.xml", FileMode.Create, FileAccess.Write))
            {
                SifWriter writer = new SifWriter(fos, Runtime);
                data.SetChanged(true);
                writer.Write(data);
                writer.Flush();
                fos.Close();
            }

            //  Parse the object from the file
            Console.WriteLine("Parsing from file...");
            SifParser p = new SifParser(Runtime);
            using (Stream fis = File.OpenRead("SifWriterTest.Temp.xml"))
            {
                data = (SIF_Data)p.Parse(fis, null);
            }

            foreach ( SchoolInfo si in data.GetChildList() )
            {
                AddressList al = si.AddressList;
                Assert.NotNull(al);

                Address addr = al.ItemAt(0);
                Assert.NotNull(addr);

                GridLocation gl = addr.GridLocation;
                Assert.NotNull(gl);

                Assert.Null(gl.PropertyEasting);
                Assert.Null(gl.PropertyNorthing);

                SimpleField sf = gl.GetField(CommonDTD.GRIDLOCATION_PROPERTYEASTING);
                Assert.NotNull(sf);
                Assert.Null(sf.Value);

                sf = gl.GetField(CommonDTD.GRIDLOCATION_PROPERTYNORTHING);
                Assert.NotNull(sf);
                Assert.Null(sf.Value);
            }

            

        }


    }
}
