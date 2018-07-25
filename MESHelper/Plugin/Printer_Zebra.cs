using System;
using System.Data;
using System.IO;
using MESHelper.Common;
using Newtonsoft.Json.Linq;

namespace MESHelper.Plugin
{
    public class Printer_Zebra
    {
        public string FilePath { get; set; }
        public string Port { get; set; }//配置窗口會傳入端口值
        Random rand = new Random();
        JObject _labValue;

        public JObject LabValue {
            set {
                _labValue = value;
            }
        }
        public void Print()
        {
            Logs log = new Logs();
            StreamReader R;
            R = new StreamReader(FilePath);
            string temp = R.ReadToEnd();            
            string temp1 = temp;
            JArray data = (JArray)_labValue["Outputs"];
            foreach (var dc in data)
            {
                string Name = dc["Name"].ToString();
                string Type = dc["Type"].ToString();
                string ItemName = "";
                if (Type == "0")
                {
                    ItemName = "@" + Name + "@";
                    try
                    {
                        temp1 = temp1.Replace(ItemName, dc["Value"].ToString());
                    }
                    catch
                    {
                    }
                }
                else
                {
                    JArray Values = (JArray)dc["Value"];
                    for (int i = 0; i < Values.Count; i++)
                    {
                        ItemName = "@" + Name + (i + 1).ToString() + "@";
                        try
                        {
                            temp1 = temp1.Replace(ItemName, Values[i].ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
            try
            {
                temp1 = temp1.Replace("@PAGE@", _labValue["PAGE"].ToString());
                temp1 = temp1.Replace("@ALLPAGE@", _labValue["ALLPAGE"].ToString());
            }
            catch
            {
            }
            if (!Directory.Exists("c:\\PRINTTEXT"))
            {
                System.IO.Directory.CreateDirectory("c:\\PRINTTEXT");
            }
            string PrintFileName = log.WritePrintFile(temp1);
            try
            {
                string cmd = "Copy \"" + PrintFileName + "\" " + Port;
                string BatFilePath = log.WriteFile("c:\\PRINTTEXT\\printcom.bat", cmd);
                Microsoft.VisualBasic.Interaction.Shell(BatFilePath, Microsoft.VisualBasic.AppWinStyle.Hide, true, 1000);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
