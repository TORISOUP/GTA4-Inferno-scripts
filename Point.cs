using System;
using System.Windows.Forms;
using GTA;
using System.Drawing;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    public class PointControll: Script
    {
        bool isAdd;
        bool MissionAdd;
        bool Missions;
        bool provision;
        GTA.Font screenFont;
        int NowPoint;
        int Timer;
        InputChecker inputChecker = new InputChecker();
        public PointControll()
        {
            Timer = 0;
            NowPoint = Share.POINTs;
            screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
            isAdd = true;
            MissionAdd = false;
            Missions = false;
            provision = false;
            Interval = 100;
            this.Tick += new EventHandler(this.MOD_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.MOD_PerFrameDrawing);
            inputChecker.AddCheckKeys(new Keys[] { Keys.R, Keys.E, Keys.S, Keys.E, Keys.T });
            inputChecker.AddCheckKeys(new Keys[] { Keys.OemMinus });
            KeyDown += new GTA.KeyEventHandler(Point_KeyDown);
            Share.LoadPoint();
        }

        void Point_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputChecker.AddInputKey(e.Key);

            if (inputChecker.Check(0) == true)
            {
                Share.ChangePoint(100);
            }

            if (inputChecker.Check(1) == true)
            {
                Share.SavePoint();
            }

        }
        private void MOD_Tick(object sender, EventArgs e)
        {
          
  
            if (GTA.Native.Function.Call<bool>("GET_MISSION_FLAG")) //意味もなくネイティブ関数でミッションフラグ取得
            {
                if (Missions == false)
                {

                    Missions = true;
                    provision = true;
                }
            }
            else
            {
                Missions = false;
            }
            if (provision && Player.Character.isAlive)
            {
                if (Share.POINTs < 100)
                {
                    Share.ChangePoint(100);
                }
                provision = false;
            }

            if (Share.POINTs < 10)
            {
                if (++Timer > 30)
                {
                    Share.AddPoint(1);
                    Timer = 0;
                }
    
            }
            else
            {
                Timer = 0;
            }



            if (Player.Character.isDead) { isAdd = false; NowPoint = 0; }
            if (Player.Character.isAlive && !isAdd)
            {
                Share.ChangePoint(100);
                
                isAdd = true;
            }

            if (!GTA.Native.Function.Call<bool>("IS_MISSION_COMPLETE_PLAYING")) { MissionAdd = false; }
            if (GTA.Native.Function.Call<bool>("IS_MISSION_COMPLETE_PLAYING") && !MissionAdd)
            {
                Share.AddPoint(10000);
                MissionAdd = true;
            }
        }

        unsafe private void MOD_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
   
            if (Player.Character.isDead || !Player.CanControlCharacter) { return; }

            e.Graphics.Scaling = FontScaling.ScreenUnits;
            RectangleF radar = e.Graphics.GetRadarRectangle(FontScaling.ScreenUnits);

            e.Graphics.DrawText(string.Format("{0}", NowPoint), new RectangleF(radar.X, 0.95f, radar.Width, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
            if (NowPoint > Share.POINTs) {
                if (NowPoint - Share.POINTs > 174)
                {
                    NowPoint -= 174;
                }
                if (NowPoint - Share.POINTs > 13)
                {
                    NowPoint -= 13;
                }else{
                NowPoint--;
                }
            }
            if (NowPoint < Share.POINTs)
            {
                if (Share.POINTs - NowPoint > 174)
                {
                    NowPoint += 174;
                }
                if (Share.POINTs - NowPoint > 13)
                {
                    NowPoint += 13;
                }
                else
                {
                    NowPoint++;
                }
            }

        }
    }
}
