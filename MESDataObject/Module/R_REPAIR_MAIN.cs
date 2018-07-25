using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_MAIN : DataObjectTable
    {
        public T_R_REPAIR_MAIN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_MAIN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_MAIN);
            TableName = "R_REPAIR_MAIN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_REPAIR_MAIN> GetRepairMainBySN(OleExec sfcdb, string sn)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_REPAIR_MAIN row_main = null;
            List<R_REPAIR_MAIN> mains = new List<R_REPAIR_MAIN>();
            string sql = $@"select * from {TableName} where sn='{sn.Replace("'", "''")}'";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_R_REPAIR_MAIN) this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
                
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public int ReplaceSnRepairFailMain(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_REPAIR_MAIN R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            
            return result;
        }


        public int GetRepairedCount(string sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;
            DataTable dt = new DataTable();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"select * from r_repair_main where sn='{sn}' and closed_flag='1'";
                dt = DB.ExecSelect(strSql).Tables[0];
                result = dt.Rows.Count;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }
    }
    public class Row_R_REPAIR_MAIN : DataObjectBase
    {
        public Row_R_REPAIR_MAIN(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_MAIN GetDataObject()
        {
            R_REPAIR_MAIN DataObject = new R_REPAIR_MAIN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.FAIL_LINE = this.FAIL_LINE;
            DataObject.FAIL_STATION = this.FAIL_STATION;
            DataObject.FAIL_DEVICE = this.FAIL_DEVICE;
            DataObject.FAIL_EMP = this.FAIL_EMP;
            DataObject.FAIL_TIME = this.FAIL_TIME;
            DataObject.DISTRIBUTION_EMP = this.DISTRIBUTION_EMP;
            DataObject.DISTRIBUTION_TIME = this.DISTRIBUTION_TIME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
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
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
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
        public string FAIL_LINE
        {
            get
            {
                return (string)this["FAIL_LINE"];
            }
            set
            {
                this["FAIL_LINE"] = value;
            }
        }
        public string FAIL_STATION
        {
            get
            {
                return (string)this["FAIL_STATION"];
            }
            set
            {
                this["FAIL_STATION"] = value;
            }
        }
        public string FAIL_DEVICE
        {
            get
            {
                return (string)this["FAIL_DEVICE"];
            }
            set
            {
                this["FAIL_DEVICE"] = value;
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
        public string DISTRIBUTION_EMP
        {
            get
            {
                return (string)this["DISTRIBUTION_EMP"];
            }
            set
            {
                this["DISTRIBUTION_EMP"] = value;
            }
        }
        public DateTime? DISTRIBUTION_TIME
        {
            get
            {
                return (DateTime?)this["DISTRIBUTION_TIME"];
            }
            set
            {
                this["DISTRIBUTION_TIME"] = value;
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
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
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
    public class R_REPAIR_MAIN
    {
        public string ID;
        public string SN;
        public string WORKORDERNO;
        public string SKUNO;
        public string FAIL_LINE;
        public string FAIL_STATION;
        public string FAIL_DEVICE;
        public string FAIL_EMP;
        public DateTime? FAIL_TIME;
        public string DISTRIBUTION_EMP;
        public DateTime? DISTRIBUTION_TIME;
        public DateTime? CREATE_TIME;
        public string CLOSED_FLAG;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}