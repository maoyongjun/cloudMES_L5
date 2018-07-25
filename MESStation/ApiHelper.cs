using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using System.Reflection;
using MESStation.MESReturnView;

namespace MESStation
{
    public class ApiHelper:BaseClass.MesAPIBase
    {
        protected APIInfo FGetApiClassList = new APIInfo()
        {
            FunctionName = "GetApiClassList",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>() 
        };
        protected APIInfo FGetApiFunctionsList = new APIInfo()
        {
            FunctionName = "GetApiFunctionsList",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName="CLASSNAME" } },
            Permissions = new List<MESPermission>()
        };
        public ApiHelper()
        {
            _MastLogin = false;
            this.Apis.Add(FGetApiClassList.FunctionName,FGetApiClassList);
            this.Apis.Add(FGetApiFunctionsList.FunctionName, FGetApiFunctionsList);

        }

        public void GetApiClassList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            MESReturnView.Public.GetApiClassListReturncs ret = new MESReturnView.Public.GetApiClassListReturncs();
            Assembly assenbly = Assembly.Load("MESStation");
            Type tagType = typeof(BaseClass.MesAPIBase);
            Type[] t = assenbly.GetTypes();
            for (int i = 0; i < t.Length; i++)
            {
                TypeInfo ti = t[i].GetTypeInfo();
                Type baseType = ti.BaseType;
                if (baseType == tagType)
                {
                    ret.ClassName.Add(ti.FullName);
                }
            }
            StationReturn.Data = ret;
            StationReturn.Status = "Pass";
            StationReturn.Message = "獲取成功";
        }

        public void GetApiFunctionsList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            string ClassName = Data["CLASSNAME"].ToString();
            Assembly assemby = Assembly.Load("MESStation");
            Type t = assemby.GetType(ClassName);
            object obj = assemby.CreateInstance(ClassName);
            MesAPIBase API = (MesAPIBase)obj;
            MESReturnView.Public.GetApiFunctionsListReturn ret = new MESReturnView.Public.GetApiFunctionsListReturn();
            ret.APIS = API.Apis;
            StationReturn.Data = ret;
            StationReturn.Status = "Pass";

        }
    }
}
