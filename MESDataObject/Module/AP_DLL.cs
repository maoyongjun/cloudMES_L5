using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class AP_DLL
    {
        public List<DataRow> C_Product_Config_GetBYSkuAndVerson(string skuno, string skuverson, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes1.c_product_config where p_no=:sku and p_version=:skuverson";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno),
                    new OleDbParameter(":skuverson", skuverson)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }
        public List<DataRow> R_PCBA_LINK_GetBYSku(string skuno, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes4.r_pcba_link where skuno=:sku";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":sku", skuno)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }
        public List<DataRow> R_TR_SN_GetBYTR_SN(string TRSN, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes4.r_tr_sn where tr_sn=:trsn";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":trsn", TRSN)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }
        public List<DataRow> R_TR_SN_WIP_GetBYTR_SN(string TRSN, OleExec DB)
        {
            List<DataRow> datarowlist = new List<DataRow>();
            string strSql = $@"select * from mes4.r_tr_sn_wip where tr_sn=:trsn";
            OleDbParameter[] paramet = new OleDbParameter[] {
                    new OleDbParameter(":trsn", TRSN)};
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    datarowlist.Add(res.Rows[i]);
                }
            }
            return datarowlist;
        }

        public string APUpdatePanlSN(string PanelSN, string SN, OleExec DB)
        {
            string ErrMessage = "";
            //Psn = PanelSession.InputValue.ToString();

            OleDbParameter[] PanelReplaceSP = new OleDbParameter[3];
            PanelReplaceSP[0] = new OleDbParameter("G_PANEL", PanelSN);
            PanelReplaceSP[1] = new OleDbParameter("G_PSN", SN);
            PanelReplaceSP[2] = new OleDbParameter();
            PanelReplaceSP[2].Size = 1000;
            PanelReplaceSP[2].ParameterName = "RES";
            PanelReplaceSP[2].Direction = System.Data.ParameterDirection.Output;
            string result = DB.ExecProcedureNoReturn("MES1.Z_PANEL_REPLACE_SP", PanelReplaceSP);
            return result;
        }


        public List<string> GetLocationList(string SKUNO, OleExec DB)
        {
            string strSql = string.Empty;
            List<string> result = new List<string>();
            strSql = $@" SELECT DISTINCT(A.LOCATION) FROM MES1.C_SMT_AP_LOCATION A,MES1.C_SMT_AP_PRODUCT B
                            WHERE A.SMT_CODE =B.SMT_CODE AND B.P_NO='{SKUNO}' ";

            DataTable res = DB.ExecSelect(strSql).Tables[0];
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    result.Add(res.Rows[i][0].ToString());
                }
            }
            return result;
        }

        public List<string> CheckLocationExist(string SKUNO,string LOCATION, OleExec DB)
        {
            string strSql = string.Empty;
            List<string> result = new List<string>();
            strSql = $@" SELECT DISTINCT(A.LOCATION) FROM MES1.C_SMT_AP_LOCATION A,MES1.C_SMT_AP_PRODUCT B
                            WHERE A.SMT_CODE =B.SMT_CODE AND B.P_NO='{SKUNO}' AND A.LOCATION ='{LOCATION}' ";

            DataTable res = DB.ExecSelect(strSql).Tables[0];
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    result.Add(res.Rows[i][0].ToString());
                }
            }
            return result;
        }

        public int APUpdateUndoSmtloading(string panel,OleExec DB)
        {
            string strSql = $@"update mes4.r_tr_product_detail set wo = '#' || substr(wo,2,11), p_sn = '#' || p_sn, tr_code = '#' || tr_code where p_sn in (select p_sn from mes4.r_sn_link where panel_no =:panel_no )";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":panel_no", panel) };
            int i = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);

            strSql = $@"update mes4.r_sn_link set p_sn = '#' || p_sn, wo = '#' || substr(wo,2,11), panel_no = '#' || panel_no where panel_no =:panel_no ";
            paramet = new OleDbParameter[] { new OleDbParameter(":panel_no", panel) };
            i = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);

            return i;
        }
    }
}
