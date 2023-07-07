using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.Serialization;

[System.Serializable]
public class TestSerilize
{
    [XmlAttribute("Id")] public int id;
    [XmlAttribute("Name")] public string name;
    [XmlArray] public List<int> list;
}