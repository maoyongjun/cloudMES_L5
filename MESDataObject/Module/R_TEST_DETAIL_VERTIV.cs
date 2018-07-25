using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TEST_DETAIL_VERTIV : DataObjectTable
    {
        public T_R_TEST_DETAIL_VERTIV(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_DETAIL_VERTIV(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_DETAIL_VERTIV);
            TableName = "R_TEST_DETAIL_VERTIV".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_TEST_DETAIL_VERTIV> GetRTestDetailVertivBySn(OleExec DB, string sn)
        {
            List<R_TEST_DETAIL_VERTIV> res = new List<R_TEST_DETAIL_VERTIV>();
            string sql = $@" select * from R_TEST_DETAIL_VERTIV where SN==:SN ";
            OleDbParameter[] paras = new OleDbParameter[]
            {
                new OleDbParameter("SN",OleDbType.VarChar,100)
            };
            paras[0].Value = sn;
            DataTable dt = DB.ExecSelect(sql, paras).Tables[0];
            foreach (DataRow VARIABLE in dt.Rows)
            {
                Row_R_TEST_DETAIL_VERTIV row = (Row_R_TEST_DETAIL_VERTIV)this.NewRow();
                row.loadData(VARIABLE);
                res.Add(row.GetDataObject());
            }
            return res;
        }

        public DataTable GetDTRTestDetailVertivBySn(OleExec DB, string sn)
        {
            List<R_TEST_DETAIL_VERTIV> res = new List<R_TEST_DETAIL_VERTIV>();
            string sql = $@" select * from R_TEST_DETAIL_VERTIV where SN=:SN ";
            OleDbParameter[] paras = new OleDbParameter[]
            {
                new OleDbParameter("SN",OleDbType.VarChar,100)
            };
            paras[0].Value = sn;
            DataTable dt = DB.ExecSelect(sql, paras).Tables[0];
            return dt;
        }
    }
    public class Row_R_TEST_DETAIL_VERTIV : DataObjectBase
    {
        public Row_R_TEST_DETAIL_VERTIV(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_DETAIL_VERTIV GetDataObject()
        {
            R_TEST_DETAIL_VERTIV DataObject = new R_TEST_DETAIL_VERTIV();
            DataObject.ERROR_CODE = this.ERROR_CODE;
            DataObject.OPERATOR = this.OPERATOR;
            DataObject.CELL = this.CELL;
            DataObject.STATION = this.STATION;
            DataObject.STATE = this.STATE;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SN = this.SN;
            DataObject.R_TEST_RECORD_ID = this.R_TEST_RECORD_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string ERROR_CODE
        {
            get
            {
                return (string)this["ERROR_CODE"];
            }
            set
            {
                this["ERROR_CODE"] = value;
            }
        }
        public string OPERATOR
        {
            get
            {
                return (string)this["OPERATOR"];
            }
            set
            {
                this["OPERATOR"] = value;
            }
        }
        public string CELL
        {
            get
            {
                return (string)this["CELL"];
            }
            set
            {
                this["CELL"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string STATE
        {
            get
            {
                return (string)this["STATE"];
            }
            set
            {
                this["STATE"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
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
        public string R_TEST_RECORD_ID
        {
            get
            {
                return (string)this["R_TEST_RECORD_ID"];
            }
            set
            {
                this["R_TEST_RECORD_ID"] = value;
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
    public class R_TEST_DETAIL_VERTIV
    {
        public string ERROR_CODE;
        public string OPERATOR;
        public string CELL;
        public string STATION;
        public string STATE;
        public DateTime? CREATETIME;
        public string SKUNO;
        public string SN;
        public string R_TEST_RECORD_ID;
        public string ID;
    }
}