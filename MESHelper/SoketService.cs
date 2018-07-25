using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IO;
using System.Collections.Specialized;
using MESHelper.Plugin;
using MESHelper.Common;

namespace MESHelper
{
    public class SocketService : WebSocketBehavior
    {
        public JObject SyncRequest;
        string LabelTempPath = Environment.CurrentDirectory + "\\LabelTemp";
        public static string ZebraPort { get; set; }
        public static string[] PrinterList;
        public static bool isLocalPath { get; set; }
        public static string LocalPath { get; set; }
        public static string ComPortName { get; set; }
        public static int BaudRate { get; set; }
        public static string WeighterType { get; set; }
        public static Reader_COM reader;
        public static MESSocketClient client;
        public static string LocalFile;
        Logs log = new Logs();
        Printer_Codesoft _csl;
        Printer_Zebra _zbl;
        Printer_Doc _Pdf;
        protected override void OnMessage(MessageEventArgs e)
        {
            JObject obj = JsonConvert.DeserializeObject<JObject>(e.Data);
            string MessageID = obj["MessageID"].ToString();
            string ClientID = obj["ClientID"].ToString();
            string msg = "";
            object ResData = null;
            object ReSend;
            string TCode = obj["TCode"].ToString();
            JObject Data = (JObject)obj["Data"];
            LocalFile = getLocalFile(Data["FileName"].ToString());
            int PrintQTY = int.Parse(Data["PrintQTY"].ToString());
            int PrinterIndex = 0;// int.Parse(Data["PrinterIndex"].ToString()) - 1;
            string ExtFileName = LocalFile.Substring(LocalFile.LastIndexOf('.') + 1).ToUpper();
            if (TCode == "PRINT")
            {
                ResData = "Label:" + LocalFile;
                switch (ExtFileName)
                {
                    case "LAB":
                        msg = CodeSoftPrinter(Data, LocalFile, PrinterIndex, PrintQTY);
                        break;
                    case "TXT":
                        msg = ZBLPrinter(Data, LocalFile, PrintQTY) + ",Label:" + LocalFile;
                        break;
                    case "XLS":
                    case "XLSX":
                        msg = ExcelPrinter(Data, LocalFile, PrinterIndex, PrintQTY) + ",Label:" + LocalFile;
                        break;
                    case "PDF":
                        msg = DocPrinter(Data, LocalFile, PrinterIndex, PrintQTY) + ",Label:" + LocalFile;
                        break;
                    default:
                        msg = " 不支持的文件類型!";
                        break;
                }
            }
            else if (TCode == "GETCOMDATA")
            {
                ResData = GetDataFromCOMPort();
            }
            if (msg.Substring(0, 2) != "OK")
            {
                ReSend = new { Status = "Fail", ClientID = ClientID, MessageID = MessageID, Data = ResData, Message = msg };
            }
            else
            {
                ReSend = new { Status = "Pass", ClientID = ClientID, MessageID = MessageID, Data = ResData, Message = msg };
            }
            log.WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + msg);
            Send(JsonConvert.SerializeObject(ReSend));
        }
        string ExcelPrinter(JObject data, string LocalFile,int PrintIndex,int PrintQTY)
        {
            try
            {
                string SaveAsFile = Environment.CurrentDirectory + "\\Temp" + LocalFile.Substring(LocalFile.LastIndexOf("\\"));
                List<object> Params = new List<object> { data, SaveAsFile, LocalFile, SocketService.PrinterList[PrintIndex] };
                byte[] filesByte = File.ReadAllBytes(Path.GetDirectoryName(Application.ExecutablePath) + "//MESHelper.Plugin.dll");
                Assembly assembly = Assembly.Load(filesByte);
                Type type = assembly.GetType("MESHelper.Plugin.Printer_Excel");
                object obj = System.Activator.CreateInstance(type);
                try
                {
                    MethodInfo method = type.GetMethod("Print");
                    method.Invoke(obj, new object[] { Params });
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                return "OK,打印成功";
            }
            catch (Exception)
            {
                return "請檢查目錄是否缺少SfcPrintExcel.dll及安裝Microsoft Office Excel 2003!";
            }
        }
        string CodeSoftPrinter(JObject data, string LocalFile,int PrinterIndex,int PrintQTY)
        {
            try { _csl.close(); }
            catch { }
            try
            {
                _csl = new Printer_Codesoft(LocalFile);
            }
            catch (Exception ex)
            {
                log.WriteLog("打開LAB時出錯:" + ex.Message);
                return "打開LAB時出錯:" + ex.Message;
            }
            _csl.LabValue = data;
            _csl.PrinterName = SocketService.PrinterList[PrinterIndex];
            _csl.Print(PrintQTY);
            _csl.close();
            return "OK,打印成功";
        }
        string ZBLPrinter(JObject data, string LocalFile,int PrintQTY)
        {
            try
            {
                _zbl = new Printer_Zebra();
                _zbl.FilePath = LocalFile;
                _zbl.Port = SocketService.ZebraPort;
                _zbl.LabValue = data;
                _zbl.Print();                
                return "OK,打印成功";
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
                return ex.Message;
            }
        }
        string DocPrinter(JObject data, string LocalFile,int PrinterIndex,int PrintQTY)
        {
            try
            {
                _Pdf = new Printer_Doc(LocalFile);
                _Pdf.PrinterName = SocketService.PrinterList[PrinterIndex];
                _Pdf.Print();
            }
            catch (Exception ex)
            {
                log.WriteLog(ex.Message);
                return ex.Message;
            }
            return "OK,打印成功";
        }
        string GetDataFromCOMPort()
        {
            string _R = "";
            try
            {
                if (reader == null)
                {
                    reader = new Reader_COM(SocketService.ComPortName, SocketService.BaudRate, SocketService.WeighterType);
                }
                if (!reader.COM.IsOpen)
                {
                    reader.Open();
                }
                double _RD = reader.GetWeight();
                _R = "OK," + _RD.ToString();
                return _R;
            }
            catch (Exception ex)
            {
                log.WriteLog("獲取電子稱數據出錯:" + ex.Message);
                return "獲取電子稱數據出錯:" + ex.Message;
            }
        }
        string getLocalFile(string _FileName)
        {
            string LabelTempPath = Environment.CurrentDirectory + "\\LabelTemp";
            string FileName = _FileName;
            if (isLocalPath)
            {
                try
                {
                    string tempPath = "";
                    if (FileName.Contains("/"))
                    {
                        tempPath = FileName.Substring(0, FileName.LastIndexOf("/"));
                        FileName = FileName.Substring(FileName.LastIndexOf("/") + 1);
                    }
                    else if (FileName.Contains("\\"))
                    {
                        tempPath = FileName.Substring(0, FileName.LastIndexOf("\\"));
                        FileName = FileName.Substring(FileName.LastIndexOf("\\") + 1);
                    }
                    if (!File.Exists(LabelTempPath + "\\" + FileName))
                    {
                        //將LAB拷貝到緩存文件夾
                        File.Copy(LocalPath + tempPath + "\\" + FileName, LabelTempPath + "\\" + FileName, true);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(" 複製模板出錯：" + ex.Message);
                }
            }
            else
            {
                client.GetFile(FileName, "LABEL", GetFile_Handler);
            }            
            LocalFile = LabelTempPath + "\\" + LocalFile;
            return LocalFile;
        }
        private void GetFile_Handler(object sender, MessageEventArgs e)
        {
            WebSocket w = (WebSocket)sender;            
            w.OnMessage -= GetFile_Handler;
            JObject Request = (JObject)JsonConvert.DeserializeObject(e.Data);
            if (Request["Status"].ToString() == "Pass")
            {
                JObject Data = (JObject)Request["Data"];
                LocalFile = Data["FILENAME"].ToString();
                string filepath = LabelTempPath + "\\" + Data["FILENAME"].ToString();
                if (System.IO.File.Exists(LabelTempPath))
                {
                    System.IO.File.Delete(LabelTempPath);
                }
                FileStream F = new FileStream(filepath, FileMode.Create);
                byte[] b = (byte[])Data["BLOB_FILE"];
                F.Write(b, 0, b.Length);
                F.Flush();
                F.Close();
            }
            else
            {
                MessageBox.Show(Request["Message"].ToString());
            }
            w.SyncRequest = (JObject)JsonConvert.DeserializeObject(e.Data);
        }
        DataSet DsJsonToDataset(string value)
        {
            StringReader s = new StringReader(value);
            DataSet ds = new DataSet();
            ds.ReadXml(s);
            return ds;
        }
    }
}
