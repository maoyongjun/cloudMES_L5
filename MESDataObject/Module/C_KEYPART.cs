using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KEYPART : DataObjectTable
    {
        public T_C_KEYPART(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KEYPART(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KEYPART);
            TableName = "C_KEYPART".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);



        }

        public List<C_KEYPART> GetKeypartListBySNAndStation(OleExec sfcdb, string sn, string station)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_C_KEYPART row_main = null;
            List<C_KEYPART> mains = new List<C_KEYPART>();
            string sql = $@" SELECT B.*
                            FROM R_WO_BASE A, C_KEYPART B, R_SN C
                            WHERE     A.KP_LIST_ID = B.KEYPART_ID
                                    AND A.WORKORDERNO = C.WORKORDERNO
                                    AND C.SN = '{sn}' --and STATION_NAME='{station}'
                        ORDER BY B.SEQ_NO ";

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_C_KEYPART)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public Row_C_KEYPART getC_MenubyKPID(string id, OleExec DB)
        {

            string strSql = $@" SELECT * FROM C_KEYPART where KEYPART_ID='{id}' ";
            DataSet res = DB.ExecSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_C_KEYPART ret = (Row_C_KEYPART)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public List<C_CATEGORY> GETCATEGORY(OleExec sfcdb)
        {
            DataTable dt = null;
            List<C_CATEGORY> mains = new List<C_CATEGORY>();
            string sql = $@" select * from c_CATEGORY ";

            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        C_CATEGORY CATEGORY = new C_CATEGORY();
                        CATEGORY.CATEGORY = dr["CATEGORY"].ToString();
                        CATEGORY.CATEGORY_NAME = dr["CATEGORY_NAME"].ToString();
                        mains.Add(CATEGORY);
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public List<C_KEYPART> GetKeypartListByWOAndStation(OleExec sfcdb, string WO, string station)
        {
            if (string.IsNullOrEmpty(WO))
            {
                return null;
            }
            DataTable dt = null;
            Row_C_KEYPART row_main = null;
            List<C_KEYPART> mains = new List<C_KEYPART>();
            string sql = $@" SELECT B.*
                            FROM R_WO_BASE A, C_KEYPART B
                            WHERE     A.KP_LIST_ID = B.KEYPART_ID
                                    AND A.WORKORDERNO = '{WO}' and STATION_NAME='{station}'
                        ORDER BY B.SEQ_NO ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_C_KEYPART)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public List<C_KEYPART> GetKeypartList(OleExec sfcdb, string skuno, string station)
        {
            DataTable dt = null;
            Row_C_KEYPART row_main = null;
            List<C_KEYPART> mains = new List<C_KEYPART>();
            string sql = $@" select * from  C_KEYPART  where 1=1 ";
            if (!string.IsNullOrEmpty(skuno))
            {
                sql += $@" and SKUNO='{skuno}' ";
            }
            if (!string.IsNullOrEmpty(station))
            {
                sql += $@" and station_name='{station}' ";
            }
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_C_KEYPART)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public List<C_KEYPART> GetKeypartListBySkuVersion(OleExec db, string skuno, string version)
        {
            DataTable dt = null;
            Row_C_KEYPART rowKeypart = null;
            List<C_KEYPART> keypartList = new List<C_KEYPART>();
            //modify by LLF 2018-04-01
            //string sql = $@"select * from c_keypart where skuno='{skuno}' and skuno_ver='{version}'";
            string sql = $@"select * from c_keypart where skuno='{skuno}'";
            try
            {
                dt = db.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    rowKeypart = (Row_C_KEYPART)this.NewRow();
                    rowKeypart.loadData(dr);
                    keypartList.Add(rowKeypart.GetDataObject());
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
            }
            return keypartList;
        }

        public List<C_KEYPART> GetKeyPartList(OleExec db, string kpListID)
        {
            string sql = null;
            List<C_KEYPART> lists = null;
            Row_C_KEYPART rckp = null;
            DataTable dt = null;
            if (!string.IsNullOrEmpty(kpListID))
            {
                sql = $@" select * from {this.TableName} where keypart_id = '{kpListID}' order by seq_no asc ";
                dt = db.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    lists = new List<C_KEYPART>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        rckp = (Row_C_KEYPART)this.NewRow();
                        rckp.loadData(dr);
                        lists.Add(rckp.GetDataObject());
                    }
                }

            }

            return lists;
        }

        public List<string> GetListBySkuno(OleExec db, string skuno)
        {
            //List<C_KEYPART> kps = new List<C_KEYPART>();
            List<string> kps = new List<string>();
            DataTable dt = null;
            Row_C_KEYPART row_kp = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select * from {TableName} where skuno='{skuno}' ";
                dt = db.ExecSelect(sql).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    //row_kp = (Row_C_KEYPART) NewRow();
                    //row_kp.loadData(dr);
                    //kps.Add(row_kp.GetDataObject());
                    kps.Add(dr["keypart_id"].ToString());
                }
                return kps;
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }
            
        }


    }
    public class Row_C_KEYPART : DataObjectBase
    {
        public Row_C_KEYPART(DataObjectInfo info) : base(info)
        {

        }
        public C_KEYPART GetDataObject()
        {
            C_KEYPART DataObject = new C_KEYPART();
            DataObject.ID = this.ID;
            DataObject.KEYPART_ID = this.KEYPART_ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.PART_NO = this.PART_NO;
            DataObject.PART_NO_VER = this.PART_NO_VER;
            DataObject.QTY = this.QTY;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.CATEGORY_NAME = this.CATEGORY_NAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKUNO_VER = this.SKUNO_VER;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string KEYPART_ID
        {
            get

            {
                return (string)this["KEYPART_ID"];
            }
            set
            {
                this["KEYPART_ID"] = value;
            }
        }
        public double? SEQ_NO
        {
            get

            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string PART_NO
        {
            get

            {
                return (string)this["PART_NO"];
            }
            set
            {
                this["PART_NO"] = value;
            }
        }
        public string PART_NO_VER
        {
            get

            {
                return (string)this["PART_NO_VER"];
            }
            set
            {
                this["PART_NO_VER"] = value;
            }
        }
        public double? QTY
        {
            get

            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string STATION_NAME
        {
            get

            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string CATEGORY
        {
            get

            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string CATEGORY_NAME
        {
            get

            {
                return (string)this["CATEGORY_NAME"];
            }
            set
            {
                this["CATEGORY_NAME"] = value;
            }
        }
        public string SKUNO
        {
            get

            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string SKUNO_VER
        {
            get

            {
                return (string)this["SKUNO_VER"];
            }
            set
            {
                this["SKUNO_VER"] = value;
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
    }
    public class C_KEYPART
    {
        public string ID;
        public string KEYPART_ID;
        public double? SEQ_NO;
        public string PART_NO;
        public string PART_NO_VER;
        public double? QTY;
        public string STATION_NAME;
        public string CATEGORY;
        public string CATEGORY_NAME;
        public string SKUNO;
        public string SKUNO_VER;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
    public class KEYPARTLIST
    {
        public string PART_NO;
        public string PART_NO_VER;
        public string QTY;
        public string CATEGORY;
        public string CATEGORY_NAME;
        public double SEQ;
    }

    public class C_CATEGORY
    {
        public string CATEGORY;
        public string CATEGORY_NAME;
    }
}