//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;
using OpenADK.Library.Tools.XPath;
using OpenADK.Library.us.Trans;

namespace OpenADK.Library.Impl.Surrogates
{
    internal class RouteElementSurrogate : AbstractRenderSurrogate, IRenderSurrogate
    {
        public RouteElementSurrogate( IElementDef def )
            : base( def )
        {
        }

        public void RenderRaw( XmlWriter writer,
                               SifVersion version,
                               Element o,
                               SifFormatter formatter )
        {
            String type = "Total";
            if ( fElementDef.Name.EndsWith( "Loaded" ) )
            {
                type = "Loaded";
            }

            SifSimpleType typedValue = o.SifValue;

            String elementName = "RouteDuration";
            if ( fElementDef.Name.StartsWith( "RouteDistance" ) )
            {
                elementName = "RouteDistance";
                SimpleField unit = ((SifElement) o).GetField( "Unit" );
                if ( unit != null && unit.TextValue.Equals( "km", StringComparison.OrdinalIgnoreCase ) )
                {
                    // Convert the km value to miles, which is how it is always represented in SIF 1.x
                    if ( typedValue != null )
                    {
                        SifDecimal kilometers = (SifDecimal) typedValue;
                        decimal? rawValue = kilometers.Value;
                        if ( rawValue.HasValue )
                        {
                            rawValue = rawValue.Value * (decimal)0.621371192;
                            typedValue = new SifDecimal( rawValue );
                        }
                    }
                }
            }

            String value = "";
            if ( typedValue != null )
            {
                value = typedValue.ToString( formatter );
            }

            if ( type != null && value != null )
            {
                writer.WriteStartElement( elementName );
                writer.WriteAttributeString( "Type", type );
                writer.WriteValue( value );
                writer.WriteEndElement();
            }
        }

        public bool ReadRaw( XmlReader reader,
                             SifVersion version,
                             SifElement parent,
                             SifFormatter formatter )
        {
            String name = reader.LocalName;

            if ( !name.StartsWith( "RouteD" ) )
            {
                return false;
            }

            BusRouteInfo busRoute = (BusRouteInfo) parent;

            String type = reader.GetAttribute( "Type" );
            String value = ConsumeElementTextValue( reader, version );
            if ( type != null && value != null )
            {
                if ( name.Equals( "RouteDuration" ) )
                {
                    DateTime? timeValue = formatter.ToTime( value );
                    if ( timeValue.HasValue )
                    {
                        TimeSpan duration = timeValue.Value.TimeOfDay;
                        if ( "Total".Equals( type ) )
                        {
                            busRoute.RouteDurationTotal = duration;
                        }
                        else if ( "Loaded".Equals( type ) )
                        {
                            busRoute.RouteDurationLoaded = duration;
                        }
                    }
                }
                else if ( name.Equals( "RouteDistance" ) )
                {
                    decimal distance = decimal.Parse( value );
                    if ( "Total".Equals( type ) )
                    {
                        busRoute.SetRouteDistanceTotal( DistanceUnit.M, distance );
                    }
                    else if ( "Loaded".Equals( type ) )
                    {
                        busRoute.SetRouteDistanceLoaded( DistanceUnit.M, distance );
                    }
                }
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

        public string Path
        {
            get
            {
                String type = "Total";
                if ( fElementDef.Name.EndsWith( "Loaded" ) )
                {
                    type = "Loaded";
                }

                String elementName = "RouteDuration";
                if ( fElementDef.Name.StartsWith( "RouteDistance" ) )
                {
                    elementName = "RouteDistance";
                }

                return elementName + "[@Type='" + type + "']";
            }
        }
    }
}
