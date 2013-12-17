using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace JqueryIntellisense
{
    public class Parser
    {
        public List<Entry> Parse(XElement doc)
        {            
            List<Entry> list = new List<Entry>();

            foreach (XElement node in doc.Element("entries").Elements("entry"))
            {
                var entry = CreateEntry(node);
                var existing = list.SingleOrDefault(e => e.Name == entry.Name);
                if (!list.Contains(entry) || (existing != null && existing.Signatures.Count < entry.Signatures.Count))
                {
                    if (existing != null) list.Remove(existing);
                    list.Add(entry);
                }
            }

            return list.OrderBy(e => e.Name).ToList();
        }

        private Entry CreateEntry(XElement node)
        {
            var entry = new Entry();
            entry.Name = GetAttr(node, "name");
            entry.Name = (entry.Name == "jQuery") ? "init" : entry.Name;
            entry.Type = DisambiguateType(GetAttr(node, "type"));
            entry.Return = GetAttr(node, "return");
            entry.Description = node.Element("desc").Value;
            entry.Signatures = CreateSignatures(node);
            entry.Arguments = CreateArguments(node);
            return entry;
        }

        private List<Signature> CreateSignatures(XElement entryNode)
        {
            List<Signature> list = new List<Signature>();
            foreach (var node in entryNode.Elements("signature"))
            {
                var sig = new Signature();
                sig.Arguments = CreateArguments(node);
                if (sig.Arguments.Count > 0)
                    list.Add(sig);
            }

            return list;
        }

        private List<Argument> CreateArguments(XElement parent)
        {
            List<Argument> list = new List<Argument>();
            foreach (var node in parent.Elements("argument"))
            {
                var arg = new Argument();
                arg.Name = GetAttr(node, "name");
                arg.Type = DisambiguateType(GetAttr(node, "type"));
                arg.Description = node.Element("desc") != null ? node.Element("desc").Value : string.Empty;
                list.Add(arg);
            }

            return list;
        }

        private string GetAttr(XElement node, string name)
        {
            if (node.Attribute(name) != null)
                return node.Attribute(name).Value;

            return null;
        }

        private static string DisambiguateType(string type)
        {
            if (type == null)
                return type;

            if (type.Contains(","))
                return type.Split(',').Last().Trim();

            if (type.Contains("/"))
                return type.Split('/').Last().Trim();

            if (type == "Callback")
                return "Function";

            if ((new[] { "Options", "Map", "Any" }).Contains(type))
                return "Object";

            if (type.Equals("selector", StringComparison.OrdinalIgnoreCase) ||
                type.Equals("HTML", StringComparison.OrdinalIgnoreCase))
                return "String";

            if (type == "Integer")
                return "Number";

            if (type == "Elements")
                return "Array";

            if (type == "boolean")
                return "Boolean";

            return type;
        }

    }
}
