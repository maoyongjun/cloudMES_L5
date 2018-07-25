using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_REPAIR_DAY : DataObjectTable
    {
        public T_C_REPAIR_DAY(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_REPAIR_DAY(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_REPAIR_DAY);
            TableName = "C_REPAIR_DAY".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_REPAIR_DAY GetDetailBySkuno(OleExec sfcdb, string skuno)
        {
            if (string.IsNullOrEmpty(skuno))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "SKUNO" }));
            }
            DataTable dt = null;
            Row_C_REPAIR_DAY row_c_repair_day = null;
            string sql = $@"select * from {TableName} where skuno='{skuno.Replace("'","''")}' ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 1)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000086"));
                    }
                    row_c_repair_day = (Row_C_REPAIR_DAY) this.NewRow();
                    row_c_repair_day.loadData(dt.Rows[0]);
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
            return row_c_repair_day.GetDataObject();
        }
    }
    public class Row_C_REPAIR_DAY : DataObjectBase
    {
        public Row_C_REPAIR_DAY(DataObjectInfo info) : base(info)
        {

        }
        public C_REPAIR_DAY GetDataObject()
        {
            C_REPAIR_DAY DataObject = new C_REPAIR_DAY();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.REPAIR_DAY_COUNT = this.REPAIR_DAY_COUNT;
            DataObject.REPAIR_COUNT = this.REPAIR_COUNT;
            DataObject.STATION_COUNT = this.STATION_COUNT;
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
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public double? REPAIR_DAY_COUNT
        {
            get
            {
                return (double?)this["REPAIR_DAY_COUNT"];
            }
            set
            {
                this["REPAIR_DAY_COUNT"] = value;
            }
        }
        public double? REPAIR_COUNT
        {
            get
            {
                return (double?)this["REPAIR_COUNT"];
            }
            set
            {
                this["REPAIR_COUNT"] = value;
            }
        }
        public double? STATION_COUNT
        {
            get
            {
                return (double?)this["STATION_COUNT"];
            }
            set
            {
                this["STATION_COUNT"] = value;
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
    public class C_REPAIR_DAY
    {
        public string ID;
        public string SKUNO;
        public string VERSION;
        public double? REPAIR_DAY_COUNT;
        public double? REPAIR_COUNT;
        public double? STATION_COUNT;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}