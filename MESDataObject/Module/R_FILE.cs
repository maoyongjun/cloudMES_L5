using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_FILE : DataObjectTable
    {
        public T_R_FILE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FILE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FILE);
            TableName = "R_FILE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_FILE> GetFileList(string UseType, OleExec DB)
        {
            List<R_FILE> ret = new List<R_FILE>();
            string strSql = $@"select r.id, r.name,r.filename,r.md5,r.usetype,r.valid,r.state,r.edit_time,r.edit_emp from r_file r where r.usetype ='{UseType}' and r.valid = 1";
            DataSet res = DB.RunSelect(strSql);

            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_FILE RRF = (Row_R_FILE)NewRow();
                RRF.loadData(res.Tables[0].Rows[i]);
                ret.Add(RRF.GetDataObject());
            }
            return ret;
        }

        public List<R_FILE> GetFileHisList(string Name , string UseType, OleExec DB)
        {
            List<R_FILE> ret = new List<R_FILE>();
            string strSql = $@"select r.id, r.name,r.filename,r.md5,r.usetype,r.valid,r.state,r.edit_time,r.edit_emp 
from r_file r where r.usetype ='{UseType}' and r.name='{Name}' order by edit_time DESC ";
            DataSet res = DB.RunSelect(strSql);

            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                Row_R_FILE RRF = (Row_R_FILE)NewRow();
                RRF.loadData(res.Tables[0].Rows[i]);
                ret.Add(RRF.GetDataObject());
            }
            return ret;
        }

        public R_FILE GetFileByName(string Name, string UseType, OleExec DB)
        {
            string strSql = $@"select r.id, r.name,r.filename,r.md5,r.usetype,r.valid,r.state,r.edit_time,r.edit_emp,blob_file from r_file r where r.name='{Name}' and r.usetype ='{UseType}' and r.valid = 1";
            DataSet ret = DB.RunSelect(strSql);
            
            Row_R_FILE r = (Row_R_FILE)this.NewRow();
            if (ret.Tables[0].Rows.Count > 0)
            {
                r.loadData(ret.Tables[0].Rows[0]);
            }

            //if (r.NAME != null)
            //{
            //    strSql = $@"select ID , clob_file , blob_file from r_file r where r.name='{Name}' and r.usetype ='{UseType}' and r.valid = 1";
            //    //System.Data.OleDb.OleDbParameter para = new System.Data.OleDb.OleDbParameter(":FILE", "");
            //    //para.Direction = ParameterDirection.Output;
            //    //para.Size = 999999999;
            //    ////para.DbType = DbType.Object;
            //    //DB.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[] { para });
            //    System.Data.OleDb.OleDbDataReader reader = DB.RunDataReader(strSql);
            //    if (reader.Read())
            //    {
            //        System.IO.TextReader TR = reader.GetTextReader(1);
            //        r.CLOB_FILE = TR.ReadToEnd();
                    
            //    }
            //    reader.Close();
            //}
            return r.GetDataObject();
            

        }

        public void SetFileDisableByName(string Name,string UseType, OleExec DB)
        {
            string strSql = $@"update R_FILE r set r.valid = 0 where r.name='{Name}' and r.usetype ='{UseType}' ";
            DB.ExecSQL(strSql);
        }
    }
    public class Row_R_FILE : DataObjectBase
    {
        public Row_R_FILE(DataObjectInfo info) : base(info)
        {

        }
        public R_FILE GetDataObject()
        {
            R_FILE DataObject = new R_FILE();
            DataObject.ID = this.ID;
            DataObject.NAME = this.NAME;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MD5 = this.MD5;
            DataObject.USETYPE = this.USETYPE;
            DataObject.VALID = this.VALID;
            DataObject.STATE = this.STATE;
            DataObject.CLOB_FILE = this.CLOB_FILE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            byte[] sb = (byte[])this["BLOB_FILE"];
            if (sb != null)
            {
                DataObject.BLOB_FILE = Convert.ToBase64String(sb);
            }
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
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public string MD5
        {
            get
            {
                return (string)this["MD5"];
            }
            set
            {
                this["MD5"] = value;
            }
        }
        public string USETYPE
        {
            get
            {
                return (string)this["USETYPE"];
            }
            set
            {
                this["USETYPE"] = value;
            }
        }
        public double? VALID
        {
            get
            {
                return (double?)this["VALID"];
            }
            set
            {
                this["VALID"] = value;
            }
        }
        public string STATE
        {
            get
            {
                return (string)this["STATE"];
            }
            set
            {
                this["STATE"] = value;
            }
        }
        public string CLOB_FILE
        {
            get
            {
                return (string)this["CLOB_FILE"];
            }
            set
            {
                this["CLOB_FILE"] = value;
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
        public string  BLOB_FILE
        {
            get
            {
                return (string)this["BLOB_FILE"];
            }
            set
            {
                this["BLOB_FILE"] = value;
            }
        }
    }
    public class R_FILE
    {
        public string ID;
        public string NAME;
        public string FILENAME;
        public string MD5;
        public string USETYPE;
        public double? VALID;
        public string STATE;
        public string CLOB_FILE;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
        public string BLOB_FILE;
    }
}