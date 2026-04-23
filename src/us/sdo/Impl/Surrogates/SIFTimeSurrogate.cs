//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;
using OpenADK.Library.Tools.XPath;

namespace OpenADK.Library.Impl.Surrogates
{
    internal class SIFTimeSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public SIFTimeSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            String elementName = fElementDef.Name;
            SifTime time = (SifTime) o.SifValue;
            if ( time.Value.HasValue )
            {
                WriteSIFTime( writer, formatter, elementName, time.Value.Value );
            }
        }

        public static void WriteSIFTime( XmlWriter writer,
                                         SifFormatter formatter,
                                         String elementName,
                                         DateTime time )
        {
            String xmlTime = formatter.ToTimeString( time );
            writer.WriteStartElement( elementName );
            writer.WriteAttributeString( "Zone", "UTC-06:00" );
            writer.WriteValue( xmlTime );
            writer.WriteEndElement();
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            String elementName = fElementDef.Name;
            if ( !reader.LocalName.Equals( elementName ) )
            {
                return false;
            }

            String value = ConsumeElementTextValue( reader, version );

            if ( value != null && value.Length > 0 )
            {
                DateTime? time = formatter.ToTime( value );
                SifTime sifTime = new SifTime( time );
                parent.SetField( sifTime.CreateField( parent, fElementDef ) );
            }
            return true;
        }

        public INodePointer CreateChild( INodePointer parentPointer, SifFormatter formatter, SifVersion version,
                                         SifXPathContext context )
        {
            return null;
        }

        public INodePointer CreateNodePointer( INodePointer parentPointer, Element element, SifVersion version )
        {
            return null;
        }

        /// <summary>
        /// Gets the element name or path to the element in this version of SIF
        /// </summary>
        public string Path
        {
            get { return "SIF_Time"; }
        }
    }
}
