using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;


namespace X
{
    [LuaCallCSharp]
    [System.Serializable]
    public class HumanoidParts
    {
        [LuaCallCSharp]
        [System.Serializable]
        public class DefaultData
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public Color color = Color.white;

            public Vector3 minScale = new Vector3( 0.1f , 0.1f , 0.1f );
            public Vector3 maxScale = Vector3.one;

            public HumanoidShapeType shapeType = HumanoidShapeType.Cube;


            public Vector3 partPosition;
            public Vector3 partRotation;
            public Vector3 partScale;
            public Color partColor = Color.white;

            public Vector3 minPartScale = new Vector3( 0.1f , 0.1f , 0.1f );
            public Vector3 maxPartScale = Vector3.one;

            public HumanoidShapeType partShapeType = HumanoidShapeType.Cube;

            public Bounds bounds;


            public int maxParts = 8;
        }

        public void Init()
        {
            Parts = new Transform[ (int)HumanoidPartType.Count ];

            Parts[ (int)HumanoidPartType.Body ] = Body;
//             Parts[ (int)HumanoidPartType.Chests ] = Chests;

            Parts[ (int)HumanoidPartType.Head ] = Head;
            Parts[ (int)HumanoidPartType.Mouse ] = Mouse;
//             Parts[ (int)HumanoidPartType.Moustache ] = Moustache;
            Parts[ (int)HumanoidPartType.Nose ] = Nose;
            Parts[ (int)HumanoidPartType.LeftEye ] = LeftEye;
            Parts[ (int)HumanoidPartType.RightEye ] = RightEye;
            Parts[ (int)HumanoidPartType.LeftEyeBrow ] = LeftEyeBrow;
            Parts[ (int)HumanoidPartType.RightEyeBrow ] = RightEyeBrow;
            Parts[ (int)HumanoidPartType.LeftEar ] = LeftEar;
            Parts[ (int)HumanoidPartType.RightEar ] = RightEar;

            Parts[ (int)HumanoidPartType.LeftArm ] = LeftArm;
            Parts[ (int)HumanoidPartType.RightArm ] = RightArm;
            Parts[ (int)HumanoidPartType.LeftHand ] = LeftHand;
            Parts[ (int)HumanoidPartType.RightHand ] = RightHand;

            Parts[ (int)HumanoidPartType.LeftLeg ] = LeftLeg;
            Parts[ (int)HumanoidPartType.RightLeg ] = RightLeg;
            Parts[ (int)HumanoidPartType.LeftFoot ] = LeftFoot;
            Parts[ (int)HumanoidPartType.RightFoot ] = RightFoot;

            Parts[ (int)HumanoidPartType.Tail1 ] = Tail1;
            Parts[ (int)HumanoidPartType.Tail2 ] = Tail2;
            Parts[ (int)HumanoidPartType.Tail3 ] = Tail3;
        }

        public Transform[] Parts;

        public Transform Body;
        public Transform Chests;

        public Transform Head;
        public Transform Mouse;
        public Transform Moustache;
        public Transform Nose;
        public Transform LeftEye;
        public Transform RightEye;
        public Transform LeftEyeBrow;
        public Transform RightEyeBrow;
        public Transform LeftEar;
        public Transform RightEar;

        public Transform LeftArm;
        public Transform RightArm;
        public Transform LeftHand;
        public Transform RightHand;

        public Transform LeftLeg;
        public Transform RightLeg;
        public Transform LeftFoot;
        public Transform RightFoot;

        public Transform Tail1;
        public Transform Tail2;
        public Transform Tail3;

    }
}
