using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_LOT_DETAIL : DataObjectTable
    {
        public T_R_LOT_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LOT_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LOT_DETAIL);
            TableName = "R_LOT_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public void LockLotBySn(string SerialNo, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"UPDATE R_LOT_DETAIL SET STATUS='1',EDIT_EMP='{EmpNo}',EDIT_TIME=SYSDATE 
                        WHERE LOT_ID IN (SELECT LOT_ID FROM R_LOT_DETAIL WHERE SN='{SerialNo}')";
                DB.ExecSQL(sql);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        public void UnLockLotBySnOrLotNo(string InputData, string EmpNo, OleExec DB)
        {
            string sql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@" UPDATE R_LOT_DETAIL SET sampling='1',EDIT_EMP='{EmpNo}',EDIT_TIME=SYSDATE 
                        WHERE (LOT_ID IN (SELECT ID FROM R_LOT_STATUS WHERE LOT_NO='{InputData}') OR SN ='{InputData}') and sampling='4'";
                DB.ExecSQL(sql);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

        }

        public Row_R_LOT_DETAIL GetByLotID(string _LotID,string StrSN,OleExec DB) //add by StrSN by LLF 2018-02-22
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                //modify by LLF 2018-02-22
                //strsql = $@" select ID from r_lot_detail where lot_id='{_LotID.Replace("'", "''")}' and rownum=1";
                if (string.IsNullOrEmpty(StrSN))
                {
                    strsql = $@" select ID from r_lot_detail where lot_id='{_LotID.Replace("'", "''")}' and rownum=1 ";
                }
                else
                {
                    strsql = $@" select ID from r_lot_detail where lot_id='{_LotID.Replace("'", "''")}' and SN='{StrSN}' ";
                }

                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID == null)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "LotID:" + _LotID });
                    throw new MESReturnMessage(errMsg);
                }
                Row_R_LOT_DETAIL Res = (Row_R_LOT_DETAIL)this.GetObjByID(ID, DB);
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// CheckLotSnLock:lock=>true
        /// </summary>
        /// <param name="_LotID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckLotNoDetailStatus(string _LotID, OleExec DB)
        {
            bool StrRet = false;
            string strsql = "";
            DataTable dt;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select * from r_lot_detail where lot_id='{_LotID.Replace("'", "''")}' and sampling='4'";
                dt = DB.ExecSelect(strsql).Tables[0];
                if (dt.Rows.Count!=0)
                {
                    StrRet = true;
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return StrRet;
        }

        public bool CheckLotDetailSNStatus(string LotID,string Station, OleExec DB)
        {
            bool StrRet = false;
            string strsql = "";
            DataTable dt;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select count(1) from r_sn where sn in (select sn from r_lot_detail where lot_id='{LotID.Replace("'", "''")}') and next_station<>'{Station}'  AND VALID_FLAG='1'";
                //dt = DB.ExecSelect(strsql).Tables[0];
                int count = Int32.Parse(DB.ExecSelectOneValue(strsql).ToString());
                if (count != 0)
                {
                    StrRet = true;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return StrRet;
        }

        public List<R_LOT_DETAIL> GetLotDetailByLotNo(string lotNo,OleExec DB)
        {
            List<R_LOT_DETAIL> res = new List<R_LOT_DETAIL>();
            string strSql = $@" select a.* from r_lot_detail a ,r_lot_status b where a.lot_id=b.id and b.lot_no='{lotNo}' ";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            {
                Row_R_LOT_DETAIL r = (Row_R_LOT_DETAIL)this.NewRow();
                r.loadData(VARIABLE);
                res.Add(r.GetDataObject());
            }
            return res;
        }
    }
    public class Row_R_LOT_DETAIL : DataObjectBase
    {
        public Row_R_LOT_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_LOT_DETAIL GetDataObject()
        {
            R_LOT_DETAIL DataObject = new R_LOT_DETAIL();
            DataObject.ID = this.ID;
            DataObject.LOT_ID = this.LOT_ID;
            DataObject.SN = this.SN;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.CREATE_DATE = this.CREATE_DATE;
            DataObject.SAMPLING = this.SAMPLING;
            DataObject.STATUS = this.STATUS;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.FAIL_LOCATION = this.FAIL_LOCATION;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CARTON_NO = this.CARTON_NO;
            DataObject.PALLET_NO = this.PALLET_NO;
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
        public string LOT_ID
        {
            get
            {
                return (string)this["LOT_ID"];
            }
            set
            {
                this["LOT_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public DateTime? CREATE_DATE
        {
            get
            {
                return (DateTime?)this["CREATE_DATE"];
            }
            set
            {
                this["CREATE_DATE"] = value;
            }
        }
        public string SAMPLING
        {
            get
            {
                return (string)this["SAMPLING"];
            }
            set
            {
                this["SAMPLING"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string FAIL_CODE
        {
            get
            {
                return (string)this["FAIL_CODE"];
            }
            set
            {
                this["FAIL_CODE"] = value;
            }
        }
        public string FAIL_LOCATION
        {
            get
            {
                return (string)this["FAIL_LOCATION"];
            }
            set
            {
                this["FAIL_LOCATION"] = value;
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
        public string CARTON_NO
        {
            get
            {
                return (string)this["CARTON_NO"];
            }
            set
            {
                this["CARTON_NO"] = value;
            }
        }
        public string PALLET_NO
        {
            get
            {
                return (string)this["PALLET_NO"];
            }
            set
            {
                this["PALLET_NO"] = value;
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
    public class R_LOT_DETAIL
    {
        public string ID;
        public string LOT_ID;
        public string SN;
        public string WORKORDERNO;
        public DateTime? CREATE_DATE;
        public string SAMPLING;
        public string STATUS;
        public string FAIL_CODE;
        public string FAIL_LOCATION;
        public string DESCRIPTION;
        public string CARTON_NO;
        public string PALLET_NO;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}