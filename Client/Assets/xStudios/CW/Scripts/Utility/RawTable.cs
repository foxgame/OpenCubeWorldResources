using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using XLua;
using X;

namespace X
{
    [LuaCallCSharp]
    public class RawTable
    {
        [Serializable]
        public class Data
        {
            public string name = "";
            public string version = "";

            public string primaryKey = "";
            public int primaryKeyIndex = Utility.InvalidID;

            public int headIndex = Utility.InvalidID;
            public int scriptIndex = Utility.InvalidID;
            public int formatIndex = Utility.InvalidID;
            public int typeIndex = Utility.InvalidID;
            public int keyIndex = Utility.InvalidID;
            public int defaultIndex = Utility.InvalidID;

            public List<List<string>> dataTable = new List<List<string>>();
            public List<List<object>> data = new List<List<object>>();
        }

        public Data data = new Data();

        Dictionary<string , int> keyIndexDictionary = new Dictionary<string , int>();
        Dictionary<string , List<object>> dataDictionary = new Dictionary<string , List<object>>();

        public Dictionary<string , int> KeyIndexDictionary { get { return keyIndexDictionary; } }

        public void Clear()
        {
            data.name = "";
            data.version = "";
            data.primaryKey = "";
            data.primaryKeyIndex = Utility.InvalidID;
            data.headIndex = Utility.InvalidID;
            data.scriptIndex = Utility.InvalidID;
            data.formatIndex = Utility.InvalidID;
            data.typeIndex = Utility.InvalidID;
            data.keyIndex = Utility.InvalidID;
            data.defaultIndex = Utility.InvalidID;
            data.data.Clear();
            data.dataTable.Clear();

            dataDictionary.Clear();
            keyIndexDictionary.Clear();
        }

        public void ClearData()
        {
            data.data.Clear();
            dataDictionary.Clear();
        }

        public void CreateTable( ref string[] head ,
            ref string[] script , ref string[] format ,
            ref string[] type , ref string[] key ,
            ref string[] d )
        {
            if ( data.dataTable.Count == 0 )
            {
                data.headIndex = 0;
                data.scriptIndex = 1;
                data.formatIndex = 2;
                data.typeIndex = 3;
                data.keyIndex = 4;
                data.defaultIndex = 5;
            }

            data.dataTable.Add( new List<string>( head ) );
            data.dataTable.Add( new List<string>( script ) );
            data.dataTable.Add( new List<string>( format ) );
            data.dataTable.Add( new List<string>( type ) );
            data.dataTable.Add( new List<string>( key ) );
            data.dataTable.Add( new List<string>( d ) );

            keyIndexDictionary.Clear();

            for ( int i = 0 ; i < key.Length ; i++ )
            {
                if ( keyIndexDictionary.ContainsKey( key[ i ] ) )
                {
                    Debug.LogError( "Load config columns contains " + key[ i ] );
                }
                else
                {
                    keyIndexDictionary.Add( key[ i ] , i );
                }
            }
        }

        public void AddRow( List<object> r )
        {
            data.data.Add( r );

            if ( data.primaryKeyIndex != Utility.InvalidID )
            {
                string key = r[ data.primaryKeyIndex ].ToString();

                if ( dataDictionary.ContainsKey( key ) )
                {
                    Debug.LogWarning( "addRow warning containsKey" + key );
                    dataDictionary[ key ] = r;
                }
                else
                {
                    dataDictionary.Add( key , r );
                }
            }
        }

        public void RemoveRow( int i )
        {
            if ( data.data.Count > i )
            {
                data.data.RemoveAt( i );
            }
        }

        public object GetValue( string value , string type , string format )
        {
            string split = "";
            if ( string.IsNullOrEmpty( format ) )
            {
                split = ",";
            }
            else
            {
                split += format[ format.Length - 1 ];
            }

            if ( type == "Int" )
            {
                int n = 0;
                Utility.ParseInt( value , out n );
                return n;
            }
            else if ( type == "Float" )
            {
                float f = 0f;
                Utility.ParseFloat( value , out f );
                return f;
            }
            else if ( type == "Bool" )
            {
                bool b = false;

                if ( string.IsNullOrEmpty( value ) )
                {
                    return b;
                }

                b = value.ToLower() == "true";

                return b;
            }
            else if ( type == "String" )
            {
                if ( string.IsNullOrEmpty( value ) )
                {
                    return "";
                }

                return value;
            }
            else if ( type == "IntList" )
            {
                IntList l = new IntList();
                l.Init( value , split );
                return l;
            }
            else if ( type == "FloatList" )
            {
                FloatList l = new FloatList();
                l.Init( value , split );
                return l;
            }
            else if ( type == "StringList" )
            {
                StringList l = new StringList();
                l.Init( value , split );
                return l;
            }

            return null;
        }


        public List<object> NewRow()
        {
            List<object> row = DefaultValue();

            data.data.Add( row );

            return row;
        }

