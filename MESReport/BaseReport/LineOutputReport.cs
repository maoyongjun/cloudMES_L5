using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;
namespace MESReport.BaseReport
{
    //線別產出
    public class LineOutputReport:ReportBase
    {
        ReportInput Bu = new ReportInput() { Name = "Bu", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "HWD","DCN"} };
        ReportInput Station = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "AOI1", "AOI2", "VI1", "VI2" } };
        ReportInput Line = new ReportInput() { Name = "Line", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "D32S1", "D32S2"} };
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

     
        public LineOutputReport()
        {
            Inputs.Add(Bu);
            Inputs.Add(Station);
            Inputs.Add(Line);
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }

        public override void Init()
        {
            try
            {
                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                StartTime.Value = DateTime.Now.AddDays(-1);
                EndTime.Value = DateTime.Now;
                InitBu(SFCDB);
                InitStation(SFCDB);
                InitLine(SFCDB);

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Run()
        {

            string bu = Bu.Value.ToString();
            string station = Station.Value.ToString();
            string line = Line.Value.ToString();
            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sqlline = $@"  SELECT line, skuno, CURRENT_STATION, count(1) pass 
                               FROM r_sn_station_detail WHERE REPAIR_FAIL_FLAG = 0 ";
                if (line != "ALL")
                {
                    sqlline = sqlline +$@" and line = '{line}'";
                }
                if (station != "ALL")
                {
                    sqlline = sqlline + $@"AND current_station = '{station}'";
                }
                sqlline = sqlline + $@"AND START_TIME BETWEEN TO_DATE('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                AND TO_DATE('{evalue}','YYYY/MM/DD HH24:MI:SS')
                                GROUP BY line, skuno, CURRENT_STATION order by line";
               DataSet res = SFCDB.RunSelect(sqlline);

                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);

                retTab.Tittle = "Line Output";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }

        }

        public void InitBu(OleExec db)
        {
            DataTable dt = new DataTable();
            List<string> allbu = new List<string>();
            T_C_BU bu = new T_C_BU(db,DB_TYPE_ENUM.Oracle);
            dt = bu.GetAllBu(db);
            allbu.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                allbu.Add(dr["BU"].ToString());
            }
            Bu.ValueForUse = allbu;
        }

        public void InitStation(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            T_C_ROUTE_DETAIL S = new T_C_ROUTE_DETAIL(db, DB_TYPE_ENUM.Oracle);
            dt = S.GetALLStation(db);
            station.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                station.Add(dr["station_name"].ToString());

            }
            Station.ValueForUse = station;
        }

        public void InitLine(OleExec db)
        {
            List<string> line = new List<string>();
            DataTable dt = new DataTable();
            T_C_LINE S = new T_C_LINE(db, DB_TYPE_ENUM.Oracle);
            dt = S.GetAllLine(db);
            line.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                line.Add(dr["line_name"].ToString());

            }
            Line.ValueForUse = line;
        }

        // GetAllLine

    }
}
