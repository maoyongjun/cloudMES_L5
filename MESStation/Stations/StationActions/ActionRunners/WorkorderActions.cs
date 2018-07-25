using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;
using MESStation.Stations.StationActions.DataLoaders;
using MESDataObject;
using MESStation.MESReturnView.Station;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class WorkorderActions
    {
        public static void WOInputResetAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationInput input = null;
            WorkOrder wo = null;
            double? TargetQty = 0.0d;
            double InputQty = 0.0d;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            //準備數據 begin
            MESStationSession WoCount = new MESStationSession();
            WoCount.SessionKey = "1";
            WoCount.MESDataType = "COUNT";
            WoCount.Value = "100";
            Station.StationSession.Add(WoCount);


            WorkOrder workorder = new WorkOrder();
            workorder.Init(Input.Value.ToString(), Station.SFCDB);
            MESStationSession Wo = new MESStationSession();
            Wo.SessionKey = "1";
            Wo.MESDataType = "WO";
            Wo.Value = workorder;
            Station.StationSession.Add(Wo);
            //準備數據 end



            MESStationSession WoInputCount = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoInputCount == null)
            {
                //WoInputCount = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                //Station.StationSession.Add(WoInputCount);
                //throw new Exception("無法獲取工單投入總量，請確認");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession WoInput = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoInput == null)
            {
                //WoInput = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                //Station.StationSession.Add(WoInput);
                //throw new Exception("無法獲取工單信息，請確認");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }


            wo = (WorkOrder)WoInput.Value;
            TargetQty = wo.WORKORDER_QTY;
            InputQty = double.Parse(WoInputCount.Value.ToString());


            if (TargetQty <= InputQty)
            {
                input = Station.FindInputByName("WO");
                if (input != null)
                {
                    input.Enable = true;

                    Station.NextInput = input;
                    WoInputCount.InputValue = "0";
                }

                Station.AddMessage(
                    "MES00000030",
                    new string[] { wo.WorkorderNo.ToString(), TargetQty.ToString(), WoInputCount.Value.ToString() },
                    MESReturnView.Station.StationMessageState.Message);
            }
            else
            {
                input = Station.FindInputByName("NEW_SN");
                if (input != null)
                {
                    Station.NextInput = input;
                    WoInputCount.InputValue = InputQty++.ToString();
                }

            }
        }

        public static void CutWOAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            int WoQty = 0;
            int WoInputQty = 0;
            int CutQty = 0;
            string StrWO = "";
            WorkOrder WO = new WorkOrder();
            T_R_WO_BASE R_Wo_Base = new T_R_WO_BASE(Station.SFCDB,Station.DBType);

            if (Paras.Count != 5)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder WoObj = (WorkOrder)WoSession.Value;
            StrWO = WoObj.WorkorderNo;

            MESStationSession WoQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoQtySession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WoQty =Convert.ToInt16(WoQtySession.Value);


            MESStationSession CutQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (CutQtySession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            CutQty = Convert.ToInt16(CutQtySession.Value);

            MESStationSession WoInputQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (WoInputQtySession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WoInputQty = Convert.ToInt16(WoInputQtySession.Value);

            MESStationSession WoFinishQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (WoFinishQtySession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WoInputQty = Convert.ToInt16(WoInputQtySession.Value);

            if (WoQty - WoInputQty< CutQty)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000208", new string[] { StrWO, CutQty.ToString(), (WoQty - WoInputQty).ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

           int Result=R_Wo_Base.UpdateWoQty(StrWO, CutQty,Station.LoginUser.EMP_NO, Station.SFCDB);

            if (Result > 0)
            {
                WO.Init(StrWO, Station.SFCDB, Station.DBType);

                WoInputQtySession.Value = WO.INPUT_QTY;
                WoFinishQtySession.Value = WO.FINISHED_QTY;
                WoQtySession.Value = WO.WORKORDER_QTY;

                Station.AddMessage("MES00000210", new string[] { StrWO, CutQty.ToString() }, StationMessageState.Pass); //回饋消息到前台
            }
            else
            {
                Station.AddMessage("MES00000209", new string[] { StrWO, CutQty.ToString() }, StationMessageState.Fail);//回饋消息到前台
            }
        }

        public static void WoSessionUpdateAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            string StrWO = "";
            if (Paras.Count != 9)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession WoQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoQtySession == null)
            {
                WoQtySession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoQtySession);
            }

            MESStationSession WoSkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (WoSkuSession == null)
            {
                WoSkuSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoSkuSession);
            }

            MESStationSession WoSkuVerSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (WoSkuVerSession == null)
            {
                WoSkuVerSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoSkuVerSession);
            }

            MESStationSession WoInputQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (WoInputQtySession == null)
            {
                WoInputQtySession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoInputQtySession);
            }

            MESStationSession WoFinishQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (WoFinishQtySession == null)
            {
                WoFinishQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoFinishQtySession);
            }

            MESStationSession WoStatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
            if (WoStatusSession == null)
            {
                WoStatusSession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoStatusSession);
            }

            MESStationSession WoRouteIDSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
            if (WoRouteIDSession == null)
            {
                WoRouteIDSession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoRouteIDSession);
            }

            MESStationSession WoRouteNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
            if (WoRouteNameSession == null)
            {
                WoRouteNameSession = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WoRouteNameSession);
            }

            WorkOrder WoObj = (WorkOrder)WoSession.Value;
            StrWO = WoObj.WorkorderNo;

            WorkOrder WO = new WorkOrder();
            WO.Init(StrWO, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            WoSession.Value = WO;

            WoQtySession.Value = WO.WORKORDER_QTY;
            WoSkuSession.Value = WO.SkuNO;
            WoSkuVerSession.Value = WO.SKU_VER;
            WoInputQtySession.Value = WO.INPUT_QTY;
            WoFinishQtySession.Value = WO.FINISHED_QTY;
            WoStatusSession.Value = WO.CLOSED_FLAG;
            WoRouteIDSession.Value = WO.RouteID;
            if (WO.ROUTE != null)
            {
                WoRouteNameSession.Value = WO.ROUTE.ROUTE_NAME;
            }
            else
            {
                WoRouteNameSession.Value = "";
            }
        }

        public static void UpdateWOKeypartID(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            try
            {
                int result;
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(Station.SFCDB, Station.DBType);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
                Row_R_WO_BASE rowWOBase;
                List<string> kpList = t_c_kp_list.GetListIDBySkuno(objWorkorder.SkuNO, Station.SFCDB);
                if (kpList.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { objWorkorder.SkuNO }));
                }
                rowWOBase = (Row_R_WO_BASE)t_r_wo_base.GetObjByID(objWorkorder.ID, Station.SFCDB);
                rowWOBase.KP_LIST_ID = kpList[0].ToString();
                result = Station.SFCDB.ExecSqlNoReturn(rowWOBase.GetUpdateString(Station.DBType), null);
                if (result <= 0)
                {
                    //報錯信息待改
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { objWorkorder.WorkorderNo }));
                }
                Station.AddMessage("MES00000063", new string[] { objWorkorder.WorkorderNo }, StationMessageState.Pass);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
