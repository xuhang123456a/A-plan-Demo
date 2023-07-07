using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Other
{
    [CreateAssetMenu(menuName = "CreateScriptableObject",fileName = "new_SO",order = 10)]
    public class AssetSerialize : ScriptableObject
    {
        public int id;
        public string sName;
        public List<string> list;
    }
}
