//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;
using OpenADK.Library.Tools.XPath;
using OpenADK.Library.us.Programs;

namespace OpenADK.Library.Impl.Surrogates
{
    internal class ServiceLocationSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public ServiceLocationSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            ServiceSetting setting = (ServiceSetting) o;
            if ( setting != null )
            {
                writer.WriteStartElement( "ServiceLocation" );
                
                String codeType = setting.CodeType;
                if ( codeType == null )
                {
                    codeType = "NCES";
                }
                writer.WriteAttributeString( "CodeType", codeType );

                String code = setting.Code;
                if ( code != null )
                {
                    writer.WriteValue( code );
                }
                writer.WriteEndElement();
            }
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            if ( !reader.LocalName.Equals( "ServiceLocation" ) )
            {
                return false;
            }

            ServiceSetting setting = new ServiceSetting();

            String codeType = reader.GetAttribute( "CodeType" );
            if ( codeType != null )
            {
                setting.CodeType = codeType;
            }

            String codeValue = ConsumeElementTextValue( reader, version );
            if ( codeValue != null )
            {
                setting.Code = codeValue;
            }

            formatter.AddChild( parent, setting, version );

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

        public string Path
        {
            get { return "ServiceLocation"; }
        }
    }
}
