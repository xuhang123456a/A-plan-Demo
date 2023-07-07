using System.Collections.Generic;
using UnityEngine;

namespace Editor.BundleEditor
{
    [CreateAssetMenu(menuName = "CreateAbConfig",fileName = "AbConfig",order = 0)]
    public class AbConfig : ScriptableObject
    {
        public List<string> m_AllPrefabPath = new List<string>();
        public List<FileDirABName> m_FileDirAB = new List<FileDirABName>();

        [System.Serializable]
        public struct FileDirABName
        {
            public string ABName;
            public string Path;
        }
    }
}
