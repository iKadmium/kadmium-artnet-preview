using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Assets.Scripts
{
    class FileAccessor
    {
        public static XElement LoadXML(string path)
        {
            XDocument doc = XDocument.Load(path);
            XElement root = StripNamespaces(doc.Root);
            return root;
        }

        private static XElement StripNamespaces(XElement rootElement)
        {
            foreach (var element in rootElement.DescendantsAndSelf())
            {
                // update element name if a namespace is available
                if (element.Name.Namespace != XNamespace.None)
                {
                    element.Name = XNamespace.None.GetName(element.Name.LocalName);
                }

                // check if the element contains attributes with defined namespaces (ignore xml and empty namespaces)
                bool hasDefinedNamespaces = element.Attributes().Any(attribute => attribute.IsNamespaceDeclaration ||
                        (attribute.Name.Namespace != XNamespace.None && attribute.Name.Namespace != XNamespace.Xml));

                if (hasDefinedNamespaces)
                {
                    // ignore attributes with a namespace declaration
                    // strip namespace from attributes with defined namespaces, ignore xml / empty namespaces
                    // xml namespace is ignored to retain the space preserve attribute
                    var attributes = element.Attributes()
                                            .Where(attribute => !attribute.IsNamespaceDeclaration)
                                            .Select(attribute =>
                                                (attribute.Name.Namespace != XNamespace.None && attribute.Name.Namespace != XNamespace.Xml) ?
                                                    new XAttribute(XNamespace.None.GetName(attribute.Name.LocalName), attribute.Value) :
                                                    attribute
                                            );

                    // replace with attributes result
                    element.ReplaceAttributes(attributes);
                }
            }
            return rootElement;
        }
    }
}
