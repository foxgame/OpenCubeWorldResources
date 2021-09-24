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
    public class DataManager : Singleton<DataManager>
    {

        Dictionary<string , DataTable> dictionary = new Dictionary<string , DataTable>();

        public void Clear()
        {
            dictionary.Clear();
        }

        public void Save( string name )
        {
            string path = Utility.DataPath + name + "/";

            Directory.CreateDirectory( path );

            foreach ( KeyValuePair<string , DataTable> keyValuePair in dictionary )
            {
                keyValuePair.Value.Save( path + keyValuePair.Key );
            }
        }

        public void Load( string name )
        {
            string path = Utility.DataPath + name + "/";

            foreach ( KeyValuePair<string , DataTable> keyValuePair in dictionary )
            {
                keyValuePair.Value.Load( path + keyValuePair.Key );
            }
        }

        public DataTable GetData( string name )
        {
            DataTable dataTable = null;

            if ( dictionary.TryGetValue( name , out dataTable ) )
            {
                return dataTable;
            }

            return null;
        }

        public DataTable CreateData( string name )
        {
            DataTable dataTable = null;

            if ( dictionary.TryGetValue( name , out dataTable ) )
            {
                return dataTable;
            }

            dataTable = new DataTable();

            return dataTable;
        }

        public DataTable LoadData( string name )
        {
            DataTable dataTable = null;

            if ( dictionary.TryGetValue( name , out dataTable ) )
            {
                return dataTable;
            }

            dataTable = new DataTable();

            string path = Utility.AssetsPath + Utility.ModsPath + name;

            if ( !dataTable.Load( path , true , false ) )
            {
                return null;
            }

            return dataTable;
        }



    }

}
