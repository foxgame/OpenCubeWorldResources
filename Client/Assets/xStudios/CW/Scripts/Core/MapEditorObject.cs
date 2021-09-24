using UnityEngine;
using System.Collections;
using X;
using UnityEngine.UI;
using XLua;

namespace X
{

    [LuaCallCSharp]
    public class MapEditorObject : MonoBehaviour
    {
        [SerializeField]
        MapObjectData mapObjectData = new MapObjectData();

        public MapObjectData MapObjectData { get { return mapObjectData; } }
        public Cube Cube { get; set; }


    }
}
