using System.IO;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using NuBuild.MSBuild;
using NUnit.Framework;

namespace Tasks.UnitTests
{
   [TestFixture]
   class NuPackageTests : TaskTestBase
   {
      #region Set up and tear down.

      #endregion

      #region Tests.


      [Test]
      public void MinimalValidFileTest()
      {
         const string fileName = "Test.Project.nupkg";
         string filePath = Path.Combine(BasePath, fileName);

         NuPackage task = CreateTaskFromMetadata("1.0.0", fileName);
         bool success = task.Execute();
         Assert.That(success, Is.True);
         Assert.That(BuildEngine.LogErrorEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogMessageEvents.Count, Is.EqualTo(0));
         Assert.That(task.NuSpec.Length, Is.EqualTo(1));
         Assert.That(task.NuSpec[0].ItemSpec, Is.Not.Empty);
         Assert.That(File.Exists(filePath));
         FilesToClean.Add(filePath);
      }

      [Test]
      public void PreReleaseVersionTest()
      {
         const string fileName = "Test.Project.nupkg";
         string filePath = Path.Combine(BasePath, fileName);

         NuPackage task = CreateTaskFromMetadata("1.0.0-pre1", fileName);
         bool success = task.Execute();
         Assert.That(success, Is.True);
         Assert.That(BuildEngine.LogErrorEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogMessageEvents.Count, Is.EqualTo(0));
         Assert.That(task.NuSpec.Length, Is.EqualTo(1));
         Assert.That(task.NuSpec[0].ItemSpec, Is.Not.Empty);
         Assert.That(File.Exists(filePath));
         FilesToClean.Add(filePath);
      }


      [Test]
      public void PreReleaseVersionInFileNameTest()
      {
         const string fileName = "Test.Project.1.0.0-pre1.nupkg";
         string filePath = Path.Combine(BasePath, fileName);

         NuPackage task = CreateTaskFromMetadata("1.0.0-pre1", fileName);
         bool success = task.Execute();
         Assert.That(success, Is.True);
         Assert.That(BuildEngine.LogErrorEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogMessageEvents.Count, Is.EqualTo(0));
         Assert.That(task.NuSpec.Length, Is.EqualTo(1));
         Assert.That(task.NuSpec[0].ItemSpec, Is.Not.Empty);
         Assert.That(File.Exists(filePath));
         FilesToClean.Add(filePath);
      }

      [Test]
      public void EmptyNuSpecTest()
      {
         NuPackage task = CreateTask("", "1.0.1", "NoFile.nupkg");
         bool success = task.Execute();
         Assert.That(success, Is.False);
         Assert.That(BuildEngine.LogErrorEvents.Count, Is.EqualTo(1));
         Assert.That(BuildEngine.LogErrorEvents[0].Message, Contains.Substring("XmlException"));
         Assert.That(BuildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(BuildEngine.LogMessageEvents.Count, Is.EqualTo(0));
      }


      #endregion

      #region Helper functions.

      private const string _basePackageContent = 
         "<?xml version='1.0' encoding='utf-8'?>\n" + 
         "<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>\n" + 
         "  <metadata>\n" + 
         "    <id>Test.Project</id>\n" + 
         "    <version>{0}</version>\n" + 
         "    <title>A Unit-Test Library Package</title>\n" + 
         "    <authors>Brent M. Spell, Daniël te Winkel</authors>\n" + 
         "    <description>A unit-test package.</description>\n" + 
         "  </metadata>\n" + 
         "</package>\n";


      [NotNull]
      private NuPackage CreateTaskFromMetadata([NotNull] string version, [NotNull] string fileName)
      {
         string nuGetSpec = string.Format(_basePackageContent, version);
         return CreateTask(nuGetSpec, version, fileName);
      }


      [NotNull]
      private NuPackage CreateTask([NotNull] string nuGetSpec, [NotNull] string version, [NotNull] string fileName)
      {
         ITaskItem taskItem = CreateTaskItem(BasePath, nuGetSpec);
         taskItem.SetMetadata("NuPackageVersion", version);
         taskItem.SetMetadata("NuPackagePath", Path.Combine(BasePath, fileName));
         return CreateTask(BasePath, BasePath, new[] { taskItem });
      }


      [NotNull]
      private NuPackage CreateTask([NotNull] string basePath, [NotNull] string projectPath, [NotNull] ITaskItem[] nuSpecTaskItems)
      {
         NuPackage task = new NuPackage
         {
            BuildEngine = BuildEngine,
            NuSpec = nuSpecTaskItems,
            OutputPath = basePath,
            ProjectPath = projectPath
         };
         return task;
      }

      #endregion
   }
}
