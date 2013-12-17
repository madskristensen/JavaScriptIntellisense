using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JqueryIntellisense
{
    [Serializable]
    public class Entry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Return { get; set; }
        public List<Signature> Signatures { get; set; }
        public List<Argument> Arguments { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Entry p = obj as Entry;
            if (p == null)
            {
                return false;
            }

            return (this.Name == p.Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }

    [Serializable]
    public class Signature {
        [XmlArray("argument")]
        public List<Argument> Arguments { get; set; }   
    }

    [Serializable]
    public class Argument {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("desc")]
        public string Description { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("optional")]
        public bool IsOptional { get; set; }
    }
}
