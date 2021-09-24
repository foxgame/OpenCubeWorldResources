using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Scenes;
using Unity.Entities;

namespace X
{
    namespace UnityMapEditor
    {
        public static class Editor
        {
            static bool button0Donw = false;
            static Vector3Int startFixedPosition = Vector3Int.zero;

            static Transform mapScenesTransform = null;
            static GameObject editorGameObject = null;
            public static Vector3 Position = Vector3.zero;
            public static Vector3Int FixedPosition = Vector3Int.zero;

            public static Dictionary<string , MapRegion> mapRegionDictionary = new Dictionary<string , MapRegion>();
            public static Dictionary<long , GameObject> fixedDictionary = new Dictionary<long , GameObject>();
            public static List<GameObject> gameObjectList = new List<GameObject>();


            static bool painting = false;
            static int undoGroupIndex = 0;

            public static string GameMapRegionName( Vector3Int vector3Int )
            {
                int rx = vector3Int.x / Manager.Setting.RegionSize;
                int ry = vector3Int.y / Manager.Setting.RegionSize;
                int rz = vector3Int.z / Manager.Setting.RegionSize;

                if ( vector3Int.x < 0 && vector3Int.x % Manager.Setting.RegionSize != 0 )
                    rx--;
                if ( vector3Int.y < 0 && vector3Int.y % Manager.Setting.RegionSize != 0 )
                    ry--;
                if ( vector3Int.z < 0 && vector3Int.z % Manager.Setting.RegionSize != 0 )
                    rz--;

                return rx + "_" + ry + "_" + rz;
            }

            public static GameObject GetLayer( Vector3Int vector3Int )
            {
                if ( mapScenesTransform == null )
                {
                    GameObject mapScenes = new GameObject( "MapScenes" );
                    EditorUtility.SetDirty( mapScenes );
                    mapScenesTransform = mapScenes.transform;
                }

                if ( Manager.PrefabEditor )
                {
                    Transform transform = mapScenesTransform.transform.Find( Manager.Layer.ToString() );

                    if ( transform == null )
                    {
                        GameObject obj = new GameObject( Manager.Layer.ToString() );
                        obj.transform.parent = mapScenesTransform.transform;

                        transform = obj.transform;

                        EditorUtility.SetDirty( obj );
                    }

                    return transform.gameObject;
                }
                else
                {
                    GameObject gameObject = GetMapRegionGameObject( vector3Int );

                    if ( gameObject == null )
                    {
                        return null;
                    }

                    Transform transform = gameObject.transform.Find( Manager.Layer.ToString() );

                    if ( transform == null )
                    {
                        GameObject obj = new GameObject( Manager.Layer.ToString() );
                        obj.transform.parent = gameObject.transform;

                        transform = obj.transform;

                        EditorUtility.SetDirty( obj );
                    }

                    return transform.gameObject;
                }
            }

            public static SceneAsset GetSubSceneAsset( Vector3Int vector3Int )
            {
                Scene scene = mapScenesTransform.gameObject.scene;

                string path = scene.path.Replace( ".unity" , "/" );
                string name = GameMapRegionName( vector3Int );
                string file = path + name + ".unity";

                SceneAsset sceneAsset = null;

                if ( !Directory.Exists( path ) )
                {
                    Directory.CreateDirectory( path );
                }

                if ( !File.Exists( path + name + ".unity" ) )
                {
                    File.Copy( "Assets/Resources/MapEditor/EmptyScene.unity" , file );
                }

                scene = EditorSceneManager.NewScene( NewSceneSetup.EmptyScene , NewSceneMode.Additive );

                EditorSceneManager.SaveScene( scene , file );

                sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>( file );

                return sceneAsset;
            }

