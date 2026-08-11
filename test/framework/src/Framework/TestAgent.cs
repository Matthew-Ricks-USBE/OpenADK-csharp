using OpenADK.Library;
using System;
using System.IO;
using NUnit.Framework;

namespace Library.UnitTesting.Framework
{
   /// <summary>
   /// Summary description for TestAgent.
   /// </summary>
   public class TestAgent : Agent
   {
      private readonly TestZoneFactory fZoneFactory;
      private readonly string fHomeDir;

      public TestAgent(IAdkRuntime runtime, IAdkComponentFactory components)
         : base("TestAgent", runtime, components)
      {
         fHomeDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "agent-work",
            Guid.NewGuid().ToString("N"));
         fZoneFactory = new TestZoneFactory(this);
      }

      public override IZoneFactory ZoneFactory => fZoneFactory;

      public override string HomeDir => fHomeDir;
   }
}
