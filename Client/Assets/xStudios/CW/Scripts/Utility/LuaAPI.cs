using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System;
using X;
using XLua;
using System.IO;

namespace X
{

    [LuaCallCSharp]
    public class LuaAPI : SingletonManager<LuaAPI>
    {
        // set your luaEnv
        public static LuaEnv luaEnv;

        public void DoString( string chunk , string chunkName = "" )
        {
            LuaTable scriptEnv = luaEnv.Global.Get<LuaTable>( chunkName );

            luaEnv.DoString( chunk , chunkName , scriptEnv );
        }

        public void LoadLuaDebug( byte[] chunk , string name )
        {
            LuaTable scriptEnv = luaEnv.NewTable();
            LuaTable meta = luaEnv.NewTable();
            meta.Set( "__index" , luaEnv.Global );
            scriptEnv.SetMetaTable( meta );

            luaEnv.DoString( chunk , name , scriptEnv );
            luaEnv.Global.Set( name , scriptEnv );
        }

        public void LoadDirectory( string path )
        {
            string[] files = Directory.GetFiles( path ,
                "*.lua" , SearchOption.AllDirectories );

            for ( int i = 0 ; i < files.Length ; i++ )
            {
                string file = files[ i ].Replace( "\\" , "/" );

                if ( file.Contains( Utility.MetaExtension ) )
                {
                    continue;
                }

                if ( file.Contains( Utility.UIPath ) )
                {
                    continue;
                }

                string data = File.ReadAllText( file );

                Load( Encoding.UTF8.GetBytes( data ) ,
                    Path.GetFileNameWithoutExtension( file ) );
            }
        }

        public void UnLoadDirectory( string path )
        {
            string[] files = Directory.GetFiles( path ,
                "*.lua" , SearchOption.AllDirectories );

            for ( int i = 0 ; i < files.Length ; i++ )
            {
                string file = files[ i ].Replace( "\\" , "/" );

                if ( file.Contains( Utility.MetaExtension ) )
                {
                    continue;
                }

                if ( file.Contains( Utility.UIPath ) )
                {
                    continue;
                }

                Unload( Path.GetFileNameWithoutExtension( file ) );
            }
        }

        public void Load( string path )
        {
            string file = path.Replace( "\\" , "/" );

            if ( !File.Exists( file ) )
            {
                return;
            }

            string data = File.ReadAllText( file );

            Load( Encoding.UTF8.GetBytes( data ) ,
                Path.GetFileNameWithoutExtension( file ) );
        }

        public void Load( byte[] chunk , string name )
        {
            Debug.Log( "Load chunk " + name );

            LuaTable scriptEnv = luaEnv.Global.Get<LuaTable>( name );

            if ( scriptEnv != null )
            {
                // can't check disposed or removed. xLua not support.
                Debug.LogWarning( "Load chunk again name=" + name );
            }

            scriptEnv = luaEnv.NewTable();

            LuaTable meta = luaEnv.NewTable();
            meta.Set( "__index" , luaEnv.Global );
            scriptEnv.SetMetaTable( meta );
            meta.Dispose();

            luaEnv.DoString( chunk , name , scriptEnv );
            luaEnv.Global.Set( name , scriptEnv );

            Action action = scriptEnv.Get<Action>( "Awake" );
            if ( action != null )
            {
                action();
            }
        }

        public void Unload( string name )
        {
            LuaTable scriptEnv = luaEnv.Global.Get<LuaTable>( name );

            if ( scriptEnv != null )
            {
                Action action = scriptEnv.Get<Action>( "OnDestroy" );
                if ( action != null )
                {
                    action();
                }

                scriptEnv.Dispose();
                scriptEnv = null;
                luaEnv.Global.Set( name , scriptEnv );
            }

        }


    }

}
