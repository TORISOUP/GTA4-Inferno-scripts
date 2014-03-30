using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using GTA;
using _InputChecker;
using _ItemListForm;

namespace TestScriptCS.Scripts
{

    public class Nikita : Script
    {
        const float TO_RADIAN = 0.01745329f;
        bool Active;
        GTA.Object rck;
        Vector3 Vec;
        Camera cam;
        float speed;
        int BombTimer;
        public Nikita()
        {
            
            Interval = 10;

            BombTimer = 0;
            Active = false;
            this.Tick += new EventHandler(this.Nikita_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }


        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            if (!Active || !Exists(rck)) { return; }

           

            try
            {
                //BackGround
                e.Graphics.DrawRectangle(new RectangleF(0.0f, 0.0f, 1.0f, 1.0f), Color.FromArgb(50, 0, 255, 0));


                //TimerBar
                float Haba = (0.3f*(float)BombTimer)/150.0f;
                e.Graphics.DrawRectangle(new RectangleF(0.3f, 0.8f, 0.3f, 0.025f), Color.FromArgb(200, 0, 0, 0));
                e.Graphics.DrawRectangle(new RectangleF(0.3f, 0.8f, Haba, 0.025f), Color.FromArgb(200, 0, 250, 0));

            }
            catch
            {
                return;
            }



        }



        private GTA.Object createObj(uint hash,Vector3 pos)
        {
            GTA.Object objCam = World.CreateObject(hash, pos);
            while (Exists(objCam) == false)
            {
                Wait(0);
                objCam = World.CreateObject(hash, pos);
            }

            return objCam;
        }

        private void Nikita_Tick(object sender, EventArgs e)
        {
            if (Active)
            {
                if (Exists(rck))
                {
                    if (BombTimer == 0)
                    {
                        rck.Velocity = speed * GTA.Game.CurrentCamera.Direction;
                        cam.Activate();
                        cam.Position = rck.Position;
                        cam.Rotation = new Vector3(0, 0, GTA.Game.CurrentCamera.Rotation.Z);
                        
                    }
                    BombTimer++;


                    if (speed < 80)
                    {
                        speed+=2;
                    }


                    GTA.Native.Pointer LX = typeof(int);
                    GTA.Native.Pointer LY = typeof(int);
                    GTA.Native.Pointer RX = typeof(int);
                    GTA.Native.Pointer RY = typeof(int);
                    GTA.Native.Function.Call("GET_POSITION_OF_ANALOGUE_STICKS", 0, LX, LY, RX, RY);

                    if (Math.Abs(RX) > 3 || Math.Abs(RY) > 3)
                    {
                        speed = 10;
                    }

                    cam.Position = rck.Position;
                    cam.Rotation = cam.Rotation + new Vector3(-10.0f*RY/126f,0,-10.0f*RX/126.0f);
                    Vec = cam.Direction;
                    rck.Velocity = speed*Vec;

                    if (GTA.Native.Function.Call<bool>("HAS_OBJECT_COLLIDED_WITH_ANYTHING", rck) || Game.isGameKeyPressed(GameKey.Sprint) || Player.Character.Position.DistanceTo(rck.Position)>150.0f || BombTimer>=150 || !Player.Character.isAlive)
                    {
                        if (rck.Position.DistanceTo(Player.Character.Position) < 10.0f)
                        {
                            GTA.World.AddExplosion(rck.Position, ExplosionType.Molotov, 1.0f);
                        }
                        else
                        {
                            GTA.World.AddExplosion(rck.Position, ExplosionType.Rocket, 1000.0f, true,false, 1.0f);
                        }
                        rck.Delete();
                        Active = false;
                        cam.Deactivate();
                        cam.Delete();
                    }
                }
                else
                {
                    Active = false;
                    cam.Deactivate();
                    cam.Delete();
                }
            }
            else
            {
                if (Player.Character.isAlive && Player.Character.Weapons == Weapon.Unarmed && Player.CanControlCharacter && !Player.Character.isInVehicle() && Player.Character.Velocity.Length() < 2.0)
                {
                    if (Game.isGameKeyPressed(GameKey.Attack) && Game.isGameKeyPressed(GameKey.Reload) && !Active)
                    {
                        Active = true;
                        speed = 30;
                        cam = new Camera();
                        rck = createObj(0xE20794ED, Player.Character.Position + new Vector3(0,0,1.0f));
                        GTA.Native.Function.Call("SET_ACTIVATE_OBJECT_PHYSICS_AS_SOON_AS_IT_IS_UNFROZEN",rck,true);
                        rck.Collision = true;
                        GTA.Native.Function.Call("SET_OBJECT_RECORDS_COLLISIONS", rck, true);
                        BombTimer = 0;
                    }
                }
            }

        }

  
    }
}
