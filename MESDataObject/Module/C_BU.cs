using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Common;

namespace MESDataObject.Module
{
    // <copyright file="C_BU.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2017-11-27 </date>
    /// <summary>
    /// 映射數據庫中的C_BU表
    /// </summary>
    public class T_C_BU : DataObjectTable
    {
        public T_C_BU(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
        }
        public T_C_BU(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_BU);
            TableName = "C_BU".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool BUIsExist(OleExec oleDB, string bu)
        {
            string sql = $@"select * from c_bu  where bu='{bu}' ";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool BUIsExistByID(OleExec oleDB, string strid)
        {
            string sql = $@"select * from c_bu  where id='{strid}' ";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Get BU ID
        /// </summary>
        /// <param name="DB">DB</param>
        /// <returns></returns>
        public string GetBUID(string bu, OleExec DB)
        {                       
            string sql = $@"select id from c_bu where bu='{bu}' ";            
            DataSet dsBU = DB.ExecSelect(sql);
            return dsBU.Tables[0].Rows[0][0].ToString();
        }
        /// <summary>
        /// Get All BU
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<string> GetAllBU(OleExec DB)
        {
            string sql = $@"select distinct bu from c_bu  order by bu";
      
            DataSet dsBU = DB.ExecSelect(sql);
            List<string> buList = new List<string>();
            
            foreach (DataRow row in dsBU.Tables[0].Rows)
            {                
                buList.Add(row["bu"].ToString());
            }
            return buList;
        }

        public List<C_BU> GetBUList(OleExec oleDB, string bu)
        {
            string sql = "";
            List<C_BU> BUList = new List<C_BU>();
            if (string.IsNullOrEmpty(bu))
            {
                sql = $@"select * from c_bu order by id";
            }
            else
            {
                sql = $@"select * from c_bu  where bu like '%{bu}%' order by id";
            }
            DataSet dsBU = oleDB.ExecSelect(sql);
            Row_C_BU rowBU;
            foreach (DataRow row in dsBU.Tables[0].Rows)
            {
                rowBU = (Row_C_BU)this.NewRow();
                rowBU.loadData(row);
                BUList.Add(rowBU.GetDataObject());
            }
            return BUList;
        }

        public DataTable GetAllBu(OleExec DB)
        {
            string sql = $@"select distinct bu from c_bu  order by bu";

            DataTable dt = DB.ExecSelect(sql).Tables[0];

            return dt;
        }
    }

    public class Row_C_BU : DataObjectBase
    {
        public Row_C_BU(DataObjectInfo info) : base(info)
        {

        }
        public C_BU GetDataObject()
        {
            C_BU DataObject = new C_BU();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
    }
    public class C_BU
    {
        public string ID;
        public string BU;
    }
}