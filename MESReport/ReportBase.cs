using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDBHelper;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace MESReport
{
    public class ReportBase
    {
        public List<ReportInput> Inputs = new List<ReportInput>();
        public List<object> Outputs = new List<object>();
        [JsonIgnore]
        [ScriptIgnore]
        public Dictionary<string, OleExecPool> DBPools;

        public Dictionary<string, string> Sqls = new Dictionary<string, string>();
        public List<string> RunSqls = new List<string>();
        public bool AutoRun = false;
        //佈局指定總行數,在Outputs的RowNun屬性可以放到對應的位置
        public int LayoutRows = 1;

        public int LayoutCols = 1;

        public virtual void Run()
        {

        }

        public virtual void InputChangeEvent()
        {

        }

        public virtual void Init()
        {
            
        }


    }
}
