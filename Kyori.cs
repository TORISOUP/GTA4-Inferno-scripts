using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //死亡距離
    public class Kyori : Script
    {
        Vector3 Alive, dead,vel;//生存時、死亡時、速度
        float dist;             //飛距離
        Ped player;

        GTA.Font screenFont;
   //     Color screenBoxColor;

        public Kyori()
        {
            screenFont = new GTA.Font(0.15F, FontScaling.ScreenUnits);
            screenFont.Color = Color.Red;

            player = Player.Character;
            Interval = 10;
            this.Tick += new EventHandler(this.Kyori_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

            //MODのロードが完了したと知らせる
            Game.DisplayText("MOD load successful.", 4000);
        }

        private void Kyori_Tick(object sender, EventArgs e)
        {
            if (player.isAlive)
            {
                Alive = player.Position;
                dist = 0.0f;
            }
            else
            {
                dead = player.Position;
                dist = Alive.DistanceTo(dead);
                vel = player.Velocity;
            }

       }
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            try
            {
                if (!player.isAlive)
                {
                    e.Graphics.Scaling = FontScaling.ScreenUnits; // size on screen will always be the same, regardless of resolution
                    RectangleF rect = new RectangleF(0.70F, 0.85F, 0.3F, 0.1F);
                    e.Graphics.DrawText(string.Format("{0:f1}", dist), rect, TextAlignment.Center | TextAlignment.VerticalCenter, screenFont);


                    //謎のメーター
            /*        float haba;
                    screenBoxColor = Color.FromArgb(127, 0, 0, 255);
                    haba = dist / 400.0f;
                    rect = new RectangleF(0.05F, 0.85F, haba, 0.2f);
                    e.Graphics.DrawRectangle(rect, screenBoxColor);

                    */
                }
            }
            catch
            {
                //描画に失敗したとき
                return;
            }
        }
    }
}
