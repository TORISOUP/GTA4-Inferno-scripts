using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;

namespace TestScriptCS.Scripts{

    public class SpeedMeter : Script
    {
        //Font
        GTA.Font screenFont;

        public SpeedMeter()
        {
            screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
            PerFrameDrawing += new GTA.GraphicsEventHandler(TextureDrawingExample_PerFrameDrawing);
        }

        //描画処理
        private void TextureDrawingExample_PerFrameDrawing(System.Object sender, GTA.GraphicsEventArgs e)
        {
            //プレイヤーが操作不可なら処理しない
            if (!Player.CanControlCharacter) { return; }

            //描画のスケーリングを正規化(0.0～1.0)
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            //レーダのサイズと位置を取得
            RectangleF radar = e.Graphics.GetRadarRectangle(FontScaling.ScreenUnits);

            float size = radar.Width * 0.75f;       //レーダのサイズ
            float LeftG = radar.X;                  //レーダの左端
            float RightG = radar.X + radar.Width;   //レーダの右端
            float Y = radar.Y + radar.Height;       //レーダーの真下
            float CC;                               //速度計の色の塗り分けの割合(0.0～1.0)

            //乗車中のみ処理をする
            Vehicle v = Player.Character.CurrentVehicle;
            if (Exists(v))
            {
                float S = v.Speed;  //速度取得
                S = S * 1.609344f;  //mphからkm/hに変換

                float J = (float)Math.IEEERemainder(S, 10.0); //速度を10.0で割った余りを取得
                CC = (1.0f-(-J+5)*0.1f)*radar.Width; //余りから塗り分けの割合を計算

                int H2 = (int)(S+5)/10 + 1; //速度計の右側の色
                int H1 = H2-1;              //速度計の左側の色

                //速度計のメータ部分の描画
                e.Graphics.DrawRectangle(new RectangleF(LeftG, Y,radar.Width-CC, 0.1f),HSV2RGB(360 - 40*H1 + 90,125,255));  //左
                e.Graphics.DrawRectangle(new RectangleF(RightG - CC, Y, CC, 0.1f), HSV2RGB(360 - 40 * H2 + 90, 125, 255));  //右

                //速度の数値描画
                screenFont.Color = HSV2RGB(360 - (int)((40*S)/10) + 90,180,255);    //色を計算
                e.Graphics.DrawText(string.Format("{0}",(int)S), new RectangleF(LeftG,Y,radar.Width,0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
            }
           
        }

        //HSVからRGBへ変換
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