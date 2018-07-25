using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESStation.Packing;

namespace MESStation.LogicObject
{
    public class LotNo
    {
        public string ID
        {
            get
            {
                return SLot.ID;
            }
        }
        public string LOT_NO
        {
            get
            {
                return SLot.LOT_NO;
            }
        }

        public string SKUNO
        {
            get
            {
                return SLot.SKUNO;
            }
        }

        public string AQL_TYPE
        {
            get
            {
                return SLot.AQL_TYPE;
            }
        }

        public double? LOT_QTY
        {
            get
            {
                return SLot.LOT_QTY;
            }
        }

        public double? REJECT_QTY
        {
            get
            {
                return SLot.REJECT_QTY;
            }
        }

        public double? SAMPLE_QTY
        {
            get
            {
                return SLot.SAMPLE_QTY;
            }
        }
        public double? PASS_QTY
        {
            get
            {
                return SLot.PASS_QTY;
            }
        }
        public double? FAIL_QTY
        {
            get
            {
                return SLot.FAIL_QTY;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return SLot.CLOSED_FLAG;
            }
        }

        public string LOT_STATUS_FLAG
        {
            get
            {
                return SLot.LOT_STATUS_FLAG;
            }
        }
        public string SAMPLE_STATION
        {
            get
            {
                return SLot.SAMPLE_STATION;
            }
        }
        public string LINE
        {
            get
            {
                return SLot.LINE;
            }
        }
        public string LotDetailID
        {
            get
            {
                return Slotdetail.ID;
            }
        }
        public string LotDetailLotID
        {
            get
            {
                return Slotdetail.LOT_ID;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return Slotdetail.WORKORDERNO;
            }
        }
        public string SAMPLING
        {
            get
            {
                return Slotdetail.SAMPLING;
            }
        }
        public string STATUS
        {
            get
            {
                return Slotdetail.STATUS;
            }
        }
        public string FAIL_CODE
        {
            get
            {
                return Slotdetail.FAIL_CODE;
            }
        }
        public string FAIL_LOCATION
        {
            get
            {
                return Slotdetail.FAIL_LOCATION;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return Slotdetail.DESCRIPTION;
            }
        }
        public string CARTON_NO
        {
            get
            {
                return Slotdetail.CARTON_NO;
            }
        }
        public string PALLET_NO
        {
            get
            {
                return Slotdetail.PALLET_NO;
            }
        }

        public LotNo()
        {
        }

        public LotNo(MESDataObject.DB_TYPE_ENUM _dbType)
        {
            DBType = _dbType;
        }

        Row_R_LOT_STATUS RLotNo;
        R_LOT_STATUS SLot;
        Row_R_LOT_DETAIL Rlotdetail;
        R_LOT_DETAIL Slotdetail;
        MESDataObject.DB_TYPE_ENUM DBType;
       

        /*Modify by LLF 2018-02-22
        public void Init(string StrLotNo, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {          
            string ColoumName = "lot_no";
            DBType = _DBType;
            T_R_LOT_STATUS TRWB = new T_R_LOT_STATUS(SFCDB, DBType);
            T_R_LOT_DETAIL TWC = new T_R_LOT_DETAIL(SFCDB, DBType);
            RLotNo = TRWB.GetByInput(StrLotNo, ColoumName, SFCDB);
            SLot = RLotNo.GetDataObject();
            Rlotdetail = TWC.GetByLotID(SLot.ID, SFCDB);
            Slotdetail = Rlotdetail.GetDataObject();
        }

        public void Init(string StrLotNo, MESDBHelper.OleExec SFCDB)
        {
            Init(StrLotNo, SFCDB, DBType);
        }*/

        public void Init(string StrLotNo, string StrSN, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            string ColoumName = "lot_no";
            DBType = _DBType;
            T_R_LOT_STATUS TRWB = new T_R_LOT_STATUS(SFCDB, DBType);
            T_R_LOT_DETAIL TWC = new T_R_LOT_DETAIL(SFCDB, DBType);
            RLotNo = TRWB.GetByInput(StrLotNo, ColoumName, SFCDB);
            SLot = RLotNo.GetDataObject();
            Rlotdetail = TWC.GetByLotID(SLot.ID, StrSN, SFCDB);
            Slotdetail = Rlotdetail.GetDataObject();
        }

