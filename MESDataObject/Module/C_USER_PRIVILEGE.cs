using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_USER_PRIVILEGE : DataObjectTable
    {
        public T_C_USER_PRIVILEGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_USER_PRIVILEGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_USER_PRIVILEGE);
            TableName = "C_USER_PRIVILEGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_C_USER_PRIVILEGE getC_PrivilegebyID(string id, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_USER_PRIVILEGE where PRIVILEGE_ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_USER_PRIVILEGE ret = (Row_C_USER_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public Row_C_USER_PRIVILEGE getC_PrivilegebyIDemp(string id,string emp, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_USER_PRIVILEGE a,c_user b where a.PRIVILEGE_ID='{id}' and EMP_NO='{emp}' and A.USER_ID=B.ID ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_USER_PRIVILEGE ret = (Row_C_USER_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }
    }
    public class Row_C_USER_PRIVILEGE : DataObjectBase
    {
        public Row_C_USER_PRIVILEGE(DataObjectInfo info) : base(info)
        {

        }
        public C_USER_PRIVILEGE GetDataObject()
        {
            C_USER_PRIVILEGE DataObject = new C_USER_PRIVILEGE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.USER_ID = this.USER_ID;
            DataObject.PRIVILEGE_ID = this.PRIVILEGE_ID;
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
        public string SYSTEM_NAME
        {
            get

            {
                return (string)this["SYSTEM_NAME"];
            }
            set
            {
                this["SYSTEM_NAME"] = value;
            }
        }
        public string USER_ID
        {
            get

            {
                return (string)this["USER_ID"];
            }
            set
            {
                this["USER_ID"] = value;
            }
        }
        public string PRIVILEGE_ID
        {
            get

            {
                return (string)this["PRIVILEGE_ID"];
            }
            set
            {
                this["PRIVILEGE_ID"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get

            {
                return (DateTime)this["EDIT_TIME"];
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
        public class C_USER_PRIVILEGE
        {
            public string ID;
            public string SYSTEM_NAME;
            public string USER_ID;
            public string PRIVILEGE_ID;
            public DateTime EDIT_TIME;
            public string EDIT_EMP;
        }
    }
}