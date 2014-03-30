using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;
using System.IO.Ports;
namespace TestScriptCS.Scripts
{

    public class HeartBeat : Script
    {
        RectangleF rect;
        GTA.Font screenFont;
        SerialPort SP1;
        string HRBT;
        public HeartBeat()
        {
            SP1 = new SerialPort("COM3", 9600);
            SP1.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
            SP1.Open();

            rect = new RectangleF(0.1F, 0.7F, 0.8F, 0.3F);
            screenFont = new GTA.Font(0.07F, FontScaling.ScreenUnits);

            Interval = 100;
            this.PerFrameDrawing += new GraphicsEventHandler(this.mPerFrameDrawing);
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            HRBT = SP1.ReadLine();

        }


        private void mPerFrameDrawing(object sender, GraphicsEventArgs e)
        {

                e.Graphics.Scaling = FontScaling.ScreenUnits;
                if (Player.CanControlCharacter)
                {
                    int hrbt;
                    try
                    {
                        hrbt = int.Parse(HRBT);
                    }
                    catch
                    {
                        return;
                    }
                    hrbt = 360-(360*(hrbt-60) / 40)-90;
                    screenFont.Color = HSV2RGB(hrbt, 255, 255);
                    e.Graphics.DrawText(HRBT, rect, TextAlignment.Center | TextAlignment.Bottom, screenFont);

                }
        }



        Color HSV2RGB(int H, int S, int V)
        {
            if (H < 0) { H += 360; }
            H = H % 360;
            if (H == 360) H = 0;
            int Hi = (int)Math.Floor((double)H / 60) % 6;

            float f = ((float)H / 60) - Hi;
            float p = ((float)V / 255) * (1 - ((float)S / 255));
            float q = ((float)V / 255) * (1 - f * ((float)S / 255));
            float t = ((float)V / 255) * (1 - (1 - f) * ((float)S / 255));

            p *= 255;
            q *= 255;
            t *= 255;

            Color rgb = new Color();

            switch (Hi)
            {
                case 0:
                    rgb = Color.FromArgb(V, (int)t, (int)p);
                    break;
                case 1:
                    rgb = Color.FromArgb((int)q, V, (int)p);
                    break;
                case 2:
                    rgb = Color.FromArgb((int)p, V, (int)t);
                    break;
                case 3:
                    rgb = Color.FromArgb((int)p, (int)q, V);
                    break;
                case 4:
                    rgb = Color.FromArgb((int)t, (int)p, V);
                    break;
                case 5:
                    rgb = Color.FromArgb(V, (int)p, (int)q);
                    break;
            }


            return rgb;
        } 
    }
}
