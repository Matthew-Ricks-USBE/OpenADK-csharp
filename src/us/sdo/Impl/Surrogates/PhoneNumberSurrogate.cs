//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;
using OpenADK.Library.Tools.XPath;
using OpenADK.Library.us.Common;

namespace OpenADK.Library.Impl.Surrogates
{
    internal class PhoneNumberSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public PhoneNumberSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            PhoneNumber pn = (PhoneNumber) o;
            if ( pn != null )
            {
                writer.WriteStartElement( "PhoneNumber" );
                writer.WriteAttributeString( "Format", "NA" );
                
                String type = pn.Type;
                if ( type != null )
                {
                    writer.WriteAttributeString( "Type", type );
                }
                
                String number = pn.Number;
                if ( number != null )
                {
                    writer.WriteValue( number );
                }
                writer.WriteEndElement();
            }
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            if ( !reader.LocalName.Equals( "PhoneNumber" ) )
            {
                return false;
            }

            PhoneNumber phone = new PhoneNumber();
            
            String type = reader.GetAttribute( "Type" );
            if ( type != null )
            {
                phone.Type = type;
            }

            String format = reader.GetAttribute( "Format" );
            if ( format != null )
            {
                phone.Format = format;
            }
            
            String number = ConsumeElementTextValue( reader, version );
            if ( number != null )
            {
                phone.Number = number;
                phone.TextValue = number;
            }

            formatter.AddChild( parent, phone, version );

            return true;
        }

        public IElementDef LookupBySQP( String sqp )
        {
            if ( sqp.Length == 0 )
            {
                // This query pattern points to the PhoneNumber element itself. The
                // resolved ElementDef should be the PhoneNumber/Number element
                return CommonDTD.PHONENUMBER_NUMBER;
            }
            if ( sqp.Equals( "@Type" ) )
            {
                return CommonDTD.PHONENUMBER_TYPE;
            }
            return null;
        }

        public INodePointer CreateChild( INodePointer parentPointer, SifFormatter formatter, SifVersion version,
                                         SifXPathContext context )
        {
            PhoneNumber phone = new PhoneNumber();
            phone.SetField( CommonDTD.PHONENUMBER_NUMBER, new SifString( null ) );
            SifElement owner = (SifElement) ((AbstractNodePointer)parentPointer).GetBaseValue();
            formatter.AddChild( owner, phone, version );
            return new PhoneNumberPointer( parentPointer, phone, version );
        }

        public INodePointer CreateNodePointer( INodePointer parentPointer, Element element, SifVersion version )
        {
            if ( !(element is PhoneNumber) )
            {
                throw new ArgumentException( "Cannot create NodePointer for Elements other than PhoneNumber" );
            }
            return new PhoneNumberPointer( parentPointer, (PhoneNumber) element, version );
        }

        public string Path
        {
            get { return "PhoneNumber"; }
        }

        private class PhoneNumberPointer : SifElementPointer
        {
            public PhoneNumberPointer( INodePointer parentPointer, SifElement element, SifVersion version )
                : base( parentPointer, element, version )
            {
            }

            public override void SetValue( Object value )
            {
                PhoneNumber phone = (PhoneNumber) GetBaseValue();
                phone.TextValue = phone.Number = value.ToString();
            }

            public override object Value
            {
                get
                {
                    PhoneNumber phone = (PhoneNumber) GetBaseValue();
                    return new SimpleMultiField(phone, new[] { CommonDTD.PHONENUMBER_NUMBER, CommonDTD.PHONENUMBER });
                }
            }
        }
    }
}
