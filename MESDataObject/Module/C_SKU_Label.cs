using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_Label : DataObjectTable
    {
        public T_C_SKU_Label(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_Label(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_Label);
            TableName = "C_SKU_Label".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<Row_C_SKU_Label> GetLabelConfigBySkuStation(string Skuno,string Station,OleExec DB)
        {
            List<Row_C_SKU_Label> ret = new List<Row_C_SKU_Label>();
            string strSql = $@"select * from c_sku_label where skuno='{Skuno}' and station='{Station}'";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_SKU_Label R =(Row_C_SKU_Label) NewRow();
                R.loadData(res.Tables[0].Rows[i]);
                ret.Add(R);
            }

            return ret;
        }
        public List<Row_C_SKU_Label> GetLabelConfigBySku(string Skuno,  OleExec DB)
        {
            List<Row_C_SKU_Label> ret = new List<Row_C_SKU_Label>();
            string strSql = $@"select * from c_sku_label where skuno='{Skuno}'";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_C_SKU_Label R = (Row_C_SKU_Label)NewRow();
                R.loadData(res.Tables[0].Rows[i]);
                ret.Add(R);
            }

            return ret;
        }

    }
    public class Row_C_SKU_Label : DataObjectBase
    {
        public Row_C_SKU_Label(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_Label GetDataObject()
        {
            C_SKU_Label DataObject = new C_SKU_Label();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION = this.STATION;
            DataObject.SEQ = this.SEQ;
            DataObject.QTY = this.QTY;
            DataObject.LABELNAME = this.LABELNAME;
            DataObject.LABELTYPE = this.LABELTYPE;
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
        public string LABELNAME
        {
            get
            {
                return (string)this["LABELNAME"];
            }
            set
            {
                this["LABELNAME"] = value;
            }
        }
        public string LABELTYPE
        {
            get
            {
                return (string)this["LABELTYPE"];
            }
            set
            {
                this["LABELTYPE"] = value;
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
    public class C_SKU_Label
    {
        public string ID;
        public string SKUNO;
        public string STATION;
        public double? SEQ;
        public double? QTY;
        public string LABELNAME;
        public string LABELTYPE;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}