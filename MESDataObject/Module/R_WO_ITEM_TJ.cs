using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_ITEM_TJ : DataObjectTable
    {
        public T_R_WO_ITEM_TJ(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_ITEM_TJ(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_ITEM_TJ);
            TableName = "R_WO_ITEM_TJ".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_WO_ITEM_TJ : DataObjectBase
    {
        public Row_R_WO_ITEM_TJ(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_ITEM_TJ GetDataObject()
        {
            R_WO_ITEM_TJ DataObject = new R_WO_ITEM_TJ();
            DataObject.POTX2 = this.POTX2;
            DataObject.POTX1 = this.POTX1;
            DataObject.LIFNR = this.LIFNR;
            DataObject.SORTF = this.SORTF;
            DataObject.POSTP = this.POSTP;
            DataObject.MOD_NO = this.MOD_NO;
            DataObject.CHARG = this.CHARG;
            DataObject.MENGE = this.MENGE;
            DataObject.BAUGR = this.BAUGR;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.LGORT = this.LGORT;
            DataObject.SHKZG = this.SHKZG;
            DataObject.REVLV = this.REVLV;
            DataObject.SOBSL = this.SOBSL;
            DataObject.DUMPS = this.DUMPS;
            DataObject.MAKTX = this.MAKTX;
            DataObject.ALPGR = this.ALPGR;
            DataObject.MATKL = this.MATKL;
            DataObject.MEINS = this.MEINS;
            DataObject.KDMAT = this.KDMAT;
            DataObject.ENMNG = this.ENMNG;
            DataObject.BDMNG = this.BDMNG;
            DataObject.MATNR = this.MATNR;
            DataObject.WERKS = this.WERKS;
            DataObject.POSNR = this.POSNR;
            DataObject.AUFNR = this.AUFNR;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string POTX2
        {
            get
            {
                return (string)this["POTX2"];
            }
            set
            {
                this["POTX2"] = value;
            }
        }
        public string POTX1
        {
            get
            {
                return (string)this["POTX1"];
            }
            set
            {
                this["POTX1"] = value;
            }
        }
        public string LIFNR
        {
            get
            {
                return (string)this["LIFNR"];
            }
            set
            {
                this["LIFNR"] = value;
            }
        }
        public string SORTF
        {
            get
            {
                return (string)this["SORTF"];
            }
            set
            {
                this["SORTF"] = value;
            }
        }
        public string POSTP
        {
            get
            {
                return (string)this["POSTP"];
            }
            set
            {
                this["POSTP"] = value;
            }
        }
        public string MOD_NO
        {
            get
            {
                return (string)this["MOD_NO"];
            }
            set
            {
                this["MOD_NO"] = value;
            }
        }
        public string CHARG
        {
            get
            {
                return (string)this["CHARG"];
            }
            set
            {
                this["CHARG"] = value;
            }
        }
        public string MENGE
        {
            get
            {
                return (string)this["MENGE"];
            }
            set
            {
                this["MENGE"] = value;
            }
        }
        public string BAUGR
        {
            get
            {
                return (string)this["BAUGR"];
            }
            set
            {
                this["BAUGR"] = value;
            }
        }
        public string QUANTITY
        {
            get
            {
                return (string)this["QUANTITY"];
            }
            set
            {
                this["QUANTITY"] = value;
            }
        }
        public string LGORT
        {
            get
            {
                return (string)this["LGORT"];
            }
            set
            {
                this["LGORT"] = value;
            }
        }
        public string SHKZG
        {
            get
            {
                return (string)this["SHKZG"];
            }
            set
            {
                this["SHKZG"] = value;
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
        public string SOBSL
        {
            get
            {
                return (string)this["SOBSL"];
            }
            set
            {
                this["SOBSL"] = value;
            }
        }
        public string DUMPS
        {
            get
            {
                return (string)this["DUMPS"];
            }
            set
            {
                this["DUMPS"] = value;
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
        public string ALPGR
        {
            get
            {
                return (string)this["ALPGR"];
            }
            set
            {
                this["ALPGR"] = value;
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
        public string MEINS
        {
            get
            {
                return (string)this["MEINS"];
            }
            set
            {
                this["MEINS"] = value;
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
        public string ENMNG
        {
            get
            {
                return (string)this["ENMNG"];
            }
            set
            {
                this["ENMNG"] = value;
            }
        }
        public string BDMNG
        {
            get
            {
                return (string)this["BDMNG"];
            }
            set
            {
                this["BDMNG"] = value;
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
        public string POSNR
        {
            get
            {
                return (string)this["POSNR"];
            }
            set
            {
                this["POSNR"] = value;
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
    }
    public class R_WO_ITEM_TJ
    {
        public string POTX2;
        public string POTX1;
        public string LIFNR;
        public string SORTF;
        public string POSTP;
        public string MOD_NO;
        public string CHARG;
        public string MENGE;
        public string BAUGR;
        public string QUANTITY;
        public string LGORT;
        public string SHKZG;
        public string REVLV;
        public string SOBSL;
        public string DUMPS;
        public string MAKTX;
        public string ALPGR;
        public string MATKL;
        public string MEINS;
        public string KDMAT;
        public string ENMNG;
        public string BDMNG;
        public string MATNR;
        public string WERKS;
        public string POSNR;
        public string AUFNR;
        public string ID;
    }
}