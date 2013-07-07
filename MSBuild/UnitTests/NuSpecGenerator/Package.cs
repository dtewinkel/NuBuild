using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JetBrains.Annotations;

namespace NuSpecGenerator
{
   /// <summary>
   /// The root item of a NuSpec file.
   /// </summary>
   public class Package : Element
   {
      /// <summary>
      /// Read-only property to get the name of the element.
      /// </summary>
      public override string ElementName
      {
         get
         {
            return "package";
         }
      }


      /// <summary>
      /// Get or set the metadata in the package. 
      /// </summary>
      [CanBeNull]
      public Metadata Metadata { get; set; }


      public Package()
      {
         Metadata = new Metadata();
      }


      /// <summary>
      /// Persist the child elements of this element using <paramref name="writer"/>.
      /// </summary>
      /// <param name="writer">The <see cref="XmlWriter"/> used to write the content.</param>
      public override void PersistChildren(XmlWriter writer)
      {
         if (Metadata != null)
         {
            Metadata.Persist(writer);
         }
      }
   }
}
