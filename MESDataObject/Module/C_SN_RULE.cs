using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SN_RULE : DataObjectTable
    {
        public T_C_SN_RULE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }

        public Row_C_SN_RULE GetDataByName(string Name, OleExec DB)
        {
            string strSql = $@"select * from C_SN_RULE where Name='{Name}'";
            DataSet res = DB.RunSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_SN_RULE ret = (Row_C_SN_RULE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }

            return null;
        }

        public List<C_SN_RULE> GetAllData( OleExec DB)
        {
            List<C_SN_RULE> ret = new List<C_SN_RULE>();
            string strSql = $@"select * from C_SN_RULE ";
            DataSet res = DB.RunSelect(strSql);
            for (int i=0;i< res.Tables[0].Rows.Count;i++)
            {
                Row_C_SN_RULE r = (Row_C_SN_RULE)NewRow();
                r.loadData(res.Tables[0].Rows[0]);
                ret.Add(r.GetDataObject());
            }
            return ret;
        }


        public bool CheckSNRule(string SN, string RuleName, OleExec DB)
        {
            int CharPosition = 0;
            char StrChar;
            if (SN.Length != RuleName.Length)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000183", new string[] { RuleName.Length.ToString(), SN.Length.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            foreach (char chr in RuleName)
            {
                CharPosition += 1;
                StrChar = Convert.ToChar(SN.Substring(CharPosition - 1, 1));
                switch (chr)
                {
                    case '#':
                        if (!(StrChar < '9' && StrChar > '0'))
                        {
                            string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000184", new string[] { CharPosition.ToString(), StrChar.ToString() });
                            throw new MESReturnMessage(errMsg);
                        }
                        break;
                    case '!':
                        if (!(StrChar < 'Z' && StrChar > 'A'))
                        {
                            string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000185", new string[] { CharPosition.ToString(), StrChar.ToString() });
                            throw new MESReturnMessage(errMsg);
                        }
                        break;
                    case '*':
                        if (!((StrChar <= '9' && StrChar >= '0') || (StrChar <= 'Z' && StrChar >= 'A') || (StrChar == '.') || (StrChar == '-')))
                        {
                            string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000186", new string[] { CharPosition.ToString(), StrChar.ToString() });
                            throw new MESReturnMessage(errMsg);
                        }
                        break;
                    default:
                        if (chr != StrChar)
                        {
                            string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000187", new string[] { CharPosition.ToString(), StrChar.ToString(), chr.ToString() });
                            throw new MESReturnMessage(errMsg);
                        }
                        break;
                }
            }
            return true;

        }

        public T_C_SN_RULE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SN_RULE);
            TableName = "C_SN_RULE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_SN_RULE : DataObjectBase
    {
        public void LockMe(OleExec DB)
        {
            string strSql = $@"select * from C_SN_RULE where ID = {ID} for update";
            DataSet res = DB.RunSelect(strSql);
            loadData(res.Tables[0].Rows[0]);
        }
        public Row_C_SN_RULE(DataObjectInfo info) : base(info)
        {

        }
        public C_SN_RULE GetDataObject()
        {
            C_SN_RULE DataObject = new C_SN_RULE();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CURVALUE = this.CURVALUE;
            DataObject.NAME = this.NAME;
            DataObject.ID = this.ID;
            return DataObject;
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
    public class C_SN_RULE
    {
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
        public string CURVALUE;
        public string NAME;
        public string ID;
    }
}