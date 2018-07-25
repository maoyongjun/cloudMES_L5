using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_MPN : DataObjectTable
    {
        public T_C_SKU_MPN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_MPN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_MPN);
            TableName = "C_SKU_MPN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool IsExists(OleExec DB, string Sku,string PartNo,string Mpn)
        {
            string sql = $@" SELECT 1 FROM C_SKU_MPN where skuno='{Sku}' and partno='{PartNo}' and mpn='{Mpn}' ";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            if (dsCSkuMpn.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public List<C_SKU_MPN> GetMpnBySku(OleExec DB,string Sku)
        {
            string sql = Sku.Equals("") ?  $@"SELECT * FROM C_SKU_MPN ":$@"SELECT * FROM C_SKU_MPN where skuno='{Sku}'";
            DataSet dsCSkuMpn = DB.ExecSelect(sql);
            List<C_SKU_MPN> CSkuMpnList = new List<C_SKU_MPN>();
            foreach (DataRow VARIABLE in dsCSkuMpn.Tables[0].Rows)
            {
                Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)this.NewRow();
                rowCSkuMpn.loadData(VARIABLE);
                CSkuMpnList.Add(rowCSkuMpn.GetDataObject());
            }
            return CSkuMpnList;
        }

        public List<C_SKU_MPN> GetMpnBySkuAndPartno(OleExec sfcdb,string sku,string partno)
        {
            string sql = $@" select * from c_sku_mpn where skuno='{sku}' and partno='{partno}' ";
            DataSet ds = sfcdb.ExecSelect(sql);
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_C_SKU_MPN rowCSkuMpn = (Row_C_SKU_MPN)this.NewRow();
                rowCSkuMpn.loadData(VARIABLE);
                skuMpnList.Add(rowCSkuMpn.GetDataObject());
            }
            return skuMpnList;
        }


    }
    public class Row_C_SKU_MPN : DataObjectBase
    {
        public Row_C_SKU_MPN(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_MPN GetDataObject()
        {
            C_SKU_MPN DataObject = new C_SKU_MPN();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.MPN = this.MPN;
            DataObject.MFRCODE = this.MFRCODE;
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
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
            }
        }
        public string MFRCODE
        {
            get
            {
                return (string)this["MFRCODE"];
            }
            set
            {
                this["MFRCODE"] = value;
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
    public class C_SKU_MPN : IComparable<C_SKU_MPN>
    {
        public string ID;
        public string SKUNO;
        public string PARTNO;
        public string MPN;
        public string MFRCODE;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;

        public int CompareTo(C_SKU_MPN other)
        {
            if (this.ID == other.ID)
            {
                return 0;
            }

            if (this.SKUNO == other.SKUNO && this.PARTNO == other.PARTNO && this.MPN == other.MPN)
            {
                return 0;
            }
            return 1;
        }
    }
}