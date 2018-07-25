using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESStation.HateEmsGetDataService;
using System.Net;
using System.Text.RegularExpressions;
using MESDBHelper;
using System.Data; 
using MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckKeypart
    {
        /// <summary>
        /// 檢查HWD Link Keypart檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        //public static void SNCallHWWSchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        public static void SNSubKPchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> KPList_Temp = new List<Dictionary<string, string>>();
            Dictionary<string, string> DicKP = new Dictionary<string, string>();
            List<C_KEYPART> SubKP = new List<C_KEYPART>();
            string KpSN = Input.Value.ToString();
            //C_KEYPART SUBKP = null;
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession SubSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SubSNSession == null)
            {
                SubSNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SubSNSession);
            }

            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SubKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (KPListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            SN Sn = new SN(KpSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (Sn == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { KpSN }));
            }

            KPListSession.Value = null;
            SubSNSession.Value = Sn;

            T_R_SN_KEYPART_DETAIL _R_SN_KEYPART_DETAIL = new T_R_SN_KEYPART_DETAIL(Station.SFCDB,DB_TYPE_ENUM.Oracle);
            T_C_KEYPART _C_KEYPART = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KEYPART_DETAIL> KEYPARTDETAIL = new List<R_SN_KEYPART_DETAIL>();
           
            if (Sn.ShippedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000070", new string[] { KpSN }));
            }

            if (Sn.RepairFailedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { KpSN }));
            }

            if (Sn.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { KpSN }));
            }

            if (Sn.CurrentStation == "MRB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { KpSN }));
            }
            else
            {
                //Modify by LLF 2018-04-01，有多階綁定
                //KEYPARTDETAIL = _R_SN_KEYPART_DETAIL.GetKeypartBySN(Station.SFCDB, KpSN, Station.StationName);
                //R_SN_KEYPART_DETAIL kpl = KEYPARTDETAIL.Find(z=>z.VALID=="1");
                //if (kpl != null)
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000165", new string[] { KpSN }));
                //}
                //else
                //{
                    SubKP = (List<C_KEYPART>)SubKPSession.Value;
                    C_KEYPART ckp= SubKP.Find(c=>c.PART_NO== Sn.SkuNo);
                    if (ckp == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000175", new string[] { Sn.SkuNo }));
                    }
                //}
            }
        }

        /// <summary>
        /// 檢查HWD Link Keypart檢查
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMainKPchecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            List<Dictionary<string, string>> KPList_Temp = new List<Dictionary<string, string>>();
            Dictionary<string, string> DicKP = new Dictionary<string, string>();
            List<C_KEYPART> MainKP = new List<C_KEYPART>();
            string MainSN = Input.Value.ToString();
      
            if (Paras.Count != 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession MainSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (MainSNSession == null)
            {
                MainSNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainSNSession);
            }

            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (MainKPSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (KPListSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            SN Sn = new SN(MainSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (Sn == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { MainSN }));
            }

            MainSNSession.Value = Sn;

            T_R_SN_KEYPART_DETAIL _R_SN_KEYPART_DETAIL = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_KEYPART _C_KEYPART = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KEYPART_DETAIL> KEYPARTDETAIL = new List<R_SN_KEYPART_DETAIL>();
            if (Sn.ShippedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000070", new string[] { MainSN }));
            }

            if (Sn.RepairFailedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000071", new string[] { MainSN }));
            }

            if (Sn.CompletedFlag == "0")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { MainSN }));
            }

            if (Sn.CurrentStation == "MRB")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000174", new string[] { MainSN }));
            }
            else
            {
                KEYPARTDETAIL = _R_SN_KEYPART_DETAIL.GetKeypartBySN(Station.SFCDB, MainSN, Station.StationName);
                R_SN_KEYPART_DETAIL KP_Main = KEYPARTDETAIL.Find(z => z.VALID == "1");
                if (KP_Main != null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000165", new string[] { MainSN }));
                }
                else
                {
                    MainKP = (List<C_KEYPART>)MainKPSession.Value;
                    C_KEYPART ckp = MainKP.Find(c => c.PART_NO == Sn.SkuNo);
                    if (ckp == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000175", new string[] { Sn.SkuNo }));
                    }
                    else
                    {
                        KPList_Temp = (List<Dictionary<string, string>>)KPListSession.Value;
                        if (KPList_Temp.Count > 0)
                        {
                            DicKP = KPList_Temp.Find(a => a.ContainsValue(Sn.SerialNo));
                            if (DicKP != null)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000193", new string[] {MainSN }));
                            }
                        }

                        Station.AddMessage("MES00000067", new string[] { MainSN }, StationMessageState.Pass);
                    }

                }
            }
        }


    }
}
