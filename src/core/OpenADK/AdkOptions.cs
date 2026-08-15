using System;

namespace OpenADK.Library
{
    /// <summary>
    /// Configures an <see cref="IAdkRuntime"/> registered with dependency injection.
    /// </summary>
    public sealed class AdkOptions
    {
        /// <summary/>
        public SifVersion SifVersion { get; set; } = SifVersion.LATEST;

        /// <summary/>
        public SIFVariant Variant { get; set; } = SIFVariant.SIF_US;

        /// <summary/>
        public int SdoLibraries { get; set; } = int.MaxValue;

        /// <summary/>
        public AdkDebugFlags Debug { get; set; } = AdkDebugFlags.Very_Detailed;

        /// <summary>
        /// Creates the DTD for a data-model variant. Override this to supply a custom schema.
        /// </summary>
        public Func<SIFVariant, IDtd> DtdFactory { get; set; } = CreateDefaultDtd;

        /// <summary/>
        public AdkComponentOptions Components { get; } = new AdkComponentOptions();

        private static IDtd CreateDefaultDtd(SIFVariant variant)
        {
            if (variant == SIFVariant.SIF_US)
            {
                return new OpenADK.Library.us.SifDtd();
            }

            if (variant == SIFVariant.SIF_UK)
            {
                return new OpenADK.Library.uk.SifDtd();
            }

            if (variant == SIFVariant.SIF_AU)
            {
                return new OpenADK.Library.au.SifDtd();
            }

            throw new AdkNotSupportedException("The SIF variant is not supported: " + variant, null);
        }
    }
}
