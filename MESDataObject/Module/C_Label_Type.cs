using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_Label_Type : DataObjectTable
    {
        public T_C_Label_Type(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_Label_Type(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_Label_Type);
            TableName = "C_Label_Type".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public Row_C_Label_Type GetConfigByName(string Name, OleExec DB)
        {
            string strSql = $@"select * from  C_LABEL_TYPE where name = '{Name}'";
            DataSet res = DB.RunSelect(strSql);

            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_Label_Type R = (Row_C_Label_Type)NewRow();
                R.loadData(res.Tables[0].Rows[i]);
                return R;
            }


            return null;
        }

    }
    public class Row_C_Label_Type : DataObjectBase
    {
        public Row_C_Label_Type(DataObjectInfo info) : base(info)
        {

        }
        public C_Label_Type GetDataObject()
        {
            C_Label_Type DataObject = new C_Label_Type();
            DataObject.ID = this.ID;
            DataObject.NAME = this.NAME;
            DataObject.DLL = this.DLL;
            DataObject.CLASS = this.CLASS;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string DLL
        {
            get
            {
                return (string)this["DLL"];
            }
            set
            {
                this["DLL"] = value;
            }
        }
        public string CLASS
        {
            get
            {
                return (string)this["CLASS"];
            }
            set
            {
                this["CLASS"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
    }
    public class C_Label_Type
    {
        public string ID;
        public string NAME;
        public string DLL;
        public string CLASS;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}