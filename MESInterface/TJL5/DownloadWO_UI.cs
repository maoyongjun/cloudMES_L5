using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MESInterface.TJL5;

namespace MESInterface.TJ_L5
{
    public partial class DownloadWO_UI : UserControl
    {
        private DownloadWO downloadWO;

        public DownloadWO_UI()
        {
            InitializeComponent();
        }

        public DownloadWO_UI(DownloadWO downloadWO)
        {
            this.downloadWO = downloadWO;
        }

        public DownloadWO_UI(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
