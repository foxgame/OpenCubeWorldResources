using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;


namespace X
{
    namespace UnityMapEditor
    {
        [CanEditMultipleObjects]
        [CustomEditor( typeof( MapEditorObject ) )]
        public class MapEditorObjectEditor : UnityEditor.Editor
        {
            float time = 0f;

            void RefreshFixedPosition()
            {
                foreach ( UnityEngine.Object obj in targets )
                {
                    MapEditorObject mapEditorObject = obj as MapEditorObject;

                    Vector3 position = mapEditorObject.gameObject.transform.position;

                    if ( mapEditorObject.MapObjectData.IsFixed )
                    {
                        Vector3Int vector3Int = Vector3Int.zero;
                        vector3Int.x = Mathf.RoundToInt( position.x - 0.5f );
                        vector3Int.y = (int)position.y;
                        vector3Int.z = Mathf.RoundToInt( position.z - 0.5f );

                        long oldIndex = FixedVector3.GetIndex( mapEditorObject.MapObjectData.FixedPosition.Value );
                        long index = FixedVector3.GetIndex( vector3Int );

                        if ( Editor.fixedDictionary != null )
                        {
                            if ( Editor.fixedDictionary.ContainsKey( index ) )
                            {
                                vector3Int = mapEditorObject.MapObjectData.FixedPosition.Value;
                                mapEditorObject.gameObject.transform.localPosition = new Vector3( vector3Int.x + 0.5f ,
                                    vector3Int.y ,
                                    vector3Int.z + 0.5f );
                            }
                            else
                            {
                                mapEditorObject.MapObjectData.FixedPosition.Value = vector3Int;
                                mapEditorObject.gameObject.transform.localPosition = new Vector3( vector3Int.x + 0.5f ,
                                    vector3Int.y ,
                                    vector3Int.z + 0.5f );

                                Editor.fixedDictionary.Remove( oldIndex );
                                Editor.fixedDictionary.Add( index , mapEditorObject.gameObject.transform.gameObject );
                            }
                        }
                        else
                        {
                            Debug.LogWarning( "Open MapEditor Window." );
                        }
                    }
                    else
                    {

                    }
                }
            }

            public override void OnInspectorGUI()
            {
                MapEditorObject mapEditorObject = target as MapEditorObject;

                GUILayout.BeginVertical();

                EditorGUILayout.TextField( "ObjectID: " , mapEditorObject.MapObjectData.ObjectID );
                string userTag = EditorGUILayout.TextField( "UserTag" , mapEditorObject.MapObjectData.UserTag );

                if ( userTag != mapEditorObject.MapObjectData.UserTag )
                {
                    foreach ( UnityEngine.Object obj in targets )
                    {
                        MapEditorObject mapEditorObject1 = obj as MapEditorObject;
                        mapEditorObject1.MapObjectData.UserTag = userTag;
                    }
                }

                bool isFixed = EditorGUILayout.Toggle( "IsFixed" , mapEditorObject.MapObjectData.IsFixed );

                if ( isFixed != mapEditorObject.MapObjectData.IsFixed )
                {
                    foreach ( UnityEngine.Object obj in targets )
                    {
                        MapEditorObject mapEditorObject1 = obj as MapEditorObject;

                        Vector3 position = mapEditorObject1.gameObject.transform.position;

                        Vector3Int vector3Int = Vector3Int.zero;
                        vector3Int.x = Mathf.RoundToInt( position.x - 0.5f );
                        vector3Int.y = (int)position.y;
                        vector3Int.z = Mathf.RoundToInt( position.z - 0.5f );

                        long index = FixedVector3.GetIndex( vector3Int );

                        if ( Editor.fixedDictionary != null )
                        {
                            if ( isFixed )
                            {
                                if ( Editor.fixedDictionary.ContainsKey( index ) )
                                {
                                    Debug.LogWarning( "Index Contains " + vector3Int );
                                }
                                else
                                {
                                    if ( mapEditorObject1.MapObjectData.IsFixed )
                                    {
                                        long oldIndex = FixedVector3.GetIndex( mapEditorObject1.MapObjectData.FixedPosition.Value );
                                        Editor.fixedDictionary.Remove( oldIndex );
                                    }

                                    mapEditorObject1.MapObjectData.IsFixed = isFixed;

                                    mapEditorObject1.MapObjectData.FixedPosition.Value = vector3Int;
                                    mapEditorObject1.gameObject.transform.localPosition = new Vector3( vector3Int.x + 0.5f ,
                                        vector3Int.y ,
                                        vector3Int.z + 0.5f );

                                    Editor.fixedDictionary.Add( index , mapEditorObject1.gameObject.transform.gameObject );
                                }
                            }
                            else
                            {
                                if ( mapEditorObject1.MapObjectData.IsFixed )
                                {
                                    long oldIndex = FixedVector3.GetIndex( mapEditorObject1.MapObjectData.FixedPosition.Value );
                                    Editor.fixedDictionary.Remove( oldIndex );
                                }

                                mapEditorObject1.MapObjectData.IsFixed = isFixed;

                                mapEditorObject1.MapObjectData.Position = position;
                            }
                        }
                        else
                        {
                            Debug.LogWarning( "Open MapEditor Window." );
                        }
                    }

                    
                }

                if ( isFixed )
                {
                    Vector3Int vector3Int = EditorGUILayout.Vector3IntField( "FixedPosition" , mapEditorObject.MapObjectData.FixedPosition.Value );
                    if ( vector3Int != mapEditorObject.MapObjectData.FixedPosition.Value )
                    {
                        long oldIndex = FixedVector3.GetIndex( mapEditorObject.MapObjectData.FixedPosition.Value );
                        long index = FixedVector3.GetIndex( vector3Int );

                        if ( Editor.fixedDictionary != null )
                        {
                            if ( Editor.fixedDictionary.ContainsKey( index ) )
                            {
                                Debug.LogWarning( "Index Contains " + vector3Int );
                            }
                            else
                            {
                                mapEditorObject.MapObjectData.FixedPosition.Value = vector3Int;
                                mapEditorObject.gameObject.transform.localPosition = new Vector3( vector3Int.x + 0.5f ,
                                    vector3Int.y ,
                                    vector3Int.z + 0.5f );

                                Editor.fixedDictionary.Remove( oldIndex );
                                Editor.fixedDictionary.Add( index , mapEditorObject.gameObject.transform.gameObject );
                            }
                        }
                        else
                        {
                            Debug.LogWarning( "Open MapEditor Window." );
                        }
                    }
                }
                else
                {
                    Vector3 vector3 = EditorGUILayout.Vector3Field( "Position" , mapEditorObject.MapObjectData.Position );
                    if ( vector3 != mapEditorObject.MapObjectData.Position )
                    {
                        mapEditorObject.MapObjectData.Position = vector3;
                        mapEditorObject.gameObject.transform.position = vector3;
                    }
                }

                if ( GUILayout.Button( "Refresh FixedPosition" ) )
                {
                    RefreshFixedPosition();
                }

                GUILayout.EndVertical();

            }

        }


    }

}
