using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace MESHelper.Plugin
{
    public class Reader_COM
    {
        double Weight = 0;
        public SerialPort COM;
        public delegate void ComDataDelegate(object sender, string Mesg);
        public event ComDataDelegate ComDataEvent;
        public string WeighterType { get; set; }
        public string strCom { get; set; }
        public int BaudRate { get; set; }
        public bool ReadCom { get; set; }
        string[] OLDSTRING = new string[4]; 
        bool TYPE2 = false;
        public Reader_COM(string _ComPortName,int _BaudRate,string _WeighterType)
        {
            COM = new SerialPort();
            strCom = _ComPortName;
            BaudRate = _BaudRate;
            WeighterType = _WeighterType;
        }
        public void Open()
        {
            for (int i = 0; i < OLDSTRING.Length; i++)
            {
                OLDSTRING[i] = "0";
            }
            ComDataEvent += new ComDataDelegate(SetWeightEvent);
            COM.PortName = strCom;
            COM.Handshake = Handshake.RequestToSend;
            COM.DataReceived += new SerialDataReceivedEventHandler(COM_DataReceived_GetSamples);
            COM.BaudRate = BaudRate;
            try
            {
                ReadCom = true;
                COM.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public double GetWeight()
        {
            return Weight;
        }
        public void close()
        {
            ReadCom = false;
            System.Threading.Thread.Sleep(1000);
            try
            {
                COM.DiscardInBuffer();
                COM.DiscardOutBuffer();
                COM.Dispose();
            }
            catch
            {
            }
            try
            {
                COM.DataReceived -= new SerialDataReceivedEventHandler(COM_DataReceived_GetSamples);                
            }
            catch
            { }
            try
            {
                ComDataEvent -= new ComDataDelegate(SetWeightEvent);
            }
            catch
            { }
            try
            {
                COM.Handshake = Handshake.None;
                COM.Close();
            }
            catch
            { }
        }
        void SetWeightEvent(object sender, string Mesg)
        {
            double sample = 0;
            string[] tmp;// = Mesg.Split(new char[]{','});
            if (WeighterType == "1")
            {
                tmp = Mesg.Split(new char[] { ',' });
                try
                {
                    tmp[2] = tmp[2].Replace("+", "");
                    tmp[2] = tmp[2].Replace("g", "");
                    tmp[2] = tmp[2].Replace(" ", "");
                    sample = double.Parse(tmp[2]);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else if (WeighterType == "2")
            {
                Mesg = Mesg.Replace(" ", "");
                Mesg = Mesg.Replace("?", "");
                tmp = Mesg.Split(new char[] { '\r' });
                tmp[0] = tmp[0].Substring(0, tmp[0].Length - 1);
                if (tmp[0] == "0")
                {
                    OLDSTRING[0] = OLDSTRING[1];
                    OLDSTRING[1] = OLDSTRING[2];
                    OLDSTRING[2] = OLDSTRING[3];
                    OLDSTRING[3] = tmp[0];
                    TYPE2 = false;
                    return;
                }
                else
                {

                }
                if (TYPE2)
                {
                    return;
                }
                if (tmp[0] == OLDSTRING[0] &&
                    tmp[0] == OLDSTRING[1] &&
                    tmp[0] == OLDSTRING[2] &&
                    tmp[0] == OLDSTRING[3])
                {
                    try
                    {
                        sample = double.Parse(tmp[0]);
                    }
                    catch
                    {
                        return;
                    }
                    sample = sample / 10;
                    TYPE2 = true;
                }
                else
                {
                    OLDSTRING[0] = OLDSTRING[1];
                    OLDSTRING[1] = OLDSTRING[2];
                    OLDSTRING[2] = OLDSTRING[3];
                    OLDSTRING[3] = tmp[0];
                    return;
                }

            }
            else if (WeighterType == "3")
            {
                //SU,NT,+   241.0g   不穩定
                //ST,NT,+   241.0g   穩定
                //ST TR +00.0003kg
                //ST TR -00.0006kg
                //ST TR -00000.3 g
                //ST TR +00000.6 g



                try
                {
                    string[] val = Mesg.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    foreach (string item in val)
                    {
                        string headStr = item.Substring(0, 2);
                        if (headStr.Equals("ST") && item.EndsWith("g"))
                        {
                            string weightStr = item.Substring(6, 8).Trim();
                            weightStr = weightStr.Replace("+", "").Replace("-", "").Trim();
                            double w = Convert.ToDouble(weightStr);
                            sample = w;
                            break;
                        }
                    }
                }
                catch 
                {
                    return;
                }                
            }
            Weight = sample;
            //ComDataEvent -= new ComDataDelegate(SetWeightEvent);
        }
        void COM_DataReceived_GetSamples(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                char[] buffer = new char[100];
                if (ReadCom)
                {
                    if (e.EventType == SerialData.Chars)
                    {
                        System.Threading.Thread.Sleep(200);
                        COM.Read(buffer, 0, 100);

                        string s = "";
                        int i = 0;
                        while (buffer[i] != '\0')
                        {
                            s += buffer[i].ToString();
                            i++;
                            if (i == 99)
                            {
                                break;
                            }
                        }
                        ComDataEvent(this, s);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