        public void Init(string StrLotNo, string StrSN, MESDBHelper.OleExec SFCDB)
        {
            Init(StrLotNo, StrSN, SFCDB, DBType);
        }
        public string GetNewLotNo(string SeqName, MESDBHelper.OleExec SFCDB)
        {
            string StrLotNo = "";
            T_C_SEQNO T_C_Seqno = new T_C_SEQNO(SFCDB, DBType);
            StrLotNo=T_C_Seqno.GetLotno(SeqName, SFCDB);
            return StrLotNo;    
        }
        
        /// <summary>
        /// OBA工站創建LOT,返回LOT信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="packNo"></param>
        /// <param name="DB"></param>
        public R_LOT_STATUS CreateLotByPackno(User user, string packNo, OleExec DB)
        {
            T_R_PACKING tRPacking = new T_R_PACKING(DB, this.DBType);
            Row_R_PACKING rowRPacking = tRPacking.GetRPackingByPackNo(DB, packNo);
            PalletBase palletBase = new PalletBase(rowRPacking);

            T_C_SKU tCSku = new T_C_SKU(DB, this.DBType);
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(DB, this.DBType);
            T_C_AQLTYPE tCAqlType = new T_C_AQLTYPE(DB, this.DBType);
            T_R_LOT_PACK tRLotPack = new T_R_LOT_PACK(DB, this.DBType);

            List<C_AQLTYPE> cAqlTypeList = tCAqlType.GetAqlTypeBySkuno(rowRPacking.SKUNO, DB);
            Row_C_SKU rCSku = tCSku.GetSku(rowRPacking.SKUNO, DB, this.DBType);
            Row_R_LOT_STATUS rowRLotStatus = (Row_R_LOT_STATUS)tRLotStatus.NewRow();
            rowRLotStatus.ID = tRLotStatus.GetNewID(user.BU, DB, this.DBType);
            rowRLotStatus.LOT_NO = SNMaker.SNmaker.GetNextSN("OBALOT", DB);
            rowRLotStatus.SKUNO = rowRPacking.SKUNO;
            rowRLotStatus.AQL_TYPE = rCSku.AQLTYPE;
            rowRLotStatus.LOT_QTY = palletBase.GetSnCount(DB);
            rowRLotStatus.REJECT_QTY = cAqlTypeList.Where(t => t.LOT_QTY > rowRLotStatus.LOT_QTY).OrderBy(t => t.LOT_QTY).Take(1).ToList<C_AQLTYPE>()[0].REJECT_QTY;
            rowRLotStatus.SAMPLE_QTY = cAqlTypeList.Where(t => t.LOT_QTY > rowRLotStatus.LOT_QTY).OrderBy(t => t.LOT_QTY).Take(1).ToList<C_AQLTYPE>()[0].SAMPLE_QTY;
            rowRLotStatus.SAMPLE_QTY = rowRLotStatus.SAMPLE_QTY > rowRLotStatus.LOT_QTY ? rowRLotStatus.LOT_QTY : rowRLotStatus.SAMPLE_QTY;
            rowRLotStatus.PASS_QTY = 0;
            rowRLotStatus.FAIL_QTY = 0;
            rowRLotStatus.CLOSED_FLAG = "0";
            rowRLotStatus.LOT_STATUS_FLAG = "0";
            rowRLotStatus.SAMPLE_STATION = "OBA";
            rowRLotStatus.LINE = "";
            rowRLotStatus.EDIT_EMP = user.EMP_NO;
            rowRLotStatus.EDIT_TIME = tRPacking.GetDBDateTime(DB);

            Row_R_LOT_PACK rowRLotPack = (Row_R_LOT_PACK)tRLotPack.NewRow();
            rowRLotPack.ID = tRLotPack.GetNewID(user.BU, DB, this.DBType);
            rowRLotPack.LOTNO = rowRLotStatus.LOT_NO;
            rowRLotPack.PACKNO = packNo;
            rowRLotPack.EDIT_EMP = user.EMP_NO;
            rowRLotPack.EDIT_TIME = rowRLotStatus.EDIT_TIME;
            DB.ThrowSqlExeception = true;
            DB.ExecSQL(rowRLotStatus.GetInsertString(this.DBType));
            DB.ExecSQL(rowRLotPack.GetInsertString(this.DBType));
            DB.ThrowSqlExeception = false;
            return rowRLotStatus.GetDataObject();
        }

