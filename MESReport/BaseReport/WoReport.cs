using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    //查詢工單信息
    public class WoReport:ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "002520000001", Enable = true, SendChangeEvent = false, ValueForUse = null };        
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL","N","Y" } };

       public WoReport()
        {
            Inputs.Add(WO);
            Inputs.Add(CloseFlag);
            //  string strGetSn = @"SELECT * FROM R_SN WHERE SN='{0}' OR BOXSN='{0}'";
            //   Sqls.Add("strGetSN", strGetSn);

        }

        public override void Run()
        {
            if (WO.Value == null)
            {
                throw new Exception("WO Can not be null");
            }
            //   string runSql = string.Format(Sqls["strGetSN"], WO.Value.ToString());
            //    RunSqls.Add(runSql);
            string wo = WO.Value.ToString();
           
            string columnName = "";
            string closeflag = CloseFlag.Value.ToString();
            string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListByWo&RunFlag=1&WO=" + wo + "&EventName=";
                                                 ;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
               string Sqlwo=$@"SELECT workorderno ,skuno,ROUTE_ID,WORKORDER_QTY , trunc ( sysdate - DOWNLOAD_DATE) DATS,INPUT_QTY,FINISHED_QTY FROM R_WO_BASE where 
                           WORKORDERNO = '{wo}' ";
                if (closeflag == "Y")
                {
                    Sqlwo = Sqlwo + " and CLOSED_FLAG = 1";
                }
                else if (closeflag == "N")
                {
                    Sqlwo = Sqlwo + " and CLOSED_FLAG = 0";
                }
                DataTable dtwo = SFCDB.RunSelect(Sqlwo).Tables[0];
                RunSqls.Add(Sqlwo);
                if (dtwo.Rows.Count==0)
                {
                    ReportAlart alart = new ReportAlart("No Data!"); 
                    Outputs.Add(alart);
                    return;
                }
                //string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[0]["route_id"].ToString()}' order by  seq_no";
                string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[0]["route_id"].ToString()}' order by  seq_no";
                DataTable dtroute = SFCDB.RunSelect(SqlRoute).Tables[0];
                RunSqls.Add(SqlRoute);

                string SqlStationRoute = $@" SELECT distinct next_station FROM r_sn where REPAIR_FAILED_FLAG <> 1 and(COMPLETED_FLAG = 0 or NEXT_STATION = 'JOBFINISH') and  workorderno = '{wo}'
                                            MINUS
                                            select STATION_NAME from c_route_detail where ROUTE_ID='{dtwo.Rows[0]["route_id"].ToString()}'  ";
                DataTable dtstationroute = SFCDB.RunSelect(SqlStationRoute).Tables[0];
                RunSqls.Add(SqlStationRoute);

                DataTable resdt = new DataTable();
                DataTable linkTable = new DataTable();

                resdt.Columns.Add("WorkOrderNo");
                resdt.Columns.Add("Skuno");
                resdt.Columns.Add("DATS");
                resdt.Columns.Add("QTY");

                linkTable.Columns.Add("WorkOrderNo");
                linkTable.Columns.Add("Skuno");
                linkTable.Columns.Add("DATS");
                linkTable.Columns.Add("QTY");

                for (int i = 0; i < dtroute.Rows.Count; i++)
                {
                    resdt.Columns.Add(dtroute.Rows[i]["STATION_NAME"].ToString());
                    linkTable.Columns.Add(dtroute.Rows[i]["STATION_NAME"].ToString());
                }

                for (int i = 0; i < dtstationroute.Rows.Count; i++)
                {
                     if (dtstationroute.Rows[i]["next_station"].ToString().Equals("JOBFINISH"))
                        continue;
                    resdt.Columns.Add(dtstationroute.Rows[i]["next_station"].ToString());
                    linkTable.Columns.Add(dtstationroute.Rows[i]["next_station"].ToString());
                }

                //   resdt.Columns.Add("STOCKIN");
                //   resdt.Columns.Add("JOBFINISH");
                //resdt.Columns.Add("NA");
                resdt.Columns.Add("RepairWip");
                resdt.Columns.Add("MRB");
                //resdt.Columns.Add("REWORK");
                resdt.Columns.Add("JOBFINISH");

                //linkTable.Columns.Add("NA");
                linkTable.Columns.Add("RepairWip");
                linkTable.Columns.Add("MRB");
                //linkTable.Columns.Add("REWORK");
                linkTable.Columns.Add("JOBFINISH");

                DataRow drd = resdt.NewRow();
                DataRow linkDataRow = linkTable.NewRow();
                drd["WorkOrderNo"] = wo;
                drd["Skuno"] = dtwo.Rows[0]["Skuno"].ToString();
                drd["DATS"] = dtwo.Rows[0]["DATS"].ToString();
                drd["QTY"] = dtwo.Rows[0]["WORKORDER_QTY"].ToString();
              //  drd["STOCKIN"]= dtwo.Rows[0]["FINISHED_QTY"].ToString();

                string Sqlsncount =$@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where  (REPAIR_FAILED_FLAG <> 1 or REPAIR_FAILED_FLAG is null) and(COMPLETED_FLAG = 0 or NEXT_STATION = 'JOBFINISH') 
                                         and  workorderno = '{wo}' group by NEXT_STATION";
                long loadingNum = 0;
                long mrbNum = 0;
                DataTable dtsncont = SFCDB.RunSelect(Sqlsncount).Tables[0];
                RunSqls.Add(Sqlsncount);
                for (int i = 0; i < dtsncont.Rows.Count; i++)
                {
                    drd[dtsncont.Rows[i]["NEXT_STATION"].ToString()] = dtsncont.Rows[i]["c"].ToString();
                    linkDataRow[dtsncont.Rows[i]["NEXT_STATION"].ToString()] = (dtsncont.Rows[i]["c"].ToString() != "0") ? (linkURL + dtsncont.Rows[i]["NEXT_STATION"].ToString()) : "";
                    loadingNum = loadingNum + Convert.ToInt64(dtsncont.Rows[i]["c"].ToString());
                }

                string SqlRepairCount = $@" select count(1) repaircount from r_sn where REPAIR_FAILED_FLAG = 1 and workorderno = '{wo}'";
                DataTable dtrepaircont = SFCDB.RunSelect(SqlRepairCount).Tables[0];
                RunSqls.Add(SqlRepairCount);
                drd["RepairWip"] = dtrepaircont.Rows[0]["repaircount"].ToString();
                linkDataRow["RepairWip"] = (dtrepaircont.Rows[0]["repaircount"].ToString() != "0") ? (linkURL + "RepairWip") : "";

                //string SqlMrbCount = $@"select count(1) mrbcount from r_mrb where workorderno = '{wo}'  and rework_wo is null";
                string SqlMrbCount = $@"select count(1) mrbcount from r_mrb where workorderno = '{wo}'  ";
                DataTable dtmrbcont = SFCDB.RunSelect(SqlMrbCount).Tables[0];
                RunSqls.Add(SqlMrbCount);
                drd["MRB"] = dtmrbcont.Rows[0]["mrbcount"].ToString();
                mrbNum = Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());
                linkDataRow["MRB"] = (dtmrbcont.Rows[0]["mrbcount"].ToString() != "0") ? (linkURL + "MRB") : "";

                //loadingNum = loadingNum + Convert.ToInt64(dtrepaircont.Rows[0]["repaircount"].ToString()) + Convert.ToInt64(dtmrbcont.Rows[0]["mrbcount"].ToString());
                foreach (DataColumn dc in resdt.Columns)
                {
                    if (dc.ColumnName.ToString().ToUpper().IndexOf("LOADING") > 0 || dc.ColumnName.ToString().ToUpper().IndexOf("LINK") > -1)
                    {
                        drd[dc.ColumnName.ToString()] = Convert.ToInt64(dtwo.Rows[0]["WORKORDER_QTY"].ToString()) - loadingNum - mrbNum;
                    }
                }

                resdt.Rows.Add(drd);
                linkTable.Rows.Add(linkDataRow);
                ReportTable retTab = new ReportTable();
                retTab.LoadData(resdt, linkTable);
                retTab.Tittle = "WO WIP";
                //retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                if (resdt.Rows.Count > 0)
                {
                    List<object> objList = new List<object>();
                    pieChart pie = new pieChart();
                    pie.Tittle ="工單"+ wo + "WIP分佈餅狀圖";
                    pie.ChartTitle = "主標題";
                    pie.ChartSubTitle = "副標題";
                    ChartData chartData = new ChartData();
                    chartData.name = "WOLIST";
                    chartData.type = ChartType.pie.ToString();
                    for (int j = 0; j < resdt.Rows.Count; j++)
                    {
                        foreach (DataColumn column in resdt.Columns)
                        {
                            columnName = column.ColumnName.ToString().ToUpper();
                            if (columnName != "WORKORDERNO" && columnName != "SKUNO" && columnName != "DATS" && columnName != "QTY" && resdt.Rows[j][columnName].ToString() != ""&& resdt.Rows[j][columnName].ToString() != "0")
                            {
                                objList.Add(new List<object> { columnName, Convert.ToInt64(resdt.Rows[j][columnName].ToString())});
                            }
                        }
                    }
                    chartData.data = objList;
                    chartData.colorByPoint = true;
                    List<ChartData> _ChartDatas = new List<ChartData> { chartData };
                    pie.ChartDatas = _ChartDatas;
                    Outputs.Add(pie);
                }
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }
    }
}
