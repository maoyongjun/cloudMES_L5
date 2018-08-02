using MESDataObject;
using MESDBHelper;
using MESStation.BaseClass;
using MESStation.SNMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Interface
{
    class StartWO : MesAPIBase
    {
        protected APIInfo info = new APIInfo()
        {
            FunctionName = "startWO",
            Description = "start WO For TjL5",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = "WO"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        public StartWO()
        {
            this.Apis.Add(info.FunctionName, info);
        }

        public void startWO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = this.DBPools["SFCDB"].Borrow();
            string WO = Data["WO"].ToString();

            String nextSN = SNmaker.GetNextSN("TEST", Sfcdb,WO);

            StationReturn.Data = nextSN;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180801141046");

        }

    }
}
