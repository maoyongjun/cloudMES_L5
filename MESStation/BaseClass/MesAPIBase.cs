using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
//using WebServer.SocketService;

namespace MESStation.BaseClass
{
  
    /// <summary>
    /// 作為所有API的基礎類
    /// </summary>
    public class MesAPIBase
    {
        protected Dictionary<string, OleExecPool> _DBPools = new Dictionary<string, OleExecPool>();

        public MESStation.LogicObject.User LoginUser;
        public string BU;
        public string Language= "CHINESE";
        public String SystemName = "MES";
        public DB_TYPE_ENUM DBTYPE = DB_TYPE_ENUM.Oracle;
        protected bool _MastLogin = true;
        public string IP = "";

        public bool MastLogin
        {
            get
            {
                return _MastLogin;
            }
        }

        public Dictionary<string, OleExecPool> DBPools
        {
            
            get
            {
                return _DBPools;
            }
        }




        Dictionary<string, APIInfo> _Apis= new Dictionary<string ,APIInfo>();
        public Dictionary<string, APIInfo> Apis
        {
            get
            {
                
                return _Apis;
            }
        }

        public DateTime GetDBDateTime()
        {
            OleExec sfcdb = _DBPools["SFCDB"].Borrow();
            try
            {
                string strSql = "select sysdate from dual";
                if (DBTYPE == DB_TYPE_ENUM.Oracle)
                {
                    strSql = "select sysdate from dual";
                }
                else if (DBTYPE == DB_TYPE_ENUM.SqlServer)
                {
                    strSql = "select get_date() ";
                }
                else
                {
                    throw new Exception(DBTYPE.ToString() + " not Work");
                }
                DateTime DBTime = (DateTime)sfcdb.ExecSelectOneValue(strSql);
                _DBPools["SFCDB"].Return(sfcdb);
                return DBTime;
            }
            catch (Exception e)
            {
                _DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        

    }

   
}
