using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ini;

namespace plotter
{
    public partial class Form1 : Form
    {
        
        private Arduino.Arduino arduino = new Arduino.Arduino();
        private byte PenUp = 70;
        private byte  PenDown = 107;
        private bool working = false;
        private Graphics g;
        private Bitmap bmp = new Bitmap(1000, 1000);
        private int Oldx;
        private int Oldy;
        private int Errors = 0;
        private int Speed = 400;
        Ini.IniFile ini = new IniFile(Application.StartupPath+"\\settings.ini");
        private int maxX=7800, maxY=8800; //initial values
        public Plt plt;
        private string ComPort;


        private void ReadSettingsFromIniFile()
        {
            log("Reading Settings from file");
           
            PenUp = (byte)Convert.ToInt16((ini.IniReadValue("settings", "PenUp")));
            log("PenUp=" + PenUp.ToString());
            PenDown = (byte)Convert.ToInt16((ini.IniReadValue("settings", "PenDown")));
            log("PenDown=" + PenDown.ToString());
            Speed = Convert.ToInt16((ini.IniReadValue("settings", "Speed")));
            log("Speed=" + Speed.ToString());

            maxX = Convert.ToInt16((ini.IniReadValue("settings", "maxX")));
            log("maxX=" + maxX.ToString());
            maxY = Convert.ToInt16((ini.IniReadValue("settings", "maxY")));
            log("maxY=" + maxY.ToString());
            
            plt=  new Plt(maxX,maxY);
            label5.Text = "MaxX=" + maxX.ToString();
            label4.Text = "MaxY=" + maxY.ToString();

            ComPort = ((ini.IniReadValue("settings", "ComPort")));
            log("ComPort=" + ComPort.ToString());

            log("---------------------------");
        }

        public Form1()
        {
            InitializeComponent();
            arduino.log += new Arduino.Arduino.Log(ArduinoLog);
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkBox3.Checked = false;
            plt.FileOpen();
            
            
        }

        void ArduinoLog(string text)
        {
            this.Invoke((ThreadStart)delegate()
            {

                if (text.Contains("OP"))
                {
                    pps++;
                    Application.DoEvents();
                    string[] dane = text.Split(' ');
                    int p = Convert.ToInt16(dane[1]);
                    int s = Convert.ToInt16(dane[2]);
                    int x = Convert.ToInt16(dane[3]);
                    int y = Convert.ToInt16(dane[4]);

                    textBox1.Text = x.ToString();
                    textBox2.Text = y.ToString();

                    textBox4.Text = p.ToString();
                    textBox5.Text = s.ToString();


                    Pen pen;
                    if (p==PenDown)
                    {
                        pen = Pens.Yellow ;
                    }
                    else
                    {
                        pen = Pens.WhiteSmoke;
                    }


                    try
                    {
                        g.DrawLine(pen, map(Oldx, 0, 10000, 0, 1000), map(Oldy, 0, 10000, 1000, 0),
                              map(x, 0, 10000, 0, 1000), map(y, 0, 10000, 1000, 0));
             
                    }catch(Exception ex)
                    {
                        log(ex.Message );
                    }
                    Oldx = x;
                    Oldy = y;
                    pictureBox2.Image = bmp;
                    //this.Refresh();
                    Application.DoEvents();
                    
                }
                if (text == "ER") Errors++;
                toolStripStatusLabel1.Text  = "Errors:" + Errors.ToString();
                log(text);
                
            });
           
        }

        void log(string tekst)
        {
            textBox3.AppendText(tekst + Environment.NewLine);
            textBox3.ScrollToCaret();
        }

       

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadSettingsFromIniFile();
            textBox6.Text = Speed.ToString();
            this.Left = 0;
            this.Top = 0;
            g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);
            RysujPoleRysowania();
            pictureBox2.Image = bmp;
            try
            {
                arduino.Connect(ComPort);
            }
            catch
            {
            }
        }

        private int map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }


        private bool draw(bool first)
        {
            pltStructure dane = new pltStructure();
            dane = plt.GetNextCommand1(false);

            if (dane.Pen == PloterPen.end)
            {
                arduino.SetPosition(PenUp, Speed, 0, 0, first);
               return  false;
            }

            switch (dane.Pen)
            {
                case PloterPen.up:
                    arduino.SetPosition(PenUp, Speed, dane.X, dane.Y, first);
                   break;
                case PloterPen.down:
                   arduino.SetPosition(PenDown, Speed, dane.X, dane.Y, first);
                   break;

            }
            return true;
        }



        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            if (checkBox1.Checked) draw(true);
            while (draw(false))
            {
                backgroundWorker1.ReportProgress(map(plt.NrNastepnaKomenda + 1, 0, plt.IloscKomend, 0, 100));
                if (!working) return;
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            checkBox3.Checked = false;
            Errors = 0;
            g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);
            RysujPoleRysowania();

            if (!working) backgroundWorker1.RunWorkerAsync(); 
            working = true;
           
        }

        private void RysujPoleRysowania()
        {
            g.DrawLine(Pens.White, map(0, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(maxX, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0));
            g.DrawLine(Pens.White, map(maxX, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(maxX, 0, 10000, 0, 1000), map(0, 0, 10000, 1000, 0));

           
            g.DrawEllipse(Pens.WhiteSmoke, map(0, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(3800, 0, 10000, 0, 1000), map(6200, 0, 10000, 1000, 0));

            int x = 1200, y = 7600, r = 1400;
            g.DrawEllipse(Pens.WhiteSmoke, map(x, 0, 10000, 0, 1000), map(y, 0, 10000, 1000, 0), map(r, 0, 10000, 0, 1000), map(10000 - r, 0, 10000, 1000, 0));

        }


        private void button8_Click(object sender, EventArgs e)
        {
            working = false;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Speed = Convert.ToInt16(textBox6.Text);
            }
            catch (Exception)
            {
                
               // throw;
            }
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
            plt.Restart();
            button7_Click(null, null);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            arduino.SetPosition((byte)Convert.ToByte(textBox4.Text),Speed , Convert.ToInt16(textBox1.Text), Convert.ToInt16(textBox2.Text),false);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }


        private int pps = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = "pps: "+pps.ToString();
            pps = 0;

            if (checkBox3.Checked ) plt.DrawPreview();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            arduino.SetPosition(PenUp, Speed, 0, 0,false );
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = (Convert.ToInt16(textBox2.Text) + 100).ToString();
            button6_Click(null,null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = (Convert.ToInt16(textBox1.Text) - 100).ToString();
            button6_Click(null, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = (Convert.ToInt16(textBox1.Text) + 100).ToString();
            button6_Click(null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = (Convert.ToInt16(textBox2.Text) - 100).ToString();
            button6_Click(null, null);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
          if (!checkBox2.Checked )
          {
              textBox4.Text = PenUp.ToString();

          }
          else
          {
              textBox4.Text = PenDown.ToString();
          }
            arduino.SetPosition((byte)Convert.ToByte(textBox4.Text),Speed , Convert.ToInt16(textBox1.Text), Convert.ToInt16(textBox2.Text),false);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            plt.DrawPreview();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            plt.MoveToMax();
            plt.DrawPreview();
        }
            
        
    }
}
