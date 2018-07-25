using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.MESReturnView.Public;
using System.Data;
using MESStation.BaseClass;
using MESDBHelper;

namespace MESStation.LogicObject
{
    public class User : MesAPIBase
    {
        public string ID;
        public string FACTORY;
        public string EMP_NO;
        public string EMP_NAME;
        public string EMP_LEVEL;
        public string DPT_NAME;
        public string EMP_PWD;
        public void CreatMenuId(Newtonsoft.Json.Linq.JObject Data, MESDBHelper.OleExec SFCDB, MESStationReturn StationReturn)
        {

        }
   
    }
}
