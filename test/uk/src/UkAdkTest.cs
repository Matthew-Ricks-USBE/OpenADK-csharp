using UsAdkTest = Library.UnitTesting.Framework.AdkTest;

namespace OpenADK.Library.xUnit.UK;

public class UkAdkTest : UsAdkTest
{
    protected override void ConfigureOptions(AdkOptions options)
    {
        options.SifVersion = SifVersion.LATEST;
        options.Variant = SIFVariant.SIF_UK;
        options.SdoLibraries = (int)uk.SdoLibraryType.All;
    }
}
