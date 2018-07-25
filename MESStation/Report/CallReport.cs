using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESReport;
using System.Reflection;
using MESDBHelper;

namespace MESStation.Report
{
    public class CallReport : MESStation.BaseClass.MesAPIBase
    {
        static Dictionary<string, ReportSession> Session = new Dictionary<string, ReportSession>();
        protected APIInfo _GetReport = new APIInfo()
        {
            FunctionName = "GetReport",
            Description = "獲取報表對象",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ClassName", InputType = "string", DefaultValue = "MESReport.Test.TEST1" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _RunReport = new APIInfo()
        {
            FunctionName = "RunReport",
            Description = "計算報表",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ClassName", InputType = "string", DefaultValue = "MESReport.Test.TEST1" },
                new APIInputInfo() {InputName = "Report", InputType = "string", DefaultValue = "MESReport.Test.TEST1" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public CallReport()
        {
            Apis.Add(_GetReport.FunctionName, _GetReport);
            Apis.Add(_RunReport.FunctionName, _RunReport);
        }

        public void GetReport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string ClassName = Data["ClassName"].ToString();
            if (ClassName == "")
            {
                throw new Exception("ClassName Not Set!");
            }
            Assembly assembly = Assembly.Load("MESReport");
            Type ReportType = assembly.GetType(ClassName);
            if(ReportType == null)
            {
                throw new Exception($@"Can Not Create {ClassName}!");
            }
            ReportBase Report = (MESReport.ReportBase)assembly.CreateInstance(ClassName);
            Report.DBPools = this.DBPools;
            Report.Init();
            StationReturn.Data = Report;
            StationReturn.Message = "";
            StationReturn.Status = StationReturnStatusValue.Pass;
        }

        public void RunReport(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string ClassName = Data["ClassName"].ToString();
            if (ClassName == "")
            {
                throw new Exception("ClassName Not Set!");
            }
            Assembly assembly = Assembly.Load("MESReport");
            Type ReportType = assembly.GetType(ClassName);
            if (ReportType == null)
            {
                throw new Exception($@"Can Not Create {ClassName}!");
            }
            ReportBase Report = (MESReport.ReportBase)assembly.CreateInstance(ClassName);
            Report.DBPools = this.DBPools;
            Report.Init();
            //循環加載input
            for (int i = 0; i < Report.Inputs.Count; i++)
            {
                ReportInput input = Report.Inputs[i];
                bool match = false;
                int mIndex = 0;
                for (int j = 0; j < Data["Report"]["Inputs"].Count(); j++)
                {
                    if (Data["Report"]["Inputs"][j]["Name"].ToString() == input.Name)
                    {
                        match = true;
                        mIndex = j;
                        break;
                    }
                }
                if (!match)
                {
                    //input.Value = null;
                    //input.ValueForUse = null;
                    continue;
                }
                try
                {

                    if (input.InputType == "DateTime")
                    {
                        //input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<DateTime>();
                        input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToString().Replace("%20", " ");
                        if(input.Value!=null)
                            input.Value =Convert.ToDateTime(input.Value);
                        input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<DateTime>>();
                    }
                    else
                    {
                        input.Value = Data["Report"]["Inputs"][mIndex]["Value"]?.ToObject<string>();
                        input.ValueForUse = Data["Report"]["Inputs"][mIndex]["ValueForUse"]?.ToObject<List<string>>();
                    }
                    input.Enable = Data["Report"]["Inputs"][mIndex]["Enable"]?.ToObject<bool>();
                }
                catch
                { }
            }
            Report.Run();

            StationReturn.Data = Report;
            StationReturn.Message = "";
            StationReturn.Status = StationReturnStatusValue.Pass;

        }

        class ReportSession
        {
            public string Token;
            public ReportBase Report;
            public DateTime LastEditTime;
        }

    }
}
