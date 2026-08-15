using OpenADK.Library.Impl;
using OpenADK.Library.Tools.Policy;

namespace OpenADK.Library
{
    /// <summary>Default component factory used by dependency-injected agents.</summary>
    public class AdkComponentFactory : IAdkComponentFactory
    {
        private readonly AdkComponentOptions _options;

        /// <summary/>
        public AdkComponentFactory(AdkOptions options)
        {
            _options = (options ?? throw new System.ArgumentNullException(nameof(options))).Components;
        }

        /// <summary/>
        public virtual IZoneFactory CreateZoneFactory(Agent agent)
        {
            return _options.ZoneFactory?.Invoke(agent) ?? new ZoneFactoryImpl(agent);
        }

        /// <summary/>
        public virtual ITopicFactory CreateTopicFactory(Agent agent)
        {
            return _options.TopicFactory?.Invoke(agent) ?? new TopicFactoryImpl(agent);
        }

        /// <summary/>
        public virtual ITransportManager CreateTransportManager(Agent agent)
        {
            return _options.TransportManager?.Invoke(agent) ?? new TransportManagerImpl(agent.Runtime);
        }

        /// <summary/>
        public virtual RequestCache CreateRequestCache(Agent agent)
        {
            RequestCache cache = _options.RequestCache?.Invoke(agent) ?? new RequestCacheFile();
            cache.Initialize(agent);
            return cache;
        }

        /// <summary/>
        public virtual PolicyFactory CreatePolicyFactory(Agent agent)
        {
            return _options.PolicyFactory?.Invoke(agent) ?? new AdkDefaultPolicy(agent);
        }

        /// <summary/>
        public virtual PolicyManager CreatePolicyManager(Agent agent)
        {
            if (_options.PolicyManager != null)
            {
                return _options.PolicyManager(agent);
            }

            return new PolicyManagerImpl(agent.Runtime, CreatePolicyFactory(agent));
        }

        /// <summary/>
        public virtual ISIFPrimitives CreateSifPrimitives(Agent agent)
        {
            return _options.SifPrimitives?.Invoke(agent) ?? new SIFPrimitives(agent.Runtime);
        }

        /// <summary/>
        public virtual DataObjectOutputStreamImpl CreateDataObjectOutputStream(Agent agent)
        {
            return _options.DataObjectOutputStream?.Invoke(agent) ?? new DataObjectOutputFileStream();
        }
    }
}
