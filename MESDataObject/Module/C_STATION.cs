using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_STATION : DataObjectTable
    {
        public T_C_STATION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_STATION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_STATION);
            TableName = "C_STATION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_STATION_DETAIL> ShowAllData(OleExec DB)
        {
            List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
            string sql = $@"select * from C_station";
            DataTable dt = new DataTable();
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    C_STATION_DETAIL ep = new C_STATION_DETAIL();
                    ep = RowToStation(item);
                    list.Add(ep);
                }
            }
            return list;
        }

        public List<C_STATION_DETAIL> GetDataByColumn(string column, string data, OleExec DB)
        {
            List<C_STATION_DETAIL> list = new List<C_STATION_DETAIL>();
            string sql = "";
            if (string.IsNullOrEmpty(column))
            {
                sql = $@"select * from C_station where station_name =:data";
            }
            else
            {
                sql = $@"select * from C_station where {column} =:data";
            }
            DataSet res = DB.ExecSelect(sql, new System.Data.OleDb.OleDbParameter[1] { new System.Data.OleDb.OleDbParameter("data", data) });
            DataTable dt = res.Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    C_STATION_DETAIL ep = new C_STATION_DETAIL();
                    ep = RowToStation(item);
                    list.Add(ep);
                }
            }
            return list;
        }

        private C_STATION_DETAIL RowToStation(DataRow item)
        {
            C_STATION_DETAIL ep = new C_STATION_DETAIL();
            ep.ID = item["ID"].ToString();
            ep.Station_Name = item["Station_Name"].ToString();
            ep.Type = item["Type"].ToString();
            return ep;
        }
    }
    public class Row_C_STATION : DataObjectBase
    {
        public Row_C_STATION(DataObjectInfo info) : base(info)
        {

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
    }

    public class C_STATION_DETAIL
    {
        public string ID { get; set; }
        public string Station_Name { get; set; }
        public string Type { get; set; }
    }
}
