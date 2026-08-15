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
    public class SifBoolean : AdkDataType<bool?>
    {
        /// <summary/>
        public SifBoolean( bool? value )
            : base( value ) {}


        /// <summary/>
        protected override SifTypeConverter<bool?> GetTypeConverter()
        {
            return SifTypeConverters.BOOLEAN;
        }
    }
}
