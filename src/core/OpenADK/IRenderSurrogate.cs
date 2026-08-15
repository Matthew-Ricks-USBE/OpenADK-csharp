//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;

namespace OpenADK.Library
{
    /// <summary/>
    public interface IRenderSurrogateToDelete
    {
        /// <summary/>
        void Render( SifWriter writer,
                     SifElement element,
                     SifFormatter formatter );
    }
}
