using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;

namespace MESDBHelper
{
    public  class OleExecPool
    {
        public DateTime? LockTime = null;
        public int MaxPoolSize = 700;
        public int MinPoolSize = 2;

        public List<string> OutPutMessage = new List<string>();
        /// <summary>
        /// 等待超時時間,單位秒
        /// </summary>
        public int PoolTimeOut = 3;
        /// <summary>
        /// 連接的最長存活時間,單位秒
        /// </summary>
        public int ActiveTimeOut = 3600;
        /// <summary>
        /// 借出超時時間
        /// </summary>
        public int BorrowTimeOut = 300;
        /// <summary>
        /// 可用對象存放集合
        /// </summary>
        List<OleExecPoolItem> All = new List<OleExecPoolItem>();
        /// <summary>
        /// 已借出對象存放集合
        /// </summary>
        Dictionary<OleExec, OleExecPoolItem> Lend = new Dictionary<OleExec, OleExecPoolItem>();

        public int PoolRemain
        {
            get
            {
                return All.Count;
            }
        }

        public int PoolBorrowed
        {
            get
            {
                return Lend.Count;
            }
        }

        public string _ConnectionString = "";

        bool AutoLock = false;
        bool useLock = false;
        string LockType = "";

        public List<string> ShowLend()
        {
            List<string> ret = new List<string>();
            OleExec[] arry = new OleExec[Lend.Keys.Count];
            Lend.Keys.CopyTo(arry, 0);

            for (int i = 0; i < arry.Length; i++)
            {
                try
                {
                    ret.Add(Lend[arry[i]].LendTime.ToString()+"  "+ (DateTime.Now - Lend[arry[i]].LendTime).TotalSeconds);
                }
                catch
                { }
            }

            return ret;
        }

        public string lockState
        {
            get
            {
                return $@"AutoLock:{AutoLock},useLock:{useLock},lockType:{LockType}";
            }
        }

        public void UNLock()
        {
            if (LockTime!=null&&((TimeSpan)(DateTime.Now - LockTime)).TotalSeconds >= 1)
            {
                AutoLock = false;
                useLock = false;
            }
        }

        public void CleanAll()
        {
            All.Clear();

        }

        /// <summary>
        /// 自動連接池管理計時器
        /// </summary>
        System.Timers.Timer AutoTimer = new System.Timers.Timer();

        System.Timers.Timer AutoULockTimer = new System.Timers.Timer();

        public OleExecPool(string ConnectionString)
        {
            LoadSetting();
            _ConnectionString = ConnectionString;
            while (All.Count < MinPoolSize)
            {
                CreateNewItem();
            }
            AutoTimer.Interval = 60000;
            AutoTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoTimer_Elapsed);
            AutoTimer.Enabled = true;

