using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Label
{
    public class SnPalletList : LabelBase
    {
        
        LabelInputValue I_PALLETNO = new LabelInputValue() { Name = "PLNO", Type = "STRING", Value = "", StationSessionType = "PRINT_PL", StationSessionKey = "1" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LableOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKU", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PLNO = new LabelOutput() { Name = "PLNO", Type = LableOutPutTypeEnum.String, Description = "", Value = "" };
        public SnPalletList()
        {
            Inputs.Add(I_PALLETNO);

            Outputs.Add(O_SN);
            Outputs.Add(O_SKUNO);
            Outputs.Add(O_VER);
            Outputs.Add(O_QTY);
            Outputs.Add(O_PLNO);
        }
        public override void MakeLabel(OleExec DB)
        {
            Packing.PalletBase pallet = new Packing.PalletBase(I_PALLETNO.Value.ToString(), DB);
            O_SN.Value = pallet.GetSNList(DB);
            if (((List<string>)O_SN.Value).Count == 0)
            {
                throw new Exception("Pallet is empty");
            }
            LogicObject.SN sn = new LogicObject.SN(((List<string>)O_SN.Value)[0], DB, DB_TYPE_ENUM.Oracle);
            LogicObject.WorkOrder wo = new LogicObject.WorkOrder();
            wo.Init(sn.WorkorderNo, DB, DB_TYPE_ENUM.Oracle);
            O_VER.Value = wo.SKU_VER;
            O_QTY.Value = ((List<string>)O_SN.Value).Count.ToString();
            O_PLNO.Value = pallet.DATA.PACK_NO;
            O_SKUNO.Value = wo.SkuNO;
        }
    }
}
