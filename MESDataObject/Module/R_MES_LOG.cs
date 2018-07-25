using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    // <copyright file="R_MES_LOG.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-03-14 </date>
    /// <summary>
    /// 
    /// </summary>
    public class T_R_MES_LOG : DataObjectTable
    {
        public T_R_MES_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MES_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MES_LOG);
            TableName = "R_MES_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// Get MESLog datatable
        /// </summary>
        /// <param name="programName">program_name</param>
        /// <param name="className">class_name</param>
        /// <param name="functionName">function_name</param>
        /// <param name="startTime">edit_time</param>
        /// <param name="endTime">edit_time</param>
        /// <param name="db"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public DataTable GetMESLog(string programName,string className,string functionName,string startTime,string endTime,OleExec db,DB_TYPE_ENUM dbType)
        {
            string sql = $@"select * from r_mes_log where 1=1";
            if (!string.IsNullOrEmpty(programName))
            {
                sql = sql + $@" and program_name='{programName}'";
            }
            if (!string.IsNullOrEmpty(className))
            {
                sql = sql + $@" and class_name='{className}'";
            }
            if (!string.IsNullOrEmpty(functionName))
            {
                sql = sql + $@" and function_name='{functionName}'";
            }
            if (!string.IsNullOrEmpty(startTime)&&!string.IsNullOrEmpty(endTime))
            {
                sql = sql + $@" and and edit_time between to_date('{startTime}','yyyy/mm/dd hh24:mi:ss') and to_date('{endTime}','yyyy/mm/dd hh24:mi:ss')";
            }
            sql = sql + " order by edit_time";

            return db.ExecSelect(sql).Tables[0];
        }
    }
    public class Row_R_MES_LOG : DataObjectBase
    {
        public Row_R_MES_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_MES_LOG GetDataObject()
        {
            R_MES_LOG DataObject = new R_MES_LOG();
            DataObject.ID = this.ID;
            DataObject.PROGRAM_NAME = this.PROGRAM_NAME;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.FUNCTION_NAME = this.FUNCTION_NAME;
            DataObject.LOG_MESSAGE = this.LOG_MESSAGE;
            DataObject.LOG_SQL = this.LOG_SQL;
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
        public string PROGRAM_NAME
        {
            get
            {
                return (string)this["PROGRAM_NAME"];
            }
            set
            {
                this["PROGRAM_NAME"] = value;
            }
        }
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string FUNCTION_NAME
        {
            get
            {
                return (string)this["FUNCTION_NAME"];
            }
            set
            {
                this["FUNCTION_NAME"] = value;
            }
        }
        public string LOG_MESSAGE
        {
            get
            {
                return (string)this["LOG_MESSAGE"];
            }
            set
            {
                this["LOG_MESSAGE"] = value;
            }
        }
        public string LOG_SQL
        {
            get
            {
                return (string)this["LOG_SQL"];
            }
            set
            {
                this["LOG_SQL"] = value;
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
    public class R_MES_LOG
    {
        public string ID;
        public string PROGRAM_NAME;
        public string CLASS_NAME;
        public string FUNCTION_NAME;
        public string LOG_MESSAGE;
        public string LOG_SQL;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}