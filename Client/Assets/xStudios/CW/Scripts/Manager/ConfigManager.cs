using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System;
using X;
using XLua;

namespace X
{
    [LuaCallCSharp]
    public class ConfigManager : Singleton<ConfigManager>
    {

        Dictionary<string , ConfigTable> dictionary = new Dictionary<string , ConfigTable>();

        public void Clear()
        {
            dictionary.Clear();
        }

        public ConfigTable GetConfig( string name )
        {
            ConfigTable configTable = null;

            if ( dictionary.TryGetValue( name , out configTable ) )
            {
                return configTable;
            }

            return null;
        }

        public ConfigTable LoadConfig( string name )
        {
            ConfigTable configTable = null;

            if ( dictionary.TryGetValue( name , out configTable ) )
            {
                return configTable;
            }

            configTable = new ConfigTable();

            string path = Utility.AssetsPath + Utility.ModsPath + name;

            if ( !configTable.Load( path ) )
            {
                return null;
            }

            dictionary.Add( name , configTable );

            return configTable;
        }



    }

}
