using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Drawing;

namespace TestScriptCS.Scripts
{
    public class FLARE : Script
    {
        bool ActiveFlag;
        bool Setting;
        Random rnd;
        InputChecker inputCheckerMOD = new InputChecker();
        Camera cam;
        GTA.Font sFont;
        Ped[] A;
        int T,S;
        public FLARE()
        {
            S = 0;
            T = 0;
            ActiveFlag = false;
            rnd = new Random();
            cam = new Camera();
            cam.Deactivate();
            A = new Ped[4];
            sFont = new GTA.Font(0.09F, FontScaling.ScreenUnits);

            Interval = 10;
             inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.F, Keys.L, Keys.A, Keys.R,Keys.E });
            this.Tick += new EventHandler(this.MOD_Tick);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
            this.PerFrameDrawing += new GraphicsEventHandler(this.MOD_PerFrameDrawing);
        }

        private void MOD_Tick(object sender, EventArgs e)
        {

            if (ActiveFlag)
            {
                if (Player.Character.isInVehicle())
                {
                    Setting = false;
                }

                if (Setting)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Exists(A[i])) { A[i].Delete(); }
                    }

                    if (!cam.isActive)
                    {
                        //カメラがまだ準備されていないなら
                        Player.Character.FreezePosition = true;
                        cam.Position = Player.Character.Position + new Vector3(0, 0, 50);
                        cam.Direction = new Vector3(0, 0, -1.0f);
                        cam.Activate();
                    }
                    else
                    {
                        //主人公の座標に明かりを出す
                        Vector3 POS = Player.Character.Position;
                        GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_green", POS.X, POS.Y, POS.Z, 0, 0, 0, 1.0f);

                        //スティックの倒し具合を取得
                        GTA.Native.Pointer LX = typeof(int);
                        GTA.Native.Pointer LY = typeof(int);
                        GTA.Native.Pointer RX = typeof(int);
                        GTA.Native.Pointer RY = typeof(int);
                        GTA.Native.Function.Call("GET_POSITION_OF_ANALOGUE_STICKS", 0, LX, LY, RX, RY);


                        float pX, pY, pZ;
                        pX = LX * 3.0f / 127.0f;
                        pY = -LY * 3.0f / 127.0f;
                        pZ = RY * 2.0f / 127.0f;
                        cam.Position += new Vector3(pX, pY, pZ);

                        if (S > 0) { S--; }


                        if (Game.isGameKeyPressed(GameKey.Jump) && S == 0)
                        {
                            S = 5;
                            float rad = 1.0f;
                            A[0] = GTA.World.CreatePed(cam.Position + new Vector3(rad * (float)Math.Cos(2 * Math.PI * ((float)T / 50f + 0f / 4f)), rad * (float)Math.Sin(2 * Math.PI * ((float)T / 50f + 0f / 4f)), 0));
                            A[1] = GTA.World.CreatePed(cam.Position + new Vector3(rad * (float)Math.Cos(2 * Math.PI * ((float)T / 50f + 1f / 4f)), rad * (float)Math.Sin(2 * Math.PI * ((float)T / 50f + 1f / 4f)), 0));
                            A[2] = GTA.World.CreatePed(cam.Position + new Vector3(rad * (float)Math.Cos(2 * Math.PI * ((float)T / 50f + 2f / 4f)), rad * (float)Math.Sin(2 * Math.PI * ((float)T / 50f + 2f / 4f)), 0));
                            A[3] = GTA.World.CreatePed(cam.Position + new Vector3(rad * (float)Math.Cos(2 * Math.PI * ((float)T / 50f + 3f / 4f)), rad * (float)Math.Sin(2 * Math.PI * ((float)T / 50f + 3f / 4f)), 0));

                            for (int i = 0; i < 4; i++)
                            {
                                if (Exists(A[i]))
                                {
                                    A[i].Visible = false;
                                    A[i].Weapons.RocketLauncher.Ammo = 3000;
                                    A[i].Weapons.Select(Weapon.Heavy_RocketLauncher);
                                    Vector3 tar = A[i].Position;
                                    GTA.Native.Function.Call("FIRE_PED_WEAPON", A[i], tar.X, tar.Y, tar.Z - 50.0f);

                                }
                            }

                            T = (T + 1) % 50;

                        }

                        else if (Game.isGameKeyPressed(GameKey.Reload) || !Player.Character.isAlive)
                        {
                            Setting = false;
                        }


                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Exists(A[i])) { A[i].Delete(); }
                    }
                    Player.Character.FreezePosition = false;
                    ActiveFlag = false;
                    Game.DisplayText("Flare OFF", 4000);
                    cam.Deactivate();
                }
            }
        }

        private void MOD_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            if (Setting)
            {
                e.Graphics.DrawText("+", new RectangleF(0.4f, 0.4f, 0.2f, 0.2f), TextAlignment.Center | TextAlignment.VerticalCenter, sFont);
            }
        }

        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD.AddInputKey(e.Key);


            if (inputCheckerMOD.Check(0))
            {
                if (!ActiveFlag)
                {
                    ActiveFlag = true;
                    Game.DisplayText("Flare ON", 4000);
                    Setting = true;
                }
                else
                {
                    ActiveFlag = false;
                    Game.DisplayText("Flare OFF", 4000);
                    cam.Deactivate();
                    Setting = false;
                    Player.Character.FreezePosition = false;
                    for (int i = 0; i < 4; i++)
                    {
                        if (Exists(A[i])) { A[i].Delete(); }
                    }
                }
 
            }

        }
    }
    
}
