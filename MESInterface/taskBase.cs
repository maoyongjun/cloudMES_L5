using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using HWDNNSFCBase;
using System.Windows.Forms;

namespace MESInterface
{
    public class taskBase:HWDNNSFCBase.Process
    {
        public taskOutput Output = new taskOutput();
        public event EventHandler<taskOutput> OutPutEvent;
        public string configFile;
        public string Section;
        public bool UI_Proccess = false;
        public taskBase()
        {
            OutPutEvent += taskBase_OutPutEvent;
        }
        public void init(string _configFile, string _Section)
        {
            //OutPutEvent += taskBase_OutPutEvent;
            configFile = _configFile;
            Section = _Section;
            init();
        }
        public virtual void init()
        { 
        
        }

        void taskBase_OutPutEvent(object sender, taskOutput e)
        {
            
        }

        public string ConfigGet(string key )
        {
            return ConfigFile.ReadIniData(Section, key, "", configFile);
        }
    }
    public class taskOutput
    {
        public List<DataTable> Tables = new List<DataTable>();
        public List<string> Text = new List<string>();
        public UserControl UI = null;
    }

    public class testTesk : taskBase
    {
        DataTable A = new DataTable();
        public testTesk():base()
        {
            
            A.TableName = "輸出表1";
            A.Columns.Add("C1");
            Output.Tables.Add(A);
            string AA = "dfsfdsfsdfsdfsdf";
            Output.Text.Add(AA);
        }

        public override void init()
        {
            //do ....
            DataRow dr = Output.Tables[0].NewRow();
            dr["C1"] = ConfigFile.ReadIniData(Section, "VAL1", "", configFile);
            Output.Tables[0].Rows.Add(dr);
            dr = Output.Tables[0].NewRow();
            dr["C1"] = ConfigFile.ReadIniData(Section, "VAL2", "", configFile);
            Output.Tables[0].Rows.Add(dr);
        }
        

        public override void Start()
        {
            DataRow dr = Output.Tables[0].NewRow();
            dr["C1"] = DateTime.Now.ToString();
            Output.Tables[0].Rows.Add(dr);

            //throw new Exception("報錯測試");
           

            



        }
    }

}
