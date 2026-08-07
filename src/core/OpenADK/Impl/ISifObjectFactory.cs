using System;

namespace OpenADK.Library.Impl
{
    /// <summary>Creates SIF objects configured for an agent's runtime.</summary>
    public interface ISifObjectFactory
    {
        T Create<T>() where T : SifElement, new();

        SifElement Create(Type objectType);

        /// <summary>Creates a query using the runtime's configured SIF version.</summary>
        Query CreateQuery(IElementDef objectType);
    }
}
