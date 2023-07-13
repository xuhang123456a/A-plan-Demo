using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor.BundleEditor
{
    public class BundleEditor
    {
        private static string m_BundleTargetPath = Application.streamingAssetsPath;
        private static string ABCONFIGPATH = "Assets/Editor/BundleEditor/AbConfig.asset";

        //key是ab包名，value是路径
        private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();

        //过滤的list
        private static List<string> m_AllFileAB = new List<string>();

        //单个prefab的ab包
        private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

        //储存所有有效路径
        private static List<string> m_ConfigFile = new List<string>();

        [MenuItem("Tools/打包")]
        public static void Build()
        {
            m_AllFileDir.Clear();
            m_AllFileAB.Clear();
            m_AllPrefabDir.Clear();
            m_ConfigFile.Clear();
            AbConfig abConfig = AssetDatabase.LoadAssetAtPath<AbConfig>(ABCONFIGPATH);
            foreach (var ab in abConfig.m_FileDirAB)
            {
                if (m_AllFileDir.ContainsKey(ab.ABName))
                {
                    Debug.LogError("AB配置错误，请检查！");
                }
                else
                {
                    m_AllFileDir.Add(ab.ABName, ab.Path);
                    m_AllFileAB.Add(ab.Path);
                    m_ConfigFile.Add(ab.Path);
                }
            }

            string[] allStr = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());
            for (int i = 0; i < allStr.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
                bool cancel = EditorUtility.DisplayCancelableProgressBar("查找Prefab", "Path:" + path, (float)i / allStr.Length);
                if (cancel)
                {
                    EditorUtility.ClearProgressBar();
                    break;
                }

                m_ConfigFile.Add(path);

                if (!ContainAllFileAB(path))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    string[] allDepend = AssetDatabase.GetDependencies(path);
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDepend.Length; j++)
                    {
                        // Debug.Log(allDepend[j]);
                        if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))
                        {
                            m_AllFileAB.Add(allDepend[j]);
                            allDependPath.Add(allDepend[j]);
                        }
                    }

                    if (m_AllPrefabDir.ContainsKey(obj.name))
                    {
                        Debug.LogError("存在相同的prefab！名字：" + obj.name);
                    }
                    else
                    {
                        m_AllPrefabDir.Add(obj.name, allDependPath);
                    }
                }
            }

            foreach (var name in m_AllFileDir.Keys)
            {
                SetBundleName(name, m_AllFileDir[name]);
            }

            foreach (var name in m_AllPrefabDir.Keys)
            {
                SetBundleName(name, m_AllPrefabDir[name]);
            }

            BuildAssetBundle();


            string[] allABNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < allABNames.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(allABNames[i], true);
                EditorUtility.DisplayProgressBar("清除AB包名", "名字： " + allABNames[i], (float)i / allABNames.Length);
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            Debug.Log("打包完成");
        }

        private static void SetBundleName(string name, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("没有该文件" + path);
            }
            else
            {
                assetImporter.assetBundleName = name;
            }
        }

        private static void SetBundleName(string name, List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                SetBundleName(name, paths[i]);
            }
        }

        /// <summary>
        /// 是否包含在已有的AB包里，用来做AB包冗余剔除
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool ContainAllFileAB(string path)
        {
            for (int i = 0; i < m_AllFileAB.Count; i++)
            {
                if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && path.Replace(m_AllFileAB[i], "")[0] == '/'))
                    return true;
            }

            return false;
        }

        private static void BuildAssetBundle()
        {
            string[] alllBundles = AssetDatabase.GetAllAssetBundleNames();
            Dictionary<string, string> resPathDic = new Dictionary<string, string>();
            for (int i = 0; i < alllBundles.Length; i++)
            {
                string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(alllBundles[i]);
                for (int j = 0; j < allBundlePath.Length; j++)
                {
                    if (allBundlePath[j].EndsWith(".cs"))
                        continue;

                    Debug.Log("此AB包： " + alllBundles[i] + " 下面包含的资源文件路径: " + allBundlePath[j]);
                    if (IsValidPath(allBundlePath[j]))
                        resPathDic.Add(allBundlePath[j], alllBundles[i]);
                }
            }

            DeleteAB();
            //生成自己的配置表
            WriteData(resPathDic);

            BuildPipeline.BuildAssetBundles(m_BundleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);
        }


        private static void WriteData(Dictionary<string, string> resPathDic)
        {
            AssetBundleConfig assetBundleConfig = new AssetBundleConfig();
            assetBundleConfig.AbList = new List<AbBase>();
            foreach (var path in resPathDic.Keys)
            {
                AbBase abBase = new AbBase();
                abBase.Path = path;
                abBase.Crc = Crc32.Compute(path);
                abBase.AbName = resPathDic[path];
                abBase.AssetName = path.Remove(0, path.LastIndexOf('/') + 1);
                abBase.AbDependence = new List<string>();
                string[] resDependence = AssetDatabase.GetDependencies(path);
                for (int i = 0; i < resDependence.Length; i++)
                {
                    if (resDependence[i] == path || resDependence[i].EndsWith(".cs"))
                        continue;

                    string abName = "";
                    if (resPathDic.TryGetValue(resDependence[i], out abName))
                    {
                        if (abName == resPathDic[path])
                            continue;

                        if (!abBase.AbDependence.Contains(abName))
                        {
                            abBase.AbDependence.Add(abName);
                        }
                    }
                }

                assetBundleConfig.AbList.Add(abBase);
            }

            //写入XML
            string xmlPath = Application.dataPath + "/AssetBundleConfig.xml";
            if (File.Exists(xmlPath))
                File.Delete(xmlPath);
            FileStream xmlfs = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(xmlfs, Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(assetBundleConfig.GetType());
            xs.Serialize(sw, assetBundleConfig);
            sw.Close();
            xmlfs.Close();
            //写入二进制
            foreach (var abBase in assetBundleConfig.AbList)
            {
                abBase.Path = "";
            }

            string bytePath = m_BundleTargetPath + "/AssetBundleConfig.bytes";
            FileStream bytefs = new FileStream(bytePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(bytefs, assetBundleConfig);
            bytefs.Close();
        }

        /// <summary>
        /// 删除无用AB包
        /// </summary>
        private static void DeleteAB()
        {
            string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
            DirectoryInfo direction = new DirectoryInfo(m_BundleTargetPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (ContainABName(files[i].Name, allBundlesName) || files[i].Name.EndsWith(".meta"))
                    continue;
                else
                {
                    Debug.Log("此AB包已被删或改名了：" + files[i].Name);
                    if (File.Exists(files[i].FullName))
                        File.Delete((files[i].FullName));
                }
            }
        }

        private static bool ContainABName(string name, string[] strs)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (name == strs[i])
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 是否是有效路径
        /// </summary>
        /// <returns></returns>
        private static bool IsValidPath(string path)
        {
            for (int i = 0; i < m_ConfigFile.Count; i++)
            {
                if (path.Contains(m_ConfigFile[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}