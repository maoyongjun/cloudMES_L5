using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESStation.BaseClass;
using MESDBHelper;


namespace MESStation.Config
{
  public  class RepairItemSelect : MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetRepairItems = new APIInfo()
        {
            FunctionName = "GetRepairItems",
            Description = "獲取C_REPAIR_ITEMS的維修大項信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "ItemName" } },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FGetRepairItemsSon = new APIInfo()
        {
            FunctionName = "GetRepairItemsSon",
            Description = "獲取C_REPAIR_ITEMS_SON的維修小項信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName= "ItemSon" } },
            Permissions = new List<MESPermission>()
        };
        #endregion 方法信息集合 end

        public RepairItemSelect()
        {
            this.Apis.Add(FGetRepairItems.FunctionName, FGetRepairItems);
            this.Apis.Add(FGetRepairItemsSon.FunctionName, FGetRepairItemsSon);
        }


        /// <summary>
        ///獲取C_REPAIR_ITEMS的維修大項信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairItems(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string ITEM_NAME = Data["ItemName"].ToString(); ;
                List<string> RepairItemsList = new List<string>();
                T_C_REPAIR_ITEMS TC_REPAIR_ITEM = new T_C_REPAIR_ITEMS(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                RepairItemsList = TC_REPAIR_ITEM.GetRepairItemsList(ITEM_NAME, sfcdb);
                StationReturn.Data = RepairItemsList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }


        // <summary>
        ///獲取C_REPAIR_ITEMS_SON的維修小項信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetRepairItemsSon(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string ITEMS_SON = Data["ItemSon"].ToString();
                List<string> RepairItemsSonList = new List<string>();
                T_C_REPAIR_ITEMS_SON TC_REPAIR_ITEM_SON = new T_C_REPAIR_ITEMS_SON(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                T_C_REPAIR_ITEMS RepairItems = new T_C_REPAIR_ITEMS(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_REPAIR_ITEMS RowItems;
                RowItems = RepairItems.GetIDByItemName(ITEMS_SON, sfcdb);
                RepairItemsSonList = TC_REPAIR_ITEM_SON.GetRepairItemsSonList(RowItems.ID, sfcdb);
                StationReturn.Data = RepairItemsSonList;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception ex)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
        }
    }

   
}
