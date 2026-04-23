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
    internal class ProportionSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public ProportionSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            String xmlValue = o.SifValue.ToString( formatter ) + "%";
            WriteSimpleElement( writer, "Proportion", xmlValue );
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            if ( !reader.LocalName.Equals( "Proportion" ) )
            {
                return false;
            }

            String value = ConsumeElementTextValue( reader, version );

            if ( value != null && value.Length > 0 )
            {
                // Strip off the trailing percentage
                if ( value.EndsWith( "%" ) )
                {
                    value = value.Substring( 0, value.Length - 1 );
                }
                SifSimpleType proportionValue = CommonDTD.RACE_PROPORTION.TypeConverter.Parse( formatter, value );
                parent.SetField( CommonDTD.RACE_PROPORTION, proportionValue );
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
            get { return "Proportion"; }
        }
    }
}
