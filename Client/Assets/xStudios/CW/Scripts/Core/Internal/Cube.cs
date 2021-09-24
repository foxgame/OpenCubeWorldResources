using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class Cube
    {
        public InternalTextureData textureData = null;

        public FixedVector3 fixedPosition = new FixedVector3();
        public Vector3 position = new Vector3();
        public bool isFixed = false;

        public Cube up = null;
        public Cube down = null;
        public Cube forward = null;
        public Cube back = null;
        public Cube left = null;
        public Cube right = null;

        public CubeMesh cubeMesh = null;

        public bool ignoreDown = true;

        public int verticeStart = 0;
        public int indexStart = 0;
        public int indexCount = 0;

        public int index = 0;

        //   6 - 7   
        //  /   /|
        // 4 - 5 | 
        // | 2 | 3
        // |/  |/
        // 0 - 1

        //         public List<int> upIndexs = null;
        //         public List<int> downIndexs = null;
        //         public List<int> forwardIndexs = null;
        //         public List<int> backIndexs = null;
        //         public List<int> leftIndexs = null;
        //         public List<int> rightIndexs = null;

        public Color upColor = Color.white;
        public Color downColor = Color.white;
        public Color forwardColor = Color.white;
        public Color backColor = Color.white;
        public Color leftColor = Color.white;
        public Color rightColor = Color.white;

    }
}
