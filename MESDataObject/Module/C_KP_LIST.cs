using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_LIST : DataObjectTable
    {
        public T_C_KP_LIST(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public bool CheckKPListName(string KPListName, OleExec DB)
        {
            string strSql = $@"select count(1) from c_kp_list where LISTNAME='{KPListName}'";
            string strRet = DB.ExecSelectOneValue(strSql).ToString();
            try
            {
                if (Int32.Parse(strRet) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ee)
            {
                throw new Exception(strSql +":" +strRet);
            }


        }

        public T_C_KP_LIST(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_LIST);
            TableName = "C_KP_LIST".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<string> GetListIDBySkuno(string Skuno, OleExec DB)
        {
            List<string> ret = new List<string>();
            string strSql = $@"select ID from c_kp_list where skuno='{Skuno}'";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            }
            return ret;
        }

        public List<string> GetItemID(string ID, OleExec DB)
        {
            List<string> ret = new List<string>();
            string strSql = $@"select ID from c_kp_list_item c where c.list_id='{ID}' order by c.seq";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            }
            return ret;
        }

        public bool KpIDIsExist(string kpID, OleExec sfcdb)
        {
            string strSql = $@"select ID from c_kp_list where id='{kpID}'";
            DataSet ds = sfcdb.RunSelect(strSql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Row_C_KP_LIST : DataObjectBase
    {
        public Row_C_KP_LIST(DataObjectInfo info) : base(info)
        {

        }

        public DataTable GetBySkuNO(string Skuno, OleExec sfcdb)
        {
            string strSql = "";
            throw new Exception();
            
        }

        public C_KP_LIST GetDataObject()
        {
            C_KP_LIST DataObject = new C_KP_LIST();
            DataObject.ID = this.ID;
            DataObject.LISTNAME = this.LISTNAME;
            DataObject.SKUNO = this.SKUNO;
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
        public string LISTNAME
        {
            get
            {
                return (string)this["LISTNAME"];
            }
            set
            {
                this["LISTNAME"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
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
    public class C_KP_LIST
    {
        public string ID;
        public string LISTNAME;
        public string SKUNO;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}