        /// <summary>
        /// OBA工站InLot,返回LOT信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="packNo"></param>
        /// <param name="DB"></param>
        public R_LOT_STATUS ObaInLotByPackno(User user, R_LOT_STATUS rLotStatus , string packNo, OleExec DB)
        {
            T_R_PACKING tRPacking = new T_R_PACKING(DB, this.DBType);
            Row_R_PACKING rowRPacking = tRPacking.GetRPackingByPackNo(DB, packNo);
            PalletBase palletBase = new PalletBase(rowRPacking);
            
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(DB, this.DBType);
            T_C_AQLTYPE tCAqlType = new T_C_AQLTYPE(DB, this.DBType);
            T_R_LOT_PACK tRLotPack = new T_R_LOT_PACK(DB, this.DBType);

            List<C_AQLTYPE> cAqlTypeList = tCAqlType.GetAqlTypeBySkuno(rowRPacking.SKUNO, DB);
            Row_R_LOT_STATUS rowRLotStatus =  (Row_R_LOT_STATUS)tRLotStatus.NewRow();
            rowRLotStatus.ID = rLotStatus.ID;
            rowRLotStatus.LOT_NO = rLotStatus.LOT_NO;
            rowRLotStatus.SKUNO = rLotStatus.SKUNO;
            rowRLotStatus.AQL_TYPE = rLotStatus.AQL_TYPE;
            rowRLotStatus.LOT_QTY = rLotStatus.LOT_QTY+ palletBase.GetSnCount(DB);
            rowRLotStatus.REJECT_QTY = cAqlTypeList.Where(t => t.LOT_QTY > rowRLotStatus.LOT_QTY).OrderBy(t => t.LOT_QTY).Take(1).ToList<C_AQLTYPE>()[0].REJECT_QTY;
            rowRLotStatus.SAMPLE_QTY = cAqlTypeList.Where(t => t.LOT_QTY > rowRLotStatus.LOT_QTY).OrderBy(t => t.LOT_QTY).Take(1).ToList<C_AQLTYPE>()[0].SAMPLE_QTY;
            rowRLotStatus.SAMPLE_QTY = rowRLotStatus.SAMPLE_QTY > rowRLotStatus.LOT_QTY ? rowRLotStatus.LOT_QTY : rowRLotStatus.SAMPLE_QTY;
            rowRLotStatus.PASS_QTY = rLotStatus.PASS_QTY;
            rowRLotStatus.FAIL_QTY = rLotStatus.FAIL_QTY;
            rowRLotStatus.CLOSED_FLAG = rLotStatus.CLOSED_FLAG;
            rowRLotStatus.LOT_STATUS_FLAG = rLotStatus.LOT_STATUS_FLAG;
            rowRLotStatus.SAMPLE_STATION = rLotStatus.SAMPLE_STATION;
            rowRLotStatus.LINE = rLotStatus.LINE;
            rowRLotStatus.EDIT_EMP = user.EMP_NO;
            rowRLotStatus.EDIT_TIME = tRPacking.GetDBDateTime(DB);

            Row_R_LOT_PACK rowRLotPack = (Row_R_LOT_PACK)tRLotPack.NewRow();
            rowRLotPack.ID = tRLotPack.GetNewID(user.BU, DB, this.DBType);
            rowRLotPack.LOTNO = rowRLotStatus.LOT_NO;
            rowRLotPack.PACKNO = packNo;
            rowRLotPack.EDIT_EMP = user.EMP_NO;
            rowRLotPack.EDIT_TIME = rowRLotStatus.EDIT_TIME;
            DB.ThrowSqlExeception = true;
            DB.ExecSQL(rowRLotStatus.GetUpdateString(this.DBType) + ";" + rowRLotPack.GetInsertString(this.DBType));
            DB.ThrowSqlExeception = false;
            return rowRLotStatus.GetDataObject();
        }
        

        public override string ToString()
        {
            return LOT_NO;
        }

    }
    
}
