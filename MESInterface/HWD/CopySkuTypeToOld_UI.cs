using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MESInterface.HWD
{
    public partial class CopySkuTypeToOld_UI : UserControl
    {
        CopySkuTypeToOld obj = null;
        public CopySkuTypeToOld_UI(CopySkuTypeToOld copySku)
        {
            InitializeComponent();
            obj = copySku;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                obj.updateDate = this.dateTimePicker1.Value.ToString("yyyy-MM-dd");
                obj.Start();
                obj.updateDate = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
