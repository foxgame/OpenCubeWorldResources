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

    public class MaterialManager : Singleton<MaterialManager>
    {
        Dictionary<string , Material> materialDic = new Dictionary<string , Material>();

        public void Clear()
        {
            materialDic.Clear();
        }

        public Material GetMaterial( string id )
        {
            Material material = null;

            if ( materialDic.TryGetValue( id , out material ) )
            {
                return material;
            }

            return null;
        }

        public void RemoveMaterial( string id )
        {
            // no release.
            materialDic.Remove( id );
        }

        public Material CreateMaterialTerrain( string id , Texture2D texture2D )
        {
            Material material = null;

            if ( materialDic.TryGetValue( id , out material ) )
            {
                return material;
            }

            string shaderName = "Game/Terrain";
            Shader shader = Shader.Find( shaderName );

            material = new Material( shader );
            material.mainTexture = texture2D;

            materialDic.Add( id , material );

            return material;
        }

        public Material CreateMaterialPlayerPart( string id , Color color )
        {
            Material material = null;

            if ( materialDic.TryGetValue( id , out material ) )
            {
                return material;
            }

            string shaderName = "Shader Graphs/PlayerPart";
            Shader shader = Shader.Find( shaderName );

            material = new Material( shader );
            material.SetColor( "mainColor" , color );

            materialDic.Add( id , material );

            return material;
        }

        public Material CreateMaterialTerrainLayer( string id , Texture2D texture2D )
        {
            Material material = null;

            if ( materialDic.TryGetValue( id , out material ) )
            {
                return material;
            }

            string shaderName = "Game/TerrainLayer";
            Shader shader = Shader.Find( shaderName );

            material = new Material( shader );
            material.mainTexture = texture2D;

            materialDic.Add( id , material );

            return material;
        }

        public Material CreateMaterialUnit( string id , Texture2D texture2D )
        {
            Material material = null;

            if ( materialDic.TryGetValue( id , out material ) )
            {
                return material;
            }

            string shaderName = "Game/Unit";
            Shader shader = Shader.Find( shaderName );

            material = new Material( shader );
            material.mainTexture = texture2D;

            materialDic.Add( id , material );

            return material;
        }

        public Material CreateMaterial( string id , Texture2D texture2D )
        {
            Material material = null;

            if ( materialDic.TryGetValue( id , out material ) )
            {
                return material;
            }

            string shaderName = "Game/Transparent";
            Shader shader = Shader.Find( shaderName );

            material = new Material( shader );
            material.mainTexture = texture2D;

            materialDic.Add( id , material );

            return material;
        }


    }

}
