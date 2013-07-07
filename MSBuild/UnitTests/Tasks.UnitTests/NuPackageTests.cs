using System.IO;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using NuBuild.MSBuild;
using NUnit.Framework;
using NuSpecGenerator;

namespace Tasks.UnitTests
{
   [TestFixture]
   internal class NuPackageTests : TaskTestBase
   {
      #region Set up and tear down.

      #endregion

      #region Tests.


      [Test]
      public void MinimalValidFileTest()
      {
         // Prepare.
         const string version = "1.0.0-test02";
         string filePath = CreatePackageFileName("Test.Project");

         NuSpec spec = new NuSpec(version);
         NuPackage task = CreateTaskFromSpec(spec, "1.0.0", filePath);

         // Act.
         bool success = task.Execute();

         // Assert.
         AssertSuccessAndNoMessages(success);
         Assert.That(task.NuSpec.Length, Is.EqualTo(1));
         Assert.That(task.NuSpec[0].ItemSpec, Is.Not.Empty);
         Assert.That(File.Exists(filePath));
      }


      [Test]
      public void PreReleaseVersionTest()
      {
         // Prepare.
         const string version = "1.0.0-test02";
         string filePath = CreatePackageFileName("Test.Project");

         NuSpec spec = new NuSpec(version);
         NuPackage task = CreateTaskFromSpec(spec, version, filePath);

         // Act.
         bool success = task.Execute();

         // Assert.
         AssertSuccessAndNoMessages(success);

         Assert.That(task.NuSpec.Length, Is.EqualTo(1));
         Assert.That(task.NuSpec[0].ItemSpec, Is.Not.Empty);
         Assert.That(File.Exists(filePath));
      }


      [Test]
      public void PreReleaseVersionInFileNameTest()
      {
         // Prepare.
         const string version = "1.0.0-pre1";
         string filePath = CreatePackageFileNameWithVersion("Test.Project", version);

         NuSpec spec = new NuSpec(version);
         NuPackage task = CreateTaskFromSpec(spec, version, filePath);

         // Act.
         bool success = task.Execute();

         // Assert.
         AssertSuccessAndNoMessages(success);

         Assert.That(task.NuSpec.Length, Is.EqualTo(1));
         Assert.That(task.NuSpec[0].ItemSpec, Is.Not.Empty);
         Assert.That(File.Exists(filePath));
      }


      [Test]
      public void EmptyNuSpecTest()
      {
         // Prepare.
         NuSpec spec = new NuSpec
         {
            Package = null
         };
         NuPackage task = CreateTaskFromSpec(spec, "", "");

         // Act.
         bool success = task.Execute();

         // Assert.
         AssertFailAndHasErrors(success, errorCount: 1);
         Assert.That(BuildEngine.LogErrorEvents[0].Message, Contains.Substring("XmlException"));
      }


      #endregion

      #region Helper functions.

      private void AssertSuccessAndNoMessages(bool success)
      {
         Assert.That(success, Is.True);

         AssertMessageCounts(0, 0, 0, 0);
      }

      private void AssertFailAndHasErrors(bool success, int errorCount, int warningCount = 0, int customCount = 0, int messageCount = 0)
      {
         Assert.That(success, Is.False);

         AssertMessageCounts(errorCount, warningCount, customCount, messageCount);
      }


      private void AssertMessageCounts(int errorCount, int warningCount, int customCount, int messageCount)
      {
         Assert.That(BuildEngine.LogErrorEvents.Count, Is.EqualTo(errorCount));
         Assert.That(BuildEngine.LogWarningEvents.Count, Is.EqualTo(warningCount));
         Assert.That(BuildEngine.LogCustomEvents.Count, Is.EqualTo(customCount));
         Assert.That(BuildEngine.LogMessageEvents.Count, Is.EqualTo(messageCount));
      }


      [NotNull]
      private string CreatePackageFileName([NotNull] string nameBase)
      {
         string fileName = string.Format("{0}.nupkg", nameBase);
         return Path.Combine(BasePath, fileName);
      }


      [NotNull]
      private string CreatePackageFileNameWithVersion([NotNull] string nameBase, [NotNull] string version)
      {
         return CreatePackageFileName(string.Format("{0}.{1}", nameBase, version));
      }


      [NotNull]
      private NuPackage CreateTaskFromSpec([NotNull] NuSpec spec, [NotNull] string version, [NotNull] string packageFileName)
      {
         ITaskItem taskItem = CreateTaskItem(spec);
         return CreateTask(taskItem, version, packageFileName);
      }


      [NotNull]
      private NuPackage CreateTask([NotNull] ITaskItem taskItem, [NotNull] string version, [NotNull] string packageFileName)
      {
         taskItem.SetMetadata("NuPackageVersion", version);
         taskItem.SetMetadata("NuPackagePath", packageFileName);
         FilesToClean.Add(packageFileName);
         return CreateTask(BasePath, BasePath, new[]
         {
            taskItem
         });
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
