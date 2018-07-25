using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface
{
    public class InterfacePublicValues
    {
        /// <summary>
        /// 是否月結
        /// </summary>
        /// <param name="DB">OleExec</param>
        /// <param name="dbType">DB_TYPE_ENUM</param>
        /// <returns></returns>
        public static bool IsMonthly(OleExec DB, DB_TYPE_ENUM dbType)
        {
            bool isMonthly = false;
            string[] times;
            string sql = string.Empty;
            T_C_CONTROL controlObject = new T_C_CONTROL(DB, DB_TYPE_ENUM.Oracle);
            C_CONTROL control = controlObject.GetControlByName("BACKFLUSH", DB);
            if (control != null && dbType == DB_TYPE_ENUM.Oracle)
            {
                times = control.CONTROL_VALUE.Split(new char[] { '~' });

                sql = $@"select 1 from dual where sysdate between to_date('{times[0]}' ,'yyyy-mm-dd hh24:mi:ss') and to_date('{times[1]}' ,'yyyy-mm-dd hh24:mi:ss')";
                DataSet temp = DB.RunSelect(sql);
                if (temp.Tables[0].Rows.Count > 0)
                {
                    isMonthly = true;
                }
            }
            return isMonthly;
        }

        public static string GetPostDate(OleExec sfcdb)
        {
            string strPostDATE = "";
            string sqlString = $@"select to_date(a.control_value, 'mm/dd/yyyy') - sysdate C,
                                  a.control_value DAT,
                                  to_char(sysdate, 'mm/dd/yyyy') NOW
                                  from c_control a where upper(CONTROL_NAME)='BACKFLUSHPOSTEDATE'";

            DataSet postDate = sfcdb.ExecuteDataSet(sqlString, CommandType.Text);

            if (float.Parse(postDate.Tables[0].Rows[0]["C"].ToString()) > 0)
            {
                strPostDATE = postDate.Tables[0].Rows[0]["DAT"].ToString();
            }
            else
            {
                strPostDATE = postDate.Tables[0].Rows[0]["NOW"].ToString();
            }
            return strPostDATE;

        }

        /// <summary>
        /// Get DB system datetime
        /// </summary>
        /// <param name="DB">OleExec</param>
        /// <param name="dbType">DB_TYPE_ENUM</param>
        /// <returns></returns>
        public static DateTime GetDBDateTime(OleExec DB, DB_TYPE_ENUM dbType)
        {           
            string strSql = "select sysdate from dual";
            if (dbType == DB_TYPE_ENUM.Oracle)
            {
                strSql = "select sysdate from dual";
            }
            else if (dbType == DB_TYPE_ENUM.SqlServer)
            {
                strSql = "select get_date() ";
            }
            else
            {
                throw new Exception(dbType.ToString() + " not Work");
            }
            DateTime DBTime = (DateTime)DB.ExecSelectOneValue(strSql);            
            return DBTime;
        }
    }
}
