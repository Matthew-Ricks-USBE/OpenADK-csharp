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
    internal class ServiceSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public ServiceSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            Service service = (Service) o;
            if ( service != null )
            {
                writer.WriteStartElement( "Service" );
                
                String codeType = service.CodeType;
                if ( codeType == null )
                {
                    codeType = "NCES";
                }
                writer.WriteAttributeString( "CodeType", codeType );

                String type = service.Type;
                if ( type == null )
                {
                    type = "Other";
                }
                writer.WriteAttributeString( "Type", type );

                String code = service.TextValue;
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
            if ( !reader.LocalName.Equals( "Service" ) )
            {
                return false;
            }

            Service service = new Service();

            String codeType = reader.GetAttribute( "CodeType" );
            if ( codeType != null )
            {
                service.CodeType = codeType;
            }

            String type = reader.GetAttribute( "Type" );
            if ( type != null )
            {
                service.Type = type;
            }

            String codeValue = ConsumeElementTextValue( reader, version );
            if ( codeValue != null )
            {
                service.TextValue = service.Code = codeValue;
            }

            formatter.AddChild( parent, service, version );

            return true;
        }

        public INodePointer CreateChild( INodePointer parentPointer, SifFormatter formatter, SifVersion version,
                                         SifXPathContext context )
        {
            var service = new Service();
            SifElement owner = (SifElement)((AbstractNodePointer)parentPointer).GetBaseValue();
            formatter.AddChild(owner, service, version);
            return new SifElementPointer(parentPointer, service, version);
        }

        public INodePointer CreateNodePointer( INodePointer parentPointer, Element element, SifVersion version )
        {
            if (!(element is Service service))
            {
                throw new ArgumentException("Cannot create NodePointer for Elements other than Service");
            }
            return new ServicePointer(parentPointer, service, version);
        }

        public string Path
        {
            get { return "Service"; }
        }

        private class ServicePointer : SifElementPointer
        {
            public ServicePointer(INodePointer parentPointer, SifElement element, SifVersion version)
                : base(parentPointer, element, version)
            {
            }

            public override void SetValue(Object value)
            {
                Service service = (Service)GetBaseValue();
                service.TextValue = service.Code = value.ToString();
            }

            public override object Value
            {
                get
                {
                    Service service = (Service)GetBaseValue();
                    return new SimpleMultiField(service, new[] { CommonDTD.SERVICE_CODE, CommonDTD.SERVICE });
                }
            }
        }
    }
}
