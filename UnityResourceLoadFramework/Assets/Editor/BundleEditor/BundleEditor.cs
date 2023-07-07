using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.BundleEditor
{
    public class BundleEditor
    {
        private static string ABCONFIGPATH = "Assets/Editor/BundleEditor/AbConfig.asset";

        //key是ab包名，value是路径
        private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
        private static List<string> m_AllFileAB = new List<string>();

        private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();

        [MenuItem("Tools/打包")]
        public static void Build()
        {
            m_AllFileDir.Clear();
            m_AllFileAB.Clear();
            m_AllPrefabDir.Clear();
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

                if (!ContainAllFileAB(path))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    string[] allDepend = AssetDatabase.GetDependencies(path);
                    List<string> allDependPath = new List<string>();
                    for (int j = 0; j < allDepend.Length; j++)
                    {
                        Debug.Log(allDepend[j]);
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

            EditorUtility.ClearProgressBar();
            Debug.Log("打包完成");
        }

        private static bool ContainAllFileAB(string path)
        {
            for (int i = 0; i < m_AllFileAB.Count; i++)
            {
                if (path == m_AllFileAB[i] || path.Contains(m_AllFileAB[i]))
                    return true;
            }

            return false;
        }
    }
}