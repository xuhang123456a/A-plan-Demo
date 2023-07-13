using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class AssetBundleConfig
{
    [XmlElement("ABList")]
    public List<AbBase> AbList { get; set; }
}

[System.Serializable]
public class AbBase
{
    [XmlAttribute("Path")]
    public string   Path { get; set; }
    [XmlAttribute("Crc")]
    public uint Crc { get; set; }
    [XmlAttribute("ABName")]
    public string AbName { get; set; }
    [XmlAttribute("AssetName")]
    public string AssetName { get; set; }
    [XmlElement("AbDependence")]
    public List<string> AbDependence { get; set; }
}