using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace TypeScriptAST;

public class XmlComment : Comment
{
    public string Name { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }

    internal XmlComment(string text, string name, IEnumerable<KeyValuePair<string, string>> properties)
        : base(text, singleLine: true, multiLine: false)
    {
        Name = name;
        Properties = new ReadOnlyDictionary<string, string>(properties.ToDictionary(_ => _.Key, _ => _.Value));
    }

    public override string ToString()
    {
        return "/// " +
               new XElement(Name, Properties.Select(p => new XAttribute(p.Key, p.Value))).ToString();
    }
}
