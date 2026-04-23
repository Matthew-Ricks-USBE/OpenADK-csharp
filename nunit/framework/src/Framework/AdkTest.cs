using OpenADK.Library;
using NUnit.Framework;
using OpenADK.Library.us;

namespace Library.UnitTesting.Framework
{
    /// <summary>
    /// Summary description for AdkTest.
    /// </summary>
    public class AdkTest
    {
        protected SifVersion fOriginalVersion;

        [SetUp]
        public virtual void SetUp()
        {
            Adk.Initialize(SifVersion.LATEST, SIFVariant.SIF_US, (int)SdoLibraryType.All);
            fOriginalVersion = Adk.SifVersion;
        }

        [TearDown]
        public virtual void TearDown()
        {
            Adk.SifVersion = fOriginalVersion;
        }
    }
}