using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace X
{
    [CreateAssetMenu()]
    public class MapEditorSetting : ScriptableObject
    {
        public int MaxMapSize = 1024;
        public int MaxMapHeight = 256;

        public int RegionSize = 8;


        public List<string> ObjectTypeList = new List<string>();
        public List<string> ObjectGroupList = new List<string>();

        public List<UnityMapEditor.ObjectConfig> ObjectConfigLsit = new List<UnityMapEditor.ObjectConfig>();
    }

}

