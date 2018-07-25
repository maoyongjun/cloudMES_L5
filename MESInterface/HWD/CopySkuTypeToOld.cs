using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MESInterface.HWD
{
    public class CopySkuTypeToOld : taskBase
    {
        private OleExec newSFCDB = null;
        private OleExec oldSFCDB = null;
        string ip;

        public string updateDate = "";
        public override void init()
        {
            //base.init();
            try
            {
                newSFCDB = new OleExec("HWDMES", false);
                oldSFCDB = new OleExec("HWD_OLD_SFCDB", false);
                List<System.Net.IPAddress> temp = HWDNNSFCBase.HostInfo.IP.Where(ipv4 => ipv4.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                ip = temp[0].ToString();
                Output.UI = new CopySkuTypeToOld_UI(this);
            }
            catch (Exception ex)
            {
                throw new Exception("Init CopySkuTypeToOld Fail" + ex.Message);
            }
        }

        public override void Start()
        {
            string oldSql = "";
            string newSql = "";
            string runSql = "";
            string codeName = "";
            string codeValue = "";
            string description = "";
            string route = "";
            DataTable dtNew = new DataTable();
            DataTable dtOld = new DataTable();
            DataTable dtRoute = new DataTable();

            try
            {

                if (updateDate != "")
                {
                    newSql = $@"select * from c_sku where to_date(to_char(edit_time,'yyyy/mm/dd'),'yyyy/mm/dd') =to_date('{updateDate}','yyyy/mm/dd')";
                }
                else
                {
                    newSql = "select * from c_sku where edit_time>sysdate-1";
                }
                dtNew = newSFCDB.ExecSelect(newSql).Tables[0];
                //WriteLog.WriteIntoMESLog(newSFCDB, "HWD", "MESInterface", "MESInterface.HWD.CopySkuTypeToOld", "CopySkuTypeToOld", ip + ";" + dtNew.Rows.Count + "; Copy sku type to old DB fail," , "", "interface");
                //newSFCDB.CommitTrain();
                foreach (DataRow row in dtNew.Rows)
                {
                    try
                    {
                        runSql = $@"select c.* from r_sku_route a,c_sku b,c_route c where a.sku_id=b.id and c.id=a.route_id and b.skuno='{row["SKUNO"].ToString()}'";
                        dtRoute = newSFCDB.ExecSelect(runSql).Tables[0];
                        if (dtRoute.Rows.Count == 0)
                        {
                            throw new Exception(row["SKUNO"].ToString() + " can't setting route!");
                        }
                        route = dtRoute.Rows[0]["ROUTE_NAME"].ToString();

                        oldSql = $@"select * from sfccodelike where skuno='{row["SKUNO"].ToString()}'";
                        dtOld = oldSFCDB.ExecSelect(oldSql).Tables[0];
                        if (dtOld.Rows.Count > 0)
                        {
                            if (row["SKU_TYPE"].ToString() != "" && dtOld.Rows[0]["CATEGORY"].ToString() != row["SKU_TYPE"].ToString())
                            {
                                runSql = $@" update sfccodelike set category='{row["SKU_TYPE"].ToString()}' where skuno='{row["SKUNO"].ToString()}'";
                            }
                        }
                        else
                        {
                            if (row["SKU_NAME"].ToString() != "")
                            {
                                codeName = row["SKU_NAME"].ToString();
                                codeValue = row["SKU_NAME"].ToString();
                            }
                            else
                            {
                                codeName = row["SKUNO"].ToString();
                                codeValue = row["SKUNO"].ToString();
                            }

                            if (row["DESCRIPTION"].ToString() != "")
                            {
                                description = row["DESCRIPTION"].ToString();
                            }
                            else
                            {
                                description = row["SKUNO"].ToString();
                            }

                            if (row["SKU_TYPE"].ToString() != "")
                            {
                                runSql = $@"insert into sfccodelike
                                (category,codename,codevalue,skuno,version,custpartno,sfcroute,createby,createdate,series,ctntype,pltype,description)
                                values
                                ('{row["SKU_TYPE"].ToString()}',
                                '{codeName}',
                                '{codeValue}',
                                '{row["SKUNO"].ToString()}',
                                '{row["VERSION"].ToString()}',
                                '{row["CUST_PARTNO"].ToString()}',
                                '{route}', 
                                '{row["EDIT_EMP"].ToString()}',
                                 sysdate,
                                'HWD','C','PL','{description}')";
                            }
                        }
                        //WriteLog.WriteIntoMESLog(newSFCDB, "HWD", "MESInterface", "MESInterface.HWD.CopySkuTypeToOld", "CopySkuTypeToOld", ip + ";" + row["SKUNO"].ToString() + "; Copy sku type to old DB fail,", runSql, "interface");
                        //newSFCDB.CommitTrain();
                        if (runSql != "")
                        {
                            oldSFCDB.ExecSQL(runSql);
                            oldSFCDB.CommitTrain();
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteIntoMESLog(newSFCDB, "HWD", "MESInterface", "MESInterface.HWD.CopySkuTypeToOld", "CopySkuTypeToOld", ip + ";" + row["SKUNO"].ToString() + ";" + ex.Message.ToString(), "", "interface");
                        newSFCDB.CommitTrain();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Start CopySkuTypeToOld Fail" + ex.Message);
                //WriteLog.WriteIntoMESLog(newSFCDB, "HWD", "MESInterface", "MESInterface.HWD.CopySkuTypeToOld", "CopySkuTypeToOld", ip + ";Copy sku type to old DB fail," + ex.Message.ToString(), "", "interface");
            }
        }
    }
}
