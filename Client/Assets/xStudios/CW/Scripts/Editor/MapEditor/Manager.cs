using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace X
{
    namespace UnityMapEditor
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        [System.Serializable]
        public class ObjectConfig
        {
            public string ObjectID = "";
            public string Name = "";
            public string Path = "";
            public string Type = "";
            public string Group = "";
            public string Icon = "";
            public string Collider = "";
            public string Obj = "";
        }

        public class Object
        {
            public ObjectConfig ObjectConfig = null;
            public GameObject GameObject = null;
            public GameObject Collider = null;
            public Texture2D Icon = null;
            public GUIContent GUIContent = null;
        }

        public static class Manager
        {
            public static bool PrefabEditor = false;

            public static bool Dynamic = false;
            public static int Layer = 0;

            public static float BuildIconScale = 1.1f;
            public static bool SnapToGrid = true;
            public static bool GridExists = true;
            public static Axis RotateAxis = Axis.Y;
            public static Axis FlipAxis = Axis.X;

            public static int SelectedDrawToolIndex = 0;
            public static int SelectedItemIndex = 0;
            public static int SelectedObjectTypeIndex = 0;
            public static int SelectedObjectGroupIndex = 0;

            public static MapEditorSetting Setting;

            public static List<Object> ObjectList = new List<Object>();
            public static Dictionary<string , Object> ObjectDictionary = new Dictionary<string , Object>();

            public static string[ ] ObjectTypes = null;
            public static string[ ] ObjectGroups = null;

            

            public static GUIContent[] GetGUIContents()
            {
                List<GUIContent> list = new List<GUIContent>();

                for ( int i = 0 ; i < ObjectList.Count ; i++ )
                {
                    Object obj = ObjectList[ i ];

                    if ( SelectedObjectTypeIndex != 0 )
                    {
                        if ( obj.ObjectConfig.Type != Setting.ObjectTypeList[ SelectedObjectTypeIndex ] )
                        {
                            continue;
                        }
                    }

                    if ( SelectedObjectGroupIndex != 0 )
                    {
                        if ( obj.ObjectConfig.Group != Setting.ObjectGroupList[ SelectedObjectGroupIndex ] )
                        {
                            continue;
                        }
                    }

                    list.Add( obj.GUIContent );
                }

                return list.ToArray();
            }

            public static void Load()
            {
                Setting = AssetDatabase.LoadAssetAtPath<MapEditorSetting>( "Assets/Resources/MapEditor/Setting.asset" );
                ObjectList.Clear();
                ObjectDictionary.Clear();
                ObjectTypes = Setting.ObjectTypeList.ToArray();
                ObjectGroups = Setting.ObjectGroupList.ToArray();

                for ( int i = 0 ; i < Setting.ObjectConfigLsit.Count ; i++ )
                {
                    ObjectConfig objectConfig = Setting.ObjectConfigLsit[ i ];

                    Object obj = new Object();
                    obj.ObjectConfig = objectConfig;
                    obj.GameObject = AssetDatabase.LoadAssetAtPath<GameObject>( "Assets/Mods/" + objectConfig.Path + objectConfig.Obj + ".prefab" );
                    obj.Collider = AssetDatabase.LoadAssetAtPath<GameObject>( "Assets/Mods/" + objectConfig.Path + objectConfig.Collider + ".prefab" );
                    obj.Icon = AssetDatabase.LoadAssetAtPath<Texture2D>( "Assets/Mods/" + objectConfig.Path + objectConfig.Icon );
                    obj.GUIContent = new GUIContent( objectConfig.Name , obj.Icon );

                    ObjectList.Add( obj );

                    if ( ObjectDictionary.ContainsKey( objectConfig.ObjectID ) )
                    {
                        Debug.LogWarning( "ObjectID Contains " + objectConfig.ObjectID );
                    }
                    else
                    {
                        ObjectDictionary.Add( objectConfig.ObjectID , obj );
                    }
                }
                
            }
        }


    }
}
