using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;

namespace MESDataObject.Module
{
    public class T_C_ROLE : DataObjectTable
    {
        public T_C_ROLE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ROLE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROLE);
            TableName = "C_ROLE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public DataTable getC_Rolebyrolename(string Role_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql;
            if (Role_Name=="")
            {
                strSql = $@"select * from c_role  ";
            }
            else
            {
                strSql = $@"select * from c_role where Role_Name like '%{Role_Name}%' ";
            }
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;
        }

        public Row_C_ROLE SELECTC_Rolebyrolename(string Role_Name, OleExec DB, DB_TYPE_ENUM DBType)
        {


            string strSql;
            if (Role_Name == "")
            {
                strSql =  $@"select * from c_role  ";
                
            }
            else
            {
                strSql = $@"select * from c_role where Role_Name  like '%{Role_Name}%' ";
            }

            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_ROLE ret = (Row_C_ROLE)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<c_role1> Getrolelist(string ROLE_NAME, string EmpLevel,string DptName, OleExec DB)
        {                       
            string sql = string.Empty;
            string Strsql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role1> rolelist = new List<c_role1>();            

            if (ROLE_NAME.Length==0)
            {
                if (EmpLevel == "9")///9级用户可以拿到整个表的角色
                {
                    sql = $@"SELECT *FROM C_ROLE  ";
                }

                if (EmpLevel == "1")///1级用户只可以拿到自己部门相关的角色
                {
                    sql = $@"SELECT *FROM C_ROLE WHERE ROLE_TYPE LIKE '%{DptName}%' ";
                }
            }
            else
            {
                sql = $@"SELECT *FROM C_ROLE WHERE ROLE_NAME  like '%{ROLE_NAME}%'";
            }  
            if   (EmpLevel == "1"|| EmpLevel == "9")
            {
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow item in dt.Rows)
                {
                    rolelist.Add(new c_role1
                    {
                        ID = item["ID"].ToString(),
                        ROLE_TYPE = item["ROLE_TYPE"].ToString(),
                        ROLE_NAME = item["ROLE_NAME"].ToString(),
                        ROLE_DESC = item["ROLE_DESC"].ToString(),
                        EDIT_EMP = item["EDIT_EMP"].ToString()
                    });
                }
            }          
            return rolelist;
        }

        /// <summary>
        /// /   两个等级用户都只能管理自己拥有的角色，不拥有的角色是不能管理的
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="EmpLevel"></param>
        /// <param name="DptName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<c_role1> GetUserRolelist(string UserID,bool LoginTrue, string EmpLevel, string DptName, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<c_role1> rolelist = new List<c_role1>();
            if (EmpLevel == "9")///9级用户可以拿到整个表该管理员拥有的角色
            {
                if (LoginTrue)
                {
                    sql = $@" SELECT * FROM C_ROLE ";
                }
                else
                sql = $@" SELECT B.* FROM C_USER_ROLE A,C_ROLE B WHERE A.ROLE_ID=B.ID AND A.USER_ID='{UserID}'";
            }

            if (EmpLevel == "1")///1级用户只可以拿到自己拥有的角色         
            {
                sql = $@" SELECT B.* FROM C_USER_ROLE A,C_ROLE B WHERE A.ROLE_ID=B.ID AND B.ROLE_TYPE='{DptName}' AND A.USER_ID='{UserID}'";
            }
            if (EmpLevel == "1" || EmpLevel == "9")
            {
                dt = DB.ExecSelect(sql).Tables[0];
                foreach (DataRow item in dt.Rows)
                {
                    rolelist.Add(new c_role1
                    {
                        ID = item["ID"].ToString(),
                        ROLE_NAME = item["ROLE_NAME"].ToString(),
                        ROLE_DESC = item["ROLE_DESC"].ToString(),
                        EDIT_EMP = item["ROLE_TYPE"].ToString()
                    });
                }
            }
            return rolelist;
        }


        public List<c_role_byempl> ManageRoleByUser(List<get_c_roleid> ROLE_ID, string DPT_NAME, string BU_NAME, string FACTORY, string EMP_LEVEL, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string allroleid = "";
            string strsql = "1=1 AND";
            List<c_role_byempl> rolelist = new List<c_role_byempl>();
            if (ROLE_ID.Count!= 0)
            {               
                foreach (get_c_roleid item in ROLE_ID)
                {
                    allroleid += "'" + item.ROLE_ID + "',";
                }

                allroleid = allroleid.TrimEnd(',');
                strsql = " ID NOT IN (" + allroleid + ")" +"AND";
            }
           
            
            if (EMP_LEVEL == "9")
            {
                //9级账户能管理相同厂别和BU下的所有角色
            //    sql = $@" select *from C_ROLE HR  WHERE {strsql}  EXISTS(SELECT 1 FROM C_USER CR WHERE CR.DPT_NAME=HR.ROLE_TYPE AND CR.BU_NAME='{BU_NAME}' AND CR.FACTORY ='{FACTORY}')   ";
                sql = $@" select *from C_ROLE HR  WHERE {strsql}  EXISTS(SELECT 1 FROM C_USER CR WHERE  CR.BU_NAME='{BU_NAME}' AND CR.FACTORY ='{FACTORY}')   ";

            }
            else
            {
                sql = $@" select *from C_ROLE HR  WHERE {strsql} EXISTS(SELECT 1 FROM C_USER CR WHERE CR.DPT_NAME=HR.ROLE_TYPE AND CR.DPT_NAME='{DPT_NAME}' AND CR.BU_NAME='{BU_NAME}' AND CR.FACTORY ='{FACTORY}') ";
            }
            dt = DB.ExecSelect(sql).Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                rolelist.Add(new c_role_byempl
                {
                    ID = item["ID"].ToString(),
                    ROLE_NAME = item["ROLE_NAME"].ToString(),
                    ROLE_DESC = item["ROLE_DESC"].ToString(),
                    DPT_NAME=item["ROLE_TYPE"].ToString(),
                    FACTORY = FACTORY,
                    BU_NAME= BU_NAME
                });
            }

            return rolelist;
        }

        public bool CheckRole(string ROLE_ID, OleExec DB)
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

        public bool CheckRoleData(string RoleName, string Role_type, string EMP_LEVEL, string DPT_NAME, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();

            //     if ((EMP_LEVEL == "1" && DPT_NAME == Role_type) || (EMP_LEVEL == "9"))
            if ((EMP_LEVEL == "1" ) || (EMP_LEVEL == "9"))
            {
                if (RoleName.Length != 0)
                {
                    sql = $@"SELECT * FROM c_role where role_name='{RoleName}'";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        res = true;
                    }
                }
            }
            return res;

        }
    }
    public class c_role1
    {
        public c_role1()
        {

        }
        public string ID { get; set; }
        public string ROLE_TYPE { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string EDIT_EMP { get; set; }


    }

    public class c_role_byempl
    {
        public string ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string FACTORY { get; set; }
        public string BU_NAME { get; set; }
        public string DPT_NAME { get; set; }
    }
    public class Row_C_ROLE : DataObjectBase
    {
        public Row_C_ROLE(DataObjectInfo info) : base(info)
        {

        }
        public C_ROLE GetDataObject()
        {
            C_ROLE DataObject = new C_ROLE();
            DataObject.ID = this.ID;
            DataObject.SYSTEM_NAME = this.SYSTEM_NAME;
            DataObject.ROLE_NAME = this.ROLE_NAME;
            DataObject.ROLE_DESC = this.ROLE_DESC;
            DataObject.ROLE_TYPE = this.ROLE_DESC;
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
        public string ROLE_NAME
        {
            get
            {
                return (string)this["ROLE_NAME"];
            }
            set
            {
                this["ROLE_NAME"] = value;
            }
        }
        public string ROLE_DESC
        {
            get
            {
                return (string)this["ROLE_DESC"];
            }
            set
            {
                this["ROLE_DESC"] = value;
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

        public string ROLE_TYPE
        {
            get
            {
                return (string)this["ROLE_TYPE"];
            }
            set
            {
                this["ROLE_TYPE"] = value;
            }
        }
        public class C_ROLE
        {
            public string ID;
            public string SYSTEM_NAME;
            public string ROLE_NAME;
            public string ROLE_DESC;
            public DateTime? EDIT_TIME;
            public string EDIT_EMP;
            public string ROLE_TYPE;
        }
    }
}