using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace NuSpecGenerator
{
   public class Metadata : Element
   {
      public override string ElementName
      {
         get
         {
            return "metadata";
         }
      }


      [CanBeNull]
      public string Id { get; set; }


      [CanBeNull]
      public string Version { get; set; }

      public Metadata()
      {
         Id = "Test.Project";
         Version = "1.0.0";
      }


      public override void PersistChildren(XmlWriter writer)
      {
         if (Id != null)
         {
            writer.WriteElementString("id", Id);
         }
         if (Version != null)
         {
            writer.WriteElementString("version", Version);
         }
         writer.WriteElementString("title", "A Unit-Test Library Package");
         writer.WriteElementString("authors", "Brent M. Spell, Daniël te Winkel");
         writer.WriteElementString("description", "A unit-test package.");
      }
   }
}
