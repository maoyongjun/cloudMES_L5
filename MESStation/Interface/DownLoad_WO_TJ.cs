using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MESDataObject;
using MESDataObject.Module;
using MESStation;
using MESStation.BaseClass;
using MESDBHelper;
using System.Data;
using System.Timers;
using MESStation.Interface;
using MESStation.Interface.SAPRFC;
using MESInterface.SAP_RFC;

namespace MESStation.Interface
{
    class DownLoad_WO_TJ: MesAPIBase
    {
        T_C_INTERFACE C_Interface;
        Row_C_INTERFACE Row_C_Interface;
        OleExec Sfcdb;
        string IP = "";

        protected APIInfo FDownload = new APIInfo()
        {
            FunctionName = "Download WO For TjL5",
            Description = "Download WO For TjL5",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = "WO"},
                new APIInputInfo() {InputName = "PLANT", InputType = "STRING", DefaultValue = "PLANT"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public DownLoad_WO_TJ()
        {
            this.Apis.Add(FDownload.FunctionName, FDownload);
        }


        public void GetInterfaceStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string Program_Name = Data["PROGRAM"].ToString();
            List<C_INTERFACE> ListInterface = new List<C_INTERFACE>();
            OleExec Sfcdb = this.DBPools["SFCDB"].Borrow();
            C_Interface = new T_C_INTERFACE(Sfcdb, DB_TYPE_ENUM.Oracle);
            ListInterface = C_Interface.GetInterfaceStatus(BU, IP, Program_Name, "ALL", LoginUser.EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);
                        
            StationReturn.Data = ListInterface;
            StationReturn.Status = StationReturnStatusValue.Pass;
        }
        /// <summary>
        /// DonwLoad WO From SAP
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Download_WO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //Interface Interface=new Interface();

            bool BoolStart = Interface.TimerStarted;
            Interface.InterfaceTimerStart("Interface");

            OleExec Sfcdb = this.DBPools["SFCDB"].Borrow();
            string status = Data["STATUS"].ToString();
            string ProgramName = Data["PROGRAM_NAME"].ToString();
            string ItemName = Data["ItemName"].ToString();
            string WO = Data["WO"].ToString();
            string PLANT = Data["PLANT"].ToString();
            string IP = Data["IP"].ToString();
            Interface InterFace=new Interface();
            string Local_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();

            if (InterFace.CheckInterfaceRun(ProgramName, ItemName, Sfcdb))// Ensure only one interface console program run;
            {
                try
                {
                    InterFace.LockItem(ProgramName, ItemName, Sfcdb);
                    Download(WO, PLANT);
                    InterFace.UpdateNextRunTime(ProgramName, ItemName, Sfcdb);//Update Next RunTime;
                    InterFace.UnLockItem(ProgramName, ItemName, Sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000102");
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message);
                    InterFace.UnLockItem(ProgramName, ItemName, Sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = ex.Message.ToString();
                    throw (ex);
                }
            }
        }

        public void Download(string StrWo, string Plant)
        {
            string StrSql = "";
            OleExec sfcdb;
            DataTable RFC_Table = new DataTable();
            T_C_TAB_COLUMN_MAP C_TAB_COLUMN_MAP;
            DataObjectBase Row;
            T_R_WO_HEADER R_WO_HEADER;
            T_R_WO_ITEM R_WO_ITEM;
            T_R_WO_TEXT R_WO_TEXT;
            T_R_WO_BASE R_WO_BASE;
            T_C_SKU C_SKU;
 
            Dictionary<string, string> DicPara = new Dictionary<string, string>();
     
            sfcdb = this.DBPools["SFCDB"].Borrow();
            

            ZRFC_GET_PRO_HEADER8 ZRFC_GET_PRO_HEADER8 = new ZRFC_GET_PRO_HEADER8();
            ZRFC_GET_PRO_HEADER8.SetValues(StrWo, Plant);//NHGZ,WDN1//WDN1,WSL3
            ZRFC_GET_PRO_HEADER8.CallRFC();

            DataTable woheader =  ZRFC_GET_PRO_HEADER8.GetTableValue("PO");

            Console.Out.WriteLine(woheader);

        }


    }
}
