using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip; 
using ICSharpCode.SharpZipLib.Zip; 
using System.IO;
using System;
using UnityEngine.Networking;
using System.Text;
using X;
using XLua;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_STANDALONE_WIN

[StructLayout( LayoutKind.Sequential , CharSet = CharSet.Auto )]
public class OpenDialogFile
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

[StructLayout( LayoutKind.Sequential , CharSet = CharSet.Auto )]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

#endif

namespace X
{
    [LuaCallCSharp]
    public partial class Utility
    {
#if UNITY_STANDALONE_WIN
        [DllImport( "Comdlg32.dll" , SetLastError = true , ThrowOnUnmappableChar = true , CharSet = CharSet.Auto )]
        public static extern bool GetOpenFileName( [In, Out] OpenDialogFile ofn );

        [DllImport( "shell32.dll" , SetLastError = true , ThrowOnUnmappableChar = true , CharSet = CharSet.Auto )]
        public static extern bool SHGetPathFromIDList( [In] IntPtr pidl , [In, Out] char[] fileName );

        [DllImport( "shell32.dll" , SetLastError = true , ThrowOnUnmappableChar = true , CharSet = CharSet.Auto )]
        public static extern IntPtr SHBrowseForFolder( [In, Out] OpenDialogDir ofn );

#endif

        public static string OpenFolderPanel( string title )
        {
#if UNITY_EDITOR
            return EditorUtility.OpenFolderPanel( title , CurrentDirectory , "" );
#elif UNITY_STANDALONE_WIN
        try
        {
            OpenDialogDir ofn2 = new OpenDialogDir();
            ofn2.pszDisplayName = new string( new char[ 1024 ] );
            ofn2.lpszTitle = title;

            IntPtr pidlPtr = SHBrowseForFolder( ofn2 );

            if ( pidlPtr != null )
            {
                char[] charArray = new char[ 1024 ];

                SHGetPathFromIDList( pidlPtr , charArray );
                string fullDirPath = new String( charArray );

                return fullDirPath.Substring( 0 , fullDirPath.IndexOf( '\0' ) );
            }
        }
        catch ( Exception )
        {
        }

        return "";
#else
        return "";
#endif
        }


        public static string OpenFilePanel( string title , string extension )
        {
#if UNITY_EDITOR
            return EditorUtility.OpenFilePanel( title , CurrentDirectory , extension );
#elif UNITY_STANDALONE_WIN
        try
        {
            OpenDialogFile ofn = new OpenDialogFile();
            ofn.structSize = Marshal.SizeOf( ofn );
            //        ofn.filter = "All Files\0*.*\0\0";
            ofn.filter = extension + "\0*." + extension + "\0\0";
            ofn.file = new string( new char[ 1024 ] );
            ofn.maxFile = ofn.file.Length;
            ofn.fileTitle = new string( new char[ 64 ] );
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = CurrentDirectory;
            ofn.title = title;
            ofn.flags = 0x00001000 | 0x00000800 | 0x00000004;

            if ( GetOpenFileName( ofn ) )
            {
                return ofn.file;
            }
        }
        catch ( Exception )
        {
        }

        return "";
#else
        return "";
#endif
        }

        public static string SaveFilePanel( string title , string extension )
        {
#if UNITY_EDITOR
            return EditorUtility.SaveFilePanel( title , CurrentDirectory , "" , extension );
#elif UNITY_STANDALONE_WIN
        try
        {
            OpenDialogFile ofn = new OpenDialogFile();
            ofn.structSize = Marshal.SizeOf( ofn );
            ofn.filter = extension + "\0*." + extension + "\0\0";
            ofn.file = new string( new char[ 1024 ] );
            ofn.maxFile = ofn.file.Length;
            ofn.fileTitle = new string( new char[ 64 ] );
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = CurrentDirectory;
            ofn.title = title;
            ofn.flags = 0x00000800 | 0x00000004 | 0x00000002;
            ofn.defExt = extension;

            if ( GetOpenFileName( ofn ) )
            {
                return ofn.file;
            }
        }
        catch ( Exception )
        {
        }

        return "";
#else
        return "";
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Sprite LoadSprite( string name , string childName )
        {
            Sprite[] arr = Resources.LoadAll<Sprite>( name );

            Sprite sprite = arr.Single( s => s.name == childName );

            return sprite;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rectX"></param>
        /// <param name="rectY"></param>
        /// <param name="rectW"></param>
        /// <param name="rectH"></param>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pixelsPerUnit"></param>
        /// <param name="extrude"></param>
        /// <param name="meshType"></param>
        /// <param name="borderX"></param>
        /// <param name="borderY"></param>
        /// <param name="borderZ"></param>
        /// <param name="borderW"></param>
        /// <param name="generateFallbackPhysicsShape"></param>
        /// <returns></returns>
        public static Sprite LoadTextureSprite( string name ,
            float rectX = 0.0f , float rectY = 0.0f , float rectW = 0.0f , float rectH = 0.0f ,
            float pX = 0.0f , float pY = 0.0f ,
            float pixelsPerUnit = 100.0f ,
            uint extrude = 0 ,
            SpriteMeshType meshType = SpriteMeshType.Tight ,
            float borderX = 0.0f , float borderY = 0.0f , float borderZ = 0.0f , float borderW = 0.0f ,
            bool generateFallbackPhysicsShape = false )
        {
            if ( !File.Exists( name ) )
            {
                return null;
            }

            Texture2D t2d = new Texture2D( 0 , 0 );

            FileStream fs = File.Open( name , FileMode.Open , FileAccess.ReadWrite , FileShare.ReadWrite );
            byte[] bytes = new byte[ fs.Length ];
            fs.Read( bytes , 0 , (int)fs.Length );
            fs.Close();
            fs.Dispose();

            t2d.LoadImage( bytes );

            Sprite sprite = Sprite.Create( t2d ,
                new Rect( rectX , rectY , rectW , rectH ) ,
                new Vector2( pX , pY ) ,
                pixelsPerUnit ,
                extrude ,
                meshType ,
                new Vector4( borderX , borderY , borderZ , borderW ) ,
                generateFallbackPhysicsShape );

            return sprite;
        }

        public static Texture2D LoadTexture( string name )
        {
            if ( !File.Exists( name ) )
            {
                return null;
            }

            Texture2D t2d = new Texture2D( 0 , 0 );

            FileStream fs = File.Open( name , FileMode.Open , FileAccess.ReadWrite , FileShare.ReadWrite );
            byte[] bytes = new byte[ fs.Length ];
            fs.Read( bytes , 0 , (int)fs.Length );
            fs.Close();
            fs.Dispose();

            t2d.LoadImage( bytes );

            return t2d;
        }



    }

}

