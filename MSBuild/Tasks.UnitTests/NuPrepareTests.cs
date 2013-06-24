using JetBrains.Annotations;
using Microsoft.Build.Framework;
using NUnit.Framework;
using NuBuild.MSBuild;

namespace Tasks.UnitTests
{
   [TestFixture]
   public class NuPrepareTests : TaskTestBase
   {
      [Test]
      public void TestSomething()
      {
         const string nuGetSpecXml = "<?xml version='1.0' encoding='utf-8'?>\n" +
                                     "<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>\n" +
                                     "  <metadata>\n" +
                                     "    <id>Test.Project</id>\n" +
                                     "    <version>1.0.0</version>\n" +
                                     "  </metadata>\n" +
                                     "</package>\n";

         NuPrepare task = CreateTask(nuGetSpecXml);
         bool success = task.Execute();
         Assert.That(success, Is.True);
         Assert.That(BuildEngine.LogErrorEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogMessageEvents.Count, Is.EqualTo(0));
      }


      [Test]
      public void TestEmptyNuSpec()
      {
         NuPrepare task = CreateTask("");
         bool success = task.Execute();
         Assert.That(success, Is.False);
         Assert.That(BuildEngine.LogErrorEvents.Count, Is.EqualTo(1));
         Assert.That(BuildEngine.LogErrorEvents[0].Message, Contains.Substring("XmlException"));
         Assert.That(BuildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogMessageEvents.Count, Is.EqualTo(0));
      }


      [NotNull]
      private NuPrepare CreateTask([NotNull] string nuGetSpec)
      {
         ITaskItem taskItem = CreateTaskItem(BasePath, nuGetSpec);
         return CreateTask(BasePath, new[] { taskItem });
      }


      [NotNull]
      private NuPrepare CreateTask([NotNull] string basePath, [NotNull] ITaskItem[] nuSpecTaskItems,
         int buildNumber = 1, string versionSource = "Manual")
      {
         return new NuPrepare
         {
            BuildEngine = BuildEngine,
            NuSpec = nuSpecTaskItems,
            OutputPath = basePath,
            BuildNumber = buildNumber,
            VersionSource = versionSource
         };
      }
   }
}