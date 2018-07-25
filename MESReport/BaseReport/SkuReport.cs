using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
namespace MESReport.BaseReport
{
    public class SkuReport : ReportBase
    {

        ReportInput Skuno = new ReportInput { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        public SkuReport()
        {
            Inputs.Add(Skuno);
            Inputs.Add(CloseFlag);
            //  string strGetSn = @"SELECT * FROM R_SN WHERE SN='{0}' OR BOXSN='{0}'";
            //   Sqls.Add("strGetSN", strGetSn);
        }
        public override void Run()
        {
            if (Skuno.Value == null)
            {
                throw new Exception("SKUNO Can not be null");
            }
            string skuno = Skuno.Value.ToString();
            DataRow linkDataRow = null;
            string closeflag = CloseFlag.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string Sqlsku = $@"select workorderno,wo_type,skuno,sku_ver,workorder_qty from r_wo_base where skuno ='{skuno}'";
                if (closeflag == "Y")
                {
                    Sqlsku = Sqlsku + " and CLOSED_FLAG = 1";
                }
                else if (closeflag == "N")
                {
                    Sqlsku = Sqlsku + " and CLOSED_FLAG = 0";
                }

                DataTable dtsku = SFCDB.RunSelect(Sqlsku).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (dtsku.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }

                DataTable linkTable = new DataTable();

                linkTable.Columns.Add("workorderno");
                linkTable.Columns.Add("wo_type");
                linkTable.Columns.Add("skuno");
                linkTable.Columns.Add("sku_ver");
                linkTable.Columns.Add("workorder_qty");



                for (int i = 0; i < dtsku.Rows.Count; i++)
                {
                    linkDataRow = linkTable.NewRow();
                    //跳轉的頁面鏈接
                    linkDataRow["workorderno"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + dtsku.Rows[i]["workorderno"].ToString() + "&EventName=";
                    linkDataRow["wo_type"] = "";
                    linkDataRow["skuno"] = "";
                    linkDataRow["sku_ver"] = "";
                    linkDataRow["workorder_qty"] = "";
                    linkTable.Rows.Add(linkDataRow);

                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dtsku, linkTable);
                retTab.Tittle = "Skuno Report";
                Outputs.Add(retTab);
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
    }
}
