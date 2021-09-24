using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


namespace X
{
    [LuaCallCSharp]
    public enum HumanoidShapeType
    {
        Cube = 0,
        Sphere,
        Cylinder,
        Capsule,

        Count
    }

    [LuaCallCSharp]
    public enum HumanoidPartType
    {
        Body = 0,
//         Chests,

        Head,
        Mouse,
//         Moustache,
        Nose,
        LeftEye,
        RightEye,
        LeftEyeBrow,
        RightEyeBrow,
        LeftEar,
        RightEar,

        LeftArm,
        RightArm,
        LeftHand,
        RightHand,

        LeftLeg,
        RightLeg,
        LeftFoot,
        RightFoot,

        Tail1,
        Tail2,
        Tail3,

        Count
    }



    [LuaCallCSharp]
    public class HumanoidPart : MonoBehaviour
    {
//         public Vector3 position;
//         public Vector3 rotation;
//         public Vector3 scale;

        public bool enable = true;
        public bool isPart = false;

        public Color color;

        public HumanoidPartType partType;
        public HumanoidShapeType shapeType;
    }

}
