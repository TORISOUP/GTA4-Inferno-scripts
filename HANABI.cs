using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Collections.Generic;
namespace TestScriptCS.Scripts
{
   
    public class HANABI : Script
    {
        int TimerPed, TimerVeh;

        static event EventHandler PedHanabiStart;
        static event EventHandler CarHanabiStart;
        HashSet<Ped> pedList;
        HashSet<Vehicle> carList;
        
        Vector3 PedPoint, CarPoint;


        public static void PedStart()
        {
            PedHanabiStart(new object(), new EventArgs());
        }

        public static void CarStart()
        {
           CarHanabiStart(new object(), new EventArgs());
        }


        public HANABI()
        {
            TimerPed = -1;
            TimerVeh = -1;

            carList = new HashSet<Vehicle>();
            pedList = new HashSet<Ped>();

            PedHanabiStart += new EventHandler(HANABI_PedHanabiStart);
            CarHanabiStart += new EventHandler(HANABI_CarHanabiStart);

            Interval = 30;
            this.Tick += new EventHandler(this.HANABI_Tick);

        }


        void HANABI_CarHanabiStart(object sender, EventArgs e)
        {

            TimerVeh = 90;
            CarPoint = Player.Character.Position.Around(10.0f) + new Vector3(0, 0, 13);


        }



        void HANABI_PedHanabiStart(object sender, EventArgs e)
        {
            TimerPed = 90;
            PedPoint = Player.Character.Position.Around(10.0f) + new Vector3(0, 0, 10);
        }

        private void HANABI_Tick(object sender, EventArgs e)
        {
            #region Ped
            if (TimerPed > 0)
            {
                TimerPed--;
                var ap = Cacher.GetPeds(Player.Character.Position, 50);
                foreach (var ped in ap)
                {
                    //
                    if (ped == Player.Character||  ped.isRequiredForMission || !Exists(ped)) { continue; }
                    pedList.Add(ped);
                }

                foreach (Ped p in pedList)
                {
                    if (Exists(p) )
                    {
                        Vector3 v2 = -(p.Position - PedPoint);
                        v2.Normalize();
                        if (p.isInVehicle())
                        {
                            p.Task.ClearAllImmediately();
                        }
                        p.ApplyForce(v2 * 10.0f);
                    }
                }
            }
            else if (TimerPed == 0)
            {
                TimerPed = -1;

                foreach (Ped p in pedList)
                {
                    if (Exists(p))
                    {
                        Vector3 v2 = (p.Position - PedPoint);
                        v2.Normalize();
                        if (p.isInVehicle())
                        {
                            p.Task.ClearAllImmediately();
                        }
                        p.ApplyForce(v2 * 10.0f);
                        p.Health = 0;
                    }
                }
                pedList.Clear();
                World.AddExplosion(PedPoint, ExplosionType.Rocket, 10.0f, true, false, 0.1f);
            }
            #endregion



            if (TimerVeh > 0)
            {
                TimerVeh--;

                if (carList.Count < 5)
                {
                    var av = Cacher.GetVehicles(Player.Character.Position, 100);
                    foreach (var veh in av)
                    {
                       
                        if (veh == Player.Character.CurrentVehicle || veh.isRequiredForMission || !Exists(veh)) { continue; }
                        if (!veh.isAlive) { continue; }
                        carList.Add(veh);
                    }

                    foreach (var car in carList)
                    {
                        if (Exists(car))
                        {
                            Vector3 v2 = -(car.Position - CarPoint);
                            v2.Normalize();

                            car.ApplyForce(v2 * 30.0f);
                        }
                    }
                }


            }
            else if (TimerVeh == 0)
            {
                TimerVeh=-1;


                foreach (var p in carList)
                {
                    if (Exists(p))
                    {
                        Vector3 v2 = (p.Position - CarPoint);
                        v2.Normalize();
                        p.ApplyForce(v2 * 20.0f);
                        p.PetrolTankHealth = -800;
                    }
                }
                carList.Clear();
                World.AddExplosion(CarPoint, ExplosionType.Rocket, 10.0f, true, false, 0.1f);
            }


        }


    }
}
