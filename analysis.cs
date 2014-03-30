using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;
using System.Collections.Generic;

namespace TestScriptCS.Scripts
{
    #region Analysis
    public class Position2Display
    {

        float a, b;

        public Position2Display(float min_x,float min_y,float max_x,float max_y)
        {
            if (max_x - min_y == 0)
            {
                a = 0;
                b = 0;
            }
            else
            {
                a = (max_y - min_y) / (max_x - min_x);
                b = max_x - a * max_x;
            }
        }

        //対象物が画面のどこらへんにあるかを割り出す処理
        //返り値はVector3(x,y,z)
        // x:0.0～1.0
        // y:0.0～1.0
        // z:距離を線形変換。フォントサイズの算出等に使える。
        public Vector3 Analysis(Camera cam, Vector3 TargetPos)
        {
            float x, y, z;
            x = -cam.Rotation.X * (float)Math.PI / 180.0f;
            y = -cam.Rotation.Y * (float)Math.PI / 180.0f;
            z = -cam.Rotation.Z * (float)Math.PI / 180.0f;
            Vector3 Output = TargetPos - cam.Position;  //平行移動
            Vector3 temp;


            //z軸回りに回転
            temp.X = (float)Math.Cos(z) * Output.X - (float)Math.Sin(z) * Output.Y;
            temp.Y = (float)Math.Sin(z) * Output.X + (float)Math.Cos(z) * Output.Y;
            temp.Z = Output.Z;
            Output = temp;

            //y軸回りに回転
            temp.X = (float)Math.Cos(y) * Output.X - (float)Math.Sin(y) * Output.Z;
            temp.Y = Output.Y;
            temp.Z = (float)Math.Sin(y) * Output.X + (float)Math.Cos(y) * Output.Z;
            Output = temp;


            //x軸回りに回転
            temp.X = Output.X;
            temp.Y = (float)Math.Cos(x) * Output.Y - (float)Math.Sin(x)*Output.Z;
            temp.Z = (float)Math.Sin(x) * Output.Y + (float)Math.Cos(x)*Output.Z;
            Output = temp;


            //ベクトルの角度を取得
            Vector2 theta;
            theta.X = (float)Math.Atan2(Output.X*10.0f,10*Output.Y);
            theta.Y = (float)Math.Atan2(10 * Output.Z, 10 * Output.Y);

            //角度より位置を割り出す
            if (Output.Y >= 0.0)
            {
                temp.X = (float)Math.Sin(-theta.X) + 0.5f;
                temp.Y = (float)Math.Sin(-theta.Y) + 0.5f;
            }
            else
            {
                temp.X = -1.0f;
                temp.Y = -1.0f;
            }

            //線形変換
            temp.Z = a * TargetPos.DistanceTo(cam.Position) + b;

            Output = temp;

            return Output;
        }

    }
        #endregion

    public class analysis : Script
    {
        Position2Display p2d;
        Ped target;
        Vector3 Disp;
        public analysis()
        {
            Interval = 50;
            p2d = new Position2Display(0, 0.1f, 100, 0);
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (!Exists(target))
            {
                target = GTA.World.GetClosestPed(Player.Character.Position, 20.0f);
            }
            else
            {
                Disp = p2d.Analysis(GTA.Game.CurrentCamera, target.Position);
                
            }
       }


        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            if (Exists(target))
            {
                
                if (Disp.X == -1.0f && Disp.Y == -1.0f) { return; }
                string S = "X";
                e.Graphics.DrawText(S, new RectangleF(Disp.X, Disp.Y, 0.05f, 0.05f), TextAlignment.Center | TextAlignment.VerticalCenter, new GTA.Font(0.04F, FontScaling.ScreenUnits));
            }
        }

        void Bombat_KeyDown(object sender, GTA.KeyEventArgs m)
        {
 
        }


    }
}
