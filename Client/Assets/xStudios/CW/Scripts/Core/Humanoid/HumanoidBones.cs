using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public enum HumanoidBoneType
    {
        Hips = 0,
        Spine,
        Chest,
        Neck,
        Head,
        Jaw,
        LeftEye,
        RightEye,
        LeftEar,
        RightEar,

        LeftShoulder,
        RightShoulder,
        LeftArm,
        RightArm,
        LeftForeArm,
        RightForeArm,
        LeftHand,
        RightHand,

        LeftUpLeg,
        RightUpLeg,
        LeftLeg,
        RightLeg,
        LeftFoot,
        RightFoot,
        LeftToes,
        RightToes,

        Tail1,
        Tail2,
        Tail3,
    }

    [System.Serializable]
    [LuaCallCSharp]
    public class HumanoidBones
    {
        public Transform Hips;
        public Transform Spine;
        public Transform Chest;
        public Transform Neck;
        public Transform Head;
        public Transform Jaw;
        public Transform LeftEye;
        public Transform RightEye;
        public Transform LeftEar;
        public Transform RightEar;

        public Transform LeftShoulder;
        public Transform RightShoulder;
        public Transform LeftArm;
        public Transform RightArm;
        public Transform LeftForeArm;
        public Transform RightForeArm;
        public Transform LeftHand;
        public Transform RightHand;

        public Transform LeftUpLeg;
        public Transform RightUpLeg;
        public Transform LeftLeg;
        public Transform RightLeg;
        public Transform LeftFoot;
        public Transform RightFoot;
        public Transform LeftToes;
        public Transform RightToes;

        public Transform Tail1;
        public Transform Tail2;
        public Transform Tail3;

    }
}
