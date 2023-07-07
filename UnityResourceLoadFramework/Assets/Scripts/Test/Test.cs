using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Other;
using UnityEditor;
using UnityEngine;
using Utils;

public class Test : MonoBehaviour
{
    void Start()
    {
        // XmlSerializeTest();
        // BinarySerializeTest();
        // UseUnityAssetSerialize();
    }

    #region XML序列化

    private void XmlSerializeTest()
    {
        TestSerilize testSerilize = new TestSerilize();
        testSerilize.id = 1;
        testSerilize.name = "XML序列化";
        testSerilize.list = new List<int>();
        testSerilize.list.Add(2);
        testSerilize.list.Add(3);
        // XmlSerialize(testSerilize);
        UseXmlDeserialize();
    }

    private void XmlSerialize(TestSerilize testSerilize)
    {
        FileStream fileStream = new FileStream(PathUtil.XMLPath + "test.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8);
        XmlSerializer xmlSerializer = new XmlSerializer(testSerilize.GetType());
        xmlSerializer.Serialize(sw, testSerilize);
        sw.Close();
        fileStream.Close();
    }

    private TestSerilize XmlDeserialize()
    {
        FileStream fileStream = new FileStream(PathUtil.XMLPath + "test.xml", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestSerilize));
        TestSerilize testSerilize = (TestSerilize)xmlSerializer.Deserialize(fileStream);
        fileStream.Close();
        return testSerilize;
    }

    private void UseXmlDeserialize()
    {
        TestSerilize testSerilize = XmlDeserialize();
        Debug.Log($"{testSerilize.id},{testSerilize.name}");
        foreach (var item in testSerilize.list)
        {
            Debug.Log(item);
        }
    }

    #endregion

    #region 二进制序列化

    private void BinarySerializeTest()
    {
        TestSerilize testSerilize = new TestSerilize();
        testSerilize.id = 3;
        testSerilize.name = "二进制序列化";
        testSerilize.list = new List<int>
        {
            4,
            5
        };
        // BinarySerialize(testSerilize);
        UseBinaryDeserialize();
    }

    private void BinarySerialize(TestSerilize testSerilize)
    {
        FileStream fileStream = new FileStream(PathUtil.XMLPath + "test.bytes", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, testSerilize);
        fileStream.Close();
    }

    private TestSerilize BinaryDeserialize()
    {
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Other/test.bytes");
        MemoryStream ms = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        TestSerilize testSerilize = (TestSerilize)bf.Deserialize(ms);
        ms.Close();
        return testSerilize;
    }

    private void UseBinaryDeserialize()
    {
        TestSerilize testSerilize = BinaryDeserialize();
        Debug.Log($"{testSerilize.id},{testSerilize.name}");
        foreach (var item in testSerilize.list)
        {
            Debug.Log(item);
        }
    }

    #endregion

    #region Unity Asset 序列化

    public void UseUnityAssetSerialize()
    {
        AssetSerialize assetSerialize = AssetDatabase.LoadAssetAtPath<AssetSerialize>("Assets/Other/test.asset");
        Debug.Log($"{assetSerialize.id},{assetSerialize.sName}");
        foreach (var item in assetSerialize.list)
        {
            Debug.Log(item);
        }
    }

    #endregion
}