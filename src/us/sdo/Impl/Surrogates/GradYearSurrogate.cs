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
    internal class GradYearSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public GradYearSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            String type = GetTypeAttribute();
            String value = null;
            if ( "Actual".Equals( type ) )
            {
                PartialDateType gradDate = (PartialDateType) o;
                int? year = gradDate.Year;
                if ( year == null )
                {
                    value = "";
                }
                else
                {
                    value = year.Value.ToString( "D4" );
                }
            }
            else
            {
                value = o.TextValue;
                if ( value == null )
                {
                    value = "";
                }
            }

            if ( type != null && value != null )
            {
                writer.WriteStartElement( "GradYear" );
                writer.WriteAttributeString( "Type", type );
                writer.WriteValue( value );
                writer.WriteEndElement();
            }
        }

        private String GetTypeAttribute()
        {
            if ( fElementDef.Name.Equals( "ProjectedGraduationYear" ) )
            {
                return "Projected";
            }
            else if ( fElementDef.Name.Equals( "OnTimeGraduationYear" ) )
            {
                return "Original";
            }
            else if ( fElementDef.Name.Equals( "PartialDateType" ) )
            {
                return "Actual";
            }
            return "";
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            if ( !reader.LocalName.Equals( "GradYear" ) )
            {
                return false;
            }

            String type = reader.GetAttribute( "Type" );
            String value = ConsumeElementTextValue( reader, version );

            IElementDef childDef = null;
            if ( type != null && value != null )
            {
                SifInt intValue = (SifInt) SifTypeConverters.INT.Parse( formatter, value );
                int? year = intValue.Value;
                if ( "Projected".Equals( type ) )
                {
                    childDef = parent.ElementDef.Dtd.LookupElementDef( parent.ElementDef, "ProjectedGraduationYear" );
                    parent.SetField( childDef, intValue );
                }
                else if ( "Original".Equals( type ) )
                {
                    childDef = parent.ElementDef.Dtd.LookupElementDef( parent.ElementDef, "OnTimeGraduationYear" );
                    parent.SetField( childDef, intValue );
                }
                else
                {
                    childDef = parent.ElementDef.Dtd.LookupElementDef( parent.ElementDef, "GraduationDate" );
                    PartialDateType gd = new PartialDateType( year );
                    parent.AddChild( childDef, gd );
                }
            }
            return true;
        }

        public IElementDef LookupBySQP( String sqp )
        {
            // This surrogate maps GradYear[@Type='...'] to different fields depending on the Type attribute
            // The empty path means GradYear element itself (which has the text value)
            if ( sqp.Length == 0 || sqp.Equals( "" ) )
            {
                // Default to the element pointed to by fElementDef
                return fElementDef;
            }
            
            // Handle attribute queries like "@Type"
            if ( sqp.Equals( "@Type" ) )
            {
                // Return null to indicate this is an attribute, not a field
                return null;
            }
            
            return null;
        }

        public INodePointer CreateChild( INodePointer parentPointer, SifFormatter formatter, SifVersion version,
                                         SifXPathContext context )
        {
            return new GradYearNodePointer( parentPointer );
        }

        public INodePointer CreateNodePointer( INodePointer parentPointer, Element element, SifVersion version )
        {
            IElementDef def = element.ElementDef;
            if ( def.Name.Equals( StudentDTD.STUDENTPERSONAL_PROJECTEDGRADUATIONYEAR.Name ) )
            {
                return new GradYearNodePointer( parentPointer, element, "Projected" );
            }
            else if ( def.Name.Equals( StudentDTD.STUDENTPERSONAL_ONTIMEGRADUATIONYEAR.Name) )
            {
                return new GradYearNodePointer( parentPointer, element, "Original" );
            }
            else if ( def.Name.Equals( StudentDTD.STUDENTPERSONAL_GRADUATIONDATE.Name) )
            {
                return new GradYearNodePointer( parentPointer, element, "Actual" );
            }
            return null;
        }

        public string Path
        {
            get { return "GradYear[@Type='" + GetTypeAttribute() + "']"; }
        }

        private class GradYearNodePointer : SurrogateElementPointer<Element>
        {
            private String fAttrValue;

            public GradYearNodePointer( INodePointer parentPointer )
                : base( parentPointer, "GradYear" )
            {
            }

            public GradYearNodePointer( INodePointer parentPointer, Element field, String attrValue )
                : base( parentPointer, "GradYear", field, false )
            {
                fAttrValue = attrValue;
            }

            public override void SetValue( Object value )
            {
                SifSimpleType sifValue = null;
                if ( value is SifInt )
                {
                    sifValue = (SifInt) value;
                }
                else
                {
                    sifValue = SifTypeConverters.INT.GetSifSimpleType( value );
                }
                Element e = getElement();
                if ( e is PartialDateType )
                {
                    SifInt intVal = sifValue as SifInt;
                    ((PartialDateType) e).Year = intVal.Value;
                }
                else
                {
                    e.SifValue = sifValue;
                }
            }

            public override INodeIterator GetAttributes()
            {
                if ( !String.IsNullOrEmpty( fAttrValue ) )
                {
                    return new FauxAttribute( this, "Type", fAttrValue );
                }
                return null;
            }

            public override INodePointer CreateAttribute( SifXPathContext context, string name )
            {
                return new GradYearTypePointer( this );
            }

            /// <summary>
            /// Returns a cloned instance of this NodePointer
            /// </summary>
            public override INodePointer Clone()
            {
                return new GradYearNodePointer( this.Parent, getElement(), fAttrValue );
            }

            private class GradYearTypePointer : FauxElementPointer
            {
                public GradYearTypePointer( INodePointer parentPointer )
                    : base( parentPointer, "Type", true )
                {
                }

                public override object GetBaseValue()
                {
                    return null;
                }

                public override object Node
                {
                    get { return null; }
                }

                public override void SetValue( object value )
                {
                    SetGradYearType( value.ToString() );
                }

                private void SetGradYearType( String type )
                {
                    GradYearNodePointer parent = (GradYearNodePointer) Parent;
                    parent.fAttrValue = type;

                    SifElementPointer parentPointer = (SifElementPointer) parent.Parent;
                    SifElement sp = (SifElement) parentPointer.GetBaseValue();
                    SifSimpleType nullValue = SifTypeConverters.INT.GetSifSimpleType( null );
                    Element field = null;

                    if ( "Projected".Equals( type ) )
                    {
                        field = sp.GetField( "ProjectedGraduationYear" );
                        if ( field == null )
                        {
                            IElementDef childDef = sp.ElementDef.Dtd.LookupElementDef( sp.ElementDef, "ProjectedGraduationYear" );
                            field = sp.SetField( childDef, nullValue );
                        }
                    }
                    else if ( "Original".Equals( type ) )
                    {
                        field = sp.GetField( "OnTimeGraduationYear" );
                        if ( field == null )
                        {
                            IElementDef childDef = sp.ElementDef.Dtd.LookupElementDef( sp.ElementDef, "OnTimeGraduationYear" );
                            field = sp.SetField( childDef, nullValue );
                        }
                    }
                    else
                    {
                        PartialDateType gd = new PartialDateType( (String) null );
                        IElementDef childDef = sp.ElementDef.Dtd.LookupElementDef( sp.ElementDef, "GraduationDate" );
                        sp.AddChild( childDef, gd );
                        field = gd;
                    }
                    parent.setElement( (Element) field );
                }

                public override INodePointer Clone()
                {
                    return new GradYearTypePointer( Parent );
                }
            }
        }
    }
}
