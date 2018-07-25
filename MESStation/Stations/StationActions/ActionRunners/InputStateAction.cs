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

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class InputStateAction
    {
        public static void InputsEnable(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[i].VALUE.ToString().Trim());
                if (input != null)
                {
                    input.Enable = true;
                }
            }
        }

        public static void InputsDisable(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[i].VALUE.ToString().Trim());
                if (input != null)
                {
                    input.Enable = false;
                }
            }
        }

        public static void InputsDisableControl(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[i].SESSION_TYPE);
                if (input != null)
                {
                    input.Visable = Paras[i].VALUE.ToString().Trim().ToUpper()=="TRUE"?true:false;
                }
            }
        }

        public static void SetNextInput(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationInput input = Station.Inputs.Find(t => t.DisplayName == Paras[0].VALUE.ToString().Trim());
            if (input != null)
            {
                Station.NextInput = input;
            }
        }

        public static void ClearInputAndMemory(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ClearFlag = string.Empty;
            string ClearItem = string.Empty;
            string ErrMessage = string.Empty;

            if (Paras.Count < 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] {});
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取清除標誌位，表示是否要進行清除輸入操作
            MESStationSession ClearFlagSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (ClearFlagSession != null)
            {
                //黃楊盛 2018年4月24日14:23:34 NPE就放FALSE
                //ClearFlag = ClearFlagSession.Value.ToString();
                ClearFlag = ClearFlagSession.Value?.ToString() ?? "false";
                Station.StationSession.Remove(ClearFlagSession);
                if (ClearFlag.ToLower().Equals("true"))
                {
                    List<R_Station_Action_Para> MemoryParas=Paras.FindAll(t => t.SESSION_TYPE.Equals("CLEARMEMORY"));
                    List<R_Station_Action_Para> InputParas = Paras.FindAll(t => t.SESSION_TYPE.Equals("CLEARINPUT"));

                    //清除指定session
                    foreach (R_Station_Action_Para para in MemoryParas)
                    {
                        Station.StationSession.Remove(Station.StationSession.Find(t => t.MESDataType.ToUpper().Equals(para.VALUE.ToUpper())));
                    }

                    foreach (R_Station_Action_Para para in InputParas)
                    {
                        //清除所有輸入框的值
                        if (para.VALUE.ToUpper().Equals("ALL"))
                        {
                            foreach (MESStationInput StationInput in Station.Inputs)
                            {
                                StationInput.Value = "";
                            }
                            return;
                        }

                        //清除指定輸入框的值
                        ClearItem = para.VALUE.ToString().ToUpper();
                        MESStationInput input = Station.Inputs.Find(t => t.DisplayName.ToUpper().Equals(ClearItem)
                                                                        || t.Name.ToUpper().Equals(ClearItem));
                        if (input != null)
                        {
                            input.Value = "";
                        }
                    }

                    //ClearFlagSession.Value = "false";
                }
            }

            
        }

        public static void SetPassOrFailInOba(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == Paras[0].SESSION_TYPE.ToString());
            MESStationInput failSnInput = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE.ToString());
            MESStationInput FailCodeInput = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE.ToString());
            MESStationInput LocationInput = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE.ToString());
            MESStationInput FailDescInput = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE.ToString());


            if (Input.Value.ToString().ToUpper().Equals("PASS"))
            {
                snInput.Visable = true; ;
                failSnInput.Visable = false;
                FailCodeInput.Visable = false;
                LocationInput.Visable = false;
                FailDescInput.Visable = false;
                Station.NextInput = snInput;
            }
            else
            {
                snInput.Visable = false; ;
                failSnInput.Visable = true;
                Station.NextInput = failSnInput;
            }
        }
    }
}
