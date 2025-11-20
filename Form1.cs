using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bat_trung
{


    public partial class Form1 : Form


    {

        bool goLeft, goRight;
        int speed = 5;
        int score = 0;
        int missed = 0;

        Random randY = new Random();
        Random randX = new Random();

        PictureBox splash = new PictureBox();

        public Form1()
        {
            InitializeComponent();
            GameTimer.Stop();

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void MainGameTimer(object sender, EventArgs e)
        {
            txtScore.Text = "Điểm số: " + score;
            txtMissed.Text = "Trứng vỡ: " + missed;

            if (goLeft && player.Left > 0)
            {
                player.Left = Math.Max(0, player.Left - 12);
                player.Image = Properties.Resources.chicken_normal2;
            }
            if (goRight && player.Right < this.ClientSize.Width)
            {
                player.Left = Math.Min(this.ClientSize.Width - player.Width, player.Left + 12);
                player.Image = Properties.Resources.chicken_normal;
            }

            foreach (Control x in this.Controls)
            {
                var pb = x as PictureBox;
                if (pb == null)
                    continue;

                var tag = (pb.Tag ?? string.Empty).ToString();

                // Only egg, and worm fall (not player)
                if (tag != "eggs" && tag != "goldenegg" && tag !="worm")
                    continue;

                // fall
                pb.Top += speed;

                // when egg hits ground
                if (pb.Top + pb.Height > this.ClientSize.Height)
                {
                    splash.Image = Properties.Resources.splash;
                    splash.Location = pb.Location;
                    splash.Size = new Size(50, 50);
                    splash.BackColor = Color.Transparent;
                    if (!this.Controls.Contains(splash))
                        this.Controls.Add(splash);

                    missed += 1;

                    // respawn depending on tag
                    if (tag == "goldenegg")
                        pb.Top = randY.Next(1500, 2000) * -1;
                    else if (tag == "eggs")
                        pb.Top = randY.Next(80, 500) * -1;
                    else 
                        pb.Top = randY.Next(4000, 7000) * -1;

                    pb.Left = randX.Next(5, this.ClientSize.Width - pb.Width);
                    player.Image = Properties.Resources.chicken_hurt;
                }

                // collision handling (mutually exclusive: goldenegg gives +5, normal egg +1)
                if (player.Bounds.IntersectsWith(pb.Bounds))
                {
                    if (tag == "goldenegg")
                    {
                        score += 5;
                        pb.Top = randY.Next(1500, 2000) * -1;
                        pb.Left = randX.Next(5, this.ClientSize.Width - pb.Width);
                    }
                    else if (tag == "eggs")
                    {
                        score += 1;
                        pb.Top = randY.Next(80, 500) * -1;
                        pb.Left = randX.Next(5, this.ClientSize.Width - pb.Width);
                    }
                    else
                    // eat worm makes the player bigger ez to catch eggs (make the player not out of bounds)
                    {
                        player.Top = Math.Max(0, player.Top - 10); // move up a bit to avoid immediate out of bounds
                        player.Width += 10;
                        player.Height += 10;
                        pb.Top = randY.Next(4000, 7000) * -1;
                        pb.Left = randX.Next(5, this.ClientSize.Width - pb.Width);


                    }
                }
            }

            if (missed > 5)
            {
                GameTimer.Stop();
                MessageBox.Show("Thất bại\n Điểm số của bạn: " + score);
            }
            //increase speed 
            if (score > 0 && score % 10 == 0)
            {
                speed = 5 + (score / 10);
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
        }

        private void startbtn_Click(object sender, EventArgs e)
        {
            GameTimer.Start();
            RestartGame();
            startbtn.Enabled = false;
            startbtn.Visible = false;

        }

        private void RestartGame()
        {
            foreach(Control x in this.Controls)
            {
                if(x is PictureBox && x.Tag == "eggs")
                {
                    x.Top = randY.Next(80, 500) * -1;
                    x.Left = randX.Next(5, this.ClientSize.Width - x.Width);
                }
                else if (x is PictureBox && x.Tag == "splash")
                {
                    this.Controls.Remove(x);
                }
                else if (x is PictureBox && x.Tag == "goldenegg")
                {
                    x.Top = randY.Next(2000, 3000) * -1;
                    x.Left = randX.Next(5, this.ClientSize.Width - x.Width);
                }
                else if (x is PictureBox && x.Tag == "worm")
                {
                    x.Top = randY.Next(4000, 7000) * -1;
                    x.Left = randX.Next(5, this.ClientSize.Width - x.Width);
                }
            }

            player.Left = this.ClientSize.Width / 2 - player.Width / 2;
            player.Image = Properties.Resources.chicken_normal;
            
            score = 0;
            missed = 0;
            speed = 8;
            GameTimer.Start();
        }
    }
}
