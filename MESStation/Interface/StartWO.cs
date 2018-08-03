using MESDataObject;
using MESDataObject.Module;
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
            T_R_WO_HEADER_TJ t_R_WO_HEADER_TJ = new T_R_WO_HEADER_TJ(Sfcdb,this.DBTYPE);
            Row_R_WO_HEADER_TJ row_R_WO_HEADER = t_R_WO_HEADER_TJ.GetWo(WO,Sfcdb);
            float qty = float.Parse(row_R_WO_HEADER.GAMNG);
            qty = 5;
            for (int i = 0; i < qty; i++) {
                String nextSN = SNmaker.GetNextSN("TEST", Sfcdb, WO);
                Console.Out.WriteLine(nextSN);
                T_R_SN t_r_sn = new T_R_SN(Sfcdb,this.DBTYPE);
                t_r_sn.addStartSNRecords(WO, nextSN, Sfcdb);
            }
            

            StationReturn.Data = qty;
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180801141046");

        }

    }
}
