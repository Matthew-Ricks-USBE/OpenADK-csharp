//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace OpenADK.Library
{
    /// <summary/>
    /// <summary/>
    [Serializable]
    public class SifLong : AdkDataType<long?>
    {
        /// <summary/>
        public SifLong(long? value)
            : base(value) { }


        /// <summary/>
        protected override SifTypeConverter<long?> GetTypeConverter()
        {
            return SifTypeConverters.LONG;
        }
    }
}
