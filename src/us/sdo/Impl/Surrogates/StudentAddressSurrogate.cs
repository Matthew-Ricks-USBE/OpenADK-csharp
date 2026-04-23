//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;
using OpenADK.Library.Tools.XPath;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;

namespace OpenADK.Library.Impl.Surrogates
{
    internal class StudentAddressSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public StudentAddressSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            if ( !(o is SifElement) )
            {
                Adk.Log.Warn( "StudentAddressSurrogate got an unacceptable element of type " + o.GetType() + "/" +
                              o.ElementDef.Name + "(" + o + ") in RenderRaw" );
                return;
            }

            SifElement element = (SifElement) o;

            foreach ( SifElement address in element.GetChildList( CommonDTD.ADDRESS ) )
            {
                writer.WriteStartElement( "StudentAddress" );
                writer.WriteAttributeString( "PickupOrDropoff", "NA" );
                writer.WriteAttributeString( "DayOfWeek", "NA" );

                // Create a nested SIF writer to write the address element
                SifWriter addressWriter = new SifWriter(writer);
                addressWriter.SuppressNamespace( true );
                addressWriter.Write( address );

                writer.WriteEndElement();
            }
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            if ( !reader.LocalName.Equals( "StudentAddress" ) )
            {
                return false;
            }

            try
            {
                StudentPersonal studentPersonal = (StudentPersonal) parent;
                StudentAddressList addressList = (StudentAddressList) studentPersonal.GetChild( StudentDTD.STUDENTPERSONAL_ADDRESSLIST );
                if ( addressList == null )
                {
                    addressList = new StudentAddressList();
                    studentPersonal.AddressList = addressList;
                }

                StudentAddressPullParser parser = new StudentAddressPullParser();
                StudentAddress studentAddress = (StudentAddress) parser.ParseOneElementFromStream( reader, version );
                Address address = studentAddress.Address;
                if ( address != null )
                {
                    studentAddress.RemoveChild( address );
                    addressList.Add( address );
                }
                if ( reader.NodeType != XmlNodeType.EndElement )
                {
                    reader.Read();
                }
            }
            catch ( Exception e )
            {
                throw new AdkParsingException( "Could not read StudentAddress: " + e.Message, null, e );
            }
            
            return true;
        }

        public INodePointer CreateChild( INodePointer parentPointer, SifFormatter formatter, SifVersion version,
                                         SifXPathContext context )
        {
            StudentPersonal studentPersonal = (StudentPersonal) ((AbstractNodePointer)parentPointer).GetBaseValue();
            StudentAddressList addressList = studentPersonal.AddressList;
            if ( addressList == null )
            {
                addressList = new StudentAddressList();
                studentPersonal.AddressList = addressList;
            }
            return new SifElementPointer( parentPointer, addressList, version );
        }

        public INodePointer CreateNodePointer( INodePointer parentPointer, Element sourceElement, SifVersion version )
        {
            return new SifElementPointer( parentPointer, (SifElement) sourceElement, version );
        }

        public string Path
        {
            get { return "StudentAddress"; }
        }

        public override String ToString()
        {
            return "StudentAddressSurrogate{}";
        }
    }
}
