using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace JqueryIntellisense
{
    class Program
    {
        private static Tuple<string, Type> _qunit = new Tuple<string, Type>("http://api.qunitjs.com/resources/api.xml", typeof(QUnitWriter));
        private static Tuple<string, Type> _jquery = new Tuple<string, Type>("http://api.jquery.com/resources/api.xml", typeof(JqueryWriter));

        static void Main(string[] args)
        {
            var project = _qunit;
            var doc = XElement.Load(project.Item1);

            var parser = new Parser();
            var entries = parser.Parse(doc);            
            
            var sb = new StringBuilder();
            IWriter writer = (IWriter)Activator.CreateInstance(project.Item2);

            writer.StartWriting(entries, sb);

            var outputFolderPath = Environment.CurrentDirectory + @"\output\";
            var outputFilePath = Path.Combine(outputFolderPath, writer.Name + ".intellisense.js");

            Directory.CreateDirectory(outputFolderPath);
            File.WriteAllText(outputFilePath, sb.ToString());

            Console.WriteLine("JavaScript documentation generated at " + outputFilePath + Environment.NewLine);
        }
    }
}
