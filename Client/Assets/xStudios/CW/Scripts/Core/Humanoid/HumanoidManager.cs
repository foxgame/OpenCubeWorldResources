using UnityEngine;
using System.Collections;
using X;
using UnityEngine.EventSystems;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class HumanoidManager : Singleton<HumanoidManager>
    {
        public HumanoidParts.DefaultData Body;
        public HumanoidParts.DefaultData Chests;

        public HumanoidParts.DefaultData Head;
        public HumanoidParts.DefaultData Mouse;
        public HumanoidParts.DefaultData Moustache;
        public HumanoidParts.DefaultData Nose;
        public HumanoidParts.DefaultData LeftEye;
        public HumanoidParts.DefaultData RightEye;
        public HumanoidParts.DefaultData LeftEyeBrow;
        public HumanoidParts.DefaultData RightEyeBrow;
        public HumanoidParts.DefaultData LeftEar;
        public HumanoidParts.DefaultData RightEar;

        public HumanoidParts.DefaultData LeftArm;
        public HumanoidParts.DefaultData RightArm;
        public HumanoidParts.DefaultData LeftHand;
        public HumanoidParts.DefaultData RightHand;

        public HumanoidParts.DefaultData LeftLeg;
        public HumanoidParts.DefaultData RightLeg;
        public HumanoidParts.DefaultData LeftFoot;
        public HumanoidParts.DefaultData RightFoot;

        public HumanoidParts.DefaultData Tail1;
        public HumanoidParts.DefaultData Tail2;
        public HumanoidParts.DefaultData Tail3;

        public HumanoidParts.DefaultData[] Data;


        protected override void InitSingleton()
        {
            Data = new HumanoidParts.DefaultData[ (int)HumanoidPartType.Count ];
            Data[ (int)HumanoidPartType.Body ] = Body;
//             Data[ (int)HumanoidPartType.Chests ] = Chests;

            Data[ (int)HumanoidPartType.Head ] = Head;
            Data[ (int)HumanoidPartType.Mouse ] = Mouse;
//             Data[ (int)HumanoidPartType.Moustache ] = Moustache;
            Data[ (int)HumanoidPartType.Nose ] = Nose;
            Data[ (int)HumanoidPartType.LeftEye ] = LeftEye;
            Data[ (int)HumanoidPartType.RightEye ] = RightEye;
            Data[ (int)HumanoidPartType.LeftEyeBrow ] = LeftEyeBrow;
            Data[ (int)HumanoidPartType.RightEyeBrow ] = RightEyeBrow;
            Data[ (int)HumanoidPartType.LeftEar ] = LeftEar;
            Data[ (int)HumanoidPartType.RightEar ] = RightEar;

            Data[ (int)HumanoidPartType.LeftArm ] = LeftArm;
            Data[ (int)HumanoidPartType.RightArm ] = RightArm;
            Data[ (int)HumanoidPartType.LeftHand ] = LeftHand;
            Data[ (int)HumanoidPartType.RightHand ] = RightHand;

            Data[ (int)HumanoidPartType.LeftLeg ] = LeftLeg;
            Data[ (int)HumanoidPartType.RightLeg ] = RightLeg;
            Data[ (int)HumanoidPartType.LeftFoot ] = LeftFoot;
            Data[ (int)HumanoidPartType.RightFoot ] = RightFoot;

            Data[ (int)HumanoidPartType.Tail1 ] = Tail1;
            Data[ (int)HumanoidPartType.Tail2 ] = Tail2;
            Data[ (int)HumanoidPartType.Tail3 ] = Tail3;
        }

    }
}

