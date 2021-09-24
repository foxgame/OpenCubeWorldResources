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
    public class InternalTextureManager : Singleton<InternalTextureManager>
    {
        int maxSize = 4096;
        int minSize = 32;

        Texture2D texture2D = null;

        Dictionary<string , InternalTextureData> textureDataDictionary = new Dictionary<string , InternalTextureData>();
        Dictionary<string , Texture2D> textureDictionary = new Dictionary<string , Texture2D>();

        public int MaxSize { get { return maxSize; } set { maxSize = value; } }

        public Texture2D Texture2D { get { return texture2D; } }


        protected override void InitSingleton()
        {
        }

        public void InitTexture2D()
        {
            texture2D = new Texture2D( maxSize , maxSize , TextureFormat.ARGB32 , false );
        }

        bool CheckPosition( Rect rect )
        {
            foreach ( KeyValuePair<string, InternalTextureData> item in textureDataDictionary )
            {
                InternalTextureData textureData = item.Value;

                if ( textureData.rect.Overlaps( rect ) )
                {
                    return false;
                }
            }

            return true;
        }

        bool FindPosition( Vector2Int size , out Vector2Int position )
        {
            position = Vector2Int.zero;

            while ( position.y < maxSize )
            {
                if ( CheckPosition( new Rect( position , size ) ) )
                {
                    return true;
                }

                position.x += minSize;

                if ( position.x + size.x > maxSize  )
                {
                    position.x = 0;
                    position.y += minSize;
                }
            }

            return false;
        }

        public Texture2D GetSingleTexture( string name )
        {
            Texture2D texture = null;

            textureDictionary.TryGetValue( name , out texture );

            return texture;
        }

        public InternalTextureData SingleTexture( string name , Texture2D texture )
        {
            InternalTextureData textureData = null;

            if ( textureDataDictionary.TryGetValue( name , out textureData ) )
            {
                return textureData;
            }

            Vector2Int position = Vector2Int.zero;
            Vector2Int size = new Vector2Int( texture.width , texture.height );

            if ( !FindPosition( size , out position ) )
            {
                return null;
            }

            textureData = new InternalTextureData();
            textureData.name = name;
            textureData.rect = new Rect( position , size );
            textureData.texture2D = texture;
            textureData.rectUV = new Rect( 0f , 0f , 1f , 1f );

            textureDictionary.Add( name , texture );
            textureDataDictionary.Add( name , textureData );

            return textureData;
        }

        public InternalTextureData BakeTexture( string name , Texture2D texture )
        {
            InternalTextureData textureData = null;

            if ( textureDataDictionary.TryGetValue( name , out textureData ) )
            {
                return textureData;
            }

            Vector2Int position = Vector2Int.zero;
            Vector2Int size = new Vector2Int( texture.width , texture.height );

            if ( !FindPosition( size , out position ) )
            {
                return null;
            }

            texture2D.SetPixels32( position.x , position.y , size.x , size.y , texture.GetPixels32() );
            texture2D.Apply();

            textureData = new InternalTextureData();
            textureData.name = name;
            textureData.rect = new Rect( position , size );
            textureData.texture2D = texture;
            textureData.rectUV = new Rect( (float)position.x / maxSize , (float)position.y / maxSize ,
                (float)size.x / maxSize , (float)size.y / maxSize );

            textureDataDictionary.Add( name , textureData );

            return textureData;
        }



    }
}
