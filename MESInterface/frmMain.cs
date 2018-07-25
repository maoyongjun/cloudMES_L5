using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HWDNNSFCBase;
using System.Reflection;

namespace MESInterface
{
    public partial class frmMain : Form
    {
        public static frmMain Ref;
        DataTable MainDT;
        Dictionary<DataRow, TaskInfo> TasksByDR = new Dictionary<DataRow, TaskInfo>();
        Dictionary<ProcessManagedItem, DataRow> ViewsByPM = new Dictionary<ProcessManagedItem, DataRow>();
        //string configName = @".\config.ini";
        string configName = @".\config_tjl5.ini";
        //string configName = @".\config_vertiv.ini";
        
        TaskInfo CurrentItem;
        public frmMain()
        {
            InitializeComponent();
            MainDT = new DataTable();
            MainDT.Columns.Add("名稱");
            MainDT.Columns.Add("執行時間");
            MainDT.Columns.Add("上次執行開始");
            MainDT.Columns.Add("上次執行結束");
            MainDT.Columns.Add("下次執行");
            MainDT.Columns.Add("運行狀態");
            MainDT.Columns.Add("輸出");
            DGTask.DataSource = MainDT;
            Ref = this;

            
            for (int i = 0; i < DGTask.Columns.Count; i++)
            {
                DGTask.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }
       
       


        private void frmMain_Load(object sender, EventArgs e)
        {
            //加載配置文件
            string strTasks = ConfigFile.ReadIniData("PROGRAM", "TASKS", "", configName);
            string[] TaskList = strTasks.Split(new char[] { ',' });
            for (int i = 0; i < TaskList.Length; i++)
            {
                DataRow dr = MainDT.NewRow();
                dr["名稱"] = ConfigFile.ReadIniData(TaskList[i], "NAME", "", configName);
                dr["執行時間"] = ConfigFile.ReadIniData(TaskList[i], "RUN_TIME", "", configName);
                string TYPE = ConfigFile.ReadIniData(TaskList[i], "TYPE", "", configName);
                //dr["名稱"] = ConfigFile.ReadIniData(TaskList[i], "NAME", "", configName);
                TaskInfo task = new TaskInfo();
                string ClassName = ConfigFile.ReadIniData(TaskList[i], "CLASS", "", configName);
                try
                {
                    Type T = Type.GetType(ClassName);
                    Assembly a1 = Assembly.GetAssembly(T);
                    taskBase taskItem = (taskBase)a1.CreateInstance(ClassName);
                    task.ProccessItem = taskItem;
                }catch(Exception ee )
                {
                    MessageBox.Show("無法創建實體類"+ClassName);
                    continue;
                }

                task.ProccessItem.init(configName, TaskList[i]);

                task.Name = dr["名稱"].ToString();
                task.OutPut = new TabControl();
                task.ViewIten = dr;
                task.OutPut.Dock = DockStyle.Fill;
                initOutputUI(task.OutPut, task.ProccessItem.Output);
                if (TYPE == "BY_TIMESPAN")
                {
                    HWDNNSFCBase.ProcessManagedItem pm = new ProcessManagedItem(task.ProccessItem, Int32.Parse(dr["執行時間"].ToString()));
                    task.Task = pm;
                }
                else if (TYPE == "BY_TIME")
                { 
                    HWDNNSFCBase.ProcessManagedItem pm = new ProcessManagedItem(task.ProccessItem, 99999);
                    task.Task = pm;
                    pm.ManagerType = ProccessManagerTypeEnum.TimeList;
                    string[] TIMES = dr["執行時間"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < TIMES.Length; j++)
                    {
                        try
                        {
                            pm.StartList.Add(TIMES[j]);
                        }
                        catch
                        { }
                    }
                }
                task.Task.RunningStateChange += Task_RunningStateChange;
                TasksByDR.Add(dr, task);
                ViewsByPM.Add(task.Task, dr);
                MainDT.Rows.Add(dr);
            }

        }

        void RefreshTab()
        {
            if (CurrentItem == null)
            {
                return;
            }
            if (CurrentItem.ProccessItem.Output.Tables.Count > 0)
            {
                lockOuput(0);
            }
            else
            {
                this.Refresh();
            }

        }

        void lockOuput(int currIndex)
        {
            lock(CurrentItem.ProccessItem.Output.Tables[currIndex].Rows.SyncRoot)
            {
                if (currIndex == CurrentItem.ProccessItem.Output.Tables.Count - 1)
                {
                    this.Refresh();
                }
                else
                {
                    lockOuput(currIndex + 1);
                }
            }
        }


        delegate void RefreshT(); 
        void Task_RunningStateChange(object sender, RunningStateChangeEventArgs ev)
        {
            HWDNNSFCBase.ProcessManagedItem pm = (ProcessManagedItem)sender;
            DataRow view = ViewsByPM[pm];
            TaskInfo task = TasksByDR[view];
            RefreshT A = new RefreshT(RefreshTab);
            this.Invoke(A);
            //task.OutPut.Refresh();            
            if (ev.Message == "開始執行")
            {
                view["上次執行開始"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                view["上次執行結束"] = "";
            }
            if (ev.Message == "成功執行")
            {
                view["上次執行結束"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                view["下次執行"] = "計算下次執行時間";
                view["輸出"] = "";
            }
            if (ev.Message == "調用Start")
            {
                view["運行狀態"] = "Y";
            }
            if (ev.Message == "調用Stop")
            {
                view["運行狀態"] = "N";
            }
            if (ev.Message.IndexOf("執行出現異常") >= 0)
            {
                view["輸出"] = ev.Message;
            }
            if (ev.Message == "")
            { 
            
            }
        }

        void initOutputUI(TabControl tab, taskOutput output)
        {
            tab.Controls.Clear();
            if (output.UI != null)
            {
                TabPage page = new TabPage();
                page.Text = "UI";

                page.Controls.Add(output.UI);
                output.UI.Dock = DockStyle.Fill;
                tab.Controls.Add(page);

            }

            for (int i = 0; i < output.Tables.Count; i++)
            {
                TabPage page = new TabPage();
                page.Text = output.Tables[i].TableName;
                DataGridView dg = new DataGridView();
                dg.DataSource = output.Tables[i];
                page.Controls.Add(dg);
                dg.Dock = DockStyle.Fill;
                tab.Controls.Add(page);
                dg.ShowCellErrors = false;
                dg.ShowRowErrors = false;
                dg.DataError += Dg_DataError;
            }
        }

        private void Dg_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        
        private void tESTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            
        }

        private void refToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void DGTask_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataRow dr = MainDT.Rows[e.RowIndex];
                CurrentItem = TasksByDR[dr];
                CurrentItem.ProccessItem.UI_Proccess = true;
                System.Threading.Thread.Sleep(100);
                //toolStripLabel1.Text = "任務:" + CurrentItem.ToString();
                程序ToolStripMenuItem.Text = "任務:" + CurrentItem.Name;

                bool reflish = true;
                int refcount = 0;
                while (reflish)
                {
                    try
                    {
                        splitContainer1.Panel2.Controls.Clear();
                        splitContainer1.Panel2.Controls.Add(CurrentItem.OutPut);
                        reflish = false;
                    }
                    catch
                    {
                        refcount++;
                        if (refcount > 10)
                        {
                            break;
                        }
                        continue;
                    }
                }
               
                    
               
                CurrentItem.ProccessItem.UI_Proccess = false;

            }
        }

        private void 程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 立即執行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentItem.ProccessItem.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 全部開始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TaskInfo> l = TasksByDR.Values.ToList();
            for (int i = 0; i < l.Count; i++)
            {
                l[i].Task.Start();
            }
        }

        private void 全部停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<TaskInfo> l = TasksByDR.Values.ToList();
            for (int i = 0; i < l.Count; i++)
            {
                l[i].Task.Stop();
            }
        }

        private void 開始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentItem != null)
            {
                CurrentItem.Task.Start();
            }
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentItem != null)
            {
                CurrentItem.Task.Stop();
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
    public class TaskInfo
    {
        public ProcessManagedItem Task;
        public DataRow ViewIten;
        public TabControl OutPut;
        public taskBase ProccessItem;
        public string Name;
    }
}
