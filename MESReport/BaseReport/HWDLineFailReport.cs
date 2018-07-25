using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="HWDLineFailReport.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// HWDLineFailReport
    /// </summary>
    public class HWDLineFailReport : ReportBase
    {
        ReportInput startTime = new ReportInput() { Name = "StartTime", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput endTime = new ReportInput() { Name = "EndTime", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public HWDLineFailReport()
        {
            Inputs.Add(startTime);
            Inputs.Add(endTime);

        }

        public override void Init()
        {
            startTime.Value = DateTime.Now.AddDays(-1);
            endTime.Value = DateTime.Now;
        }

        public object GetChartDataSourse(string BTime,string ETime,DataTable dt)
        {
            columnChart retChart_column = new columnChart();
            retChart_column.Tittle = "HWDLineFailReport";
            retChart_column.ChartTitle = "HWD"+ BTime +"-"+ ETime + "生產投入統計圖";
            retChart_column.ChartSubTitle = "線別/機種產出趨勢圖";
            XAxis _XAxis = new XAxis();
            _XAxis.Title = "機種";
            //_XAxis.Categories = new List<string> { "B32S1","B32S2","B32S3","B32S4"};
            //_XAxis.XAxisType = XAxisType.datetime;
            retChart_column.XAxis = _XAxis;
            retChart_column.Tooltip = "Pic";

            Yaxis _YAxis = new Yaxis();
            _YAxis.Title = "投入數";
            retChart_column.YAxis = _YAxis;

            ChartData ChartData1 = new ChartData();
            ChartData1.name = "HWD 產出統計";
            ChartData1.type = ChartType.column.ToString();
            ChartData1.colorByPoint = true;
            List<object> chartDataSourse = new List<object>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                columnData columnData = new columnData();
                columnData.name = dt.Rows[i]["料號"].ToString();
                columnData.y = Convert.ToInt32(dt.Rows[i]["投入"]);
                chartDataSourse.Add(columnData);
            }
            ChartData1.data = chartDataSourse;
            List<ChartData> _ChartDatas = new List<ChartData> { ChartData1 };
            retChart_column.ChartDatas = _ChartDatas;
            return retChart_column;
        }

        public override void Run()
        {           
            DateTime startDT = (DateTime)startTime.Value;
            DateTime endDT = (DateTime)endTime.Value;
            string dateFrom = $@"to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            string dateTO = $@"to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            string sqlRun = $@"select line ,skuno 料號,input 投入, fail 不良總數,decode(failrate,0,'0',to_char(round(failrate * 100, 2),'fm9999990.9999')) ||'%' as 不良率
                                from (select d.line,d.skuno,count(distinct d.sn) input,count(distinct f.sn) fail,
                                count(distinct f.sn) / count(distinct d.sn) failrate from (
                                    select a.sn, a.skuno, b.line from r_sn_station_detail a
                                    inner join r_sn_station_detail b on a.sn = b.sn
                                    inner join c_sku c on a.skuno = c.skuno
                                    where a.edit_time between {dateFrom} and {dateTO} and a.current_station = 'BIP' and b.current_station = 'AOI1') d
                                    left join (select sn from r_repair_main e where e.create_time between {dateFrom} and {dateTO}) f
                                on d.sn = f.sn group by d.skuno, d.line ) order by line , FAILRATE desc";

            #region 原報表查詢語句
            //select line 線別, skuno 料號, input 投入, fail 不良, substr(FAILRATE, 1, 4) || '%' 不良率
            //  from(select d.line,
            //               d.skuno,
            //               count(distinct d.sn) input,
            //               count(distinct f.sn) fail,
            //               count(distinct f.sn) / count(distinct d.sn) * 100 FAILRATE
            //          from(select a.sysserialno SN, a.skuno SKUNO, b.productionline LINE
            //                  from mfsysevent a
            //                 inner
            //                  join mfsysevent b
            //                    on a.sysserialno = b.sysserialno
            //                 inner
            //                  join sfccodelike c
            //                    on a.skuno = c.skuno
            //                 where a.scandatetime between
            //                       to_date('', 'yyyy/mm/dd hh24:mi:ss') and
            //                       to_date('', 'yyyy/mm/dd hh24:mi:ss')
            //                   and a.eventname = 'BIP'
            //                   and b.eventname = 'AOI1') d
            //          left join(select e.sysserialno SN
            //                      from sfcrepairmain e
            //                     where e.createdate between
            //                           to_date('',
            //                                   'yyyy/mm/dd hh24:mi:ss') and
            //                           to_date('',
            //                                   'yyyy/mm/dd hh24:mi:ss')) f
            //            on d.sn = f.sn
            //         group by d.skuno, d.line)
            // order by line , FAILRATE desc;
            #endregion

            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                DataSet dsLineFial = SFCDB.RunSelect(sqlRun);
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dsLineFial.Tables[0], null);
                reportTable.Tittle = "LineFailTable";
                Outputs.Add(reportTable);
                if (dsLineFial.Tables[0].Rows.Count > 0)
                    Outputs.Add(GetChartDataSourse(startTime.Value.ToString(), endTime.Value.ToString(), dsLineFial.Tables[0]));                
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
