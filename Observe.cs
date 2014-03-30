using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //HUDが消えた時に復旧させるやつ？
    //こんなのあったっけ…
    public class Ovserve : Script
    {
        Camera cam;
        bool Flag = false;
        float pZ;
        Ped tar,tar2;
        int TIME = 0;
        Random rnd;
        InputChecker inputChecker= new InputChecker();
        public Ovserve()
        {
            cam = new Camera();
            rnd = new Random();
            Interval = 100;
            this.Tick += new EventHandler(this.Nikita_Tick);
            inputChecker.AddCheckKeys(new Keys[] { Keys.O, Keys.V, Keys.S});
            KeyDown += new GTA.KeyEventHandler(key_KeyDown);
        }

        private void Nikita_Tick(object sender, EventArgs e)
        {
            if (!Flag) { return; }
            GTA.Native.Pointer LX = typeof(int);
            GTA.Native.Pointer LY = typeof(int);
            GTA.Native.Pointer RX = typeof(int);
            GTA.Native.Pointer RY = typeof(int);
            GTA.Native.Function.Call("GET_POSITION_OF_ANALOGUE_STICKS", 0, LX, LY, RX, RY);
            float pX = 0;
            float pY = 0;

            
 
            pX = 0.5f*LX/128.0f;
            pY = -0.5f*LY/128.0f;

            Vector3 cp = new Vector3(pX, pY, pZ);

            if (!Exists(tar) || tar.isDead || tar.Position.DistanceTo(cam.Position)>50.0f || TIME>10*15)
            {
                Ped[] ped = Cacher.GetPeds(cam.Position, 30.0f);
                tar = ped[rnd.Next(0,ped.Length)];
                

            }
            if (Exists(tar))
            {
                cam.LookAt(tar);
                if (tar2 == tar)
                {
                    TIME++;
                }
                else
                {
                    TIME = 0;
                }
            }
            tar2 = tar;

            cam.Position = cam.Position + cp;
//            cam.Rotation = cam.Rotation + new Vector3(-10.0f * RY / 126f, 0, -10.0f * RX / 126.0f);

            pZ = 0;
          
        }
        void key_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            if (e.Key == Keys.D8)
            {
                pZ += 0.5f;
            }
            if (e.Key == Keys.D2)
            {
                pZ -= 0.5f;
            }

            inputChecker.AddInputKey(e.Key);
            if (inputChecker.Check(0) == true)
            {
                if (Flag)
                {
                    GTA.World.PedDensity = 1.0f;
                    cam.Deactivate();
                    Flag = false;
                    Player.Character.FreezePosition = false;
                    Player.Character.Invincible = false;
                    Player.Character.Visible = true;
                    Player.CanControlCharacter = true;
                    GTA.Native.Function.Call("DISPLAY_HUD", true);
                    GTA.Native.Function.Call("DISPLAY_RADAR", true);
                }
                else
                {
                    Flag = true;
                    GTA.World.PedDensity = 10.0f;
                    cam.Position = Player.Character.Position;
                    cam.Activate();
                    Player.Character.FreezePosition = true;
                    Player.Character.Invincible = true;
                    Player.Character.Visible = false;
                    Player.CanControlCharacter = false;
                    GTA.Native.Function.Call("DISPLAY_HUD", false);
                    GTA.Native.Function.Call("DISPLAY_RADAR", false);
                }
            }

        }
    }
}
                        