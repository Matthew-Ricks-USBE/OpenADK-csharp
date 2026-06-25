//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace OpenADK.Util
{
    /// <summary>  Indicates an internal error has occurred
    /// 
    /// </summary>
    /// <author>  Eric Petersen
    /// </author>
    /// <version>  1.0
    /// </version>
    [Serializable]
    public class InternalErrorException : ApplicationException
    {
        /// <summary>  Constructs an error with a message</summary>
        public InternalErrorException( string msg )
            : base( msg ) {}

        /// <summary>
        /// Constructs an exception with a message and the internal exception
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="internalException"></param>
        public InternalErrorException( string msg,
                                       Exception internalException )
            : base( msg, internalException ) {}

    }
}
