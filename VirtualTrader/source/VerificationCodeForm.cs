using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VirtualTrader
{
    public partial class VerificationCodeForm : Form
    {
        public VerificationCodeForm()
        {
            InitializeComponent();
        }

        private void VerficationCodeValueTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.OKButton.PerformClick();
            }
        }
    }
}
