using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class WoLinkedReport : ReportBase
    {
        ReportInput WoInput = new ReportInput()
        {
            Name = "WO",
            InputType = "TXT",
            Value = "002510026166",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };

        ReportInput SnInput = new ReportInput()
        {
            Name = "SN",
            InputType = "Select",
            Value = "主SN",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = new string[] { "主SN", "副SN" }
        };

        public WoLinkedReport()
        {
            this.Inputs.Add(WoInput);
            this.Inputs.Add(SnInput);
        }


        public override void Run()
        {
            string sql = null;
            DataTable dt = null;
            string workorderno = WoInput.Value?.ToString();
            if (string.IsNullOrEmpty(workorderno))
            {
                return;
            }
            string SelectValue = SnInput.Value?.ToString();
            if ("主SN".Equals(SelectValue))
            {
                sql = $@"select t.sn,kp.sn linkedsn from (select sn from r_sn where id <> sn and workorderno='{workorderno.Replace("'", "''")}' 
                    ) t left join r_sn_keypart_detail kp on t.sn=kp.sn ";
            }
            else
            {
                sql = $@"select t.sn,kp.sn linkedsn from (select sn from r_sn where id <> sn and workorderno='{workorderno.Replace("'", "''")}' 
                    ) t left join r_sn_keypart_detail kp on t.sn=kp.keypart_sn ";
            }

            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            //string sql = $@"select rownum rn,rsn.sn,kp.sn linkedsn from r_sn rsn left join r_sn_keypart_detail kp on rsn.sn = kp.sn 
            //        where rsn.workorderno='{workorderno.Replace("'", "''")}' ";
            dt = sfcdb.RunSelect(sql).Tables[0];
            
            if (sfcdb != null) DBPools["SFCDB"].Return(sfcdb);

            ReportTable retTab = new ReportTable();
            retTab.LoadData(dt, null);
            retTab.Tittle = "WO Linked Report";
            Outputs.Add(retTab);
        }
    }
}
