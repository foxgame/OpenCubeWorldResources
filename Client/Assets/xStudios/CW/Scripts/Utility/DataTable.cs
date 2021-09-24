using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using X;

namespace X
{
    [LuaCallCSharp]
    public class DataTable
    {
        RawTable rawTable = new RawTable();

        public List<List<string>> Table { get { return rawTable.data.dataTable; } }
        public List<List<object>> Data { get { return rawTable.data.data; } }

        public string Name { get { return rawTable.data.name; } }
        public string Version { get { return rawTable.data.version; } }
        public string PrimaryKey { get { return rawTable.data.primaryKey; } }

        public List<string> Head { get { return rawTable.data.dataTable[ rawTable.data.headIndex ]; } }
        public List<string> Key { get { return rawTable.data.dataTable[ rawTable.data.keyIndex ]; } }
        public List<string> Script { get { return rawTable.data.dataTable[ rawTable.data.scriptIndex ]; } }
        public List<string> Format { get { return rawTable.data.dataTable[ rawTable.data.formatIndex ]; } }
        public List<string> Type { get { return rawTable.data.dataTable[ rawTable.data.typeIndex ]; } }
        public List<string> Default { get { return rawTable.data.dataTable[ rawTable.data.defaultIndex ]; } }

        public int PrimaryKeyIndex { get { return rawTable.data.primaryKeyIndex; } }
        public int DataIndex { get { return rawTable.data.keyIndex + 1; } }
        public int HeadIndex { get { return rawTable.data.headIndex; } }
        public int ScriptIndex { get { return rawTable.data.scriptIndex; } }
        public int FormatIndex { get { return rawTable.data.formatIndex; } }
        public int TypeIndex { get { return rawTable.data.typeIndex; } }
        public int KeyIndex { get { return rawTable.data.keyIndex; } }
        public int DefaultIndex { get { return rawTable.data.defaultIndex; } }

        public void Clear()
        {
            rawTable.Clear();
        }

        public void ClearData()
        {
            rawTable.ClearData();
        }

        public List<object> DefaultValue()
        {
            return rawTable.DefaultValue();
        }

        public void CreateTable( ref string[] head ,
           ref string[] script , ref string[] format ,
           ref string[] type , ref string[] key ,
           ref string[] d )
        {
            rawTable.CreateTable( ref head , ref script , ref format , ref type , ref key , ref d );
        }

        public List<object> NewRow()
        {
            return rawTable.NewRow();
        }

        public List<object> GetRow( int i )
        {
            return rawTable.GetRow( i );
        }

        public object GetRowValue( int i , string key )
        {
            return rawTable.GetRowValue( i , key );
        }

        public void SetRow( int i , List<object> r )
        {
            rawTable.SetRow( i , r );
        }

        public void SetRowValue( int i , string key , object value )
        {
            rawTable.SetRowValue( i , key , value );
        }

        public List<object> GetData( string primaryKey )
        {
            return rawTable.GetData( primaryKey );
        }

        public object GetDataValue( string primaryKey , string key )
        {
            return rawTable.GetDataValue( primaryKey , key );
        }

        public void SetData( string primaryKey , List<object> value )
        {
            rawTable.SetData( primaryKey , value );
        }

        public void SetDataValue( string primaryKey , string key , object value )
        {
            rawTable.SetDataValue( primaryKey , key , value );
        }

        public int GetKeyIndex( string key )
        {
            return rawTable.GetKeyIndex( key );
        }

        public void Save( string path )
        {
            rawTable.Save( path );
        }

        public bool Load( string path , bool isTable = false , bool isData = true )
        {
            if ( !rawTable.Load( path , isTable , isData ) )
            {
                return false;
            }

            return true;
        }

    }


}

