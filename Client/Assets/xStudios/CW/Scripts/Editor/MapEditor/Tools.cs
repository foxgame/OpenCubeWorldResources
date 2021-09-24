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
        public static class Tools
        {
            public static void DisplayPaletteGUI( float toolBarIconSize )
            {
                GUILayout.BeginVertical();


                GUILayout.Label( "Clear Scene:" );
                if ( GUILayout.Button( "Clear Scene" ) )
                {
                    Editor.ClearScene();
                }

                GUILayout.Space( 20f );

                GUILayout.EndVertical();


            }

        }
    }
}
