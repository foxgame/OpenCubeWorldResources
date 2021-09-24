using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class HumanoidMesh : MonoBehaviour
    {
        [SerializeField]
        HumanoidBones bones;

        [SerializeField]
        HumanoidParts parts;

        List<GameObject>[] meshList = null;

        public HumanoidBones Bones { get { return bones; } }
        public HumanoidParts Parts { get { return parts; } }


        public List<GameObject>[] MeshList
        {
            get
            {
                return meshList;
            }
        }

        private void Awake()
        {
            parts.Init();

            meshList = new List<GameObject>[ (int)HumanoidPartType.Count ];
            for ( int i = 0 ; i < (int)HumanoidPartType.Count ; i++ )
            {
                meshList[ i ] = new List<GameObject>();
            }
        }

        private void Start()
        {
        }

        public void Clear()
        {
            for ( int i = 0 ; i < meshList.Length ; i++ )
            {
                List<GameObject> list = meshList[ i ];

                for ( int j = 0 ; j < list.Count ; j++ )
                {
                    GameObject gameObject = list[ j ];

                    Destroy( gameObject );
                    gameObject.transform.parent = null;
                }

                list.Clear();
            }
        }

        public string Save( string path )
        {
            string log = "";

            FileStream fs = null;
            BinaryWriter bf = null;

            try
            {
                fs = File.Open( path , FileMode.Create , FileAccess.ReadWrite , FileShare.ReadWrite );
                bf = new BinaryWriter( fs );

                bf.Write( "XHMS" );
                bf.Write( 1 );

                bf.Write( meshList.Length );

                for ( int i = 0 ; i < meshList.Length ; i++ )
                {
                    List<GameObject> list = meshList[ i ];

                    bf.Write( i );
                    bf.Write( list.Count );

                    for ( int j = 0 ; j < list.Count ; j++ )
                    {
                        GameObject obj = list[ j ];

                        HumanoidPart humanoidPart = obj.GetComponent<HumanoidPart>();

                        bf.Write( obj.transform.localPosition.x );
                        bf.Write( obj.transform.localPosition.y );
                        bf.Write( obj.transform.localPosition.z );
                        bf.Write( obj.transform.localEulerAngles.x );
                        bf.Write( obj.transform.localEulerAngles.y );
                        bf.Write( obj.transform.localEulerAngles.z );
                        bf.Write( obj.transform.localScale.x );
                        bf.Write( obj.transform.localScale.y );
                        bf.Write( obj.transform.localScale.z );

                        bf.Write( humanoidPart.color.r );
                        bf.Write( humanoidPart.color.g );
                        bf.Write( humanoidPart.color.b );
                        bf.Write( humanoidPart.color.a );

                        bf.Write( humanoidPart.enable );
                        bf.Write( (int)humanoidPart.shapeType );
                    }
                }
            }
            catch ( System.Exception e )
            {
                log = e.Message;
            }
            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                    fs.Dispose();
                }
                if ( bf != null )
                {
                    bf.Close();
                    bf.Dispose();
                }
            }

            return log;
        }

        public string Load( string path )
        {
            Clear();

            string log = "";

            FileStream fs = null;
            BinaryReader br = null;

            try
            {
                fs = File.Open( path , FileMode.Open , FileAccess.ReadWrite , FileShare.ReadWrite );
                br = new BinaryReader( fs );
                string magic = br.ReadString();
                int version = br.ReadInt32();

                int length = br.ReadInt32();

                for ( int i = 0 ; i < length ; i++ )
                {
                    HumanoidPartType partType = (HumanoidPartType)br.ReadInt32();
                    int count = br.ReadInt32();

                    for ( int j = 0 ; j < count ; j++ )
                    {
                        Vector3 localPosition = Vector3.zero;
                        Vector3 localEulerAngles = Vector3.zero;
                        Vector3 localScale = Vector3.zero;
                        Color color = Color.white;

                        localPosition.x = br.ReadSingle();
                        localPosition.y = br.ReadSingle();
                        localPosition.z = br.ReadSingle();
                        localEulerAngles.x = br.ReadSingle();
                        localEulerAngles.y = br.ReadSingle();
                        localEulerAngles.z = br.ReadSingle();
                        localScale.x = br.ReadSingle();
                        localScale.y = br.ReadSingle();
                        localScale.z = br.ReadSingle();

                        color.r = br.ReadSingle();
                        color.g = br.ReadSingle();
                        color.b = br.ReadSingle();
                        color.a = br.ReadSingle();

                        bool enable = br.ReadBoolean();
                        HumanoidShapeType shapeType = (HumanoidShapeType)br.ReadInt32();

                        GameObject obj = null;

                        if ( j == 0 )
                        {
                            obj = CreateDefault( partType );
                        }
                        else
                        {
                            obj = CreateDefaultPart( partType );
                        }

                        HumanoidPart humanoidPart = obj.GetComponent<HumanoidPart>();

                        if ( humanoidPart.shapeType != shapeType )
                        {
                            obj = ChangeShape( shapeType , obj );
                        }

                        SetColor( obj , color );

                        humanoidPart.enable = enable;

                        obj.transform.localPosition = localPosition;
                        obj.transform.localEulerAngles = localEulerAngles;
                        obj.transform.localScale = localScale;

                        obj.SetActive( enable );
                    }
                }
                
            }
            catch ( System.Exception e )
            {
                log = e.Message;
            }
            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                    fs.Dispose();
                }
                if ( br != null )
                {
                    br.Close();
                    br.Dispose();
                }
            }

            Collider[] colliders = GetComponentsInChildren<Collider>();
            for ( int i = 0 ; i < colliders.Length ; i++ )
            {
                colliders[ i ].enabled = false;
                colliders[ i ].enabled = true;
            }

            return log;
        }

        public void SetColor( GameObject gameObject , Color color )
        {
            if ( gameObject == null )
            {
                return;
            }

            HumanoidPart humanoidPart = gameObject.GetComponent<HumanoidPart>();
            humanoidPart.color = color;

            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            Color[] colors = new Color[ mesh.vertexCount ];
            for ( int i = 0 ; i < colors.Length ; i++ )
            {
                colors[ i ] = color;
            }

            mesh.colors = colors;
        }

        public GameObject CreateDefault( HumanoidPartType partType )
        {
            HumanoidParts.DefaultData data = HumanoidManager.Instance.Data[ (int)partType ];
            Transform parent = parts.Parts[ (int)partType ];

            string path = Utility.AssetsPath + Utility.ModsPath + Utility.XPath + Utility.CommonPath + 
                Utility.PlayersPath + Utility.PartsPath + data.shapeType + Utility.PrefabExtension;

            Asset asset = AssetManager.Instance.LoadSingle( path );
            GameObject gameObject = asset.GetAsset( data.shapeType.ToString() ) as GameObject;

            GameObject obj = Instantiate( gameObject , parent );
            obj.transform.localScale = data.scale;
            obj.transform.localEulerAngles = data.rotation;
            obj.transform.localPosition = data.position;
            obj.name = partType.ToString();

            HumanoidPart humanoidPart = obj.GetComponent<HumanoidPart>();
            humanoidPart.partType = partType;
            humanoidPart.color = data.color;
            humanoidPart.shapeType = data.shapeType;
            humanoidPart.isPart = false;
            humanoidPart.enable = true;

            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            Color[] colors = new Color[ mesh.vertexCount ];
            for ( int i = 0 ; i < colors.Length ; i++ )
            {
                colors[ i ] = data.color;
            }

            mesh.colors = colors;

            Material material = MaterialManager.Instance.CreateMaterialPlayerPart( "PlayerPart" , Color.white );

            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = material;

            meshList[ (int)partType ].Add( obj );

            Collider collider = obj.GetComponent<Collider>();
            collider.enabled = true;

            return obj;
        }

        public GameObject ChangeShape( HumanoidShapeType shapeType , GameObject gameObject )
        {
            HumanoidPart humanoidPart = gameObject.GetComponent<HumanoidPart>();
            Color color = humanoidPart.color;
            HumanoidPartType partType = humanoidPart.partType;
            bool isPart = humanoidPart.isPart;
            bool enable = humanoidPart.enable;

            int index = meshList[ (int)partType ].IndexOf( gameObject );

            string path = Utility.AssetsPath + Utility.ModsPath + Utility.XPath + Utility.CommonPath + 
                Utility.PlayersPath + Utility.PartsPath + shapeType + Utility.PrefabExtension;

            Asset asset = AssetManager.Instance.LoadSingle( path );
            GameObject gameObject1 = asset.GetAsset( shapeType.ToString() ) as GameObject;

            Vector3 localPosition = gameObject.transform.localPosition;
            Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
            Vector3 localScale = gameObject.transform.localScale;
            string name = gameObject.name;

            Transform parent = gameObject.transform.parent;

            Destroy( gameObject );
            gameObject.transform.parent = null;

            gameObject = Instantiate( gameObject1 , parent );
            gameObject.transform.localScale = localScale;
            gameObject.transform.localEulerAngles = localEulerAngles;
            gameObject.transform.localPosition = localPosition;
            gameObject.name = name;

            humanoidPart = gameObject.GetComponent<HumanoidPart>();
            humanoidPart.partType = partType;
            humanoidPart.color = color;
            humanoidPart.shapeType = shapeType;
            humanoidPart.isPart = isPart;
            humanoidPart.enable = enable;

            MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            Color[] colors = new Color[ mesh.vertexCount ];
            for ( int i = 0 ; i < colors.Length ; i++ )
            {
                colors[ i ] = humanoidPart.color;
            }

            mesh.colors = colors;

            Material material = MaterialManager.Instance.CreateMaterialPlayerPart( "PlayerPart" , Color.white );

            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = material;

            meshList[ (int)humanoidPart.partType ][ index ] = gameObject;

            Collider collider = gameObject.GetComponent<Collider>();
            collider.enabled = true;

            return gameObject;
        }

        public GameObject CreateDefaultPart( HumanoidPartType partType )
        {
            HumanoidParts.DefaultData data = HumanoidManager.Instance.Data[ (int)partType ];
            Transform parent = parts.Parts[ (int)partType ];

            if ( meshList[ (int)partType ].Count >= data.maxParts )
            {
                return null;
            }

            string path = Utility.AssetsPath + Utility.ModsPath + Utility.XPath + Utility.CommonPath +
                Utility.PlayersPath + Utility.PartsPath + data.shapeType + Utility.PrefabExtension;

            Asset asset = AssetManager.Instance.LoadSingle( path );
            GameObject gameObject = asset.GetAsset( data.shapeType.ToString() ) as GameObject;

            GameObject obj = Instantiate( gameObject , parent );
            obj.transform.localScale = data.partScale;
            obj.transform.localEulerAngles = data.partRotation;
            obj.transform.localPosition = data.partPosition;
            obj.name = partType.ToString() + meshList[ (int)partType ].Count;

            HumanoidPart humanoidPart = obj.GetComponent<HumanoidPart>();
            humanoidPart.partType = partType;
            humanoidPart.color = data.partColor;
            humanoidPart.shapeType = data.partShapeType;
            humanoidPart.isPart = true;
            humanoidPart.enable = true;

            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;

            Color[] colors = new Color[ mesh.vertexCount ];
            for ( int i = 0 ; i < colors.Length ; i++ )
            {
                colors[ i ] = data.partColor;
            }

            mesh.colors = colors;

            Material material = MaterialManager.Instance.CreateMaterialPlayerPart( "PlayerPart" , Color.white );

            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.material = material;

            meshList[ (int)partType ].Add( obj );

            Collider collider = obj.GetComponent<Collider>();
            collider.enabled = true;

            return obj;
        }

        public void RemoveDefaultPart( GameObject gameObject )
        {
            HumanoidPart humanoidPart = gameObject.GetComponent<HumanoidPart>();
            HumanoidPartType partType = humanoidPart.partType;

            int index = meshList[ (int)partType ].IndexOf( gameObject );

            if ( index == 0 )
            {
                return;
            }

            Destroy( gameObject );
            gameObject.transform.parent = null;

            meshList[ (int)partType ].RemoveAt( index );
        }

        public void CreateDefault()
        {

            CreateDefault( HumanoidPartType.Body );
//             CreateDefault( HumanoidPartType.Chests );
            CreateDefault( HumanoidPartType.Head );
            CreateDefault( HumanoidPartType.Mouse );
//             CreateDefault( HumanoidPartType.Moustache );
            CreateDefault( HumanoidPartType.Nose );
            CreateDefault( HumanoidPartType.LeftEye );
            CreateDefault( HumanoidPartType.RightEye );
            CreateDefault( HumanoidPartType.LeftEyeBrow );
            CreateDefault( HumanoidPartType.RightEyeBrow );
            CreateDefault( HumanoidPartType.LeftEar );
            CreateDefault( HumanoidPartType.RightEar );

            CreateDefault( HumanoidPartType.LeftArm );
            CreateDefault( HumanoidPartType.RightArm );
            CreateDefault( HumanoidPartType.LeftHand );
            CreateDefault( HumanoidPartType.RightHand );

            CreateDefault( HumanoidPartType.LeftLeg );
            CreateDefault( HumanoidPartType.RightLeg );
            CreateDefault( HumanoidPartType.LeftFoot );
            CreateDefault( HumanoidPartType.RightFoot );

            CreateDefault( HumanoidPartType.Tail1 );
            CreateDefault( HumanoidPartType.Tail2 );
            CreateDefault( HumanoidPartType.Tail3 );
        }

    }
}
