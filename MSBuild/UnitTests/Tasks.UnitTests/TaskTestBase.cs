using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using JetBrains.Annotations;
using NuSpecGenerator;

namespace Tasks.UnitTests
{
   public abstract class TaskTestBase
   {
      [NotNull]
      protected FakeBuildEngine BuildEngine
      {
         get;
         set;
      }

      [NotNull]
      protected List<string> FilesToClean
      {
         get;
         set;
      }

      protected string BasePath { get; set; }


      protected TaskTestBase()
      {
         FilesToClean = new List<string>();
         BuildEngine = new FakeBuildEngine();
      }


      [TestFixtureSetUp]
      public void TestFixtureSetUp()
      {
         BasePath = Path.Combine(Path.GetTempPath(), "NuBuild.UnitTests\\");
         if (!Directory.Exists(BasePath))
         {
            Directory.CreateDirectory(BasePath);
         }
         Assert.That(Directory.Exists(BasePath));
      }


      [TestFixtureTearDown]
      public void TestFixtureTearDown()
      {
         if (Directory.Exists(BasePath))
         {
            Directory.Delete(BasePath, true);
         }
      }


      [SetUp]
      public void TestSetUp()
      {
         BuildEngine.ClearLoggedEvents();
      }


      [TearDown]
      public void TearDown()
      {
         foreach (string fileName in FilesToClean)
         {
            if (File.Exists(fileName))
            {
               File.Delete(fileName);
            }
         }
      }


      [NotNull]
      protected ITaskItem CreateTaskItem([NotNull] string basePath, [NotNull] string nuGetSpec)
      {
         string inputFileName = GetNuSpecFileName(BasePath);

         File.WriteAllText(inputFileName, nuGetSpec, Encoding.UTF8);

         ITaskItem taskItem = new TaskItem(inputFileName);
         return taskItem;
      }


      [NotNull]
      protected ITaskItem CreateTaskItem([NotNull] NuSpec spec)
      {
         string inputFileName = GetNuSpecFileName(BasePath);

         spec.WriteToFile(inputFileName);

         ITaskItem taskItem = new TaskItem(inputFileName);
         return taskItem;
      }


      [NotNull]
      string GetNuSpecFileName([NotNull] string basePath)
      {
         string fileName = Path.Combine(basePath, Guid.NewGuid() + ".nuspec");
         FilesToClean.Add(fileName);
         return fileName;
      }

   }
}
