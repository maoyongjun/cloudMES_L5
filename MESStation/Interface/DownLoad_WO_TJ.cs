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
            FunctionName = "Download_WO",
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


   
        /// <summary>
        /// DonwLoad WO From SAP
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void Download_WO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = this.DBPools["SFCDB"].Borrow();
            string WO = Data["WO"].ToString();
            string PLANT = Data["PLANT"].ToString();
            Sfcdb.BeginTrain();
            Download(WO, PLANT);
            DownloadDetail(WO, PLANT);
            Sfcdb.CommitTrain();
            StationReturn.Status = StationReturnStatusValue.Pass;
            StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000102");
  
        }

        public void Download(string StrWo, string Plant)
        {

            OleExec sfcdb;
            DataTable RFC_Table = new DataTable();

            T_R_WO_HEADER_TJ R_WO_HEADER;
        
 
            Dictionary<string, string> DicPara = new Dictionary<string, string>();
     
            sfcdb = this.DBPools["SFCDB"].Borrow();

           
            ZRFC_GET_PRO_HEADER8 ZRFC_GET_PRO_HEADER8 = new ZRFC_GET_PRO_HEADER8();
            ZRFC_GET_PRO_HEADER8.SetValues(StrWo, Plant);//NHGZ,WDN1//WDN1,WSL3
            ZRFC_GET_PRO_HEADER8.CallRFC();

            DataTable woheader =  ZRFC_GET_PRO_HEADER8.GetTableValue("PO");

             R_WO_HEADER = new T_R_WO_HEADER_TJ(sfcdb, DB_TYPE_ENUM.Oracle);

            Row_R_WO_HEADER_TJ rowRWOHeader = (Row_R_WO_HEADER_TJ)R_WO_HEADER.NewRow();
            if (woheader.Rows.Count > 0) {
            
                rowRWOHeader.ID = R_WO_HEADER.GetNewID(BU, sfcdb);
                rowRWOHeader.AUFNR = woheader.Rows[0]["AUFNR"].ToString();
                rowRWOHeader.WERKS = woheader.Rows[0]["WERKS"].ToString();
                rowRWOHeader.AUART = woheader.Rows[0]["AUART"].ToString();
                rowRWOHeader.MATNR = woheader.Rows[0]["MATNR"].ToString();
                rowRWOHeader.REVLV = woheader.Rows[0]["REVLV"].ToString();
                rowRWOHeader.KDAUF = woheader.Rows[0]["KDAUF"].ToString();
                rowRWOHeader.GSTRS = woheader.Rows[0]["GSTRS"].ToString();
                rowRWOHeader.DISPO = woheader.Rows[0]["DISPO"].ToString();
                rowRWOHeader.GAMNG = woheader.Rows[0]["GAMNG"].ToString();
                rowRWOHeader.VERID = woheader.Rows[0]["VERID"].ToString();
                rowRWOHeader.ARBPL = woheader.Rows[0]["ARBPL"].ToString();
                rowRWOHeader.KUNNR = woheader.Rows[0]["KUNNR"].ToString();
                rowRWOHeader.KDMAT = woheader.Rows[0]["KDMAT"].ToString();
                rowRWOHeader.AEDAT = woheader.Rows[0]["AEDAT"].ToString();
                rowRWOHeader.AENAM = woheader.Rows[0]["AENAM"].ToString();
                rowRWOHeader.MATKL = woheader.Rows[0]["MATKL"].ToString();
                rowRWOHeader.MAKTX = woheader.Rows[0]["MAKTX"].ToString();
                rowRWOHeader.GMEIN = woheader.Rows[0]["GMEIN"].ToString();
                rowRWOHeader.STATUS = woheader.Rows[0]["STATUS"].ToString();
                rowRWOHeader.ERDAT = woheader.Rows[0]["ERDAT"].ToString();
                rowRWOHeader.ERFZEIT = woheader.Rows[0]["ERFZEIT"].ToString();
                rowRWOHeader.AEZEIT = woheader.Rows[0]["AEZEIT"].ToString();
                rowRWOHeader.GSUZS = woheader.Rows[0]["GSUZS"].ToString();
                rowRWOHeader.RSNUM = woheader.Rows[0]["RSNUM"].ToString();
                rowRWOHeader.KBEASOLL = woheader.Rows[0]["KBEASOLL"].ToString();
                rowRWOHeader.TASKGROUP = woheader.Rows[0]["TASKGROUP"].ToString();
                rowRWOHeader.CY_SEQNR = woheader.Rows[0]["CY_SEQNR"].ToString();
                rowRWOHeader.ZAPLFL = woheader.Rows[0]["ZAPLFL"].ToString();
                rowRWOHeader.ZVORNR = woheader.Rows[0]["ZVORNR"].ToString();
                rowRWOHeader.ZTDLINE = woheader.Rows[0]["ZTDLINE"].ToString();
                rowRWOHeader.KDPOS = woheader.Rows[0]["KDPOS"].ToString();
                rowRWOHeader.BSTKD = woheader.Rows[0]["BSTKD"].ToString();
                rowRWOHeader.POSEX_E = woheader.Rows[0]["POSEX_E"].ToString();
                rowRWOHeader.TKNUM = woheader.Rows[0]["TKNUM"].ToString();
                rowRWOHeader.APRIO = woheader.Rows[0]["APRIO"].ToString();
                rowRWOHeader.MAUFNR = woheader.Rows[0]["MAUFNR"].ToString();
                rowRWOHeader.OBJNR = woheader.Rows[0]["OBJNR"].ToString();
                rowRWOHeader.FEVOR = woheader.Rows[0]["FEVOR"].ToString();
                rowRWOHeader.WEMNG = woheader.Rows[0]["WEMNG"].ToString();
                rowRWOHeader.ERNAM = woheader.Rows[0]["ERNAM"].ToString();
                rowRWOHeader.IDAT2 = woheader.Rows[0]["IDAT2"].ToString();
                rowRWOHeader.PHAS2 = woheader.Rows[0]["PHAS2"].ToString();
                rowRWOHeader.STLAL = woheader.Rows[0]["STLAL"].ToString();
                rowRWOHeader.STLAN = woheader.Rows[0]["STLAN"].ToString();
                rowRWOHeader.VDATU = woheader.Rows[0]["VDATU"].ToString();
                rowRWOHeader.VGW03 = woheader.Rows[0]["VGW03"].ToString();
                String WO = rowRWOHeader.AUFNR;
                string sql = $@"DELETE FROM R_WO_HEADER_TJ WHERE AUFNR = '{WO}'";
                sfcdb.ExecSQL(sql);
                sql = rowRWOHeader.GetInsertString(DB_TYPE_ENUM.Oracle);
                sfcdb.ExecSQL(sql);
            }



        }

        public void DownloadDetail(string StrWo, string Plant)
        {

            OleExec sfcdb;
            DataTable RFC_Table = new DataTable();


            Dictionary<string, string> DicPara = new Dictionary<string, string>();

            sfcdb = this.DBPools["SFCDB"].Borrow();

            ZRFC_GET_PRO_DETAIL ZRFC_GET_PRO_DETAIL = new ZRFC_GET_PRO_DETAIL();
            ZRFC_GET_PRO_DETAIL.SetValues(StrWo, Plant);//NHGZ,WDN1//WDN1,WSL3
            ZRFC_GET_PRO_DETAIL.CallRFC();

            DataTable woDetail = ZRFC_GET_PRO_DETAIL.GetTableValue("POD");


            T_R_WO_ITEM_TJ T_R_WO_ITEM_TJ = new T_R_WO_ITEM_TJ(sfcdb, DB_TYPE_ENUM.Oracle);

            Row_R_WO_ITEM_TJ rowRWODetail = (Row_R_WO_ITEM_TJ)T_R_WO_ITEM_TJ.NewRow();
            string sql = $@"DELETE FROM R_WO_ITEM_TJ WHERE AUFNR = '{StrWo}'";
            if (woDetail.Rows.Count > 0) {
                sfcdb.ExecSQL(sql);
            }

            foreach (DataRow R_woDetail in woDetail.Rows) {
                rowRWODetail.ID = T_R_WO_ITEM_TJ.GetNewID(BU, sfcdb);
                rowRWODetail.AUFNR = R_woDetail["AUFNR"].ToString();
                rowRWODetail.POSNR = R_woDetail["POSNR"].ToString();
                rowRWODetail.WERKS = R_woDetail["WERKS"].ToString();
                rowRWODetail.MATNR = R_woDetail["MATNR"].ToString();
                rowRWODetail.BDMNG = R_woDetail["BDMNG"].ToString();
                rowRWODetail.ENMNG = R_woDetail["ENMNG"].ToString();
                rowRWODetail.KDMAT = R_woDetail["KDMAT"].ToString();
                rowRWODetail.MEINS = R_woDetail["MEINS"].ToString();
                rowRWODetail.MATKL = R_woDetail["MATKL"].ToString();
                rowRWODetail.ALPGR = R_woDetail["ALPGR"].ToString();
                rowRWODetail.MAKTX = R_woDetail["MAKTX"].ToString();
                rowRWODetail.DUMPS = R_woDetail["DUMPS"].ToString();
                rowRWODetail.SOBSL = R_woDetail["SOBSL"].ToString();
                rowRWODetail.REVLV = R_woDetail["REVLV"].ToString();
                rowRWODetail.SHKZG = R_woDetail["SHKZG"].ToString();
                rowRWODetail.LGORT = R_woDetail["LGORT"].ToString();
                rowRWODetail.QUANTITY = R_woDetail["QUANTITY"].ToString();
                rowRWODetail.BAUGR = R_woDetail["BAUGR"].ToString();
                rowRWODetail.MENGE = R_woDetail["MENGE"].ToString();
                rowRWODetail.CHARG = R_woDetail["CHARG"].ToString();
                rowRWODetail.MOD_NO = R_woDetail["MOD_NO"].ToString();
                rowRWODetail.POSTP = R_woDetail["POSTP"].ToString();
                rowRWODetail.SORTF = R_woDetail["SORTF"].ToString();
                rowRWODetail.LIFNR = R_woDetail["LIFNR"].ToString();
                rowRWODetail.POTX1 = R_woDetail["POTX1"].ToString();
                rowRWODetail.POTX2 = R_woDetail["POTX2"].ToString();
               
              
                sql = rowRWODetail.GetInsertString(DB_TYPE_ENUM.Oracle);
                sfcdb.ExecSQL(sql);

            }


        }


    }
}
