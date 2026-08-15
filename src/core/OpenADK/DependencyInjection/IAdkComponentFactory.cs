using OpenADK.Library.Impl;
using OpenADK.Library.Tools.Policy;

namespace OpenADK.Library
{
    /// <summary>Creates replaceable components owned by an <see cref="Agent"/>.</summary>
    public interface IAdkComponentFactory
    {
        /// <summary/>
        IZoneFactory CreateZoneFactory(Agent agent);

        /// <summary/>
        ITopicFactory CreateTopicFactory(Agent agent);

        /// <summary/>
        ITransportManager CreateTransportManager(Agent agent);

        /// <summary/>
        RequestCache CreateRequestCache(Agent agent);

        /// <summary/>
        PolicyFactory CreatePolicyFactory(Agent agent);

        /// <summary/>
        PolicyManager CreatePolicyManager(Agent agent);

        /// <summary/>
        ISIFPrimitives CreateSifPrimitives(Agent agent);

        /// <summary/>
        DataObjectOutputStreamImpl CreateDataObjectOutputStream(Agent agent);
    }
}
