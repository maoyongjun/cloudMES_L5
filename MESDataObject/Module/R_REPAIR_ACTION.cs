using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_r_repair_action : DataObjectTable
    {
        public T_r_repair_action(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_r_repair_action(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_r_repair_action);
            TableName = "r_repair_action".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable SelectRepairActionBySN(string sn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from r_repair_action where sn='{sn}' ";
            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;

        }
    }
    public class Row_r_repair_action : DataObjectBase
    {
        public Row_r_repair_action(DataObjectInfo info) : base(info)
        {

        }
        public r_repair_action GetDataObject()
        {
            r_repair_action DataObject = new r_repair_action();
            DataObject.ID = this.ID;
            DataObject.REPAIR_FAILCODE_ID = this.REPAIR_FAILCODE_ID;
            DataObject.SN = this.SN;
            DataObject.ACTION_CODE = this.ACTION_CODE;
            DataObject.SECTION_ID = this.SECTION_ID;
            DataObject.PROCESS = this.PROCESS;
            DataObject.ITEMS_ID = this.ITEMS_ID;
            DataObject.ITEMS_SON_ID = this.ITEMS_SON_ID;
            DataObject.REASON_CODE = this.REASON_CODE;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.FAIL_LOCATION = this.FAIL_LOCATION;
            DataObject.FAIL_CODE = this.FAIL_CODE;
            DataObject.KEYPART_SN = this.KEYPART_SN;
            DataObject.NEW_KEYPART_SN = this.NEW_KEYPART_SN;
            DataObject.KP_NO = this.KP_NO;
            DataObject.TR_SN = this.TR_SN;
            DataObject.MFR_CODE = this.MFR_CODE;
            DataObject.MFR_NAME = this.MFR_NAME;
            DataObject.DATE_CODE = this.DATE_CODE;
            DataObject.LOT_CODE = this.LOT_CODE;
            DataObject.NEW_KP_NO = this.NEW_KP_NO;
            DataObject.NEW_TR_SN = this.NEW_TR_SN;
            DataObject.NEW_MFR_CODE = this.NEW_MFR_CODE;
            DataObject.NEW_MFR_NAME = this.NEW_MFR_NAME;
            DataObject.NEW_DATE_CODE = this.NEW_DATE_CODE;
            DataObject.NEW_LOT_CODE = this.NEW_LOT_CODE;
            DataObject.REPAIR_EMP = this.REPAIR_EMP;
            DataObject.REPAIR_TIME = this.REPAIR_TIME;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string REPAIR_FAILCODE_ID
        {
            get

            {
                return (string)this["REPAIR_FAILCODE_ID"];
            }
            set
            {
                this["REPAIR_FAILCODE_ID"] = value;
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
        public string ACTION_CODE
        {
            get

            {
                return (string)this["ACTION_CODE"];
            }
            set
            {
                this["ACTION_CODE"] = value;
            }
        }
        public string SECTION_ID
        {
            get

            {
                return (string)this["SECTION_ID"];
            }
            set
            {
                this["SECTION_ID"] = value;
            }
        }
        public string PROCESS
        {
            get

            {
                return (string)this["PROCESS"];
            }
            set
            {
                this["PROCESS"] = value;
            }
        }
        public string ITEMS_ID
        {
            get

            {
                return (string)this["ITEMS_ID"];
            }
            set
            {
                this["ITEMS_ID"] = value;
            }
        }
        public string ITEMS_SON_ID
        {
            get

            {
                return (string)this["ITEMS_SON_ID"];
            }
            set
            {
                this["ITEMS_SON_ID"] = value;
            }
        }
        public string REASON_CODE
        {
            get

            {
                return (string)this["REASON_CODE"];
            }
            set
            {
                this["REASON_CODE"] = value;
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
        public string KEYPART_SN
        {
            get

            {
                return (string)this["KEYPART_SN"];
            }
            set
            {
                this["KEYPART_SN"] = value;
            }
        }
        public string NEW_KEYPART_SN
        {
            get

            {
                return (string)this["NEW_KEYPART_SN"];
            }
            set
            {
                this["NEW_KEYPART_SN"] = value;
            }
        }
        public string KP_NO
        {
            get

            {
                return (string)this["KP_NO"];
            }
            set
            {
                this["KP_NO"] = value;
            }
        }
        public string TR_SN
        {
            get

            {
                return (string)this["TR_SN"];
            }
            set
            {
                this["TR_SN"] = value;
            }
        }
        public string MFR_CODE
        {
            get

            {
                return (string)this["MFR_CODE"];
            }
            set
            {
                this["MFR_CODE"] = value;
            }
        }
        public string MFR_NAME
        {
            get

            {
                return (string)this["MFR_NAME"];
            }
            set
            {
                this["MFR_NAME"] = value;
            }
        }
        public string DATE_CODE
        {
            get

            {
                return (string)this["DATE_CODE"];
            }
            set
            {
                this["DATE_CODE"] = value;
            }
        }
        public string LOT_CODE
        {
            get

            {
                return (string)this["LOT_CODE"];
            }
            set
            {
                this["LOT_CODE"] = value;
            }
        }
        public string NEW_KP_NO
        {
            get

            {
                return (string)this["NEW_KP_NO"];
            }
            set
            {
                this["NEW_KP_NO"] = value;
            }
        }
        public string NEW_TR_SN
        {
            get

            {
                return (string)this["NEW_TR_SN"];
            }
            set
            {
                this["NEW_TR_SN"] = value;
            }
        }
        public string NEW_MFR_CODE
        {
            get

            {
                return (string)this["NEW_MFR_CODE"];
            }
            set
            {
                this["NEW_MFR_CODE"] = value;
            }
        }
        public string NEW_MFR_NAME
        {
            get
            {
                return (string)this["NEW_MFR_NAME"];
            }
            set
            {
                this["NEW_MFR_NAME"] = value;
            }
        }
        public string NEW_DATE_CODE
        {
            get

            {
                return (string)this["NEW_DATE_CODE"];
            }
            set
            {
                this["NEW_DATE_CODE"] = value;
            }
        }
        public string NEW_LOT_CODE
        {
            get

            {
                return (string)this["NEW_LOT_CODE"];
            }
            set
            {
                this["NEW_LOT_CODE"] = value;
            }
        }
        public string REPAIR_EMP
        {
            get

            {
                return (string)this["REPAIR_EMP"];
            }
            set
            {
                this["REPAIR_EMP"] = value;
            }
        }
        public DateTime? REPAIR_TIME
        {
            get

            {
                return (DateTime?)this["REPAIR_TIME"];
            }
            set
            {
                this["REPAIR_TIME"] = value;
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
    public class r_repair_action
    {
        public string ID;
        public string REPAIR_FAILCODE_ID;
        public string SN;
        public string ACTION_CODE;
        public string SECTION_ID;
        public string PROCESS;
        public string ITEMS_ID;
        public string ITEMS_SON_ID;
        public string REASON_CODE;
        public string DESCRIPTION;
        public string FAIL_LOCATION;
        public string FAIL_CODE;
        public string KEYPART_SN;
        public string NEW_KEYPART_SN;
        public string KP_NO;
        public string TR_SN;
        public string MFR_CODE;
        public string MFR_NAME;
        public string DATE_CODE;
        public string LOT_CODE;
        public string NEW_KP_NO;
        public string NEW_TR_SN;
        public string NEW_MFR_CODE;
        public string NEW_MFR_NAME;
        public string NEW_DATE_CODE;
        public string NEW_LOT_CODE;
        public string REPAIR_EMP;
        public DateTime? REPAIR_TIME;
        public DateTime? EDIT_TIME;
        public string EDIT_EMP;
    }
}