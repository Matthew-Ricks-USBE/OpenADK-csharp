using System;

namespace OpenADK.Library
{
    /// <summary>
    /// Configures an <see cref="IAdkRuntime"/> registered with dependency injection.
    /// </summary>
    public sealed class AdkOptions
    {
        public SifVersion SifVersion { get; set; } = SifVersion.LATEST;

        public SIFVariant Variant { get; set; } = SIFVariant.SIF_US;

        public int SdoLibraries { get; set; } = int.MaxValue;

        public AdkDebugFlags Debug { get; set; } = AdkDebugFlags.Very_Detailed;

        /// <summary>
        /// Creates the DTD for a data-model variant. Override this to supply a custom schema.
        /// </summary>
        public Func<SIFVariant, IDtd> DtdFactory { get; set; } = CreateDefaultDtd;

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
