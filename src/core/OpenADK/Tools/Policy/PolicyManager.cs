//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Text;
using OpenADK.Library.Impl;
using System.Runtime.CompilerServices;

namespace OpenADK.Library.Tools.Policy
{
    /// <summary/>
    public abstract class PolicyManager
    {
        /// <summary>
        /// Applies ADK policy to the outbound message
        /// </summary>
        /// <param name="payload">The message being sent from the ADK</param>
        /// <param name="zone">The zone that the message is being sent to</param>
        public abstract void ApplyOutboundPolicy(SifMessagePayload payload, IZone zone);

    }
}
