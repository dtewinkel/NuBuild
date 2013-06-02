using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.Build.Construction;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace Tasks.UnitTests
{
   [TestFixture]
   public class NuPrepareTests
   {
      private const string _projectXml = "<?xml version='1.0' encoding='utf-8'?>\n" +
                                         "<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003' ToolsVersion='4.0' >\n" +
                                         "  <Target Name='build'>\n" +
                                         "{0}" +
                                         "    </NuPrepare>\n" +
                                         "  </Target>\n" +
                                         "</Project>\n";

      [Test]
      public void TestSomething()
      {
         const string taskXml = "    <NuPrepare NuSpec='{0}'\n" +
                                "               ProjectName='Test'\n" +
                                "               VersionSource='Manual'\n" +
                                "               OutputPath='{1}'\n" +
                                "               BuildNumber='12'\n" +
                                "               ReferenceLibraries=''>\n" +
                                "      <Output TaskParameter='NuSpec' ItemName='NuPrepared'/>\n" +
                                "      <Output TaskParameter='Sources' ItemName='NuSources'/>\n" +
                                "      <Output TaskParameter='Targets' ItemName='NuTargets'/>\n";

         const string nuGetSpecXml = "<?xml version='1.0' encoding='utf-8'?>\n" +
                                    "<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>\n" +
                                    "  <metadata>\n" +
                                    "\n" +
                                    "  </metadata>\n" +
                                    "</package>\n";

         string processedSpec;
         BuildResult result = RunTask(nuGetSpecXml, out processedSpec, taskXml);
         Assert.AreEqual(result.OverallResult, BuildResultCode.Success);
      }

      [NotNull]
      BuildResult RunTask([NotNull] string nuGetSpec, out string processedSpec, [NotNull] string taskXml)
      {
         string inputFileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".nuspec";
         string outputFileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".nuspec";

         try
         {
            File.WriteAllText(inputFileName, nuGetSpec, Encoding.UTF8);

            string formattedTaskXml = string.Format(taskXml, inputFileName, outputFileName);
            string xml = string.Format(_projectXml, formattedTaskXml);
            ProjectRootElement rootElement = ProjectRootElement.Create(new XmlTextReader(new StringReader(xml)));
            rootElement.AddUsingTask("NuBuild.MSBuild.NuPrepare", "NuBuild.MSBuild.Tasks.dll", null);

            ProjectInstance project = new ProjectInstance(rootElement);
            BuildRequestData requestData = new BuildRequestData(project, new[]
               {
                  "build"
               });
            BuildParameters buildParameters = new BuildParameters
               {
                  Loggers = new List<ILogger>
                     {
                        new LogInterceptor()
                     }
               };

            BuildResult result = BuildManager.DefaultBuildManager.Build(buildParameters, requestData);

            processedSpec = null;
            if (File.Exists(outputFileName))
            {
               processedSpec = File.ReadAllText(outputFileName);
            }

            return result;
         }
         finally
         {
            if (File.Exists(inputFileName))
            {
               File.Delete(inputFileName);
            }
            if (File.Exists(outputFileName))
            {
               File.Delete(outputFileName);
            }
         }
      }
   }
}