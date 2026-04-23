using UsAdkTest = Library.UnitTesting.Framework.AdkTest;

namespace OpenADK.Library.Nunit.UK;

public class UkAdkTest : UsAdkTest
{
    public override void setUp()
    {
        if (!Adk.Initialized)
        {
            Adk.Initialize(SifVersion.LATEST, SIFVariant.SIF_UK, (int)uk.SdoLibraryType.All);
        }
        Adk.SifVersion = SifVersion.LATEST;
    }
}
