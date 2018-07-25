using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="TestReportBySN.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-05-11 </date>
    /// <summary>
    /// TestReportBySN
    /// </summary>
    public class TestReportBySN:ReportBase
    {

        ReportInput inputStartDate = new ReportInput() { Name = "StartDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEndDate = new ReportInput() { Name = "EndDate", InputType = "DateTime", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSN = new ReportInput() { Name = "SN", InputType = "TXT", Value = "02166621850G002C", Enable = true, SendChangeEvent = false, ValueForUse = null };      
        ReportInput inputStationName = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStateType = new ReportInput() { Name = "StateType", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "P", "F"} };
        string sqlGetSation = "";
        public TestReportBySN()
        {
            Inputs.Add(inputStartDate);
            Inputs.Add(inputEndDate);
            Inputs.Add(inputSN);
            Inputs.Add(inputStationName);
            Inputs.Add(inputStateType);
            sqlGetSation = "select distinct te_station from c_temes_station_mapping order by te_station";
            Sqls.Add("GetSation", sqlGetSation);
        }
        public override void Init()
        {
            //base.Init();
            inputStartDate.Value = DateTime.Now.AddDays(-1) ;
            inputEndDate.Value = DateTime.Now;
            inputStationName.ValueForUse = GetStation();
        }

        public override void Run()
        {
            //base.Run();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;

                string sn = inputSN.Value.ToString();
                string station = inputStationName.Value.ToString();
                string state = inputStateType.Value.ToString();
                string runSql = $@" select skuno,sn,state,station,cell,operator,error_code,createtime from r_test_detail_vertiv where 1=1 ";
                if(inputStartDate.Value.ToString()!="")
                {
                    startDate = (DateTime)inputStartDate.Value;
                }
                if (inputStartDate.Value.ToString() != "")
                {
                    endDate = (DateTime)inputEndDate.Value;
                }
                if (inputStartDate.Value.ToString() != "" && inputStartDate.Value.ToString() != "")
                {
                    runSql = runSql + $@"createtime   between to_date('{startDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') 
                                and  to_date('{endDate.ToString("yyyy-MM-dd HH-mm-ss")}','yyyy/mm/dd hh24:mi:ss') ";
                }
               
                if (sn != "")
                {
                    runSql = runSql + $@" and sn='{sn}' ";
                }
                if (station != "ALL")
                {
                    runSql = runSql + $@" and station='{station}' ";
                }
                if (state != "ALL")
                {
                    runSql = runSql + $@" and state='{state}' ";
                }
                RunSqls.Add(runSql);
                DataTable dtTestReport = SFCDB.RunSelect(runSql).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                DataTable linkTable = new DataTable();
                DataRow linkRow = null;
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("STATE");
                linkTable.Columns.Add("STATION");
                linkTable.Columns.Add("CELL");
                linkTable.Columns.Add("OPERATOR");
                linkTable.Columns.Add("ERROR_CODE");
                linkTable.Columns.Add("CREATETIME");
                for (int i = 0; i < dtTestReport.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + dtTestReport.Rows[i]["SN"].ToString();
                    linkRow["SKUNO"] = "";
                    linkRow["STATE"] = "";
                    linkRow["STATION"] = "";
                    linkRow["CELL"] = "";
                    linkRow["OPERATOR"] = "";
                    linkRow["ERROR_CODE"] = "";
                    linkRow["CREATETIME"] = "";
                    linkTable.Rows.Add(linkRow);
                }                
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dtTestReport, linkTable);
                reportTable.Tittle = "SN TEST REPORT";               
                Outputs.Add(reportTable);
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw exception;
            }
        }

        private List<string> GetStation()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtStation = SFCDB.RunSelect(Sqls["GetSation"]).Tables[0];
            List<string> stationList = new List<string>();
            stationList.Add("ALL");
            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            if (dtStation.Rows.Count > 0)
            {
                foreach (DataRow row in dtStation.Rows)
                {
                    stationList.Add(row["te_station"].ToString());
                }
            }
            else
            {
                throw new Exception("no station in system!");
            }
            return stationList;
        }
    }
}
