using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;

namespace TestScriptCS.Scripts{

    //車の耐久値をマップに重ねて表示する
    //クラス名がサンプルコードのままなのか（困惑）
    public class TextureDrawingExample : Script
    {

        private Texture Gauge;
        private Texture Needle;
        Vehicle v2;
        GTA.Font TimerFont;
        public TextureDrawingExample()
        {
            Gauge = Resources.GetTexture("rpm_gauge.png");
            Needle = Resources.GetTexture("rpm_needle.png");
            TimerFont = new GTA.Font(0.08F, FontScaling.ScreenUnits);
            TimerFont.Color = Color.Yellow;
            PerFrameDrawing += new GTA.GraphicsEventHandler(TextureDrawingExample_PerFrameDrawing);
        }

        private void TextureDrawingExample_PerFrameDrawing(System.Object sender, GTA.GraphicsEventArgs e)
        {
            if (!Player.CanControlCharacter) { return; }
            RectangleF radar = e.Graphics.GetRadarRectangle(FontScaling.Pixel);  // this retrieves the rectangle of the radar on screen
            float size = radar.Width * 0.75f;  // scale the size of the gauge according to the size of the radar

            // calculate the center of the radar
            float radarCenterX = radar.X + radar.Width * 0.5f;
            float radarCenterY = radar.Y + radar.Height * 0.5f;
           
            Vehicle v = Player.Character.CurrentVehicle;
            if (Exists(v))
            {

                float H, P, E;
                H = v.Health;
                P = v.PetrolTankHealth;
                E = v.EngineHealth;

                if (H > 1000.0f) { H = 1000.0f; }
                if (H < 0.0f) { H = 0.0f; }

                if (P > 1000.0f) { P = 1000.0f; }
                if (P < 0.0f) { P = 0.0f; }

                if (E > 1000.0f) { E = 1000.0f; }
                if (E < 0.0f) { E = 0.0f; }

                float body = (H / 1000.0f) * (float)Math.PI;  // here we calulate the radians required for the rotation of the needle
                float Petro = P * (float)Math.PI / 1000;
                float Engine = E * (float)Math.PI / 1000;

                e.Graphics.Scaling = FontScaling.Pixel;  // Pixel is the default setting, but you could also use any other scaling instead


                // The gauge itself is easy to draw, since no rotation is required. The upper left corner of the image is at the center of the radar.
                // The source images only use white color. This allows us to draw the image in any color we want using the last parameter.
                //e.Graphics.DrawSprite(Gauge, new RectangleF(radarCenterX, radarCenterY, size, size), System.Drawing.Color.Black);


                // The needle is more complicated due to the rotation.
                // We build a matrix here to position the needle exactly as required.
                e.Graphics.DrawSprite(Needle,
                   Matrix.Translation(-0.5f, 0.0f, 0.0f) *               // first we shift the texture half of it's size to the left, because the needle is in the horizontal center of the texture
                   Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
                   Matrix.RotationZ(body) *                               // here we apply the rotation based on our RPM value (given in radians)
                   Matrix.Translation(radarCenterX, radarCenterY, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
                   Color.Blue);

                e.Graphics.DrawSprite(Needle,
            Matrix.Translation(-0.5f, 0.0f, 0.0f) *               // first we shift the texture half of it's size to the left, because the needle is in the horizontal center of the texture
            Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
            Matrix.RotationZ(Petro) *                               // here we apply the rotation based on our RPM value (given in radians)
            Matrix.Translation(radarCenterX, radarCenterY, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
            Color.Red);

             e.Graphics.DrawSprite(Needle,
             Matrix.Translation(-0.5f, 0.0f, 0.0f) *               // first we shift the texture half of it's size to the left, because the needle is in the horizontal center of the texture
             Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
             Matrix.RotationZ(Engine) *                               // here we apply the rotation based on our RPM value (given in radians)
             Matrix.Translation(radarCenterX, radarCenterY, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
             Color.Green);
            }




            if (!Exists(v2))
            {
                v2 = Player.Character.CurrentVehicle;
                if (v2 == null) { return; }
            }
            else
            {
                if (Player.Character.isInVehicle())
                {
                    v2 = Player.Character.CurrentVehicle;
                }
            }

            if (v2 == null) { return; }


            if (v2.PetrolTankHealth < 0 && v2.isAlive)
            {
                int S;
                S = (int)Math.Abs(v2.PetrolTankHealth) % 100;

                float Rot1 = (-S * 2 * (float)Math.PI / 100) - (float)Math.PI;
                float Rot2 = (v2.PetrolTankHealth * 2 * (float)Math.PI / 1000) - (float)Math.PI;

             e.Graphics.DrawSprite(Needle,
             Matrix.Translation(-0.5f, 0.0f, 0.0f) *               // first we shift the texture half of it's size to the left, because the needle is in the horizontal center of the texture
             Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
             Matrix.RotationZ(Rot1) *                               // here we apply the rotation based on our RPM value (given in radians)
             Matrix.Translation(radarCenterX, radarCenterY, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
             Color.Yellow);
                         e.Graphics.DrawSprite(Needle,
             Matrix.Translation(-0.5f, 0.0f, 0.0f) *               // first we shift the texture half of it's size to the left, because the needle is in the horizontal center of the texture
             Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
             Matrix.RotationZ(Rot2) *                               // here we apply the rotation based on our RPM value (given in radians)
             Matrix.Translation(radarCenterX, radarCenterY, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
             Color.Yellow);
            }

        }


    }

}