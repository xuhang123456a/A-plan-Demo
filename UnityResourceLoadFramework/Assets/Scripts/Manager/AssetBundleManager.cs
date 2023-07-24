using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    //资源关系依赖配表，根据crc来找对应资源
    protected Dictionary<uint, ResourceItem> m_ResourceItemDic = new Dictionary<uint, ResourceItem>();
    public bool LoadAssetBundleConfig()
    {
        m_ResourceItemDic.Clear();
        string configPath = Application.streamingAssetsPath + "/AssetBundleConfig";
        AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
        TextAsset textAsset = configAB.LoadAsset<TextAsset>("AssetBundleConfig");
        if (textAsset == null)
        {
            Debug.LogError("AssestBundleConfig is not exist!");
            return false;
        }

        MemoryStream ms = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig assetBundleConfig = (AssetBundleConfig)bf.Deserialize(ms);
        ms.Close();

        foreach (var abBase in assetBundleConfig.AbList)
        {
            ResourceItem item = new ResourceItem();
            item.m_Crc = abBase.Crc;
            item.m_AssetName = abBase.AssetName;
            item.m_ABName = abBase.AbName;
            item.m_DependAssetBundle = abBase.AbDependence;
            if (m_ResourceItemDic.ContainsKey(abBase.Crc))
            {
                Debug.LogError($"有重复的crc ab包名：{abBase.AbName} 资源名：{abBase.AssetName}");
            }
            else
            {
                m_ResourceItemDic.Add(abBase.Crc,item);
            }
        }

        return true;
    }
}

public class ResourceItem
{
    //资源路径的crc
    public uint m_Crc;

    //该资源的文件名
    public string m_AssetName = string.Empty;

    //该资源所在的AB包名
    public string m_ABName = string.Empty;

    //该资源所依赖的AssetBundle
    public List<string> m_DependAssetBundle = null;

    //该资源加载完的AB包
    public AssetBundle m_AssetBundle = null;
}