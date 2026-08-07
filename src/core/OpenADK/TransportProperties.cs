//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using System;

namespace OpenADK.Library
{
    /// <summary>  Properties describing operational settings of a transport protocol.
    /// 
    /// </summary>
    /// <author>  Eric Petersen
    /// </author>
    /// <version>  1.0
    /// </version>
    [Serializable]
    public abstract class TransportProperties : AdkProperties
    {
        private bool fEnabled = true;

        public bool Enabled
        {
            get { return fEnabled; }
            set { fEnabled = value; }
        }


        /// <summary>  Gets the name of the transport protocol associated with these properties</summary>
        /// <returns> A protocol name such as <i>http</i> or <i>https</i>
        /// </returns>
        public abstract string Protocol { get; }

        /// <summary>  Constructor</summary>
        public TransportProperties()
            : this( (TransportProperties) null ) {}

        /// <summary>  Constructs a TransportProperties object that inherits its properties
        /// from a parent. Call the Agent.getDefaultTransportProperties method to
        /// obtain the default TransportProperties object for a given transport
        /// protocol.
        /// 
        /// </summary>
        /// <param name="parent">The parent TransportProperties object, usually obtained
        /// by calling Agent.getDefaultTransportProperties
        /// </param>
        public TransportProperties( TransportProperties parent )
        {
            fParent = parent;
        }

        /// <summary>  Initialize the TransportProperties with default values</summary>
        public override void Defaults( Object owner )
        {
        }
    }
}
