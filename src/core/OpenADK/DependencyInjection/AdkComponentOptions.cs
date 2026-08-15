using System;
using OpenADK.Library.Impl;
using OpenADK.Library.Tools.Policy;

namespace OpenADK.Library
{
    /// <summary>
    /// Factory delegates for agent-owned ADK components. Replace only the component
    /// that needs customization and retain the defaults for everything else.
    /// </summary>
    public sealed class AdkComponentOptions
    {
        /// <summary/>
        public Func<Agent, IZoneFactory> ZoneFactory { get; set; }

        /// <summary/>
        public Func<Agent, ITopicFactory> TopicFactory { get; set; }

        /// <summary/>
        public Func<Agent, ITransportManager> TransportManager { get; set; }

        /// <summary/>
        public Func<Agent, RequestCache> RequestCache { get; set; }

        /// <summary/>
        public Func<Agent, PolicyFactory> PolicyFactory { get; set; }

        /// <summary/>
        public Func<Agent, PolicyManager> PolicyManager { get; set; }

        /// <summary/>
        public Func<Agent, ISIFPrimitives> SifPrimitives { get; set; }

        /// <summary/>
        public Func<Agent, DataObjectOutputStreamImpl> DataObjectOutputStream { get; set; }
    }
}
