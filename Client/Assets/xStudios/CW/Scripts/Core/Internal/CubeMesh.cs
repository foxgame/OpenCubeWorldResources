using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class CubeMesh
    {
        public enum Face
        {
            Up = 0,
            Down,

            Forward,
            Back,

            Left,
            Right,


            Count
        }

        List<Vector3> normalList = new List<Vector3>();
        List<Vector3> verticeList = new List<Vector3>();
        List<int> indexList = new List<int>();
        List<Vector2> uvList = new List<Vector2>();
        List<Color> colorList = new List<Color>();

        Dictionary<long , Cube> fixedCubeDictionary = new Dictionary<long , Cube>();
        List<Cube> cubeList = new List<Cube>();

        [SerializeField]
        Mesh mesh = new Mesh();

        [SerializeField]
        Vector3Int boundingMin;

        [SerializeField]
        Vector3Int boundingMax;

        [SerializeField]
        bool indexChanged = false;

        public bool IndexChanged { get { return indexChanged; } }

        //   6 - 7   
        //  /   /|
        // 4 - 5 | 
        // | 2 | 3
        // |/  |/
        // 0 - 1

        static Vector2 uvForward2 = new Vector2( 0.25f , 1f );
        static Vector2 uvForward3 = new Vector2( 0.5f , 1f );

        static Vector2 uvBack0 = new Vector2( 0.25f , 0.25f );
        static Vector2 uvBack1 = new Vector2( 0.5f , 0.25f );

        static Vector2 uvLeft0 = new Vector2( 0f , 0.5f );
        static Vector2 uvLeft2 = new Vector2( 0f , 0.75f );

        static Vector2 uvRight1 = new Vector2( 0.75f , 0.5f );
        static Vector2 uvRight3 = new Vector2( 0.75f , 0.75f );

        static Vector2 uvDown0 = new Vector2( 1f , 0.5f );
        static Vector2 uvDown2 = new Vector2( 1f , 0.75f );

        static Vector2 uv4 = new Vector2( 0.25f , 0.5f );
        static Vector2 uv5 = new Vector2( 0.5f , 0.5f );
        static Vector2 uv6 = new Vector2( 0.25f , 0.75f );
        static Vector2 uv7 = new Vector2( 0.5f , 0.75f );

        static Vector3 vertice0 = Vector3.zero;
        static Vector3 vertice1 = Vector3.zero;
        static Vector3 vertice2 = Vector3.zero;
        static Vector3 vertice3 = Vector3.zero;
        static Vector3 vertice4 = Vector3.zero;
        static Vector3 vertice5 = Vector3.zero;
        static Vector3 vertice6 = Vector3.zero;
        static Vector3 vertice7 = Vector3.zero;


        static int verticeCount = 4 * 6;

        public Mesh Mesh
        {
            get
            {
                return mesh;
            }
        }

        public Vector3Int BoundingMin
        {
            get
            {
                return boundingMin;
            }
            set
            {
                boundingMin = value;
            }
        }
        public Vector3Int BoundingMax
        {
            get
            {
                return boundingMax;
            }
            set
            {
                boundingMax = value;
            }
        }


        public void AddCube( Cube cube )
        {
            vertice0.x = -0.5f; vertice0.y = -0.5f; vertice0.z = -0.5f;
            vertice1.x = vertice0.x + 1; vertice1.y = vertice0.y; vertice1.z = vertice0.z;
            vertice2.x = vertice0.x; vertice2.y = vertice0.y; vertice2.z = vertice0.z + 1;
            vertice3.x = vertice0.x + 1; vertice3.y = vertice0.y; vertice3.z = vertice0.z + 1;
            vertice4.x = vertice0.x; vertice4.y = vertice0.y + 1; vertice4.z = vertice0.z;
            vertice5.x = vertice0.x + 1; vertice5.y = vertice0.y + 1; vertice5.z = vertice0.z;
            vertice6.x = vertice0.x; vertice6.y = vertice0.y + 1; vertice6.z = vertice0.z + 1;
            vertice7.x = vertice0.x + 1; vertice7.y = vertice0.y + 1; vertice7.z = vertice0.z + 1;

            cube.verticeStart = verticeCount * cubeList.Count;
            cube.index = cubeList.Count;

            AddVertice( cube );
            AddIndex( cube );

            cubeList.Add( cube );
        }

        public void AddFixedCube( Cube cube )
        {
            vertice0.x = cube.fixedPosition.x - boundingMin.x; 
            vertice0.y = cube.fixedPosition.y - boundingMin.y - 0.5f; 
            vertice0.z = cube.fixedPosition.z - boundingMin.z;
            vertice1.x = vertice0.x + 1; vertice1.y = vertice0.y; vertice1.z = vertice0.z;
            vertice2.x = vertice0.x; vertice2.y = vertice0.y; vertice2.z = vertice0.z + 1;
            vertice3.x = vertice0.x + 1; vertice3.y = vertice0.y; vertice3.z = vertice0.z + 1;
            vertice4.x = vertice0.x; vertice4.y = vertice0.y + 1; vertice4.z = vertice0.z;
            vertice5.x = vertice0.x + 1; vertice5.y = vertice0.y + 1; vertice5.z = vertice0.z;
            vertice6.x = vertice0.x; vertice6.y = vertice0.y + 1; vertice6.z = vertice0.z + 1;
            vertice7.x = vertice0.x + 1; vertice7.y = vertice0.y + 1; vertice7.z = vertice0.z + 1;

            cube.verticeStart = verticeCount * cubeList.Count;
            cube.index = cubeList.Count;

            AddVertice( cube );
            AddIndex( cube );

            cubeList.Add( cube );
            fixedCubeDictionary.Add( cube.fixedPosition.Index , cube );
        }

        Vector2 GetUV( Cube cube , Vector2 uv )
        {
            return new Vector2( cube.textureData.rectUV.x + uv.x * cube.textureData.rectUV.width ,
                cube.textureData.rectUV.y + uv.y * cube.textureData.rectUV.height );
        }

        void AddVertice( Cube cube )
        {
            //   6 - 7   
            //  /   /|
            // 4 - 5 | 
            // | 2 | 3
            // |/  |/
            // 0 - 1

            // up
            verticeList.Add( vertice4 );
            verticeList.Add( vertice5 );
            verticeList.Add( vertice6 );
            verticeList.Add( vertice7 );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            uvList.Add( GetUV( cube , uv4 ) );
            uvList.Add( GetUV( cube , uv5 ) );
            uvList.Add( GetUV( cube , uv6 ) );
            uvList.Add( GetUV( cube , uv7 ) );
            normalList.Add( Vector3.up );
            normalList.Add( Vector3.up );
            normalList.Add( Vector3.up );
            normalList.Add( Vector3.up );

            // back
            verticeList.Add( vertice0 );
            verticeList.Add( vertice1 );
            verticeList.Add( vertice4 );
            verticeList.Add( vertice5 );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            uvList.Add( GetUV( cube , uvBack0 ) );
            uvList.Add( GetUV( cube , uvBack1 ) );
            uvList.Add( GetUV( cube , uv4 ) );
            uvList.Add( GetUV( cube , uv5 ) );
            normalList.Add( Vector3.back );
            normalList.Add( Vector3.back );
            normalList.Add( Vector3.back );
            normalList.Add( Vector3.back );

            // left
            verticeList.Add( vertice0 );
            verticeList.Add( vertice2 );
            verticeList.Add( vertice4 );
            verticeList.Add( vertice6 );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            uvList.Add( GetUV( cube , uvLeft0 ) );
            uvList.Add( GetUV( cube , uvLeft2 )  );
            uvList.Add( GetUV( cube , uv4 ) );
            uvList.Add( GetUV( cube , uv6 ) );
            normalList.Add( Vector3.left );
            normalList.Add( Vector3.left );
            normalList.Add( Vector3.left );
            normalList.Add( Vector3.left );

            // forward
            verticeList.Add( vertice2 );
            verticeList.Add( vertice3 );
            verticeList.Add( vertice6 );
            verticeList.Add( vertice7 );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            uvList.Add( GetUV( cube , uvForward2 ) );
            uvList.Add( GetUV( cube , uvForward3 ) );
            uvList.Add( GetUV( cube , uv6 ) );
            uvList.Add( GetUV( cube , uv7 ) );
            normalList.Add( Vector3.forward );
            normalList.Add( Vector3.forward );
            normalList.Add( Vector3.forward );
            normalList.Add( Vector3.forward );

            //right
            verticeList.Add( vertice1 );
            verticeList.Add( vertice3 );
            verticeList.Add( vertice5 );
            verticeList.Add( vertice7 );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            uvList.Add( GetUV( cube , uvRight1 ) );
            uvList.Add( GetUV( cube , uvRight3 ) );
            uvList.Add( GetUV( cube , uv5 ) );
            uvList.Add( GetUV( cube , uv7 ) );
            normalList.Add( Vector3.right );
            normalList.Add( Vector3.right );
            normalList.Add( Vector3.right );
            normalList.Add( Vector3.right );

            // bottom
            verticeList.Add( vertice0 );
            verticeList.Add( vertice1 );
            verticeList.Add( vertice2 );
            verticeList.Add( vertice3 );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            colorList.Add( Color.white );
            uvList.Add( GetUV( cube , uvDown0 ) );
            uvList.Add( GetUV( cube , uvRight1 ) );
            uvList.Add( GetUV( cube , uvDown2 ) );
            uvList.Add( GetUV( cube , uvRight3 ) );
            normalList.Add( Vector3.down );
            normalList.Add( Vector3.down );
            normalList.Add( Vector3.down );
            normalList.Add( Vector3.down );
        }

        public void AddIndex( Cube cube )
        {
            int index = cube.verticeStart;

            cube.indexStart = indexList.Count;
            cube.indexCount = 0;

            if ( cube.up == null )
            {
                indexList.Add( index );
                indexList.Add( index + 2 );
                indexList.Add( index + 1 );
                indexList.Add( index + 1 );
                indexList.Add( index + 2 );
                indexList.Add( index + 3 );

                cube.indexCount += 6;
            }

            if ( cube.back == null )
            {
                indexList.Add( index + 4 );
                indexList.Add( index + 6 );
                indexList.Add( index + 5 );
                indexList.Add( index + 5 );
                indexList.Add( index + 6 );
                indexList.Add( index + 7 );

                cube.indexCount += 6;
            }

            if ( cube.left == null )
            {
                indexList.Add( index + 8 );
                indexList.Add( index + 9 );
                indexList.Add( index + 10 );
                indexList.Add( index + 9 );
                indexList.Add( index + 11 );
                indexList.Add( index + 10 );

                cube.indexCount += 6;
            }

            if ( cube.forward == null )
            {
                indexList.Add( index + 12 );
                indexList.Add( index + 13 );
                indexList.Add( index + 14 );
                indexList.Add( index + 13 );
                indexList.Add( index + 15 );
                indexList.Add( index + 14 );

                cube.indexCount += 6;
            }

            if ( cube.right == null )
            {
                indexList.Add( index + 16 );
                indexList.Add( index + 18 );
                indexList.Add( index + 17 );
                indexList.Add( index + 17 );
                indexList.Add( index + 18 );
                indexList.Add( index + 19 );

                cube.indexCount += 6;
            }

            if ( !cube.ignoreDown && 
                cube.down == null )
            {
                indexList.Add( index + 20 );
                indexList.Add( index + 21 );
                indexList.Add( index + 22 );
                indexList.Add( index + 21 );
                indexList.Add( index + 23 );
                indexList.Add( index + 22 );

                cube.indexCount += 6;
            }

            indexChanged = true;
        }

        public void InsertIndex( Cube cube , int pos )
        {
            int index = cube.verticeStart;

            cube.indexStart = pos;
            cube.indexCount = 0;

            if ( cube.up == null )
            {
                indexList.Insert( pos , index ); pos++;
                indexList.Insert( pos , index + 2 ); pos++;
                indexList.Insert( pos , index + 1 ); pos++;
                indexList.Insert( pos , index + 1 ); pos++;
                indexList.Insert( pos , index + 2 ); pos++;
                indexList.Insert( pos , index + 3 ); pos++;

                cube.indexCount += 6;
            }

            if ( cube.back == null )
            {
                indexList.Insert( pos , index + 4 ); pos++;
                indexList.Insert( pos , index + 6 ); pos++;
                indexList.Insert( pos , index + 5 ); pos++;
                indexList.Insert( pos , index + 5 ); pos++;
                indexList.Insert( pos , index + 6 ); pos++;
                indexList.Insert( pos , index + 7 ); pos++;

                cube.indexCount += 6;
            }

            if ( cube.left == null )
            {
                indexList.Insert( pos , index + 8 ); pos++;
                indexList.Insert( pos , index + 9 ); pos++;
                indexList.Insert( pos , index + 10 ); pos++;
                indexList.Insert( pos , index + 9 ); pos++;
                indexList.Insert( pos , index + 11 ); pos++;
                indexList.Insert( pos , index + 10 ); pos++;

                cube.indexCount += 6;
            }

            if ( cube.forward == null )
            {
                indexList.Insert( pos , index + 12 ); pos++;
                indexList.Insert( pos , index + 13 ); pos++;
                indexList.Insert( pos , index + 14 ); pos++;
                indexList.Insert( pos , index + 13 ); pos++;
                indexList.Insert( pos , index + 15 ); pos++;
                indexList.Insert( pos , index + 14 ); pos++;

                cube.indexCount += 6;
            }

            if ( cube.right == null )
            {
                indexList.Insert( pos , index + 16 ); pos++;
                indexList.Insert( pos , index + 18 ); pos++;
                indexList.Insert( pos , index + 17 ); pos++;
                indexList.Insert( pos , index + 17 ); pos++;
                indexList.Insert( pos , index + 18 ); pos++;
                indexList.Insert( pos , index + 19 ); pos++;

                cube.indexCount += 6;
            }

            if ( !cube.ignoreDown &&
                cube.down == null )
            {
                indexList.Insert( pos , index + 20 ); pos++;
                indexList.Insert( pos , index + 21 ); pos++;
                indexList.Insert( pos , index + 22 ); pos++;
                indexList.Insert( pos , index + 21 ); pos++;
                indexList.Insert( pos , index + 23 ); pos++;
                indexList.Insert( pos , index + 22 ); pos++;

                cube.indexCount += 6;
            }

            indexChanged = true;
        }

        public void UpdateIndex( Cube cube )
        {
            int lastIndex = cube.indexStart;
            indexList.RemoveRange( cube.indexStart , cube.indexCount );

            for ( int i = cube.index + 1 ; i < cubeList.Count ; i++ )
            {
                Cube cube1 = cubeList[ i ];
                cube1.indexStart -= cube.indexCount;
            }

            InsertIndex( cube , lastIndex );

            for ( int i = cube.index + 1 ; i < cubeList.Count ; i++ )
            {
                Cube cube1 = cubeList[ i ];
                cube1.indexStart += cube.indexCount;
            }
        }

        public void UpdateColor( Cube cube )
        {
            int index = cube.verticeStart;

            colorList[ index ] = cube.upColor; index++;
            colorList[ index ] = cube.upColor; index++;
            colorList[ index ] = cube.upColor; index++;
            colorList[ index ] = cube.upColor; index++;

            colorList[ index ] = cube.backColor; index++;
            colorList[ index ] = cube.backColor; index++;
            colorList[ index ] = cube.backColor; index++;
            colorList[ index ] = cube.backColor; index++;

            colorList[ index ] = cube.leftColor; index++;
            colorList[ index ] = cube.leftColor; index++;
            colorList[ index ] = cube.leftColor; index++;
            colorList[ index ] = cube.leftColor; index++;

            colorList[ index ] = cube.forwardColor; index++;
            colorList[ index ] = cube.forwardColor; index++;
            colorList[ index ] = cube.forwardColor; index++;
            colorList[ index ] = cube.forwardColor; index++;

            colorList[ index ] = cube.rightColor; index++;
            colorList[ index ] = cube.rightColor; index++;
            colorList[ index ] = cube.rightColor; index++;
            colorList[ index ] = cube.rightColor; index++;

            colorList[ index ] = cube.downColor; index++;
            colorList[ index ] = cube.downColor; index++;
            colorList[ index ] = cube.downColor; index++;
            colorList[ index ] = cube.downColor; index++;
        }

        public void UpdateAllIndex()
        {
            indexList.Clear();

            for ( int i = 0 ; i < cubeList.Count ; i++ )
            {
                Cube cube = cubeList[ i ];
                AddIndex( cube );
            }
        }

        public void UpdateMeshColor()
        {
            mesh.colors = colorList.ToArray();
        }

        public void UpdateMeshVertice()
        {
            mesh.triangles = new int[ 0 ];
            mesh.vertices = verticeList.ToArray();
            mesh.uv = uvList.ToArray();
            mesh.normals = normalList.ToArray();
        }

        public void UpdateMeshIndex()
        {
            mesh.triangles = indexList.ToArray();

            indexChanged = false;
        }

        public void RemoveCube( Cube cube )
        {
            verticeList.RemoveRange( cube.verticeStart , verticeCount );
            uvList.RemoveRange( cube.verticeStart , verticeCount );
            normalList.RemoveRange( cube.verticeStart , verticeCount );
            colorList.RemoveRange( cube.verticeStart , verticeCount );
            indexList.RemoveRange( cube.indexStart , cube.indexCount );

            for ( int i = cube.index + 1 ; i < cubeList.Count ; i++ )
            {
                Cube cube1 = cubeList[ i ];
                cube1.indexStart -= cube.indexCount;
                cube1.verticeStart -= verticeCount;

                cube1.index--;
            }

            for ( int i = cube.indexStart ; i < indexList.Count ; i++ )
            {
                indexList[ i ] -= verticeCount;
            }

            if ( cube.isFixed )
            {
                fixedCubeDictionary.Remove( cube.fixedPosition.Index );
            }

            cubeList.RemoveAt( cube.index );

            indexChanged = true;
        }

        public void ReBuildMesh()
        {
            mesh = new Mesh();
        }
    }
}
