using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PRIVILEGE : DataObjectTable
    {
        public T_C_PRIVILEGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PRIVILEGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PRIVILEGE);
            TableName = "C_PRIVILEGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public DataTable CheckPrivilegeID(string PRIVILEGE_ID,string PRIVILEGE_NAME,  OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@" select * from c_privilege where PRIVILEGE_ID='{PRIVILEGE_ID}' or PRIVILEGE_NAME='{PRIVILEGE_NAME}' ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;
        }

        public DataTable SelectPrivilegeID( OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@" select * from c_privilege ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;
        }
        public Row_C_PRIVILEGE getC_PrivilegebyID(string id, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_PRIVILEGE where ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_PRIVILEGE ret = (Row_C_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public Row_C_PRIVILEGE getC_PrivilegebyMenuID(string id, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_PRIVILEGE where MENU_ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_PRIVILEGE ret = (Row_C_PRIVILEGE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<PrivilegeEditModel> GetUserRolePrivilege(string LoginUserEmp, string EditEmp, string EmpLevel, MESDBHelper.OleExec SFCDB)
        {
            string sql = string.Empty;
            /////9 emp_leve
            if (EmpLevel == "9")
            {
                sql = $@" SELECT A.ID,
       A.SYSTEM_NAME,
       A.PRIVILEGE_NAME,
       A.PRIVILEGE_DESC
  FROM C_PRIVILEGE A
 WHERE     A.ID  NOT IN
              (SELECT PRIVILEGE_ID
                 FROM C_USER_PRIVILEGE
                WHERE USER_ID IN (SELECT ID
                                    FROM C_USER
                                   WHERE EMP_NO = '{EditEmp}'))
UNION
SELECT A.ID,
       A.SYSTEM_NAME,
       A.PRIVILEGE_NAME,
       A.PRIVILEGE_DESC
  FROM C_PRIVILEGE A  
 WHERE     A.ID NOT IN
              (SELECT Q.PRIVILEGE_ID
                 FROM C_ROLE_PRIVILEGE Q, C_USER_ROLE W
                WHERE     Q.ROLE_ID = W.ROLE_ID
                      AND W.USER_ID IN (SELECT ID
                                          FROM C_USER
                                         WHERE EMP_NO = '{EditEmp}')) ";
            }
            else
            {
                sql = $@" SELECT A.ID,
                       A.SYSTEM_NAME,
                       A.PRIVILEGE_NAME,
                       A.PRIVILEGE_DESC
                  FROM C_PRIVILEGE A, C_USER B, C_USER_PRIVILEGE C
                 WHERE     C.USER_ID = B.ID
                       AND C.PRIVILEGE_ID = A.MENU_ID 
                       AND B.EMP_NO = '{LoginUserEmp}' 
                       AND C.PRIVILEGE_ID NOT IN (SELECT PRIVILEGE_ID
                                    FROM C_USER_PRIVILEGE
                                   WHERE USER_ID IN (SELECT ID
                                                       FROM C_USER
                                                      WHERE EMP_NO =
                                                               '{EditEmp}'))
                                    UNION
                                    SELECT A.ID,
                                           A.SYSTEM_NAME,
                                           A.PRIVILEGE_NAME,
                                           A.PRIVILEGE_DESC
                                      FROM C_PRIVILEGE A,
                                           C_USER_ROLE B,
                                           C_ROLE_PRIVILEGE C,
                                           C_USER D
                                     WHERE     C.ROLE_ID = B.ROLE_ID
                                           AND A.ID = C.PRIVILEGE_ID 
                                           AND D.EMP_NO = '{LoginUserEmp}' 
                                           AND D.ID = B.USER_ID
                       AND C.PRIVILEGE_ID NOT IN (SELECT Q.PRIVILEGE_ID
                                                    FROM C_ROLE_PRIVILEGE Q, C_USER_ROLE W
                                                   WHERE     Q.ROLE_ID = W.ROLE_ID
                                                         AND W.USER_ID IN (SELECT ID
                                                                             FROM C_USER
                                                                            WHERE EMP_NO =
                                                                                     '{EditEmp}'))  ";
            }

            DataSet res = SFCDB.ExecSelect(sql);
            List<PrivilegeEditModel> Privilegelist = new List<PrivilegeEditModel>();
            if (res.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in res.Tables[0].Rows)
                {
                    Privilegelist.Add(new PrivilegeEditModel
                    {
                        ID = item["ID"].ToString(),
                        SYSTEM_NAME= item["SYSTEM_NAME"].ToString(),
                        PRIVILEGE_NAME= item["PRIVILEGE_NAME"].ToString(),
                        PRIVILEGE_DESC= item["PRIVILEGE_DESC"].ToString()
                    });
                }

            }
            return Privilegelist;
        }

        public List<PrivilegeEditModel> GetUserEditPrivilege(string LoginUserEmp, string EditEmp, string EmpLevel, MESDBHelper.OleExec SFCDB)
        {
            string sql = string.Empty;
            /////9 emp_leve
            sql = $@"   SELECT A.ID,
                               A.SYSTEM_NAME,
                               A.PRIVILEGE_NAME,
                               A.PRIVILEGE_DESC
                          FROM C_PRIVILEGE A, C_USER B, C_USER_PRIVILEGE C
                         WHERE C.USER_ID = B.ID
                           AND C.PRIVILEGE_ID = A.ID  AND B.EMP_NO = '{EditEmp}' ";

            DataSet res = SFCDB.ExecSelect(sql);
            List<PrivilegeEditModel> Privilegelist = new List<PrivilegeEditModel>();
            if (res.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in res.Tables[0].Rows)
                {
                    Privilegelist.Add(new PrivilegeEditModel
                    {
                        ID = item["ID"].ToString(),
                        SYSTEM_NAME = item["SYSTEM_NAME"].ToString(),
                        PRIVILEGE_NAME = item["PRIVILEGE_NAME"].ToString(),
                        PRIVILEGE_DESC = item["PRIVILEGE_DESC"].ToString()
                    });
                }

            }
            return Privilegelist;
        }
    }
    public class Row_C_PRIVILEGE : DataObjectBase
    {
        public Row_C_PRIVILEGE(DataObjectInfo info) : base(info)
        {

        }
        public C_PRIVILEGE GetDataObject()
        {
            C_PRIVILEGE DataObject = new C_PRIVILEGE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.MENU_ID = this.MENU_ID;
            DataObject.PRIVILEGE_NAME = this.PRIVILEGE_NAME;
            DataObject.PRIVILEGE_DESC = this.PRIVILEGE_DESC;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            return DataObject;
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
        public string MENU_ID
        {
            get
            {
                return (string)this["MENU_ID"];
            }
            set
            {
                this["MENU_ID"] = value;
            }
        }
        public string PRIVILEGE_NAME
        {
            get
            {
                return (string)this["PRIVILEGE_NAME"];
            }
            set
            {
                this["PRIVILEGE_NAME"] = value;
            }
        }
        public string PRIVILEGE_DESC
        {
            get
            {
                return (string)this["PRIVILEGE_DESC"];
            }
            set
            {
                this["PRIVILEGE_DESC"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
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

        public string BASECONFIG_FLAG
        {
            get
            {
                return (string)this["BASECONFIG_FLAG"];
            }
            set
            {
                this["BASECONFIG_FLAG"] = value;
            }
        }
        public class C_PRIVILEGE
        {
            public string ID;
            public string SYSTEM_NAME;
            public string MENU_ID;
            public string PRIVILEGE_NAME;
            public string PRIVILEGE_DESC;
            public DateTime EDIT_TIME;
            public string EDIT_EMP;
        }

    }

    public class PrivilegeEditModel
    {
        public string ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string PRIVILEGE_NAME { get; set; }
        public string PRIVILEGE_DESC { get; set; }
    }
}