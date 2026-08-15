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
    public class SifInt : AdkDataType<int?>
    {
        /// <summary/>
        public SifInt( int? value )
            : base( value ) {}


        /// <summary/>
        protected override SifTypeConverter<int?> GetTypeConverter()
        {
            return SifTypeConverters.INT;
        }
    }
}
