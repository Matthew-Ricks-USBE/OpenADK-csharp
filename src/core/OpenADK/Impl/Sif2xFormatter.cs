//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Xml;

namespace OpenADK.Library.Impl
{
    /// <summary/>
    public class Sif2xFormatter : SifFormatter
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";

        /// <summary/>
        public override string ToDateString( DateTime? date )
        {
            if ( !date.HasValue ) {
                return null;
            }
            return date.Value.ToString( DATE_FORMAT );
        }

        /// <summary/>
        public override string ToDateTimeString( DateTime? date )
        {
            if ( !date.HasValue ) {
                return null;
            }
            DateTime dt = date.Value;
            if (dt.Kind == DateTimeKind.Unspecified)
            {
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            }
            return XmlConvert.ToString( dt, XmlDateTimeSerializationMode.Utc );
        }

        /// <summary/>
        public override string ToTimeString( DateTime? time )
        {
            if ( !time.HasValue ) {
                return null;
            }
            DateTime dt = time.Value;
            if( dt.Kind == DateTimeKind.Unspecified )
            {
                dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            }
            string xmlDateTime =
                XmlConvert.ToString( dt, XmlDateTimeSerializationMode.Utc );
            // Remove the date portion
            int tStart = xmlDateTime.IndexOf( 'T' );
            if ( tStart > -1 ) {
                return xmlDateTime.Substring( tStart + 1 );
            }
            return xmlDateTime;
        }

        /// <summary/>
        public override string ToString( int? intValue )
        {
            if ( !intValue.HasValue ) {
                return null;
            }
            return XmlConvert.ToString( intValue.Value );
        }


        /// <summary/>
        public override string ToString(long? longValue)
        {
            if (!longValue.HasValue)
            {
                return null;
            }
            return XmlConvert.ToString(longValue.Value);
        }

        /// <summary/>
        public override string ToString( decimal? decimalValue )
        {
            if ( !decimalValue.HasValue ) {
                return null;
            }
            return XmlConvert.ToString( decimalValue.Value );
        }

        /// <summary/>
        public override string ToString( bool? boolValue )
        {
            if ( !boolValue.HasValue ) {
                return null;
            }
            return XmlConvert.ToString( boolValue.Value );
        }

        /// <summary/>
        public override DateTime? ToDate( string dateValue )
        {
            if ( dateValue == null ) {
                return null;
            }
            return XmlConvert.ToDateTime( dateValue, DATE_FORMAT );
        }

        /// <summary/>
        public override DateTime? ToDateTime( string xmlValue )
        {
            if ( xmlValue == null ) {
                return null;
            }
            return XmlConvert.ToDateTime( xmlValue, XmlDateTimeSerializationMode.Local );
        }

        /// <summary/>
        public override DateTime? ToTime( string xmlValue )
        {
            if ( xmlValue == null ) {
                return null;
            }
            return XmlConvert.ToDateTime( xmlValue, XmlDateTimeSerializationMode.Local );
        }

        /// <summary/>
        public override int? ToInt( string intValue )
        {
            if ( intValue == null ) {
                return null;
            }
            return XmlConvert.ToInt32( intValue );
        }

        /// <summary/>
        public override long? ToLong(string longValue)
        {
            if (longValue == null)
            {
                return null;
            }
            return XmlConvert.ToInt64(longValue);
        }

        /// <summary/>
        public override decimal? ToDecimal( string decimalValue )
        {
            if ( decimalValue == null ) {
                return null;
            }
            return XmlConvert.ToDecimal(decimalValue);
        }

        /// <summary/>
        public override bool? ToBool( string inValue )
        {
            if ( inValue == null ) {
                return null;
            }
            return XmlConvert.ToBoolean( inValue );
        }

        /// <summary/>
        public override bool SupportsNamespaces
        {
            get { return true; }
        }

        /// <summary>
        /// Converts a <code>TimeSpan?</code> value to a String XML representation as an XML duration
        /// </summary>
        /// <param name="duration">A nullable TimeSpan to convert</param>
        /// <returns></returns>
        public override string ToString( TimeSpan? duration )
        {
            if( duration == null ) {
                return null;
            }
            return XmlConvert.ToString( duration.Value );
        }

        /// <summary>
        /// Converts a SIF XML duration value to a .NET <code>TimeSpan?</code> valueS
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override TimeSpan? ToTimeSpan( string value )
        {
           if( value == null ) {
               return null;
           }
            return XmlConvert.ToTimeSpan( value );
        }
    }
}
