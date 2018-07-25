using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using WebServer.SocketService;
using System.Threading;



namespace WebServer_Console
{
    class Program
    {
        static WebSocketServer SFCSocket;
        static string format = "ClientCount:{0} || Client:{1}:{2} || ID:{3} >> Message:{3}";
        static void Main(string[] args)
        {
            DateTime StartTime = DateTime.Now;
            Thread T = new Thread(new ThreadStart(ServiceStart));

            T.Start();
            while (true)
            {
                string M = Console.ReadLine();
                Console.WriteLine("SFCDB:" + Report.SFCDBPool.PoolRemain.ToString() + " borrow:" + Report.SFCDBPool.PoolBorrowed.ToString());
                Console.WriteLine($@"SFCDB lock state { Report.SFCDBPool.lockState}");
                Console.WriteLine("Login:" + Report.LoginUsers.Count.ToString());
                Console.WriteLine("StartTime:" + StartTime.ToString() + " Run:" + (DateTime.Now - StartTime).TotalSeconds);
                if (M.ToUpper() == "USERS")
                {
                    showLogusers();
                }
                if (M.ToUpper().StartsWith("KILL"))
                {
                    string[] a = M.Split(new char[] { ' ', ',' });
                    foreach (string t in a)
                    {
                        killLogusers(t);
                       
                    }
                }
                if (M.ToUpper().StartsWith("UNLOCK"))
                {
                    Report.SFCDBPool.UNLock();
                    Console.WriteLine($@"SFCDB lock state { Report.SFCDBPool.lockState}");
                }
                if (M.ToUpper().StartsWith("LEND"))
                {
                    List<string> l = Report.SFCDBPool.ShowLend();
                    Console.WriteLine("Lend count:" + l.Count.ToString());
                    //Console.WriteLine($@"SFCDB lock state { Report.SFCDBPool.lockState}");
                    foreach (string t in l)
                    {
                        Console.WriteLine(t);
                    }
                }
                if (M.ToUpper().StartsWith("POOL"))
                {
                    List<string> l = Report.SFCDBPool.OutPutMessage;
                    Console.WriteLine("OUT:" + l.Count.ToString());
                    //Console.WriteLine($@"SFCDB lock state { Report.SFCDBPool.lockState}");
                    foreach (string t in l)
                    {
                        Console.WriteLine(t);
                    }
                }

                if (M.ToUpper().StartsWith("CLPOOL"))
                {
                    Report.SFCDBPool.CleanAll();
                    Console.WriteLine("SFCDBPool.CleanAll()");
                }
                if (M.ToUpper().StartsWith("SELECT"))
                {
                    MESDBHelper.OleExec db = null;
                    try
                    {
                        db = Report.SFCDBPool.Borrow();
                        System.Data.DataSet res = db.RunSelect(M);
                        for (int i = 0; i < res.Tables[0].Columns.Count; i++)
                        {
                            Console.Write(res.Tables[0].Columns[i].ColumnName+"\t");
                        }
                        Console.Write("\r\n");
                        for (int i = 0; i < res.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < res.Tables[0].Columns.Count; j++)
                            {
                                Console.Write(res.Tables[0].Rows[i][ res.Tables[0].Columns[j].ColumnName].ToString() + "\t");
                            }
                            Console.Write("\r\n");
                        }

                    }
                    catch (Exception ee)
                    {
                        Console.Write(ee.Message);
                        Console.WriteLine("");
                    }
                    if (db != null)
                    {
                        try
                        {
                            Report.SFCDBPool.Return(db);
                        }
                        catch (Exception ee)
                        {
                            Console.Write("Return db:"+ ee.Message);
                            Console.WriteLine("");
                        }
                    }

                }


                if (M.ToUpper().StartsWith("HELP"))
                {
                    Console.WriteLine("KILL [Token]");
                    Console.WriteLine("UNLOCK --Set SfcDBPool lock State false");
                    Console.WriteLine("LEND --Show SfcDBPool Borrows");
                    Console.WriteLine("POOL --Show SfcDBPool OutPutMessage");
                    Console.WriteLine("CLPOOL --Clean SfcDBPool All Table");
                    Console.WriteLine("SELECT --Run select");
                }

            }
            
        }

        static void ServiceStart()
        {
            Console.WriteLine("Start");
            SFCSocket = new WebSocketServer(System.Net.IPAddress.Any, 2130);
            SFCSocket.Log.Level = WebSocketSharp.LogLevel.Error;
            SFCSocket.WaitTime = TimeSpan.FromSeconds(2);
            SFCSocket.AddWebSocketService<Report>("/ReportService", new Func<Report>(NewReportService));
            
            SFCSocket.Start();
        }


        static void killLogusers(string Token)
        {
            if (Report.LoginUsers.ContainsKey(Token))
            {
                Report.LoginUsers.Remove(Token);
                MESStation.Stations.CallStation.logout(Token);

                Console.WriteLine($@"Token:{Token} have killed");
            }
            else
            {
                Console.WriteLine($@"Token:{Token} not find");
            }
        }



        static void showLogusers()
        {
            List<MESStation.LogicObject.User> u = Report.LoginUsers.Values.ToList();
            List<string> K = Report.LoginUsers.Keys.ToList();
            for (int i = 0; i < K.Count; i++)
            {
                string k = K[i];
                Console.WriteLine($@"{k}, {Report.LoginUsers[k].EMP_NO}" );
            }
        }

        private static Report NewReportService()
        {
            Report _ReportService = new Report();
            _ReportService.OnSocketOpen += _ReportService_OnSocketOpen;
            _ReportService.OnSocketClose += _ReportService_OnSocketClose;
            _ReportService.OnSocketError += _ReportService_OnSocketError;
            _ReportService.OnSocketMessage += _ReportService_OnSocketMessage;
            return _ReportService;
        }

        private static void _ReportService_OnSocketOpen(object sender, EventArgs e)
        {
            //Report s = (Report)sender;
            //Console.WriteLine(format, SFCSocket.WebSocketServices.SessionCount, s.ClientIP, s.ClientPort, s.ID, "Connetion Open");
        }

        private static void _ReportService_OnSocketMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            //Report s = (Report)sender;

            //Console.WriteLine(format, SFCSocket.WebSocketServices.SessionCount, s.ClientIP, s.ClientPort, s.ID, e.Data);
        }


        private static void _ReportService_OnSocketClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            Report s = (Report)sender;
            if (s.Token != null)
            {
                killLogusers(s.Token);
            }
            //Console.WriteLine(format, SFCSocket.WebSocketServices.SessionCount, s.ClientIP, s.ClientPort, s.ID, "Connetion Close");
        }
        private static void _ReportService_OnSocketError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Report s = (Report)sender;
            Console.WriteLine(format, SFCSocket.WebSocketServices.SessionCount, s.ClientIP, s.ClientPort, s.ID, e.Message);
        }

    }
}