        public List<object> DefaultValue()
        {
            List<string> defaults = data.dataTable[ data.defaultIndex ];
            List<string> type = data.dataTable[ data.typeIndex ];
            List<string> format = data.dataTable[ data.formatIndex ];
            List<object> row = new List<object>();

            for ( int i = 0 ; i < defaults.Count ; i++ )
            {
                row.Add( GetValue( defaults[ i ] , type[ i ] , format[ i ] ) );
            }

            return row;
        }

        public List<object> GetRow( int i )
        {
            if ( data.data.Count > i )
            {
                return data.data[ i ];
            }

            return null;
        }

        public object GetRowValue( int i , string key )
        {
            if ( data.data.Count > i )
            {
                List<object> dataRow = data.data[ i ];

                int index = GetKeyIndex( key );

                if ( index == Utility.InvalidID )
                {
                    return null;
                }

                return dataRow[ index ];
            }

            return null;
        }

        public void SetRow( int i , List<object> r )
        {
            if ( data.data.Count > i )
            {
                data.data[ i ] = r;
            }
        }

        public void SetRowValue( int i , string key , object value )
        {
            if ( data.data.Count > i )
            {
                List<object> dataRow = data.data[ i ];

                int index = GetKeyIndex( key );

                if ( index == Utility.InvalidID )
                {
                    return;
                }

                dataRow[ index ] = value;
            }
            else
            {
                Debug.LogError( "SetRowData Error i " + i + " key " + key );
            }
        }

        public List<object> GetData( string primaryKey )
        {
            List<object> row = null;

            if ( dataDictionary.TryGetValue( primaryKey , out row ) )
            {
                return row;
            }

            return null;
        }

        public object GetDataValue( string primaryKey , string key )
        {
            List<object> row = null;

            if ( dataDictionary.TryGetValue( primaryKey , out row ) )
            {
                int index = GetKeyIndex( key );

                if ( index == Utility.InvalidID )
                {
                    return null;
                }

                return row[ index ];
            }

            return null;
        }

        public void SetData( string primaryKey , List<object> value )
        {
            if ( !dataDictionary.ContainsKey( primaryKey ) )
            {
                Debug.LogError( "SetData Error " + primaryKey );
                return;
            }

            dataDictionary[ primaryKey ] = value;
        }

        public void SetDataValue( string primaryKey , string key , object value )
        {
            List<object> row = null;

            if ( dataDictionary.TryGetValue( primaryKey , out row ) )
            {
                int index = GetKeyIndex( key );

                if ( index == Utility.InvalidID )
                {
                    Debug.LogError( "SetData Error " + key );
                    return;
                }

                row[ index ] = value;
            }
            else
            {
                Debug.LogError( "SetData Error " + key );
            }
        }

        public int GetKeyIndex( string key )
        {
            int n = Utility.InvalidID;

            if ( keyIndexDictionary.TryGetValue( key , out n ) )
            {
                return n;
            }

            return Utility.InvalidID;
        }

        public void SetKeyIndex( string key , int n )
        {
            keyIndexDictionary[ key ] = n;
        }

