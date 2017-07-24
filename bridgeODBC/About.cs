using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bridgeODBC
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void BackMainButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PdfButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "documents");
        }
    }
}
