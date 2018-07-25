using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    // <copyright file="R_WO_TYPE.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-03-16 </date>
    /// <summary>
    /// 映射數據庫中的R_WO_TYPE表
    /// </summary>
    public class T_R_WO_TYPE : DataObjectTable
    {
        public T_R_WO_TYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_TYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_TYPE);
            TableName = "R_WO_TYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// Get r_wo_type object by wo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="wo"></param>
        /// <returns></returns>
        public R_WO_TYPE GetWOTypeByWO(OleExec db,string order_type)
        {
            R_WO_TYPE woType = new R_WO_TYPE();
            string sql = "";
            try
            {                
                sql = $@"select * from R_WO_TYPE where order_type='{order_type}'";
                DataSet ds = db.ExecSelect(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Row_R_WO_TYPE rowWOType = (Row_R_WO_TYPE)this.NewRow();
                    rowWOType.loadData(ds.Tables[0].Rows[0]);
                    woType = rowWOType.GetDataObject();
                }
                else
                {
                    woType = null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return woType;
        }

        public List<string> GetAllType(OleExec db)
        {
            string sql = null;
            DataTable dt = null;
            List<string> types = new List<string>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"select distinct(workorder_type) wotype from {TableName} ";
                try
                {
                    dt = db.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        types.Add(dr[0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                    
                }
                return types;
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }
            
        }

    }
    public class Row_R_WO_TYPE : DataObjectBase
    {
        public Row_R_WO_TYPE(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_TYPE GetDataObject()
        {
            R_WO_TYPE DataObject = new R_WO_TYPE();
            DataObject.ID = this.ID;
            DataObject.WORKORDER_TYPE = this.WORKORDER_TYPE;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.PREFIX = this.PREFIX;
            DataObject.ORDER_TYPE = this.ORDER_TYPE;
            DataObject.PRODUCT_TYPE = this.PRODUCT_TYPE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string WORKORDER_TYPE
        {
            get
            {
                return (string)this["WORKORDER_TYPE"];
            }
            set
            {
                this["WORKORDER_TYPE"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string PREFIX
        {
            get
            {
                return (string)this["PREFIX"];
            }
            set
            {
                this["PREFIX"] = value;
            }
        }
        public string ORDER_TYPE
        {
            get
            {
                return (string)this["ORDER_TYPE"];
            }
            set
            {
                this["ORDER_TYPE"] = value;
            }
        }
        public string PRODUCT_TYPE
        {
            get
            {
                return (string)this["PRODUCT_TYPE"];
            }
            set
            {
                this["PRODUCT_TYPE"] = value;
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
    public class R_WO_TYPE
    {
        public string ID;
        public string WORKORDER_TYPE;
        public string CATEGORY;
        public string PREFIX;
        public string ORDER_TYPE;
        public string PRODUCT_TYPE;
        public string DESCRIPTION;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}