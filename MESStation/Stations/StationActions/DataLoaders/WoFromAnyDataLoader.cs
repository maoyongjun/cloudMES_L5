using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using MESDataObject;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class WoFromAnyDataLoader
    {
        /// <summary>
        /// 從SNLoadPoint保存的SN對象加載工單對象到指定位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">2個參數,WO,SN保存的位置</param>
        public static void WoFromSNDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession Ssn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Swo == null)
            {
                //throw new Exception("Can Not Fint " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }

            if (Ssn == null)
            {
                //throw new Exception("请输入SN!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            else
            {
                SN ObjSN = (SN)Ssn.Value;
                WorkOrder ObjWorkorder = new WorkOrder();
                String SNLoadPoint = Input.Value.ToString();
                string WOSavePoint = null;

                try
                {
                    WOSavePoint = ObjSN.WorkorderNo.Trim();
                    ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    Swo.Value = ObjWorkorder;

                    Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESReturnView.Station.StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    string msgCode = ex.Message;
                    throw ex;

                }
            }
        }

        /// <summary>
        /// 從PanelLoadPoint保存的Panel對象加載工單對象到指定位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">2個參數,WO,Panle保存的位置</param>
        public static void WoFromPanelDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession Spanel = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }

            if (Spanel == null)
            {
                //throw new Exception("请输入PANEL!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "PANEL" }));
            }
            else
            {
                Panel ObjPanel = (Panel)Spanel.Value;
                WorkOrder ObjWorkorder = new WorkOrder();
                string PanelLoadPoint = Input.Value.ToString();
                string WOSavePoint = null;

                try
                {
                    if (ObjPanel.PanelCollection.Count != 0)
                    {
                        WOSavePoint = ObjPanel.PanelCollection[0].WORKORDERNO.ToString();
                    }
                    else
                    {
                        //throw new Exception("Can Not Find " + PanelLoadPoint + " 'Information ' !");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { PanelLoadPoint }));
                    }
                    ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    if (ObjWorkorder == null)
                    {
                        //throw new Exception("Can Not Find " + WOSavePoint + " 'Information ' !");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { WOSavePoint }));
                    }
                    Swo.Value = ObjWorkorder;

                    Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESReturnView.Station.StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    string msgCode = ex.Message;
                    throw ex;

                }
            }
        }

        /// <summary>
        /// 從輸入加載工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void WoDataloader(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }

            WorkOrder ObjWorkorder = new WorkOrder();
            string WOSavePoint = Input.Value.ToString();

            try
            {
                ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (ObjWorkorder == null)
                {
                    //throw new Exception("Can Not Find " + WOSavePoint + " 'Information ' !");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { WOSavePoint }));
                }
                Swo.Value = ObjWorkorder;
                Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;
            }
        }
    }
}
