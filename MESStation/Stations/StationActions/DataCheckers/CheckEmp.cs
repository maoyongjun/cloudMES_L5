using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESStation.LogicObject;

namespace MESStation.Stations.StationActions.DataCheckers
{
    class CheckEmp
    {
        public static void InputEmpPrivchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession EMP_NOLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (EMP_NOLoadPoint == null)
            {
                EMP_NOLoadPoint = new MESStationSession() { MESDataType = "INPUTEMP", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(EMP_NOLoadPoint);
            }
            bool bPrivilege = false;
            string empNo = Input.Value.ToString();
            //T_c_user cUser = new T_c_user(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //Row_c_user rUser = cUser.getC_Userbyempno(empNo, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            T_c_user_role cUserRole = new T_c_user_role(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<get_c_roleid> roleList = cUserRole.GetRoleID(empNo, Station.SFCDB);
            List<string> listRoleID = new List<string>();
            foreach (var item in roleList)
            {
                listRoleID.Add(item.ROLE_ID);
            }
            T_C_ROLE_PRIVILEGE tRolePrivilege = new T_C_ROLE_PRIVILEGE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<c_role_privilegeinfobyemp> privilegeList = new List<c_role_privilegeinfobyemp>();
            foreach (string item in listRoleID)
            {
                List<c_role_privilegeinfobyemp> tempList = tRolePrivilege.QueryRolePrivilege(item, Station.SFCDB);
                privilegeList.AddRange(tempList);
            }
            EMP_NOLoadPoint.Value = privilegeList;
            foreach (var item in privilegeList)
            {
                if (item.PRIVILEGE_NAME == Station.DisplayName)
                {
                    bPrivilege = true;
                }
            }
            if (bPrivilege)
            {
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                throw new Exception("no privilege");
            }
        }

        public static void LoginEmpPrivchecker(MESStation.BaseClass.MESStationBase Station, MESStation.BaseClass.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception("參數數量不正確!");
            }
            MESStationSession EMP_LoginLoadPoint = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (EMP_LoginLoadPoint == null)
            {
                EMP_LoginLoadPoint = new MESStationSession() { MESDataType = "LOGINOUTEMP", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
                Station.StationSession.Add(EMP_LoginLoadPoint);
            }

            bool bPrivilege = false;
            string loginUserEmpNo = Input.Value.ToString();
            T_c_user_role cUserRole = new T_c_user_role(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<get_c_roleid> roleList = cUserRole.GetRoleID(loginUserEmpNo, Station.SFCDB);
            List<string> listRoleID = new List<string>();
            foreach (var item in roleList)
            {
                listRoleID.Add(item.ROLE_ID);
            }
            T_C_ROLE_PRIVILEGE tRolePrivilege = new T_C_ROLE_PRIVILEGE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<c_role_privilegeinfobyemp> privilegeList = new List<c_role_privilegeinfobyemp>();
            foreach (string item in listRoleID)
            {
                List<c_role_privilegeinfobyemp> tempList = tRolePrivilege.QueryRolePrivilege(item, Station.SFCDB);
                privilegeList.AddRange(tempList);
            }
            EMP_LoginLoadPoint.Value = privilegeList;
            foreach (var item in privilegeList)
            {
                if (item.PRIVILEGE_NAME==Station.DisplayName)
                {
                    bPrivilege = true;
                }
            }
            if (bPrivilege)
            {
                Station.AddMessage("MES00000001", new string[] { }, MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                throw new Exception("no privilege");
            }
        }
    }
}
