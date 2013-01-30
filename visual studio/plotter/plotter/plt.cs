using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace plotter
{
   public  class Plt
    {
        List<string> FileContent=new List<string>();
        private Form2 forma = new Form2();
        public int IloscKomend;
        public int NrNastepnaKomenda=0;

        public  Bitmap bmp = new Bitmap(1000, 1000);

        private Graphics g;



        private int pictureMinX = 0;// values for picture
        private int pictureMinY = 0;// values for picture
        private int pictureMaxX, pictureMaxY;// values for picture

        private int maxX, maxY; // drawing area


       private List<pltStructure> Picture = new List<pltStructure>();

       public void Restart()
       {
           NrNastepnaKomenda = 0;
           pictureMinX = 0;
           pictureMinY = 0;
           pictureMaxX = 0;
           pictureMaxY = 0;
           

       }


        public Plt(int _maxX, int _maxY)
        {
            maxX = _maxX;
            maxY = _maxY;
            g = Graphics.FromImage(bmp);
            rysujPoleRysowania();
        }


        private void FindMinXandMinY()
        {
            pictureMinX = 0;
            pictureMinY = 0;
            pictureMaxX = 0;
            pictureMaxY = 0;

            pltStructure structureNew = new pltStructure();
            while (structureNew.Pen != PloterPen.end)
            {
                structureNew = GetNextCommand1(false);
                if (structureNew.X < pictureMinX)
                {
                    pictureMinX = structureNew.X;
                }
                if (structureNew.Y < pictureMinY)
                {
                    pictureMinY = structureNew.Y;
                }

                if (structureNew.X > pictureMaxX)
                {
                    pictureMaxX = structureNew.X;
                }
                if (structureNew.Y > pictureMaxY)
                {
                    pictureMaxY = structureNew.Y;
                }
            }
        }


       public void MoveToMax()
       {
           forma.trackBar1.Value = maxX - pictureMaxX;
           forma.trackBar2.Value = maxY - pictureMaxY;
           forma.MoveX = forma.trackBar1.Value;
           forma.MoveY = forma.trackBar2.Value;
       }



        public void DrawPreview()
        {
            

            pltStructure structureOld =new pltStructure(PloterPen.up,0,0);
            pltStructure structureNew =new pltStructure();

            forma.ClearLog();
            forma.log("Nr of Commands:"+IloscKomend.ToString());
            //forma.log("Xmin:"+ MinX.ToString( )+" Ymin:"+MinY.ToString( ));
            forma.log("Xmax:" + pictureMaxX.ToString() + " Ymax:" + pictureMaxY.ToString());
            g.Clear(Color.Black);
            rysujPoleRysowania();
            forma.checkedListBox1.Items.Clear();
            NrNastepnaKomenda = 0;

            while (structureNew.Pen!=PloterPen.end)
            {
                structureNew = GetNextCommand1(false);
                forma.progressBar1.Value = NrNastepnaKomenda;
                forma.progressBar1.Refresh();
                Application.DoEvents();


                if (structureNew.Pen== PloterPen.up )
                {
                    //g.DrawLine(Pens.WhiteSmoke, map(structureOld.X, 0, 10000, 0, 1000), map(structureOld.Y, 0, 10000, 1000, 0), map(structureNew.X, 0, 10000, 0, 1000), map(structureNew.Y, 0, 10000, 1000, 0));
                }

                if (structureNew.Pen == PloterPen.down)
                {
                    g.DrawLine(Pens.Yellow , map(structureOld.X, 0, 10000, 0, 1000), map(structureOld.Y, 0, 10000, 1000, 0), map(structureNew.X, 0, 10000, 0, 1000), map(structureNew.Y, 0, 10000, 1000, 0));
                }
                structureOld = structureNew;
                // forma.pictureBox1.Image = bmp;
            }

            NrNastepnaKomenda = 0;
            forma.pictureBox1.Image   = bmp;
        }

        public void FileOpen()
        {
            IloscKomend = 0;
            Restart();
            forma.ClearLog();
            string path = FileOpenDialog("C:\\Users\\eee\\Desktop");
            if (path!="") LoadFile(path);
        }

        private string FileOpenDialog(string initialDirectory)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "plt files (*.plt)|*.plt|All files (*.*)|*.*";
            dialog.InitialDirectory = initialDirectory;
            dialog.Title = "Select a text file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return "";
            }

            
        }


        private void rysujPoleRysowania()
        {
            g.DrawLine(Pens.White, map(0, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(maxX, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0));
            g.DrawLine(Pens.White, map(maxX, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(maxX, 0, 10000, 0, 1000), map(0, 0, 10000, 1000, 0));

            //g.DrawLine(Pens.White, map(400, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(400, 0, 10000, 0, 1000), map(0, 0, 10000, 1000, 0));
            //g.DrawLine(Pens.White, map(400, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(6700, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0));
            //g.DrawLine(Pens.White, map(6700, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(6700, 0, 10000, 0, 1000), map(0, 0, 10000, 1000, 0));
           
            
            g.DrawEllipse(Pens.WhiteSmoke,map(0, 0, 10000, 0, 1000), map(maxY, 0, 10000, 1000, 0), map(3800, 0, 10000, 0, 1000), map(6200, 0, 10000, 1000, 0));

            int x=1200, y=7600, r=1400;
            g.DrawEllipse(Pens.WhiteSmoke, map(x, 0, 10000, 0, 1000), map(y, 0, 10000, 1000, 0), map(r, 0, 10000, 0, 1000), map(10000-r, 0, 10000, 1000, 0));
            
        }

        private void LoadFile(string path)
        {
            if (forma.IsDisposed) forma = new Form2();
            forma.Show();

            forma.listBox.Visible = false;
            using (StreamReader sr = new StreamReader(path))
            {
                String line;
                FileContent.Clear();
                forma.listBox.Items.Clear();
                IloscKomend = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("PU") || line.Contains("PD") || line.Contains("SP0"))
                    {
                        FileContent.Add(line);
                        forma.listBox.Items.Add(line);
                        IloscKomend++;
                        Application.DoEvents();
                    }

                }
            }

            
            forma.listBox.Visible = true;

            LoadPicture();
            FindMinXandMinY();
            DrawPreview();
            

        }

        

       private void LoadPicture()
       {
           forma.reset();
           Restart();
           Picture.Clear();
           while(true)
           {
               pltStructure command = GetNextCommand(false);
               Picture.Add(command);
               if (command.Pen==PloterPen.end) break;

           }
       }

       public pltStructure GetNextCommand1(bool kalibracja)
       {
           forma.progressBar1.Maximum = IloscKomend;

          PloterPen ploterPen; 
           int X = 0;
           int Y = 0;
        

           if (kalibracja)
           {
               X = Picture[NrNastepnaKomenda].X;
               Y = Picture[NrNastepnaKomenda].Y;
               ploterPen = Picture[NrNastepnaKomenda].Pen;
           }
           else
           {
               X = Picture[NrNastepnaKomenda].X + Math.Abs(pictureMinX) + forma.MoveX;
               Y = Picture[NrNastepnaKomenda].Y + Math.Abs(pictureMinY) + forma.MoveY;
               ploterPen = Picture[NrNastepnaKomenda].Pen;
           }





           NrNastepnaKomenda++;
           if (kalibracja)
           {
               forma.progressBar1.Value = NrNastepnaKomenda;
               forma.progressBar1.Refresh();
               Application.DoEvents();
               
           }


           ///////////////////////////////////////////////////////////////////////
           if (true)
           {
               if (X + Math.Abs(pictureMinX) < 0 || Y + Math.Abs(pictureMinY) < 0)
               {
                   forma.checkedListBox1.Items.Add(
                       ploterPen + " " + (X + Math.Abs(pictureMinX)).ToString() + " " +
                       (Y + Math.Abs(pictureMinY)).ToString(), true);
               }
               else
               {
                   //forma.checkedListBox1.Items.Add(ploterPen + " " + (X + Math.Abs(MinX)).ToString() + " " + (Y + Math.Abs(MinY)).ToString());
               }


           }
           /////////////////////////////////////////////////////////////////////

          
       

           if (Picture[NrNastepnaKomenda].Pen== PloterPen.end )
           {

               NrNastepnaKomenda++;
               //forma.progressBar1.Value = NrNastepnaKomenda;
               //Application.DoEvents();
               NrNastepnaKomenda = 0;

               return new pltStructure(PloterPen.end, 0, 0);

           }


            return new pltStructure(ploterPen, X, Y);
       }



        public pltStructure GetNextCommand(bool kalibracja)
        {
            forma.progressBar1.Maximum = IloscKomend;

            for (int i=0; i<forma.listBox.Items.Count;i++ )
            {
                forma.listBox.SetItemChecked(i, false);
            }

            string linia = FileContent[NrNastepnaKomenda];
             PloterPen ploterPen= new PloterPen();
                int X=0;
                int Y=0;
            if (linia.Contains("PU") || linia.Contains( ("PD")))
            {
              
               
                switch (linia.Substring(0, 2))
                {
                    case "PU":
                        ploterPen = PloterPen.up;
                        break;
                    case "PD":
                        ploterPen = PloterPen.down;
                        break;
                }

                linia = linia.Replace("PU", "");
                linia = linia.Replace("PD", "");
                linia = linia.Replace(";", "");

                string[] koordynaty = linia.Split(' ');

                if (kalibracja)
                {
                    X = Convert.ToInt16(koordynaty[0]) ;
                    Y = Convert.ToInt16(koordynaty[1]) ;
                }
                else
                {
                    X = Convert.ToInt16(koordynaty[0]) + Math.Abs(pictureMinX) + forma.MoveX;
                    Y = Convert.ToInt16(koordynaty[1]) + Math.Abs(pictureMinY) + forma.MoveY;     
                }
               

                
                                              

                NrNastepnaKomenda++;
                if (kalibracja)
                {
                    forma.progressBar1.Value = NrNastepnaKomenda;
                    //Application.DoEvents();
                    forma.progressBar1.Refresh();
                }


                ///////////////////////////////////////////////////////////////////////
                if (true)
                {
                    if (X + Math.Abs(pictureMinX) < 0 || Y + Math.Abs(pictureMinY) < 0)
                    {
                        forma.checkedListBox1.Items.Add(ploterPen + " " + (X + Math.Abs(pictureMinX)).ToString() + " " + (Y + Math.Abs(pictureMinY)).ToString(), true);
                    }
                    else
                    {
                        //forma.checkedListBox1.Items.Add(ploterPen + " " + (X + Math.Abs(MinX)).ToString() + " " + (Y + Math.Abs(MinY)).ToString());
                    }
                    

                }
                /////////////////////////////////////////////////////////////////////
                
                return new pltStructure(ploterPen, X, Y);
            }

            if (linia.Contains("SP0"))
            {
                
                NrNastepnaKomenda++;
                forma.progressBar1.Value = NrNastepnaKomenda;
                //Application.DoEvents();
                NrNastepnaKomenda = 0;
                
                return new pltStructure( PloterPen.end ,0,0);
                
            }


            return new pltStructure(PloterPen.end, 0, 0);
          
        }

        private int map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }



    }
}


public class pltStructure
{
    public  PloterPen Pen;
    public int X;
    public int Y;

    public pltStructure( PloterPen _pen, int _x, int _y)
    {

        Pen = _pen;
        X = _x;
        Y = _y;
    }
    public pltStructure()
    {
        
    }
}

public enum PloterPen
{
    up,
    down,
    end
}
