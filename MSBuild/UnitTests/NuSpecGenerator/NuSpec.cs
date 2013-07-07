using System;
using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace NuSpecGenerator
{
   /// <summary>
   /// A definition of a NuSpec file for testing.
   /// </summary>
   public class NuSpec
   {
      /// <summary>
      /// Get or Set the root element for the NuSpec file.
      /// </summary>
      /// <value>
      /// The root element for the NuSpec file.
      /// </value>
      [CanBeNull]
      public Package Package { get; set; }


      [CanBeNull]
      public string Version
      {
         get
         {
            if (Package != null && Package.Metadata != null)
            {
               return Package.Metadata.Version;
            }
            return null;
         }


         set
         {
            if (Package != null && Package.Metadata != null)
            {
               Package.Metadata.Version = value;
            }
            else
            {
               throw new InvalidOperationException("");
            }
         }
      }


      public NuSpec()
      {
         Package = new Package();
      }

      public NuSpec([CanBeNull] string id, [CanBeNull] string version)
      {
         Package = new Package
         {
            Metadata =
            {
               Id = id,
               Version = version
            }
         };

      }

      /// <summary>
      /// Write the spec to the file in <paramref name="filename"/>.
      /// </summary>
      /// <param name="filename">
      /// The file name to write the contents of the file to. 
      /// If the file exists, it truncates it and overwrites it with the new content.
      /// </param>
      public void WriteToFile(string filename)
      {
         XmlWriter writer = new XmlTextWriter(filename, Encoding.UTF8);

         if (Package != null)
         {
            Package.Persist(writer);
         }
         
         writer.Close();
      }
   }
}