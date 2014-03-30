using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Drawing;
using System.Diagnostics;
namespace TestScriptCS.Scripts
{


    public class vsListner : Script
    {

        bool StartFlag = false;
        InputChecker inputCheckerBomb = new InputChecker();
        Stopwatch stopWatch;
        int playerPoint, ListnerPoint;
        GTA.Font screenFont;
        int isGifted = 0;

        public vsListner()
        {
            screenFont = new GTA.Font(0.04F, FontScaling.ScreenUnits);
            Interval = 100;

            this.Tick += new EventHandler(this.ISONO_Tick);
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.L,Keys.I,Keys.S});
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
        }


        private void ISONO_Tick(object sender, EventArgs e)
        {
            if (!StartFlag){return;}
            
                if (Player.Character.isDead)
                {
                    if (stopWatch.IsRunning)
                    {
                        stopWatch.Stop();
                        ListnerPoint++;
                    }
                }

                if (Player.Character.isAlive)
                {
                    if (!stopWatch.IsRunning)
                    {
                        stopWatch.Restart();
                        isGifted = 0;
                    }

                    if (stopWatch.Elapsed.Minutes != isGifted)
                    {
                        playerPoint++;
                        isGifted = stopWatch.Elapsed.Minutes;
                    }



                }
                

            
        }

        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            if (stopWatch != null && stopWatch.IsRunning)
            {
                    e.Graphics.DrawText( "とり: "+playerPoint+" vs "+ListnerPoint+":視聴者 " , new RectangleF(0.0f, 0.08f, 1.0f, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
                    e.Graphics.DrawText(stopWatch.Elapsed.Minutes+"分"+stopWatch.Elapsed.Seconds+"秒", new RectangleF(0.0f, 0.12f, 1.0f, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
  
            }


        }


        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);

            if (inputCheckerBomb.Check(0) == true)
            {
                stopWatch = new Stopwatch();
                StartFlag = !StartFlag;
                ListnerPoint = 0;
                playerPoint = 0;
            }
        }
  
    }
}
