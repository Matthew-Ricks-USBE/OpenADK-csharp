using UsAdkTest = Library.UnitTesting.Framework.AdkTest;

namespace OpenADK.Library.Nunit.UK;

public class UkAdkTest : UsAdkTest
{
    private SifVersion fOriginalUkVersion;

    public override void SetUp()
    {
        if (!Adk.Initialized)
        {
            Adk.Initialize(SifVersion.LATEST, SIFVariant.SIF_UK, (int)uk.SdoLibraryType.All);
        }
        fOriginalUkVersion = Adk.SifVersion;
        Adk.SifVersion = SifVersion.LATEST;
    }

    public override void TearDown()
    {
        Adk.SifVersion = fOriginalUkVersion;
    }
}
