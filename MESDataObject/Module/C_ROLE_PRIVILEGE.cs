using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;

namespace MESDataObject.Module
{
    public class T_C_ROLE_PRIVILEGE : DataObjectTable
    {
        public T_C_ROLE_PRIVILEGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ROLE_PRIVILEGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROLE_PRIVILEGE);
            TableName = "C_ROLE_PRIVILEGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<c_role_privilegeinfobyemp> QueryRolePrivilegeByEmpNo(String LEVEL_FLAG, String BU, string FACTORY, string DPT_NAME, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfobyemp> RolePrivilegeList = new List<c_role_privilegeinfobyemp>();
            if (LEVEL_FLAG == "9")
            {
                sql = $@" select b.id,b.role_name,a.privilege_id,C.PRIVILEGE_NAME,c.privilege_desc from c_role_privilege a,c_role b ,c_privilege c 
                          where b.id=a.role_id and a.privilege_id=c.id and  EXISTS( SELECT 1 FROM C_USER C WHERE B.ROLE_TYPE=C.DPT_NAME AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}')";
            }
            else
            {
                sql = $@" select b.id,b.role_name,a.privilege_id,C.PRIVILEGE_NAME,c.privilege_desc from c_role_privilege a,c_role b ,c_privilege c 
                          where b.id=a.role_id and a.privilege_id=c.id and  EXISTS( SELECT 1 FROM C_USER C WHERE B.ROLE_TYPE=C.DPT_NAME AND C.DPT_NAME='{DPT_NAME}' AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}')";
            }

            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                RolePrivilegeList.Add(new c_role_privilegeinfobyemp
                {
                    ID = item["ID"].ToString().Trim(),
                    ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                    PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                    PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                    PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                });
            }
            return RolePrivilegeList;
        }

        public List<c_role_privilegeinfo> QueryRolePrivilegeByUserID(String LEVEL_FLAG, String BU, string FACTORY, string DPT_NAME, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfo> RolePrivilegeList = new List<c_role_privilegeinfo>();
            if (LEVEL_FLAG == "9")
            {
                sql = $@" SELECT B.* FROM C_USER_ROLE A,C_ROLE B WHERE B.ID=A.ROLE_ID  AND EXISTS( SELECT 1 FROM C_USER C WHERE A.USER_ID=C.ID AND  B.ROLE_TYPE=C.DPT_NAME AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}' )";
            }
            else
            {
                sql = $@" SELECT B.* FROM C_USER_ROLE A,C_ROLE B WHERE B.ID=A.ROLE_ID  AND EXISTS( SELECT 1 FROM C_USER C WHERE A.USER_ID=C.ID AND  B.ROLE_TYPE=C.DPT_NAME AND C.DPT_NAME='{DPT_NAME}' AND C.BU_NAME='{BU}' AND C.FACTORY='{FACTORY}' )";
            }

            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                RolePrivilegeList.Add(new c_role_privilegeinfo
                {
                    ID = item["ID"].ToString().Trim(),
                    ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                    SON_ROLE = QueryRolePrivilegeByRoleID(item["ID"].ToString().Trim(), DB)
                });
            }
            return RolePrivilegeList;
        }

        public List<SON_ROLE> QueryRolePrivilegeByRoleID(string ROLE_ID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<SON_ROLE> SonRolePrivilegeList = new List<SON_ROLE>();
            sql = $@" SELECT * FROM C_PRIVILEGE B WHERE   EXISTS (SELECT 1 FROM C_ROLE_PRIVILEGE A WHERE B.ID=A.PRIVILEGE_ID AND A.ROLE_ID='{ROLE_ID}')";
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                SonRolePrivilegeList.Add(new SON_ROLE
                {
                    ID = item["ID"].ToString().Trim(),
                    PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString().Trim(),
                    PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString().Trim()

                });
            }
            return SonRolePrivilegeList;
        }

        public bool CheckPrivilegeData(string RoleId, String RolePrivilegeId, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            string RolePrivileges = "";
            DataTable dt = new DataTable();
            sql = $@" SELECT *FROM C_ROLE_PRIVILEGE WHERE ROLE_ID='{RoleId}'AND PRIVILEGE_ID ='{RolePrivilegeId}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }

            return res;

        }

        public List<c_role_privilegeinfobyemp> QueryRolePrivilege(string ROLE_ID, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfobyemp> RolePrivilegeList = new List<c_role_privilegeinfobyemp>();

            if (ROLE_ID.Length != 0)
            {
                sql = $@" select c.id,c.role_name,a.privilege_id,b.privilege_name,b.privilege_desc from c_role_privilege a,c_privilege b,c_role c where c.id=a.role_id and a.privilege_id=b.id and a.role_id='{ROLE_ID}'";
            }
            if (ROLE_ID.Length == 0)
            {
                sql = $@" select c.id,c.role_name,a.privilege_id,b.privilege_name,b.privilege_desc from c_role_privilege a,c_privilege b,c_role c where c.id=a.role_id and a.privilege_id=b.id";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                RolePrivilegeList.Add(new c_role_privilegeinfobyemp
                {
                    ID = item["ID"].ToString().Trim(),
                    ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                    PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                    PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                    PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                });
            }
            return RolePrivilegeList;
        }


        public List<c_role_privilegeinfobyemp> CheckTwoRolePrivilegeID(List<c_role_byempl> AllRoleID, string EDITROLE_ID,string EmpLevel, OleExec DB)
        {
            string allroleid = "";
            string PrivilegeID = "";
            string strsql = "";
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role_privilegeinfobyemp> CheckRolePrivilegeIDList = new List<c_role_privilegeinfobyemp>();
            if (AllRoleID.Count != 0)
            {
                ///当登录用户等级不为9时,只能管理自己拥有的角色
                AllRoleID = AllRoleID.Where(Q => Q.ID != EDITROLE_ID).ToList();
                foreach (c_role_byempl item in AllRoleID)
                {
                    allroleid += "'" + item.ID + "',";
                }

                allroleid = allroleid.TrimEnd(',');
                strsql = " a.role_id in (" + allroleid + ")";
                if (EmpLevel!="0")
                {
                    ///当登录用户等级不为9时,只能管理自己拥有的角色
                    sql = $@" select c.id,c.role_name,a.privilege_id,d.privilege_name,d.privilege_desc from c_role_privilege a,c_role c,c_privilege d where a.role_id=c.id and a.privilege_id=d.id  and  {strsql}  
                      and not exists(select 1 from c_role_privilege b where b.privilege_id=a.privilege_id and b.role_id='{EDITROLE_ID}')";
                    ///当登录用户等级不为9时,只能管理自己拥有的角色

                    dt = DB.ExecSelect(sql).Tables[0];
                    foreach (DataRow item in dt.Rows)
                    {
                        CheckRolePrivilegeIDList.Add(new c_role_privilegeinfobyemp
                        {
                            ID = item["ID"].ToString().Trim(),
                            ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                            PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                            PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                            PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                        });
                    }
                }
                if (EmpLevel=="9")
                {
                    if (CheckRolePrivilegeIDList!=null)
                    {
                        foreach (c_role_privilegeinfobyemp item in CheckRolePrivilegeIDList)
                        {
                            PrivilegeID += "'" + item.PRIVILEGE_ID + "',";
                        }
                        PrivilegeID = PrivilegeID.TrimEnd(',');
                        strsql = " and  a.id not in (" + PrivilegeID + ")";
                    }
                    sql = $@" select   'NO' ID ,
                                 'NO' role_name  ,
                                 a.id privilege_id,
                                 a.privilege_name,
                                 a.privilege_desc from c_privilege a    where
                                     NOT EXISTS
                                   (SELECT 1
                                     FROM c_role_privilege b
                                    WHERE b.privilege_id = a.id AND b.role_id = '{EDITROLE_ID}' ) {strsql}";

                    dt = DB.ExecSelect(sql).Tables[0];
                    foreach (DataRow item in dt.Rows)
                    {
                        CheckRolePrivilegeIDList.Add(new c_role_privilegeinfobyemp
                        {
                            ID = item["ID"].ToString().Trim(),
                            ROLE_NAME = item["ROLE_NAME"].ToString().Trim(),
                            PRIVILEGE_ID = item["PRIVILEGE_ID"].ToString(),
                            PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                            PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()

                        });
                    }
                }
        
            }

            return CheckRolePrivilegeIDList;
        }

        public Row_C_ROLE_PRIVILEGE GetC_Role_Privilege_ID(string ROLE_ID,string PRIVILEGE_ID,OleExec DB)
        {
            string sql = string.Empty;
            sql = $@" SELECT * FROM  C_ROLE_PRIVILEGE  WHERE ROLE_ID='{ROLE_ID}' AND PRIVILEGE_ID='{PRIVILEGE_ID}'  ";
            DataSet res = DB.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_ROLE_PRIVILEGE ret = (Row_C_ROLE_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }

             
        }


        public bool CheckRolePrivilege(string ROLE_ID, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            sql = $@" SELECT *FROM C_USER_ROLE WHERE ROLE_ID ='{ROLE_ID}' ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        public DataObjectBase GetObjByRoleID(string RoleID, OleExec DB)
        {
            return GetObjByRoleID(RoleID, DB, DBType);
        }

        public DataObjectBase GetObjByRoleID(string RoleID, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from {TableName} where USER_ID = '{RoleID}'";
            DataSet res = DB.ExecSelect(strSql);
            if (RowType == null)
            {
                DataObjectBase ret = NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                Assembly assembly = Assembly.Load("MESDataObject");
                object API_CLASS = assembly.CreateInstance(RowType.FullName, true, BindingFlags.CreateInstance, null, new object[] { DataInfo }, null, null);
                MethodInfo Function = RowType.GetMethod("loadData");
                Function.Invoke(API_CLASS, new object[] { res.Tables[0].Rows[0] });
                return (DataObjectBase)API_CLASS;
            }
        }

    }


    public class c_role_privilegeinfobyemp
    {
        public string ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string PRIVILEGE_ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }

    }

    public class c_role_privilegeinfo
    {
        public string ID { get; set; }
        public string ROLE_NAME { get; set; }
        public List<SON_ROLE> SON_ROLE { get; set; }


    }

    public class SON_ROLE
    {
        public string ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }

    }

    public class C_ROLE_PRIVILEGE
    {
        public string ID { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }
    }
    public class Row_C_ROLE_PRIVILEGE : DataObjectBase
    {
        public Row_C_ROLE_PRIVILEGE(DataObjectInfo info) : base(info)
        {

        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string SYSTEM_NAME
        {
            get
            {
                return (string)this["SYSTEM_NAME"];
            }
            set
            {
                this["SYSTEM_NAME"] = value;
            }
        }
        public string ROLE_ID
        {
            get
            {
                return (string)this["ROLE_ID"];
            }
            set
            {
                this["ROLE_ID"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public string PRIVILEGE_ID
        {
            get
            {
                return (string)this["PRIVILEGE_ID"];
            }
            set
            {
                this["PRIVILEGE_ID"] = value;
            }
        }
    }
}