using System;
using System.IO;
using OpenADK.Library;
using NUnit.Framework;
using Library.UnitTesting.Framework;


namespace Library.Nunit.US
{
    public class UsAdkTest : AdkTest
    {
        protected Stream GetResourceStream(string shortName)
        {
            Type thisType = typeof (UsAdkTest);
            return thisType.Assembly.GetManifestResourceStream(thisType.Namespace + ".res." + shortName);
        }
    }
}
