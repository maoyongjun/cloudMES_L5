using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;

namespace MESStation.KeyPart
{
    public class KPListBase
    {
        public string SkuNo
        {
            get
            { return value.SKUNO; }
        }

        public string ListName
        {
            get
            { return value.LISTNAME; }
        }

        public DateTime? EditTime
        {
            get
            { return value.EDIT_TIME; }
        }

        public string EditEmp
        {
            get
            { return value.EDIT_EMP; }
        }

        C_KP_LIST value = new C_KP_LIST();
        public List<KPListItem> Item = new List<KPListItem>();
        public static List<KPListBase> GetKPListBySkuNo(string Skuno, MESDBHelper.OleExec SFCDB)
        {
            List<KPListBase> ret = new List<KPListBase>();
            T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> IDS = T.GetListIDBySkuno(Skuno, SFCDB);
            for (int i = 0; i < IDS.Count; i++)
            {
                KPListBase I = new KPListBase(IDS[i], SFCDB);
                ret.Add(I);
            }
            
            return ret;
        }

        public static KPListBase GetKPListByListName(string ListName, MESDBHelper.OleExec SFCDB)
        {
            List<KPListBase> ret = new List<KPListBase>();
            T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            //List<string> IDS = T.GetListIDBySkuno(Skuno, SFCDB);
            string strSql = $@"select ID from c_kp_list where listname='{ListName}'";
            DataSet res = SFCDB.RunSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                return new KPListBase(res.Tables[0].Rows[0]["ID"].ToString(), SFCDB);
            }
            else
            {
                return null;
            }
            

            
        }


        public static List<C_KP_LIST> getAllData(OleExec sfcdb)
        {
            List<C_KP_LIST> ret = new List<C_KP_LIST>();
            T_C_KP_LIST T = new T_C_KP_LIST(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            
            string strSql = "select * from C_KP_LIST order by skuno";
            DataSet res = sfcdb.RunSelect(strSql);
            Row_C_KP_LIST R = (Row_C_KP_LIST)T.NewRow();
            for (int i = 0; i < res.Tables[0].Rows.Count; i++)
            {
                R.loadData(res.Tables[0].Rows[i]);
                ret.Add(R.GetDataObject());
            }
            
            return ret;
        }

        public KPListBase(string ID , OleExec sfcdb)
        {
            T_C_KP_LIST T = new T_C_KP_LIST(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_C_KP_LIST R =(Row_C_KP_LIST)T.GetObjByID(ID, sfcdb);
            if (R != null)
            {
                value = R.GetDataObject();
            }
            else
            {
                throw new Exception($@"查詢不到ID為：{ID} 的KP_LIST");
            }
            List<string> itemID = T.GetItemID(ID, sfcdb);
          
            for (int i = 0; i < itemID.Count; i++)
            {
                KPListItem I = new KPListItem(itemID[i],this ,sfcdb);
                Item.Add(I);
            }
        }

        public void ReMoveFromDB(OleExec sfcdb)
        {
            for (int i = 0; i < Item.Count; i++)
            {
                Item[i].ReMoveFromDB(sfcdb);
            }
            string strSql = $@"delete from  C_KP_List where ID = '{value.ID}'";
            sfcdb.ExecSQL(strSql);
        }

    }

    public class KPListItem
    {
        public string KPPartNo
        {
            get
            { return value.KP_PARTNO; }
        }

        public string KPName
        {
            get
            { return value.KP_NAME; }
        }

        public double? QTY
        {
            get
            { return value.QTY; }
        }

        public double? SEQ
        {
            get
            { return value.SEQ; }
        }

        public string Station
        {
            get
            { return value.STATION; }
        }
        KPListBase KPList = null;

        C_KP_List_Item value;
        public List<KPListDetail> Detail = new List<KPListDetail>();
        public KPListItem(string ID, KPListBase _KPListBase, OleExec sfcdb)
        {
            KPList = _KPListBase;
            T_C_KP_List_Item T = new T_C_KP_List_Item(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_C_KP_List_Item R = (Row_C_KP_List_Item)T.GetObjByID(ID, sfcdb);
            if (R != null)
            {
                value = R.GetDataObject();
            }
            else
            {
                throw new Exception($@"查詢不到ID為：{ID} KP_List_Item");
            }
            List<string> itemID = T.GetItemDetailID(ID, sfcdb);

            for (int i = 0; i < itemID.Count; i++)
            {
                KPListDetail I = new KPListDetail(itemID[i], this,sfcdb);
                Detail.Add(I);
            }
        }
        public void ReMoveFromDB(OleExec sfcdb)
        {
            for (int i = 0; i < Detail.Count; i++)
            {
                Detail[i].ReMoveFromDB(sfcdb);
            }
            string strSql = $@"delete from C_KP_List_Item where ID = '{value.ID}'";
            sfcdb.ExecSQL(strSql);
        }
    }

    public class KPListDetail
    {
        public string SCANTYPE
        {
            get
            { return value.SCANTYPE; }
        }

        public double? SEQ
        {
            get
            { return value.SEQ; }
        }

        KPListItem ListItem = null;
        C_KP_List_Item_Detail value;
        public KPListDetail(string ID, KPListItem _Item, OleExec sfcdb)
        {
            ListItem = _Item;
            T_C_KP_List_Item_Detail T = new T_C_KP_List_Item_Detail(sfcdb, MESDataObject.DB_TYPE_ENUM.Oracle);
            Row_C_KP_List_Item_Detail R = (Row_C_KP_List_Item_Detail)T.GetObjByID(ID, sfcdb);
            if (R != null)
            {
                value = R.GetDataObject();
            }
            else
            {
                throw new Exception($@"查詢不到ID為：{ID} KP_List_Item_Detail");
            }
        }
        public void ReMoveFromDB(OleExec sfcdb)
        {
            string strSql = $@"delete from C_KP_List_Item_Detail where ID = '{value.ID}'";
            sfcdb.ExecSQL(strSql);
        }

    }

}
