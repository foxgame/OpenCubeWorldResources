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
    public class InternalObjectManager : Singleton<InternalObjectManager>
    {
        public Dictionary<long , Cube> fixedCubeDictionary = new Dictionary<long , Cube>();
        public List<Cube> cubeList = new List<Cube>();

        public Dictionary<long , CubeMesh> fixedCubeMeshDictionary = new Dictionary<long , CubeMesh>();
        public List<CubeMesh> cubeMeshList = new List<CubeMesh>();

        public void Clear()
        {
            fixedCubeDictionary.Clear();
            cubeList.Clear();

            fixedCubeMeshDictionary.Clear();
            cubeMeshList.Clear();
        }

        public CubeMesh GetFixedCubeMesh( Vector3Int position )
        {
            int px = position.x / Setting.FixedCubeMeshSize;
            int py = position.y / Setting.FixedCubeMeshSize;
            int pz = position.z / Setting.FixedCubeMeshSize;

            CubeMesh fixedCubeMesh = null;

            long index = FixedVector3.GetIndex( px , py , pz );

            if ( !fixedCubeMeshDictionary.TryGetValue( index , out fixedCubeMesh ) )
            {
                fixedCubeMesh = new CubeMesh();
                fixedCubeMesh.BoundingMin = new Vector3Int( px , py , pz );
                fixedCubeMesh.BoundingMax = new Vector3Int( px + Setting.FixedCubeMeshSize ,
                    py + Setting.FixedCubeMeshSize ,
                    pz + Setting.FixedCubeMeshSize );

                cubeMeshList.Add( fixedCubeMesh );
                fixedCubeMeshDictionary.Add( index , fixedCubeMesh );
            }

            return fixedCubeMesh;
        }

        public Cube GetFixedCube( int index )
        {
            Cube fixedCube = null;

            fixedCubeDictionary.TryGetValue( index , out fixedCube );

            return fixedCube;
        }

        public Cube GetFixedCube( Vector3Int position )
        {
            Cube fixedCube = null;

            fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y , position.z ) , out fixedCube );

            return fixedCube;
        }

        public Cube AddCube( Vector3 position , InternalTextureData textureData )
        {
            Cube cube = new Cube();
            cube.position = position;
            cube.textureData = textureData;

            cubeList.Add( cube );

            CubeMesh cubeMesh = new CubeMesh();
            cubeMesh.BoundingMin = cubeMesh.BoundingMax = new Vector3Int( (int)position.x , (int)position.y , (int)position.z );
            cubeMeshList.Add( cubeMesh );

            cube.cubeMesh = cubeMesh;
            cubeMesh.AddCube( cube );

            return cube;
        }

        public Cube AddFixedCube( Vector3Int position , InternalTextureData textureData )
        {
            Cube cube = new Cube();
            cube.isFixed = true;
            cube.fixedPosition.Value = position;
            cube.textureData = textureData;

            cubeList.Add( cube );
            fixedCubeDictionary.Add( cube.fixedPosition.Index , cube );

            CubeMesh cubeMesh = GetFixedCubeMesh( position );
            cube.cubeMesh = cubeMesh;

            UpdateFixedNeighbor( cube );

            cubeMesh.AddFixedCube( cube );

            return cube;
        }

        public void UpdateFixedCube( Cube cube , Vector3Int position )
        {
            Vector3Int lastPosition = cube.fixedPosition.Value;

            CubeMesh lastCubeMesh = GetFixedCubeMesh( lastPosition );
            CubeMesh cubeMesh = GetFixedCubeMesh( position );

            RemoveFixedNeighbor( cube );
            RemoveCube( cube );

            cube.fixedPosition.Value = position;

            cubeList.Add( cube );
            fixedCubeDictionary.Add( cube.fixedPosition.Index , cube );

            cube.cubeMesh = cubeMesh;

            UpdateFixedNeighbor( cube );

            cubeMesh.AddFixedCube( cube );
        }

        public void UpdateFixedNeighbor( Cube cube )
        {
            Cube cubeOther = null;

            Vector3Int position = cube.fixedPosition.Value;

            // left
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x - 1 , position.y , position.z ) , out cubeOther ) )
            {
                cubeOther.right = cube;
                cube.left = cubeOther;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // right
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x + 1 , position.y , position.z ) , out cubeOther ) )
            {
                cubeOther.left = cube;
                cube.right = cubeOther;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // up
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y + 1 , position.z ) , out cubeOther ) )
            {
                cubeOther.down = cube;
                cube.up = cubeOther;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // down
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y - 1 , position.z ) , out cubeOther ) )
            {
                cubeOther.up = cube;
                cube.down = cubeOther;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // forward
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y , position.z + 1 ) , out cubeOther ) )
            {
                cubeOther.back = cube;
                cube.forward = cubeOther;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // back
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y , position.z - 1 ) , out cubeOther ) )
            {
                cubeOther.forward = cube;
                cube.back = cubeOther;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }
        }

        public void RemoveFixedNeighbor( Cube cube )
        {
            Cube cubeOther = null;

            Vector3Int position = cube.fixedPosition.Value;

            cube.left = null;
            cube.right = null;
            cube.up = null;
            cube.down = null;
            cube.forward = null;
            cube.back = null;

            // left
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x - 1 , position.y , position.z ) , out cubeOther ) )
            {
                cubeOther.right = null;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // right
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x + 1 , position.y , position.z ) , out cubeOther ) )
            {
                cubeOther.left = null;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // up
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y + 1 , position.z ) , out cubeOther ) )
            {
                cubeOther.down = null;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // down
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y - 1 , position.z ) , out cubeOther ) )
            {
                cubeOther.up = null;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // forward
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y , position.z + 1 ) , out cubeOther ) )
            {
                cubeOther.back = null;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }

            // back
            if ( fixedCubeDictionary.TryGetValue( FixedVector3.GetIndex( position.x , position.y , position.z - 1 ) , out cubeOther ) )
            {
                cubeOther.forward = null;

                cubeOther.cubeMesh.UpdateIndex( cubeOther );
            }
        }

        public void RemoveCube( Cube cube )
        {
            CubeMesh cubeMesh = cube.cubeMesh;

            cubeMesh.RemoveCube( cube );

            if ( cube.isFixed )
            {
                fixedCubeDictionary.Remove( cube.fixedPosition.Index );
            }
            else
            {
                cubeMeshList.Remove( cubeMesh );
            }

            cube.cubeMesh = null;

            cubeList.Remove( cube );
        }


    }
}

