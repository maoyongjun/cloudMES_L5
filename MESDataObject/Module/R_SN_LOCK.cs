using System;
using MESDBHelper;
using System.Data;
using System.Collections.Generic;

namespace MESDataObject.Module
{
    public class T_R_SN_LOCK : DataObjectTable
    {
        public T_R_SN_LOCK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_LOCK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_LOCK);
            TableName = "R_SN_LOCK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_SN_LOCK> GetLockList(string LOCK_LOT, string SN,string WORKORDERNO, OleExec DB)
        {
            List<R_SN_LOCK> Seq = new List<R_SN_LOCK>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SEQNO");
            Row_R_SN_LOCK SeqRow = (Row_R_SN_LOCK)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select * from R_SN_LOCK where 1=1  ";
                if (LOCK_LOT != "")
                    sql += $@" and LOCK_LOT='{LOCK_LOT}' ";
                if (SN != "")
                    sql += $@" and SN='{SN}' ";
                if(WORKORDERNO != "")
                    sql += $@" and WORKORDERNO='{WORKORDERNO}' ";
                if (LOCK_LOT == "" && SN == "" &&  WORKORDERNO == "")
                    sql += $@" and  rownum<21  order by LOCK_TIME ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SeqRow.loadData(dr);
                    Seq.Add(SeqRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return Seq;
        }

        public R_SN_LOCK GetDetailBySN(OleExec sfcdb, string sn, string station)
        {
            if (string.IsNullOrEmpty(sn) || string.IsNullOrEmpty(station))
            {
                return null;
            }
            string sql = $@"select * from {TableName} where sn='{sn.Replace("'", "''")}' 
                            and lock_station='{station.Replace("'", "''")}' and lock_status='0' ";
            DataTable dt = null;
            Row_R_SN_LOCK row_r_sn_lock = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row_r_sn_lock = (Row_R_SN_LOCK) this.NewRow();
                    row_r_sn_lock.loadData(dt.Rows[0]);
                }
            }
            else
            {
                return null;
            }
            return row_r_sn_lock == null? null: row_r_sn_lock.GetDataObject();
        }

        public bool IsExist(OleExec sfcdb, string SerialNoOrLineName, string CurrentStation)
        {
            if (string.IsNullOrEmpty(SerialNoOrLineName) || string.IsNullOrEmpty(CurrentStation))
            {
                return true;
            }
            string sql = $@" select id from {TableName} where sn='{SerialNoOrLineName.Replace("'", "''")}' 
                            and lock_station='{CurrentStation.Replace("'", "''")}' and lock_status='0' ";
            //object obj = sfcdb.ExecSelectOneValue(sql);
            try
            {
                DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }
            
        }

        public List<R_SN_LOCK> GetLockListByPackNo(string packNo,OleExec DB)
        {
            List<R_SN_LOCK> res = new List<R_SN_LOCK>();
           string strSql = $@" select E.* from R_PACKING A,R_PACKING B ,R_SN_PACKING C,R_SN D,R_SN_LOCK E WHERE A.ID=C.PACK_ID AND B.ID=A.PARENT_PACK_ID AND C.SN_ID=D.ID AND D.SN=E.SN
                                AND B.PACK_NO='{packNo}' AND E.LOCK_STATUS='1' ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_SN_LOCK r = (Row_R_SN_LOCK)this.NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }

        public void LockSnInOba(string lotNo,OleExec DB)
        {
            try
            {
                string strSql = $@" INSERT INTO R_SN_LOCK (ID,LOCK_LOT,SN,TYPE,WORKORDERNO,LOCK_STATION,LOCK_STATUS,LOCK_REASON,LOCK_EMP,LOCK_TIME)
                            SELECT 'MES'||SFC.SEQ_C_ID.NEXTVAL,'{lotNo}',E.SN,'SN',E.WORKORDERNO,E.NEXT_STATION,'1','OBAFAILSAMPLE','SYSTEM',SYSDATE FROM R_LOT_PACK A,R_PACKING B,R_PACKING C,R_SN_PACKING D,R_SN E
                             WHERE A.LOTNO='{lotNo}' AND A.PACKNO=B.PACK_NO AND B.ID=C.PARENT_PACK_ID AND C.ID=D.PACK_ID AND D.SN_ID=E.ID ";
                DB.ThrowSqlExeception = true;
                DB.ExecSQL(strSql);
            }
            catch(Exception e) { throw e; }
            finally
            {
                DB.ThrowSqlExeception = false;
            }
        }


    }
    public class Row_R_SN_LOCK : DataObjectBase
    {
        public Row_R_SN_LOCK(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_LOCK GetDataObject()
        {
            R_SN_LOCK DataObject = new R_SN_LOCK();
            DataObject.ID = this.ID;
            DataObject.LOCK_LOT = this.LOCK_LOT;
            DataObject.TYPE = this.TYPE;
            DataObject.SN = this.SN;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.LOCK_STATION = this.LOCK_STATION;
            DataObject.LOCK_STATUS = this.LOCK_STATUS;
            DataObject.LOCK_REASON = this.LOCK_REASON;
            DataObject.UNLOCK_REASON = this.UNLOCK_REASON;
            DataObject.LOCK_EMP = this.LOCK_EMP;
            DataObject.LOCK_TIME = this.LOCK_TIME;
            DataObject.UNLOCK_EMP = this.UNLOCK_EMP;
            DataObject.UNLOCK_TIME = this.UNLOCK_TIME;
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
        public string LOCK_LOT
        {
            get
            {
                return (string)this["LOCK_LOT"];
            }
            set
            {
                this["LOCK_LOT"] = value;
            }
        }
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
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
        public string LOCK_STATION
        {
            get
            {
                return (string)this["LOCK_STATION"];
            }
            set
            {
                this["LOCK_STATION"] = value;
            }
        }
        public string LOCK_STATUS
        {
            get
            {
                return (string)this["LOCK_STATUS"];
            }
            set
            {
                this["LOCK_STATUS"] = value;
            }
        }
        public string LOCK_REASON
        {
            get
            {
                return (string)this["LOCK_REASON"];
            }
            set
            {
                this["LOCK_REASON"] = value;
            }
        }
        public string UNLOCK_REASON
        {
            get
            {
                return (string)this["UNLOCK_REASON"];
            }
            set
            {
                this["UNLOCK_REASON"] = value;
            }
        }
        public string LOCK_EMP
        {
            get
            {
                return (string)this["LOCK_EMP"];
            }
            set
            {
                this["LOCK_EMP"] = value;
            }
        }
        public DateTime? LOCK_TIME
        {
            get
            {
                return (DateTime)this["LOCK_TIME"];
            }
            set
            {
                this["LOCK_TIME"] = value;
            }
        }
        public string UNLOCK_EMP
        {
            get
            {
                return (string)this["UNLOCK_EMP"];
            }
            set
            {
                this["UNLOCK_EMP"] = value;
            }
        }
        public DateTime? UNLOCK_TIME
        {
            get
            {
                return (DateTime)this["UNLOCK_TIME"];
            }
            set
            {
                this["UNLOCK_TIME"] = value;
            }
        }
    }
    public class R_SN_LOCK
    {
        public string ID;
        public string LOCK_LOT;
        public string TYPE;
        public string SN;
        public string WORKORDERNO;
        public string LOCK_STATION;
        public string LOCK_STATUS;
        public string LOCK_REASON;
        public string UNLOCK_REASON;
        public string LOCK_EMP;
        public DateTime? LOCK_TIME;
        public string UNLOCK_EMP;
        public DateTime? UNLOCK_TIME;
    }
}