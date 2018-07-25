using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_Label : DataObjectTable
    {
        public T_R_Label(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_Label(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_Label);
            TableName = "R_Label".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_R_Label GetLabelConfigByLabelName(string LabName, OleExec DB)
        {
            
            string strSql = $@"select * from R_Label r where r.labelname = '{LabName}'";
            DataSet res = DB.RunSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_Label A = (Row_R_Label)NewRow();
                A.loadData(res.Tables[0].Rows[0]);
                return A;
            }
            return null;
        }

        public List< R_Label> GetLabelList(OleExec DB)
        {
            List<R_Label> ret = new List<R_Label>();
            string strSql = $@"select * from R_Label";
            DataSet res = DB.RunSelect(strSql);
            for (int i=0;i< res.Tables[0].Rows.Count;i++)
            {
                Row_R_Label A = (Row_R_Label)NewRow();
                A.loadData(res.Tables[0].Rows[i]);
                ret.Add((R_Label)A.GetDataObject());
            }
            return ret;
        }

    }
    public class Row_R_Label : DataObjectBase
    {
        public Row_R_Label(DataObjectInfo info) : base(info)
        {

        }
        public R_Label GetDataObject()
        {
            R_Label DataObject = new R_Label();
            DataObject.ID = this.ID;
            DataObject.R_FILE_NAME = this.R_FILE_NAME;
            DataObject.LABELNAME = this.LABELNAME;
            DataObject.PRINTTYPE = this.PRINTTYPE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.ARRYLENGTH = this.ARRYLENGTH;
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
        public string R_FILE_NAME
        {
            get
            {
                return (string)this["R_FILE_NAME"];
            }
            set
            {
                this["R_FILE_NAME"] = value;
            }
        }
        public string LABELNAME
        {
            get
            {
                return (string)this["LABELNAME"];
            }
            set
            {
                this["LABELNAME"] = value;
            }
        }
        public string PRINTTYPE
        {
            get
            {
                return (string)this["PRINTTYPE"];
            }
            set
            {
                this["PRINTTYPE"] = value;
            }
        }

        public double? ARRYLENGTH
        {
            get
            {
                return (double?)this["ARRYLENGTH"];
            }
            set
            {
                this["ARRYLENGTH"] = value;
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
    }
    public class R_Label
    {
        public string ID;
        public string R_FILE_NAME;
        public string LABELNAME;
        public string PRINTTYPE;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
        public double? ARRYLENGTH;
    }
}