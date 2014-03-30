using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;

namespace TestScriptCS.Scripts{

   // パルプンテ：ブラックアウトで動く
    public class BLACKOUT : Script
    {
        Random rnd;
        int Counter;
        int NextTimer;

        bool AF;
        GTA.Font screenFont;
        public BLACKOUT()
        {

            GUID = new Guid("BE5C26AC-0E9A-11E0-8F04-06D4DFD72085");
            BindScriptCommand("Activate", new ScriptCommandDelegate(Activate));

            rnd = new Random();
            AF = false;

            Interval = 100;
 
            PerFrameDrawing += new GTA.GraphicsEventHandler(TextureDrawingExample_PerFrameDrawing);
            KeyDown += new GTA.KeyEventHandler(JAM_KeyDown);
            this.Tick += new EventHandler(this.Bombat_Tick);
        }


        void Start()
        {
            AF = true;
            Counter = -1;
            NextTimer = rnd.Next(400, 2000);
        }
        void Stop()
        {
            AF = false;
            Counter = -1;
        }

        private void Activate(GTA.Script sender, GTA.ObjectCollection Parameter)
        {
            Start();
        }


        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (AF)
            {
                if (Player.Character.isDead)
                {
                    Stop();
                }
                if (!Player.CanControlCharacter)
                {
       
                    Counter = -1;
                    NextTimer = rnd.Next(400, 2000);
                    return;
                }


                if (NextTimer >= 0)
                {
                    Counter = -1;
                    NextTimer--;
                }
                if (NextTimer < 0 && Counter < 0 )
                {
                    Counter = 10 + 15 + 35;
                }

                if (Counter >= 0)
                {
                    Counter--;
                    if (Counter == -1)
                    {
     
                        NextTimer = rnd.Next(400, 2000);
                    }
                }

            }

        }




        private void TextureDrawingExample_PerFrameDrawing(System.Object sender, GTA.GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            if (Counter > 15 + 35)
            {
                screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
                e.Graphics.DrawText("ブラックアウト！", new RectangleF(0.0f, 0.85f, 1.0f, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
            }
            else if(Counter > 35)
            {
  
                screenFont = new GTA.Font(0.1F, FontScaling.ScreenUnits);
                screenFont.Color = Color.FromArgb(64,235,50);
                e.Graphics.DrawRectangle(new RectangleF(0, 0, 1.0f, 1.0f), Color.FromArgb(255, 0, 0, 0));
                e.Graphics.DrawText("ヒデオ", new RectangleF(0.75f, 0.05f, 0.2f, 0.1f), TextAlignment.Right | TextAlignment.Top, screenFont);
               
                
            }
            else if (Counter > 0)
            {
                screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
                screenFont.Color = Color.FromArgb(64, 235, 50);
                e.Graphics.DrawRectangle(new RectangleF(0, 0, 1.0f, 1.0f), Color.FromArgb(255, 0, 0, 0));
                e.Graphics.DrawText("ヒデオ", new RectangleF(0.75f, 0.05f, 0.2f, 0.1f), TextAlignment.Right | TextAlignment.Top, screenFont);
            }
        }



        void JAM_KeyDown(object sender, GTA.KeyEventArgs e)
        {

          

        }

    }

}