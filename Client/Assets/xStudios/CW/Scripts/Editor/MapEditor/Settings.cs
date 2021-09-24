using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;

namespace X
{
    namespace UnityMapEditor
    {
        public static class Settings
        {
            public static void DisplayPaletteGUI( float toolBarIconSize )
            {
                GUILayout.BeginVertical();

                GUILayout.Label( "Base:" );

                GUILayout.BeginHorizontal();
                Manager.PrefabEditor = GUILayout.Toggle( Manager.PrefabEditor , "Prefab Edit Mode." );

                Manager.Dynamic = GUILayout.Toggle( Manager.Dynamic , "Dynamic Edit Mode." );

                GUILayout.EndHorizontal();

                GUILayout.Space( 20f );

                GUILayout.EndVertical();


            }

        }
    }
}
