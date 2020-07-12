using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhepHinh
{
    public partial class Choose : Form
    {
        public bool isServer;

        public Choose()
        {
            InitializeComponent();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            isServer = true;
            DialogResult = DialogResult.OK;
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            isServer = false;
            DialogResult = DialogResult.OK;
        }
    }
}
