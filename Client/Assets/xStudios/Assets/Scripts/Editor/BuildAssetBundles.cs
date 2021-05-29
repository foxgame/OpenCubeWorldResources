using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace X
{
    public class BuildAssetBundles
    {

        public static void ClearTemp()
        {
            string tempPath = "Temp/assets";

            if ( Directory.Exists( tempPath ) )
            {
                Directory.Delete( tempPath , true );
            }

            Directory.CreateDirectory( tempPath );
        }

        public static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach ( UnityEngine.Object obj in Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.Assets ) )
            {
                path = AssetDatabase.GetAssetPath( obj );
                if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    path = Path.GetDirectoryName( path );
                    break;
                }
            }

            return path;
        }


        [MenuItem( "Assets/Build Assets Preview" )]
        static void BuildAssetsPreview()
        {
            UnityEngine.Object[] objects = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.Assets );

            for ( int i = 0 ; i < objects.Length ; i++ )
            {
                UnityEngine.Object obj = objects[ i ];
                string path = AssetDatabase.GetAssetPath( obj );
                string name = Path.GetDirectoryName( path ) + "\\Icons\\" + Path.GetFileNameWithoutExtension( path ) + ".png";

                Texture2D texture = AssetPreview.GetAssetPreview( obj );
                byte[] bytes = texture.EncodeToPNG();

                File.WriteAllBytes( name , bytes );

//                 Texture2D.DestroyImmediate( texture );
            }

        }
        
        [MenuItem( "Assets/Build AssetBundles" )]
        static void BuildAllAssetBundles()
        {
            try
            {
                ClearTemp();
            }
            catch ( System.Exception )
            {
            }

            string path = "Assets";
            UnityEngine.Object[] objects = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.Assets );

            if ( objects.Length == 0 )
            {
                return;
            }

            DirectoryInfo pathInfo = new DirectoryInfo( AssetDatabase.GetAssetPath( objects[ 0 ] ) );
            string[] dir = Path.GetDirectoryName( AssetDatabase.GetAssetPath( objects[ 0 ] ) ).Replace( "\\" , "/" ).Split( '/' );
            string name1 = dir[ dir.Length - 1 ];

            AssetSetting assetSetting = Resources.Load<AssetSetting>( "AssetSetting" );

            List<AssetBundleBuild> builds = GetSharesBuilds();
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = name1 + Utility.AssetExtension;
            build.assetNames = new string[ objects.Length ];
            build.addressableNames = new string[ objects.Length ];

            for ( int i = 0 ; i < objects.Length ; i++ )
            {
                UnityEngine.Object obj = objects[ i ];

                path = AssetDatabase.GetAssetPath( obj );

                if ( !path.Contains( "." ) || path.Contains( Utility.AssetExtension ) )
                {
                    continue;
                }

                if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    string name = Path.GetFileNameWithoutExtension( path );

                    build.assetNames[ i ] = path;
                    build.addressableNames[ i ] = name;
                }
            }

            builds.Add( build );

            BuildPipeline.BuildAssetBundles( "Temp" , builds.ToArray() , 
                BuildAssetBundleOptions.None , assetSetting.buildTarget );

            string filePath = Path.GetDirectoryName( path ) + "/" + name1 + Utility.AssetExtension;

            if ( File.Exists( filePath ) )
            {
                File.Delete( filePath );
            }

            FileInfo fi = new FileInfo( "Temp/" + name1 + Utility.AssetExtension );
            fi.MoveTo( filePath );

            Resources.UnloadAsset( assetSetting );
        }


        static List<AssetBundleBuild> GetSharesBuilds()
        {
            List<string> pathList = new List<string>();

            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/cube_mat_black.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/cube_mat_green.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/cube_mat_grey.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/cube_mat_orange.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/cube_mat_pink.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/cube_mat_white.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/cubeworld_mat.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/grass_texture.mat" );
            pathList.Add( "Assets/Bitgem/Cube_World/_Materials/URP_Multicolor_Material/URP_multicolor_mat.mat" );

            AssetSetting assetSetting = Resources.Load<AssetSetting>( "AssetSetting" );

            List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = "Cube_World" + Utility.AssetExtension;
            build.assetNames = new string[ pathList.Count ];
            build.addressableNames = new string[ pathList.Count ];

            for ( int i = 0 ; i < pathList.Count ; i++ )
            {
                string path = pathList[ i ];

                if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    string name = Path.GetFileNameWithoutExtension( path );

                    build.assetNames[ i ] = path;
                    build.addressableNames[ i ] = name;
                }
            }

            builds.Add( build );

            return builds;
        }

        [MenuItem( "Assets/Build Single AssetBundles" )]
        static void BuildSingleAssetBundles()
        {
            try
            {
                ClearTemp();
            }
            catch ( System.Exception )
            {
            }

            string path = "Assets";
            UnityEngine.Object[] objects = Selection.GetFiltered( typeof( UnityEngine.Object ) , SelectionMode.Assets );

            AssetSetting assetSetting = Resources.Load<AssetSetting>( "AssetSetting" );


            for ( int i = 0 ; i < objects.Length ; i++ )
            {
                UnityEngine.Object obj = objects[ i ];

                path = AssetDatabase.GetAssetPath( obj );

                if ( !path.Contains( "." ) || path.Contains( Utility.AssetExtension ) )
                {
                    continue;
                }

                if ( !string.IsNullOrEmpty( path ) && File.Exists( path ) )
                {
                    List<AssetBundleBuild> builds = GetSharesBuilds();
                    AssetBundleBuild build = new AssetBundleBuild();

                    string name = Path.GetFileNameWithoutExtension( path );

                    build.assetBundleName = path;
                    build.assetNames = new string[] { path };
                    build.addressableNames = new string[] { name };

                    builds.Add( build );

                    BuildPipeline.BuildAssetBundles( "Temp" , builds.ToArray() ,
                        BuildAssetBundleOptions.None , assetSetting.buildTarget );

                    string filePath = Path.GetDirectoryName( path ) + "/" + name + Utility.AssetExtension;

                    if ( File.Exists( filePath ) )
                    {
                        File.Delete( filePath );
                    }

                    FileInfo fi = new FileInfo( "Temp/" + path );
                    fi.MoveTo( filePath );
                }
            }


            Resources.UnloadAsset( assetSetting );
        }
    }

}


