using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;

namespace TestScriptCS.Scripts{

   // ### This example will show you how to load and draw textures ###
    public class JAMMING : Script
    {

        // To use textures in your script, add them to your Visual Studio Project (in the "Solution Explorer").
        // Afterwards, select the added image and change the option "Build Action" to "Embedded Resource".
        // This will embed the image into your script and allows you to load it via Resources.GetTexture .

        private Texture JAM_NO;
        private Texture JAM_DOT;
        GTA.Font TimerFont;
        float scale;
        float pX, pY;
        Random rnd;
        float xxx;
        float yyy;
        float sss;
        int Timer;
        bool JAMMING_FLAG;

        Guid GuidOfScript2;

        public JAMMING()
        {

            GUID = new Guid("915B924A-0D24-11E0-A534-02BCDFD72085");
            BindScriptCommand("ChangeFlag", new ScriptCommandDelegate(ChangeFlag));

            rnd = new Random();
            JAMMING_FLAG = false;

            Interval = 200;
            scale = 0.14f;
            pX = 0.88f;
            pY = 0.04f;
            JAM_NO = Resources.GetTexture("JAMMING_NODOT.png");
            JAM_DOT = Resources.GetTexture("JAMMING_DOT.png");
            TimerFont = new GTA.Font(0.08F, FontScaling.ScreenUnits);
            TimerFont.Color = Color.Yellow;
            GTA.Native.Function.Call("ENABLE_FRONTEND_RADIO");
            PerFrameDrawing += new GTA.GraphicsEventHandler(TextureDrawingExample_PerFrameDrawing);
          //  KeyDown += new GTA.KeyEventHandler(JAM_KeyDown);
            this.Tick += new EventHandler(this.Bombat_Tick);
        }

        private void ChangeFlag(GTA.Script sender, GTA.ObjectCollection Parameter)
        {
            JAMMING_FLAG = Parameter.Convert<bool>(0);
            if (JAMMING_FLAG)
            {
                Timer = 45 * 5;
                JAMMING_FLAG = true;

                GuidOfScript2 = new Guid("EEF9D7C2-0D24-11E0-9289-45BCDFD72085");
                SendScriptCommand(GuidOfScript2, "HORMING_DEACTIVATE_FLAG",true);

            }
            else
            {
                GTA.Native.Function.Call("ENABLE_FRONTEND_RADIO");
                Timer = -1;
                GuidOfScript2 = new Guid("EEF9D7C2-0D24-11E0-9289-45BCDFD72085");
                SendScriptCommand(GuidOfScript2, "HORMING_DEACTIVATE_FLAG", false);
            }
        }


        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (JAMMING_FLAG)
            {
                   GTA.Object[] AO = World.GetAllObjects(new Model(0x5A6525AE));

                   for (int i = 0; i < AO.Length; i++)
                   {
                       if (!Exists(AO[i])) { continue; }
                       AO[i].ApplyForceRelative(new Vector3(3.0f * ((float)rnd.NextDouble() - 0.5f), 3.0f * ((float)rnd.NextDouble() - 0.5f), 3.0f * ((float)rnd.NextDouble() - 0.5f)));


                   }

                       GTA.Native.Function.Call("DISABLE_FRONTEND_RADIO");


                if (--Timer < 0 || Player.Character.isDead)
                {
                    Timer = -1;
                    GTA.Native.Function.Call("ENABLE_FRONTEND_RADIO");
                    GuidOfScript2 = new Guid("EEF9D7C2-0D24-11E0-9289-45BCDFD72085");
                    SendScriptCommand(GuidOfScript2, "HORMING_DEACTIVATE_FLAG", false);
                    JAMMING_FLAG = false;
                }
            }
        }

        private bool BlinkDot(){
            if (Timer % 15 < 8)
            {
                return true;
            }

            if(Timer%2==1){
                return true;
            }else{
                return false;
            }

        }


        private void TextureDrawingExample_PerFrameDrawing(System.Object sender, GTA.GraphicsEventArgs e)
        {
            if (!JAMMING_FLAG) { return; }
            if (!Player.CanControlCharacter) { return; }
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            #region METER

            if (BlinkDot())
            {
                e.Graphics.DrawSprite(JAM_DOT,
                 Matrix.Translation(-0.5f, 0.0f, 0.0f) *               // first we shift the texture half of it's size to the left, because the needle is in the horizontal center of the texture
                 Matrix.Scaling(scale * 1.5f, scale, 1.0f) *                    // now we scale the image to the desired size
                 Matrix.Translation(pX, pY, 0.0f) // and finally we move the image to the desired location on the screen (the center of the radar in this case)
                 );


            }
            else
            {

                e.Graphics.DrawSprite(JAM_NO,
                 Matrix.Translation(-0.5f, 0.0f, 0.0f) *               // first we shift the texture half of it's size to the left, because the needle is in the horizontal center of the texture
                 Matrix.Scaling(scale * 1.5f, scale, 1.0f) *                    // now we scale the image to the desired size
                 Matrix.Translation(pX, pY, 0.0f) // and finally we move the image to the desired location on the screen (the center of the radar in this case)
                 );

            }
            e.Graphics.DrawRectangle(new RectangleF(0.775f, 0.12f, 0.21f*((float)Timer/(45.0f*5.0f)), 0.06f), Color.FromArgb(16,73,31));

            #endregion


            // e.Graphics.DrawText(string.Format("x:{0:f2} y:{1:f2} scale{2:f2}", 0.825f+xxx, 0.15f+yyy, 0.06f+sss), new RectangleF(0.1f, 0.1f, 1.0f, 1.0f));

            e.Graphics.DrawRectangle(new RectangleF(0, 0, 1.0f, 1.0f), Color.FromArgb(20, 0, 200, 0));
        }



        void JAM_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            if (e.Key == Keys.R)
            {
                Timer = 45*5;
                JAMMING_FLAG = true;
            }


            
            switch (e.Key)
            {
                case Keys.H:
                    xxx -= 0.01f;
                    break;
                case Keys.J:
                    yyy += 0.01f;
                    break;
                case Keys.K:
                    xxx += 0.01f;
                    break;
                case Keys.U:
                    yyy -= 0.01f;
                    break;
                case Keys.T:
                    sss += 0.01f;
                    break;
                case Keys.Y:
                    sss-= 0.01f;
                    break;
            }
            

        }

    }

}