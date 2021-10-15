using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {

        bool running = true;
        public Form1()
        {
            InitializeComponent();
        }

        public static float byteArrayToFloat(byte[] bytes)
        {
            int intBits =
              bytes[0] << 24 | (bytes[1] & 0xFF) << 16 | (bytes[2] & 0xFF) << 8 | (bytes[3] & 0xFF);

            byte[] outb= new byte[4];
            for (int i = 0; i < 4; i++)
                outb[3 - i] = bytes[i];

            return BitConverter.ToSingle(outb, 0);
        }

        static TcpListener udpc;
        static TcpListener udpc1;


        public float lastX = 0.0f;
        public float lastY = 0.0f;


        private void udpClientThrdX()
        {
            UdpClient udpc = new UdpClient(23000);
            var remoteEP = new IPEndPoint(IPAddress.Any, 23000);
            bool cont = true;
            while (running)
            {
                
                byte[] data = udpc.Receive(ref remoteEP);
                
                byte[] tmpX = new byte[4];
                for (int i = 0; i < 4; i++)
                    tmpX[i] = data[i];

                byte[] tmpY = new byte[4];
                for (int i = 4; i < 8; i++)
                    tmpY[i - 4] = data[i];

                float valX = byteArrayToFloat(tmpX);

                float valY = byteArrayToFloat(tmpY) * -1;

               

                lastX = valX;
                lastY = valY;
              /*      blocked1 = true;
                controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)(valX * 32000.0));
               // Thread.Sleep(10);
                controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)(valY * 32000.0));
                //Thread.Sleep(10);
                blocked1 = false;
                //controller.SetAxisValue(Xbox360Axis.LeftThumbY, 20000);
              */

                // MessageBox.Show(((float)data[0]).ToString() + "//" + data[1].ToString());



                // listen on port 11000

            }



        }

        public static int blockedCount = 0;
        public static bool blocked1 = false;

        private void udpClientThrdY()
        {
            UdpClient udpc = new UdpClient(23001);

            var remoteEP = new IPEndPoint(IPAddress.Any, 23001);
            udpc.Ttl = 50;
            udpc.Client.Ttl = 50;


            bool cont = true;
            while (cont)
            {

                byte[] data = udpc.Receive(ref remoteEP);

                int iX = data[0];
                int iY = data[1];
                int type = data[2];

                if (type == 0)
                {
                 

                    if (iY >= 125 & iY <= 255)
                    {
                        short valX = 32000;
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, valX);
                    }
                    else if (iY > 0 && iY <= 125)
                    {
                        short valX = -32000;
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, valX);
                    }
                    else
                    {
                        short valX = 0;
                        controller.SetAxisValue(Xbox360Axis.LeftThumbY, valX);
                    }
                }
                else
                {

                    if (iX == 96)
                    {
                        if (iY == 1)
                            controller.SetButtonState(Xbox360Button.A, true);
                        else
                            controller.SetButtonState(Xbox360Button.A, false);
                    }
                }

                //controller.SetAxisValue(Xbox360Axis.LeftThumbY, 20000);


                // MessageBox.Show(((float)data[0]).ToString() + "//" + data[1].ToString());

                System.Threading.Thread.Sleep(5);

                // listen on port 11000

            }



        }

        bool reconnect = true;

        private void udpClientThrdBn()
        {
         
          /*  UdpClient udpc = new UdpClient(23002);
            var remoteEP = new IPEndPoint(IPAddress.Any, 23002);
          */
           // var remoteEP = new IPEndPoint(IPAddress.Any, 23002);

            bool cont = true;
            
            while (running)
                {
                
                TcpClient client=null;
                udpc1.Start();
                
                
                if (udpc1.Pending())
                   client = udpc1.AcceptTcpClient();  //if a connection exists, the server will accept it

                if(client!=null)
                {
                    reconnect = false;
                    NetworkStream ns = client.GetStream();


                    while (client.Connected && !reconnect)
                    {

                        byte[] data = new byte[3];
                        ns.ReadTimeout = -1;
                        try { 
                int ret = ns.Read(data, 0, data.Length);
                        if (ret == 0)
                            reconnect = true;
                        else
                            ns.Write(new byte[] { 1 }, 0, 1);
                    }catch(Exception ex)
                    {
                            reconnect = true;
                    }
                    //    while (blocked1)
                      //      Thread.Sleep(10);
                        
                        try
                        {
                            
                            if (true)
                            {
                                if(data[0]!=0)
                                    blockedCount++;
                                if (mapButtons && data[0] != 0)
                                {
                                    doMapButtons((int)data[0]);
                                }
                                else
                                {
                                    int iX = data[0];
                                    int iY = data[1];
                                    int type = data[2];

                                    if (type == 0)
                                    {

                                        if (iX >= 140 && iX <= 255)
                                        {
                                            short valX = -32000;
                                            controller.SetAxisValue(Xbox360Axis.LeftThumbX, valX);
                                        }
                                        else if (iX > 0 && iX <= 100)
                                        {
                                            short valX = 32000;
                                            controller.SetAxisValue(Xbox360Axis.LeftThumbX, valX);
                                        }
                                        else
                                        {
                                            short valX = 0;
                                            controller.SetAxisValue(Xbox360Axis.LeftThumbX, valX);
                                        }

                                        if (iY >= 140 & iY <= 255)
                                        {
                                            short valX = 32000;
                                            controller.SetAxisValue(Xbox360Axis.LeftThumbY, valX);
                                        }
                                        else if (iY > 0 && iY <= 100)
                                        {
                                            short valX = -32000;
                                            controller.SetAxisValue(Xbox360Axis.LeftThumbY, valX);
                                        }
                                        else
                                        {
                                            short valX = 0;
                                            controller.SetAxisValue(Xbox360Axis.LeftThumbY, valX);
                                        }
                                    }
                                    else
                                    {
                                       // Thread.Sleep(10);

                                        if (iY == 1)
                                            toPress.Add(iX);
                                        else
                                            toRelease.Add(iX);
                                        
                                        
                                        blockedCount--;

                                    }
                                   

                                    //controller.SetAxisValue(Xbox360Axis.LeftThumbY, 20000);


                                    // MessageBox.Show(((float)data[0]).ToString() + "//" + data[1].ToString());
                                }

                            }
                        }catch(Exception ex) { blockedCount=0; }
                        // listen on port 11000
                    }
                }
            }



        }
        public String buttonToMap = "";
        private void doMapButtons(int v)
        {
            if (buttonToMap == "A")
                Settings.buttonMapping.A = v;
            else if (buttonToMap == "B")
                Settings.buttonMapping.B = v;
            else if (buttonToMap == "X")
                Settings.buttonMapping.X = v;
            else if (buttonToMap == "Y")
                Settings.buttonMapping.Y = v;
            else if (buttonToMap == "ST")
                Settings.buttonMapping.Start = v;
            else if (buttonToMap == "SE")
                Settings.buttonMapping.Select = v;
            else if (buttonToMap == "R1")
                Settings.buttonMapping.R1 = v;
            else if (buttonToMap == "R2")
                Settings.buttonMapping.R2s = v;
            else if (buttonToMap == "L1")
                Settings.buttonMapping.L1 = v;
            else if (buttonToMap == "L2")
                Settings.buttonMapping.L2 = v;
            mapButtons = false;
            Settings.SerializeNow();
            WriteTextSafe(label2, "Button " + buttonToMap + " has been mapped.");
        }

        public bool mapButtons=false;


        ViGEmClient client = new ViGEmClient();

        IXbox360Controller controller;

        public void WriteTextSafe(Control control, string text)
        {
            if (control.InvokeRequired)
            {
                Action safeWrite = delegate { WriteTextSafe(control, text); };
                control.Invoke(safeWrite);
            }
            else
            {
                control.Text = text;
            }
        }


        List<int> toPress = new List<int>();
        List<int> toRelease = new List<int>();


        public void pressButton(int bn, int press)
        {
            int iX = bn;
            int iY = press;

            if (iX == Settings.buttonMapping.A)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.A, true);
                else
                    controller.SetButtonState(Xbox360Button.A, false);
            }
            else if (iX == Settings.buttonMapping.B)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.B, true);
                else
                    controller.SetButtonState(Xbox360Button.B, false);
            }
            else if (iX == Settings.buttonMapping.X)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.X, true);
                else
                    controller.SetButtonState(Xbox360Button.X, false);
            }

            else if (iX == Settings.buttonMapping.Y)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.Y, true);
                else
                    controller.SetButtonState(Xbox360Button.Y, false);
            }

            else if (iX == Settings.buttonMapping.L2)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.LeftShoulder, true);
                else
                    controller.SetButtonState(Xbox360Button.LeftShoulder, false);
            }

            else if (iX == Settings.buttonMapping.R2s)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.RightShoulder, true);
                else
                    controller.SetButtonState(Xbox360Button.RightShoulder, false);
            }


            else if (iX == Settings.buttonMapping.Start)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.Start, true);
                else
                    controller.SetButtonState(Xbox360Button.Start, false);
            }

            else if (iX == Settings.buttonMapping.Select)
            {
                if (iY == 1)
                    controller.SetButtonState(Xbox360Button.Back, true);
                else
                    controller.SetButtonState(Xbox360Button.Back, false);
            }

        }

        public void sendReportThrd()
        {
            while(running)
            {
                //while (blockedCount > 0)
                  //  Thread.Sleep(10);

                //   controller.SubmitReport();
                blocked1 = true;
                controller.SetAxisValue(Xbox360Axis.LeftThumbX, (short)(lastX * 32000.0));
                Thread.Sleep(2);
                controller.SetAxisValue(Xbox360Axis.LeftThumbY, (short)(lastY * 32000.0));
                Thread.Sleep(2);

                List<int> tmpPress = toPress.ToList<int>();
                List<int> tmpRelease = toRelease.ToList<int>();

                foreach(int bn in tmpPress)
                {
                    if(tmpRelease.Contains(bn))
                    {
                        pressButton(bn, 1);
                        Thread.Sleep(50);
                        pressButton(bn, 0);
                        toPress.Remove(bn);
                        toRelease.Remove(bn);
                    }
                    else
                    {
                        pressButton(bn, 1);
                        toPress.Remove(bn);
                    }                    
                }

                foreach(int bn in tmpRelease)
                {
                    pressButton(bn, 0);
                    toRelease.Remove(bn);
                }

                blocked1 = false;
                Thread.Sleep(2);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings.DeSerializeNow();

            //udpc = new TcpListener(IPAddress.Any,23000);
            udpc1 = new TcpListener(IPAddress.Any,23002);
            //udpc.Start();
            udpc1.Start();


            controller = client.CreateXbox360Controller();
            controller.Connect();

            backgroundWorker1.RunWorkerAsync();


            controller.AutoSubmitReport = true;
            System.Threading.Thread t = new Thread(udpClientThrdX);
            t.Start();

            System.Threading.Thread t3 = new Thread(sendReportThrd);
            t3.Start();
            

            System.Threading.Thread t2 = new Thread(udpClientThrdBn);
            t2.Start();
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "X";
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                if (mapButtons == true)
                    WriteTextSafe(label2, "Please click Button " + buttonToMap + " on the controller");

                Thread.Sleep(3000);
                if(reconnect)
                {
                    WriteTextSafe(label3, "Disconnected");
                }
                else
                {
                    WriteTextSafe(label3, "Connected");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "A";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "B";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "Y";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "R1";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "R2";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "L1";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "L2";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "SE";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            mapButtons = true;
            buttonToMap = "ST";
        }
    }
}
