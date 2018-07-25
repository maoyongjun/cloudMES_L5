using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_HEADER : DataObjectTable
    {
        public T_R_WO_HEADER(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_HEADER(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_HEADER);
            TableName = "R_WO_HEADER".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable GetConvertWoList(OleExec DB, DB_TYPE_ENUM DBType)
        {
            string StrSql = "";
            StrSql = $@"select * from R_WO_HEADER a where not exists (select 1 from R_WO_BASE b WHERE a.AUFNR=b.WORKORDERNO) ";
            StrSql = StrSql + " and SYSDATE-TO_DATE(FTRMI,'YYYY-MM-DD')>0 ";
            DataTable dt = DB.ExecSelect(StrSql).Tables[0];            
            return dt;
        }

        public DataTable GetConvertWoTable(OleExec db,DB_TYPE_ENUM dbtype,string [] arrayConvertWO)
        {
            string sql = $@"select * from r_wo_header a where not exists (select 1 from r_wo_base b where a.aufnr=b.workorderno) 
                            and sysdate-to_date(ftrmi,'yyyy-mm-dd')>0";
            string tempSql = "";
            for (int i = 0; i < arrayConvertWO.Length; i++)
            {
                if (!string.IsNullOrEmpty(arrayConvertWO[i]) && arrayConvertWO[i] != "")
                {
                    if (i == 0)
                    {
                        tempSql = tempSql + $@" aufnr like '{arrayConvertWO[i]}%'";
                    }
                    else
                    {
                        tempSql = tempSql + $@" or aufnr like '{arrayConvertWO[i]}%'";
                    }
                }
            }
            if (tempSql != "")
            {
                tempSql = $@" and ({tempSql})";
            }
            sql = sql + tempSql;
           // sql = "select *  from r_wo_header where aufnr ='002530045770'";
            return db.ExecSelect(sql).Tables[0];
        }
        public DataTable GetConvertWoTableByWO(OleExec db, DB_TYPE_ENUM dbtype, string wo)
        {
            string sql = $@"select * from r_wo_header a where a.aufnr='{wo}' and not exists (select 1 from r_wo_base b where a.aufnr=b.workorderno) 
                            and sysdate-to_date(ftrmi,'yyyy-mm-dd')>0";
            return db.ExecSelect(sql).Tables[0];
        }

        public DataTable GetConvertWoTableById(OleExec db, DB_TYPE_ENUM dbtype, string wo)
        {
            string sql = $@"select 
    id, aufnr workorderno,werks factory,auart ordertype, matnr skuno,
    revlv version,gstrs startdate,gamng qty,matkl materialgroup, maktx description,
    erdat||' '||erfzeit create_date,gltrs schedule_date,lgort storage_loc,ablad unload_point,rohs_value 
    from r_wo_header a where a.id='{wo}' and not exists (select 1 from r_wo_base b where a.aufnr=b.workorderno) 
                            and sysdate-to_date(ftrmi,'yyyy-mm-dd')>0";
            return db.ExecSelect(sql).Tables[0];
        }

        public DataTable GetConvertWoTableById(OleExec sfcdb, string id)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select a.id,a.aufnr workorderno,a.werks factory,b.workorder_type ordertype, a.matnr skuno, 
                    a.revlv version, a.gstrs startdate, a.gamng qty, a.matkl materialgroup, a.maktx description, 
                    a.erdat||' '||a.erfzeit create_date,a.gltrs schedule_date,a.lgort storage_loc,a.ablad unload_point,a.rohs_value 
                    from r_wo_header a left join r_wo_type b on a.auart=b.order_type where a.id='{id}' ";
                
                try
                {
                    DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                    else
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000034"));
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }
            
        }

        public R_WO_HEADER GetDetailByWo(OleExec db, string wo)
        {
            DataTable dt = null;
            R_WO_HEADER wo_header = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select * from {TableName} where aufnr='{wo}' ";
                try
                {
                    dt = db.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        Row_R_WO_HEADER row_header = (Row_R_WO_HEADER) NewRow();
                        row_header.loadData(dt.Rows[0]);
                        wo_header = row_header.GetDataObject();
                    }
                    return wo_header;
                }
                catch (Exception ex)
                {

                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }
                
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { this.DBType.ToString() }));
            }
        }


        /// <summary>
        /// 工單轉換列表-特定字段
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="arrayConvertWo"></param>
        /// <returns></returns>
        public DataTable GetWoSpecialVar(OleExec db, string[] arrayConvertWO)
        {
            string sql = $@"select 
    id, aufnr workorderno,werks factory,auart ordertype, matnr skuno,
    revlv version,gstrs startdate,gamng qty,matkl materialgroup, maktx description,
    erdat create_date,gltrs schedule_date,lgort storage_loc,ablad unload_point,rohs_value 
from r_wo_header a where not exists (select 1 from r_wo_base b where a.aufnr=b.workorderno) 
                            and sysdate-to_date(ftrmi,'yyyy-mm-dd')>0";
            string tempSql = "";
            for (int i = 0; i < arrayConvertWO.Length; i++)
            {
                if (!string.IsNullOrEmpty(arrayConvertWO[i]) && arrayConvertWO[i] != "")
                {
                    if (i == 0)
                    {
                        tempSql = tempSql + $@" aufnr like '{arrayConvertWO[i]}%'";
                    }
                    else
                    {
                        tempSql = tempSql + $@" or aufnr like '{arrayConvertWO[i]}%'";
                    }
                }
            }
            if (tempSql != "")
            {
                tempSql = $@" and ({tempSql})";
            }
            sql = sql + tempSql;
            DataTable dt = db.ExecSelect(sql).Tables[0];
            return dt;
        }

        public bool CheckWoHeadByWo(string Workorderno, bool Download_Auto, string ColumnName, OleExec DB, DB_TYPE_ENUM DBType)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;
            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_HEADER where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }
            }
            else
            {
                StrSql = $@"select * from R_WO_HEADER where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    DB.ExecSQL($@" delete from H_WO_HEADER where AUFNR = '{Workorderno}'");
                    StrSql = $@"insert into H_WO_HEADER(ID,{ ColumnName }) ";
                    StrSql = StrSql + $@" select* from R_WO_HEADER where AUFNR = '{Workorderno}' ";
                    StrReturnMsg = DB.ExecSQL(StrSql);
                    int.TryParse(StrReturnMsg, out n);
                    if (n > 0)
                    {
                        CheckFlag = false;
                        StrSql = $@" delete from R_WO_HEADER where AUFNR = '{Workorderno}' ";
                        StrReturnMsg = DB.ExecSQL(StrSql);
                    }
                }
            }

            return CheckFlag;
        }

        public bool CheckWoHeadByWo(string Workorderno, bool Download_Auto,OleExec DB)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;
            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_HEADER where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }
            }
            else
            {
                StrSql = $@"select * from R_WO_HEADER where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    DB.ExecSQL($@" delete from H_WO_HEADER where AUFNR = '{Workorderno}'");
                    StrSql = $@"insert into H_WO_HEADER select* from R_WO_HEADER where AUFNR = '{Workorderno}' ";
                    StrReturnMsg = DB.ExecSQL(StrSql);
                    int.TryParse(StrReturnMsg, out n);
                    if (n > 0)
                    {
                        CheckFlag = false;
                        StrSql = $@" delete from R_WO_HEADER where AUFNR = '{Workorderno}' ";
                        StrReturnMsg = DB.ExecSQL(StrSql);
                    }
                }
            }

            return CheckFlag;
        }
        public string EditWoHead(string EditSql, OleExec DB, DB_TYPE_ENUM DBType)
        {
            DB.ThrowSqlExeception = true;
            //string ReturnMsg = DB.ExecSQL(EditSql);

            return DB.ExecSQL(EditSql);
        }

    }
    public class Row_R_WO_HEADER : DataObjectBase
    {
        public Row_R_WO_HEADER(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_HEADER GetDataObject()
        {
            R_WO_HEADER DataObject = new R_WO_HEADER();
            DataObject.ID = this.ID;
            DataObject.AUFNR = this.AUFNR;
            DataObject.WERKS = this.WERKS;
            DataObject.AUART = this.AUART;
            DataObject.MATNR = this.MATNR;
            DataObject.REVLV = this.REVLV;
            DataObject.KDAUF = this.KDAUF;
            DataObject.GSTRS = this.GSTRS;
            DataObject.GAMNG = this.GAMNG;
            DataObject.KDMAT = this.KDMAT;
            DataObject.AEDAT = this.AEDAT;
            DataObject.AENAM = this.AENAM;
            DataObject.MATKL = this.MATKL;
            DataObject.MAKTX = this.MAKTX;
            DataObject.ERDAT = this.ERDAT;
            DataObject.GSUPS = this.GSUPS;
            DataObject.ERFZEIT = this.ERFZEIT;
            DataObject.GLTRS = this.GLTRS;
            DataObject.GLUPS = this.GLUPS;
            DataObject.LGORT = this.LGORT;
            DataObject.ABLAD = this.ABLAD;
            DataObject.ROHS_VALUE = this.ROHS_VALUE;
            DataObject.FTRMI = this.FTRMI;
            DataObject.MVGR3 = this.MVGR3;
            DataObject.WEMNG = this.WEMNG;
            DataObject.BISMT = this.BISMT;
            DataObject.CHARG = this.CHARG;
            DataObject.SAENR = this.SAENR;
            DataObject.AETXT = this.AETXT;
            DataObject.GLTRP = this.GLTRP;
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
        public string GSUPS
        {
            get
            {
                return (string)this["GSUPS"];
            }
            set
            {
                this["GSUPS"] = value;
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
        public string GLTRS
        {
            get
            {
                return (string)this["GLTRS"];
            }
            set
            {
                this["GLTRS"] = value;
            }
        }
        public string GLUPS
        {
            get
            {
                return (string)this["GLUPS"];
            }
            set
            {
                this["GLUPS"] = value;
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
        public string ROHS_VALUE
        {
            get
            {
                return (string)this["ROHS_VALUE"];
            }
            set
            {
                this["ROHS_VALUE"] = value;
            }
        }
        public string FTRMI
        {
            get
            {
                return (string)this["FTRMI"];
            }
            set
            {
                this["FTRMI"] = value;
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
        public string SAENR
        {
            get
            {
                return (string)this["SAENR"];
            }
            set
            {
                this["SAENR"] = value;
            }
        }
        public string AETXT
        {
            get
            {
                return (string)this["AETXT"];
            }
            set
            {
                this["AETXT"] = value;
            }
        }
        public string GLTRP
        {
            get
            {
                return (string)this["GLTRP"];
            }
            set
            {
                this["GLTRP"] = value;
            }
        }
    }
    public class R_WO_HEADER
    {
        public string ID;
        public string AUFNR;
        public string WERKS;
        public string AUART;
        public string MATNR;
        public string REVLV;
        public string KDAUF;
        public string GSTRS;
        public string GAMNG;
        public string KDMAT;
        public string AEDAT;
        public string AENAM;
        public string MATKL;
        public string MAKTX;
        public string ERDAT;
        public string GSUPS;
        public string ERFZEIT;
        public string GLTRS;
        public string GLUPS;
        public string LGORT;
        public string ABLAD;
        public string ROHS_VALUE;
        public string FTRMI;
        public string MVGR3;
        public string WEMNG;
        public string BISMT;
        public string CHARG;
        public string SAENR;
        public string AETXT;
        public string GLTRP;
    }
}