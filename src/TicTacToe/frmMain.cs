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
    public partial class frmMain : Form
    {
        private const string PLAYER1 = "O";
        private const string PLAYER2 = "X";
        private const int GAMEOVER_NUMBER = 15;
        private const string START_GAME = "&Start Game";
        private const string RESET_GAME = "Re&set Game";
        private const string PLEASE_PRESS_START_GAME_BUTTON = "Please Press Start Game Button";
        private const string WHOS_TERN = "Palyer {0}'s Tern.";
        private const string DRAW_GAME = "Draw Game. Cheer up.";
        private const string WON_GAME = "Player {0} won!.";

        private int[][] magicSquare;
        private string[][] players;

        private string whosTern;

        public frmMain()
        {
            InitializeComponent();

            magicSquare = new int[][] {
                new int[] { 2, 7, 6 },
                new int[] { 9, 5, 1 },
                new int[] { 4, 3, 8 } };
            players = new string[][] {
                new string[] { string.Empty, string.Empty , string.Empty },
                new string[] { string.Empty , string.Empty , string.Empty },
                new string[] { string.Empty , string.Empty , string.Empty }
            };

            this.dataGridView1.CellContentClick += (s, e) =>
            {
                this.SetPlayer(e.RowIndex, e.ColumnIndex);
            };

            this.btnStartGame.Click += (s, e) =>
            {
                Button thisControl = (Button)s;
                if (thisControl.Text.Equals(START_GAME))
                {
                    thisControl.Text = RESET_GAME;
                    this.startGameToolStripMenuItem.Text = RESET_GAME;
                }

                this.ResetGame();
            };

            this.startGameToolStripMenuItem.Click += (s, e) =>
            {
                ToolStripMenuItem thisControl = (ToolStripMenuItem)s;
                if (thisControl.Text.Equals(START_GAME))
                {
                    thisControl.Text = RESET_GAME;
                    this.btnStartGame.Text = RESET_GAME;
                }

                this.ResetGame();
            };

            this.statisticsToolStripMenuItem.Click += (s, e) =>
            {
                frmStatistics frm = new frmStatistics();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();
            };

            this.panel1.EnabledChanged += (s, e) =>
            {
                Panel thisControl = (Panel)s;
                this.chkPlayer1.Enabled = !thisControl.Enabled;
                this.chkPlayer2.Enabled = !thisControl.Enabled;
            };

            this.exitToolStripMenuItem.Click += (s, e) =>
            {
                this.Close();
            };

            this.aboutToolStripMenuItem.Click += (s, e) => {
                frmAbout frm = new frmAbout();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog();
            };

            this.chkPlayer1.CheckedChanged += (s, e) => { this.MakeSurePlayer(); };
            this.chkPlayer2.CheckedChanged += (s, e) => { this.MakeSurePlayer(); };

            this.btnStartGame.Text = START_GAME;
            this.startGameToolStripMenuItem.Text = START_GAME;

            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ScrollBars = ScrollBars.None;
            this.dataGridView1.Parent.Enabled = false;

            this.chkPlayer1.Text = PLAYER1;
            this.chkPlayer2.Text = PLAYER2;

            for (int i = 0; i < 3; i++)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Height = this.dataGridView1.Height / 3;
            }

            this.whosTern = PLAYER1;
            this.chkPlayer1.Checked = true;
            this.toolStripStatusLabel.Text = PLEASE_PRESS_START_GAME_BUTTON;
        }

        private void SetPlayer(int rowIndex, int colIndex)
        {
            string currentValue = string.Empty;
            DataGridView thisControl = this.dataGridView1;

            if (this.TestMakeHere(rowIndex, colIndex))
            {
                if (string.IsNullOrEmpty(this.whosTern)) { this.whosTern = PLAYER1; }

                thisControl.Rows[rowIndex].Cells[colIndex].Value = this.whosTern;
                this.players[rowIndex][colIndex] = this.whosTern;

                if (this.GameOver(rowIndex, colIndex, this.whosTern))
                {
                    // Game over
                    //MessageBox.Show(string.Format(WON_GAME, this.whosTern));
                    this.toolStripStatusLabel.Text = string.Format(WON_GAME, this.whosTern);

                    // Increase win count
                    this.IncreaseCount(CountType.WinCount, this.whosTern);

                    thisControl.Parent.Enabled = false;
                    this.btnStartGame.Text = START_GAME;
                    this.startGameToolStripMenuItem.Text = START_GAME;

                }
                else
                {

                    switch (this.whosTern)
                    {
                        case PLAYER1:
                            this.whosTern = PLAYER2;
                            break;
                        case PLAYER2:
                            this.whosTern = PLAYER1;
                            break;
                        default:
                            this.whosTern = PLAYER2;
                            break;
                    }

                    this.toolStripStatusLabel.Text = string.Format(WHOS_TERN, this.whosTern);

                    if (!this.DrawGame())
                    {
                        if (this.whosTern.Equals(this.GetAIPlayer()))
                        {
                            this.AITern();
                        }
                    }
                }                
            }                  
        }

        private void ResetGame()
        {
            this.dataGridView1.Parent.Enabled = true;

            int rowCount = this.dataGridView1.Rows.Count;
            int colCount = this.dataGridView1.Columns.Count;

            for(int r= 0; r < rowCount; r++)
            {
                for(int c = 0; c < colCount; c++)
                {
                    this.dataGridView1.Rows[r].Cells[c].Value = string.Empty;
                    this.players[r][c] = string.Empty;
                }
            }

            this.whosTern = PLAYER1;
            this.toolStripStatusLabel.Text = string.Format(WHOS_TERN, this.whosTern);

            // Increase Play Count
            this.IncreaseCount(CountType.PlayCount, string.Empty);

            if (this.whosTern.Equals(this.GetAIPlayer()))
            {
                this.AITern();
            }
        }

        private bool GameOver(int rowIndex, int colIndex, string player)
        {
            // sum == 15
            if (GAMEOVER_NUMBER == this.SumHorizontal(player, rowIndex, colIndex))
            {
                return true;
            }

            if (GAMEOVER_NUMBER == this.SumVertical(player, rowIndex, colIndex))
            {
                return true;
            }

            if (GAMEOVER_NUMBER == this.SumDiagonal(player, rowIndex, colIndex))
            {
                return true;
            }

            return false;
        }

        private int SumHorizontal(string player, int rowIndex, int colIndex, int? testRowIndex, int? testColIndex)
        {
            int sum = 0;
            string tmpPlayer;
            int rowCount = this.dataGridView1.Rows.Count;
            int colCount = this.dataGridView1.Columns.Count;

            for (int c = 0; c < colCount; c++)
            {
                tmpPlayer = string.Format("{0}", this.players[rowIndex][c]);
                if (tmpPlayer.Equals(player))
                {
                    sum += this.magicSquare[rowIndex][c];
                }
            }

            return sum;
        }

        private int SumHorizontal(string player, int rowIndex, int colIndex)
        {
            return this.SumHorizontal(player, rowIndex, colIndex, null, null);
        }

        private int SumVertical(string player, int rowIndex, int colIndex, int? testRowIndex, int? testColIndex)
        {
            int sum = 0;
            string tmpPlayer;
            int rowCount = this.dataGridView1.Rows.Count;
            int colCount = this.dataGridView1.Columns.Count;


            for (int r = 0; r < rowCount; r++)
            {
                tmpPlayer = string.Format("{0}", this.players[r][colIndex]);
                if (tmpPlayer.Equals(player))
                {
                    sum += this.magicSquare[r][colIndex];
                }
            }

            return sum;
        }

        private int SumVertical(string player, int rowIndex, int colIndex)
        {
            return this.SumVertical(player, rowIndex, colIndex, null, null);
        }

        private int SumDiagonal(string player, int rowIndex, int colIndex, int? testRowIndex, int? testColIndex)
        {
            int sum1 = 0;
            int sum2 = 0;

            bool contains = false;

            string tmpPlayer;
            int rowCount = this.dataGridView1.Rows.Count;
            int colCount = this.dataGridView1.Columns.Count;

            int right = 0;
            int left1 = 0;
            int left2 = 0;

            for (right = 0; right < rowCount; right++)
            {
                if ((right == rowIndex && right == colIndex) || contains)
                {
                    contains = true;
                }
                tmpPlayer = string.Format("{0}", this.players[right][right]);
                if (tmpPlayer.Equals(player))
                {
                    sum1 += this.magicSquare[right][right];
                }
            }
            if (!contains) { sum1 = 0; }

            contains = false;
            for (left1 = rowCount - 1; left1 >= 0; left1--)
            {
                left2 = (colCount - 1) - left1;
                if ((left1 == rowIndex && left2 == colIndex) || contains)
                {
                    contains = true;
                }

                tmpPlayer = string.Format("{0}", this.players[left1][left2]);
                if (tmpPlayer.Equals(player))
                {
                    sum2 += this.magicSquare[left1][left2];
                }
            }
            if (!contains) { sum2 = 0; }

            if (sum1 == GAMEOVER_NUMBER) { return sum1; }
            if (sum2 == GAMEOVER_NUMBER) { return sum2; }
            if (sum1 > sum2) { return sum1; }
            else { return sum2; }
        }

        private int SumDiagonal(string player, int rowIndex, int colIndex)
        {
            return this.SumDiagonal(player, rowIndex, colIndex, null, null);
        }

        private bool DrawGame()
        {
            string tmpPlayer = string.Empty;

            int rowCount = this.dataGridView1.Rows.Count;
            int colCount = this.dataGridView1.Columns.Count;
            int nCheck = 0;
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    tmpPlayer = string.Format("{0}", this.dataGridView1.Rows[r].Cells[c].Value);
                    if (!string.IsNullOrEmpty(tmpPlayer)) { nCheck++; }
                }
            }
            if (nCheck == (rowCount * colCount))
            {
                this.dataGridView1.Parent.Enabled = false;
                //MessageBox.Show(DRAW_GAME);
                this.btnStartGame.Text = START_GAME;
                this.startGameToolStripMenuItem.Text = START_GAME;

                this.toolStripStatusLabel.Text = DRAW_GAME;
                // Increase Draw Count
                this.IncreaseCount(CountType.Draw, string.Empty);
                return true;
            }

            return false;
        }

        private void AITern()
        {
            int nRowCount = this.dataGridView1.Rows.Count;
            int nColCount = this.dataGridView1.Columns.Count;

            string aiPlayer = this.GetAIPlayer();
            string player = this.GetPlayer();
            bool needDefense = false;
            bool winToPlay = false;
            bool noIdea = true;

            Point pt = new Point(1, 1);
            if (!this.TestMakeHere(pt.X, pt.Y))
            {
                List<Point> pts = new List<Point>();
                List<int> points = new List<int>();
                int sumTest = 0;
                int nIndex = 0;

                // Defense : Test Players Point
                for (int r = 0; r < nRowCount; r++)
                {
                    for (int c = 0; c < nColCount; c++)
                    {
                        if (string.IsNullOrEmpty(this.players[r][c]))
                        {
                            pts.Add(new Point(r, c));
                            // Will Clear after Test
                            this.players[r][c] = player;
                            sumTest = this.SumHorizontal(player, r, c);
                            points.Add(sumTest);
                            sumTest = this.SumVertical(player, r, c);
                            if (points[points.Count - 1] < sumTest) { points[points.Count - 1] = sumTest; }
                            sumTest = this.SumDiagonal(player, r, c);
                            if (points[points.Count - 1] < sumTest) { points[points.Count - 1] = sumTest; }
                            // Clear
                            this.players[r][c] = string.Empty;
                        }
                    } /* end for c */
                } /* end for r */

                sumTest = 0;
                for (int p = 0; p < points.Count; p++)
                {
                    if (points[p] == GAMEOVER_NUMBER)
                    {
                        pt = pts[p];
                        needDefense = true;
                        break;
                    }
                }


                pts = new List<Point>();
                points = new List<int>();
                sumTest = 0;
                nIndex = 0;

                for (int r = 0; r < nRowCount; r++)
                {
                    for (int c = 0; c < nColCount; c++)
                    {
                        if (string.IsNullOrEmpty(this.players[r][c]))
                        {
                            pts.Add(new Point(r, c));
                            // Will Clear after Test
                            this.players[r][c] = aiPlayer;
                            sumTest = this.SumHorizontal(aiPlayer, r, c);
                            points.Add(sumTest);
                            sumTest = this.SumVertical(aiPlayer, r, c);
                            if (points[points.Count - 1] < sumTest) { points[points.Count - 1] = sumTest; }
                            sumTest = this.SumDiagonal(aiPlayer, r, c);
                            if (points[points.Count - 1] < sumTest) { points[points.Count - 1] = sumTest; }
                            // Clear
                            this.players[r][c] = string.Empty;
                        }
                    } /* end for c */
                } /* end for r */

                sumTest = 0;
                for (int p = 0; p < points.Count; p++)
                {
                    if (points[p] == GAMEOVER_NUMBER)
                    {
                        nIndex = p;
                        winToPlay = true;
                        break;
                    }
                }

                if (winToPlay) { pt = pts[nIndex]; }
                else if(needDefense)
                {
                    //pt = pts[nIndex];
                }else if(winToPlay)
                {
                    pt = pts[nIndex];
                }
                else
                {
                    Point[] corners = { new Point(0,0), new Point(0,2), new Point(2,0), new Point(2,2) };

                    foreach(var point in corners)
                    {
                        if(this.TestMakeHere(point.X, point.Y))
                        {
                            pt = point;
                            noIdea = false;
                        }
                    }

                    if (noIdea)
                    {
                        Random rn = new Random();
                        int nRndX = -1;
                        int nRndY = -1;
                        do
                        {
                            nRndX = rn.Next(3);
                            nRndY = rn.Next(3);
                            pt = new Point(nRndX, nRndY);
                        } while (!this.TestMakeHere(nRndX, nRndY));
                    }
                }
            }

            this.SetPlayer(pt.X, pt.Y);
        }

        private bool TestMakeHere(int rowIndex, int colIndex)
        {
            string strPlayer = string.Format("{0}", this.players[rowIndex][colIndex]);
            return string.IsNullOrEmpty(strPlayer);
        }

        private string GetAIPlayer()
        {
            if (this.chkPlayer1.Checked && this.chkPlayer2.Checked) { return string.Empty; }
            if (!this.chkPlayer1.Checked) { return PLAYER1; }
            else { return PLAYER2; }
        }

        private string GetPlayer()
        {
            if (this.chkPlayer1.Checked) { return PLAYER1; }
            return PLAYER2;
        }

        private void MakeSurePlayer()
        {
            if(!this.chkPlayer1.Checked && !this.chkPlayer2.Checked)
            {
                this.chkPlayer1.Checked = true;
            }
        }

        private void IncreaseCount(CountType countType, string player)
        {
            Properties.Settings.Default.Reload();
            bool hasChanged = false;
            switch (countType)
            {
                case CountType.PlayCount:
                    Properties.Settings.Default.PlayCount++;
                    hasChanged = true;
                    break;
                case CountType.WinCount:
                    if (player.Equals(PLAYER1)) { Properties.Settings.Default.Player1Won++; hasChanged = true; }
                    if (player.Equals(PLAYER2)) { Properties.Settings.Default.Player2Won++; hasChanged = true; }
                    break;
                case CountType.Draw:
                    Properties.Settings.Default.Draw++; hasChanged = true;
                    break;
            }

            if (hasChanged)
            {
                Properties.Settings.Default.Save();
            }
        }
    }
}