            public static GameObject GetMapRegionGameObject( Vector3Int vector3Int )
            {
                string name = GameMapRegionName( vector3Int );

                MapRegion mapRegion = null;

                if ( mapRegionDictionary.TryGetValue( name , out mapRegion ) )
                {
                    if ( mapRegion == null )
                    {
                        return null;
                    }
                }

                if ( mapRegion != null )
                {
                    return mapRegion.gameObject;
                }
                else
                {
                    int rx = vector3Int.x / Manager.Setting.RegionSize;
                    int ry = vector3Int.y / Manager.Setting.RegionSize;
                    int rz = vector3Int.z / Manager.Setting.RegionSize;

                    if ( vector3Int.x < 0 && vector3Int.x % Manager.Setting.RegionSize != 0 )
                        rx--;
                    if ( vector3Int.y < 0 && vector3Int.y % Manager.Setting.RegionSize != 0 )
                        ry--;
                    if ( vector3Int.z < 0 && vector3Int.z % Manager.Setting.RegionSize != 0 )
                        rz--;

                    if ( Manager.Dynamic )
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = name;
                        gameObject.transform.parent = mapScenesTransform;

                        mapRegion = gameObject.AddComponent<MapRegion>();
                        mapRegion.Position = new Vector3Int( rx , ry , rz );
                        mapRegion.Size = Manager.Setting.RegionSize;
                        mapRegion.UpdateBounds();

                        mapRegionDictionary.Add( name , mapRegion );

                        return gameObject;
                    }
                    else
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = name;
                        gameObject.transform.parent = mapScenesTransform;

                        SubScene subScene = gameObject.AddComponent<SubScene>();
                        SceneAsset sceneAsset = GetSubSceneAsset( vector3Int ); ;

                        Scene scene = EditorSceneManager.OpenScene( AssetDatabase.GetAssetPath( sceneAsset ) , OpenSceneMode.Additive );
                        scene.isSubScene = true;

                        subScene.SceneAsset = sceneAsset;

                        GameObject gameObject1 = new GameObject();
                        gameObject1.name = "MapScene";

                        mapRegion = gameObject1.AddComponent<MapRegion>();
                        mapRegion.Position = new Vector3Int( rx , ry , rz );
                        mapRegion.Size = Manager.Setting.RegionSize;
                        mapRegion.UpdateBounds();

                        EditorSceneManager.MoveGameObjectToScene( gameObject1 , scene );

                        EditorSceneManager.SaveScene( scene , AssetDatabase.GetAssetPath( sceneAsset ) );

                        mapRegionDictionary.Add( name , mapRegion );

                        return gameObject1;
                    }
                }

                return null;
            }


            public static void StartPainting()
            {
                painting = true;

                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName( "Painting" );
                undoGroupIndex = Undo.GetCurrentGroup();
            }

            public static void StopPainting()
            {
                Undo.CollapseUndoOperations( undoGroupIndex );
                painting = false;
            }

            public static void Clear()
            {
                EditorUtility.SetDirty( editorGameObject );
                GameObject.DestroyImmediate( editorGameObject );
            }

            public static void ClearScene()
            {
                GameObject mapScenes = GameObject.Find( "MapScenes" );

                Undo.DestroyObjectImmediate( mapScenes );

                mapScenes = new GameObject( "MapScenes" );

                EditorUtility.SetDirty( mapScenes );

                mapScenesTransform = mapScenes.transform;

                Reload();
            }

            static bool KeysPressed( Event currentEvent , KeyCode key , bool ctrl , bool shift )
            {
                if ( currentEvent.keyCode == key )
                {
                    if ( ctrl && currentEvent.control )
                        return true;

                    if ( shift && currentEvent.shift )
                        return true;

                    return true;
                }

                return false;
            }

