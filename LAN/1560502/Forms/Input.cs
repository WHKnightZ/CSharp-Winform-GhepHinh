using System;
using System.Configuration;
using System.Windows.Forms;

namespace GhepHinh
{
    public partial class Input : Form
    {
        public string serverIP;
        public string clientName;

        public Input(string IP)
        {
            InitializeComponent();
            Text= ConfigurationManager.AppSettings["TEXT_INPUT_NAME"];
            lblServerIP.Text = ConfigurationManager.AppSettings["TEXT_SERVER_IP"];
            lblClientName.Text = ConfigurationManager.AppSettings["TEXT_CLIENT_NAME"];

            this.serverIP = IP;
            txtServerIP.Text = IP;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtServerIP.Text == "" || txtClientName.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin");
                return;
            }
            serverIP = txtServerIP.Text;
            clientName = txtClientName.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtServerIP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(this, new EventArgs());
            }
        }

        private void txtClientName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(this, new EventArgs());
            }
        }
    }
}
