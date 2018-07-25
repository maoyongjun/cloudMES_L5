using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAP.Middleware.Connector;
using System.Configuration;

namespace MESStation.Interface
{
    public class Download_WO
    {  
        public RfcConfigParameters GetConfigParams()
        {

            RfcConfigParameters configParams = new RfcConfigParameters(); // Name property is neccessary, otherwise, NonInvalidParameterException will be thrown configParams.Add(RfcConfigParameters.Name, "ECC"); 
            configParams.Add(RfcConfigParameters.SystemNumber, "10"); // instance number configParams.Add(RfcConfigParameters.SystemID, "D01"); 
            configParams.Add(RfcConfigParameters.User, "NSGBG");
            configParams.Add(RfcConfigParameters.Password, "MESEDICU");
            configParams.Add(RfcConfigParameters.Client, "800");
            configParams.Add(RfcConfigParameters.Language, "ZF");
            configParams.Add(RfcConfigParameters.MessageServerHost, "10.134.108.111");
            //configParams.Add(RfcConfigParameters.GatewayHost,"10.134.108.122");
            configParams.Add(RfcConfigParameters.LogonGroup, "CNSBG_800");
            configParams.Add(RfcConfigParameters.SystemID, "CNP");
            configParams.Add(RfcConfigParameters.Name, "CON1");
            //configParams.Add(RfcConfigParameters.PoolSize, "5");
            return configParams;
        }

        public RfcDestination GetDestination()
        {
            RfcConfigParameters configParams = this.GetConfigParams();
            RfcDestination dest = null;
            try
            {
                dest = RfcDestinationManager.GetDestination(configParams);
            }
            catch (Exception EX)
            {
                string strmessage = EX.Message;
            }

            return dest;
        }

        public void DownloadWO()
        {
            string StrWO = "";
            IRfcFunction DownloadWo_Func;
            IRfcTable RfcTable_ITAB;
            IRfcTable RfcTable_WO_HEAD;
            IRfcTable RfcTable_WO_ITEM;
            IRfcTable RfcTable_WO_TEXT;

            try
            {
                RfcDestination Dest = RfcDestinationManager.GetDestination(this.GetConfigParams());
                RfcRepository rfc = Dest.Repository;

                DownloadWo_Func = rfc.CreateFunction("ZRFC_SFC_NSG_0001B");
                //myfun.SetValue("PLANT", "NHGZ,AMEZ");
                DownloadWo_Func.SetValue("PLANT", "ALL");
                DownloadWo_Func.SetValue("SCHEDULED_DATE", "20171129");
                DownloadWo_Func.SetValue("RLDATE", "20171129");
                DownloadWo_Func.SetValue("COUNT", 14);
                DownloadWo_Func.SetValue("CUST", "ALL");
                //RfcTable.SetValue("AUFNR", "002320001171");
                if (StrWO != "")
                {
                    RfcTable_ITAB = DownloadWo_Func.GetTable("ITAB");
                    RfcTable_ITAB.Append();
                    RfcTable_ITAB.SetValue("AUFNR", StrWO);
                    DownloadWo_Func.Invoke(Dest);
                    string StrMessage = RfcTable_ITAB.GetString("ERRMSG");
                    if (StrMessage != "")
                    {
                        throw new Exception(StrMessage);
                    }
                }
                else
                {
                    DownloadWo_Func.Invoke(Dest);
                }


                //myfun.Invoke(destination1);
                //RfcTable
                RfcTable_WO_HEAD = DownloadWo_Func.GetTable("WO_HEADER");
                RfcTable_WO_ITEM = DownloadWo_Func.GetTable("WO_ITEM");
                RfcTable_WO_TEXT = DownloadWo_Func.GetTable("WO_TEXT");

                // int n = Rfctable_Wo_head.Count();
                //for (int i = 0; i < n; i++)
                //{
                //Rfctable_Wo_head.CurrentIndex = i;
                //string str= rfctable.GetString(i).ToString();
                string StrColumn = ConfigurationManager.AppSettings["R_WO_HEAD"].ToString();
                string StrValue = "";
                string[] StrColumn_Name = StrColumn.Split(',');
                string[] StrColumn_Value = new string[StrColumn_Name.Count()];

                for (int m = 0; m < RfcTable_WO_HEAD.Count; m++)
                {
                    RfcTable_WO_HEAD.CurrentIndex = m;
                    for (int j = 0; j < StrColumn_Name.Count(); j++)
                    {
                        StrColumn_Value[j] = RfcTable_WO_HEAD.GetString(StrColumn_Name[j]).ToString();
                        if (j == 0)
                        {
                            StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                        }
                        else
                        {
                            StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                        }
                    }

                    string strSql = "insert into R_WO_HEAD（" + StrColumn + ") values(" + StrValue + ")";
                }
                //}

                //n = Rfctable_Wo_item.Count();
                //for (int i = 0; i < n; i++)
                //{
                //Rfctable_Wo_item.CurrentIndex = i;
                //string str= rfctable.GetString(i).ToString();
                StrColumn = ConfigurationManager.AppSettings["R_WO_ITEM"].ToString();
                StrValue = "";
                StrColumn_Name = StrColumn.Split(',');
                StrColumn_Value = new string[StrColumn_Name.Count()];

                for (int m = 0; m < RfcTable_WO_ITEM.Count; m++)
                {
                    RfcTable_WO_ITEM.CurrentIndex = m;
                    for (int j = 0; j < StrColumn_Name.Count(); j++)
                    {
                        StrColumn_Value[j] = RfcTable_WO_ITEM.GetString(StrColumn_Name[j]).ToString();
                        if (j == 0)
                        {
                            StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                        }
                        else
                        {
                            StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                        }
                    }

                    string strSql = "insert into R_WO_ITEM（" + StrColumn + ") values(" + StrValue + ")";
                }
                //}

                //n = Rfctable_Wo_text.Count();
                //for (int i = 0; i < n; i++)
                //{
                //Rfctable_Wo_text.CurrentIndex = i;
                //string str= rfctable.GetString(i).ToString();
                StrColumn = ConfigurationManager.AppSettings["R_WO_TEXT"].ToString();
                StrValue = "";
                StrColumn_Name = StrColumn.Split(',');
                StrColumn_Value = new string[StrColumn_Name.Count()];

                for (int m = 0; m < RfcTable_WO_TEXT.Count; m++)
                {
                    RfcTable_WO_TEXT.CurrentIndex = m;
                    for (int j = 0; j < StrColumn_Name.Count(); j++)
                    {
                        StrColumn_Value[j] = RfcTable_WO_TEXT.GetString(StrColumn_Name[j]).ToString();
                        if (j == 0)
                        {
                            StrValue = "'" + StrColumn_Value[j].ToString() + "'";
                        }
                        else
                        {
                            StrValue = StrValue + ",'" + StrColumn_Value[j].ToString() + "'";
                        }
                    }

                    string strSql = "insert into R_WO_TEXT（" + StrColumn + ") values(" + StrValue + ")";
                }

            }
            catch (Exception ex)
            {
                string string1 = ex.Message;
            }
        }
    }
}



