//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;

namespace OpenADK.Library.Impl.Surrogates
{
    /// <summary>
    /// Specialized parser for reading StudentAddress elements from a stream
    /// </summary>
    internal class StudentAddressPullParser
    {
        /// <summary>
        /// Parses one StudentAddress element from the XML reader stream.
        /// Manually walks through the StudentAddress element and extracts Address child elements.
        /// </summary>
        /// <param name="reader">The XML reader positioned at a StudentAddress element</param>
        /// <param name="version">The SIF version being parsed</param>
        /// <returns>A StudentAddress element parsed from the stream</returns>
        public SifElement ParseOneElementFromStream( XmlReader reader, SifVersion version )
        {
            if ( reader.LocalName != "StudentAddress" )
            {
                throw new AdkParsingException( "Expected StudentAddress element but found " + reader.LocalName, null );
            }

            // Create a StudentAddress wrapper
            StudentAddress studentAddress = new StudentAddress();

            // Read StudentAddress attributes
            while ( reader.MoveToNextAttribute() )
            {
                if ( reader.LocalName == "PickupOrDropoff" )
                {
                    studentAddress.PickupOrDropoff = reader.Value;
                }
                else if ( reader.LocalName == "DayOfWeek" )
                {
                    studentAddress.DayOfWeek = reader.Value;
                }
            }
            reader.MoveToElement();

            // Read into the element
            if ( !reader.IsEmptyElement )
            {
                reader.Read();

                // Read child elements
                while ( reader.NodeType != XmlNodeType.EndElement )
                {
                    if ( reader.NodeType == XmlNodeType.Element && reader.LocalName == "Address" )
                    {
                        // Create an Address element and extract the raw XML
                        // Then use SifParser to parse just this Address element
                        string addressXml = reader.ReadOuterXml();
                        
                        // Parse the Address XML
                        SifParser parser = SifParser.NewInstance();
                        Address address = (Address) parser.Parse( addressXml, null, SifParserFlags.None, version );
                        
                        if ( address != null )
                        {
                            studentAddress.Address = address;
                        }
                    }
                    else if ( reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.SignificantWhitespace )
                    {
                        reader.Read();
                    }
                    else
                    {
                        reader.Skip();
                    }
                }

                // Move past the end element
                if ( reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "StudentAddress" )
                {
                    reader.Read();
                }
            }
            else
            {
                reader.Read();
            }

            return studentAddress;
        }
    }
}
