using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_LOT_STATUS : DataObjectTable
    {
        public T_R_LOT_STATUS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LOT_STATUS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LOT_STATUS);
            TableName = "R_LOT_STATUS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_R_LOT_STATUS GetByLotNo(string LotNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_LOT_STATUS row = (Row_R_LOT_STATUS)NewRow();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row.loadData(dt.Rows[0]);
                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public Row_R_LOT_STATUS GetByInput(string _InputData, string ColoumName, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS R = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                //Modify by LLF 2018-02-24
                //strsql = $@" select ID from r_lot_status where {ColoumName}='{_InputData.Replace("'", "''")}' and closed_flag='0'";
                strsql = $@" select ID from r_lot_status where {ColoumName}='{_InputData.Replace("'", "''")}' ";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { ColoumName+":" + _InputData });
                    //throw new MESReturnMessage(errMsg);
                    R = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public Row_R_LOT_STATUS GetLotBySku(string _InputData, string ColoumName, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS R = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@" select ID from r_lot_status where {ColoumName}='{_InputData.Replace("'", "''")}' and closed_flag='0'";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { ColoumName+":" + _InputData });
                    //throw new MESReturnMessage(errMsg);
                    R = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return R;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 通過SN 獲取它所在的LOT_NO
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_LOT_STATUS GetLotBySNForInLot(string _SN, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_lot_status a where a.closed_flag=0 and exists (select 1 from r_lot_detail b where b.sn = '{_SN.Replace("'", "''")}' and a.id = b.lot_id) order by a.id desc";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //Modify by LLF 2018-02-07
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + _SN });
                    //throw new MESReturnMessage(errMsg);
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 通過SN 獲取它所在的LOT_NO
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_LOT_STATUS GetLotBySN(string _SN, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_lot_status a where exists (select 1 from r_lot_detail b where b.sn = '{_SN.Replace("'", "''")}' and a.id = b.lot_id) order by a.id desc";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    //Modify by LLF 2018-02-07
                    //string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + _SN });
                    //throw new MESReturnMessage(errMsg);
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 通過SN獲取沒有COLSED的LOT信息
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_LOT_STATUS GetLotBySNNotCloesd(string sn, OleExec DB)
        {
            string strsql = "";
            string id = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select id from r_lot_status a where a.closed_flag='0' and exists (select 1 from r_lot_detail b 
                                where b.sn = '{sn.Replace("'", "''")}' and a.id = b.lot_id) order by a.id desc";
                id = DB.ExecSelectOneValue(strsql)?.ToString();
                if (id != null)
                {
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(id, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }


        public Row_R_LOT_STATUS GetSampleLotBySN(string _SN, OleExec DB)
        {
            string strsql = "";
            Row_R_LOT_STATUS Res = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select ID from r_lot_status a where exists (select 1 from r_lot_detail b where b.sn = '{_SN.Replace("'", "''")}' and a.id = b.lot_id and a.closed_flag='1' and a.lot_status_flag='0') order by a.id desc";
                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                if (ID != null)
                {
                    Res = (Row_R_LOT_STATUS)this.GetObjByID(ID, DB);
                }
                return Res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }


        /// <summary>
        /// By packNo 取LotInfo     
        /// </summary>
        /// <param name="packNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_LOT_STATUS> getSampleLotByPackNo(string packNo, OleExec DB)
        {
            List<R_LOT_STATUS> res = new List<R_LOT_STATUS>();
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string strsql = $@" select b.* from r_lot_pack a,r_lot_status b where a.lotno=b.lot_no and a.packno='{packNo}'  ";
                DataSet ds = DB.ExecSelect(strsql);
                foreach (DataRow VARIABLE in ds.Tables[0].Rows)
                {
                    Row_R_LOT_STATUS row = (Row_R_LOT_STATUS)this.NewRow();
                    row.loadData(VARIABLE);
                    res.Add(row.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return res;
        }

        /// <summary>
        /// 獲取數據庫時間
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DateTime GetDBDateTime(OleExec DB)
        {
            string strSql = "select sysdate from dual";
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = "select sysdate from dual";
            }
            else if (this.DBType == DB_TYPE_ENUM.SqlServer)
            {
                strSql = "select get_date() ";
            }
            else
            {
                throw new Exception(this.DBType.ToString() + " not Work");
            }
            return (DateTime)DB.ExecSelectOneValue(strSql);

        }

        /// <summary>
        /// FQC Lot 過站
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="LotNo"></param>
        /// <param name="PassOrFail"></param>
        /// <param name="EmpNo"></param>
        /// <param name="Station"></param>
        /// <param name="Line"></param>
        /// <param name="BU"></param>
        /// <param name="DB"></param>
        /// <param name="FailInfos"></param>
        public void LotPassStation(string SerialNo, string LotNo, string PassOrFail, string EmpNo, string Station, string DeviceName, string Line, string BU, OleExec DB, params string[] FailInfos)
        {
            bool PassedFlag = true;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_LOT_STATUS StatusRow = (Row_R_LOT_STATUS)NewRow();
            T_R_LOT_DETAIL DetailTable = new T_R_LOT_DETAIL(DB, this.DBType);
            Row_R_LOT_DETAIL DetailRow = (Row_R_LOT_DETAIL)DetailTable.NewRow();
            R_LOT_STATUS Status = null;
            R_LOT_DETAIL Detail = null;
            T_R_SN SnTable = new T_R_SN(DB, this.DBType);
            List<string> LotsSN = new List<string>();

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                PassedFlag = PassOrFail.ToUpper().Equals("PASS") ? true : false;
                //sql = $@"SELECT * FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}' AND SAMPLE_STATION='{Station}' AND LINE='{Line}'"; //判斷有沒有 LOT
                sql = $@"SELECT * FROM R_LOT_STATUS WHERE LOT_NO='{LotNo}' AND SAMPLE_STATION='{Station}'"; //判斷有沒有 LOT
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    StatusRow.loadData(dt.Rows[0]);
                    Status = StatusRow.GetDataObject();
                    sql = $@"SELECT A.* FROM R_LOT_DETAIL A,R_SN B WHERE LOT_ID='{StatusRow.ID}' AND B.SN='{SerialNo}' AND A.SN=B.SN"; //判斷Lot中有沒有這個SN並且沒有被抽檢過
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        DetailRow.loadData(dt.Rows[0]);
                        Detail = DetailRow.GetDataObject();
                        if (Detail.SAMPLING.Equals("1"))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000093", new string[] { SerialNo }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000094", new string[] { SerialNo, LotNo }));
                    }

                    if (Status.CLOSED_FLAG == "1") //Lot 關閉
                    {
                        if (PassedFlag)
                        {
                            //更新 R_LOT_DETAIL STATUS
                            Detail.STATUS = "1"; //1 表示抽檢通過
                            Detail.SAMPLING = "1";//1 表示被抽檢了
                            //更新 R_LOT_STATUS PASS_QTY
                            Status.PASS_QTY++;
                        }
                        else
                        {
                            //更新 R_LOT_DETAIL STATUS ,FAIL_CODE,FAIL_LOCATION,DESCRIPTION

                            Detail.STATUS = "0";//0 表示抽檢失敗
                            Detail.SAMPLING = "1";//1 表示被抽檢了
                            if (FailInfos != null && FailInfos.Length == 3) //記錄失敗原因
                            {
                                Detail.FAIL_CODE = FailInfos[0];
                                Detail.FAIL_LOCATION = FailInfos[1];
                                Detail.DESCRIPTION = FailInfos[2];
                            }


                            //更新 R_LOT_STATUS FAIL_QTY
                            Status.FAIL_QTY++;

                        }
                        if (Status.FAIL_QTY >= Status.REJECT_QTY && Status.FAIL_QTY!=0)
                        {
                            //更新 R_LOT_STATUS 關閉，NG，
                            //Status.CLOSED_FLAG = "1";// 1 表示關閉Lot
                            Status.LOT_STATUS_FLAG = "2";// 2 表示整個 Lot 不良
                            //更新 R_LOT_DETAIL 鎖定LOT 中所有
                            Detail.EDIT_EMP = EmpNo;
                            Detail.EDIT_TIME = GetDBDateTime(DB);
                            DetailRow.ConstructRow(Detail);
                            DB.ExecSQL(DetailRow.GetUpdateString(this.DBType));
                            //該批次鎖定--add by Eden 2018-05-04
                            sql = $@"update r_lot_detail set sampling='4' where lot_id='{Detail.LOT_ID}'";
                            DB.ExecSQL(sql);
                            //DetailTable.LockLotBySn(SerialNo, EmpNo, DB);
                        }
                        else
                        {
                            if (Status.PASS_QTY + Status.FAIL_QTY >= Status.SAMPLE_QTY)
                            {
                                //更新 R_LOT_STATUS 關閉，OK
                                //Status.CLOSED_FLAG = "1";
                                Status.LOT_STATUS_FLAG = "1"; // 1 表示整個 Lot 正常
                                //更新 R_LOT_DETAIL 鎖定FAIL 的，其他的正常過站
                                //sql = $@"SELECT * FROM R_LOT_DETAIL WHERE LOT_ID='{StatusRow.ID}' AND STATUS='0'";
                                sql = $@"SELECT * FROM R_LOT_DETAIL WHERE LOT_ID='{StatusRow.ID}' AND ((SAMPLING='1' AND STATUS='1') OR (SAMPLING='0'))";
                                dt = DB.ExecSelect(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        LotsSN.Add(dr["SN"].ToString());
                                    }
                                    SnTable.LotsPassStation(LotsSN, Line, Station, DeviceName, BU, PassOrFail, EmpNo, DB); // 過站
                                }
                                //記錄通過數 ,UPH
                                foreach (string SN in LotsSN)
                                {
                                    SnTable.RecordYieldRate(Detail.WORKORDERNO, 1, SN, "PASS", Line, Station, EmpNo, BU, DB);
                                    SnTable.RecordUPH(Detail.WORKORDERNO, 1, SN, "PASS", Line, Station, EmpNo, BU, DB);
                                }
                            }

                            Detail.EDIT_EMP = EmpNo;
                            Detail.EDIT_TIME = GetDBDateTime(DB);
                            DetailRow.ConstructRow(Detail);
                            DB.ExecSQL(DetailRow.GetUpdateString(this.DBType));

                        }

                        Status.EDIT_EMP = EmpNo;
                        Status.EDIT_TIME = GetDBDateTime(DB);
                        StatusRow.ConstructRow(Status);
                        DB.ExecSQL(StatusRow.GetUpdateString(this.DBType));
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000201", new string[] { LotNo }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000091", new string[] { LotNo }));
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public void InLotPassStation(string NewLotFlag,R_SN SNObj, string LotNo,string LotSatusID, string Station,string EmpNo,string AQL_TYPE, string Line, string BU, OleExec DB)
        {
            T_R_LOT_STATUS Table_R_Lot_Status = new T_R_LOT_STATUS(DB, DBType);
            Row_R_LOT_STATUS Row_R_Lot_Status = (Row_R_LOT_STATUS)NewRow();
            T_R_LOT_DETAIL Table_R_Lot_Detail = new T_R_LOT_DETAIL(DB, DBType);
            Row_R_LOT_DETAIL Row_R_Lot_Detail = (Row_R_LOT_DETAIL)Table_R_Lot_Detail.NewRow();

            T_C_AQLTYPE Table_C_AQLTYPE = new T_C_AQLTYPE(DB, DBType);

            string LotID = "";
            if (NewLotFlag == "1")
            {
                //Modify by LLF 2018-03-19,生成ID，需根據Table生成
                //LotID= GetNewID(BU, DB);
                LotID = Table_R_Lot_Status.GetNewID(BU, DB);
                Row_R_Lot_Status.ID = LotID;
                Row_R_Lot_Status.LOT_NO = LotNo;
                Row_R_Lot_Status.SKUNO = SNObj.SKUNO;
                Row_R_Lot_Status.AQL_TYPE = AQL_TYPE;
                Row_R_Lot_Status.LOT_QTY = 1;
                Row_R_Lot_Status.REJECT_QTY = 0;
                Row_R_Lot_Status.SAMPLE_QTY = 1;
                Row_R_Lot_Status.PASS_QTY = 0;
                Row_R_Lot_Status.FAIL_QTY = 0;
                Row_R_Lot_Status.CLOSED_FLAG = "0";
                Row_R_Lot_Status.LOT_STATUS_FLAG = "0";
                Row_R_Lot_Status.LINE = Line;
                Row_R_Lot_Status.SAMPLE_STATION = Station;
                Row_R_Lot_Status.EDIT_EMP = EmpNo;
                Row_R_Lot_Status.EDIT_TIME = GetDBDateTime(DB);
                DB.ExecSQL(Row_R_Lot_Status.GetInsertString(DBType));
            }
            else
            {
                LotID = LotSatusID;
                Row_R_Lot_Status = (Row_R_LOT_STATUS)Table_R_Lot_Status.GetObjByID(LotSatusID, DB);
                int LotQty = (int)Row_R_Lot_Status.LOT_QTY + 1;
                int SampleQty = Table_C_AQLTYPE.GetSampleQty(AQL_TYPE, LotQty, DB);
                Row_R_Lot_Status.SAMPLE_QTY = SampleQty;

                Row_R_Lot_Status.LOT_QTY += 1;
                DB.ExecSQL(Row_R_Lot_Status.GetUpdateString(DBType));
            }

            //Modify by LLF 2018-03-19,生成ID，需根據Table生成
            //Row_R_Lot_Detail.ID = GetNewID(BU, DB);
            Row_R_Lot_Detail.ID = Table_R_Lot_Detail.GetNewID(BU, DB);
            Row_R_Lot_Detail.LOT_ID = LotID;
            //Row_R_Lot_Detail.LOT_ID = LotID;
            Row_R_Lot_Detail.SN = SNObj.SN;
            Row_R_Lot_Detail.WORKORDERNO = SNObj.WORKORDERNO;
            Row_R_Lot_Detail.CREATE_DATE = GetDBDateTime(DB);
            Row_R_Lot_Detail.SAMPLING = "0";
            Row_R_Lot_Detail.STATUS = "0";
            Row_R_Lot_Detail.FAIL_CODE = "";
            Row_R_Lot_Detail.FAIL_LOCATION = "";
            Row_R_Lot_Detail.DESCRIPTION = "";
            Row_R_Lot_Detail.CARTON_NO = "";
            Row_R_Lot_Detail.PALLET_NO = "";
            Row_R_Lot_Detail.EDIT_EMP = EmpNo;
            Row_R_Lot_Detail.EDIT_TIME = GetDBDateTime(DB);
            DB.ExecSQL(Row_R_Lot_Detail.GetInsertString(DBType));
        }
    }
    public class Row_R_LOT_STATUS : DataObjectBase
    {
        public Row_R_LOT_STATUS(DataObjectInfo info) : base(info)
        {

        }
        public R_LOT_STATUS GetDataObject()
        {
            R_LOT_STATUS DataObject = new R_LOT_STATUS();
            DataObject.ID = this.ID;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.AQL_TYPE = this.AQL_TYPE;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.REJECT_QTY = this.REJECT_QTY;
            DataObject.SAMPLE_QTY = this.SAMPLE_QTY;
            DataObject.PASS_QTY = this.PASS_QTY;
            DataObject.FAIL_QTY = this.FAIL_QTY;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.LOT_STATUS_FLAG = this.LOT_STATUS_FLAG;
            DataObject.SAMPLE_STATION = this.SAMPLE_STATION;
            DataObject.LINE = this.LINE;
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
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
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
        public string AQL_TYPE
        {
            get
            {
                return (string)this["AQL_TYPE"];
            }
            set
            {
                this["AQL_TYPE"] = value;
            }
        }
        public double? LOT_QTY
        {
            get
            {
                return (double?)this["LOT_QTY"];
            }
            set
            {
                this["LOT_QTY"] = value;
            }
        }
        public double? REJECT_QTY
        {
            get
            {
                return (double?)this["REJECT_QTY"];
            }
            set
            {
                this["REJECT_QTY"] = value;
            }
        }
        public double? SAMPLE_QTY
        {
            get
            {
                return (double?)this["SAMPLE_QTY"];
            }
            set
            {
                this["SAMPLE_QTY"] = value;
            }
        }
        public double? PASS_QTY
        {
            get
            {
                return (double?)this["PASS_QTY"];
            }
            set
            {
                this["PASS_QTY"] = value;
            }
        }
        public double? FAIL_QTY
        {
            get
            {
                return (double?)this["FAIL_QTY"];
            }
            set
            {
                this["FAIL_QTY"] = value;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public string LOT_STATUS_FLAG
        {
            get
            {
                return (string)this["LOT_STATUS_FLAG"];
            }
            set
            {
                this["LOT_STATUS_FLAG"] = value;
            }
        }
        public string SAMPLE_STATION
        {
            get
            {
                return (string)this["SAMPLE_STATION"];
            }
            set
            {
                this["SAMPLE_STATION"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
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
    public class R_LOT_STATUS
    {
        public string ID;
        public string LOT_NO;
        public string SKUNO;
        public string AQL_TYPE;
        public double? LOT_QTY;
        public double? REJECT_QTY;
        public double? SAMPLE_QTY;
        public double? PASS_QTY;
        public double? FAIL_QTY;
        public string CLOSED_FLAG;
        public string LOT_STATUS_FLAG;
        public string SAMPLE_STATION;
        public string LINE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}