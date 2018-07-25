using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ROUTE_DETAIL : DataObjectTable
    {
        public T_C_ROUTE_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
            RowType = typeof(Row_T_C_ROUTE_DETAIL);
            TableName = "c_route_detail".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public T_C_ROUTE_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_T_C_ROUTE_DETAIL);
            TableName = "c_route_detail".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 取Station之前的測試工站,包括當前工站
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> GetTestStationByNameBefor(OleExec DB,string routeId, string CurrentStation)
        {
            List<C_ROUTE_DETAIL> r = new List<C_ROUTE_DETAIL>();
            string strSql = $@" select a.* from c_route_detail a,c_temes_station_mapping b,c_route_detail c where a.route_id=:id
                            and c.station_name=:station_name and c.route_id =a.route_id and a.seq_no<=c.seq_no and a.station_name=b.mes_station order by A.seq_no asc ";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":id", OleDbType.VarChar, 240),
                new OleDbParameter(":station_name", OleDbType.VarChar, 20)
            };
            paramet[0].Value = routeId;
            paramet[1].Value = CurrentStation;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            foreach (DataRow item in res.Rows)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(item);
                r.Add(ret.GetDataObject());
            }
            return r;
        }

        /// <summary>
        /// 通過id獲取
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_ROUTE_DETAIL GetById(string id, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id",OleDbType.VarChar, 240) };
            paramet[0].Value = id;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                ret.loadData(res.Rows[0]);
                return ret.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 通過路由ID獲取所有C_ROUTE_DETAIL，並且order by seq_no asc
        /// </summary>
        /// <param name="RouteID">路由ID</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<C_ROUTE_DETAIL> GetByRouteIdOrderBySEQASC(string RouteID, OleExec DB)
        {
            string strSql = $@"select * from c_route_detail where route_id=:RouteID order by seq_no asc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":RouteID",OleDbType.VarChar, 240) };
            paramet[0].Value = RouteID;
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                List<C_ROUTE_DETAIL> retlist = new List<C_ROUTE_DETAIL>();
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_T_C_ROUTE_DETAIL ret = (Row_T_C_ROUTE_DETAIL)NewRow();
                    ret.loadData(res.Rows[i]);
                    retlist.Add(ret.GetDataObject());
                }
                return retlist;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 添加C_ROUTE_DETAIL
        /// </summary>
        /// <param name="newc_route_detail"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int Add(C_ROUTE_DETAIL newc_route_detail, OleExec DB)
        {
            string strSql = $@"insert into c_route_detail(id,seq_no,route_id,station_name,station_type,return_flag)";
            strSql = strSql + $@"values(:id,:seq_no,:route_id,:station_name,:station_type,:return_flag)";
            OleDbParameter[] paramet = new OleDbParameter[6];
            paramet[0] = new OleDbParameter(":id", newc_route_detail.ID);
            paramet[1] = new OleDbParameter(":seq_no", newc_route_detail.SEQ_NO);
            paramet[2] = new OleDbParameter(":route_id", newc_route_detail.ROUTE_ID);
            paramet[3] = new OleDbParameter(":station_name", newc_route_detail.STATION_NAME);
            paramet[4] = new OleDbParameter(":station_type", newc_route_detail.STATION_TYPE);
            paramet[5] = new OleDbParameter(":return_flag", newc_route_detail.RETURN_FLAG);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過ID刪除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteById(string id, OleExec DB)
        {
            string strSql = $@"delete c_route_detail where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":id", id);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過路由ID刪除
        /// </summary>
        /// <param name="routeid"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int DeleteByRouteId(string routeid, OleExec DB)
        {
            string strSql = $@"delete c_route_detail where route_id=:route_id";
            OleDbParameter[] paramet = new OleDbParameter[1];
            paramet[0] = new OleDbParameter(":route_id", routeid);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        /// <summary>
        /// 通過ID更新
        /// </summary>
        /// <param name="updateitem"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateById(C_ROUTE_DETAIL updateitem, OleExec DB)
        {
            string strSql = $@"update c_route_detail set seq_no=:seq_no,station_name=:station_name,station_type=:station_type,return_flag=:return_flag where id=:id";
            OleDbParameter[] paramet = new OleDbParameter[5];
            paramet[0] = new OleDbParameter(":seq_no", updateitem.SEQ_NO);
            paramet[1] = new OleDbParameter(":station_name", updateitem.STATION_NAME);
            paramet[2] = new OleDbParameter(":station_type", updateitem.STATION_TYPE);
            paramet[3] = new OleDbParameter(":return_flag", updateitem.RETURN_FLAG);
            paramet[4] = new OleDbParameter(":id", updateitem.ID);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }

        public Dictionary<string,object> GetNextStations(string RouteId,string CurrentStation,OleExec DB)
        {
            Dictionary<string, object> Routes = new Dictionary<string, object>();
            List<string> NextStations = new List<string>();
            List<string> Returns = new List<string>();
            List<string> DirectLinks = new List<string>();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string CurrentStationSeq = string.Empty;
            string CurrentStationId = string.Empty;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND STATION_NAME='{CurrentStation}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    CurrentStationId = dt.Rows[0]["ID"].ToString();
                    CurrentStationSeq = dt.Rows[0]["SEQ_NO"].ToString();
                    sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ROUTE_ID='{RouteId}' AND SEQ_NO>'{CurrentStationSeq}' ORDER BY SEQ_NO";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        NextStations.Add(dt.Rows[0]["STATION_NAME"].ToString());
                    }
                    sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ID IN (
                        SELECT DIRECTLINK_ROUTE_DETAIL_ID FROM C_ROUTE_DETAIL_DIRECTLINK WHERE C_ROUTE_DETAIL_ID='{CurrentStationId}')";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            DirectLinks.Add(dr["STATION_NAME"].ToString());
                        }
                    }

                    sql = $@"SELECT * FROM C_ROUTE_DETAIL WHERE ID IN (
                        SELECT RETURN_ROUTE_DETAIL_ID FROM C_ROUTE_DETAIL_RETURN WHERE ROUTE_DETAIL_ID='{CurrentStationId}')";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            Returns.Add(dr["STATION_NAME"].ToString());
                        }
                    }

                    Routes.Add("NextStations", NextStations);
                    Routes.Add("Returns", Returns);
                    Routes.Add("DirectLinks", DirectLinks);
                }
                else if (dt.Rows.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000154",new string[] { CurrentStation}));
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000153",new string[] { CurrentStation }));
                }
                

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return Routes;
        }

        public DataTable GetALLStation(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            string sql = $@"select distinct station_name from c_route_detail";
            dt = db.ExecSelect(sql).Tables[0];
            return dt;
        }
    }
    public class Row_T_C_ROUTE_DETAIL : DataObjectBase
    {
        public Row_T_C_ROUTE_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_ROUTE_DETAIL GetDataObject()
        {
            C_ROUTE_DETAIL DataObject = new C_ROUTE_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.ROUTE_ID = this.ROUTE_ID;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.STATION_TYPE = this.STATION_TYPE;
            DataObject.RETURN_FLAG = this.RETURN_FLAG;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string ROUTE_ID
        {
            get
            {
                return (string)this["ROUTE_ID"];
            }
            set
            {
                this["ROUTE_ID"] = value;
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
        public string STATION_TYPE
        {
            get
            {
                return (string)this["STATION_TYPE"];
            }
            set
            {
                this["STATION_TYPE"] = value;
            }
        }
        public string RETURN_FLAG
        {
            get
            {
                return (string)this["RETURN_FLAG"];
            }
            set
            {
                this["RETURN_FLAG"] = value;
            }
        }

    }
    public class C_ROUTE_DETAIL
    {
        public string ID;
        public double? SEQ_NO;
        public string ROUTE_ID;
        public string STATION_NAME;
        public string STATION_TYPE;
        public string RETURN_FLAG;
    }
}