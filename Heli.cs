using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Heli : Script
    {

        InputChecker inputCheckerBomb = new InputChecker();
        bool AllF = false;
        Vehicle heli;
        Ped driver;
        public Heli()
        {

            Interval = 500;
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.H, Keys.E, Keys.L, Keys.I });
            this.Tick += new EventHandler(this.Bombat_Tick);
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }
        private void Reset()
        {
            if (Exists(heli))
            {
                if (heli.PetrolTankHealth > 0)
                {
                    heli.PetrolTankHealth = -1.0f;
                }
                heli.NoLongerNeeded();
            }
            if (Exists(driver))
            {
                driver.Money = 0;
                driver.Invincible = false;
                driver.NoLongerNeeded();
            }
        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (AllF == true)
            {
                if (!Player.Character.isAlive)
                {
                    Reset();
                    AllF = false;
                    return;
                }
                Vector3 Pos = Player.Character.Position;
                if (!Exists(heli))
                {
                    heli = GTA.World.CreateVehicle("MAVERICK",Pos.Around(5.0f));
                    if (Exists(driver)) { driver.Delete(); }
                    driver = heli.CreatePedOnSeat(VehicleSeat.Driver);
                

                    return;
                }
                else
                {
                    if (Player.Character.isInVehicle(heli) || Pos.DistanceTo(heli.Position)>50.0f || !heli.isAlive)
                    {
                        Reset();
                        AllF = false;
                    }
                    if (Exists(driver))
                    {
                        if (driver.isInVehicle(heli))
                        {
                            driver.Money = 500;
                            driver.Invincible = true;
                        }
                        else
                        {
                            driver.WarpIntoVehicle(heli, VehicleSeat.Driver);
                        }
                        
                    }
                    else
                    {
                        driver = heli.CreatePedOnSeat(VehicleSeat.Driver);
                        return;
                    }
                    Blip tar = GTA.Game.GetWaypoint();
                    if (Exists(tar))
                    {
                        if (tar.Position.DistanceTo(tar.Position) > 50)
                        {
                            driver.Task.DriveTo(tar.Position+new Vector3(0,0,20), 50, false);
                        }
                        else
                        {
                            driver.Task.DriveTo(tar.Position, 30, false);
                        }
                    }
                    else
                    {
                        Reset();
                        AllF = false;
                    }
                   
                }

            }



       }
        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);

            if (inputCheckerBomb.Check(0) == true)
            {
                if (AllF)
                {
                    Game.DisplayText("Heli OFF", 4000);
                    AllF = false;
                    Reset();

                }
                else
                {
                    Game.DisplayText("Heli ON", 4000);
                    AllF = true;
                    heli = null;
                    driver = null;
                }
            }
        }
    }
}
