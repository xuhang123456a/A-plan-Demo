using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Other;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    private static readonly string TestOutPath = "Assets/Other/SerializeTestOut/";

    void Start()
    {
        if (!Directory.Exists(TestOutPath))
            Directory.CreateDirectory(TestOutPath);


        Hero hero = new Hero()
        {
            id = 1,
            name = "牛头",
            hp = 1000,
            mp = 100,
            equip = new List<int>() { 1, 2, 3, 4 }
        };

        // xml序列化
        XmlSerialize(hero);
        Hero heroXml = XmlDeserialize();
        var showHeroXml = heroXml;
        ShowHeroData(showHeroXml);

        // 二进制序列化
        BinarySerialize(hero);
        Hero heroBytes = BinaryDeserialize();
        var showHeroBytes = heroBytes;
        ShowHeroData(showHeroBytes);

        // asset序列化
        AssetSerialize assetSerialize = AssetDeserialize();
        ShowHeroData(assetSerialize);
    }

    private void ShowHeroData(Hero showHero)
    {
        Debug.Log($"英雄的 id = {showHero.id}\n name = {showHero.name}\n hp = {showHero.hp}\n mp = {showHero.mp}");
        foreach (var equip in showHero.equip)
        {
            Debug.Log($"装备有{equip}");
        }
    }

    private void ShowHeroData(AssetSerialize showHero)
    {
        Debug.Log($"英雄的 id = {showHero.id}\n name = {showHero.name}\n hp = {showHero.hp}\n mp = {showHero.mp}");
        foreach (var equip in showHero.equip)
        {
            Debug.Log($"装备有{equip}");
        }
    }

    #region xml序列化

    private void XmlSerialize(Hero hero)
    {
        using FileStream fs = new FileStream(TestOutPath + $"{hero.GetType()}.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        using StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
        XmlSerializer xmlSerializer = new XmlSerializer(hero.GetType());
        xmlSerializer.Serialize(sw, hero);
    }

    private Hero XmlDeserialize()
    {
        using FileStream fs = new FileStream(TestOutPath + "Hero.xml", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Hero));
        Hero hero = (Hero)xmlSerializer.Deserialize(fs);
        return hero;
    }

    #endregion

    #region 二进制序列化

    private void BinarySerialize(Hero hero)
    {
        FileStream fs = new FileStream(TestOutPath + $"{hero.GetType()}.bytes", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, hero);
        fs.Close();
    }

    private Hero BinaryDeserialize()
    {
        // FileStream fs = new FileStream(TestOutPath + "hero.bytes", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        // BinaryFormatter bf = new BinaryFormatter();
        // Hero hero = (Hero)bf.Deserialize(fs);
        // fs.Close();
        // return hero;

        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(TestOutPath + "Hero.bytes");
        MemoryStream ms = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        Hero hero = (Hero)bf.Deserialize(ms);
        ms.Close();
        return hero;
    }

    #endregion

    #region Asset序列化

    private AssetSerialize AssetDeserialize()
    {
        AssetSerialize assetSerialize = AssetDatabase.LoadAssetAtPath<AssetSerialize>(TestOutPath + "Hero.asset");
        return assetSerialize;
    }

    #endregion
}