            public static void UpdateHotKey()
            {
                int controlID = GUIUtility.GetControlID( FocusType.Passive );

                if ( Event.current.GetTypeForControl( controlID ) == EventType.KeyDown )
                {                   
                    if ( KeysPressed( Event.current , KeyCode.BackQuote , false , false ) )
                    {
                        Manager.SelectedDrawToolIndex = Utility.InvalidID;
                        Event.current.Use();
                    }
                    if ( KeysPressed( Event.current , KeyCode.Alpha1 , false , false ) )
                    {
                        Manager.SelectedDrawToolIndex = 0;
                        Event.current.Use();
                    }
                    if ( KeysPressed( Event.current , KeyCode.Alpha2 , false , false ) )
                    {
                        Manager.SelectedDrawToolIndex = 1;
                        Event.current.Use();
                    }
                    if ( KeysPressed( Event.current , KeyCode.Alpha3 , false , false ) )
                    {
                        Manager.SelectedDrawToolIndex = 2;
                        Event.current.Use();
                    }
                    if ( KeysPressed( Event.current , KeyCode.Alpha4 , false , false ) )
                    {
                        Manager.SelectedDrawToolIndex = 3;
                        Event.current.Use();
                    }
                    if ( KeysPressed( Event.current , KeyCode.Alpha5 , false , false ) )
                    {
                        Manager.SelectedDrawToolIndex = 4;
                        Event.current.Use();
                    }
                    if ( KeysPressed( Event.current , KeyCode.Q , false , false ) )
                    {
                        Manager.Layer++;
                        Event.current.Use();

                        UpdateLayer();
                        UpdateBorder();
                        UpdateRuler();
                    }
                    if ( KeysPressed( Event.current , KeyCode.E , false , false ) )
                    {
                        Manager.Layer--;
                        Event.current.Use();

                        UpdateLayer();
                        UpdateBorder();
                        UpdateRuler();
                    }

                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                if ( Event.current.GetTypeForControl( controlID ) == EventType.KeyUp )
                {
                    if ( KeysPressed( Event.current , KeyCode.S , false , true ) ||
                        KeysPressed( Event.current , KeyCode.Y , false , true ) ||
                        KeysPressed( Event.current , KeyCode.Z , false , true ) ||
                        KeysPressed( Event.current , KeyCode.V , false , true ) ||
                        KeysPressed( Event.current , KeyCode.Delete , false , false ) )
                    {
                        Reload();
                    }
                }
            }

            public static void UpdateDraw()
            {
                if ( Manager.SelectedDrawToolIndex == Utility.InvalidID )
                {
                    return;
                }

                switch ( Manager.SelectedDrawToolIndex )
                {
                    case 0:
                        {
                            if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 )
                            {
                                StartPainting();

                                button0Donw = true;
                                GUIUtility.hotControl = GUIUtility.GetControlID( FocusType.Passive );
                                Event.current.Use();

                                if ( !Event.current.control )
                                {
                                    AddObject( Position , FixedPosition , Palette.GetSelectObject() , false );
                                }
                            }
                            if ( button0Donw && Manager.SnapToGrid )
                            {
                                if ( !Event.current.control )
                                {
                                    AddObject( Position , FixedPosition , Palette.GetSelectObject() , false );
                                }
                            }
                            if ( Event.current.type == EventType.MouseUp && Event.current.button == 0 )
                            {
                                button0Donw = false;

                                if ( Event.current.control )
                                {
                                    AddObject( Position , FixedPosition , Palette.GetSelectObject() , true );
                                }

                                StopPainting();
                            }
                            

                        }
                        break;
                    case 1:
                        {
                            if ( !Manager.SnapToGrid )
                            {
                                return;
                            }

                            if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 )
                            {
                                StartPainting();

                                GUIUtility.hotControl = GUIUtility.GetControlID( FocusType.Passive );
                                Event.current.Use();

                                startFixedPosition = FixedPosition;
                            }
                            if ( Event.current.type == EventType.MouseUp && Event.current.button == 0 )
                            {
                                int xMin = Mathf.Min( startFixedPosition.x , FixedPosition.x );
                                int xMax = Mathf.Max( startFixedPosition.x , FixedPosition.x );

                                int zMin = Mathf.Min( startFixedPosition.z , FixedPosition.z );
                                int zMax = Mathf.Max( startFixedPosition.z , FixedPosition.z );

                                for ( int i = xMin ; i <= xMax ; i++ )
                                {
                                    for ( int j = zMin ; j <= zMax ; j++ )
                                    {
                                        AddObject( Vector3.zero , new Vector3Int( i , FixedPosition.y , j ) , 
                                            Palette.GetSelectObject() , false );
                                    }
                                }

                                StopPainting();
                            }
                        }
                        break;
                    case 2:
                        {
                            if ( !Manager.SnapToGrid )
                            {
                                return;
                            }

                            if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 )
                            {
                                StartPainting();

                                GUIUtility.hotControl = GUIUtility.GetControlID( FocusType.Passive );
                                Event.current.Use();
                            }
                            if ( Event.current.type == EventType.MouseUp && Event.current.button == 0 )
                            {
                                int rx = FixedPosition.x / Manager.Setting.RegionSize;
                                int rz = FixedPosition.z / Manager.Setting.RegionSize;

                                if ( FixedPosition.x < 0 && FixedPosition.x % Manager.Setting.RegionSize != 0 )
                                    rx--;
                                if ( FixedPosition.z < 0 && FixedPosition.z % Manager.Setting.RegionSize != 0 )
                                    rz--;

                                int xMin = rx * Manager.Setting.RegionSize;
                                int xMax = rx * Manager.Setting.RegionSize + Manager.Setting.RegionSize;

                                int zMin = rz * Manager.Setting.RegionSize;
                                int zMax = rz * Manager.Setting.RegionSize + Manager.Setting.RegionSize;

                                for ( int i = xMin ; i < xMax ; i++ )
                                {
                                    for ( int j = zMin ; j < zMax ; j++ )
                                    {
                                        AddObject( Vector3.zero , new Vector3Int( i , FixedPosition.y , j ) ,
                                            Palette.GetSelectObject() , false );
                                    }
                                }

                                StopPainting();
                            }
                        }
                        break;
                    case 3:
                        {
                            if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 )
                            {
                                StartPainting();

                                button0Donw = true;
                                GUIUtility.hotControl = GUIUtility.GetControlID( FocusType.Passive );
                                Event.current.Use();

                                if ( !Event.current.control )
                                {
                                    AddObject( Position , FixedPosition , Palette.GetRandomObject() , false );
                                }
                            }
                            if ( button0Donw && Manager.SnapToGrid )
                            {
                                if ( !Event.current.control )
                                {
                                    AddObject( Position , FixedPosition , Palette.GetRandomObject() , false );
                                }
                            }
                            if ( Event.current.type == EventType.MouseUp && Event.current.button == 0 )
                            {
                                button0Donw = false;

                                if ( Event.current.control )
                                {
                                    AddObject( Position , FixedPosition , Palette.GetRandomObject() , true );
                                }

                                StopPainting();
                            }

                        }
                        break;
                    case 4:
                        {
                            if ( Event.current.type == EventType.MouseDown && Event.current.button == 0 )
                            {
                                StartPainting();

                                button0Donw = true;
                                GUIUtility.hotControl = GUIUtility.GetControlID( FocusType.Passive );
                                Event.current.Use();

                                if ( !Event.current.control )
                                {
                                    RemoveObject();
                                }
                            }
                            if ( button0Donw && Manager.SnapToGrid )
                            {
                                if ( !Event.current.control )
                                {
                                    RemoveObject();
                                }
                            }
                            if ( Event.current.type == EventType.MouseUp && Event.current.button == 0 )
                            {
                                button0Donw = false;

                                if ( Event.current.control )
                                {
                                    RemoveObject();
                                }

                                StopPainting();
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            public static void UpdateCursor()
            {
                if ( Cursor.lockState != CursorLockMode.Locked &&
                    Manager.SelectedDrawToolIndex != Utility.InvalidID )
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );

                    RaycastHit hit;

                    LayerMask l = 1 << LayerMask.NameToLayer( "MapEditorPlane" );

                    if ( Physics.Raycast( ray , out hit , Mathf.Infinity , l.value ) )
                    {
                        Position = hit.point;

                        FixedPosition.x = (int)Position.x;
                        FixedPosition.z = (int)Position.z;
                        FixedPosition.y = Manager.Layer;

                        if ( Position.x < 0 )
                        {
                            FixedPosition.x--;
                        }
                        if ( Position.z < 0 )
                        {
                            FixedPosition.z--;
                        }

                        if ( Manager.SnapToGrid )
                        {
                            ShowCursor( FixedPosition.x , FixedPosition.z );
                        }
                        else
                        {
                            ShowCursor( Position.x , Position.z );
                        }

                        float CamDist = Vector3.Distance( Camera.current.transform.position , hit.point );

                        Handles.Label( Position + Camera.current.transform.rotation * new Vector3( 0.025f , 0.150f , 0f ) * CamDist , "mouse: " + Position.x.ToString( "f2" ) + " " + Position.y.ToString( "f2" ) + " " + Position.z.ToString( "f2" ) , EditorStyles.whiteLargeLabel );
                        Handles.Label( Position + Camera.current.transform.rotation * new Vector3( 0.025f , 0.130f , 0f ) * CamDist , "grid: " + FixedPosition.x + " " + FixedPosition.y + " " + FixedPosition.z , EditorStyles.whiteLargeLabel );
                    }
                }
                else
                {
                    UnshowCursor();
                }
            }

            public static void ShowCursor( int x , int z )
            {
                Transform cursor = editorGameObject.transform.Find( "Plane/Cursor" );

                cursor.transform.position = new Vector3( x + 0.5f , Manager.Layer , z + 0.5f );
                cursor.gameObject.SetActive( true );
            }

            public static void ShowCursor( float x , float z )
            {
                Transform cursor = editorGameObject.transform.Find( "Plane/Cursor" );

                cursor.transform.position = new Vector3( x , Manager.Layer , z );
                cursor.gameObject.SetActive( true );
            }

            public static void UnshowCursor()
            {
                Transform cursor = editorGameObject.transform.Find( "Plane/Cursor" );
                cursor.gameObject.SetActive( false );
            }

            public static void UpdateLayer()
            {
                Transform plane = editorGameObject.transform.Find( "Plane/Plane" );
                plane.transform.position = new Vector3( 0f , Manager.Layer , 0f );

                Transform Text = editorGameObject.transform.Find( "Plane/Text" );
                Text.transform.position = new Vector3( 0f , Manager.Layer , 0f );

                TextMeshPro textMeshPro = Text.GetComponent<TextMeshPro>();
                textMeshPro.SetText( Manager.Layer.ToString() );
            }

            public static void UpdatePlane()
            {
                Transform plane = editorGameObject.transform.Find( "Plane/Plane" );
                plane.localScale = new Vector3( Manager.Setting.MaxMapSize , Manager.Setting.MaxMapSize , 0f );
            }

            public static void UpdateRuler()
            {
                Transform x = editorGameObject.transform.Find( "Ruler/X" );
                Transform z = editorGameObject.transform.Find( "Ruler/Z" );

                LineRenderer lineRendererX = x.GetComponent<LineRenderer>();
                LineRenderer lineRendererZ = z.GetComponent<LineRenderer>();

                lineRendererX.SetPosition( 0 , new Vector3( -Manager.Setting.MaxMapHeight , Manager.Layer + 0.01f , 0f ) );
                lineRendererX.SetPosition( 1 , new Vector3( Manager.Setting.MaxMapHeight , Manager.Layer + 0.01f , 0f ) );

                lineRendererZ.SetPosition( 0 , new Vector3( 0f , Manager.Layer + 0.01f , -Manager.Setting.MaxMapHeight ) );
                lineRendererZ.SetPosition( 1 , new Vector3( 0f , Manager.Layer + 0.01f , Manager.Setting.MaxMapHeight ) );
            }

            public static void UpdateBorder()
            {
                Transform border = editorGameObject.transform.Find( "Plane/Border" );

                LineRenderer lineRenderer = border.GetComponent<LineRenderer>();

                List<Vector3> list = new List<Vector3>();

                int w = Manager.Setting.MaxMapSize / 2;
                int h = Manager.Setting.MaxMapSize / 2;

                int wStart = -w;
                int hStart = -h;

                list.Add( new Vector3( w , Manager.Layer , -h ) );

                for ( int i = -h ; i <= h ; i++ )
                {
                    list.Add( new Vector3( wStart , Manager.Layer , i ) );

                    if ( i != h )
                    {
                        list.Add( new Vector3( wStart , Manager.Layer , i + 1 ) );
                    }

                    wStart = -wStart;
                }

                list.Add( new Vector3( -w , Manager.Layer , -h ) );
                list.Add( new Vector3( w , Manager.Layer , -h ) );

                for ( int i = -w ; i <= w ; i++ )
                {
                    list.Add( new Vector3( i , Manager.Layer , hStart ) );

                    if ( i != w )
                    {
                        list.Add( new Vector3( i + 1 , Manager.Layer , hStart ) );
                    }

                    hStart = -hStart;
                }

                lineRenderer.positionCount = list.Count;
                lineRenderer.SetPositions( list.ToArray() );
            }

            public static void ShowBoder( bool b )
            {
                Transform border = editorGameObject.transform.Find( "Plane" );
                border.gameObject.SetActive( b );
            }

            public static void AddObject( Vector3 position , Vector3Int fixedPosition ,
                UnityMapEditor.Object editorObject , bool checkUp )
            {
                if ( editorObject == null )
                {
                    return;
                }

                long index = FixedVector3.GetIndex( fixedPosition );

                if ( Manager.SnapToGrid )
                {
                    GameObject gameObject1 = null;

                    if ( checkUp )
                    {
                        while ( fixedDictionary.TryGetValue( index , out gameObject1 ) )
                        {
                            fixedPosition.y++;
                            index = FixedVector3.GetIndex( fixedPosition );
                        }
                    }
                    else
                    {
                        if ( fixedDictionary.TryGetValue( index , out gameObject1 ) )
                        {
                            return;
                        }
                    }
                }


                if ( editorObject.ObjectConfig.Type == "Template" )
                {
                    MapEditorObject[] mapEditorObjects = editorObject.GameObject.GetComponentsInChildren<MapEditorObject>();

                    for ( int i = 0 ; i < mapEditorObjects.Length ; i++ )
                    {
                        MapEditorObject mapEditorObject = mapEditorObjects[ i ];

                        Object obj = null;

                        if ( Manager.ObjectDictionary.TryGetValue( mapEditorObject.MapObjectData.ObjectID , out obj ) )
                        {
                            GameObject gameObject = null;

                            if ( Manager.SnapToGrid )
                            {
                                gameObject = AddObject( position , fixedPosition + mapEditorObject.MapObjectData.FixedPosition.Value , obj );
                            }
                            else
                            {
                                gameObject = AddObject( position + mapEditorObject.transform.localPosition , fixedPosition , obj );
                            }


                            if ( gameObject != null )
                            {
                                gameObject.transform.localEulerAngles = mapEditorObject.transform.localEulerAngles;
                                gameObject.transform.localScale = mapEditorObject.transform.localScale;
                            }
                        }
                    }
                }
                else
                {
                    AddObject( position , fixedPosition , editorObject );
                }
            }

            public static GameObject AddObject( Vector3 position , Vector3Int fixedPosition , UnityMapEditor.Object editorObject )
            {
                GameObject layer = GetLayer( fixedPosition );

                if ( layer == null )
                {
                    return null;
                }

                long index = FixedVector3.GetIndex( fixedPosition );

                GameObject gameObject = null;

                if ( fixedDictionary.TryGetValue( index , out gameObject ) )
                {
                    return null;
                }

                gameObject = (GameObject)PrefabUtility.InstantiatePrefab( editorObject.GameObject , layer.transform );
                gameObject.name = editorObject.ObjectConfig.ObjectID;

                if ( editorObject.Collider != null )
                {
                    GameObject objCollider = (GameObject)PrefabUtility.InstantiatePrefab( editorObject.Collider , gameObject.transform );
                    objCollider.name = "Collider";
                }


                MapEditorObject mapEditorObject = gameObject.AddComponent<MapEditorObject>();
                MapObjectData mapObjectData = mapEditorObject.MapObjectData;

                mapObjectData.ObjectID = editorObject.ObjectConfig.ObjectID;
                mapObjectData.FixedPosition.Value = Manager.SnapToGrid ? fixedPosition : Vector3Int.zero;
                mapObjectData.Position = Manager.SnapToGrid ? Vector3.zero : position;
                mapObjectData.IsFixed = Manager.SnapToGrid;
                mapObjectData.VertexColor = Color.white;
                mapObjectData.UserTag = "";

                if ( Manager.SnapToGrid )
                {
                    gameObject.transform.localPosition = new Vector3( fixedPosition.x + 0.5f ,
                            fixedPosition.y ,
                            fixedPosition.z + 0.5f );

                    fixedDictionary.Add( index , gameObject );
                }
                else
                {
                    gameObject.transform.localPosition = position;
                }

                Undo.RegisterCreatedObjectUndo( gameObject , "Create" );

                EditorUtility.SetDirty( gameObject );

                gameObjectList.Add( gameObject );

                if ( Event.current.alt )
                {
                    Selection.activeGameObject = gameObject;
                }

                return gameObject;
            }

            public static void RemoveObject()
            {
                if ( Manager.SnapToGrid )
                {
                    long index = FixedVector3.GetIndex( FixedPosition );

                    GameObject gameObject = null;

                    if ( fixedDictionary.TryGetValue( index , out gameObject ) )
                    {
                        Undo.RecordObject( gameObject , "Delete" );

                        fixedDictionary.Remove( index );
                        gameObjectList.Remove( gameObject );

                        Undo.DestroyObjectImmediate( gameObject );
                    }
                }
                else
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );

                    RaycastHit hit;

                    LayerMask l = 1 << LayerMask.NameToLayer( "MapObject" );

                    if ( Physics.Raycast( ray , out hit , Mathf.Infinity , l.value ) )
                    {
                        if ( hit.collider.gameObject != null )
                        {
                            GameObject gameObject = hit.collider.transform.parent.gameObject;

                            MapEditorObject mapEditorObject = gameObject.GetComponent<MapEditorObject>();

                            if ( mapEditorObject != null )
                            {
                                gameObjectList.Remove( gameObject );
                                Undo.DestroyObjectImmediate( gameObject );
                            }
                        }
                    }
                }
            }

            public static void Reload()
            {
                fixedDictionary.Clear();
                gameObjectList.Clear();

                MapEditorObject[ ] mapEditorObjects = mapScenesTransform.GetComponentsInChildren<MapEditorObject>();
                for ( int i = 0 ; i < mapEditorObjects.Length ; i++ )
                {
                    MapEditorObject mapEditorObject = mapEditorObjects[ i ];

                    if ( mapEditorObject.MapObjectData.IsFixed )
                    {
                        FixedVector3 fixedPosition = mapEditorObject.MapObjectData.FixedPosition;

                        long index = FixedVector3.GetIndex( fixedPosition.x , fixedPosition.y , fixedPosition.z );

                        if ( fixedDictionary.ContainsKey( index ) )
                        {
                            Debug.LogWarning( "Index Contains " + fixedPosition.Value );
                        }
                        else
                        {
                            fixedDictionary.Add( index , mapEditorObject.gameObject );
                        }
                    }

                    gameObjectList.Add( mapEditorObject.gameObject );
                }

                mapRegionDictionary.Clear();

                SubScene[ ] subScenes = mapScenesTransform.GetComponentsInChildren<SubScene>();

                for ( int i = 0 ; i < subScenes.Length ; i++ )
                {
                    SubScene subScene = subScenes[ i ];

                    Scene scene = EditorSceneManager.OpenScene( AssetDatabase.GetAssetPath( subScene.SceneAsset ) , OpenSceneMode.Additive );
                    scene.isSubScene = true;

                    MapRegion mapRegion = scene.GetRootGameObjects()[ 0 ].GetComponent<MapRegion>();

                    string name = mapRegion.Position.x + "_" + mapRegion.Position.y + "_" + mapRegion.Position.z;

                    if ( mapRegionDictionary.ContainsKey( name ) )
                    {
                        Debug.LogWarning( "Region Contains " + name );
                    }
                    else
                    {
                        mapRegionDictionary.Add( name , mapRegion );
                    }



                    mapEditorObjects = mapRegion.GetComponentsInChildren<MapEditorObject>();
                    for ( int j = 0 ; j < mapEditorObjects.Length ; j++ )
                    {
                        MapEditorObject mapEditorObject = mapEditorObjects[ j ];

                        if ( mapEditorObject.MapObjectData.IsFixed )
                        {
                            FixedVector3 fixedPosition = mapEditorObject.MapObjectData.FixedPosition;

                            long index = FixedVector3.GetIndex( fixedPosition.x , fixedPosition.y , fixedPosition.z );

                            if ( fixedDictionary.ContainsKey( index ) )
                            {
                                Debug.LogWarning( "Index Contains " + fixedPosition.Value );
                            }
                            else
                            {
                                fixedDictionary.Add( index , mapEditorObject.gameObject );
                            }
                        }

                        gameObjectList.Add( mapEditorObject.gameObject );
                    }
                }

                MapRegion[ ] mapRegions = mapScenesTransform.GetComponentsInChildren<MapRegion>();
                for ( int i = 0 ; i < mapRegions.Length ; i++ )
                {
                    MapRegion mapRegion = mapRegions[ i ];

                    string name = mapRegion.Position.x + "_" + mapRegion.Position.y + "_" + mapRegion.Position.z;

                    if ( mapRegionDictionary.ContainsKey( name ) )
                    {
                        Debug.LogWarning( "Region Contains " + name );
                    }
                    else
                    {
                        mapRegionDictionary.Add( name , mapRegion );
                    }
                }

                Debug.Log( "Editor Reload." );
                Debug.Log( "Region Dictionary Count = " + mapRegionDictionary.Count );
                Debug.Log( "Fixed Dictionary Count = " + fixedDictionary.Count );
                Debug.Log( "GameObject Count = " + gameObjectList.Count );
            }

            public static void Init()
            {
                Manager.PrefabEditor = EditorSceneManager.GetActiveScene().name == "UnityPrefabEditor";

                if ( mapScenesTransform == null )
                {
                    GameObject mapScenes = GameObject.Find( "MapScenes" );

                    if ( mapScenes == null )
                    {
                        mapScenes = new GameObject( "MapScenes" );
                    }

                    EditorUtility.SetDirty( mapScenes );

                    mapScenesTransform = mapScenes.transform;
                }

                EditorSceneManager.SetActiveScene( mapScenesTransform.gameObject.scene );

                editorGameObject = GameObject.Find( "MapEditor" );

                if ( editorGameObject == null )
                {
                    GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>( "Assets/Resources/MapEditor/MapEditor.prefab" );
                    editorGameObject = GameObject.Instantiate( gameObject );
                    editorGameObject.name = "MapEditor";
                }

                Reload();

                UpdateLayer();
                UpdateRuler();
                UpdatePlane();
                UpdateBorder();
            }


        }
    }
}
