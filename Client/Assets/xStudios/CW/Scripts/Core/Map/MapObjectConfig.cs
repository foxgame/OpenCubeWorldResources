using UnityEngine;
using System.Collections;
using X;
using System.Collections.Generic;
using XLua;
using System.IO;
using System.Text;

namespace X
{
    [LuaCallCSharp]
    [System.Serializable]
    public class MapObjectConfig
    {
        public string ID = "";

        public MapObjectType Type = MapObjectType.Count; 
        public bool Dynamic = false;
        public FloatList Rigidbody = null;
        public FloatList Gravity = null;
        public string Path = "";
        public string Obj = "";
        public string Collider = "";
        public string Texture = "";

        public Color VertexColor0 = Color.white;
        public Color VertexColor1 = Color.white;

        public IntList VertexIndex = null;
    }

}
