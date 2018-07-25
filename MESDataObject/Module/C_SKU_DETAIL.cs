using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_DETAIL : DataObjectTable
    {
        public T_C_SKU_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_DETAIL);
            TableName = "C_SKU_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }


        public Row_C_SKU_DETAIL getC_SKU_DETAILbyID(string id, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_SKU_DETAIL where ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_SKU_DETAIL ret = (Row_C_SKU_DETAIL)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 獲取所有的 C_SKU_DETAIL 記錄
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_SKU_DETAIL> GetAllSkuDetail(OleExec DB)
        {
            List<C_SKU_DETAIL> SkuDetails = new List<C_SKU_DETAIL>();
            string sql = string.Empty;
            DataTable dt = new DataTable("AllSkuDetail");
            Row_C_SKU_DETAIL SkuDetailRow = (Row_C_SKU_DETAIL)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = "SELECT * FROM C_SKU_DETAIL ORDER BY EDIT_TIME DESC ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SkuDetailRow.loadData(dr);
                    SkuDetails.Add(SkuDetailRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return SkuDetails;
        }

        public List<C_SKU_DETAIL> GetSkuDetailBySkuno(string skuno, OleExec DB)
        {
            List<C_SKU_DETAIL> SkuDetails = new List<C_SKU_DETAIL>();
            string sql = string.Empty;
            DataTable dt = new DataTable("AllSkuDetail");
            Row_C_SKU_DETAIL SkuDetailRow = (Row_C_SKU_DETAIL)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@" SELECT * FROM C_SKU_DETAIL where  ";
                if (skuno == "")
                    sql += $@" rownum<21 order by EDIT_TIME DESC  ";
                else
                    sql += $@" SKUNO='{skuno}' ORDER BY EDIT_TIME DESC ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SkuDetailRow.loadData(dr);
                    SkuDetails.Add(SkuDetailRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return SkuDetails;
        }

        /// <summary>
        /// 根據料號，類別，類別具體項目來獲得設置在 C_SKU_DETAIL 裡面的值
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="CategoryName"></param>
        /// <param name="Skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SKU_DETAIL GetSkuDetail(string Category,string CategoryName,string Skuno,OleExec DB)
        {
            C_SKU_DETAIL SkuDetail = null;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_C_SKU_DETAIL SkuDetailRow = (Row_C_SKU_DETAIL)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT * FROM C_SKU_DETAIL WHERE SKUNO='{Skuno}' AND CATEGORY='{Category}' AND CATEGORY_NAME='{CategoryName}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    SkuDetailRow.loadData(dt.Rows[0]);
                    SkuDetail = SkuDetailRow.GetDataObject();
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return SkuDetail;
        }

        /// <summary>
        /// 添加或者更新一個 C_SKU_DETAIL 記錄
        /// 需要傳遞一個完完整整的 C_SKU_DETAIL 對象，包括 ID
        /// </summary>
        /// <param name="SkuDetail"></param>
        /// <param name="Bu"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddOrUpdateSkuDetail(string Operation,C_SKU_DETAIL SkuDetail,OleExec DB)
        {
            Row_C_SKU_DETAIL SkuDetailRow = (Row_C_SKU_DETAIL)NewRow();
            string sql = string.Empty;
            int result = 0;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (SkuDetail != null && SkuDetail.ID!=null)
                {
                    switch (Operation.Trim().ToUpper())
                    {
                        case "ADD":
                            SkuDetailRow = (Row_C_SKU_DETAIL)ConstructRow(SkuDetail);
                            sql = SkuDetailRow.GetInsertString(this.DBType);
                            break;
                        case "UPDATE":
                            SkuDetailRow = (Row_C_SKU_DETAIL)GetObjByID(SkuDetail.ID, DB);
                            SkuDetailRow.SKUNO = SkuDetailRow.SKUNO;
                            SkuDetailRow.CATEGORY = SkuDetail.CATEGORY;
                            SkuDetailRow.CATEGORY_NAME = SkuDetail.CATEGORY_NAME;
                            SkuDetailRow.VALUE = SkuDetail.VALUE;
                            SkuDetailRow.EXTEND = SkuDetail.EXTEND;
                            SkuDetailRow.VERSION = SkuDetail.VERSION;
                            SkuDetailRow.BASETEMPLATE = SkuDetail.BASETEMPLATE;
                            SkuDetailRow.EDIT_EMP = SkuDetail.EDIT_EMP;
                            SkuDetailRow.EDIT_TIME = SkuDetail.EDIT_TIME;
                            sql = SkuDetailRow.GetUpdateString(this.DBType);
                            break;
                    }

                    result = DB.ExecSqlNoReturn(sql, null);
                }
               
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// 刪除一個 C_SKU_DETAIL 記錄
        /// </summary>
        /// <param name="SkuDetailId"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteSkuDetail(string SkuDetailId, OleExec DB)
        {
            int result = 0;
            string sql = string.Empty;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                if (SkuDetailId.Length > 0)
                {
                    sql = $@"DELETE FROM C_SKU_DETAIL WHERE ID='{SkuDetailId}'";
                    result = DB.ExecSqlNoReturn(sql, null);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;

        }

        
    }
    public class Row_C_SKU_DETAIL : DataObjectBase
    {
        public Row_C_SKU_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_DETAIL GetDataObject()
        {
            C_SKU_DETAIL DataObject = new C_SKU_DETAIL();
            DataObject.SKUNO = this.SKUNO;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.CATEGORY_NAME = this.CATEGORY_NAME;
            DataObject.VALUE = this.VALUE;
            DataObject.EXTEND = this.EXTEND;
            DataObject.VERSION = this.VERSION;
            DataObject.BASETEMPLATE = this.BASETEMPLATE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.ID = this.ID;
            return DataObject;
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
        public string CATEGORY_NAME
        {
            get
            {
                return (string)this["CATEGORY_NAME"];
            }
            set
            {
                this["CATEGORY_NAME"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public string EXTEND
        {
            get
            {
                return (string)this["EXTEND"];
            }
            set
            {
                this["EXTEND"] = value;
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
        public string BASETEMPLATE
        {
            get
            {
                return (string)this["BASETEMPLATE"];
            }
            set
            {
                this["BASETEMPLATE"] = value;
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
    public class C_SKU_DETAIL
    {
        public string SKUNO;
        public string CATEGORY;
        public string CATEGORY_NAME;
        public string VALUE;
        public string EXTEND;
        public string VERSION;
        public string BASETEMPLATE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
        public string ID;
    }
}