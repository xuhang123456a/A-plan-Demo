using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class TestSerilize
{
    [XmlAttribute("Id")] public int id;
    [XmlAttribute("Name")] public string name;
    [XmlArray] public List<int> list;
}