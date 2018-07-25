using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESStation.Label
{
    public class TestLabel:LabelBase
    {
        LabelInputValue SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LableOutPutTypeEnum.String, Description = "", Value = "fsfdfsfs" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LableOutPutTypeEnum.String, Description = "", Value = "OOO" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LableOutPutTypeEnum.String, Description = "", Value = "01" };
        LabelOutput O_MAC = new LabelOutput() { Name = "MAC", Type = LableOutPutTypeEnum.StringArry, Description = "", Value = "" };
        public TestLabel()
        {
            this.Inputs.Add(SN);
        }
        public override void MakeLabel(OleExec DB)
        {
            base.MakeLabel(DB);
        }
        
    }
}
