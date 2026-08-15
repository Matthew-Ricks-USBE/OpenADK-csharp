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
    public class SifDecimal : AdkDataType<decimal?>
    {
        /// <summary/>
        public SifDecimal( decimal? value )
            : base( value ) {}


        /// <summary/>
        protected override SifTypeConverter<decimal?> GetTypeConverter()
        {
            return SifTypeConverters.DECIMAL;
        }
    }
}
