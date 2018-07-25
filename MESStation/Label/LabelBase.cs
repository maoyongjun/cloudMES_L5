using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MESDataObject.Module;
using Newtonsoft.Json;
using System.Reflection;

namespace MESStation.Label
{
    public class LabelBase
    {
        public List<LabelInputValue> Inputs = new List<LabelInputValue>();
        public List<LabelOutput> Outputs = new List<LabelOutput>()
        {
            new LabelOutput() { Name="PAGE" , Type = LableOutPutTypeEnum.String , Value = "1" },
            new LabelOutput() { Name="ALLPAGE" , Type = LableOutPutTypeEnum.String , Value = "1" }
        };
        public string LabelName = "";
        public string FileName = "";
        public int PrintQTY = 1;
        public virtual void MakeLabel(OleExec DB)
        {
            throw new NotImplementedException();
        }
        public int PAGE
        {
            get
            {
                try
                {
                    return int.Parse((string)Outputs.Find(T => T.Name == "PAGE").Value);
                }
                catch
                {
                    return 1;
                }
            }

            set
            {
                Outputs.Find(T => T.Name == "PAGE").Value = value.ToString();
            }
        }

        public int ALLPAGE
        {
            get
            {
                try
                {
                    return int.Parse((string)Outputs.Find(T => T.Name == "ALLPAGE").Value);
                }
                catch
                {
                    return 1;
                }
            }

            set
            {
                Outputs.Find(T => T.Name == "ALLPAGE").Value = value.ToString();
            }
        }

    }

    public class LabelInputValue
    {
        public string Name;
        [JsonIgnore]
        [ScriptIgnore]
        public object Value;
        public string Type;

        public string StationSessionType;
        public string StationSessionKey;
    }


    public class LabelOutput
    {
        public string Name = "";
        public string Description = "";
        public LableOutPutTypeEnum Type = LableOutPutTypeEnum.String;
        public object Value;
    }

    public enum LableOutPutTypeEnum
    {
        String = 0,
        StringArry = 1
    }

}
