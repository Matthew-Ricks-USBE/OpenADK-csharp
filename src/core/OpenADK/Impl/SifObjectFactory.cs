using System;

namespace OpenADK.Library.Impl
{
    public sealed class SifObjectFactory : ISifObjectFactory
    {
        private readonly IAdkRuntime _runtime;

        public SifObjectFactory(IAdkRuntime runtime)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
        }

        public T Create<T>() where T : SifElement, new()
        {
            return (T)Configure(new T());
        }

        public SifElement Create(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            if (!typeof(SifElement).IsAssignableFrom(objectType))
            {
                throw new ArgumentException("The requested type must derive from SifElement.", nameof(objectType));
            }

            return Configure((SifElement)Activator.CreateInstance(objectType));
        }

        public Query CreateQuery(IElementDef objectType)
        {
            EnsureInitialized();
            Query query = new Query(objectType);
            query.SifVersions = new[] { _runtime.SifVersion };
            return query;
        }

        private SifElement Configure(SifElement element)
        {
            EnsureInitialized();

            element.SifVersion = _runtime.SifVersion;
            return element;
        }

        private void EnsureInitialized()
        {
            if (!_runtime.Initialized)
            {
                _runtime.Initialize();
            }
        }
    }
}
