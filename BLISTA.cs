using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Blista : Script
    {
        InputChecker inputCheckerMOD = new InputChecker();
        bool ActiveFlag;
        Random rnd;
        Vehicle V;
        Ped[] peds;
        public Blista()
        {
            peds = new Ped[2];
            ActiveFlag = false;
            rnd = new Random();
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.V, Keys.L, Keys.I});
            Interval = 500;
            this.Tick += new EventHandler(this.MOD_Tick);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
        }

        private void MOD_Tick(object sender, EventArgs e)
        {
            if (ActiveFlag)
            {
                V = GTA.World.CreateVehicle("BLISTA", Player.Character.Position.Around(5.0f));
                if (!Exists(V)) { return; }

                if (Exists(V)) { V.NoLongerNeeded(); }

                V.CreatePedOnSeat(VehicleSeat.Driver);
                V.CreatePedOnSeat(VehicleSeat.RightFront);

                peds[0] = V.GetPedOnSeat(VehicleSeat.Driver);
                peds[1] = V.GetPedOnSeat(VehicleSeat.RightFront);

             

                for (int i = 0; i < 2; i++)
                {
                    if (Exists(peds[i]))
                    {
                        peds[i].NoLongerNeeded();
                    }
                }

                V = null;
                ActiveFlag = false;
            }

        }


        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD.AddInputKey(e.Key);

            if (inputCheckerMOD.Check(0) == true)
            {
                ActiveFlag = true;
            }


        }
    }
}
