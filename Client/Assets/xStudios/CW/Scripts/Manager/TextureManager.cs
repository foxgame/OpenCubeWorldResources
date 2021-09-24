using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using X;

namespace X
{

    public class TextureManager : Singleton<TextureManager>
    {
        Dictionary<string , Texture2D> textureDic = new Dictionary<string , Texture2D>();

        public void Clear()
        {
            textureDic.Clear();
        }

        public Texture2D GetTexture2D( string id )
        {
            Texture2D texture2D = null;

            if ( textureDic.TryGetValue( id , out texture2D ) )
            {
                return texture2D;
            }

            return null;
        }

        public void RemoveTexture2D( string id )
        {
            // no release.
            textureDic.Remove( id );
        }

        public Texture2D CreateTexture2D( string id , string path )
        {
            Texture2D texture2D = null;

            if ( textureDic.TryGetValue( id , out texture2D ) )
            {
                return texture2D;
            }

            texture2D = new Texture2D( 0 , 0 );
            textureDic.Add( id , texture2D );

            if ( !File.Exists( path ) )
            {
                if ( File.Exists( path + ".png" ) )
                {
                    path += ".png";
                }
                else if ( File.Exists( path + ".jpg" ) )
                {
                    path += ".jpg";
                }
            }

            Utility.LoadRes( this ,
               path ,
               delegate ( byte[] bytes , bool err )
               {
                   // may delay.

                   if ( err )
                   {
                       Debug.LogError( "load texture failed " + path );
                       return;
                   }

                   texture2D.LoadImage( bytes );
                   texture2D.wrapMode = TextureWrapMode.Clamp;

                   if ( Setting.PointFilterMode )
                       texture2D.filterMode = FilterMode.Point;

                   texture2D.Apply();
               } );

            return texture2D;
        }



    }
}
