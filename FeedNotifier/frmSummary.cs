using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FeedNotifier
{
    public partial class frmSummary : Form
    {
        public frmSummary(string htmltext)
        {
            InitializeComponent();
            webBrowser1.DocumentText = htmltext;
        }
    }
}
