using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
//using System.Windows.Forms;

namespace Arduino
{
    public class Arduino
    {
        public delegate void Log(string text);
        public event Log log;

        public SerialPort com = new SerialPort();
        public bool Connected = false;



        public static string[] PortsList()
        {
            return SerialPort.GetPortNames();
        }


        public void Connect(string port)
        {
            try
            {
                com.PortName = port;
                com.BaudRate = 115200;
                com.Open();
                Connected = true;
            }
            catch (Exception ex)
            {
                Connected = false;
                log(ex.Message);
            }

            if (Connected) log("Connected to " + port);
        }

        public void Disconnect()
        {
            try
            {
                com.Close();
                Connected = false;
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }








        public void PauseForMilliSeconds(int MilliSecondsToPauseFor)
        {


            System.DateTime ThisMoment = System.DateTime.Now;
            System.TimeSpan duration = new System.TimeSpan(0, 0, 0, 0, MilliSecondsToPauseFor);
            System.DateTime AfterWards = ThisMoment.Add(duration);


            while (AfterWards >= ThisMoment)
            {
                //System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
                ThisMoment = System.DateTime.Now;
            }



        }

        public void Reset()
        {
            try
            {
                com.DtrEnable = false;
                Thread.Sleep(100);
                com.DtrEnable = true;
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }





        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static string ByteArrayToStr(byte[] dBytes)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(dBytes);
        }


        public void SetPosition(byte pen, int Errors, int x, int y, bool first)
        {
            if (log != null) log("GoTo: " + x.ToString() + "x" + y.ToString() + " ->");


            if (Connected)
            {
                byte[] dane = new byte[8];
                dane[0] = 0xFA;

                dane[1] = pen;

                byte[] tmp = BitConverter.GetBytes((Int16)Errors);
                dane[2] = tmp[0];
                dane[3] = tmp[1];

                tmp = BitConverter.GetBytes((Int16)x);

                dane[4] = tmp[0];
                dane[5] = tmp[1];

                tmp = BitConverter.GetBytes((Int16)y);
                dane[6] = tmp[0];
                dane[7] = tmp[1];



                try
                {
                    com.Write(dane, 0, dane.Length);
                }
                catch (Exception ex)
                {
                    log(ex.Message);
                }

                if (first) return;

                int t = 0;
                do
                {
                    try
                    {
                        if (com.BytesToRead >= 8)
                        {
                            byte[] tmp1 = new byte[1];
                            do
                            {
                                com.Read(tmp1, 0, 1);
                                // log(tmp1[0].ToString());
                                if (tmp1[0] != 0xFB) break;
                                if (tmp1[0] != 0xFA) break;

                            } while (true);

                            byte[] dane1 = new byte[7];
                            com.Read(dane1, 0, 7);

                            byte penr = dane1[0];
                            int Errorsr = BitConverter.ToInt16(dane1, 1);
                            int xr = BitConverter.ToInt16(dane1, 3);
                            int yr = BitConverter.ToInt16(dane1, 5);
                            if (tmp1[0] == 0xFB)
                            {
                                if (log != null)
                                {

                                    log("OP " + penr.ToString() + " " + Errorsr.ToString() + " " + xr.ToString() + " " +
                                        yr.ToString());

                                    log("pen:" + penr.ToString() + " Errors:" + Errorsr.ToString() + " x:" + xr.ToString() +
                                        " y:" + yr.ToString() + "  -  OK !");

                                    t = 0;
                                }
                                return;
                            }

                            if (log != null && tmp1[0] == 0xFA)
                            {
                                log("OP " + penr.ToString() + " " + Errorsr.ToString() + " " + xr.ToString() + " " +
                                    yr.ToString());
                                t = 0;
                            }

                        }
                        PauseForMilliSeconds(1);
                        // Thread.Sleep(1);

                    }
                    catch { }

                    if (t > 10000)
                    {
                        log("ER");
                        log("TIME OUT!!!");
                        return;
                    }
                    t++;
                } while (true);
            }

        }

    }
}
