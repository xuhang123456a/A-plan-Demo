using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class Hero
{
    [XmlAttribute("Id")] public int id;
    [XmlAttribute("Name")] public string name;
    [XmlAttribute("Lv")] public int lv;
    [XmlAttribute("Hp")] public int hp;
    [XmlAttribute("Mp")] public int mp;
    [XmlElement("Equip")] public List<int> equip;
}