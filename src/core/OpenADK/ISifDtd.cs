//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using OpenADK.Util;
using OpenADK.Library.Impl;


namespace OpenADK.Library
{
    /// <summary/>
    public interface ISifDtd : IDtd
    {
        /// <summary/>
        string Variant { get; }

        /// <summary/>
        string XMLNS_BASE{ get; }

        /// <summary/>
        String BasePackageName { get; }
        
        /// <summary/>
        int[] AvailableLibraries{ get; }

        /// <summary/>
        List<string> LoadedLibraryNames{ get; }
    }
}
