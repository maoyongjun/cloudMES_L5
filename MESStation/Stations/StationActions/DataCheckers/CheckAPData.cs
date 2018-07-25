using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESDataObject;
using MESDBHelper;
using MESStation.MESReturnView.Station;
using MESStation.Stations.StationActions.DataLoaders;
using System.Data.OleDb;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckAPData
    {
        /// <summary>
        /// SMTLoadingTRSN狀態檢查,
        /// 2018/1/3 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTTRSNStateDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //return;
            int EXTQTY = 0;//TRSN剩餘數量
            string TRSN_WORKFLAG = "0";//上線標誌
            string TRSNWIP_WORKFLAG = "0";//上線標誌
            string TRSN_LOCATION_FLAG = "";//上線標誌2
            string WIPSKU = "";//r_tr_sn_wip 表中的料號
           List<string> LINKSKU =new List<string>();//r_pcba_link表中的料號
            int LINKQTY = 0;//連板數量          
            Dictionary<string, List<DataRow>> APInfo = new Dictionary<string, List<DataRow>>();
            Dictionary<string, DataRow> TRInfo = new Dictionary<string, DataRow>();
            string strTRSN = "";
            string StrTrSNExtQTY = "";  //add by LLF 2018-03
            int TrSN_EXTQTY = 0;//TRSN剩餘數量,add by LLF 2018-03

            if (Paras.Count != 5)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (TRSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {               
                if (TRSN_Session.Value != null)
                {
                    //重新加載                  
                    APInfoDataloader.TRSNDataloader(Station, TRSN_Session.ResetInput, new List<R_Station_Action_Para>() { Paras[0] });
                    TRInfo = (Dictionary<string, DataRow>)TRSN_Session.Value;
                    if (TRInfo.Keys.Contains("R_TR_SN"))
                    {
                        if (TRInfo["R_TR_SN"] == null)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY+" T_R_SN" }));
                        }
                        else
                        {
                            strTRSN = TRInfo["R_TR_SN"]["TR_SN"].ToString();
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                    }
                    if (TRInfo.Keys.Contains("R_TR_SN_WIP"))
                    {
                        if (TRInfo["R_TR_SN_WIP"] == null)
                        {
                            // throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY +" T_R_SN_WIP"}));
                            //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN_WIP" }));
                            //Add by LLF 2018-03
                            StrTrSNExtQTY = TRInfo["R_TR_SN"]["EXT_QTY"].ToString().Trim();
                            StrTrSNExtQTY = (StrTrSNExtQTY == "") ? "0" : StrTrSNExtQTY;
                            TrSN_EXTQTY = Convert.ToInt32(StrTrSNExtQTY);
                            if (TrSN_EXTQTY <= 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000073", new string[] { strTRSN }));
                            }

                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { "TRSN:" + strTRSN, "R_TR_SN_WIP" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN_WIP" }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }
           
            MESStationSession APCONFIG_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (APCONFIG_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (APCONFIG_Session.Value != null)
                {
                    APInfo = (Dictionary<string,List<DataRow>>)APCONFIG_Session.Value;
                    if (APInfo.Keys.Contains("R_PCBA_LINK"))
                    {
                        if (APInfo["R_PCBA_LINK"] == null|| APInfo["R_PCBA_LINK"].Count<=0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " R_PCBA_LINK" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " R_PCBA_LINK" }));
                    }
                    if (APInfo.Keys.Contains("C_PRODUCT_CONFIG"))
                    {
                        if (APInfo["C_PRODUCT_CONFIG"] == null|| APInfo["C_PRODUCT_CONFIG"].Count<=0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " C_PRODUCT_CONFIG" }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY + " C_PRODUCT_CONFIG" }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
            }
            MESStationSession LinkQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (LinkQTY_Session == null)
            {
                LinkQTY_Session = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(LinkQTY_Session);
            }
            MESStationSession TRSNEXTQTY_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (TRSNEXTQTY_Session == null)
            {
                TRSNEXTQTY_Session = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNEXTQTY_Session);
            }
            MESStationSession TRSNPcbSku_Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (TRSNPcbSku_Session == null)
            {
                TRSNPcbSku_Session = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(TRSNPcbSku_Session);
            }
            DataRow R_TR_SN_Row = TRInfo["R_TR_SN"];
            DataRow R_TR_SN_WIP_Row = TRInfo["R_TR_SN_WIP"];
            string strextQTY = R_TR_SN_WIP_Row["EXT_QTY"].ToString().Trim();
            strextQTY = (strextQTY == "") ? "0" : strextQTY;
            EXTQTY = Convert.ToInt32(strextQTY);
            TRSNEXTQTY_Session.Value = EXTQTY;
            TRSN_WORKFLAG = (R_TR_SN_Row["WORK_FLAG"] == null) ? "" : R_TR_SN_Row["WORK_FLAG"].ToString().Trim();
            TRSN_LOCATION_FLAG = (R_TR_SN_Row["LOCATION_FLAG"] == null) ? "" :R_TR_SN_Row["LOCATION_FLAG"].ToString().Trim();
            TRSNWIP_WORKFLAG = (R_TR_SN_WIP_Row["WORK_FLAG"] == null) ? "" : R_TR_SN_WIP_Row["WORK_FLAG"].ToString().Trim();
            WIPSKU = R_TR_SN_WIP_Row["KP_NO"].ToString();
            List<DataRow> R_PCBA_LINK_Row_List = APInfo["R_PCBA_LINK"];
            List<DataRow> C_PRODUCT_CONFIG_Row_List = APInfo["C_PRODUCT_CONFIG"];
            TRSNPcbSku_Session.Value = WIPSKU; //add by LLF 2018-03
            foreach (DataRow pcbLingRow in R_PCBA_LINK_Row_List)
            {
                LINKSKU.Add(pcbLingRow["PCBA_SKUNO"].ToString());
            }
            string strLinkQTY = C_PRODUCT_CONFIG_Row_List[0]["LINK_QTY"].ToString().Trim();
            strLinkQTY = (strLinkQTY == "") ? "0" : strLinkQTY;
            LINKQTY = Convert.ToInt32(strLinkQTY);
            LinkQTY_Session.Value = LINKQTY;
            try
            {
                if (EXTQTY <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000073", new string[] { R_TR_SN_Row["TR_SN"].ToString() }));
                }
                if (TRSN_WORKFLAG != "0"|| TRSNWIP_WORKFLAG!="0")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000074", new string[] { R_TR_SN_Row["TR_SN"].ToString() }));
                }
                if (TRSN_LOCATION_FLAG != "2")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000140", new string[] { R_TR_SN_Row["TR_SN"].ToString(), TRSN_LOCATION_FLAG }));
                }
                if ( !LINKSKU.Contains(WIPSKU))
                {
                    string strLinkSku = "";
                    for (int i=0;i<LINKSKU.Count;i++)
                    {
                        if (strLinkSku == "")
                        {
                            strLinkSku = strLinkSku + LINKSKU[i];
                        }
                        else
                        {
                            strLinkSku = strLinkSku+"," + LINKSKU[i];
                        }
                    }
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000075", new string[] { R_TR_SN_Row["TR_SN"].ToString(), WIPSKU, strLinkSku }));
                }
            }
            catch(Exception ex)
            {
                TRSNEXTQTY_Session.Value = 0;
                LinkQTY_Session.Value = 0;
                throw ex;
            }             
            Station.AddMessage("MES00000001", new string[] { R_TR_SN_Row["TR_SN"].ToString() }, MESReturnView.Station.StationMessageState.Pass);
           
        }
        /// <summary>
        /// 連板數量檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        //Add by LLF 2018-01-26 Begin
        public static void SMTLoadingLinkQtyChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                MESStationSession AP_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (AP_LinkQty == null)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000133", new string[] { }));
                }

                if (Convert.ToInt16(AP_LinkQty.Value.ToString()) <= 0)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000137", new string[] { AP_LinkQty.Value.ToString() }));
                }

                MESStationSession Input_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (Input_LinkQty == null)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000132", new string[] { }));
                }

                if (Convert.ToInt16(Input_LinkQty.Value.ToString()) > Convert.ToInt16(AP_LinkQty.Value.ToString()))
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000131", new string[] { Input_LinkQty.Value.ToString(), AP_LinkQty.Value.ToString() }));
                }
                Station.Inputs[3].Enable = true;
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }
        //Add by LLF 2018-01-26 End

        /// <summary>
        /// 檢查當前工單是否上料齊套
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTPanelNoCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WO_Session = Station.StationSession.Find(t => t.MESDataType == "WO" && t.SessionKey == "1");
            if (WO_Session == null)
            {
                Station.AddMessage("MES00000007", new string[] { "WO" }, StationMessageState.Fail);
                return;
            }
            string wo = WO_Session.Value.ToString();

            
            OleExec apdb = Station.APDB;
            //string msg = apdb.ExecProcedureNoReturn("", null);
            if (apdb != null)
            {
                
            }

        }

        /// <summary>
        /// 檢查當前工單的錫膏是否上線:
        ///HWD CHECK&補Allparts錫膏資料（連板&非連板均調用該SP）
        ///MES1.CHECK_SOLDER_INSERTDATA(Panelno,Nextevent,L_tmp_line,)
        ///var_message返回 OK則OK, 反之，throw(ErrorMessage)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTSolderDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //Marked by LLF 2018-01-29
            //if (Paras.Count == 0)
            //{
            //    throw new Exception("參數數量不正確!");
            //}
            
            OleExec apdb = Station.APDB;
            string PsnInsert = Input.Value.ToString();
            string Line = Station.Line;
            List<R_SN> ListRsn = new List<R_SN>();
            T_R_SN RSn = new T_R_SN(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            ListRsn = RSn.GetRSNbyPsn(PsnInsert, Station.SFCDB);
            //Modify BY LLF 20108-01-29,應該是獲取當前工站
            //string Next_Station = ListRsn[0].NEXT_STATION;
            string StationName = Station.StationName;
            OleDbParameter[] SolderSP = new OleDbParameter[4];
            SolderSP[0] = new OleDbParameter("G_PSN", PsnInsert);
            SolderSP[1] = new OleDbParameter("G_EVENTNAME", StationName);
            SolderSP[2] = new OleDbParameter("G_LINE", Line);
            SolderSP[3] = new OleDbParameter();
            SolderSP[3].Size = 1000;
            SolderSP[3].ParameterName = "RES";
            SolderSP[3].Direction = System.Data.ParameterDirection.Output;
            SolderSP[3].Size = 200;
            string result = apdb.ExecProcedureNoReturn("MES1.CHECK_SOLDER_INSERTDATA", SolderSP);
            if (result == "OK")
            {
                
                Station.AddMessage("MES00000062", new string[] { PsnInsert }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                
                throw new Exception(result);
            }
        }

        /// <summary>
        /// 檢查當前工單的鋼網是否上線
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTStencilDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception("參數數量不正確!");
            }

            
            OleExec apdb = Station.APDB;

            if (apdb != null)
            {
                
            }
        }
        /// <summary>
        /// HWD Allparts AOI測試資料檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// MES1.CHECK_AOI_STATUS@mbdallpart(VAR_PANELNO,var_nextevent,var_productionline,var_LASTEDITBY,var_message )
        /// (G_SYSSERIALNO IN VARCHAR2,
        public static void AOITestAPDatachecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            
            OleExec apdb = Station.APDB;
            string Psn = Input.Value.ToString();
            string Line = Station.Line;
            string StationName = Station.StationName;
            string EMP_NO = Station.LoginUser.EMP_NO;

            OleDbParameter[] StencilSP = new OleDbParameter[5];
            StencilSP[0] = new OleDbParameter("G_SYSSERIALNO", Psn);
            StencilSP[1] = new OleDbParameter("G_EVENTNAME", StationName);
            StencilSP[2] = new OleDbParameter("G_LINE_NAME", Line);
            StencilSP[3] = new OleDbParameter("G_EMP", EMP_NO);
            StencilSP[4] = new OleDbParameter();
            StencilSP[4].Size = 1000;
            StencilSP[4].ParameterName = "RES";
            StencilSP[4].Direction = System.Data.ParameterDirection.Output;
            //string result = apdb.ExecProcedureNoReturn("MES1.CHECK_AOI_STATUS@mbdallpart", StencilSP);
            string result = apdb.ExecProcedureNoReturn("MES1.CHECK_AOI_STATUS", StencilSP);
            if (result == "OK")
            {
                
                Station.AddMessage("MES00000062", new string[] { Psn }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                
                throw new Exception(result);
            }
        }

        /// <summary>
        /// SMT Loading 檢查allpart條碼的數量是否大於輸入的link數量,AllPart條碼的數量是否是連半數的整倍數
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTLoadingCheckLinkQtyAndTRQty(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            try
            {
                int trsnQty = 0;
                Dictionary<string, DataRow> TRInfo = new Dictionary<string, DataRow>();
                if (Paras.Count != 2)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (TRSN_Session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else
                {
                    if (TRSN_Session.Value != null)
                    {
                        //重新加載                  
                        APInfoDataloader.TRSNDataloader(Station, TRSN_Session.ResetInput, new List<R_Station_Action_Para>() { Paras[0] });
                        TRInfo = (Dictionary<string, DataRow>)TRSN_Session.Value;
                        if (TRInfo.Keys.Contains("R_TR_SN"))
                        {
                            if (TRInfo["R_TR_SN"] == null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                            }
                            else
                            {
                                trsnQty = Convert.ToInt32(TRInfo["R_TR_SN"]["QTY"].ToString());
                            }
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY + " T_R_SN" }));
                        }

                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }

                MESStationSession Input_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (Input_LinkQty == null)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000132", new string[] { }));
                }

                //AllPart 條碼的數量不能小於輸入的連半數
                if (Convert.ToInt32(Input_LinkQty.Value.ToString()) > trsnQty)
                {
                    Station.Inputs[2].Value = "";
                    Station.Inputs[3].Enable = false;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000239", new string[] { Input_LinkQty.Value.ToString(), trsnQty.ToString() }));
                }

                //MESStationSession AP_LinkQty = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                //if (AP_LinkQty == null)
                //{
                //    Station.Inputs[2].Value = "";
                //    Station.Inputs[3].Enable = false;
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000133", new string[] { }));
                //}

                //if (Convert.ToInt32(AP_LinkQty.Value.ToString()) <= 0)
                //{
                //    Station.Inputs[2].Value = "";
                //    Station.Inputs[3].Enable = false;
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000137", new string[] { AP_LinkQty.Value.ToString() }));
                //}
                ////AllPart 條碼的數量必須是連半數的整倍數
                //if (trsnQty % Convert.ToInt32(AP_LinkQty.Value.ToString()) != 0)
                //{
                //    Station.Inputs[2].Value = "";
                //    Station.Inputs[3].Enable = false;
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000241", new string[] { AP_LinkQty.Value.ToString() }));
                //}
                
                Station.Inputs[3].Enable = true;
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
    }
}