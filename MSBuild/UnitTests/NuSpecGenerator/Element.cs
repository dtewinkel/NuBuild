using System.Xml;
using JetBrains.Annotations;

namespace NuSpecGenerator
{
   /// <summary>
   /// Base class for writable elements in a <see cref="NuSpec"/> file.
   /// </summary>
   /// <remarks>
   /// Each element must be able to write itself and its children throug the <see cref="Persist"/> method.
   /// </remarks>
   public abstract class Element
   {
      /// <summary>
      /// Read-only property to get the name of the element.
      /// </summary>
      /// <value>
      /// The name of the element.
      /// </value>
      [NotNull]
      public abstract string ElementName { get; }


      /// <summary>
      /// Persist yourself and your children through <paramref name="writer"/>.
      /// </summary>
      /// <param name="writer">The <see cref="XmlWriter"/> used to write the content.</param>
      public virtual void Persist([NotNull] XmlWriter writer)
      {
         writer.WriteStartElement(ElementName);
         PersistChildren(writer);
         writer.WriteEndElement();
      }

      /// <summary>
      /// Persist the child elements of this element using <paramref name="writer"/>.
      /// </summary>
      /// <param name="writer">The <see cref="XmlWriter"/> used to write the content.</param>
      public virtual void PersistChildren([NotNull] XmlWriter writer)
      {
      }
   }
}
