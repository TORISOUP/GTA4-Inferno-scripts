using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    
    public class Ucam : Script
    {
        InputChecker inputCheckerCam = new InputChecker();
        bool CamOn;
        Camera ncam;
        Camera DefaultCam;
        bool init = false;
        public Ucam()
        {
            CamOn =false;
            DefaultCam = GTA.Game.CurrentCamera;

            Interval = 0;
            this.Tick += new EventHandler(this.Radio_Tick);


        }

        private void Radio_Tick(object sender, EventArgs e)
        {
            if (CamOn)
            {

                if (Game.isGameKeyPressed(GameKey.LookBehind) && !Game.isGameKeyPressed(GameKey.RadarZoom) )
                {
                    if (Exists(ncam)) { ncam.Deactivate(); }
                    CamOn = false;
                    init = false;
                    return;
                }
                if (Exists(ncam))
                {
                    if (!ncam.isActive)
                    {
                        ncam.Deactivate();
                        CamOn = false;
                        init = false;
                        return;
                    }
                }


                float N;
                if (Player.Character.isInVehicle())
                {
                    Vehicle pV = Player.Character.CurrentVehicle;
                    if (pV.Speed > 20)
                    {
                        N = 0.045f;
                    }
                    else if (pV.Speed > 10)
                    {
                        N = 0.030f;
                    }
                    else
                    {
                        N = 0.01f;
                    }
                }
                else
                {
                    if (Exists(ncam)) { ncam.Deactivate(); }
                    CamOn = false;
                    init = false;
                    return;
                }
                Vector3 D;
                if (init)
                {
                    ncam.Position = Player.Character.Position.Around(5.0f) + (new Vector3(0, 0, 15));
                    D = Player.Character.Position - ncam.Position;
                    D.Normalize();
                    ncam.Direction = D;
                    ncam.Activate();
                    init = false;
                }

                Vector3 P = ncam.Position;
                if (P.DistanceTo2D(Player.Character.Position) > 10.0f)
                {
                    Vector2 nP = new Vector2(Player.Character.Position.X - P.X, Player.Character.Position.Y - P.Y);
                    float Disitance = nP.Length();
                    nP.Normalize();
                    nP = (N * Disitance) * nP;
                    ncam.Position = new Vector3(ncam.Position.X + nP.X, ncam.Position.Y + nP.Y, Player.Character.Position.Z + 15.0f);
                }

                D = Player.Character.Position - ncam.Position;
                D.Normalize();
                ncam.Direction = D;
                if (Exists(DefaultCam))
                {
                    DefaultCam.Position = ncam.Position;
                    DefaultCam.Direction = ncam.Direction;
                }
            }
            else
            {
                if (Exists(ncam)) { ncam.Deactivate(); }
                if (Player.Character.isInVehicle())
                {
                    if (Game.isGameKeyPressed(GameKey.LookBehind) && Game.isGameKeyPressed(GameKey.RadarZoom))
                    {
                        if (Exists(ncam)) { ncam.Deactivate(); }
                        init = true;
                        CamOn = true;
                    }
                }
            }
        }

  
    }
}
