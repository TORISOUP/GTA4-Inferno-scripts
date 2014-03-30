using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;
using System.Collections.Generic;

namespace TestScriptCS.Scripts
{

    public class Soliton : Script
    {
        Ped player;
        Ped[] ped;
        RectangleF rect,prect,erect;
        private Texture Shiya,Shiya2,Shiya3;
        float X, Y, H, W,P;
        InputChecker inputCheckerBomb = new InputChecker();
        Color screenBoxColor;
        Color PlayerColor;
        Color ShiyaColor;
        Vector3 CheckPoint;
        int timer;
        bool ActiveF;
        float Scale;
        public Soliton()
        {
            
            X = 0.75f;
            Y = 0.15f;
           W = 0.2f;
            H = 0.2f;

            ActiveF = false;

            Shiya = Resources.GetTexture("shiya.png");
            Shiya2 = Resources.GetTexture("shiya2.png");
            Shiya3 = Resources.GetTexture("shiya3.png");

            
            screenBoxColor = Color.FromArgb(200, 0, 111, 0);
            PlayerColor = Color.FromArgb(200, 162, 240, 162);
            ShiyaColor = Color.FromArgb(200, 0, 0, 255);

            Scale = 1000.0f;
            player = Player.Character;
            
            ped = World.GetPeds(Player.Character.Position, 50.0f);
            CheckPoint = Player.Character.Position;

            Interval = 1000;
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.S, Keys.O, Keys.L, Keys.I, Keys.T, Keys.O, Keys.N });
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (!ActiveF) { return; }
            if (ActiveF)
            {
 
                
                if (player.isAlive)
                {

                          ped = World.GetPeds(Player.Character.Position, 80.0f);
                    
                }


            }


       }

        private float GetViewDirect(float Head)
        {
            float D;
            Camera cam = GTA.Game.CurrentCamera;
            D = (Head -cam.Rotation.Z + 180.0f) * 3.1415f / 180;

            return D;
        }

        private bool GetNikosGroup(Ped ped)
        {
            for (int i = 0; i < Player.Group.MemberCount; i++)
            {
                if (Player.Group.GetMember(i) == ped)
                {
                    return true;
                }
            }
            return false;
        }

        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            if (!ActiveF) { return; }
            if (Player.CanControlCharacter && player.isAlive )
            {
                RectangleF radar = e.Graphics.GetRadarRectangle(FontScaling.ScreenUnits);  // this retrieves the rectangle of the radar on screen
                float size;

                    
                    P = 0.01f;
                    size = 0.020f;
                  //  X = radar.X;
                  //  Y = radar.Y;
                  //  W = radar.Width;
                  //  H = radar.Height;
                    Scale = 1100.0f;
                
                rect = new RectangleF(X, Y, W, H);
                prect = new RectangleF(X + W / 2 - P / 2, Y + H / 2 - P / 2, P, P);
                erect = new RectangleF(X + W / 2 - P / 2, Y + H / 2 - P / 2, P, P);
                try
                {
                    e.Graphics.Scaling = FontScaling.ScreenUnits; // size on screen will always be the same, regardless of resolution

                    
                    e.Graphics.DrawRectangle(rect, screenBoxColor);
                    for (int i = 0; i < 11; i++)
                    {
                        e.Graphics.DrawLine(X + i * W / 10.0f, Y, X + i * W / 10.0f, Y + H, 0.001f, Color.FromArgb(200, 0, 50, 0));
                        e.Graphics.DrawLine(X, Y + i * H / 10.0f, X + W, Y + i * H / 10.0f, 0.001f, Color.FromArgb(200, 0, 50, 0));
                    }
                     

                 //   e.Graphics.DrawRectangle(prect, PlayerColor);

                    for (int i = 0, length = ped.Length; i < length; i++)
                    {
                        if (Exists(ped[i]))
                        {
                            if (ped[i] == player) { continue; }
                            if (ped[i].Position.DistanceTo2D(player.Position) > 80.0f) { continue; }
                            if (Math.Abs(ped[i].Position.Z - player.Position.Z) > 20.0) { continue; }
                            if (ped[i].Money == 9) { continue; }
                            float x, y, rot;
                            rot = GetViewDirect(0.0f);
                            x = player.Position.X - ped[i].Position.X;
                            y = (player.Position.Y - ped[i].Position.Y);
                            float xx, yy;
                            xx = (float)(x * Math.Cos(rot) - y * Math.Sin(rot));
                            yy = (float)(x * Math.Sin(rot) + y * Math.Cos(rot));
                            x = xx/size;
                            y = yy/size;

                            xx = x + X + W / 2 -P/2;
                            yy = -y + Y + H / 2 - P/2;
                            if (i == 2) { xx = 0.5f; yy = 0.5f; }
                            

                            erect.Location = new PointF(xx, yy);
                            if (ped[i].isAlive)
                            {
                                if (ped[i].isInjured)
                                {
       
                                    e.Graphics.DrawSprite(Shiya3,
                                    Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
                                    Matrix.RotationZ(((180.0f - ped[i].Heading) + GTA.Game.CurrentCamera.Rotation.Z + 45.0f) * 3.1415f / 180.0f) *                               // here we apply the rotation based on our RPM value (given in radians)
                                    Matrix.Translation(xx + P / 2, yy + P / 2, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
                                    Color.FromArgb(50, 255, 255, 255));
                                }
                                else if (ped[i].isInMeleeCombat || ped[i].isInCombat)
                                {

                                    e.Graphics.DrawSprite(Shiya2,
                                    Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
                                    Matrix.RotationZ(((180.0f - ped[i].Heading) + GTA.Game.CurrentCamera.Rotation.Z + 45.0f) * 3.1415f / 180.0f) *                               // here we apply the rotation based on our RPM value (given in radians)
                                    Matrix.Translation(xx + P / 2, yy + P / 2, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
                                    Color.FromArgb(50, 255, 255, 255));
                                }
                                else
                                {

                                    e.Graphics.DrawSprite(Shiya,
                                    Matrix.Scaling(size, size, 1.0f) *                    // now we scale the image to the desired size
                                    Matrix.RotationZ(((180.0f - ped[i].Heading) + GTA.Game.CurrentCamera.Rotation.Z + 45.0f) * 3.1415f / 180.0f) *                               // here we apply the rotation based on our RPM value (given in radians)
                                    Matrix.Translation(xx + P / 2, yy + P / 2, 0.0f), // and finally we move the image to the desired location on the screen (the center of the radar in this case)
                                    Color.FromArgb(50, 255, 255, 255));
                                }
                            }
                            if (Exists(ped[i]))
                            {

                                int Kosa = (230 * (20-(int)Math.Abs(ped[i].Position.Z - player.Position.Z)))/20;
                                if (!ped[i].isAlive)
                                {
                                    e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 0, 0, 0));

                                }
                                else if (GTA.Native.Function.Call<bool>("IS_PLAYER_FREE_AIMING_AT_CHAR", Player, ped[i]) || GTA.Native.Function.Call<bool>("IS_PLAYER_TARGETTING_CHAR", Player, ped[i]))
                                {
                                    e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 234, 234, 0));
                                }
                                else if (ped[i].isRequiredForMission)
                                {
                                    if (ped[i].isInGroup)
                                    {
                                        if (GetNikosGroup(ped[i]))
                                        {
                                            e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 0, 255, 0));
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 0, 0, 255));
                                    }
                                }
                                else if (ped[i].Weapons == Weapon.Heavy_RocketLauncher || ped[i].Weapons == Weapon.Misc_Rocket)
                                {
                                    e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 255, 0, 255));
                                }
                                else if (ped[i].isInCombat || ped[i].isShooting)
                                {
                                    e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 255, 0, 0));
                                }
                                else if (ped[i].isInMeleeCombat)
                                {
                                    e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 255, 128, 0));

                        
                                }
                                else
                                {
                                    e.Graphics.DrawRectangle(erect, Color.FromArgb(Kosa, 255, 0, 0));
                                }
                            }

                        }
                    }
                }
                catch
                {

                    return;
                }
            }
        }

        void Bombat_KeyDown(object sender, GTA.KeyEventArgs m)
        {
            inputCheckerBomb.AddInputKey(m.Key);
            if (inputCheckerBomb.Check(0) == true)
            {
                if (ActiveF)
                {
                    Game.DisplayText("Soliton Lader OFF", 4000);
                    ActiveF = false;
                }
                else
                {
                    Game.DisplayText("Soliton Lader ON", 4000);
                    ActiveF = true;
                }
            }
        }


    }
}
