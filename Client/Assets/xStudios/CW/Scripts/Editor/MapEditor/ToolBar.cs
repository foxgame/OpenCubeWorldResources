using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace X
{
    namespace UnityMapEditor
    {
        public static class ToolBar
        {
            private static Texture2D iconGridToggle;
            private static Texture2D iconGridUp;
            private static Texture2D iconGridDown;
            private static Texture2D iconGridSnap;
            private static GUIContent[] guiContentDrawTool;
            private static Texture2D iconRotate;
            private static Texture2D iconFlip;
            private static Texture2D iconAxisX;
            private static Texture2D iconAxisY;
            private static Texture2D iconAxisZ;
            private static Texture2D iconLoadFromFolder;
            private static Texture2D iconSettings;
            private static Texture2D iconTools;

            private static void LoadImages()
            {
                string path = "Assets/Resources/MapEditor/Textures/";
                iconGridToggle = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Grid_Toggle.png" );

                iconGridUp = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Grid_Up.png" );
                iconGridDown = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Grid_Down.png" );
 
                iconGridSnap = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Grid_Snap.png" );

                guiContentDrawTool = new GUIContent[ 5 ];
//                 guiContentDrawTool[ 1 ] = new GUIContent( AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Palette_Item_Dark_Active.png" ) , "None" );
                guiContentDrawTool[ 0 ] = new GUIContent( AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Pencil.png" ) , "Draw Single Tool" );
                guiContentDrawTool[ 1 ] = new GUIContent( AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Paint_Roller.png" ) , "Draw Continuous Tool" );
                guiContentDrawTool[ 2 ] = new GUIContent( AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Paint_Bucket.png" ) , "Paint Area Tool" );
                guiContentDrawTool[ 3 ] = new GUIContent( AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Randomizer.png" ) , "Randomizer Tool" );
                guiContentDrawTool[ 4 ] = new GUIContent( AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Eraser.png" ) , "Eraser Tool" );

                iconRotate = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Rotate.png" );
                iconFlip = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Flip.png" );
                iconAxisX = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Axis_X.png" );
                iconAxisY = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Axis_Y.png" );
                iconAxisZ = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Axis_Z.png" );
                iconLoadFromFolder = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Load_From_Folder.png" );
                iconSettings = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Settings.png" );
                iconTools = AssetDatabase.LoadAssetAtPath<Texture2D>( path + "Tools.png" );
            }

            public static void DisplayToolbarGUI( float toolBarIconSize )
            {
                if ( iconGridToggle == null )
                {
                    LoadImages();
                }


                GUILayout.Space( toolBarIconSize / 10 );

                GUILayout.BeginVertical();

                GUILayout.Space( toolBarIconSize / 7.5f );

                GUILayout.BeginVertical();

                GUILayout.Label( "Layer: " + Manager.Layer );

                if ( GUILayout.Button( new GUIContent( iconGridUp , "Move Grid Up to " + ( Manager.Layer + 1 ) ) ,
                    GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                {
                    Manager.Layer++;
                    Editor.UpdateLayer();
                    Editor.UpdateBorder();
                    Editor.UpdateRuler();
                }

                EditorGUI.BeginChangeCheck();

                bool gridExists = GUILayout.Toggle( Manager.GridExists ,
                    new GUIContent( iconGridToggle , "Toggle Scene Grid - Current Level " + Manager.Layer ) ,
                    "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize ) );

                if ( gridExists != Manager.GridExists )
                {
                    Manager.GridExists = gridExists;
                    Editor.Reload();
                }

                if ( EditorGUI.EndChangeCheck() )
                {
                    Editor.ShowBoder( Manager.GridExists );

                    Editor.UpdateLayer();
                    Editor.UpdateBorder();
                    Editor.UpdateRuler();
                }

                if ( GUILayout.Button( new GUIContent( iconGridDown , "Move Grid Down to " + ( Manager.Layer - 1 ) ) ,
                    GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                {
                    Manager.Layer--;
                    Editor.UpdateLayer();
                    Editor.UpdateBorder();
                    Editor.UpdateRuler();
                }

                GUILayout.EndVertical();

                GUILayout.Space( toolBarIconSize / 5 );

                GUILayout.BeginVertical();

                string tooltipGridSnap;
                if ( Manager.SnapToGrid )
                    tooltipGridSnap = "Turn OFF Grid Snap";
                else
                    tooltipGridSnap = "Turn ON Grid Snap";

                Manager.SnapToGrid = GUILayout.Toggle(
                    Manager.SnapToGrid ,
                    new GUIContent( iconGridSnap , tooltipGridSnap ) ,
                    "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize ) );

                GUILayout.EndVertical();

                GUILayout.Space( toolBarIconSize / 5 );

                GUILayout.BeginVertical();

                EditorGUI.BeginChangeCheck();

                int newSelectedDrawToolIndex = GUILayout.SelectionGrid(
                    Manager.SelectedDrawToolIndex ,
                    guiContentDrawTool , 1 , "Button" ,
                    GUILayout.Width( toolBarIconSize ) ,
                    GUILayout.Height( toolBarIconSize * 5 ) );

                if ( EditorGUI.EndChangeCheck() )
                {
                    if ( newSelectedDrawToolIndex == Manager.SelectedDrawToolIndex )
                    {
                        Manager.SelectedDrawToolIndex = Utility.InvalidID;
                    }
                    else
                    {
                        Manager.SelectedDrawToolIndex = newSelectedDrawToolIndex;
                    }
                }

                GUILayout.EndVertical();

                GUILayout.Space( toolBarIconSize / 5 );

                GUILayout.BeginVertical( );

                if ( GUILayout.Button( new GUIContent( iconRotate , "Rotate Prefab/Selection" ) ,
                    "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize ) ) )
                {
                    GameObject[] gameObjects = UnityEditor.Selection.gameObjects;

                    for ( int i = 0 ; i < gameObjects.Length ; i++ )
                    {
                        GameObject gameObject = gameObjects[ i ];

                        Undo.RegisterCompleteObjectUndo( gameObject.transform , "Rotate" );

                        if ( gameObject != null )
                        {
                            MapEditorObject mapEditorObject = gameObject.GetComponent<MapEditorObject>();

                            if ( mapEditorObject != null )
                            {
                                switch ( Manager.RotateAxis )
                                {
                                    case Axis.X:
                                        mapEditorObject.transform.localEulerAngles += new Vector3( 90f , 0f , 0f );
                                        break;
                                    case Axis.Y:
                                        mapEditorObject.transform.localEulerAngles += new Vector3( 0f , 90f , 0f );
                                        break;
                                    case Axis.Z:
                                        mapEditorObject.transform.localEulerAngles += new Vector3( 0f , 0f , 90f );
                                        break;
                                }

                                EditorUtility.SetDirty( gameObject );
                            }
                        }
                    }
                }

                switch ( Manager.RotateAxis )
                {
                    case Axis.X:
                        if ( GUILayout.Button( new GUIContent( iconAxisX , "Change Rotate Axis" ) ,
                            "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                        {
                            Manager.RotateAxis = Axis.Y;
                        }
                        break;
                    case Axis.Y:
                        if ( GUILayout.Button( new GUIContent( iconAxisY , "Change Rotate Axis" ) ,
                            "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                        {
                            Manager.RotateAxis = Axis.Z;
                        }
                        break;
                    case Axis.Z:
                        if ( GUILayout.Button( new GUIContent( iconAxisZ , "Change Rotate Axis" ) ,
                            "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                        {
                            Manager.RotateAxis = Axis.X;
                        }
                        break;
                }

                GUILayout.Space( toolBarIconSize / 10 );

                if ( GUILayout.Button( new GUIContent( iconFlip , "Flip Prefab/Selection" ) ,
                    "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize ) ) )
                {
                    GameObject[ ] gameObjects = UnityEditor.Selection.gameObjects;

                    for ( int i = 0 ; i < gameObjects.Length ; i++ )
                    {
                        GameObject gameObject = gameObjects[ i ];

                        Undo.RegisterCompleteObjectUndo( gameObject.transform , "Flip" );

                        if ( gameObject != null )
                        {
                            MapEditorObject mapEditorObject = gameObject.GetComponent<MapEditorObject>();
                            Vector3 vector3 = mapEditorObject.transform.localScale;

                            if ( mapEditorObject != null )
                            {
                                switch ( Manager.FlipAxis )
                                {
                                    case Axis.X:
                                        mapEditorObject.transform.localScale = new Vector3( -vector3.x , vector3.y , vector3.z );
                                        break;
                                    case Axis.Y:
                                        mapEditorObject.transform.localScale = new Vector3( vector3.x , -vector3.y , vector3.z );
                                        break;
                                    case Axis.Z:
                                        mapEditorObject.transform.localScale = new Vector3( vector3.x , vector3.y , -vector3.z );
                                        break;
                                }

                                EditorUtility.SetDirty( gameObject );
                            }
                        }
                    }
                }

                switch ( Manager.FlipAxis )
                {
                    case Axis.X:
                        if ( GUILayout.Button( new GUIContent( iconAxisX , "Change Flip Axis" ) ,
                            "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                        {
                            Manager.FlipAxis = Axis.Y;
                        }
                        break;
                    case Axis.Y:
                        if ( GUILayout.Button( new GUIContent( iconAxisY , "Change Flip Axis" ) ,
                            "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                        {
                            Manager.FlipAxis = Axis.Z;
                        }
                        break;
                    case Axis.Z:
                        if ( GUILayout.Button( new GUIContent( iconAxisZ , "Change Flip Axis" ) ,
                            "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize / 2 ) ) )
                        {
                            Manager.FlipAxis = Axis.X;
                        }
                        break;
                }

                GUILayout.EndVertical();

                GUILayout.Space( toolBarIconSize / 5 );

                GUILayout.BeginVertical();


                if ( GUILayout.Button( new GUIContent( iconLoadFromFolder , "ReLoad prefabs" ) ,
                    "Button" , GUILayout.Width( toolBarIconSize ) , GUILayout.Height( toolBarIconSize ) ) )
                {
                }

                GUILayout.EndVertical();

                GUILayout.EndVertical();
            }

        }
    }
}

