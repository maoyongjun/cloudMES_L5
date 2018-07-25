using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="SNListByWo.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-03-07 </date>
    /// <summary>
    /// SNListByWo
    /// </summary>
    public class SNListByWo:ReportBase
    {
        ReportInput inputWo = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputEventName = new ReportInput() { Name = "EventName", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public SNListByWo() {
            Inputs.Add(inputWo);
            Inputs.Add(inputEventName);           
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            //base.Run();
            string wo = inputWo.Value.ToString();
            string eventName = inputEventName.Value.ToString().ToUpper();
            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();        
            DataRow linkRow = null;
            if (eventName.Equals("REPAIRWIP"))
            {
                sqlRun = $@"select distinct sn,next_station  as station,edit_time from r_sn where REPAIR_FAILED_FLAG = 1 and workorderno ='{wo}'";
            } 
            else if (eventName.Equals("MRB"))
            {
                sqlRun = $@"select distinct sn,'MRB' as station,edit_time  from r_mrb where workorderno = '{wo}'   and rework_wo is null";
            }
            else
            {
                //sqlRun = $@"select sn,next_station as station,edit_time  from r_sn where workorderno='{wo}' and next_station='{eventName}'";
                sqlRun = $@"select a.sn,a.next_station as station,a.edit_time,b.panel  from r_sn a,r_panel_sn b where a.workorderno='{wo}' and a.next_station='{eventName}' and a.sn=b.sn";
            }
           
            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                DBPools["SFCDB"].Return(SFCDB);
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("STATION");
                linkTable.Columns.Add("EDIT_TIME");
                linkTable.Columns.Add("PANEL");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN="+ snListTable.Rows[i]["SN"].ToString();
                    linkRow["STATION"] = "";
                    linkRow["EDIT_TIME"] = "";
                    linkRow["PANEL"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(snListTable, linkTable);
                reportTable.Tittle = "SNList";
                //reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);
                
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
