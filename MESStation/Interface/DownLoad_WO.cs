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

namespace MESStation.Interface
{
    class DownLoad_WO: MesAPIBase
    {
        T_C_INTERFACE C_Interface;
        Row_C_INTERFACE Row_C_Interface;
        OleExec Sfcdb;
        string IP = "";

        protected APIInfo FDownload = new APIInfo()
        {
            FunctionName = "GetInterfaceStatus",
            Description = "Get Interface Status",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PROGRAM", InputType = "STRING", DefaultValue = "INTERFACE"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public DownLoad_WO()
        {
            this.Apis.Add(FDownload.FunctionName, FDownload);
        }

        public Dictionary<string, string> GetInterfacePara()
        {
            Dictionary<string, string> Dic_Interface = new Dictionary<string, string>();
            Dic_Interface.Add("Start", "0");
            Dic_Interface.Add("CurrentDate", "2017-12-21");
            Dic_Interface.Add("NextDate", "2017-12-22");
            Dic_Interface.Add("ConsoleIP", "127.0.0.1");

            return Dic_Interface;
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
            string ItemName = Data["ITEM_NAME"].ToString();
            string StrDate =Data["DATE_TIME"].ToString();
            string IP = Data["IP"].ToString();
            Interface InterFace=new Interface();
            string Local_IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
            if (IP != Local_IP)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;               
                StationReturn.Message = MESReturnMessage.GetMESReturnMessage("MES00000103");

            }
            else if (InterFace.CheckInterfaceRun(ProgramName, ItemName, Sfcdb))// Ensure only one interface console program run;
            {
                try
                {
                    InterFace.LockItem(ProgramName, ItemName, Sfcdb);
                    Download(ItemName, StrDate);
                    AutoConvert();//Regular WO AutoConvert
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

        public void Download(string ItemName, string Date)
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
            T_C_PARAMETER C_PARAMETER;
            Row_C_PARAMETER Row_PARAMETER;
            Dictionary<string, string> DicPara = new Dictionary<string, string>();
            string StrColumn = "";
            string StrValue = "";
            string[] StrColumn_Name;
            string[] StrColumn_Value;
            string StrWo = "";
            bool Exist_WO_Flag = false;
            bool Exist_WO_Base_Flag = false;
            bool Exist_SKU_Flag = false;
            bool DownLoad_Auto = false;

            if (string.IsNullOrEmpty(StrWo))
            {
                DownLoad_Auto = true;
            }

            sfcdb = this.DBPools["SFCDB"].Borrow();
            C_PARAMETER = new T_C_PARAMETER(sfcdb, DB_TYPE_ENUM.Oracle);
            DicPara = C_PARAMETER.Get_Interface_Parameter_2(ItemName, sfcdb, DB_TYPE_ENUM.Oracle);
            this.DBPools["SFCDB"].Return(sfcdb);

            ZRFC_SFC_NSG_0001B Zrfc_SFC_NSG_001B = new ZRFC_SFC_NSG_0001B(StrWo);
            Zrfc_SFC_NSG_001B.SetValue("PLANT", DicPara["PLANT"]);//NHGZ,WDN1//WDN1,WSL3
            Zrfc_SFC_NSG_001B.SetValue("SCHEDULED_DATE", Date);
            Zrfc_SFC_NSG_001B.SetValue("RLDATE", Date);
            Zrfc_SFC_NSG_001B.SetValue("COUNT", DicPara["COUNT"]);
            Zrfc_SFC_NSG_001B.SetValue("CUST", DicPara["CUST"]);
            Zrfc_SFC_NSG_001B.SetValue("IN_CNF", DicPara["IN_CNF"]);  //IN_CNF=0,Download WO not Confirmed
            Zrfc_SFC_NSG_001B.CallRFC();

            for (int i = 0; i < Zrfc_SFC_NSG_001B.ReturnDatatableByIndex.Count; i++)
            {
                string ErrorMessage = "";
                switch (Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i].TableName.ToString().ToUpper())
                {
                    case "ITAB":
                        if (Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[0].Rows.Count > 0)
                        {
                            ErrorMessage = Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[0].Rows[0][1].ToString();
                            throw new Exception(ErrorMessage);
                        }
                        break;
                    case "WO_HEADER":
                        sfcdb = this.DBPools["SFCDB"].Borrow();

                        C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
                        Row = C_TAB_COLUMN_MAP.GetTableColumnMap("R_WO_HEADER", sfcdb, DB_TYPE_ENUM.Oracle);

                        StrColumn = Row["TAB_COLUMN"].ToString();
                        StrValue = "";
                        StrColumn_Name = StrColumn.Split(',');
                        StrColumn_Value = new string[StrColumn_Name.Count()];

                        RFC_Table = (DataTable)Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i];

                        for (int m = 0; m < RFC_Table.Rows.Count; m++)
                        {
                            R_WO_BASE = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Base_Flag = R_WO_BASE.CheckDataExist(RFC_Table.Rows[m]["AUFNR"].ToString(), sfcdb);

                            R_WO_HEADER = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Flag = R_WO_HEADER.CheckWoHeadByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), DownLoad_Auto, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            C_SKU = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_SKU_Flag = C_SKU.CheckSku(RFC_Table.Rows[m][3].ToString(), sfcdb);

                            if (!Exist_WO_Flag && Exist_SKU_Flag && !Exist_WO_Base_Flag)
                            {
                                string StrID = C_TAB_COLUMN_MAP.GetNewID(BU, sfcdb);
                                for (int j = 0; j < StrColumn_Name.Count(); j++)
                                {
                                    //StrColumn_Value[j] = ReplaceSpecialChar(RFC_Table.Rows[m][StrColumn_Name[j]].ToString());
                                    StrColumn_Value[j] = RFC_Table.Rows[m][StrColumn_Name[j]].ToString();
                                    if (j == 0)
                                    {
                                        StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                    else
                                    {
                                        StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                }
                                StrSql = " insert into R_WO_HEADER（" + StrColumn + ",ID " + ") values(" + StrValue + ",'" + StrID + "'" + ");";

                                R_WO_HEADER = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle);
                                R_WO_HEADER.EditWoHead(StrSql, sfcdb, DB_TYPE_ENUM.Oracle);
                            }
                        }
                        this.DBPools["SFCDB"].Return(sfcdb);

                        break;
                    case "WO_ITEM":
                        StrSql = "";
                        sfcdb = this.DBPools["SFCDB"].Borrow();
                        C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
                        Row = C_TAB_COLUMN_MAP.GetTableColumnMap("R_WO_ITEM", sfcdb, DB_TYPE_ENUM.Oracle);

                        StrColumn = Row["TAB_COLUMN"].ToString();
                        StrValue = "";
                        StrColumn_Name = StrColumn.Split(',');
                        StrColumn_Value = new string[StrColumn_Name.Count()];

                        RFC_Table = (DataTable)Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i];

                        for (int m = 0; m < RFC_Table.Rows.Count; m++)
                        {
                            R_WO_BASE = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Base_Flag = R_WO_BASE.CheckDataExist(RFC_Table.Rows[m]["AUFNR"].ToString(), sfcdb);

                            R_WO_ITEM = new T_R_WO_ITEM(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Flag = R_WO_ITEM.CheckWoItemByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), RFC_Table.Rows[m]["MATNR"].ToString(), DownLoad_Auto, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            C_SKU = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_SKU_Flag = C_SKU.CheckSku(RFC_Table.Rows[m][8].ToString(), sfcdb);

                            if (!Exist_WO_Flag && Exist_SKU_Flag && !Exist_WO_Base_Flag)
                            {
                                string StrID = C_TAB_COLUMN_MAP.GetNewID(BU, sfcdb);
                                for (int j = 0; j < StrColumn_Name.Count(); j++)
                                {
                                    //StrColumn_Value[j] = ReplaceSpecialChar(RFC_Table.Rows[m][StrColumn_Name[j]].ToString());
                                    StrColumn_Value[j] = RFC_Table.Rows[m][StrColumn_Name[j]].ToString();
                                    if (j == 0)
                                    {
                                        StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                    else
                                    {
                                        StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                }

                                StrSql = "insert into R_WO_ITEM（" + StrColumn + ",ID " + ") values(" + StrValue + ",'" + StrID + "'" + ");\n";

                                R_WO_ITEM = new T_R_WO_ITEM(sfcdb, DB_TYPE_ENUM.Oracle);
                                R_WO_ITEM.EditWoItem(StrSql, sfcdb, DB_TYPE_ENUM.Oracle);
                            }
                        }
                        this.DBPools["SFCDB"].Return(sfcdb);
                        break;
                    case "WO_TEXT":
                        StrSql = "";
                        sfcdb = this.DBPools["SFCDB"].Borrow();
                        C_TAB_COLUMN_MAP = new T_C_TAB_COLUMN_MAP(sfcdb, DB_TYPE_ENUM.Oracle);
                        Row = C_TAB_COLUMN_MAP.GetTableColumnMap("R_WO_TEXT", sfcdb, DB_TYPE_ENUM.Oracle);

                        StrColumn = Row["TAB_COLUMN"].ToString();
                        StrValue = "";
                        StrColumn_Name = StrColumn.Split(',');
                        StrColumn_Value = new string[StrColumn_Name.Count()];

                        RFC_Table = (DataTable)Zrfc_SFC_NSG_001B.ReturnDatatableByIndex[i];

                        for (int m = 0; m < RFC_Table.Rows.Count; m++)
                        {
                            R_WO_BASE = new T_R_WO_BASE(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Base_Flag = R_WO_BASE.CheckDataExist(RFC_Table.Rows[m]["AUFNR"].ToString(), sfcdb);

                            R_WO_TEXT = new T_R_WO_TEXT(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_WO_Flag = R_WO_TEXT.CheckWoTextByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), DownLoad_Auto, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            R_WO_HEADER = new T_R_WO_HEADER(sfcdb, DB_TYPE_ENUM.Oracle);
                            Exist_SKU_Flag = R_WO_HEADER.CheckWoHeadByWo(RFC_Table.Rows[m]["AUFNR"].ToString(), true, StrColumn, sfcdb, DB_TYPE_ENUM.Oracle);

                            if (!Exist_WO_Flag && !Exist_WO_Base_Flag && Exist_SKU_Flag)
                            {
                                string StrID = C_TAB_COLUMN_MAP.GetNewID(BU, sfcdb);
                                for (int j = 0; j < StrColumn_Name.Count(); j++)
                                {
                                    StrColumn_Value[j] = RFC_Table.Rows[m][StrColumn_Name[j]].ToString();
                                    if (j == 0)
                                    {
                                        StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                    else
                                    {
                                        StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                                    }
                                }
                                StrSql = "insert into R_WO_TEXT（" + StrColumn + ",ID " + ") values(" + StrValue + ",'" + StrID + "'" + ");";

                                R_WO_TEXT = new T_R_WO_TEXT(sfcdb, DB_TYPE_ENUM.Oracle);
                                R_WO_TEXT.EditWoText(StrSql, sfcdb, DB_TYPE_ENUM.Oracle);
                            }
                        }
                        this.DBPools["SFCDB"].Return(sfcdb);
                        break;
                }
            }
        }

        public void AutoConvert()
        {
            T_R_WO_HEADER R_WO_HEADER;
            T_R_WO_BASE R_WO_BASE;
            T_C_SKU C_SKU;
            T_C_ROUTE C_ROUTE;
            OleExec Sfcdb;
            string Rows_ID = "";

            Sfcdb = this.DBPools["SFCDB"].Borrow();
            R_WO_HEADER = new T_R_WO_HEADER(Sfcdb, DB_TYPE_ENUM.Oracle);
            DataTable dt = R_WO_HEADER.GetConvertWoList(Sfcdb, DB_TYPE_ENUM.Oracle);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    R_WO_BASE = new T_R_WO_BASE(Sfcdb, DB_TYPE_ENUM.Oracle);
                    Rows_ID = R_WO_BASE.GetNewID(BU, Sfcdb);
                    Row_R_WO_BASE Rows = (Row_R_WO_BASE)R_WO_BASE.NewRow();

                    C_SKU = new T_C_SKU(Sfcdb, DB_TYPE_ENUM.Oracle);
                    Row_C_SKU Rows_SKU = (Row_C_SKU)C_SKU.GetSku(dr["MATNR"].ToString(), Sfcdb, DB_TYPE_ENUM.Oracle);

                    C_ROUTE = new T_C_ROUTE(Sfcdb, DB_TYPE_ENUM.Oracle);
                    Row_C_ROUTE Rows_Route = (Row_C_ROUTE)C_ROUTE.GetRouteBySkuno(dr["MATNR"].ToString(), Sfcdb, DB_TYPE_ENUM.Oracle);

                    if (Rows != null && Rows_SKU != null && Rows_Route != null)
                    {
                        Rows.ID = Rows_ID;
                        Rows.WORKORDERNO = dr["AUFNR"].ToString();
                        Rows.SKUNO = dr["MATNR"].ToString();
                        Rows.CUSTOMER_NAME = Rows_SKU.CUST_SKU_CODE;
                        Rows.CUST_PN = Rows_SKU.CUST_PARTNO;
                        Rows.WORKORDER_QTY = Convert.ToDouble(dr["GAMNG"]);
                        Rows.SKU_VER = dr["REVLV"].ToString();
                        Rows.SKU_NAME = Rows_SKU.SKU_NAME;
                        Rows.SKU_DESC = Rows_SKU.DESCRIPTION;
                        Rows.ROHS = dr["ROHS_VALUE"].ToString();
                        Rows.ROUTE_ID = Rows_Route.ID; //路由應該加版本//Rows.KP_LIST_ID
                        Rows.CLOSED_FLAG = "0";
                        Rows.EDIT_EMP = "LLF";
                        string str = Sfcdb.ExecSQL(Rows.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                }
            }
        }
    }
}
