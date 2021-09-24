using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InitDOTS : MonoBehaviour
{
    private void Awake()
    {
        DefaultWorldInitialization.Initialize( "Default World" , false );
    }

}
