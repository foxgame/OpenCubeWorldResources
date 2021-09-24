using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class HumanoidSkinnedMesh : MonoBehaviour
    {
        [SerializeField]
        SkinnedMeshRenderer skinnedMeshRenderer;

        [SerializeField]
        Transform hips;

        [SerializeField]
        Transform leftUpLeg;
        [SerializeField]
        Transform leftLeg;
        [SerializeField]
        Transform leftFoot;


        [SerializeField]
        Transform rightUpLeg;
        [SerializeField]
        Transform spine;

        [SerializeField]
        Transform chest;

        [SerializeField]
        Transform[] bones;

        private void Awake()
        {
        }

        private void Start()
        {
            Debug.Log( skinnedMeshRenderer.sharedMesh.boneWeights.Length );
        }
    }
}
