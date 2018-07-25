using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TEST_RECORD : DataObjectTable
    {
        public T_R_TEST_RECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_RECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_RECORD);
            TableName = "R_TEST_RECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 取Sn loading之後每個測試工站的最後一筆測試記錄
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="snId"></param>
        /// <param name="StartTime"></param>
        /// <returns></returns>
        public List<R_TEST_RECORD> GetTestDataByTimeBefor(OleExec DB,string snId,DateTime StartTime)
        {
            List<R_TEST_RECORD> l = new List<R_TEST_RECORD>();
            string strSql = $@" select * from (
                            select a.*,RANK() over(partition by messtation order by endtime desc) as rk from r_test_record a
                             where r_sn_id ='{snId}' and endtime>to_date('{StartTime.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy/mm/dd hh24:mi:ss')) where rk=1 ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Row_R_TEST_RECORD r = (Row_R_TEST_RECORD)this.NewRow();
                r.loadData(dr);
                l.Add(r.GetDataObject());
            }
            return l;
        }

        /// <summary>
        /// add by fgg 2018.5.11
        /// </summary>
        /// <param name="snID">snID</param>
        /// <param name="sn">sn</param>
        /// <param name="station">station</param>
        /// <param name="db">db</param>
        /// <returns></returns>
        public bool CheckTestBySNAndStation(string snID,string station, OleExec db)
        {            
            string sql = $@" select * from r_test_record rt where rt.r_sn_id='{snID}' 
                            and exists (select * from c_temes_station_mapping tmap where tmap.te_station=rt.testation 
                            and tmap.mes_station='{station}') 
                            and exists (select 1 from R_SN rs where rt.r_sn_id=rs.id and rt.endtime>rs.start_time  ) order by endtime desc";
            DataTable passDT = db.ExecSelect(sql).Tables[0];
            if (passDT.Rows.Count > 0)
            {
                if (passDT.Rows[0]["STATE"].ToString() == "PASS")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
    public class Row_R_TEST_RECORD : DataObjectBase
    {
        public Row_R_TEST_RECORD(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_RECORD GetDataObject()
        {
            R_TEST_RECORD DataObject = new R_TEST_RECORD();
            DataObject.DETAILTABLE = this.DETAILTABLE;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.STARTTIME = this.STARTTIME;
            DataObject.DEVICE = this.DEVICE;
            DataObject.MESSTATION = this.MESSTATION;
            DataObject.TESTATION = this.TESTATION;
            DataObject.TEGROUP = this.TEGROUP;
            DataObject.STATE = this.STATE;
            DataObject.SN = this.SN;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string DETAILTABLE
        {
            get
            {
                return (string)this["DETAILTABLE"];
            }
            set
            {
                this["DETAILTABLE"] = value;
            }
        }
        public DateTime? ENDTIME
        {
            get
            {
                return (DateTime?)this["ENDTIME"];
            }
            set
            {
                this["ENDTIME"] = value;
            }
        }
        public DateTime? STARTTIME
        {
            get
            {
                return (DateTime?)this["STARTTIME"];
            }
            set
            {
                this["STARTTIME"] = value;
            }
        }
        public string DEVICE
        {
            get
            {
                return (string)this["DEVICE"];
            }
            set
            {
                this["DEVICE"] = value;
            }
        }
        public string MESSTATION
        {
            get
            {
                return (string)this["MESSTATION"];
            }
            set
            {
                this["MESSTATION"] = value;
            }
        }
        public string TESTATION
        {
            get
            {
                return (string)this["TESTATION"];
            }
            set
            {
                this["TESTATION"] = value;
            }
        }
        public string TEGROUP
        {
            get
            {
                return (string)this["TEGROUP"];
            }
            set
            {
                this["TEGROUP"] = value;
            }
        }
        public string STATE
        {
            get
            {
                return (string)this["STATE"];
            }
            set
            {
                this["STATE"] = value;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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
    public class R_TEST_RECORD
    {
        public string DETAILTABLE;
        public DateTime? ENDTIME;
        public DateTime? STARTTIME;
        public string DEVICE;
        public string MESSTATION;
        public string TESTATION;
        public string TEGROUP;
        public string STATE;
        public string SN;
        public string R_SN_ID;
        public string ID;
    }
}