using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PACKING : DataObjectTable
    {
        public T_R_PACKING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PACKING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PACKING);
            TableName = "R_PACKING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_R_PACKING GetRPackingByPackNo(OleExec DB,string PackNo)
        {
            string strSql = $@" SELECT * FROM R_PACKING where PACK_NO='{PackNo}' ";
            DataSet ds = DB.ExecSelect(strSql);
            Row_R_PACKING r = (Row_R_PACKING)this.NewRow();
            r.loadData(ds.Tables[0].Rows[0]);
            return r;
        }
    }
    public class Row_R_PACKING : DataObjectBase
    {
        public Row_R_PACKING(DataObjectInfo info) : base(info)
        {

        }
        public R_PACKING GetDataObject()
        {
            R_PACKING DataObject = new R_PACKING();
            DataObject.IP = this.IP;
            DataObject.STATION = this.STATION;
            DataObject.LINE = this.LINE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.QTY = this.QTY;
            DataObject.MAX_QTY = this.MAX_QTY;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARENT_PACK_ID = this.PARENT_PACK_ID;
            DataObject.PACK_TYPE = this.PACK_TYPE;
            DataObject.PACK_NO = this.PACK_NO;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
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
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public double? MAX_QTY
        {
            get
            {
                return (double?)this["MAX_QTY"];
            }
            set
            {
                this["MAX_QTY"] = value;
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
        public string PARENT_PACK_ID
        {
            get
            {
                return (string)this["PARENT_PACK_ID"];
            }
            set
            {
                this["PARENT_PACK_ID"] = value;
            }
        }
        public string PACK_TYPE
        {
            get
            {
                return (string)this["PACK_TYPE"];
            }
            set
            {
                this["PACK_TYPE"] = value;
            }
        }
        public string PACK_NO
        {
            get
            {
                return (string)this["PACK_NO"];
            }
            set
            {
                this["PACK_NO"] = value;
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
    public class R_PACKING
    {
        public string IP;
        public string STATION;
        public string LINE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
        public DateTime? CREATE_TIME;
        public string CLOSED_FLAG;
        public double? QTY;
        public double? MAX_QTY;
        public string SKUNO;
        public string PARENT_PACK_ID;
        public string PACK_TYPE;
        public string PACK_NO;
        public string ID;
    }
}