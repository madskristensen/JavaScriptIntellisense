using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JqueryIntellisense
{
    public class JqueryUIWriter : IWriter
    {
        public int Count { get; set; }

        public string Name
        {
            get { return "jquery.ui"; }
        }

        public void StartWriting(List<Entry> list, StringBuilder sb)
        {
            Count = 0;

            Write(list, "jQuery", sb);
        }

        private void Write(IEnumerable<Entry> entries, string objectName, StringBuilder sb)
        {
            Count += entries.Count();
            bool isRoot = false;
            var hash = "_" + Math.Abs(objectName.GetHashCode());


            sb.AppendLine("intellisense.annotate(" + objectName + ", {");
            isRoot = true;

            WriteEntry(entries, sb);

            sb.AppendLine("});" + Environment.NewLine);

            if (!isRoot)
            {
                sb.AppendLine("return _object;");
                sb.AppendLine("};");
                sb.AppendLine("intellisense.redirectDefinition(jQuery.Callbacks, " + hash + ");" + Environment.NewLine);
            }
        }

        private static void WriteEntry(IEnumerable<Entry> entries, StringBuilder sb)
        {
            foreach (Entry entry in entries)
            {
                sb.AppendLine("  '" + ResolveName(entry.Name) + "': function() {");

                if (entry.Signatures.Count == 0 || entry.Signatures.Count(s => s.Arguments.Count() > 0) == 0)
                    sb.AppendLine("    /// <summary>" + entry.Description.Replace("\r", " ").Replace("\n", " ").Trim() + "</summary>");

                WriteSignatures(sb, entry);
                entry.Arguments.ForEach(a => WriteArgument(a, sb));

                if (entry.Signatures.Count == 0 && !string.IsNullOrEmpty(entry.Return) && entry.Return != "undefined")
                    sb.AppendLine("    /// <returns type=\"" + entry.Return + "\" />");

                sb.AppendLine("  },");
            }
        }

        private static void WriteCallbacks(string objectName, string parameters, StringBuilder sb, string hash)
        {
            sb.AppendLine("var " + hash + " = " + objectName + ";");
            sb.AppendLine(objectName + " = function(" + parameters + ") {");
            sb.AppendLine("var _object = " + hash + "(" + parameters + ");");
            sb.AppendLine("intellisense.annotate(_object, {");
        }

        static private string ResolveName(string name)
        {
            int index = name.IndexOf('.') + 1;
            return (index > 0) ? name.Substring(index) : name;
        }

        private static void WriteSignatures(StringBuilder sb, Entry entry)
        {
            foreach (var signature in entry.Signatures)
            {
                sb.AppendLine("    /// <signature>");
                sb.AppendLine("    ///   <summary>" + entry.Description.Replace("\r", " ").Replace("\n", " ").Trim() + "</summary>");
                signature.Arguments.ForEach(a => WriteArgument(a, sb));

                if (!string.IsNullOrEmpty(entry.Return) && entry.Return != "undefined")
                    sb.AppendLine("    ///   <returns type=\"" + entry.Return + "\" />");

                sb.AppendLine("    /// </signature>");
            }
        }

        static private void WriteArgument(Argument argument, StringBuilder sb)
        {
            sb.Append("    ///   <param name=\"" + argument.Name + "\" type=\"" + argument.Type + "\">");
            sb.Append(argument.Description.Replace("\r", " ").Replace("\n", " ").Trim());
            sb.Append("</param>" + Environment.NewLine);
        }
    }
}
