using OpenADK.Library.Impl;
using OpenADK.Library.Tools.Policy;

namespace OpenADK.Library
{
    /// <summary>Creates replaceable components owned by an <see cref="Agent"/>.</summary>
    public interface IAdkComponentFactory
    {
        IZoneFactory CreateZoneFactory(Agent agent);

        ITopicFactory CreateTopicFactory(Agent agent);

        ITransportManager CreateTransportManager(Agent agent);

        RequestCache CreateRequestCache(Agent agent);

        PolicyFactory CreatePolicyFactory(Agent agent);

        PolicyManager CreatePolicyManager(Agent agent);

        ISIFPrimitives CreateSifPrimitives(Agent agent);

        DataObjectOutputStreamImpl CreateDataObjectOutputStream(Agent agent);
    }
}
