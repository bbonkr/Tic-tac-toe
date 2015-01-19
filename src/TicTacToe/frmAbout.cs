using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            this.lblTitle.Text = this.ProductName;
            this.lblVersion.Text = this.ProductVersion;

            this.lblUrl.LinkClicked += (s, e) => {
                LinkLabel thisControl = (LinkLabel)s;
                Process.Start(string.Format("http://{0}", thisControl.Text));
            };
        }
    }
}
