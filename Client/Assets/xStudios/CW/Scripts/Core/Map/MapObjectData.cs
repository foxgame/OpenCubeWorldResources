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
    public class MapObjectData
    {
        public string ObjectID = "";
        public string UserTag = "";

        public bool IsFixed = false;
        public FixedVector3 FixedPosition = new FixedVector3();
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.one;

        public Color VertexColor = Color.white;

        public string CollisionEnter = "";
        public string CollisionExit = "";
        public string CollisionStay = "";
        public string TriggerEnter = "";
        public string TriggerExit = "";
        public string TriggerStay = "";

        public void Copy( MapObjectData mapObjectData )
        {
            ObjectID = mapObjectData.ObjectID;
            UserTag = mapObjectData.UserTag;
            IsFixed = mapObjectData.IsFixed;
            FixedPosition.Value = mapObjectData.FixedPosition.Value;
            Position = mapObjectData.Position;
            Rotation = mapObjectData.Rotation;
            Scale = mapObjectData.Scale;
            VertexColor = mapObjectData.VertexColor;

            CollisionEnter = mapObjectData.CollisionEnter;
            CollisionExit = mapObjectData.CollisionExit;
            CollisionStay = mapObjectData.CollisionStay;
            TriggerEnter = mapObjectData.TriggerEnter;
            TriggerExit = mapObjectData.TriggerExit;
            TriggerStay = mapObjectData.TriggerStay;
        }

    }

}