        public void Save( string path )
        {
            FileStream fs = null;
            BinaryWriter bf = null;

            try
            {
                fs = File.Open( path , FileMode.Create , FileAccess.ReadWrite , FileShare.ReadWrite );
                bf = new BinaryWriter( fs );

                bf.Write( "XDAT" );
                bf.Write( 1 );

                bf.Write( data.name );
                bf.Write( data.version );
                bf.Write( data.primaryKey );
                bf.Write( data.primaryKeyIndex );

                bf.Write( data.headIndex );
                bf.Write( data.scriptIndex );
                bf.Write( data.formatIndex );
                bf.Write( data.typeIndex );
                bf.Write( data.keyIndex );
                bf.Write( data.defaultIndex );

                bf.Write( data.dataTable.Count );
                bf.Write( data.data.Count );

                for ( int i = 0 ; i < data.dataTable.Count ; i++ )
                {
                    bf.Write( data.dataTable[ i ].Count );
                    for ( int j = 0 ; j < data.dataTable[ i ].Count ; j++ )
                    {
                        bf.Write( data.dataTable[ i ][ j ] );
                    }
                }

                for ( int i = 0 ; i < data.data.Count ; i++ )
                {
                    bf.Write( data.data[ i ].Count );
                    for ( int j = 0 ; j < data.data[ i ].Count ; j++ )
                    {
                        bf.Write( data.data[ i ][ j ].ToString() );
                    }
                }

                bf.Close();
                bf.Dispose();

                Debug.Log( "save dat " + path );
            }
            catch ( Exception e )
            {
                Debug.LogError( "save dat error. " + path + e.Message );
            }
            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                    fs.Dispose();
                }
                if ( bf != null )
                {
                    bf.Close();
                    bf.Dispose();
                }
            }
        }

        public bool Load( string path , bool isTable , bool isData )
        {
            if ( !File.Exists( path ) )
            {
                Debug.LogError( "Load config error " + path );
                return false;
            }

            FileStream fs = null;
            BinaryReader br = null;

            if ( isTable )
            {
                Clear();
            }

            if ( isData )
            {
                ClearData();
            }

            bool error = false;

            try
            {
                fs = File.Open( path , FileMode.Open , FileAccess.ReadWrite , FileShare.ReadWrite );
                br = new BinaryReader( fs );
                string magic = br.ReadString();
                int version = br.ReadInt32();

                if ( version >= 1 )
                {
                    string dataName = br.ReadString();
                    string dataVersion = br.ReadString();
                    string dataPrimaryKey = br.ReadString();

                    int dataPrimaryKeyIndex = br.ReadInt32();
                    int dataHeadIndex = br.ReadInt32();
                    int dataScriptIndex = br.ReadInt32();
                    int dataFormatIndex = br.ReadInt32();
                    int dataTypeIndex = br.ReadInt32();
                    int dataKeyIndex = br.ReadInt32();
                    int dataDefaultIndex = br.ReadInt32();

                    if ( isTable )
                    {
                        data.name = dataName;
                        data.version = dataVersion;
                        data.primaryKey = dataPrimaryKey;

                        data.primaryKeyIndex = dataPrimaryKeyIndex;

                        data.headIndex = dataHeadIndex;
                        data.scriptIndex = dataScriptIndex;
                        data.formatIndex = dataFormatIndex;
                        data.typeIndex = dataTypeIndex;
                        data.keyIndex = dataKeyIndex;
                        data.defaultIndex = dataDefaultIndex;
                    }

                    int tableCount = br.ReadInt32();
                    int dataCount = br.ReadInt32();

                    if ( isTable )
                    {
                        for ( int i = 0 ; i < tableCount ; i++ )
                        {
                            int length = br.ReadInt32();
                            List<string> d = new List<string>();

                            for ( int j = 0 ; j < length ; j++ )
                                d.Add( br.ReadString() );

                            data.dataTable.Add( d );
                        }

                        List<string> keys = data.dataTable[ data.keyIndex ];

                        for ( int i = 0 ; i < keys.Count ; i++ )
                        {
                            if ( keyIndexDictionary.ContainsKey( keys[ i ] ) )
                            {
#if UNITY_EDITOR
                                Debug.LogWarning( "Load config columns contains " + keys[ i ] );
#endif
                            }
                            else
                            {
                                keyIndexDictionary.Add( keys[ i ] , i );
                            }
                        }
                    }
                    else
                    {
                        for ( int i = 0 ; i < tableCount ; i++ )
                        {
                            int length = br.ReadInt32();

                            for ( int j = 0 ; j < length ; j++ )
                                br.ReadString();
                        }
                    }

                    if ( isData )
                    {
                        List<string> keys = data.dataTable[ data.keyIndex ];
                        List<string> types = data.dataTable[ data.typeIndex ];
                        List<string> formats = data.dataTable[ data.formatIndex ];
                        List<string> defaults = null;

                        if ( data.defaultIndex != Utility.InvalidID )
                            defaults = data.dataTable[ data.defaultIndex ];

                        for ( int i = 0 ; i < dataCount ; i++ )
                        {
                            object[] d = new object[ keys.Count ];

                            if ( defaults != null )
                            {
                                for ( int j = 0 ; j < d.Length ; j++ )
                                {
                                    d[ j ] = GetValue( defaults[ j ] , types[ j ] , formats[ j ] );
                                }
                            }

                            int length = br.ReadInt32();

                            for ( int j = 0 ; j < length ; j++ )
                            {
                                string value = br.ReadString();

                                string key = keys[ j ];
                                int index = 0;

                                if ( keyIndexDictionary.TryGetValue( key , out index ) )
                                {
                                    d[ index ] = GetValue( value , types[ index ] , formats[ index ] );
                                }
                                else
                                {
#if UNITY_EDITOR
                                    Debug.LogWarning( "key not found key = " + key + " path = " + path );
#endif
                                }
                            }

                            data.data.Add( new List<object>( d ) );
                        }

                        if ( data.primaryKeyIndex != Utility.InvalidID )
                        {
                            for ( int i = 0 ; i < data.data.Count ; i++ )
                            {
                                List<object> row = data.data[ i ];
                                string key = row[ data.primaryKeyIndex ].ToString();

                                if ( dataDictionary.ContainsKey( key ) )
                                {
#if UNITY_EDITOR
                                    Debug.LogWarning( "addRow warning containsKey" + key );
#endif
                                    dataDictionary[ key ] = row;
                                }
                                else
                                {
                                    dataDictionary.Add( key , row );
                                }
                            }
                        }
                    }
                }
            }
            catch ( Exception e )
            {
                Debug.LogError( e.Message );
                error = true;
            }
            finally
            {
                if ( fs != null )
                {
                    fs.Close();
                    fs.Dispose();
                }
                if ( br != null )
                {
                    br.Close();
                    br.Dispose();
                }
            }

            if ( error )
            {
                Debug.LogError( "Load config error " + path );
                return false;
            }

            return true;
        }


    }

}

