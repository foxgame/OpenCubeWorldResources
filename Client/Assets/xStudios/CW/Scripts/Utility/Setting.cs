using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.Networking;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class Setting
    {
        public static bool EnabledMusic = true;
        public static bool EnabledSound = true;

        public static float MusicVolume = 0.7f;
        public static float SoundVolume = 0.7f;

        public static int Version = 1000001;

        public static bool UseAsync = true;

        public static bool IsWWW = false;

        public static bool DebugMode = false;

        public static bool PointFilterMode = true;

        public static string StreamingAssetsPath;
        public static string PersistentDataPath;
        public static string AddonPath;

        public static string Language = "Default";

        public static float ScreenWidth = 720f;
        public static float ScreenHeight = 1280f;

        public static float GameSpeed = 1f;

        public static float FrameTime = 0.05f;

        public static int UnitMoveSpeed = 6;
        public static int UnitJumpPower = 6;

        public static int MaxMapLayer = 32;
        public static int MaxMapSize = 256;

        public static int IconSize = 64;

        public static int FixedCubeMeshSize = 2;

        //         public static int BattleMapWidth = 0;
        //         public static int BattleMapHeight = 0;
        // 
        //         public static int BattleMapTileWidth = 0;
        //         public static int BattleMapTileHeight = 0;
        // 
        //         public static int BattleMapTileWidthHalf = 0;
        //         public static int BattleMapTileHeightHalf = 0;

        public static string SyncLog = "";


        public static string GetVersion()
        {
            int mainVer = Version / 1000000;
            int mVer = ( Version / 1000 ) % 1000;
            int sourceVer = Version % 1000;

            return "V " + mainVer + "." + string.Format( "{0:D2}" , mVer ) + "." + string.Format( "{0:D3}" , sourceVer );
        }

        public static void Save()
        {
            PlayerPrefs.SetString( "Language" , Language );
            PlayerPrefs.SetFloat( "MusicVolume" , MusicVolume );
            PlayerPrefs.SetFloat( "SoundVolume" , SoundVolume );
            PlayerPrefs.SetInt( "EnabledMusic" , EnabledMusic ? 1 : 0 );
            PlayerPrefs.SetInt( "EnabledSound" , EnabledSound ? 1 : 0 );

            PlayerPrefs.Save();
        }

        public static void Load()
        {
            MusicVolume = 0.7f;
            SoundVolume = 0.7f;
            Language = "Default";

            Language = PlayerPrefs.GetString( "Language" , Language );
            MusicVolume = PlayerPrefs.GetFloat( "MusicVolume" , MusicVolume );
            SoundVolume = PlayerPrefs.GetFloat( "SoundVolume" , SoundVolume );
            EnabledMusic = ( PlayerPrefs.GetInt( "EnabledMusic" , 1 ) == 1 ) ? true : false;
            EnabledSound = ( PlayerPrefs.GetInt( "EnabledSound" , 1 ) == 1 ) ? true : false;
        }

        public static void Init()
        {
#if UNITY_EDITOR
            StreamingAssetsPath = Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
			StreamingAssetsPath = Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
			StreamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_WEBPLAYER
			StreamingAssetsPath = Application.dataPath + "/StreamingAssets/";			
#else
			StreamingAssetsPath = Application.streamingAssetsPath + "/";
			//StreamingAssetsPath = Application.dataPath + "/StreamingAssets/";
#endif
#if UNITY_EDITOR
            SyncLog = "./sync.txt";
#else
            SyncLog = Application.persistentDataPath + "/sync.txt";
#endif

            //#if UNITY_EDITOR
            Debug.Log( "Screen: " + Screen.width + " " + Screen.height );
            Debug.Log( "memory: setting " + System.GC.GetTotalMemory( true ) );
            //#endif

            PersistentDataPath = Application.persistentDataPath;

            Debug.Log( Application.persistentDataPath + " " + PersistentDataPath );
            Debug.Log( Application.streamingAssetsPath );

            IsWWW = Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.WebGLPlayer ||
            Application.platform == RuntimePlatform.WSAPlayerARM ||
            Application.platform == RuntimePlatform.WSAPlayerX64 ||
            Application.platform == RuntimePlatform.WSAPlayerX86;

            Utility.CurrentDirectory = Environment.CurrentDirectory.Replace( "\\" , "/" ) + "/";

#if !UNITY_EDITOR
            Utility.AssetsPath = Utility.CurrentDirectory;
            Utility.MapsPath = Utility.CurrentDirectory + "Maps/";
            Utility.PlayerDataPath = Utility.CurrentDirectory + "PlayerData/";
#endif

            Debug.Log( Utility.CurrentDirectory );
            Debug.Log( Utility.AssetsPath );
            Debug.Log( Utility.MapsPath );

#if UNITY_WEBPLAYER || UNITY_WEBGL
			AddonPath = StreamingAssetsPath;
#else
            AddonPath = PersistentDataPath;
#endif
        }


    }

}
