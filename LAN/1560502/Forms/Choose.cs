using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

namespace GhepHinh
{
    public partial class Choose : Form
    {
        public bool isServer;

        public Choose()
        {
            InitializeComponent();
            Text = ConfigurationManager.AppSettings["TEXT_CHOOSE_NAME"];
            lbl.Text = ConfigurationManager.AppSettings["TEXT_SERVER_OR_CLIENT"];
            btnServer.Text = ConfigurationManager.AppSettings["TEXT_SERVER"];
            btnClient.Text = ConfigurationManager.AppSettings["TEXT_CLIENT"];
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
