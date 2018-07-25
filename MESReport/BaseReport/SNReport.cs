using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    //SN 信息報表
    public class SNReport : ReportBase
    {
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "DW03024QFA000100", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SNReport()
        {
            Inputs.Add(SN);
            //string strGetSn = @"SELECT * FROM R_SN WHERE SN='{0}' OR BOXSN='{0}'";
            string strGetSn = @"select sn,skuno,workorderno,plant,route_name,current_station,next_station,started_flag,start_time,completed_flag,
                                completed_time,packed_flag,packdate,shipped_flag,shipdate,repair_failed_flag,po_no,cust_order_no,cust_pn,boxsn,
                                scraped_flag,scraped_time,product_status,rework_count,valid_flag,stock_status,stock_in_time,a.edit_time 
                                from R_SN a,c_route b  where a.route_id=b.id and (sn='{0}' or boxsn='{0}')";
            Sqls.Add("strGetSN", strGetSn);
            string strGetSnDetail = @"SELECT  c.panel,a.sn,skuno,a.workorderno,plant,route_name,line,current_station,next_station,device_name,repair_failed_flag, started_flag,
                                    start_time,completed_flag,completed_time,packed_flag,packed_time,shipped_flag,shipdate,po_no,cust_order_no,cust_pn,boxsn,
                                    scraped_flag,scraped_time,product_status,rework_count,valid_flag,class_name, a.edit_time  FROM R_SN_STATION_DETAIL a,c_route b ,r_panel_sn c
                                    where a.sn=c.sn and a.route_id=b.id and a.SN IN ( SELECT SN FROM R_SN WHERE SN = '{0}' OR BOXSN ='{0}') order by a.edit_time ";
            Sqls.Add("strGetSnDetail", strGetSnDetail);
            string strGetSnKeypart = @"SELECT * FROM R_SN_KEYPART_DETAIL  WHERE R_SN_ID IN (SELECT ID
                     FROM R_SN  WHERE SN ='{0}' OR BOXSN = '{0}')";
            Sqls.Add("strGetSnKeypart", strGetSnKeypart);
        }

        public override void Run()
        {
            if (SN.Value == null)
            {
                throw new Exception("SN Can not be null");
            }
            string runSql = string.Format(Sqls["strGetSN"],SN.Value.ToString());
            string runSql1 = string.Format(Sqls["strGetSnDetail"], SN.Value.ToString());
            string runSql2 = string.Format(Sqls["strGetSnKeypart"], SN.Value.ToString());
            RunSqls.Add(runSql);
            RunSqls.Add(runSql1);
            RunSqls.Add(runSql2);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet res = SFCDB.RunSelect(runSql);
                DataSet res1 = SFCDB.RunSelect(runSql1);
                DataSet res2 = SFCDB.RunSelect(runSql2);
                ReportTable retTab = new ReportTable();
                DataTable dt = res.Tables[0].Copy();
                ReportLink link;


                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&=ALL&WO=" + row["workorderno"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "WORKORDERNO")
                        {
                            linkRow[dc.ColumnName] = linkURL;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }                    
                    linkTable.Rows.Add(linkRow);
                }



                foreach (DataRow dr in dt.Rows)
                {
                    // ClassName: 'sdsdsdw',Data: { Wo: 'sdsd',Station: '' }
                    //link = new ReportLink();
                    //link.ClassName = "MESReport.BaseReport.WoReport";
                    //link.Data.Add("WO", dr["workorderno"].ToString());
                    //link.Data.Add("CloseFlag","ALL");                    
                    //dr["WORKORDERNO"] = link;                   
                    //linkTable.Rows.Add(dr);                                     
                }
                

                retTab.LoadData(res.Tables[0], linkTable);



                retTab.Tittle = "OSN";
                //retTab.ColNames.RemoveAt(0);
                Outputs.Add(retTab);
                ReportTable retTab1 = new ReportTable();
                retTab1.LoadData(res1.Tables[0], null);
                retTab1.Tittle = "SN DETAIL";
                //retTab1.ColNames.RemoveAt(0);
                Outputs.Add(retTab1);
                ReportTable retTab2 = new ReportTable();
                retTab2.LoadData(res2.Tables[0], null);
                retTab2.Tittle = "SN KEYPARDT";
                retTab2.ColNames.RemoveAt(0);
                Outputs.Add(retTab2);


                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        }

    }
}
