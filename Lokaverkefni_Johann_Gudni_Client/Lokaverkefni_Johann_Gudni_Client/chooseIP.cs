using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lokaverkefni_Johann_Gudni_Client
{
    public partial class chooseIP : Form
    {
        public chooseIP()
        {
            InitializeComponent();
        }
        public string ipaddress { get; set; }
        private ClientForm form;

        public chooseIP(ClientForm sendForm)
        {
            form = sendForm;
        }

        private void bt_ip_Click(object sender, EventArgs e)
        {
            ipaddress = tb_ip.Text;

            

            DialogResult = DialogResult.OK;

            this.Close();
            
        }
    }
}
