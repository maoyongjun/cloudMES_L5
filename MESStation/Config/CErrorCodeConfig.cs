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
 public class CErrorCodeConfig :MesAPIBase
    {
        #region 方法信息集合
        protected APIInfo FGetByErrorCode = new APIInfo()
        {
            FunctionName = "GetByErrorCode",
            Description = "通過ErrorCode獲取ErrorCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "ErrorCode" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetErrorCodeById = new APIInfo()
        {
            FunctionName = "GetErrorCodeById",
            Description = "通過Id獲取ErrorCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "Id" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetErrorCodeByFuzzySearch = new APIInfo()
        {
            FunctionName = "GetErrorCodeByFuzzySearch",
            Description = "通過模糊查找（不區分大小寫）獲取ErrorCode信息",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SearchValue" } },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FAddNewErrorCode = new APIInfo()
        {
            FunctionName = "AddNewErrorCode",
            Description = "添加新的ErrorCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "ErrorCode" },
                new APIInputInfo() { InputName = "EnglishDescription" },
                new APIInputInfo() { InputName = "ChineseDescription" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FUpdateErrorCodeById = new APIInfo()
        {
            FunctionName = "UpdateErrorCodeById",
            Description = "通過Id更新ErrorCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "Id" },
                new APIInputInfo() { InputName = "ErrorCode" },
                new APIInputInfo() { InputName = "EnglishDescription" },
                new APIInputInfo() { InputName = "ChineseDescription" }
            },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FGetAllErrorCode = new APIInfo()
        {
            FunctionName = "GetAllErrorCode",
            Description = "獲取所有ErrorCode信息",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
        };
        protected APIInfo FDeleteErrorCodeById = new APIInfo()
        {
            FunctionName = "DeleteErrorCodeById",
            Description = "通過Id刪除ErrorCode信息",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "Id" }
            },
            Permissions = new List<MESPermission>()
        };
      
        #endregion 方法信息集合
        public CErrorCodeConfig()
        {
            this.Apis.Add(FGetByErrorCode.FunctionName, FGetByErrorCode);
            this.Apis.Add(FGetErrorCodeById.FunctionName, FGetErrorCodeById);
            this.Apis.Add(FGetErrorCodeByFuzzySearch.FunctionName, FGetErrorCodeByFuzzySearch);
            this.Apis.Add(FAddNewErrorCode.FunctionName, FAddNewErrorCode);
            this.Apis.Add(FUpdateErrorCodeById.FunctionName, FUpdateErrorCodeById);
            this.Apis.Add(FGetAllErrorCode.FunctionName, FGetAllErrorCode);
            this.Apis.Add(FDeleteErrorCodeById.FunctionName, FDeleteErrorCodeById);
        }
        public void GetByErrorCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb,MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string ErrorCode = Data["ErrorCode"].ToString();
                SelectErrorCode = TC_ERROR_CODE.GetByErrorCode(ErrorCode, sfcdb);
                if (SelectErrorCode != null)
                {
                    StationReturn.Data = SelectErrorCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(ErrorCode);
                }
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
        public void GetErrorCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string ErrorCodeId = Data["Id"].ToString();
                SelectErrorCode = TC_ERROR_CODE.GetByid(ErrorCodeId, sfcdb);
                if (SelectErrorCode != null)
                {
                    StationReturn.Data = SelectErrorCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(ErrorCodeId);
                }
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
        public void GetErrorCodeByFuzzySearch(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
               List<C_ERROR_CODE> SelectErrorCode = new List<C_ERROR_CODE>();
                string SearchValue = Data["SearchValue"].ToString().ToUpper();
                SelectErrorCode = TC_ERROR_CODE.GetByFuzzySearch(SearchValue, sfcdb);
                if (SelectErrorCode != null)
                {
                    StationReturn.Data = SelectErrorCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(SearchValue);
                }
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
        public void AddNewErrorCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string ErrorCode = Data["ErrorCode"].ToString().Trim();
                string EDescription = Data["EnglishDescription"].ToString().Trim();
                string CDescription = Data["ChineseDescription"].ToString().Trim();
                if (ErrorCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ErrorCode");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (EDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("EnglishDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (CDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ChineseDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                SelectErrorCode = TC_ERROR_CODE.GetByErrorCode(ErrorCode, sfcdb);
                if (SelectErrorCode == null)
                {
                    C_ERROR_CODE NewErrorCode = new C_ERROR_CODE();
                    NewErrorCode.ID = TC_ERROR_CODE.GetNewID(BU,sfcdb);
                    NewErrorCode.ERROR_CODE = ErrorCode;
                    NewErrorCode.ENGLISH_DESCRIPTION = EDescription;
                    NewErrorCode.CHINESE_DESCRIPTION = CDescription;
                    NewErrorCode.EDIT_EMP = LoginUser.EMP_NO;
                    NewErrorCode.EDIT_TIME = GetDBDateTime();
                    int result = TC_ERROR_CODE.AddNewErrorCode(NewErrorCode,sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Data = NewErrorCode;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                        StationReturn.MessagePara.Add(ErrorCode);
                    }
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(ErrorCode);
                }
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
        public void UpdateErrorCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                string strId = Data["Id"].ToString().Trim();
                string ErrorCode = Data["ErrorCode"].ToString().Trim();
                string EDescription = Data["EnglishDescription"].ToString().Trim();
                string CDescription = Data["ChineseDescription"].ToString().Trim();
                if (ErrorCode.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ErrorCode");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (EDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("EnglishDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                if (CDescription.Length <= 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ChineseDescription");
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }
                SelectErrorCode= TC_ERROR_CODE.GetByid(strId, sfcdb);
                if (SelectErrorCode == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add("Id:"+strId);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;

                }
                SelectErrorCode = TC_ERROR_CODE.GetByErrorCode(ErrorCode, sfcdb);
                if (SelectErrorCode != null && SelectErrorCode.ID != strId)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000008";
                    StationReturn.MessagePara.Add(ErrorCode);
                    this.DBPools["SFCDB"].Return(sfcdb);
                    return;
                }               
                C_ERROR_CODE NewErrorCode = new C_ERROR_CODE();
                NewErrorCode.ID =strId;
                NewErrorCode.ERROR_CODE = ErrorCode;
                NewErrorCode.ENGLISH_DESCRIPTION = EDescription;
                NewErrorCode.CHINESE_DESCRIPTION = CDescription;
                NewErrorCode.EDIT_EMP = LoginUser.EMP_NO;
                NewErrorCode.EDIT_TIME = GetDBDateTime();
                int result = TC_ERROR_CODE.UpdateById(NewErrorCode, sfcdb);
                if (result > 0)
                {
                    StationReturn.Data = NewErrorCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000025";
                    StationReturn.MessagePara.Add(ErrorCode);
                }
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
        public void GetAllErrorCode(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<C_ERROR_CODE> SelectErrorCode = new List<C_ERROR_CODE>();
                SelectErrorCode = TC_ERROR_CODE.GetAllErrorCode(sfcdb);
                StationReturn.Data = SelectErrorCode;
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
        public void DeleteErrorCodeById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                string strId = Data["Id"].ToString().Trim();
                T_C_ERROR_CODE TC_ERROR_CODE = new T_C_ERROR_CODE(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
                C_ERROR_CODE SelectErrorCode = new C_ERROR_CODE();
                SelectErrorCode = TC_ERROR_CODE.GetByid(strId, sfcdb);
                if (SelectErrorCode == null)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara.Add(strId);

                }
                else
                {
                    int result = TC_ERROR_CODE.DeleteById(strId, sfcdb);
                    if (result > 0)
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Data = "";
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000023";
                        StationReturn.MessagePara.Add(strId);
                    }
                }
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
