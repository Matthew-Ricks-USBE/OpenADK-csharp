//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace OpenADK.Library
{
    /// <summary>  Thrown when an element or attribute is referenced but does not exist.
    /// 
    /// </summary>
    /// <author>  Eric Petersen
    /// </author>
    /// <version>  1.0
    /// </version>
    [Serializable]
    public class AdkSchemaException : AdkException
    {
        /// <summary>
        /// Constructs an exception with an error message
        /// </summary>
        /// <param name="msg"></param>
        public AdkSchemaException( String msg )
            : base( msg, null ) {}

        /// <summary>
        /// Constructs an exception with the error message and the zone that is associated with the error
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="zone"></param>
        public AdkSchemaException( String msg,
                                   IZone zone )
            : base( msg, zone ) {}

        /// <summary>
        /// Constructs an exception with the error message, the zone that is associated with the error
        /// and the Exception that is the cause
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="zone">The zone that this exception applies to, or null</param>
        /// <param name="cause">The root exception</param>
        public AdkSchemaException( String msg,
                                   IZone zone,
                                   Exception cause )
            : base( msg, zone, cause ) {}
    }
}
