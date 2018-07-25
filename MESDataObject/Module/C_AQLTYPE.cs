using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_AQLTYPE : DataObjectTable
    {
        public T_C_AQLTYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_AQLTYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_AQLTYPE);
            TableName = "C_AQLTYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_AQLTYPE> GetAqlBySkuno(string aqltype, OleExec DB)
        {
            List<C_AQLTYPE> aqls = new List<C_AQLTYPE>();
            string sql = string.Empty;
            DataTable dt = new DataTable("Allc_aqltype");
            Row_C_AQLTYPE aqlsRow = (Row_C_AQLTYPE)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" select * from c_aqltype where AQL_TYPE='{aqltype}'  ";
                
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqlsRow.loadData(dr);
                    aqls.Add(aqlsRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }

        public List<C_AQLTYPE> GetAqlTypeBySkuno(string skuno, OleExec DB)
        {
            List<C_AQLTYPE> aqls = new List<C_AQLTYPE>();
            string sql = string.Empty;
            DataTable dt = new DataTable("Allc_aqltype");
            Row_C_AQLTYPE aqlsRow = (Row_C_AQLTYPE)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" select b.* from c_sku a,c_aqltype b where a.aqltype=b.aql_type and a.skuno='{skuno}'  ";

                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqlsRow.loadData(dr);
                    aqls.Add(aqlsRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }

        public List<string> GetAql( OleExec DB)
        {
            List<string> aqls = new List<string>();
            string sql = string.Empty;
            DataTable dt = new DataTable("Allc_aqltype");
            Row_C_AQLTYPE aqlsRow = (Row_C_AQLTYPE)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" select distinct AQL_TYPE from c_aqltype   ";

                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    aqls.Add(dr[0].ToString());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return aqls;
        }

        public Row_C_AQLTYPE GetByAqltype(string _Aqltype, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select ID from C_AQLTYPE where AQL_TYPE='{_Aqltype.Replace("'", "''")}' ";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "AQLTYPE AT C_AQLTYPE:" + _Aqltype });
                    throw new MESReturnMessage(errMsg);
                }
                Row_C_AQLTYPE R = (Row_C_AQLTYPE)this.GetObjByID(ID, DB);
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 獲取SAMPLEQTY
        /// </summary>
        /// <param name="AQLType"></param>
        /// <param name="LotQty"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int GetSampleQty(string AQLType, int LotQty, OleExec DB)
        {
            string StrSql = "";
            int SampleQty = 0;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                StrSql = $@"select case when {LotQty} < sample_qty then {LotQty} else sample_qty end as SAMPLEQTY from 
                     (select * from C_AQLTYPE where LOT_QTY >= {LotQty} order by LOT_QTY) where rownum = 1";
                SampleQty = Convert.ToInt16(DB.ExecSelectOneValue(StrSql)?.ToString());

                return SampleQty;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_C_AQLTYPE : DataObjectBase
    {
        public Row_C_AQLTYPE(DataObjectInfo info) : base(info)
        {

        }
        public C_AQLTYPE GetDataObject()
        {
            C_AQLTYPE DataObject = new C_AQLTYPE();
            DataObject.ID = this.ID;
            DataObject.AQL_TYPE = this.AQL_TYPE;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.GL_LEVEL = this.GL_LEVEL;
            DataObject.SAMPLE_QTY = this.SAMPLE_QTY;
            DataObject.ACCEPT_QTY = this.ACCEPT_QTY;
            DataObject.REJECT_QTY = this.REJECT_QTY;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string AQL_TYPE
        {
            get

            {
                return (string)this["AQL_TYPE"];
            }
            set
            {
                this["AQL_TYPE"] = value;
            }
        }
        public double? LOT_QTY
        {
            get

            {
                return (double?)this["LOT_QTY"];
            }
            set
            {
                this["LOT_QTY"] = value;
            }
        }
        public string GL_LEVEL
        {
            get

            {
                return (string)this["GL_LEVEL"];
            }
            set
            {
                this["GL_LEVEL"] = value;
            }
        }
        public double? SAMPLE_QTY
        {
            get

            {
                return (double?)this["SAMPLE_QTY"];
            }
            set
            {
                this["SAMPLE_QTY"] = value;
            }
        }
        public double? ACCEPT_QTY
        {
            get

            {
                return (double?)this["ACCEPT_QTY"];
            }
            set
            {
                this["ACCEPT_QTY"] = value;
            }
        }
        public double? REJECT_QTY
        {
            get

            {
                return (double?)this["REJECT_QTY"];
            }
            set
            {
                this["REJECT_QTY"] = value;
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
    }
    public class C_AQLTYPE
    {
        public string ID;
        public string AQL_TYPE;
        public double? LOT_QTY;
        public string GL_LEVEL;
        public double? SAMPLE_QTY;
        public double? ACCEPT_QTY;
        public double? REJECT_QTY;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}