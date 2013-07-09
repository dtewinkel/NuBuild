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

      #region Required fields.

      [CanBeNull]
      public string Id { get; set; }

      [CanBeNull]
      public string Version { get; set; }

      [CanBeNull]
      public string Description { get; set; }

      [CanBeNull]
      public string Authors { get; set; }

      #endregion

      #region Optional fields.

      /// <summary>
      /// Property to get or set the title of the project.
      /// </summary>
      /// <value>
      /// The title of the project. By default a generated <see cref="Guid"/>.
      /// </value>
      [CanBeNull]
      public string Title { get; set; }

      #endregion

      public Metadata()
      {
         Id = "Test.Project";
         Version = "1.0.0";
         Description = "A unit-test package.";
         Authors = "Brent M. Spell, Daniël te Winkel";
         Title = Guid.NewGuid().ToString();
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
         if (Title != null)
         {
            writer.WriteElementString("title", Title);
         }
         if (Authors != null)
         {
            writer.WriteElementString("authors", Authors);
         }
         if (Description != null)
         {
            writer.WriteElementString("description", Description);
         }
      }
   }
}
