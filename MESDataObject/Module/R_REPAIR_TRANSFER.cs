using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_TRANSFER : DataObjectTable
    {
        public T_R_REPAIR_TRANSFER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_TRANSFER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_TRANSFER);
            TableName = "R_REPAIR_TRANSFER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_REPAIR_TRANSFER> GetReSNbysn(string RelSn, OleExec DB)
        {

            string strSql = $@" SELECT *

                                  FROM R_REPAIR_TRANSFER
                                 WHERE (REPAIR_MAIN_ID, SN) IN (SELECT ID, SN
                                                                  FROM R_REPAIR_MAIN
                                                                 WHERE (SN, EDIT_TIME) IN (  SELECT SN,
                                                                                                    MAX (
                                                                                                       EDIT_TIME)
                                                                                               FROM R_REPAIR_MAIN
                                                                                              WHERE     SN =
                                                                                                           '{RelSn}'
                                                                                                    AND CLOSED_FLAG =
                                                                                                           '0'
                                                                                           GROUP BY SN)) ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            List<R_REPAIR_TRANSFER> listSn = new List<R_REPAIR_TRANSFER>();
            if (res.Rows.Count > 0)
            {
                foreach (DataRow item in res.Rows)
                {
                    Row_R_REPAIR_TRANSFER ret = (Row_R_REPAIR_TRANSFER)NewRow();
                    ret.loadData(item);
                    listSn.Add(ret.GetDataObject());
                }
            }
            else
            {
                return null;
            }
            return listSn;
        }
    }
    public class Row_R_REPAIR_TRANSFER : DataObjectBase
    {
        public Row_R_REPAIR_TRANSFER(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_TRANSFER GetDataObject()
        {
            R_REPAIR_TRANSFER DataObject = new R_REPAIR_TRANSFER();
            DataObject.ID = this.ID;
            DataObject.REPAIR_MAIN_ID = this.REPAIR_MAIN_ID;
            DataObject.SN = this.SN;
            DataObject.LINE_NAME = this.LINE_NAME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.IN_TIME = this.IN_TIME;
            DataObject.IN_SEND_EMP = this.IN_SEND_EMP;
            DataObject.IN_RECEIVE_EMP = this.IN_RECEIVE_EMP;
            DataObject.OUT_TIME = this.OUT_TIME;
            DataObject.OUT_SEND_EMP = this.OUT_SEND_EMP;
            DataObject.OUT_RECEIVE_EMP = this.OUT_RECEIVE_EMP;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.CREATE_TIME = this.CREATE_TIME;
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
        public string REPAIR_MAIN_ID
        {
            get

            {
                return (string)this["REPAIR_MAIN_ID"];
            }
            set
            {
                this["REPAIR_MAIN_ID"] = value;
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
        public string LINE_NAME
        {
            get

            {
                return (string)this["LINE_NAME"];
            }
            set
            {
                this["LINE_NAME"] = value;
            }
        }
        public string STATION_NAME
        {
            get

            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
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
        public string WORKORDERNO
        {
            get

            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public DateTime? IN_TIME
        {
            get

            {
                return (DateTime?)this["IN_TIME"];
            }
            set
            {
                this["IN_TIME"] = value;
            }
        }
        public string IN_SEND_EMP
        {
            get

            {
                return (string)this["IN_SEND_EMP"];
            }
            set
            {
                this["IN_SEND_EMP"] = value;
            }
        }
        public string IN_RECEIVE_EMP
        {
            get

            {
                return (string)this["IN_RECEIVE_EMP"];
            }
            set
            {
                this["IN_RECEIVE_EMP"] = value;
            }
        }
        public DateTime? OUT_TIME
        {
            get

            {
                return (DateTime?)this["OUT_TIME"];
            }
            set
            {
                this["OUT_TIME"] = value;
            }
        }
        public string OUT_SEND_EMP
        {
            get

            {
                return (string)this["OUT_SEND_EMP"];
            }
            set
            {
                this["OUT_SEND_EMP"] = value;
            }
        }
        public string OUT_RECEIVE_EMP
        {
            get

            {
                return (string)this["OUT_RECEIVE_EMP"];
            }
            set
            {
                this["OUT_RECEIVE_EMP"] = value;
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
    public class R_REPAIR_TRANSFER
    {
        public string ID;
        public string REPAIR_MAIN_ID;
        public string SN;
        public string LINE_NAME;
        public string STATION_NAME;
        public string SKUNO;
        public string WORKORDERNO;
        public DateTime? IN_TIME;
        public string IN_SEND_EMP;
        public string IN_RECEIVE_EMP;
        public DateTime? OUT_TIME;
        public string OUT_SEND_EMP;
        public string OUT_RECEIVE_EMP;
        public string DESCRIPTION;
        public string CLOSED_FLAG;
        public DateTime? CREATE_TIME;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}