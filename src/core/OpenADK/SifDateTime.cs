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
    public class SifDateTime : AdkDataType<DateTime?>
    {
        /// <summary/>
        public SifDateTime( DateTime? value )
            : base( value ) {}


        /// <summary/>
        protected override SifTypeConverter<DateTime?> GetTypeConverter()
        {
            return SifTypeConverters.DATETIME;
        }
    }
}
