using System;
using System.Windows.Forms;

namespace GhepHinh
{
    public partial class InputIP : Form
    {
        public string IP;

        public InputIP(string IP)
        {
            InitializeComponent();
            this.IP = IP;
            txtIP.Text = IP;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IP = txtIP.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
