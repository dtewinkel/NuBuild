using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using NuBuild.MSBuild;

namespace Tasks.UnitTests
{
   [TestFixture]
   public class NuPrepareTests
   {
      [NotNull]
      private readonly FakeBuildEngine _buildEngine = new FakeBuildEngine();

      [NotNull]
      private readonly List<string> _filesToClean = new List<string>();

      private string _basePath;


      [TestFixtureSetUp]
      public void TestFixtureSetUp()
      {
         _basePath = Path.Combine(Path.GetTempPath(), "NuBuild.UnitTests\\");
         if (!Directory.Exists(_basePath))
         {
            Directory.CreateDirectory(_basePath);
         }
         Assert.That(Directory.Exists(_basePath));
      }


      [TestFixtureTearDown]
      public void TestFixtureTearDown()
      {
         if (Directory.Exists(_basePath))
         {
            Directory.Delete(_basePath, true);
         }
      }


      [SetUp]
      public void TestSetUp()
      {
         _buildEngine.ClearLoggedEvents();
      }


      [TearDown]
      public void TearDown()
      {
         foreach (string fileName in _filesToClean)
         {
            if (File.Exists(fileName))
            {
               File.Delete(fileName);
            }
         }
      }


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
         Assert.That(_buildEngine.LogErrorEvents.Count, Is.EqualTo(0));
         Assert.That(_buildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(_buildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(_buildEngine.LogMessageEvents.Count, Is.EqualTo(0));
      }


      [Test]
      public void TestEmptyNuSpec()
      {
         NuPrepare task = CreateTask("");
         bool success = task.Execute();
         Assert.That(success, Is.False);
         Assert.That(_buildEngine.LogErrorEvents.Count, Is.EqualTo(1));
         Assert.That(_buildEngine.LogErrorEvents[0].Message, Contains.Substring("XmlException"));
         Assert.That(_buildEngine.LogWarningEvents.Count, Is.EqualTo(0));
         Assert.That(_buildEngine.LogCustomEvents.Count, Is.EqualTo(0));
         Assert.That(_buildEngine.LogMessageEvents.Count, Is.EqualTo(0));
      }


      [NotNull]
      private NuPrepare CreateTask([NotNull] string nuGetSpec)
      {
         string inputFileName = Path.Combine(_basePath, Guid.NewGuid().ToString() + ".nuspec");
         _filesToClean.Add(inputFileName);
         string outputPath = _basePath;

         File.WriteAllText(inputFileName, nuGetSpec, Encoding.UTF8);

         ITaskItem taskItem = new TaskItem(inputFileName);
         return new NuPrepare
            {
               BuildEngine = _buildEngine,
               NuSpec = new[]
                  {
                     taskItem
                  },
               OutputPath = outputPath,
               BuildNumber = 12,
               VersionSource = "Manual"
            };
      }
   }
}