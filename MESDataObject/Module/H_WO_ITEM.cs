using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_WO_ITEM : DataObjectTable
    {
        public T_H_WO_ITEM(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_WO_ITEM(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_WO_ITEM);
            TableName = "H_WO_ITEM".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_H_WO_ITEM : DataObjectBase
    {
        public Row_H_WO_ITEM(DataObjectInfo info) : base(info)
        {

        }
        public H_WO_ITEM GetDataObject()
        {
            H_WO_ITEM DataObject = new H_WO_ITEM();
            DataObject.VORNR = this.VORNR;
            DataObject.RSPOS = this.RSPOS;
            DataObject.CHARG = this.CHARG;
            DataObject.SHKZG = this.SHKZG;
            DataObject.XLOEK = this.XLOEK;
            DataObject.BISMT = this.BISMT;
            DataObject.DUMPS = this.DUMPS;
            DataObject.ENMNG = this.ENMNG;
            DataObject.LGORT = this.LGORT;
            DataObject.RGEKZ = this.RGEKZ;
            DataObject.MVGR3 = this.MVGR3;
            DataObject.ABLAD = this.ABLAD;
            DataObject.ALPOS = this.ALPOS;
            DataObject.WGBEZ = this.WGBEZ;
            DataObject.MATKL = this.MATKL;
            DataObject.MAKTX = this.MAKTX;
            DataObject.AEDAT = this.AEDAT;
            DataObject.AENAM = this.AENAM;
            DataObject.AUART = this.AUART;
            DataObject.REPPARTNO = this.REPPARTNO;
            DataObject.REPNO = this.REPNO;
            DataObject.BAUGR = this.BAUGR;
            DataObject.REVLV = this.REVLV;
            DataObject.MEINS = this.MEINS;
            DataObject.BDMNG = this.BDMNG;
            DataObject.KDMAT = this.KDMAT;
            DataObject.PARTS = this.PARTS;
            DataObject.MATNR = this.MATNR;
            DataObject.POSNR = this.POSNR;
            DataObject.AUFNR = this.AUFNR;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string VORNR
        {
            get
            {
                return (string)this["VORNR"];
            }
            set
            {
                this["VORNR"] = value;
            }
        }
        public string RSPOS
        {
            get
            {
                return (string)this["RSPOS"];
            }
            set
            {
                this["RSPOS"] = value;
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
        public string XLOEK
        {
            get
            {
                return (string)this["XLOEK"];
            }
            set
            {
                this["XLOEK"] = value;
            }
        }
        public string BISMT
        {
            get
            {
                return (string)this["BISMT"];
            }
            set
            {
                this["BISMT"] = value;
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
        public string RGEKZ
        {
            get
            {
                return (string)this["RGEKZ"];
            }
            set
            {
                this["RGEKZ"] = value;
            }
        }
        public string MVGR3
        {
            get
            {
                return (string)this["MVGR3"];
            }
            set
            {
                this["MVGR3"] = value;
            }
        }
        public string ABLAD
        {
            get
            {
                return (string)this["ABLAD"];
            }
            set
            {
                this["ABLAD"] = value;
            }
        }
        public string ALPOS
        {
            get
            {
                return (string)this["ALPOS"];
            }
            set
            {
                this["ALPOS"] = value;
            }
        }
        public string WGBEZ
        {
            get
            {
                return (string)this["WGBEZ"];
            }
            set
            {
                this["WGBEZ"] = value;
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
        public string REPPARTNO
        {
            get
            {
                return (string)this["REPPARTNO"];
            }
            set
            {
                this["REPPARTNO"] = value;
            }
        }
        public string REPNO
        {
            get
            {
                return (string)this["REPNO"];
            }
            set
            {
                this["REPNO"] = value;
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
        public string PARTS
        {
            get
            {
                return (string)this["PARTS"];
            }
            set
            {
                this["PARTS"] = value;
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
    public class H_WO_ITEM
    {
        public string VORNR;
        public string RSPOS;
        public string CHARG;
        public string SHKZG;
        public string XLOEK;
        public string BISMT;
        public string DUMPS;
        public string ENMNG;
        public string LGORT;
        public string RGEKZ;
        public string MVGR3;
        public string ABLAD;
        public string ALPOS;
        public string WGBEZ;
        public string MATKL;
        public string MAKTX;
        public string AEDAT;
        public string AENAM;
        public string AUART;
        public string REPPARTNO;
        public string REPNO;
        public string BAUGR;
        public string REVLV;
        public string MEINS;
        public string BDMNG;
        public string KDMAT;
        public string PARTS;
        public string MATNR;
        public string POSNR;
        public string AUFNR;
        public string ID;
    }
}