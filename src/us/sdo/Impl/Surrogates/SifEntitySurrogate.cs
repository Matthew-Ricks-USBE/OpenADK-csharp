//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;
using OpenADK.Library.Tools.XPath;
using OpenADK.Library.Global;
using OpenADK.Library.us.Reporting;

namespace OpenADK.Library.Impl.Surrogates
{
    internal class SifEntitySurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public SifEntitySurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            SIF_RefId refIdElement = (SIF_RefId) o;
            writer.WriteStartElement( "SifEntity" );
            writer.WriteAttributeString( "ObjectName", refIdElement.SIF_RefObject );
            writer.WriteAttributeString( "RefId", refIdElement.Value );
            writer.WriteEndElement();
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            if ( !reader.LocalName.Equals( "SifEntity" ) )
            {
                return false;
            }
            SIF_RefId refIdElement = new SIF_RefId();
            refIdElement.SIF_RefObject = reader.GetAttribute( "ObjectName" );
            refIdElement.Value = reader.GetAttribute( "RefId" );
            parent.AddChild(ReportingDTD.REPORTSUBMITTERINFO_SIF_REFID, refIdElement );
            reader.Read();
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
            get { return "SifEntity"; }
        }
    }
}
