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
using System.Collections.Generic;
using MESDataObject.Module;
using System.Reflection;
using MESStation.Label;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class LabelPrintAction
    {
        /// <summary>
        /// 提供工站打印本站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PrintStationLabelAction(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }


            SKU SKU = (SKU)(Station.StationSession.Find(T => T.MESDataType == Paras[1].SESSION_TYPE && T.SessionKey == Paras[1].SESSION_KEY).Value);

            List<string> ProcessLabType = new List<string>();
            if (Paras.Count > 1)
            {
                for (int i = 2; i < Paras.Count; i++)
                {
                    ProcessLabType.Add(Paras[i].VALUE.ToString());
                }
            }

            T_C_SKU_Label TCSL = new T_C_SKU_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            //獲取label配置
            List<Row_C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(SKU.SkuBase.SKUNO, Station.StationName, SFCDB);

            List<Label.LabelBase> PrintLabs = new List<Label.LabelBase>();
            T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DB_TYPE_ENUM.Oracle);
            for (int i = 0; i < labs.Count; i++)
            {
                if (ProcessLabType.Count > 0)
                {
                    if (!ProcessLabType.Contains(labs[i].LABELTYPE))
                    {
                        continue;
                    }
                }
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(labs[i].LABELNAME, SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(labs[i].LABELTYPE, SFCDB);

                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                System.Type APIType = assembly.GetType(RC.CLASS);
                object API_CLASS = assembly.CreateInstance(RC.CLASS);

                LabelBase Lab = (LabelBase)API_CLASS;
                //給label的輸入變量加載值
                for (int j = 0; j < Lab.Inputs.Count; j++)
                {
                    if (Lab.Inputs[j].Name.ToUpper() == "STATION")
                    {
                        Lab.Inputs[j].Value = Station.StationName;
                    }

                    MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[j].StationSessionType && T.SessionKey == Lab.Inputs[j].StationSessionKey);
                    if (S != null)
                    {
                        Lab.Inputs[i].Value = S.Value;
                    }
                }
                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrintQTY = (int)labs[i].QTY;
                Lab.MakeLabel(SFCDB);
                List<LabelBase> pages = MakePrintPage(Lab, RL);

                for (int k = 0; k < pages.Count; k++)
                {
                    pages[k].ALLPAGE = pages.Count;
                    Station.LabelPrint.Add(pages[k]);
                }
                
            }

        }
        /// <summary>
        /// 根據R_LABEL的配置將文檔分頁
        /// </summary>
        /// <param name="lab"></param>
        /// <param name="RL"></param>
        /// <returns></returns>
        static List<LabelBase> MakePrintPage(LabelBase lab , Row_R_Label RL)
        {
            List<LabelBase> RET = new List<LabelBase>();
            int? L = (int?)RL.ARRYLENGTH;
            if (L == null || L==0)
            {
                RET.Add(lab);
                return RET;
            }
            LabelOutput arry1 = lab.Outputs.Find(T => T.Type == LableOutPutTypeEnum.StringArry);
            if (arry1 == null)
            {
                RET.Add(lab);
                return RET;
            }

            int l = ((List<string>)arry1.Value).Count;
            if (l == 0)
            {
                RET.Add(lab);
                return RET;
            }
            LabelBase p = null;

             List< LabelOutput> O = lab.Outputs.FindAll(T => T.Type == LableOutPutTypeEnum.StringArry && T.Name != "PAGE" && T.Name != "ALLPAGE");

            int page = 0;
            for (int i = 0; i < l; i++)
            {
                
                if (i % L == 0)
                {
                    p = new LabelBase();
                    page++;
                    p.PAGE = page;
                    CopyLabString(lab, p);
                    RET.Add(p);
                }
                for (int j = 0; j < O.Count; j++)
                {
                    LabelOutput OP = p.Outputs.Find(T => T.Name == O[j].Name);
                    ((List<string>)OP.Value).Add(((List<string>) O[j].Value)[i]);
                }
            }
            
            return RET;

        }
        /// <summary>
        /// 拷貝2個輸出的固定部分
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        static void CopyLabString(LabelBase l1, LabelBase l2)
        {
            List<LabelOutput> O = l1.Outputs.FindAll(T => T.Type == LableOutPutTypeEnum.String && T.Name != "PAGE" && T.Name != "ALLPAGE");
            for (int i = 0; i < O.Count; i++)
            {
                l2.Outputs.Add(new LabelOutput() { Name=O[i].Name, Type = O[i].Type, Value = O[i].Value.ToString(), Description = O[i].Description  } );
            }
            O = l1.Outputs.FindAll(T => T.Type == LableOutPutTypeEnum.StringArry && T.Name != "PAGE" && T.Name != "ALLPAGE");
            for (int i = 0; i < O.Count; i++)
            {
                l2.Outputs.Add(new LabelOutput() { Name = O[i].Name, Type = O[i].Type, Value = new List<string>(), Description = O[i].Description });
            }
            l2.LabelName = l1.LabelName;
            l2.FileName = l1.FileName;
            l2.PrintQTY = l1.PrintQTY;

        }

    }
}
