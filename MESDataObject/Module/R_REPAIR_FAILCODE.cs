using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_FAILCODE : DataObjectTable
    {
        public T_R_REPAIR_FAILCODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_FAILCODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_FAILCODE);
            TableName = "R_REPAIR_FAILCODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_REPAIR_FAILCODE> GetFailCodeBySN(OleExec sfcdb, string sn)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_REPAIR_FAILCODE row_fail = null;
            List<R_REPAIR_FAILCODE> repairFailCodes = new List<R_REPAIR_FAILCODE>();
            string sql = $@"select * from {TableName} where sn='{sn.Replace("'", "''")}' ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_fail = (Row_R_REPAIR_FAILCODE) this.NewRow();
                        row_fail.loadData(dr);
                        repairFailCodes.Add(row_fail.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return repairFailCodes;
        }

        public Row_R_REPAIR_FAILCODE GetByFailCodeID(string _FailCodeID, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select * from r_repair_failcode where ID='{_FailCodeID.Replace("'", "''")}' and REPAIR_FLAG='0' ";
                DataSet res = DB.ExecSelect(strsql);
                if (res.Tables[0].Rows.Count>0)
                {
                    Row_R_REPAIR_FAILCODE Ret = (Row_R_REPAIR_FAILCODE)this.GetObjByID(_FailCodeID, DB);
                    return Ret;
                }
                else
                {
                    return null;
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "FailCodeID:" + _FailCodeID });
                    //    throw new MESReturnMessage(errMsg);
                }                
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }


        public List<R_REPAIR_FAILCODE> CheckSNRepairFinishAction(OleExec sfcdb, string sn,string RepairMainID)
        {
            if (string.IsNullOrEmpty(sn)||string.IsNullOrEmpty(RepairMainID))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_REPAIR_FAILCODE row_fail = null;
            List<R_REPAIR_FAILCODE> repairFailCodes = new List<R_REPAIR_FAILCODE>();
            string sql = $@" select *from r_repair_failcode where  repair_main_id ='{RepairMainID}' and sn ='{sn}' and id not in (select repair_failcode_id from r_repair_action where sn='{sn}')";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_fail = (Row_R_REPAIR_FAILCODE)this.NewRow();
                        row_fail.loadData(dr);
                        repairFailCodes.Add(row_fail.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return repairFailCodes;
        }

        public DataTable SelectFailCodeBySN(string sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from r_repair_failcode where sn='{sn}' ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;

        }

        public int ReplaceSnRepairFailCode(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;


            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_REPAIR_FAILCODE R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            

            return result;
        }
    }
    public class Row_R_REPAIR_FAILCODE : DataObjectBase
    {
        public Row_R_REPAIR_FAILCODE(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_FAILCODE GetDataObject()
        {
            R_REPAIR_FAILCODE DataObject = new R_REPAIR_FAILCODE();
            DataObject.ID = this.ID;
            DataObject.REPAIR_MAIN_ID = this.REPAIR_MAIN_ID;
            DataObject.SN = this.SN;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.FAIL_LOCATION = this.FAIL_LOCATION;
            DataObject.FAIL_CATEGORY = this.FAIL_CATEGORY;
            DataObject.FAIL_PROCESS = this.FAIL_PROCESS;
            DataObject.FAIL_TIME = this.FAIL_TIME;
            DataObject.FAIL_EMP = this.FAIL_EMP;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.REPAIR_FLAG = this.REPAIR_FLAG;
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
        public string REPAIR_MAIN_ID
        {
            get
            {
                return (string)this["REPAIR_MAIN_ID"];
            }
            set
            {
                this["REPAIR_MAIN_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string FAIL_CODE
        {
            get
            {
                return (string)this["FAIL_CODE"];
            }
            set
            {
                this["FAIL_CODE"] = value;
            }
        }
        public string FAIL_LOCATION
        {
            get
            {
                return (string)this["FAIL_LOCATION"];
            }
            set
            {
                this["FAIL_LOCATION"] = value;
            }
        }
        public string FAIL_CATEGORY
        {
            get
            {
                return (string)this["FAIL_CATEGORY"];
            }
            set
            {
                this["FAIL_CATEGORY"] = value;
            }
        }
        public string FAIL_PROCESS
        {
            get
            {
                return (string)this["FAIL_PROCESS"];
            }
            set
            {
                this["FAIL_PROCESS"] = value;
            }
        }
        public DateTime? FAIL_TIME
        {
            get
            {
                return (DateTime?)this["FAIL_TIME"];
            }
            set
            {
                this["FAIL_TIME"] = value;
            }
        }
        public string FAIL_EMP
        {
            get
            {
                return (string)this["FAIL_EMP"];
            }
            set
            {
                this["FAIL_EMP"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public string REPAIR_FLAG
        {
            get
            {
                return (string)this["REPAIR_FLAG"];
            }
            set
            {
                this["REPAIR_FLAG"] = value;
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
    public class R_REPAIR_FAILCODE
    {
        public string ID;
        public string REPAIR_MAIN_ID;
        public string SN;
        public string FAIL_CODE;
        public string FAIL_LOCATION;
        public string FAIL_CATEGORY;
        public string FAIL_PROCESS;
        public DateTime? FAIL_TIME;
        public string FAIL_EMP;
        public string DESCRIPTION;
        public DateTime? CREATE_TIME;
        public string REPAIR_FLAG;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}