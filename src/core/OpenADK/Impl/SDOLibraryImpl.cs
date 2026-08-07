//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections.Generic;

namespace OpenADK.Library.Impl
{
    /// <summary> Title:
    /// Description:
    /// Copyright:    Copyright (c) 2002
    /// Company:
    /// </summary>
    /// <author> 
    /// </author>
    /// <version>  1.0
    /// </version>
    public abstract class SdoLibraryImpl
    {
        /// <summary>The DTD instance into which this library is being loaded.</summary>
        protected internal IDtd Dtd { get; internal set; }

        public abstract void Load();

        public abstract void AddElementMappings( IDictionary<String, IElementDef> dtdMap );
    }
}
