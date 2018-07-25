using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.LogicObject
{
    public class SN
    {
        public string ID { get { return baseSN.ID; } }    // ID號 
        public string SerialNo { get { return baseSN.SN; } }    // 產品SN 
        public string SkuNo { get { return baseSN.SKUNO; } }    // 機種料號 
        public string WorkorderNo { get { return baseSN.WORKORDERNO; } }    // 工單號 
        public string Plant { get { return baseSN.PLANT; } }    // 廠別 
        public string RouteID { get { return baseSN.ROUTE_ID; } }    // 工單路由 
        public string StartedFlag { get { return baseSN.STARTED_FLAG; } }    // 產品是否已loading進工單標位 
        public DateTime? StartTime { get { return baseSN.START_TIME; } }    // 產品Loading時間 
        public string PackedFlag { get { return baseSN.PACKED_FLAG; } }    // 包裝標志位 
        public DateTime? PackDate { get { return baseSN.PACKDATE; } }    // 包裝時間 
        public string CompletedFlag { get { return baseSN.COMPLETED_FLAG; } }    // 產品完工狀態標誌位 
        public DateTime? CompletedTime { get { return baseSN.COMPLETED_TIME; } }    // 產品完工時間 
        public string ShippedFlag { get { return baseSN.SHIPPED_FLAG; } }    // 出貨標誌位 
        public DateTime? ShipDate { get { return baseSN.SHIPDATE; } }    // 出貨時間 
        public string RepairFailedFlag { get { return baseSN.REPAIR_FAILED_FLAG; } }    // Fail標誌位 
        public string CurrentStation { get { return baseSN.CURRENT_STATION; } }    // 當前站 
        public string NextStation { get { return baseSN.NEXT_STATION; } }    // 下一站 
        //public string KP_LIST_ID;    // PE配置的keypart信息 
        public string PONO { get { return baseSN.PO_NO; } }    // 產品的PO 
        public string CustomerOrderNo { get { return baseSN.CUST_ORDER_NO; } }    // 產品的任務令 
        public string CustomerPartNo { get { return baseSN.CUST_PN; } }    // 客戶料號 
        public string BoxSN { get { return baseSN.BOXSN; } }    // Box條碼 
        public string ScrapedFlag { get { return baseSN.SCRAPED_FLAG; } }    // 是否報廢 
        public DateTime? ScrapedTime { get { return baseSN.SCRAPED_TIME; } }    // 報廢日期 
        public string ProductStatus { get { return baseSN.PRODUCT_STATUS; } }    // 產品狀態 
        public double? ReworkCount { get { return baseSN.REWORK_COUNT; } }    // 重工次數 
        public string ValidFlag { get { return baseSN.VALID_FLAG; } }    // 是否有效 
        public string StockStatus { get { return baseSN.STOCK_STATUS; } }    //是否入庫  
        //public string EDIT_EMP;    // 最後編輯人 
        //public DateTime EDIT_TIME;    // 最後編輯時間 

        public List<C_KEYPART> KeyPartList { get { return _keyPartList; } }

        public bool isPacked(OleExec DB)
        {
            string SNID = this.baseSN.ID;

            string strSql = $@"SELECT COUNT(1) FROM R_SN_PACKING WHERE SN_ID ='{SNID}'";
            int count;
            if (! Int32.TryParse(DB.ExecSelectOneValue(strSql).ToString(), out count))
            {
                throw new Exception("Err Select !");
            }
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }


        public Route Route
        {
            get
            {
                if (_route == null)
                {
                    //_route = new Route(this.RouteID, sfcdb, DBType);
                    _route = new Route();
                }
                return _route;
            }
        }

        Row_R_SN rBaseSN;
        R_SN baseSN;
        OleExec sfcdb;
        MESDataObject.DB_TYPE_ENUM DBType;
        List<C_KEYPART> _keyPartList;
        Route _route;

        public SN() { }

        //add by LLF 2018-02-22 begin
        public void PanelAndSN(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbType)
        {
            PanelAndSNLoad(Sn, sfcdb, dbType);
        }
        //add by LLF 2018-02-22 end
        public SN(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbType)
        {
            Load(Sn, sfcdb, dbType);
        }

        public void PanelSN(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM dbType)
        {
            PanelLoad(Sn, sfcdb, dbType);
        }

        public void PanelLoad(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            T_R_SN trsn = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                baseSN = trsn.GetDetailByPanelSN(Sn, sfcdb);
            }
            T_C_KEYPART tKeyPart = new T_C_KEYPART(sfcdb, DBType);
            if (!string.IsNullOrEmpty(baseSN.KP_LIST_ID))
            {
                _keyPartList = tKeyPart.GetKeyPartList(sfcdb, baseSN.KP_LIST_ID);
            }
        }

        //Add by LLF 2018-02-22 begin
        public void PanelAndSNLoad(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            T_R_SN trsn = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                baseSN = trsn.GetDetailByPanelAndSN(Sn, sfcdb);
            }
        }
        //Add by LLF 2018-02-22 end

        public void Load(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            T_R_SN trsn = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                baseSN = trsn.GetDetailBySN(Sn, sfcdb);
            }
            T_C_KEYPART tKeyPart = new T_C_KEYPART(sfcdb, DBType);
            if (!string.IsNullOrEmpty(baseSN.KP_LIST_ID))
            {
                _keyPartList = tKeyPart.GetKeyPartList(sfcdb, baseSN.KP_LIST_ID);
            }
        }

        public void Reload(string Sn, OleExec sfcdb)
        {
            Load(Sn, sfcdb, DBType);
        }
        /// <summary>
        /// 獲取實際連板數量
        /// </summary>
        /// <param name="Sn"></param>
        /// <param name="PanelFlag"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        //Add by LLF 2018-01-27 Begin
        public int GetLinkQty(string Sn, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            int LinkQty = 1;
            this.sfcdb = sfcdb;
            this.DBType = _DBType;
            List<R_SN> ListSN = new List<R_SN>();

            T_R_SN R_SN = new T_R_SN(sfcdb, DBType);
            if (!string.IsNullOrEmpty(Sn))
            {
                ListSN = R_SN.GetRSNbySN(Sn, sfcdb);
                if (ListSN == null)
                {
                    ListSN = R_SN.GetRSNbyPsn(Sn, sfcdb);
                    LinkQty = ListSN.Count;
                }
            }
            return LinkQty;
        }
        /// <summary>
        /// 檢查序列號規則
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="RuleName"></param>
        /// <param name="sfcdb"></param>
        /// <param name="_DBType"></param>
        /// <returns></returns>
        //Add by LLF 2018-02-01 begin
        public bool CheckSNRule(string StrSN, string RuleName, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            bool CheckFlag = false;
            T_C_SN_RULE C_SN_RULE = new T_C_SN_RULE(sfcdb, DBType);
            CheckFlag = C_SN_RULE.CheckSNRule(StrSN, RuleName, sfcdb);
            return CheckFlag;
        }
        //Add by LLF 2018-02-01 end
        //public bool CheckSNExist(string StrSN,OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        //{
        //    bool CheckFlag = false;
        //    T_R_SN R_SN = new T_R_SN(sfcdb, DBType);
        //    CheckFlag = R_SN.CheckSNExists(StrSN,sfcdb);
        //    return CheckFlag;
        //}

        public R_SN LoadSN(string SerialNo, OleExec DB)
        {
            R_SN RSN = null;
            T_R_SN R_Sn = new T_R_SN(DB, DBType);
            RSN = R_Sn.LoadSN(SerialNo, DB);
            return RSN;
        }


        public R_PANEL_SN LoadPanelBySN(string StrSN, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            R_PANEL_SN R_Panel_SN = null;
            T_R_PANEL_SN R_PANEL_SN = new T_R_PANEL_SN(sfcdb, DBType);
            R_Panel_SN = R_PANEL_SN.GetPanelBySn(StrSN, sfcdb);
            return R_Panel_SN;
        }

        public bool CheckPanelVirtualSNExist(string StrSN, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            bool CheckFlag = false;
            T_R_PANEL_SN R_PANEL_SN = new T_R_PANEL_SN(sfcdb, DBType);
            CheckFlag = R_PANEL_SN.CheckPanelVirtualSN(StrSN, sfcdb);
            return CheckFlag;
        }
        /// <summary>
        /// 獲取虛擬SN add by LLF 2018-02-05
        /// </summary>
        /// <param name="StrSN"></param>
        /// <param name="sfcdb"></param>
        /// <param name="_DBType"></param>
        /// <returns></returns>
        public R_PANEL_SN GetPanelVirtualSN(string StrSN, OleExec sfcdb, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            R_PANEL_SN Row = null;
            T_R_PANEL_SN R_PANEL_SN = new T_R_PANEL_SN(sfcdb, DBType);
            Row = R_PANEL_SN.GetPanelVirtualSN(StrSN, sfcdb);
            return Row;
        }

        public string StringListToString(List<string> Obj)
        {
            string Str = "";
            if (Obj.Count > 0)
            {
                for (int i = 0; i < Obj.Count; i++)
                {
                    if (i == Obj.Count - 1)
                    {
                        Str += Obj[i].ToString();
                    }
                    else
                    {
                        Str += Obj[i].ToString() + ",";
                    }
                }
            }
            return Str;
        }


        public override string ToString()
        {
            //return SerialNo;
            return this.baseSN == null ? null : SerialNo;
        }

        //void GetKeyPart(string PN, string seq)
        //{

        //}


        /// <summary>
        ///  寫入 r_sn_kp 
        /// </summary>
        /// <param name="rowWo"></param>
        /// <param name="r_sn"></param>
        /// <param name="sfcdb"></param>
        /// <param name="Station"></param>
        /// <param name="sfcdbType"></param>
        /// <param name=""></param>
        public void InsertR_SN_KP(WorkOrder woObject, R_SN r_sn, OleExec sfcdb, BaseClass.MESStationBase Station, MESDataObject.DB_TYPE_ENUM sfcdbType)
        {
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(sfcdb, sfcdbType);
            T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(sfcdb, sfcdbType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(sfcdb, sfcdbType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(sfcdb, sfcdbType);
            T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(sfcdb, sfcdbType);
            Row_R_SN_KP rowSNKP;

            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();
            int scanseq = 0;
            int result;
            string skuMpn = "";
            try
            {

                kpItemList = t_c_kp_list_item.GetItemObjectByListId(woObject.KP_LIST_ID, sfcdb);
                if (kpItemList == null || kpItemList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                }

                foreach (C_KP_List_Item kpItem in kpItemList)
                {
                    itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, sfcdb);
                    if (itemDetailList == null || itemDetailList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                    }

                    skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(sfcdb, woObject.SkuNO, kpItem.KP_PARTNO);
                    if (skuMpnList.Count != 0)
                    {
                        skuMpn = skuMpnList[0].MPN;
                    }

                    foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                    {
                        scanseq = scanseq + 1;
                        kpRule = c_kp_rule.GetKPRule(sfcdb, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                        if (kpRule == null)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }
                        if (kpRule.REGEX == "")
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }
                        rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                        rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, sfcdb);
                        rowSNKP.R_SN_ID = r_sn.ID;
                        rowSNKP.SN = r_sn.SN;
                        rowSNKP.VALUE = "";
                        rowSNKP.PARTNO = kpItem.KP_PARTNO;
                        rowSNKP.KP_NAME = kpItem.KP_NAME;
                        rowSNKP.MPN = skuMpn;
                        rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                        rowSNKP.ITEMSEQ = kpItem.SEQ;
                        rowSNKP.SCANSEQ = scanseq;
                        rowSNKP.DETAILSEQ = itemDetail.SEQ;
                        rowSNKP.STATION = kpItem.STATION;
                        rowSNKP.REGEX = kpRule.REGEX;
                        rowSNKP.VALID_FLAG = 1;
                        rowSNKP.EXKEY1 = "";
                        rowSNKP.EXVALUE1 = "";
                        rowSNKP.EXKEY2 = "";
                        rowSNKP.EXVALUE2 = "";
                        rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                        result = Convert.ToInt32(sfcdb.ExecSQL(rowSNKP.GetInsertString(sfcdbType)));
                        if (result <= 0)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateSNKP(WorkOrder woObject, List<R_SN> snList,BaseClass.MESStationBase Station)
        {
            T_C_KP_LIST t_c_kp_list = new T_C_KP_LIST(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item t_c_kp_list_item = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item_Detail t_c_kp_list_item_detail = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            T_C_SKU_MPN t_c_sku_mpn = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
            T_C_KP_Rule c_kp_rule = new T_C_KP_Rule(Station.SFCDB, Station.DBType);
            Row_R_SN_KP rowSNKP;

            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();
            int scanseq = 0;
            int result;
            string skuMpn = "";
            try
            {

                kpItemList = t_c_kp_list_item.GetItemObjectByListId(woObject.KP_LIST_ID, Station.SFCDB);
                if (kpItemList == null || kpItemList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                }

                foreach (R_SN r_sn in snList)
                {
                    result = t_r_sn_kp.DeleteBySNID(r_sn.ID, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "DELETE" }));
                    }

                    foreach (C_KP_List_Item kpItem in kpItemList)
                    {
                        itemDetailList = t_c_kp_list_item_detail.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                        if (itemDetailList == null || itemDetailList.Count == 0)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { woObject.SkuNO }));
                        }

                        skuMpnList = t_c_sku_mpn.GetMpnBySkuAndPartno(Station.SFCDB, woObject.SkuNO, kpItem.KP_PARTNO);
                        if (skuMpnList.Count != 0)
                        {
                            skuMpn = skuMpnList[0].MPN;
                        }


                        foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                        {
                            scanseq = scanseq + 1;
                            kpRule = c_kp_rule.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                            if (kpRule == null)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                            }
                            if (kpRule.REGEX == "")
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                            }
                            rowSNKP = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                            rowSNKP.ID = t_r_sn_kp.GetNewID(Station.BU, Station.SFCDB);
                            rowSNKP.R_SN_ID = r_sn.ID;
                            rowSNKP.SN = r_sn.SN;
                            rowSNKP.VALUE = "";
                            rowSNKP.PARTNO = kpItem.KP_PARTNO;
                            rowSNKP.KP_NAME = kpItem.KP_NAME;
                            rowSNKP.MPN = skuMpn;
                            rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                            rowSNKP.ITEMSEQ = kpItem.SEQ;
                            rowSNKP.SCANSEQ = scanseq;
                            rowSNKP.DETAILSEQ = itemDetail.SEQ;
                            rowSNKP.STATION = kpItem.STATION;
                            rowSNKP.REGEX = kpRule.REGEX;
                            rowSNKP.VALID_FLAG = 1;
                            rowSNKP.EXKEY1 = "";
                            rowSNKP.EXVALUE1 = "";
                            rowSNKP.EXKEY2 = "";
                            rowSNKP.EXVALUE2 = "";
                            rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                            result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                            if (result <= 0)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
