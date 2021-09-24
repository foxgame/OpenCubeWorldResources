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
        public static class Palette
        {
            static Vector2 paletteScrollPos = new Vector2();
            static int prefabPaletteColumnCount = 5;
            const int scrollBarWidth = 19;

            static GUIContent[] GUIContents = null;
            static bool[ ] Selection = null;

            public static Object GetSelectObject()
            {
                if ( Manager.SelectedItemIndex >= 0 &&
                    Manager.SelectedItemIndex < GUIContents.Length )
                {
                    string id = GUIContents[ Manager.SelectedItemIndex ].text;
                    Object obj = null;

                    if ( Manager.ObjectDictionary.TryGetValue( id , out obj ) )
                    {
                        return obj;
                    }

                    return null;
                }

                return null;
            }

            public static Object GetRandomObject()
            {
                List<int> list = new List<int>();
                for ( int i = 0 ; i < Selection.Length ; i++ )
                {
                    if ( Selection[ i ] )
                    {
                        list.Add( i );
                    }
                }

                if ( list.Count > 0 )
                {
                    int r = Random.Range( 0 , list.Count );

                    string id = GUIContents[ r ].text;
                    Object obj = null;

                    if ( Manager.ObjectDictionary.TryGetValue( id , out obj ) )
                    {
                        return obj;
                    }
                }

                return null;
            }

            public static void DisplayPaletteGUI( float toolBarIconSize )
            {
                GUILayout.BeginVertical( );


                GUILayout.BeginHorizontal();

                if ( GUIContents == null )
                {
                    GUIContents = Manager.GetGUIContents();
                    Selection = new bool[ GUIContents.Length ];
                }

                int newSelectedObjectTypeIndex = EditorGUILayout.Popup( Manager.SelectedObjectTypeIndex , Manager.ObjectTypes );
                UnityEngine.GUI.FocusControl( null );
                if ( newSelectedObjectTypeIndex != Manager.SelectedObjectTypeIndex )
                {
                    Manager.SelectedObjectTypeIndex = newSelectedObjectTypeIndex;
                    GUIContents = Manager.GetGUIContents();
                    Selection = new bool[ GUIContents.Length ];
                    RemovePrefabSelection();
                }

                int newSelectedObjectGroupIndex = EditorGUILayout.Popup( Manager.SelectedObjectGroupIndex , Manager.ObjectGroups ) ;
                UnityEngine.GUI.FocusControl( null );
                if ( newSelectedObjectGroupIndex != Manager.SelectedObjectGroupIndex )
                {
                    Manager.SelectedObjectGroupIndex = newSelectedObjectGroupIndex;
                    GUIContents = Manager.GetGUIContents();
                    Selection = new bool[ GUIContents.Length ];
                    RemovePrefabSelection();
                }


                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();


                paletteScrollPos = EditorGUILayout.BeginScrollView( paletteScrollPos );

                float scrollViewWidth = EditorGUIUtility.currentViewWidth - scrollBarWidth - toolBarIconSize - 20;

                int rowCount = Mathf.CeilToInt( GUIContents.Length / (float)prefabPaletteColumnCount );
                float scrollViewHeight = rowCount * ( ( scrollViewWidth ) / prefabPaletteColumnCount );


                EditorGUI.BeginChangeCheck();

                // ---------------------------------------------
                // Draw Palette SelectionGrid
                // ---------------------------------------------

                int newSelectedPaletteItemIndex = GUILayout.SelectionGrid(
                    Manager.SelectedItemIndex ,
                    GUIContents ,
                    prefabPaletteColumnCount ,
                    "Button" ,
                    GUILayout.Width( (float)scrollViewWidth ) ,
                    GUILayout.Height( scrollViewHeight )
                    );

                // If the user clicked the prefab palette SelectionGrid
                if ( EditorGUI.EndChangeCheck() )
                {
                    // If palette item was deselected by being clicked again, remove the selection
                    if ( newSelectedPaletteItemIndex == Manager.SelectedItemIndex )
                        RemovePrefabSelection();

                    // If palette item selection has changed
                    else
                    {
                        // Process the prefab selection
                        ChangePrefabSelection( newSelectedPaletteItemIndex );
                    }
                }

                GUILayout.BeginHorizontal();

                for ( int i = 0 ; i < GUIContents.Length ; i++ )
                {
                    Selection[ i ] = GUILayout.Toggle( Selection[ i ] , GUIContents[ i ].text );
                }

                GUILayout.EndHorizontal();


                EditorGUILayout.EndScrollView();

                GUILayout.EndHorizontal();

                prefabPaletteColumnCount = (int)GUILayout.HorizontalSlider( prefabPaletteColumnCount , 1 , 10 );

                GUILayout.Space( 20f );

                GUILayout.EndVertical();


            }




            public static void ChangePrefabSelection( int index = Utility.InvalidID )
            {
                if ( index != Utility.InvalidID )
                    Manager.SelectedItemIndex = index;
            }

            public static void RemovePrefabSelection()
            {
                Manager.SelectedItemIndex = Utility.InvalidID;
            }


        }
    }
}
