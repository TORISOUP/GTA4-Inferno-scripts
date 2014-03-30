using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Nitro : Script
    {
        Ped player;
        int timer;
        bool flag;
        bool bombflag;
        Vehicle LastV;
        Random rnd;
        public Nitro()
        {
            rnd = new Random();
            flag = false;
            bombflag = false;
            timer = 0;
            player = Player.Character;
            Interval = 100;
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
        }
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            RectangleF Nit;
            try
            {
                if (timer > 0)
                {
                    Nit = new RectangleF(0.01f, 0.01f, 0.2f * timer / 100.0f, 0.03f);
                    e.Graphics.DrawRectangle(new RectangleF(0.01f, 0.01f, 0.2f, 0.03f), Color.FromArgb(150, 0, 0, 0));
                    e.Graphics.DrawRectangle(Nit, Color.FromArgb(150, 0, 255, 64));
                }
            }
            catch
            {
                return;
            }



        }
        private void Bombat_Tick(object sender, EventArgs e)
        {

            if (player.isInVehicle())
            {
                Vehicle pV = Player.Character.CurrentVehicle;
                LastV = pV;
                if (timer == 0 && flag == false)
                {

                    if (GetGamePad() && Share.POINTs-5>=0)
                    {
                        if (!player.isAlive)
                        {
                            Game.DisplayText("Nitro Start", 4000);
                            flag = true;
                            timer = 0;
                            if (!Share.NitroLimit)
                            {
                                Share.AddPoint(-5);
                            }
                        }
                        else
                        {
                            if (pV.EngineHealth > 0 && pV.PetrolTankHealth > 0)
                            {
                                Game.DisplayText("Nitro Start", 4000);
                                flag = true;
                                timer = 0;
                                if (!Share.NitroLimit)
                                {
                                    Share.AddPoint(-5);
                                }
                            }
                        }

                    }
                }


                if (flag == true)
                {
                    if (timer == 0)
                    {
                        pV.MakeProofTo(false, false,true, false, false);
                        bombflag = true;
                        //GTA.Native.Function.Call("ADD_EXPLOSION", pV.Position.X, pV.Position.Y, pV.Position.Z, 6, 10.0f, 30, 0, 0.5f);

                        GTA.World.AddExplosion(pV.Position,ExplosionType.Rocket, 0.0f, true, false, 0.5f);
                        pV.HazardLightsOn = true;

                        GTA.Native.Pointer LX = typeof(int);
                        GTA.Native.Pointer LY = typeof(int);
                        GTA.Native.Pointer RX = typeof(int);
                        GTA.Native.Pointer RY = typeof(int);
                        GTA.Native.Function.Call("GET_POSITION_OF_ANALOGUE_STICKS",0,LX,LY,RX,RY);
                        if (Math.Abs(LY) > 20 && Share.POINTs - 5 >= 0)
                        {
                          //  player.Money -= (int)(player.Money * 0.1);
                        }
                        if (Player.WantedLevel == 0)
                        {
                            pV.Rotation = new Vector3(pV.Rotation.X + (LY * 30) / 126, pV.Rotation.Y, pV.Rotation.Z);
                        }
                        if ((int)pV.EngineHealth*3 > rnd.Next(0, 1001) || !pV.isAlive)
                        {
                            if (Game.isGameKeyPressed(GameKey.Crouch))
                            {
                                if (Math.Abs(LY) > 20)
                                {
                                    pV.Speed -= 20.0f;
                                }
                                else
                                {
                                    pV.Speed -= 55.0f;
                                }
                            }
                            else
                            {
                                if (Math.Abs(LY) > 20)
                                {
                                    pV.Speed += 20.0f;
                                }
                                else
                                {
                                    pV.Speed += 55.0f;
                                }
                            }
                        }
                        else
                        {
                            pV.Speed += 5.0f;
                            pV.EngineHealth = 0.1f;
                        }
                    }
                    else if (timer == 3 * 10)
                    {
                        bombflag = false;

                    }
                    else if (timer == 10 * 10)
                    {
                        pV.HazardLightsOn = false;
                        flag = false;
                        timer = -1;
                        Game.DisplayText("Nitro OK", 4000);

                    }


                    if (bombflag == true)
                    {
                        if (pV.Speed <= 20.0f)
                        {
                            bombflag = false;
                        }
                        else
                        {
                            pV.MakeProofTo(false, false, false, false, false);
                            //    GTA.Native.Function.Call("ADD_EXPLOSION", pV.Position.X, pV.Position.Y, pV.Position.Z, 3, 0.0f, 30, 0, 0.1f);
                        }
                    }
                    else
                    {
                        pV.MakeProofTo(false, false, false, false, false);

                    }
                    timer++;
                    if (Share.NitroLimit) { timer = 100; }

                }
                else
                {
                    flag = false;
                    timer = 0;
                    if (Exists(LastV))
                    {
                        LastV.MakeProofTo(false, false, false, false, false);
                    }
                }
            }
            else
            {
                timer = 0;
                flag = false;
                if (Exists(LastV))
                {
                    LastV.MakeProofTo(false, false, false, false, false);
                }
            }
        }
     
        bool GetGamePad()
        {
           if (Game.isGameKeyPressed(GameKey.SeekCover) && Game.isGameKeyPressed(GameKey.Jump) && Game.isGameKeyPressed(GameKey.Attack))
            {

                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