            AutoULockTimer.Interval = 1000;
            AutoULockTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoULockTimer_Elapsed);
            AutoULockTimer.Enabled = true;
            AutoULockTimer.Start();
        }

        public OleExecPool(string ConnStrName, bool ReadXMLConfig)
        {
            LoadSetting();
            if (ReadXMLConfig)
            {
                ConnectionManager.Init();
                _ConnectionString = ConnectionManager.GetConnString(ConnStrName);
            }
            else
            {
                _ConnectionString = ConfigurationManager.AppSettings[ConnStrName];
            }
            while (All.Count < MinPoolSize)
            {
                CreateNewItem();
            }
            AutoTimer.Interval = 60000;
            AutoTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoTimer_Elapsed);
            AutoTimer.Enabled = true;

            AutoULockTimer.Interval = 1000;
            AutoULockTimer.Elapsed += new System.Timers.ElapsedEventHandler(AutoULockTimer_Elapsed);
            AutoULockTimer.Enabled = true;
            AutoULockTimer.Start();
        }

        public void LoadSetting()
        {
            try
            {
                MaxPoolSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxPoolSize"]);
                MinPoolSize = Convert.ToInt32(ConfigurationManager.AppSettings["MinPoolSize"]);
                PoolTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["PoolTimeOut"]);
                ActiveTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["ActiveTimeOut"]);
                BorrowTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["BorrowTimeOut"]);
            }
            catch { }
        }

        public void Collect()
        {
            OutPutMessage.Clear();
            string Debug = "";
            int sleepCount = 0;
            while (AutoLock || useLock)
            {
                sleepCount++;
                System.Threading.Thread.Sleep(100);
            }
            AutoLock = true;
            LockTime = DateTime.Now;
            LockType = "AutoTimer_Elapsed";
            try
            {
                Debug = "List<OleExecPoolItem> del = new List<OleExecPoolItem>()";
                List<OleExecPoolItem> del = new List<OleExecPoolItem>();
                //檢查對象超時
                foreach (OleExecPoolItem i in All)
                {
                    if ((DateTime.Now - i.CreateTime).TotalSeconds >= ActiveTimeOut)
                    {
                        del.Add(i);
                    }
                }
                Debug = "foreach (OleExecPoolItem i in del)";
                //刪除超時對象
                foreach (OleExecPoolItem i in del)
                {
                    All.Remove(i);
                    try
                    {
                        i.Data.FreeMe();
                    }
                    catch
                    { }
                }
                Debug = "del.Clear();";
                del.Clear();
                List<OleExec> remove = new List<OleExec>();
                OleExec[] arry = new OleExec[Lend.Keys.Count];
                Lend.Keys.CopyTo(arry, 0);
                Debug = "foreach (OleExec o in arry)";
                foreach (OleExec o in arry)
                {
                    try
                    {
                        Debug = "double lendLong = (DateTime.Now - Lend[o].LendTime).TotalSeconds;";
                        double lendLong = 0;
                        try
                        {
                            lendLong = (DateTime.Now - Lend[o].LendTime).TotalSeconds;
                        }
                        catch
                        {
                            remove.Add(o);
                            continue;
                        }
                        Debug = "if (lendLong > BorrowTimeOut)";
                        if (lendLong > BorrowTimeOut)
                        {
                            Debug = "remove.Add(o);";
                            remove.Add(o);
                        }
                    }
                    catch
                    {
                        remove.Add(o);
                    }
                }
                Debug = " foreach (OleExec r in remove)";
                foreach (OleExec r in remove)
                {
                    try
                    {
                        Lend.Remove(r);
                    }
                    catch
                    { }
                }
                remove.Clear();
                Debug = "while ((All.Count + Lend.Count) < MinPoolSize)";
                //創建新對象
                while ((All.Count + Lend.Count) < MinPoolSize)
                {
                    CreateNewItem();
                }
            }
            catch(Exception ee)
            {
                
                OutPutMessage.Add(Debug+":"+ee.Message);
                AutoLock = false;
                LockType = "";
                LockTime = null;
            }
            AutoLock = false;
            LockType = "";
            LockTime = null;
        }

        void AutoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Collect();
        }
        void AutoULockTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UNLock();
        }

        void CreateNewItem()
        {
            OleExecPoolItem Item = null;
            OleExec newOle = new OleExec(_ConnectionString , this);
            Item = new OleExecPoolItem();
            Item.Data = newOle;
            Item.CreateTime = DateTime.Now;
            All.Add(Item);
        }
        /// <summary>
        /// 借出一個連接
        /// </summary>
        /// <returns></returns>
        public OleExec Borrow()
        {
            lock (this)
            {
                int sleepCount = 0;
                while (AutoLock || useLock)
                {
                    sleepCount++;
                    if ((sleepCount * 100) > PoolTimeOut * 1000)
                    {
                        throw new Exception("連接池借出等待超時!");
                    }
                    System.Threading.Thread.Sleep(100);
                }
                useLock = true;
                LockTime = DateTime.Now;
                LockType = "Borrow";
                OleExec ret = null;
                OleExecPoolItem Item = null;
                try
                {
                    if (All.Count == 0 && Lend.Count < MaxPoolSize)
                    {
                        LockType = "Borrow CreateNewItem";
                        CreateNewItem();
                    }
                    if (All.Count > 0)
                    {
                        Item = All[0];
                        LockType = "Borrow All.Remove(Item)";
                        All.Remove(Item);
                        ret = Item.Data;
                        Item.LendTime = DateTime.Now;
                        LockType = "Borrow Lend.Add(ret, Item);";
                        Lend.Add(ret, Item);

                    }
                    else
                    {
                        throw new Exception("連接池超過最大配置,無法借出");
                    }
                }
                catch (Exception ee)
                {
                    LockType = "";
                    LockTime = null;
                    useLock = false;
                    throw ee;
                }
                finally
                {
                    LockType = "";
                    LockTime = null;
                    useLock = false;

                }

                return ret;
            }
        }
        /// <summary>
        /// 向連接池歸還連接
        /// </summary>
        /// <param name="db"></param>
        public void Return(OleExec db)
        {
            //int sleepCount = 0;
            while (AutoLock || useLock)
            {
                //sleepCount++;
                //if ((sleepCount * 100) > PoolTimeOut * 1000)
                //{
                //    throw new Exception("連接池借出等待超時!");
                //}
                System.Threading.Thread.Sleep(100);
            }
            useLock = true;
            LockTime = DateTime.Now;
            LockType = "Return db.RollbackTrain();";
            try
            {
                db.RollbackTrain();
            }
            catch
            { }
            try
            {
                OleExecPoolItem item = Lend[db];
                LockType = "Return All.Add(item);";
                All.Add(item);
                LockType = "Return Lend.Remove(db);";
                Lend.Remove(db);
            }
            catch
            {
                //LockType = "";
                //useLock = false;
            }
            finally
            {
                LockType = "";
                LockTime = null;
                useLock = false;
            }
        }

        public bool TestBorrow(OleExec db)
        {
            if (Lend.ContainsKey(db))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class OleExecPoolItem
    {
        public OleExec Data;
        public DateTime CreateTime;
        public DateTime LendTime;
    }
}
