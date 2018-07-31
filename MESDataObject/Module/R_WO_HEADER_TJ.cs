using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_HEADER_TJ : DataObjectTable
    {
        public T_R_WO_HEADER_TJ(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_HEADER_TJ(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_HEADER_TJ);
            TableName = "R_WO_HEADER_TJ".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_WO_HEADER_TJ : DataObjectBase
    {
        public Row_R_WO_HEADER_TJ(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_HEADER_TJ GetDataObject()
        {
            R_WO_HEADER_TJ DataObject = new R_WO_HEADER_TJ();
            DataObject.STLAL = this.STLAL;
            DataObject.PHAS2 = this.PHAS2;
            DataObject.IDAT2 = this.IDAT2;
            DataObject.ERNAM = this.ERNAM;
            DataObject.WEMNG = this.WEMNG;
            DataObject.FEVOR = this.FEVOR;
            DataObject.OBJNR = this.OBJNR;
            DataObject.MAUFNR = this.MAUFNR;
            DataObject.APRIO = this.APRIO;
            DataObject.TKNUM = this.TKNUM;
            DataObject.POSEX_E = this.POSEX_E;
            DataObject.BSTKD = this.BSTKD;
            DataObject.KDPOS = this.KDPOS;
            DataObject.ZTDLINE = this.ZTDLINE;
            DataObject.ZVORNR = this.ZVORNR;
            DataObject.ZAPLFL = this.ZAPLFL;
            DataObject.CY_SEQNR = this.CY_SEQNR;
            DataObject.TASKGROUP = this.TASKGROUP;
            DataObject.KBEASOLL = this.KBEASOLL;
            DataObject.RSNUM = this.RSNUM;
            DataObject.GSUZS = this.GSUZS;
            DataObject.AEZEIT = this.AEZEIT;
            DataObject.ERFZEIT = this.ERFZEIT;
            DataObject.ERDAT = this.ERDAT;
            DataObject.STATUS = this.STATUS;
            DataObject.GMEIN = this.GMEIN;
            DataObject.MAKTX = this.MAKTX;
            DataObject.MATKL = this.MATKL;
            DataObject.AENAM = this.AENAM;
            DataObject.AEDAT = this.AEDAT;
            DataObject.KDMAT = this.KDMAT;
            DataObject.KUNNR = this.KUNNR;
            DataObject.ARBPL = this.ARBPL;
            DataObject.VERID = this.VERID;
            DataObject.GAMNG = this.GAMNG;
            DataObject.DISPO = this.DISPO;
            DataObject.GSTRS = this.GSTRS;
            DataObject.KDAUF = this.KDAUF;
            DataObject.REVLV = this.REVLV;
            DataObject.MATNR = this.MATNR;
            DataObject.AUART = this.AUART;
            DataObject.WERKS = this.WERKS;
            DataObject.AUFNR = this.AUFNR;
            DataObject.ID = this.ID;
            DataObject.VGW03 = this.VGW03;
            DataObject.VDATU = this.VDATU;
            DataObject.STLAN = this.STLAN;
            return DataObject;
        }
        public string STLAL
        {
            get
            {
                return (string)this["STLAL"];
            }
            set
            {
                this["STLAL"] = value;
            }
        }
        public string PHAS2
        {
            get
            {
                return (string)this["PHAS2"];
            }
            set
            {
                this["PHAS2"] = value;
            }
        }
        public string IDAT2
        {
            get
            {
                return (string)this["IDAT2"];
            }
            set
            {
                this["IDAT2"] = value;
            }
        }
        public string ERNAM
        {
            get
            {
                return (string)this["ERNAM"];
            }
            set
            {
                this["ERNAM"] = value;
            }
        }
        public string WEMNG
        {
            get
            {
                return (string)this["WEMNG"];
            }
            set
            {
                this["WEMNG"] = value;
            }
        }
        public string FEVOR
        {
            get
            {
                return (string)this["FEVOR"];
            }
            set
            {
                this["FEVOR"] = value;
            }
        }
        public string OBJNR
        {
            get
            {
                return (string)this["OBJNR"];
            }
            set
            {
                this["OBJNR"] = value;
            }
        }
        public string MAUFNR
        {
            get
            {
                return (string)this["MAUFNR"];
            }
            set
            {
                this["MAUFNR"] = value;
            }
        }
        public string APRIO
        {
            get
            {
                return (string)this["APRIO"];
            }
            set
            {
                this["APRIO"] = value;
            }
        }
        public string TKNUM
        {
            get
            {
                return (string)this["TKNUM"];
            }
            set
            {
                this["TKNUM"] = value;
            }
        }
        public string POSEX_E
        {
            get
            {
                return (string)this["POSEX_E"];
            }
            set
            {
                this["POSEX_E"] = value;
            }
        }
        public string BSTKD
        {
            get
            {
                return (string)this["BSTKD"];
            }
            set
            {
                this["BSTKD"] = value;
            }
        }
        public string KDPOS
        {
            get
            {
                return (string)this["KDPOS"];
            }
            set
            {
                this["KDPOS"] = value;
            }
        }
        public string ZTDLINE
        {
            get
            {
                return (string)this["ZTDLINE"];
            }
            set
            {
                this["ZTDLINE"] = value;
            }
        }
        public string ZVORNR
        {
            get
            {
                return (string)this["ZVORNR"];
            }
            set
            {
                this["ZVORNR"] = value;
            }
        }
        public string ZAPLFL
        {
            get
            {
                return (string)this["ZAPLFL"];
            }
            set
            {
                this["ZAPLFL"] = value;
            }
        }
        public string CY_SEQNR
        {
            get
            {
                return (string)this["CY_SEQNR"];
            }
            set
            {
                this["CY_SEQNR"] = value;
            }
        }
        public string TASKGROUP
        {
            get
            {
                return (string)this["TASKGROUP"];
            }
            set
            {
                this["TASKGROUP"] = value;
            }
        }
        public string KBEASOLL
        {
            get
            {
                return (string)this["KBEASOLL"];
            }
            set
            {
                this["KBEASOLL"] = value;
            }
        }
        public string RSNUM
        {
            get
            {
                return (string)this["RSNUM"];
            }
            set
            {
                this["RSNUM"] = value;
            }
        }
        public string GSUZS
        {
            get
            {
                return (string)this["GSUZS"];
            }
            set
            {
                this["GSUZS"] = value;
            }
        }
        public string AEZEIT
        {
            get
            {
                return (string)this["AEZEIT"];
            }
            set
            {
                this["AEZEIT"] = value;
            }
        }
        public string ERFZEIT
        {
            get
            {
                return (string)this["ERFZEIT"];
            }
            set
            {
                this["ERFZEIT"] = value;
            }
        }
        public string ERDAT
        {
            get
            {
                return (string)this["ERDAT"];
            }
            set
            {
                this["ERDAT"] = value;
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
        public string GMEIN
        {
            get
            {
                return (string)this["GMEIN"];
            }
            set
            {
                this["GMEIN"] = value;
            }
        }
        public string MAKTX
        {
            get
            {
                return (string)this["MAKTX"];
            }
            set
            {
                this["MAKTX"] = value;
            }
        }
        public string MATKL
        {
            get
            {
                return (string)this["MATKL"];
            }
            set
            {
                this["MATKL"] = value;
            }
        }
        public string AENAM
        {
            get
            {
                return (string)this["AENAM"];
            }
            set
            {
                this["AENAM"] = value;
            }
        }
        public string AEDAT
        {
            get
            {
                return (string)this["AEDAT"];
            }
            set
            {
                this["AEDAT"] = value;
            }
        }
        public string KDMAT
        {
            get
            {
                return (string)this["KDMAT"];
            }
            set
            {
                this["KDMAT"] = value;
            }
        }
        public string KUNNR
        {
            get
            {
                return (string)this["KUNNR"];
            }
            set
            {
                this["KUNNR"] = value;
            }
        }
        public string ARBPL
        {
            get
            {
                return (string)this["ARBPL"];
            }
            set
            {
                this["ARBPL"] = value;
            }
        }
        public string VERID
        {
            get
            {
                return (string)this["VERID"];
            }
            set
            {
                this["VERID"] = value;
            }
        }
        public string GAMNG
        {
            get
            {
                return (string)this["GAMNG"];
            }
            set
            {
                this["GAMNG"] = value;
            }
        }
        public string DISPO
        {
            get
            {
                return (string)this["DISPO"];
            }
            set
            {
                this["DISPO"] = value;
            }
        }
        public string GSTRS
        {
            get
            {
                return (string)this["GSTRS"];
            }
            set
            {
                this["GSTRS"] = value;
            }
        }
        public string KDAUF
        {
            get
            {
                return (string)this["KDAUF"];
            }
            set
            {
                this["KDAUF"] = value;
            }
        }
        public string REVLV
        {
            get
            {
                return (string)this["REVLV"];
            }
            set
            {
                this["REVLV"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string AUART
        {
            get
            {
                return (string)this["AUART"];
            }
            set
            {
                this["AUART"] = value;
            }
        }
        public string WERKS
        {
            get
            {
                return (string)this["WERKS"];
            }
            set
            {
                this["WERKS"] = value;
            }
        }
        public string AUFNR
        {
            get
            {
                return (string)this["AUFNR"];
            }
            set
            {
                this["AUFNR"] = value;
            }
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
        public string VGW03
        {
            get
            {
                return (string)this["VGW03"];
            }
            set
            {
                this["VGW03"] = value;
            }
        }
        public string VDATU
        {
            get
            {
                return (string)this["VDATU"];
            }
            set
            {
                this["VDATU"] = value;
            }
        }
        public string STLAN
        {
            get
            {
                return (string)this["STLAN"];
            }
            set
            {
                this["STLAN"] = value;
            }
        }
    }
    public class R_WO_HEADER_TJ
    {
        public string STLAL;
        public string PHAS2;
        public string IDAT2;
        public string ERNAM;
        public string WEMNG;
        public string FEVOR;
        public string OBJNR;
        public string MAUFNR;
        public string APRIO;
        public string TKNUM;
        public string POSEX_E;
        public string BSTKD;
        public string KDPOS;
        public string ZTDLINE;
        public string ZVORNR;
        public string ZAPLFL;
        public string CY_SEQNR;
        public string TASKGROUP;
        public string KBEASOLL;
        public string RSNUM;
        public string GSUZS;
        public string AEZEIT;
        public string ERFZEIT;
        public string ERDAT;
        public string STATUS;
        public string GMEIN;
        public string MAKTX;
        public string MATKL;
        public string AENAM;
        public string AEDAT;
        public string KDMAT;
        public string KUNNR;
        public string ARBPL;
        public string VERID;
        public string GAMNG;
        public string DISPO;
        public string GSTRS;
        public string KDAUF;
        public string REVLV;
        public string MATNR;
        public string AUART;
        public string WERKS;
        public string AUFNR;
        public string ID;
        public string VGW03;
        public string VDATU;
        public string STLAN;
    }
}