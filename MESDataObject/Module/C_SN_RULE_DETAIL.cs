using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SN_RULE_DETAIL : DataObjectTable
    {
        public T_C_SN_RULE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SN_RULE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SN_RULE_DETAIL);
            TableName = "C_SN_RULE_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<Row_C_SN_RULE_DETAIL> GetDataByRuleID(string RuleID, OleExec DB)
        {
            List<Row_C_SN_RULE_DETAIL> RET = null;
            string strSql = $@"select * from C_SN_RULE_DETAIL c where c.c_sn_rule_id = '{RuleID}' order by seq ";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                if (i == 0)
                {
                    RET = new List<Row_C_SN_RULE_DETAIL>();
                }
                Row_C_SN_RULE_DETAIL R = (Row_C_SN_RULE_DETAIL)NewRow();
                R.loadData(res.Tables[0].Rows[i]);
                RET.Add(R);
            }

            return RET;

        }
    }
    public class Row_C_SN_RULE_DETAIL : DataObjectBase
    {
        public Row_C_SN_RULE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public void LockMe(OleExec DB)
        {
            string strSql = $@"select * from C_SN_RULE_DETAIL where ID = {ID} for update";
            DataSet res = DB.RunSelect(strSql);
            loadData(res.Tables[0].Rows[0]);
        }

        public C_SN_RULE_DETAIL GetDataObject()
        {
            C_SN_RULE_DETAIL DataObject = new C_SN_RULE_DETAIL();
            DataObject.VALUE10 = this.VALUE10;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CHECK_FLAG = this.CHECK_FLAG;
            DataObject.RESETVALUE = this.RESETVALUE;
            DataObject.RESETSN_FLAG = this.RESETSN_FLAG;
            DataObject.CURVALUE = this.CURVALUE;
            DataObject.CODETYPE = this.CODETYPE;
            DataObject.INPUTTYPE = this.INPUTTYPE;
            DataObject.SEQ = this.SEQ;
            DataObject.C_SN_RULE_ID = this.C_SN_RULE_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string VALUE10
        {
            get
            {
                return (string)this["VALUE10"];
            }
            set
            {
                this["VALUE10"] = value;
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
        public double? CHECK_FLAG
        {
            get
            {
                return (double?)this["CHECK_FLAG"];
            }
            set
            {
                this["CHECK_FLAG"] = value;
            }
        }
        public string RESETVALUE
        {
            get
            {
                return (string)this["RESETVALUE"];
            }
            set
            {
                this["RESETVALUE"] = value;
            }
        }
        public double? RESETSN_FLAG
        {
            get
            {
                return (double?)this["RESETSN_FLAG"];
            }
            set
            {
                this["RESETSN_FLAG"] = value;
            }
        }
        public string CURVALUE
        {
            get
            {
                return (string)this["CURVALUE"];
            }
            set
            {
                this["CURVALUE"] = value;
            }
        }
        public string CODETYPE
        {
            get
            {
                return (string)this["CODETYPE"];
            }
            set
            {
                this["CODETYPE"] = value;
            }
        }
        public string INPUTTYPE
        {
            get
            {
                return (string)this["INPUTTYPE"];
            }
            set
            {
                this["INPUTTYPE"] = value;
            }
        }
        public double? SEQ
        {
            get
            {
                return (double?)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
        public string C_SN_RULE_ID
        {
            get
            {
                return (string)this["C_SN_RULE_ID"];
            }
            set
            {
                this["C_SN_RULE_ID"] = value;
            }
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
    }
    public class C_SN_RULE_DETAIL
    {
        public string VALUE10;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
        public double? CHECK_FLAG;
        public string RESETVALUE;
        public double? RESETSN_FLAG;
        public string CURVALUE;
        public string CODETYPE;
        public string INPUTTYPE;
        public double? SEQ;
        public string C_SN_RULE_ID;
        public string ID;
    }
}