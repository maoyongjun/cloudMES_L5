using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_List_Item : DataObjectTable
    {
        public T_C_KP_List_Item(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_List_Item(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_List_Item);
            TableName = "C_KP_List_Item".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<string> GetItemDetailID(string ID, OleExec DB)
        {
            List<string> ret = new List<string>();
            string strSql = $@"select ID from c_kp_list_item_detail c where c.item_id='{ID}' order by c.seq ";
            DataSet res = DB.RunSelect(strSql);
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                ret.Add(res.Tables[0].Rows[i]["ID"].ToString());
            }
            return ret;
        }

        public List<C_KP_List_Item> GetItemObjectByListId(string listID,OleExec sfcdb)
        {
            List<C_KP_List_Item> itemList = new List<C_KP_List_Item>();
            string sql = $@" select * from c_kp_list_item where list_id='{listID}' order by seq ";
            DataSet ds = sfcdb.RunSelect(sql);
            Row_C_KP_List_Item rowItem;
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    rowItem = (Row_C_KP_List_Item)this.NewRow();
                    rowItem.loadData(row);
                    itemList.Add(rowItem.GetDataObject());
                }
            }
            catch(Exception ex)
            {
                itemList = null;
            }
            return itemList;
        }
    }
    public class Row_C_KP_List_Item : DataObjectBase
    {
        public Row_C_KP_List_Item(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_List_Item GetDataObject()
        {
            C_KP_List_Item DataObject = new C_KP_List_Item();
            DataObject.ID = this.ID;
            DataObject.LIST_ID = this.LIST_ID;
            DataObject.KP_NAME = this.KP_NAME;
            DataObject.KP_PARTNO = this.KP_PARTNO;
            DataObject.STATION = this.STATION;
            DataObject.QTY = this.QTY;
            DataObject.SEQ = this.SEQ;
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
        public string LIST_ID
        {
            get
            {
                return (string)this["LIST_ID"];
            }
            set
            {
                this["LIST_ID"] = value;
            }
        }
        public string KP_NAME
        {
            get
            {
                return (string)this["KP_NAME"];
            }
            set
            {
                this["KP_NAME"] = value;
            }
        }
        public string KP_PARTNO
        {
            get
            {
                return (string)this["KP_PARTNO"];
            }
            set
            {
                this["KP_PARTNO"] = value;
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
    public class C_KP_List_Item
    {
        public string ID;
        public string LIST_ID;
        public string KP_NAME;
        public string KP_PARTNO;
        public string STATION;
        public double? QTY;
        public double? SEQ;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}