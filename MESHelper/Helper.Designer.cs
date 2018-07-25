namespace MESHelper
{
    partial class Helper
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Helper));
            this.bnt_SaveSetting = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_tx = new System.Windows.Forms.TabPage();
            this.cmb_bu = new System.Windows.Forms.ComboBox();
            this.lab_bu = new System.Windows.Forms.Label();
            this.lab_TestServer = new System.Windows.Forms.Label();
            this.txt_Password = new System.Windows.Forms.TextBox();
            this.txt_UserName = new System.Windows.Forms.TextBox();
            this.txt_MESServer = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.lab_UserName = new System.Windows.Forms.Label();
            this.lab_MESServer = new System.Windows.Forms.Label();
            this.txtServicePort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tab_print = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.cb_isLocalPath = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.bnt_localPathChose = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtLocalPath = new System.Windows.Forms.TextBox();
            this.cbx_Printer4 = new System.Windows.Forms.ComboBox();
            this.cbx_Printer1 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ZebraPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbx_Printer3 = new System.Windows.Forms.ComboBox();
            this.cbx_Printer2 = new System.Windows.Forms.ComboBox();
            this.tab_comport = new System.Windows.Forms.TabPage();
            this.cbx_comportlist = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_BaudRate = new System.Windows.Forms.TextBox();
            this.cbx_WeighterType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bnt_Stop = new System.Windows.Forms.Button();
            this.lab_status = new System.Windows.Forms.Label();
            this.bnt_run = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.MenuStrip_RightBnt = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ts_Option = new System.Windows.Forms.ToolStripMenuItem();
            this.ts_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.ts_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.ts_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tab_tx.SuspendLayout();
            this.tab_print.SuspendLayout();
            this.tab_comport.SuspendLayout();
            this.MenuStrip_RightBnt.SuspendLayout();
            this.SuspendLayout();
            // 
            // bnt_SaveSetting
            // 
            this.bnt_SaveSetting.Location = new System.Drawing.Point(317, 294);
            this.bnt_SaveSetting.Name = "bnt_SaveSetting";
            this.bnt_SaveSetting.Size = new System.Drawing.Size(75, 23);
            this.bnt_SaveSetting.TabIndex = 28;
            this.bnt_SaveSetting.Text = "保存設置";
            this.bnt_SaveSetting.UseVisualStyleBackColor = true;
            this.bnt_SaveSetting.Click += new System.EventHandler(this.bnt_SaveSetting_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tab_tx);
            this.tabControl1.Controls.Add(this.tab_print);
            this.tabControl1.Controls.Add(this.tab_comport);
            this.tabControl1.Location = new System.Drawing.Point(11, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(409, 254);
            this.tabControl1.TabIndex = 27;
            // 
            // tab_tx
            // 
            this.tab_tx.Controls.Add(this.cmb_bu);
            this.tab_tx.Controls.Add(this.lab_bu);
            this.tab_tx.Controls.Add(this.lab_TestServer);
            this.tab_tx.Controls.Add(this.txt_Password);
            this.tab_tx.Controls.Add(this.txt_UserName);
            this.tab_tx.Controls.Add(this.txt_MESServer);
            this.tab_tx.Controls.Add(this.label15);
            this.tab_tx.Controls.Add(this.lab_UserName);
            this.tab_tx.Controls.Add(this.lab_MESServer);
            this.tab_tx.Controls.Add(this.txtServicePort);
            this.tab_tx.Controls.Add(this.label1);
            this.tab_tx.Location = new System.Drawing.Point(4, 22);
            this.tab_tx.Name = "tab_tx";
            this.tab_tx.Padding = new System.Windows.Forms.Padding(3);
            this.tab_tx.Size = new System.Drawing.Size(401, 228);
            this.tab_tx.TabIndex = 0;
            this.tab_tx.Text = "通訊設置";
            this.tab_tx.UseVisualStyleBackColor = true;
            // 
            // cmb_bu
            // 
            this.cmb_bu.FormattingEnabled = true;
            this.cmb_bu.Location = new System.Drawing.Point(91, 63);
            this.cmb_bu.Name = "cmb_bu";
            this.cmb_bu.Size = new System.Drawing.Size(121, 20);
            this.cmb_bu.TabIndex = 13;
            // 
            // lab_bu
            // 
            this.lab_bu.AutoSize = true;
            this.lab_bu.Location = new System.Drawing.Point(61, 67);
            this.lab_bu.Name = "lab_bu";
            this.lab_bu.Size = new System.Drawing.Size(24, 12);
            this.lab_bu.TabIndex = 12;
            this.lab_bu.Text = "BU:";
            // 
            // lab_TestServer
            // 
            this.lab_TestServer.AutoSize = true;
            this.lab_TestServer.Location = new System.Drawing.Point(91, 207);
            this.lab_TestServer.Name = "lab_TestServer";
            this.lab_TestServer.Size = new System.Drawing.Size(0, 12);
            this.lab_TestServer.TabIndex = 11;
            // 
            // txt_Password
            // 
            this.txt_Password.Location = new System.Drawing.Point(91, 172);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.PasswordChar = '*';
            this.txt_Password.Size = new System.Drawing.Size(140, 22);
            this.txt_Password.TabIndex = 9;
            this.txt_Password.Text = "TEST";
            this.txt_Password.UseSystemPasswordChar = true;
            // 
            // txt_UserName
            // 
            this.txt_UserName.Location = new System.Drawing.Point(91, 135);
            this.txt_UserName.Name = "txt_UserName";
            this.txt_UserName.Size = new System.Drawing.Size(140, 22);
            this.txt_UserName.TabIndex = 8;
            this.txt_UserName.Text = "TEST";
            // 
            // txt_MESServer
            // 
            this.txt_MESServer.Location = new System.Drawing.Point(91, 98);
            this.txt_MESServer.Name = "txt_MESServer";
            this.txt_MESServer.Size = new System.Drawing.Size(275, 22);
            this.txt_MESServer.TabIndex = 7;
            this.txt_MESServer.Text = "10.120.115.142:2130";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(34, 175);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(51, 12);
            this.label15.TabIndex = 6;
            this.label15.Text = "Password:";
            // 
            // lab_UserName
            // 
            this.lab_UserName.AutoSize = true;
            this.lab_UserName.Location = new System.Drawing.Point(26, 139);
            this.lab_UserName.Name = "lab_UserName";
            this.lab_UserName.Size = new System.Drawing.Size(59, 12);
            this.lab_UserName.TabIndex = 5;
            this.lab_UserName.Text = "User Name:";
            // 
            // lab_MESServer
            // 
            this.lab_MESServer.AutoSize = true;
            this.lab_MESServer.Location = new System.Drawing.Point(21, 103);
            this.lab_MESServer.Name = "lab_MESServer";
            this.lab_MESServer.Size = new System.Drawing.Size(64, 12);
            this.lab_MESServer.TabIndex = 4;
            this.lab_MESServer.Text = "MES Server:";
            // 
            // txtServicePort
            // 
            this.txtServicePort.Location = new System.Drawing.Point(91, 26);
            this.txtServicePort.Name = "txtServicePort";
            this.txtServicePort.Size = new System.Drawing.Size(55, 22);
            this.txtServicePort.TabIndex = 2;
            this.txtServicePort.Text = "2600";
            this.txtServicePort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_BaudRate_KeyPress);
            this.txtServicePort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtServicePort_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Local Port:";
            // 
            // tab_print
            // 
            this.tab_print.Controls.Add(this.label14);
            this.tab_print.Controls.Add(this.cb_isLocalPath);
            this.tab_print.Controls.Add(this.label13);
            this.tab_print.Controls.Add(this.bnt_localPathChose);
            this.tab_print.Controls.Add(this.label12);
            this.tab_print.Controls.Add(this.label3);
            this.tab_print.Controls.Add(this.label11);
            this.tab_print.Controls.Add(this.txtLocalPath);
            this.tab_print.Controls.Add(this.cbx_Printer4);
            this.tab_print.Controls.Add(this.cbx_Printer1);
            this.tab_print.Controls.Add(this.label10);
            this.tab_print.Controls.Add(this.label2);
            this.tab_print.Controls.Add(this.txt_ZebraPort);
            this.tab_print.Controls.Add(this.label7);
            this.tab_print.Controls.Add(this.label9);
            this.tab_print.Controls.Add(this.label8);
            this.tab_print.Controls.Add(this.cbx_Printer3);
            this.tab_print.Controls.Add(this.cbx_Printer2);
            this.tab_print.Location = new System.Drawing.Point(4, 22);
            this.tab_print.Name = "tab_print";
            this.tab_print.Padding = new System.Windows.Forms.Padding(3);
            this.tab_print.Size = new System.Drawing.Size(401, 228);
            this.tab_print.TabIndex = 1;
            this.tab_print.Text = "打印機設置";
            this.tab_print.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label14.Location = new System.Drawing.Point(218, 173);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(170, 12);
            this.label14.TabIndex = 35;
            this.label14.Text = "PALLET LIST列印到四號打印機";
            // 
            // cb_isLocalPath
            // 
            this.cb_isLocalPath.AutoSize = true;
            this.cb_isLocalPath.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_isLocalPath.Location = new System.Drawing.Point(17, 22);
            this.cb_isLocalPath.Name = "cb_isLocalPath";
            this.cb_isLocalPath.Size = new System.Drawing.Size(120, 16);
            this.cb_isLocalPath.TabIndex = 12;
            this.cb_isLocalPath.Text = "使用本地模板路徑";
            this.cb_isLocalPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cb_isLocalPath.UseVisualStyleBackColor = true;
            this.cb_isLocalPath.Click += new System.EventHandler(this.cb_isLocalPath_CheckStateChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label13.Location = new System.Drawing.Point(218, 145);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(155, 12);
            this.label13.TabIndex = 34;
            this.label13.Text = "PALLET標籤列印到三號標籤";
            // 
            // bnt_localPathChose
            // 
            this.bnt_localPathChose.Location = new System.Drawing.Point(308, 50);
            this.bnt_localPathChose.Name = "bnt_localPathChose";
            this.bnt_localPathChose.Size = new System.Drawing.Size(75, 23);
            this.bnt_localPathChose.TabIndex = 10;
            this.bnt_localPathChose.Text = "選擇";
            this.bnt_localPathChose.UseVisualStyleBackColor = true;
            this.bnt_localPathChose.Click += new System.EventHandler(this.bnt_localPathChose_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label12.Location = new System.Drawing.Point(218, 117);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(172, 12);
            this.label12.TabIndex = 33;
            this.label12.Text = "CARTON標籤列印到二號打印機";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "本地模板:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label11.Location = new System.Drawing.Point(218, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(173, 12);
            this.label11.TabIndex = 32;
            this.label11.Text = "一般標籤默認列印到一號打印機";
            // 
            // txtLocalPath
            // 
            this.txtLocalPath.Location = new System.Drawing.Point(91, 50);
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.Size = new System.Drawing.Size(196, 22);
            this.txtLocalPath.TabIndex = 8;
            // 
            // cbx_Printer4
            // 
            this.cbx_Printer4.FormattingEnabled = true;
            this.cbx_Printer4.Location = new System.Drawing.Point(91, 171);
            this.cbx_Printer4.Name = "cbx_Printer4";
            this.cbx_Printer4.Size = new System.Drawing.Size(121, 20);
            this.cbx_Printer4.TabIndex = 31;
            this.cbx_Printer4.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer4_SelectedIndexChanged);
            // 
            // cbx_Printer1
            // 
            this.cbx_Printer1.FormattingEnabled = true;
            this.cbx_Printer1.Location = new System.Drawing.Point(91, 84);
            this.cbx_Printer1.Name = "cbx_Printer1";
            this.cbx_Printer1.Size = new System.Drawing.Size(121, 20);
            this.cbx_Printer1.TabIndex = 21;
            this.cbx_Printer1.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer1_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(17, 175);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 12);
            this.label10.TabIndex = 30;
            this.label10.Text = "四號打印機:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "一號打印機:";
            // 
            // txt_ZebraPort
            // 
            this.txt_ZebraPort.Location = new System.Drawing.Point(106, 197);
            this.txt_ZebraPort.Name = "txt_ZebraPort";
            this.txt_ZebraPort.Size = new System.Drawing.Size(45, 22);
            this.txt_ZebraPort.TabIndex = 28;
            this.txt_ZebraPort.Text = "LPT1";
            this.txt_ZebraPort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_ZebraPort_KeyUp);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 12);
            this.label7.TabIndex = 23;
            this.label7.Text = "二號打印機:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 201);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 12);
            this.label9.TabIndex = 27;
            this.label9.Text = "Zebra打印端口:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 146);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 12);
            this.label8.TabIndex = 24;
            this.label8.Text = "三號打印機:";
            // 
            // cbx_Printer3
            // 
            this.cbx_Printer3.FormattingEnabled = true;
            this.cbx_Printer3.Location = new System.Drawing.Point(91, 142);
            this.cbx_Printer3.Name = "cbx_Printer3";
            this.cbx_Printer3.Size = new System.Drawing.Size(121, 20);
            this.cbx_Printer3.TabIndex = 26;
            this.cbx_Printer3.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer3_SelectedIndexChanged);
            // 
            // cbx_Printer2
            // 
            this.cbx_Printer2.FormattingEnabled = true;
            this.cbx_Printer2.Location = new System.Drawing.Point(91, 113);
            this.cbx_Printer2.Name = "cbx_Printer2";
            this.cbx_Printer2.Size = new System.Drawing.Size(121, 20);
            this.cbx_Printer2.TabIndex = 25;
            this.cbx_Printer2.SelectedIndexChanged += new System.EventHandler(this.cbx_Printer2_SelectedIndexChanged);
            // 
            // tab_comport
            // 
            this.tab_comport.Controls.Add(this.cbx_comportlist);
            this.tab_comport.Controls.Add(this.label5);
            this.tab_comport.Controls.Add(this.txt_BaudRate);
            this.tab_comport.Controls.Add(this.cbx_WeighterType);
            this.tab_comport.Controls.Add(this.label6);
            this.tab_comport.Controls.Add(this.label4);
            this.tab_comport.Location = new System.Drawing.Point(4, 22);
            this.tab_comport.Name = "tab_comport";
            this.tab_comport.Size = new System.Drawing.Size(401, 228);
            this.tab_comport.TabIndex = 2;
            this.tab_comport.Text = "COM口設置";
            this.tab_comport.UseVisualStyleBackColor = true;
            // 
            // cbx_comportlist
            // 
            this.cbx_comportlist.FormattingEnabled = true;
            this.cbx_comportlist.Location = new System.Drawing.Point(79, 21);
            this.cbx_comportlist.Name = "cbx_comportlist";
            this.cbx_comportlist.Size = new System.Drawing.Size(121, 20);
            this.cbx_comportlist.TabIndex = 19;
            this.cbx_comportlist.SelectedIndexChanged += new System.EventHandler(this.cbx_comportlist_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "波特率:";
            // 
            // txt_BaudRate
            // 
            this.txt_BaudRate.Location = new System.Drawing.Point(79, 51);
            this.txt_BaudRate.MaxLength = 5;
            this.txt_BaudRate.Name = "txt_BaudRate";
            this.txt_BaudRate.Size = new System.Drawing.Size(121, 22);
            this.txt_BaudRate.TabIndex = 16;
            this.txt_BaudRate.Text = "9600";
            this.txt_BaudRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_BaudRate_KeyPress);
            this.txt_BaudRate.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_BaudRate_KeyUp);
            // 
            // cbx_WeighterType
            // 
            this.cbx_WeighterType.FormattingEnabled = true;
            this.cbx_WeighterType.Items.AddRange(new object[] {
            "穩定傳輸",
            "連續傳輸",
            "帶ST標記"});
            this.cbx_WeighterType.Location = new System.Drawing.Point(79, 80);
            this.cbx_WeighterType.Name = "cbx_WeighterType";
            this.cbx_WeighterType.Size = new System.Drawing.Size(121, 20);
            this.cbx_WeighterType.TabIndex = 17;
            this.cbx_WeighterType.SelectedIndexChanged += new System.EventHandler(this.cbx_WeighterType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "傳輸類型:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "連接端口:";
            // 
            // bnt_Stop
            // 
            this.bnt_Stop.Location = new System.Drawing.Point(195, 294);
            this.bnt_Stop.Name = "bnt_Stop";
            this.bnt_Stop.Size = new System.Drawing.Size(75, 23);
            this.bnt_Stop.TabIndex = 26;
            this.bnt_Stop.Text = "Stop";
            this.bnt_Stop.UseVisualStyleBackColor = true;
            this.bnt_Stop.Click += new System.EventHandler(this.bnt_Stop_Click);
            // 
            // lab_status
            // 
            this.lab_status.AutoSize = true;
            this.lab_status.Font = new System.Drawing.Font("新細明體", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_status.ForeColor = System.Drawing.Color.Black;
            this.lab_status.Location = new System.Drawing.Point(11, 282);
            this.lab_status.Name = "lab_status";
            this.lab_status.Size = new System.Drawing.Size(155, 35);
            this.lab_status.TabIndex = 25;
            this.lab_status.Text = "開始偵聽";
            // 
            // bnt_run
            // 
            this.bnt_run.Location = new System.Drawing.Point(195, 294);
            this.bnt_run.Name = "bnt_run";
            this.bnt_run.Size = new System.Drawing.Size(75, 23);
            this.bnt_run.TabIndex = 24;
            this.bnt_run.Text = "Run";
            this.bnt_run.UseVisualStyleBackColor = true;
            this.bnt_run.Click += new System.EventHandler(this.bnt_run_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.MenuStrip_RightBnt;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Socket Serice";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // MenuStrip_RightBnt
            // 
            this.MenuStrip_RightBnt.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ts_Option,
            this.ts_Start,
            this.ts_Stop,
            this.ts_Exit});
            this.MenuStrip_RightBnt.Name = "MenuStrip_RightBnt";
            this.MenuStrip_RightBnt.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.MenuStrip_RightBnt.Size = new System.Drawing.Size(101, 92);
            this.MenuStrip_RightBnt.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuStrip_RightBnt_ItemClicked);
            // 
            // ts_Option
            // 
            this.ts_Option.Name = "ts_Option";
            this.ts_Option.Size = new System.Drawing.Size(100, 22);
            this.ts_Option.Text = "設置";
            // 
            // ts_Start
            // 
            this.ts_Start.Name = "ts_Start";
            this.ts_Start.Size = new System.Drawing.Size(100, 22);
            this.ts_Start.Text = "開始";
            // 
            // ts_Stop
            // 
            this.ts_Stop.Name = "ts_Stop";
            this.ts_Stop.Size = new System.Drawing.Size(100, 22);
            this.ts_Stop.Text = "停止";
            // 
            // ts_Exit
            // 
            this.ts_Exit.Name = "ts_Exit";
            this.ts_Exit.Size = new System.Drawing.Size(100, 22);
            this.ts_Exit.Text = "退出";
            // 
            // Helper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 331);
            this.Controls.Add(this.bnt_SaveSetting);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lab_status);
            this.Controls.Add(this.bnt_Stop);
            this.Controls.Add(this.bnt_run);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Helper";
            this.Text = "Mes Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HelperForm_FormClosing);
            this.SizeChanged += new System.EventHandler(this.HelperForm_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tab_tx.ResumeLayout(false);
            this.tab_tx.PerformLayout();
            this.tab_print.ResumeLayout(false);
            this.tab_print.PerformLayout();
            this.tab_comport.ResumeLayout(false);
            this.tab_comport.PerformLayout();
            this.MenuStrip_RightBnt.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnt_SaveSetting;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_tx;
        private System.Windows.Forms.TextBox txtServicePort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tab_print;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cb_isLocalPath;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button bnt_localPathChose;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtLocalPath;
        private System.Windows.Forms.ComboBox cbx_Printer4;
        private System.Windows.Forms.ComboBox cbx_Printer1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_ZebraPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbx_Printer3;
        private System.Windows.Forms.ComboBox cbx_Printer2;
        private System.Windows.Forms.TabPage tab_comport;
        private System.Windows.Forms.ComboBox cbx_comportlist;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_BaudRate;
        private System.Windows.Forms.ComboBox cbx_WeighterType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bnt_Stop;
        private System.Windows.Forms.Label lab_status;
        private System.Windows.Forms.Button bnt_run;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        public System.Windows.Forms.ContextMenuStrip MenuStrip_RightBnt;
        private System.Windows.Forms.ToolStripMenuItem ts_Option;
        private System.Windows.Forms.ToolStripMenuItem ts_Start;
        private System.Windows.Forms.ToolStripMenuItem ts_Stop;
        private System.Windows.Forms.ToolStripMenuItem ts_Exit;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label lab_UserName;
        private System.Windows.Forms.Label lab_MESServer;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_Password;
        private System.Windows.Forms.TextBox txt_UserName;
        private System.Windows.Forms.TextBox txt_MESServer;
        private System.Windows.Forms.Label lab_TestServer;
        private System.Windows.Forms.ComboBox cmb_bu;
        private System.Windows.Forms.Label lab_bu;
    }
}

