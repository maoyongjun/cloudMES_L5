using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport
{
    public class ReportOutputBase
    {
        public int ColCount = 12;
        //
        public int ColNum = 0;
        //第幾行
        public int RowNun = 0;

        
    }

    /// <summary>
    /// 提供Alart
    /// </summary>
    public class ReportAlart : ReportOutputBase
    {
        public string Msg;
        public string AlartType = "warning";
        public String OutputType
        {
            get
            {
                return "ReportAlart";
            }
        }
        public ReportAlart(string _Msg){ Msg = _Msg; }
    }
}
