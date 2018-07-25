using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDataObject.Module;
using MESDataObject;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace MESStation.FileUpdate
{
    public class Fileuplaod : MesAPIBase
    {
        protected APIInfo _GetLabelList = new APIInfo()
        {
            FunctionName = "GetLabelList",
            Description = "GetLabelList",
            Parameters = new List<APIInputInfo>()
            {
               
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _UpLoadLabelFile = new APIInfo()
        {
            FunctionName = "UpLoadLabelFile",
            Description = "UpLoadLabelFile",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "FileName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "MD5", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "LabelName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "PrintType", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ArryLength", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "Bas64File", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _UpLoadFile = new APIInfo()
        {
            FunctionName = "UpLoadFile",
            Description = "UpLoadFile",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "FileName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "MD5", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "Bas64File", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetFileByName = new APIInfo()
        {
            FunctionName = "GetFileByName",
            Description = "GetFileByName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = "test"},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = "LAB"}
               
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetFileList = new APIInfo()
        {
            FunctionName = "GetFileList",
            Description = "GetFileList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = "LAB"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetFileUseType = new APIInfo()
        {
            FunctionName = "GetFileUseType",
            Description = "GetFileUseType",
            Parameters = new List<APIInputInfo>(),
            
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _FileDownLoad = new APIInfo()
        {
            FunctionName = "FileDownLoad",
            Description = "FileDownLoad",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "STRING", DefaultValue = "LAB"}
            },

            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetFileHisListByName = new APIInfo()
        {
            FunctionName = "GetFileHisListByName",
            Description = "GetFileHisListByName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "Name", InputType = "STRING", DefaultValue = "test"},
                new APIInputInfo() {InputName = "UseType", InputType = "STRING", DefaultValue = "LAB"}

            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public Fileuplaod()
        {
            Apis.Add(_UpLoadFile.FunctionName, _UpLoadFile);
            Apis.Add(_GetFileUseType.FunctionName, _GetFileUseType);
            Apis.Add(_GetFileByName.FunctionName, _GetFileByName);
            Apis.Add(_GetFileList.FunctionName, _GetFileList);
            Apis.Add(_FileDownLoad.FunctionName, _FileDownLoad);
            Apis.Add(_GetFileHisListByName.FunctionName, _GetFileHisListByName);
            Apis.Add(_UpLoadLabelFile.FunctionName, _UpLoadLabelFile);
            Apis.Add(_GetLabelList.FunctionName, _GetLabelList);
        }
        public void GetLabelList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
           
            try
            {
                T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_Label> ret = TRL.GetLabelList(SFCDB);

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }


        public void UpLoadLabelFile(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            

            string LabelName = Data["LabelName"].ToString();
            string PrintType = Data["PrintType"].ToString();
            string ArryLength = Data["ArryLength"].ToString();
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            SFCDB.BeginTrain();
            try
            {

                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);

                Row_R_FILE RRF = (Row_R_FILE)TRF.NewRow();
                RRF.ID = TRF.GetNewID(BU, SFCDB);
                RRF.NAME = Data["Name"].ToString();
                RRF.FILENAME = Data["FileName"].ToString();
                RRF.MD5 = Data["MD5"].ToString();
                RRF.USETYPE = Data["UseType"].ToString();
                RRF.STATE = "1";
                RRF.VALID = 1;
                //不使用CLOB字段
                //RRF.CLOB_FILE = ":CLOB_FILE";// Data["Bas64File"].ToString();
                RRF.BLOB_FILE = ":BLOB_FILE";
                RRF.EDIT_EMP = LoginUser.EMP_NO;
                RRF.EDIT_TIME = DateTime.Now;
                SFCDB.ThrowSqlExeception = true;
                //將同類文件改為歷史版本
                TRF.SetFileDisableByName(RRF.NAME, RRF.USETYPE, SFCDB);

                string strSql = RRF.GetInsertString(this.DBTYPE);
                strSql = strSql.Replace("':CLOB_FILE'", ":CLOB_FILE");
                strSql = strSql.Replace("':BLOB_FILE'", ":BLOB_FILE");
                System.Data.OleDb.OleDbParameter p = new System.Data.OleDb.OleDbParameter(":BLOB_FILE", System.Data.OleDb.OleDbType.Binary);
                string B64 = Data["Bas64File"].ToString();

                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);
                p.Value = data;
                //new System.Data.OleDb.OleDbParameter(":CLOB_FILE", Data["Bas64File"].ToString()),
                SFCDB.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[]
                { p
                });
                T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_Label RRL = TRL.GetLabelConfigByLabelName(LabelName, SFCDB);
                if (RRL == null)
                {
                    RRL = (Row_R_Label)TRL.NewRow();
                    RRL.ID = TRL.GetNewID(BU, SFCDB);
                    RRL.LABELNAME = LabelName;
                    RRL.R_FILE_NAME = RRF.FILENAME;
                    RRL.PRINTTYPE = PrintType;
                    RRL.ARRYLENGTH = double.Parse(ArryLength);
                    strSql = RRL.GetInsertString(DB_TYPE_ENUM.Oracle);
                }
                else
                {
                    RRL.LABELNAME = LabelName;
                    RRL.R_FILE_NAME = RRF.FILENAME;
                    RRL.PRINTTYPE = PrintType;
                    RRL.ARRYLENGTH = double.Parse(ArryLength);
                    strSql = RRL.GetUpdateString(DB_TYPE_ENUM.Oracle);
                }
                string strRes = SFCDB.ExecSQL(strSql);

                //SFCDB.ExecSQL(RRF.GetInsertString(this.DBTYPE));
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileHisListByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string UseType = Data["UseType"].ToString();
            string FileName = Data["Name"].ToString();
            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_FILE> ret = TRF.GetFileHisList(FileName, UseType, SFCDB);


                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void FileDownLoad(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ID = Data["ID"].ToString();
            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_FILE RRF = (Row_R_FILE)TRF.GetObjByID(ID, SFCDB);
                string filePath = ConfigurationManager.AppSettings["WebFilePath"];
                filePath += "\\" + RRF.FILENAME;
                if (System.IO.File.Exists(filePath  ))
                {
                    System.IO.File.Delete(filePath);
                }

                System.IO.FileStream F = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                byte[] b = (byte[])RRF["BLOB_FILE"];
                F.Write(b, 0, b.Length);
                F.Flush();
                F.Close();
                


                StationReturn.Data = "DOWNLOAD\\"+ RRF.FILENAME;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string UseType = Data["UseType"].ToString();
            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_FILE> ret = TRF.GetFileList(UseType,SFCDB);


                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {
                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                R_FILE R = TRF.GetFileByName(Data["Name"].ToString(),Data["UseType"].ToString(),SFCDB );


                StationReturn.Data = R;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void GetFileUseType(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            try
            {

                List<string> ret = new List<string>();
                ret.Add("LABEL");
                ret.Add("PPL");
                ret.Add("FACK");
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {

                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            //SFCDB.ThrowSqlExeception = false;
            //this.DBPools["SFCDB"].Return(SFCDB);
        }

        public void UpLoadFile(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();

            SFCDB.BeginTrain();
            try
            {

                T_R_FILE TRF = new T_R_FILE(SFCDB, DB_TYPE_ENUM.Oracle);
                
                Row_R_FILE RRF = (Row_R_FILE)TRF.NewRow();
                RRF.ID = TRF.GetNewID(BU, SFCDB);
                RRF.NAME = Data["Name"].ToString();
                RRF.FILENAME = Data["FileName"].ToString();
                RRF.MD5 = Data["MD5"].ToString();
                RRF.USETYPE = Data["UseType"].ToString();
                RRF.STATE = "1";
                RRF.VALID = 1;
                //不使用CLOB字段
                //RRF.CLOB_FILE = ":CLOB_FILE";// Data["Bas64File"].ToString();
                RRF.BLOB_FILE = ":BLOB_FILE";
                RRF.EDIT_EMP = LoginUser.EMP_NO;
                RRF.EDIT_TIME = DateTime.Now;
                SFCDB.ThrowSqlExeception = true;
                //將同類文件改為歷史版本
                TRF.SetFileDisableByName(RRF.NAME, RRF.USETYPE, SFCDB);

                string strSql = RRF.GetInsertString(this.DBTYPE);
                strSql = strSql.Replace("':CLOB_FILE'", ":CLOB_FILE");
                strSql = strSql.Replace("':BLOB_FILE'", ":BLOB_FILE");
                System.Data.OleDb.OleDbParameter p = new System.Data.OleDb.OleDbParameter(":BLOB_FILE", System.Data.OleDb.OleDbType.Binary);
                string B64 = Data["Bas64File"].ToString();
                
                string b64 = B64.Remove(0, B64.LastIndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(b64);
                p.Value = data;
                //new System.Data.OleDb.OleDbParameter(":CLOB_FILE", Data["Bas64File"].ToString()),
                SFCDB.ExecSqlNoReturn(strSql, new System.Data.OleDb.OleDbParameter[] 
                { p
                });

                //SFCDB.ExecSQL(RRF.GetInsertString(this.DBTYPE));
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            SFCDB.ThrowSqlExeception = false;
            this.DBPools["SFCDB"].Return(SFCDB);
        }




    }
}
