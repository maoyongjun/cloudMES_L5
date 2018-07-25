using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.BaseReport
{
    //OBA 報表
    public class OBAReport : ReportBase
    {
        ReportInput StartTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput EndTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public OBAReport()
        {
            Inputs.Add(StartTime);
            Inputs.Add(EndTime);
        }

        public override void Init()
        {
            StartTime.Value = DateTime.Now.AddDays(-1);
            EndTime.Value = DateTime.Now;

        }
        public override void Run()
        {

            DateTime stime = Convert.ToDateTime(StartTime.Value);
            DateTime etime = Convert.ToDateTime(EndTime.Value);
            string svalue = stime.ToString("yyyy/MM/dd HH:mm:ss");
            string evalue = etime.ToString("yyyy/MM/dd HH:mm:ss");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string sqlOba = $@"  SELECT* FROM R_LOT_STATUS WHERE EDIT_TIME BETWEEN TO_DATE('{svalue}', 'YYYY/MM/DD HH24:MI:SS')
                                  AND TO_DATE('{evalue}', 'YYYY/MM/DD HH24:MI:SS')";

                DataSet res = SFCDB.RunSelect(sqlOba);

                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "OBA";
                retTab.ColNames.RemoveAt(0);//不顯示ID列 add by fgg 2018.03.09
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ee;
            }
        }
    }
}
