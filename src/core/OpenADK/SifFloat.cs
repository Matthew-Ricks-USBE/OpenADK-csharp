//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;

namespace OpenADK.Library
{
    /// <summary/>
    /// <summary/>
    [Serializable]
    public class SifFloat : AdkDataType<float?>
    {
        /// <summary/>
        public SifFloat(float? value)
            : base(value) { }


        /// <summary/>
        protected override SifTypeConverter<float?> GetTypeConverter()
        {
            return SifTypeConverters.FLOAT;
        }
    }
}
