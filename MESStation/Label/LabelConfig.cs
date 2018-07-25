using MESStation.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using Newtonsoft.Json.Linq;

namespace MESStation.Label
{
    public class LabelConfig : MesAPIBase
    {
        private APIInfo _GetLabelConfigBySkuno = new APIInfo()
        {
            FunctionName = "GetLabelConfigBySkuno",
            Description = "獲取SKU的Label配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="Skuno", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            { }
        };
        private APIInfo _AlertLabelConfig = new APIInfo()
        {
            FunctionName = "AlertLabelConfig",
            Description = "增加或SKU的Label配置",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="LabelObject", InputType= "string" , DefaultValue="" }
            },
            Permissions = new List<MESPermission>()
            {
            }
        };

        public LabelConfig()
        {
            Apis.Add(_GetLabelConfigBySkuno.FunctionName, _GetLabelConfigBySkuno);
            Apis.Add(_GetLabelConfigBySkuno.FunctionName, _GetLabelConfigBySkuno);
        }
        public void GetLabelConfigBySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                List<C_SKU_Label> ret = new List<C_SKU_Label>();
                string strSkuno = Data["Skuno"].ToString();
                T_C_SKU_Label TCSL = new T_C_SKU_Label(db, DB_TYPE_ENUM.Oracle);
                List<Row_C_SKU_Label> RCSLS = TCSL.GetLabelConfigBySku(strSkuno, db);
                for (int i = 0; i < RCSLS.Count; i++)
                {
                    ret.Add(RCSLS[i].GetDataObject());
                }
                StationReturn.Data = ret;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }

        public void AlertLabelConfig(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec db = DBPools["SFCDB"].Borrow();
            try
            {
                JToken OBJ = Data["LabelObject"];
                //T_C_PACKING TCP = new T_C_PACKING(db, DB_TYPE_ENUM.Oracle);
                T_C_SKU_Label TCSL = new T_C_SKU_Label(db, DB_TYPE_ENUM.Oracle);
                //判斷ID如果為空則插入,如果不為空則更新
                if (OBJ["ID"].ToString() == "")
                {
                    Row_C_SKU_Label RCKL = (Row_C_SKU_Label)TCSL.NewRow();
                    RCKL.SKUNO = OBJ["SKUNO"].ToString();
                    RCKL.STATION = OBJ["STATION"].ToString();
                    RCKL.SEQ = double.Parse(OBJ["SEQ"].ToString());
                    RCKL.QTY = double.Parse(OBJ["QTY"].ToString());
                    RCKL.LABELNAME = OBJ["LABELNAME"].ToString();
                    RCKL.LABELTYPE = OBJ["LABELTYPE"].ToString();

                    RCKL.EDIT_EMP = LoginUser.EMP_NO;
                    RCKL.EDIT_TIME = DateTime.Now;

                    RCKL.ID = TCSL.GetNewID(BU, db);

                    db.ExecSQL(RCKL.GetInsertString(DB_TYPE_ENUM.Oracle));

                }
                else
                {

                    Row_C_SKU_Label RCKL = (Row_C_SKU_Label)TCSL.GetObjByID(OBJ["ID"].ToString(), db);
                    RCKL.SKUNO = OBJ["SKUNO"].ToString();
                    RCKL.STATION = OBJ["STATION"].ToString();
                    RCKL.SEQ = double.Parse(OBJ["SEQ"].ToString());
                    RCKL.QTY = double.Parse(OBJ["QTY"].ToString());
                    RCKL.LABELNAME = OBJ["LABELNAME"].ToString();
                    RCKL.LABELTYPE = OBJ["LABELTYPE"].ToString();

                    RCKL.EDIT_EMP = LoginUser.EMP_NO;
                    RCKL.EDIT_TIME = DateTime.Now;

                    RCKL.ID = TCSL.GetNewID(BU, db);

                    db.ExecSQL(RCKL.GetUpdateString(DB_TYPE_ENUM.Oracle));
                }
            }
            catch
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(db);
            }
        }
    }
}
