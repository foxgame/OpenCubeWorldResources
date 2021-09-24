using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class MapRegion : MonoBehaviour
    {
        [SerializeField]
        Vector3Int position;
        [SerializeField]
        int size = 0;
        [SerializeField]
        Bounds bounds;

        public Vector3Int Position { get { return position; } set { position = value; } }
        public int Size { get { return size; } set { size = value; } }
        public Bounds Bounds { get { return bounds; } set { bounds = value; } }

        public void UpdateBounds()
        {

        }
    }
}

