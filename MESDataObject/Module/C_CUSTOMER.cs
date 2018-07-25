using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_CUSTOMER : DataObjectTable
    {
        public T_C_CUSTOMER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_CUSTOMER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_CUSTOMER);
            TableName = "C_CUSTOMER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable GetCustomer(Dictionary<string, string> parameters, OleExec DB)
        {
            string sql = $@"select * from c_customer  where 1=1 ";
            string tempSql = "";
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> paras in parameters)
                {
                    if (paras.Value != "")
                    {
                        tempSql = tempSql + $@" and {paras.Key} = '{paras.Value}' ";
                    }
                }
            }
            sql = sql + tempSql;
            return DB.ExecSelect(sql).Tables[0];
        }

        public List<C_CUSTOMER> GetCustomerList(Dictionary<string, string> parameters, OleExec oleDB)
        {
            string sql = $@"select * from c_customer  where 1=1 ";
            string tempSql = "";
            DataTable dtCustomer = new DataTable();
            foreach (KeyValuePair<string, string> paras in parameters)
            {
                if (paras.Value != "")
                {
                    if (paras.Key.Equals("CUSTOMER_NAME"))
                    {
                        tempSql = tempSql + $@" and {paras.Key} like '%{paras.Value}%' ";
                    }
                    else
                    {
                        tempSql = tempSql + $@" and {paras.Key} = '{paras.Value}' ";
                    }
                }
            }
            sql = sql + tempSql;
            dtCustomer = oleDB.ExecSelect(sql).Tables[0];
            List<C_CUSTOMER> costomerList = new List<C_CUSTOMER>();
            Row_C_CUSTOMER customerRow;
            foreach (DataRow row in dtCustomer.Rows)
            {
                customerRow = (Row_C_CUSTOMER)this.NewRow();
                customerRow.loadData(row);
                costomerList.Add(customerRow.GetDataObject());
            }
            return costomerList;
        }
        public List<Dictionary<string, string>> GetCustomerDetail(Dictionary<string, string> parameters, OleExec oleDB)
        {
            string sql = $@"select * from c_customer  where 1=1 ";
            string tempSql = "";
            string sqlExpand = "";
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> detail;
            DataTable temp;
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> paras in parameters)
                {
                    if (paras.Value != "")
                    {
                        tempSql = tempSql + $@" and {paras.Key} = '{paras.Value}' ";
                    }
                }
            }
            sql = sql + tempSql;
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                detail = new Dictionary<string, string>();
                //detail.Add("ID", row["ID"].ToString());
                detail.Add("BU", row["BU"].ToString());
                detail.Add("CUSTOMER_NAME", row["CUSTOMER_NAME"].ToString());
                detail.Add("DESCRIPTION", row["DESCRIPTION"].ToString());
                sqlExpand = $@"select * from c_customer_ex where id='{row["ID"].ToString()}'";
                temp = oleDB.ExecSelect(sqlExpand).Tables[0];
                foreach (DataRow rowExpand in temp.Rows)
                {
                    detail.Add(rowExpand["NAME"].ToString(), rowExpand["VALUE"].ToString());
                }
                list.Add(detail);
            }
            return list;
        }
        public bool CustomerIsExist(OleExec oleDB, string strbu, string customerName)
        {
            string sql = $@"select * from c_customer  where bu='{strbu}' and  customer_name='{customerName}'";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CustomerIsExist(OleExec oleDB, string strID)
        {
            string sql = $@"select * from c_customer  where id='{strID}' ";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetCustomerID(OleExec oleDB, string strbu, string customerName)
        {
            string sql = $@"select id from c_customer  where bu='{strbu}' and  customer_name='{customerName}'";
            DataTable dt = oleDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count != 1)
            {
                throw new Exception("Can't match the Customer");
            }
            string id = dt.Rows[0]["id"].ToString();
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception();
            }
            return id;
            //if (string.IsNullOrEmpty(oleDB.ExecSqlReturn(sql)))
            //{
            //    throw new Exception(strbu + " " + customerName + "can't exist");
            //}
            //return oleDB.ExecSqlReturn(sql);
        }

        public string GetCustomerName(OleExec oleDB, string strID)
        {
            string sql = $@"select customer_name from c_customer  where id='{strID}' ";
            return oleDB.ExecSqlReturn(sql); 
        }
    }
    public class Row_C_CUSTOMER : DataObjectBase
    {
        public Row_C_CUSTOMER(DataObjectInfo info) : base(info)
        {

        }
        public C_CUSTOMER GetDataObject()
        {
            C_CUSTOMER DataObject = new C_CUSTOMER();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.CUSTOMER_NAME = this.CUSTOMER_NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string CUSTOMER_NAME
        {
            get
            {
                return (string)this["CUSTOMER_NAME"];
            }
            set
            {
                this["CUSTOMER_NAME"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
    }
    public class C_CUSTOMER
    {
        public string ID;
        public string BU;
        public string CUSTOMER_NAME;
        public string DESCRIPTION;

        public override string ToString()
        {
            return CUSTOMER_NAME == null ? "" : CUSTOMER_NAME;
        }
    }
}