using UnityEngine;
using System.Collections;
using X;
using System.Collections.Generic;
using XLua;
using System.IO;
using System.Text;

namespace X
{

    [LuaCallCSharp]
    [System.Serializable]
    public class FixedVector3
    {
        public static int MinIndex = -10000;
        public static int MaxIndex = 10000;
        public static int MaxIndexTemp = 100000;

        public static long GetIndex( int x , int y , int z )
        {
            return ( x + MaxIndex ) + ( (long)( y + MaxIndex ) * MaxIndexTemp ) + ( (long)( z + MaxIndex ) * MaxIndexTemp * MaxIndexTemp );
        }

        public static long GetIndex( Vector3Int vector3Int )
        {
            return ( vector3Int.x + MaxIndex ) + ( (long)( vector3Int.y + MaxIndex ) * MaxIndexTemp ) + ( (long)( vector3Int.z + MaxIndex ) * MaxIndexTemp * MaxIndexTemp );
        }

        public static Vector3Int GetXYZ( long index )
        {
            Vector3Int vector3Int = Vector3Int.zero;

            int x = (int)( index % MaxIndexTemp ); 
            index -= x; index /= MaxIndexTemp;
            int y = (int)( index % MaxIndexTemp ); 
            index -= y; index /= MaxIndexTemp;
            int z = (int)( index % MaxIndexTemp );

            vector3Int.x = x - MaxIndex;
            vector3Int.y = y - MaxIndex;
            vector3Int.z = z - MaxIndex;

            return vector3Int;
        }

        [SerializeField]
        long index = 0;
        [SerializeField]
        Vector3Int vector3Int = Vector3Int.zero;

        public long Index
        {
            set
            {
                index = value;
                vector3Int = GetXYZ( index );
            }
            get
            {
                return index;
            }
        }

        public int x
        {
            get
            {
                return vector3Int.x;
            }
        }
        public int y
        {
            get
            {
                return vector3Int.y;
            }
        }
        public int z
        {
            get
            {
                return vector3Int.z;
            }
        }


        public Vector3Int Value
        {
            set
            {
                vector3Int = value;
                index = GetIndex( vector3Int );
            }
            get
            {
                return vector3Int;
            }
        }
    }
}
