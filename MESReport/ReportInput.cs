using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport
{
    public class ReportInput
    {
        public string InputType = "";
        public string Name = "";
        public object Value;
        public object ValueForUse;
        public bool? Enable = true;
        public bool? SendChangeEvent = false;
        public event EventHandler<ReportInputChangeArgs> Change;
        public ReportInput()
        {
            Change += ReportInput_Change;
        }

        private void ReportInput_Change(object sender, ReportInputChangeArgs e)
        {
            
        }
    }

    public class ReportInputChangeArgs : EventArgs
    {
        
    }






}
