using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace plotter
{
    public partial class Form2 : Form
    {
        public int MoveX, MoveY;

        public Form2()
        {
            InitializeComponent();
        }

       public void log(string text)
       {
           textBox1.AppendText(text + Environment.NewLine);
           textBox1.ScrollToCaret();
       }

       private void Form2_Load(object sender, EventArgs e)
       {
           
       }

       private void checkBox1_CheckedChanged(object sender, EventArgs e)
       {
          if (checkBox1.Checked )
          {
              
              pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
              panel1.AutoScroll = true;
          }
          else
          {

              
              int size = panel1.Width > panel1.Height ? panel1.Height : panel1.Width;
              pictureBox1.Width = size ;
              pictureBox1.Height =size;
              pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
          }
             
       }

       private void trackBar1_Scroll(object sender, EventArgs e)
       {
           MoveX = trackBar1.Value;
       }

       private void button1_Click(object sender, EventArgs e)
       {
          
       }

        public void ClearLog()
        {
            textBox1.Text = "";
        }
       private void trackBar2_Scroll(object sender, EventArgs e)
       {
           MoveY = trackBar2.Value;
       }

       private void groupBox1_Enter(object sender, EventArgs e)
       {

       }

       private void Form2_ResizeEnd(object sender, EventArgs e)
       {
           //pictureBox1.Width = panel1.Width;
           //pictureBox1.Height = panel1.Height;
           //if (this.WindowState == FormWindowState.Maximized) ;
 
       }


        public void reset()
        {
            trackBar1.Value = 0;
            trackBar2.Value = 0;
            MoveX = 0;
            MoveY = 0;
        }

       private void Form2_Resize(object sender, EventArgs e)
       {
           //if (this.WindowState == FormWindowState.Maximized)
           {
               int size = panel1.Width > panel1.Height ? panel1.Height : panel1.Width;
               pictureBox1.Width = size;
               pictureBox1.Height = size;
           }
       }
    }
}
