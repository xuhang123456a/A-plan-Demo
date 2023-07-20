using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    //[CreateAssetMenu(menuName = "CreateScriptableObject",fileName = "Hero",order = 10)]
    public class AssetSerialize : ScriptableObject
    {
        public int id;
        public string name;
        public int lv;
        public int hp;
        public int mp;
        public List<string> equip;
    }
}
