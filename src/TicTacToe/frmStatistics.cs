using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class frmStatistics : Form
    {
        public frmStatistics()
        {
            InitializeComponent();

            this.Load += (sender, e) => {
                this.LoadStatistics();
            };

            this.btnReset.Click += (s, e) =>
            {
                Properties.Settings.Default.Reload();
                Properties.Settings.Default.PlayCount = 0;
                Properties.Settings.Default.Player1Won = 0;
                Properties.Settings.Default.Player2Won = 0;
                Properties.Settings.Default.Draw = 0;
                Properties.Settings.Default.Save();

                this.LoadStatistics();
            };     
        }

        private void LoadStatistics()
        {
            Properties.Settings.Default.Reload();
            this.label1.Text = string.Format("Play : {0:n0}", Properties.Settings.Default.PlayCount);
            this.label2.Text = string.Format("Player 'O' Won : {0:n0}", Properties.Settings.Default.Player1Won);
            this.label3.Text = string.Format("Player 'X' Won : {0:n0}", Properties.Settings.Default.Player2Won);
            this.label4.Text = string.Format("Draw : {0:n0}", Properties.Settings.Default.Draw);
        }
    }
}
