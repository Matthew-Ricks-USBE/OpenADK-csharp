using System;
using System.IO;
using OpenADK.Library;
using NUnit.Framework;


namespace Library.Nunit.US
{
    public class UsAdkTest
    {
        protected SifVersion fOriginalVersion;

        [SetUp]
        public virtual void SetUp()
        {
            Adk.Initialize();
            fOriginalVersion = Adk.SifVersion;
        }

        [TearDown]
        public virtual void TearDown()
        {
            Adk.SifVersion = fOriginalVersion;
        }

        protected Stream GetResourceStream(string shortName)
        {
            Type thisType = typeof (UsAdkTest);
            return thisType.Assembly.GetManifestResourceStream(thisType.Namespace + ".res." + shortName);
        }
    }
}