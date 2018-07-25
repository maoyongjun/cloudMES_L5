using MESDataObject.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESStation.LogicObject;
using MESStation.MESReturnView.Station;
using System.Collections;
using MESDataObject;
using System.Data;
using MESDBHelper;
using MESStation.Stations.StationActions.DataLoaders;
using System.Data.OleDb;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class APDataAction
    {
        /// <summary>
        /// HWD Allpart鋼網計數，add by LLF 2018-01-29
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void APStencilUpdateCountAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {


            OleExec apdb = Station.APDB;
            string Psn = Input.Value.ToString();
            string Line = Station.Line;
            string StationName = Station.StationName;
         
            OleDbParameter[] StencilSP = new OleDbParameter[4];
            StencilSP[0] = new OleDbParameter("IN_P_SN", Psn);
            StencilSP[1] = new OleDbParameter("IN_STATION", StationName);
            StencilSP[2] = new OleDbParameter("IN_LINE", Line);
            StencilSP[3] = new OleDbParameter();
            StencilSP[3].Size = 1000;
            StencilSP[3].ParameterName = "RES";
            StencilSP[3].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.CHECK_STENCIL_COUNT_UPDATE", StencilSP);
            if (result == "OK")
            {
                //apdbPool.Return(apdb);
                Station.AddMessage("MES00000062", new string[] { Psn }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                //apdbPool.Return(apdb);
                throw new Exception(result);
            }            
        }

        public static void APPanelSNReplaceAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string PanelSN;
            SN SNObj = new SN();

            string StrSN = "";
            R_PANEL_SN Psn = null;

            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            PanelSN = PanelSession.InputValue.ToString();
            Psn = SNObj.GetPanelVirtualSN(PanelSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            StrSN = SNSession.Value.ToString();



            OleExec apdb = Station.APDB;
            //Psn = PanelSession.InputValue.ToString();

            OleDbParameter[] StencilSP = new OleDbParameter[4];
            StencilSP[0] = new OleDbParameter("G_PANEL", "");
            StencilSP[1] = new OleDbParameter("G_PSN", StrSN);
            StencilSP[2] = new OleDbParameter();
            StencilSP[2].Size = 1000;
            StencilSP[2].ParameterName = "RES";
            StencilSP[2].Direction = System.Data.ParameterDirection.Output;
            string result = apdb.ExecProcedureNoReturn("MES1.Z_PANEL_REPLACE_SP", StencilSP);
            if (result == "OK")
            {
                //apdbPool.Return(apdb);
                Station.AddMessage("MES00000062", new string[] { "" }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                //apdbPool.Return(apdb);
                throw new Exception(result);
            }
        }

        /// HWD PTH Allpart扣料，add by LLF 2018-02-19
        /// </summary>
        public static void APAssignMaterialPTHAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
//            OleExecPool apdbPool = null;
            OleExec apdb = null;

            try
            {

                apdb = Station.APDB;
                string Line = Station.Line;
                string StationName = string.Empty;
                string ErrMessage = string.Empty;
                string StrWO = string.Empty;
                string StationNum = string.Empty;
                string TRCode = string.Empty;
                string SN = string.Empty;

                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                SN = SNSession.Value.ToString();

                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (WOSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                StrWO = WOSession.Value.ToString();

                MESStationSession StationNumSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (StationNumSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }
                StationNum = StationNumSession.Value.ToString();
                StationName = Station.Line + Station.StationName + StationNum;

                MESStationSession PTHTRCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (PTHTRCodeSession == null)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                    throw new MESReturnMessage(ErrMessage);
                }

                TRCode = PTHTRCodeSession.Value.ToString();

                OleDbParameter[] AssignMaterialPTHSP = new OleDbParameter[6];
                AssignMaterialPTHSP[0] = new OleDbParameter("g_type", "0010");
                AssignMaterialPTHSP[1] = new OleDbParameter("g_wo", StrWO);
                AssignMaterialPTHSP[2] = new OleDbParameter("g_station", StationName);
                AssignMaterialPTHSP[3] = new OleDbParameter("g_tr_code", TRCode);
                AssignMaterialPTHSP[4] = new OleDbParameter("g_psn", SN);
                AssignMaterialPTHSP[5] = new OleDbParameter();
                AssignMaterialPTHSP[5].Size = 1000;
                AssignMaterialPTHSP[5].ParameterName = "RES";
                AssignMaterialPTHSP[5].Direction = System.Data.ParameterDirection.Output;
                string result = apdb.ExecProcedureNoReturn("MES1.assign_material_pth", AssignMaterialPTHSP);
                if (result.IndexOf("OK") >= 0)
                {
                    OleDbParameter[] AssignMaterialPTH0020 = new OleDbParameter[6];
                    AssignMaterialPTH0020[0] = new OleDbParameter("g_type", "0020");
                    AssignMaterialPTH0020[1] = new OleDbParameter("g_wo", StrWO);
                    AssignMaterialPTH0020[2] = new OleDbParameter("g_station", StationName);
                    AssignMaterialPTH0020[3] = new OleDbParameter("g_tr_code", TRCode);
                    AssignMaterialPTH0020[4] = new OleDbParameter("g_psn", SN);
                    AssignMaterialPTH0020[5] = new OleDbParameter();
                    AssignMaterialPTH0020[5].Size = 1000;
                    AssignMaterialPTH0020[5].ParameterName = "RES";
                    AssignMaterialPTH0020[5].Direction = System.Data.ParameterDirection.Output;
                    result = apdb.ExecProcedureNoReturn("MES1.assign_material_pth", AssignMaterialPTH0020);
                    //apdbPool.Return(apdb);
                    if (result.IndexOf("OK") == -1)
                    {
                        throw new Exception(result);
                    }
                }
                else
                {
                    //apdbPool.Return(apdb);
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                //if (apdb != null)
                //{
                //    //apdbPool.Return(apdb);
                //}
                throw new Exception(ex.Message.ToString());
            }

        }
        /// HWD PTH Allpart扣料，add by LLF 2018-02-19

        /// <summary>
        /// 更新TR_SN數據加載，查詢R_TR_SN,R_TR_SN_WIP的數據保存到Dictionary<string_Datarow>中,key為表名 "R_TR_SN","R_TR_SN_WIP"
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void TRSNDataSessionUpdateAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Dictionary<string, DataRow> APInfo = new Dictionary<string, DataRow>();
            string strTRSN = "";
            string ErrMessage = "";
            OleExec apdb = null;
            int LinkQty = 0;
            int TrSNExtQty = 0;
            if (Paras.Count < 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            strTRSN = TRSN_Session.InputValue.ToString();

            MESStationSession TRSNExtQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (TRSNExtQty_Session == null)
            {

                TRSNExtQty_Session = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNExtQty_Session);
                TRSNExtQty_Session.Value = 0;
            }

            MESStationSession TRSNPcbSku_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TRSNPcbSku_Session == null)
            {

                TRSNPcbSku_Session = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNPcbSku_Session);
                TRSNPcbSku_Session.Value = "";
            }

            MESStationSession LinkQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (LinkQty_Session != null)
            {
                int.TryParse(LinkQty_Session.Value.ToString(), out LinkQty);
            }

            //獲取ALLPART數據
            AP_DLL APDLL = new AP_DLL();
            try
            {
                apdb = Station.APDB;

                List<DataRow> TRSNWIPlist = APDLL.R_TR_SN_WIP_GetBYTR_SN(strTRSN, apdb);
                if (TRSNWIPlist.Count > 0)
                {
                    TRSNExtQty_Session.Value = TRSNWIPlist[0]["EXT_QTY"];
                    TRSNPcbSku_Session.Value = TRSNWIPlist[0]["KP_NO"];
                }
                else
                {
                    TRSNExtQty_Session.Value = 0;
                }

                //Station.DBS["APDB"].Return(apdb);

                int.TryParse(TRSNExtQty_Session.Value.ToString(), out TrSNExtQty);
                if (TrSNExtQty < LinkQty)
                {
                    MESStationInput StationInput = Station.Inputs.Find(t => t.DisplayName == "TR_SN");
                    StationInput.Enable = true;
                    Station.NextInput = StationInput;
                }
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    //Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }
        }
    }
}
