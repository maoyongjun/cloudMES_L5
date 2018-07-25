using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.MESReturnView.Public;
using MESStation.BaseClass;
using MESDBHelper;
using MESDataObject;
using System.Data;
using MESDataObject.Module;

namespace MESStation.MESUserManager
{
    class UserLogin : MesAPIBase
    {
        static Random rand = new Random();
        ///List<APIInfo> TCodes = new List<APIInfo>();
        protected APIInfo FLogin = new APIInfo()
        {
            FunctionName = "Login",
            Description = "用戶登錄，成功返回Token",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "EMP_NO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Password", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "Language", InputType = "string", DefaultValue = "" },
                 new APIInputInfo() { InputName = "BU_NAME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
           
        };


        public UserLogin()
        {
            this.Apis.Add(FLogin.FunctionName, FLogin);
        }


        public void Login(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            string EMP_NO = Data["EMP_NO"].ToString();
            string PWD = Data["Password"].ToString();
            string BU_NAME = Data["BU_NAME"].ToString();
            DataSet res = new DataSet();
            Language = Data["Language"].ToString();
            MESReturnMessage.Language = Language;
            LoginReturn lr = new LoginReturn();
            MESDBHelper.OleExec SFCDB = _DBPools["SFCDB"].Borrow();

            MESDataObject.Module.T_c_user GetLoginUser = new MESDataObject.Module.T_c_user(SFCDB, this.DBTYPE);
            MESDataObject.Module.Row_c_user rcu = (Row_c_user)GetLoginUser.NewRow();
            rcu = GetLoginUser.getC_Userbyempno(EMP_NO, SFCDB, this.DBTYPE);
            if (rcu == null)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000010";
                _DBPools["SFCDB"].Return(SFCDB);
                return;
            }
            c_user_info user_info = new c_user_info();
            user_info = GetLoginUser.GetLoginUser(EMP_NO, SFCDB);

            LogicObject.User lu = new LogicObject.User();
            if (PWD == rcu.EMP_PASSWORD)
            {
                lu.ID = user_info.ID;
                lu.FACTORY = user_info.FACTORY;
                lu.BU = user_info.BU_NAME;
                lu.EMP_NO = user_info.EMP_NO;
                lu.EMP_LEVEL = user_info.EMP_LEVEL;
                lu.DPT_NAME = user_info.DPT_NAME;
                string token1 = DateTime.Now.ToString("yyyyMMddHHmmss");
                string token2 = rand.Next(100, 999).ToString();
                char[] TokenChars = (token1 + token2).ToArray();
                byte[] TokenBytes = Encoding.Default.GetBytes(TokenChars);
                string TokenBas64 = Convert.ToBase64String(TokenBytes);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000009";
                LoginUser = lu;
                //lr = new LoginReturn() { Token = TokenBas64, User_ID = user.EMP_NO};
                lr = new LoginReturn() { Token = TokenBas64, User_ID = LoginUser.EMP_NO, UserInfo = user_info };
            }
            else
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000010";
            }
            StationReturn.Data = lr;
            _DBPools["SFCDB"].Return(SFCDB);

        }
    }
}
