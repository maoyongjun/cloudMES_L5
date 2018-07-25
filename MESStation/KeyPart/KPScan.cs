using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESDataObject;
using Newtonsoft.Json.Linq;

namespace MESStation.KeyPart
{
    public class KPScan : MesAPIBase
    {
        protected APIInfo _UpLoadKPList = new APIInfo()
        {
            FunctionName = "UpLoadKPList",
            Description = "UpLoadKPList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SkuNo", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ListName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ListData", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _RemoveKPList = new APIInfo()
        {
            FunctionName = "RemoveKPList",
            Description = "RemoveKPList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ListNames", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetSNStationKPList = new APIInfo()
        {
            FunctionName = "GetSNStationKPList",
            Description = "GetSNStationKPList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "STATION", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _ScanKPItem = new APIInfo()
        {
            FunctionName = "ScanKPItem",
            Description = "ScanKPItem",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "STATION", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "KPITEM", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetKPListBySkuno = new APIInfo()
        {
            FunctionName = "GetKPListBySkuno",
            Description = "GetKPListBySkuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetAllKPList = new APIInfo()
        {
            FunctionName = "GetAllKPList",
            Description = "GetAllKPList",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _CheckKPListName = new APIInfo()
        {
            FunctionName = "CheckKPListName",
            Description = "CheckKPListName",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "ListName", InputType = "STRING", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetKPListByListName = new APIInfo()
        {
            FunctionName = "GetKPListByListName",
            Description = "GetKPListByListName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ListName", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public KPScan()
        {
            this.Apis.Add(_GetSNStationKPList.FunctionName, _GetSNStationKPList);
            this.Apis.Add(_GetKPListBySkuno.FunctionName, _GetKPListBySkuno);
            this.Apis.Add(_GetAllKPList.FunctionName, _GetAllKPList);
            this.Apis.Add(_UpLoadKPList.FunctionName, _UpLoadKPList);
            this.Apis.Add(_RemoveKPList.FunctionName, _RemoveKPList);
            this.Apis.Add(_CheckKPListName.FunctionName, _CheckKPListName);
            this.Apis.Add(_GetKPListByListName.FunctionName, _GetKPListByListName);
            this.Apis.Add(_ScanKPItem.FunctionName, _ScanKPItem);
        }
        public void ScanKPItem(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                /*
                 new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "STATION", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "KPITEM", InputType = "STRING", DefaultValue = ""}
                 */
                string strSN = Data["SN"].ToString();
                string station = Data["STATION"].ToString();
                JToken _ItemData = Data["KPITEM"];
                T_R_SN_KP TRKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_SN_KP> ItemData = new List<R_SN_KP>();
                for (int i = 0; i < _ItemData.Count(); i++)
                {
                    if (TRKP.CheckLinkByValue(_ItemData[i]["VALUE"].ToString(), SFCDB))
                    {
                        throw new Exception(_ItemData[i]["VALUE"].ToString()+" has been link on other sn!");
                    }
                    R_SN_KP I = new R_SN_KP();
                    I.ID = _ItemData[i]["ID"].ToString();
                    I.VALUE = _ItemData[i]["VALUE"].ToString();
                    I.MPN = _ItemData[i]["MPN"].ToString();
                    I.PARTNO = _ItemData[i]["PARTNO"].ToString();
                    I.SCANTYPE = _ItemData[i]["SCANTYPE"].ToString();
                    I.ITEMSEQ = double.Parse(_ItemData[i]["ITEMSEQ"].ToString());
                    I.SCANSEQ = double.Parse(_ItemData[i]["SCANSEQ"].ToString());
                    I.DETAILSEQ = double.Parse(_ItemData[i]["DETAILSEQ"].ToString());

                    ItemData.Add(I);
                }
                if (ItemData.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }

                LogicObject.SN SN = new LogicObject.SN();
                SN.Load(strSN, SFCDB, DB_TYPE_ENUM.Oracle);
                MESDataObject.Module.T_R_WO_BASE TWO = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE RWO = TWO.GetWo(SN.WorkorderNo, SFCDB);
                
                List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(SN.ID, station, SFCDB);
                SN_KP KPCONFIG = new SN_KP(snkp, SN.WorkorderNo, SN.SkuNo, SFCDB);

                R_SN_KP kpItem = KPCONFIG.KPS.Find(T => T.ID == ItemData[0].ID);
                if (kpItem == null)
                {
                    throw new Exception("Data Error!");
                }

                List<R_SN_KP> ConfigItem = KPCONFIG.KPS.FindAll(T => T.PARTNO == kpItem.PARTNO
                && T.ITEMSEQ == kpItem.ITEMSEQ
                && T.SCANSEQ == kpItem.SCANSEQ
                );

                if (ConfigItem.Count != ItemData.Count)
                {
                    throw new Exception("Data Error! ConfigItem.Count != ItemData.Count");
                }

                
                for (int i = 0; i < ItemData.Count; i++)
                {
                    Row_R_SN_KP item = (Row_R_SN_KP)TRKP.GetObjByID(ItemData[i].ID, SFCDB);
                    if (item.ITEMSEQ == ItemData[i].ITEMSEQ
                && item.SCANSEQ == ItemData[i].SCANSEQ
                && item.DETAILSEQ == ItemData[i].DETAILSEQ)
                    {
                        item.VALUE = ItemData[i].VALUE;
                        item.MPN = ItemData[i].MPN;
                        item.PARTNO = ItemData[i].PARTNO;
                        item.EDIT_TIME = DateTime.Now;
                        item.EDIT_EMP = LoginUser.EMP_NO;
                        SFCDB.ExecSQL(item.GetUpdateString(DB_TYPE_ENUM.Oracle));
                    }
                    else
                    {
                        throw new Exception("Data Error! 1");
                    }
                }



                StationReturn.Status = StationReturnStatusValue.Pass;
                SFCDB.CommitTrain();
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void GetKPListByListName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ListName = Data["ListName"].ToString();
            try
            {
                KPListBase ret = KPListBase.GetKPListByListName(ListName, SFCDB);
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void CheckKPListName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ListName = Data["ListName"].ToString();
            try
            {
                MESDataObject.Module.T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                StationReturn.Data = T.CheckKPListName(ListName,SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void GetSNStationKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string strSN = Data["SN"].ToString();
                string strSTATION = Data["STATION"].ToString();
                string strWO = null;
                try
                {
                    strWO = Data["WO"].ToString();
                }
                catch
                { }

                LogicObject.SN SN = new LogicObject.SN();
                SN.Load(strSN, SFCDB, DB_TYPE_ENUM.Oracle);

                MESDataObject.Module.T_R_WO_BASE TWO = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE RWO = TWO.GetWo(SN.WorkorderNo, SFCDB);
                T_R_SN_KP TRKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);

                List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(SN.ID, strSTATION, SFCDB);

                SN_KP ret = new SN_KP(snkp, SN.WorkorderNo, SN.SkuNo, SFCDB);


                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void UpLoadKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string Skuno = Data["SkuNo"].ToString();
                string ListName = Data["ListName"].ToString();
                Newtonsoft.Json.Linq.JToken ListData = Data["ListData"];
                T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string NewListID = T.GetNewID(this.BU,SFCDB);
                Row_C_KP_LIST R = (Row_C_KP_LIST)T.NewRow();
                DateTime Now = DateTime.Now;

                T_C_KP_List_Item TItem = new T_C_KP_List_Item(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item RItem = (Row_C_KP_List_Item)TItem.NewRow();

                T_C_KP_List_Item_Detail TDetail = new T_C_KP_List_Item_Detail(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item_Detail RDetail = (Row_C_KP_List_Item_Detail)TDetail.NewRow();

                KPListBase oldList = KPListBase.GetKPListByListName(ListName, SFCDB);
                if (oldList != null)
                {
                    oldList.ReMoveFromDB(SFCDB);
                }

                R.ID = NewListID;
                R.SKUNO = Skuno.Trim();
                R.LISTNAME = ListName;
                R.EDIT_EMP = this.LoginUser.EMP_NO;
                R.EDIT_TIME = Now;

                SFCDB.ExecSQL(R.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                //Item	PartNO	KPName	Station	QTY	ScanType
                DataTable dt = new DataTable();
                dt.Columns.Add("Item");
                dt.Columns.Add("PartNO");
                dt.Columns.Add("KPName");
                dt.Columns.Add("Station");
                dt.Columns.Add("QTY");
                dt.Columns.Add("ScanType");
                List<DataRow> ListItem = new List<DataRow>();
                for (int i = 0; i < ListData.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Item"] = ListData[i]["Item"].ToString();
                    dr["PartNO"] = ListData[i]["PartNO"].ToString();
                    dr["KPName"] = ListData[i]["KPName"].ToString();
                    dr["Station"] = ListData[i]["Station"].ToString();
                    dr["QTY"] = ListData[i]["QTY"].ToString();
                    dr["ScanType"] = ListData[i]["ScanType"].ToString();
                    dt.Rows.Add(dr);
                    ListItem.Add(dr);
                }
                Dictionary<string, string> Item = new Dictionary<string, string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!Item.ContainsKey(dt.Rows[i]["Item"].ToString()))
                    {
                        Item.Add(dt.Rows[i]["Item"].ToString(), dt.Rows[i]["PartNO"].ToString());
                    }
                }
                string[] itemNo = new string[Item.Keys.Count];
                Item.Keys.CopyTo(itemNo, 0);
                for (int i = 0; i < itemNo.Length; i++)
                {
                    List<DataRow> Items = ListItem.FindAll(TT => TT["Item"].ToString() == itemNo[i]);
                    for (int j = 0; j < Items.Count; j++)
                    {
                        if (j == 0)
                        {
                            RItem.ID = TItem.GetNewID(BU, SFCDB);
                            RItem.LIST_ID = NewListID;
                            RItem.KP_NAME = Items[j]["KPName"].ToString();
                            RItem.KP_PARTNO = Items[j]["PartNO"].ToString();
                            RItem.STATION = Items[j]["Station"].ToString();
                            RItem.QTY = double.Parse( Items[j]["QTY"].ToString());
                            RItem.SEQ = i;
                            RItem.EDIT_EMP = this.LoginUser.EMP_NO;
                            RItem.EDIT_TIME = Now;
                            SFCDB.ExecSQL(RItem.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        }

                        RDetail.ID = TDetail.GetNewID(BU, SFCDB);
                        RDetail.ITEM_ID = RItem.ID;
                        RDetail.SCANTYPE = Items[j]["ScanType"].ToString();
                        RDetail.SEQ = j+1;
                        RDetail.EDIT_EMP = RItem.EDIT_EMP;
                        RDetail.EDIT_TIME = Now;
                        SFCDB.ExecSQL(RDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            SFCDB.CommitTrain();
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void RemoveKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                Newtonsoft.Json.Linq.JToken names = Data["ListNames"];
                for (int i = 0; i < names.Count(); i++)
                {
                    KPListBase list = KPListBase.GetKPListByListName(names[i].ToString(), SFCDB);
                    try
                    {
                        SFCDB.BeginTrain();
                        list.ReMoveFromDB(SFCDB);
                        ret.Add(names[i].ToString());
                        SFCDB.CommitTrain();
                    }
                    catch
                    {
                        SFCDB.RollbackTrain();
                    }
                }
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void GetAllKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<MESDataObject.Module.C_KP_LIST > list = KPListBase.getAllData(SFCDB);
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetKPListBySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string Skuno = Data["SKUNO"].ToString();
                List<KPListBase> list = KPListBase.GetKPListBySkuNo(Skuno,SFCDB);
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
            } catch(Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            this.DBPools["SFCDB"].Return(SFCDB);
            

        }
    }
}
