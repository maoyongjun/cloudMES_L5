using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;

namespace MESInterface
{
    public class WriteLog
    {
        public static void WriteIntoMESLog(OleExec SFCDB, string bu,string programName, string className, string functionName,string logMessage,string logSql, string editEmp)
        {
            //OleExec SFCDB = new OleExec(db, false);
            T_R_MES_LOG mesLog = new T_R_MES_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
            string id = mesLog.GetNewID(bu, SFCDB);
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
            rowMESLog.ID = id;
            rowMESLog.PROGRAM_NAME = programName;
            rowMESLog.CLASS_NAME = className;
            rowMESLog.FUNCTION_NAME = functionName;
            rowMESLog.LOG_MESSAGE = logMessage;
            rowMESLog.LOG_SQL = logSql;
            rowMESLog.EDIT_EMP = editEmp;
            rowMESLog.EDIT_TIME = System.DateTime.Now;
            SFCDB.ThrowSqlExeception = true;
            SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
        }
    }
}
