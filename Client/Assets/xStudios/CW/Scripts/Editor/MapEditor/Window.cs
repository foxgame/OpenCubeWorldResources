using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Scenes;

namespace X
{
    namespace UnityMapEditor
    {
        public class Window : EditorWindow
        {
            static bool isInited = false;

            int selectedTab = 0;
            float buildIconSize;

            [MenuItem( "X/MapEditorWindow" )]
            private static void ShowWindow()
            {
                // Get existing open window or if none, make a new one:
                EditorWindow.GetWindow( typeof( Window ) , false , "MapEditor" ).Show();

            }

            static void CloseWindow()
            {
                EditorWindow.GetWindow( typeof( Window ) , false , "MapEditor" ).Close();
            }

//             [MenuItem( "X/MapEditor1" )]
//             private static void ShowWindow1()
//             {
//                 //                 GameObject gameObject = new GameObject();
//                 // 
//                 //                 SubScene subScene = gameObject.AddComponent<SubScene>();
//                 //                 SceneAsset sceneAsset = Editor.GetSubSceneAsset( Vector3Int.zero ); ;
//                 // 
//                 //                 Scene scene = EditorSceneManager.OpenScene( AssetDatabase.GetAssetPath( sceneAsset ) , OpenSceneMode.Additive );
//                 //                 scene.isSubScene = true;
//                 // 
//                 //                 subScene.SceneAsset = sceneAsset;
//                 // 
//                 //                 Scene scene1 = EditorSceneManager.GetSceneByName( subScene.SceneAsset.name );
//                 // 
//                 //                 Debug.Log( scene1 == scene );
//                 // 
//                 //                 GameObject gameObject1 = new GameObject();
//                 //                 EditorSceneManager.MoveGameObjectToScene( gameObject1 , scene );
//                 // 
//                 //                 EditorSceneManager.SaveScene( scene , AssetDatabase.GetAssetPath( subScene.SceneAsset ) );
//             }

            void Init()
            {
                if ( isInited )
                {
                    return;
                }

                Manager.Load();
                Editor.Init();
                Palette.Reload();

                isInited = true;
            }

            void Awake()
            {
                isInited = false;
                Init();
            }

            public bool ProcessHotkeys()
            {
                bool changeMade = false;

                Event currentEvent = Event.current;

                int controlID = GUIUtility.GetControlID( FocusType.Passive );

                // If a key is pressed
                if ( Event.current.GetTypeForControl( controlID ) == EventType.KeyDown )
                {
                }

                return changeMade;
            }

            private void DisplayBuildGUI()
            {
                GUILayout.BeginHorizontal();

                buildIconSize = ( position.height / 15.3f ) * Manager.BuildIconScale;

                ToolBar.DisplayToolbarGUI( buildIconSize );
                Palette.DisplayPaletteGUI( buildIconSize );

                GUILayout.EndHorizontal();
            }

            private void DisplaySettingGUI()
            {
                GUILayout.BeginHorizontal();

                buildIconSize = ( position.height / 15.3f ) * Manager.BuildIconScale;

                Settings.DisplayPaletteGUI( buildIconSize );

                GUILayout.EndHorizontal();
            }

            private void DisplayToolsGUI()
            {
                GUILayout.BeginHorizontal();

                buildIconSize = ( position.height / 15.3f ) * Manager.BuildIconScale;

                Tools.DisplayPaletteGUI( buildIconSize );

                GUILayout.EndHorizontal();
            }

            private void OnEnable()
            {
                SceneView.duringSceneGui -= this.OnScene;
                SceneView.duringSceneGui += this.OnScene;
            }


            private void OnDisable()
            {
                SceneView.duringSceneGui -= this.OnScene;
            }


            private void OnScene( SceneView sceneView )
            {
                Init();

                Editor.UpdateHotKey();
                Editor.UpdateCursor();
                Editor.UpdateDraw();
            }

            private void OnDestroy()
            {
                Editor.Clear();
                isInited = false;
            }

            void OnGUI()
            {
                Init();

                string[ ] tabCaptions = { "Build" , "Settings" , "Tools" };

                selectedTab = GUILayout.Toolbar( selectedTab , tabCaptions );

                switch ( selectedTab )
                {
                    case 0:
                        DisplayBuildGUI();
                        break;
                    case 1:
                        DisplaySettingGUI();
                        break;
                    case 2:
                        DisplayToolsGUI();
                        break;
                }


                if ( Event.current.type == EventType.MouseMove )
                    Repaint();

                if ( ProcessHotkeys() )
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }


        }

    }
}
