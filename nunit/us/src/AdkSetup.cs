using NUnit.Framework;
using OpenADK.Library;
using OpenADK.Library.us;

#pragma warning disable S3903,CA1050 // Declare types in namespaces
/// <summary>
/// Sets up (global) Adk once for all tests in this assembly
/// superceding any other Adk setup.
/// </summary>
/// <remarks>
/// Some tests naively specify the minimal Adk setup they require;
/// however, since Adk is global, and only accepts the first initialization,
/// this has negative impacts when tests with different Adk requirements are run together.
/// </remarks>
/// <see href="https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html"/>
[SetUpFixture]
public class AdkSetup
#pragma warning restore S3903,CA1050
{
    [OneTimeSetUp]
    public void Setup()
    {
        Adk.Initialize(SifVersion.LATEST, SIFVariant.SIF_US, (int)SdoLibraryType.All);
    }
}
