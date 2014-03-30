using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Collections.Generic;
namespace TestScriptCS.Scripts
{

    public class VOLGA : Script
    {

        bool  InitFlag;
        Ped ped;
        System.Diagnostics.Stopwatch stopwatch;
        Random rnd;

        static event EventHandler IsonoStartHandler;

        public static void VOLGAStart()
        {

            IsonoStartHandler(new object(), new EventArgs());
        }




        public VOLGA()
        {
            IsonoStartHandler += ISONO_IsonoStartHandler;
            rnd = new Random();
            Interval = 100;
            this.Tick += new EventHandler(this.ISONO_Tick);
            stopwatch = new System.Diagnostics.Stopwatch();


        }


        void ISONO_IsonoStartHandler(object sender, EventArgs e)
        {
            stopwatch.Restart();
            InitFlag = true;
            ped = GTA.World.CreatePed(Player.Character.Position.Around(rnd.Next(20)) + new Vector3(0, 0, 50));
        }



        private void ISONO_Tick(object sender, EventArgs e)
        {
            if (InitFlag)
            {

                if (!Exists(ped) || stopwatch.ElapsedMilliseconds>10*1000)
                {
                    stopwatch.Stop();
                    InitFlag = false;
                    return;
                }

                if (ped.HasBeenDamagedBy(Weapon.Misc_Fall) || ped.isDead)
                {
                    var vec = ped.Position;
                    ped.NoLongerNeeded();
                    World.AddExplosion(vec, ExplosionType.Rocket, 20.0f);

                    var AP = Cacher.GetPeds(vec, 50.0f);
                    var AV = Cacher.GetVehicles(vec, 50.0f);
                    GTA.Object[] AO = World.GetAllObjects();

                    for (int i = 0; i < AP.Length; i++)
                    {
                        if (!Exists(AP[i])) { continue; }
                        if (AP[i] == Player.Character)
                        {
                            AP[i].ApplyForce(100 * (AP[i].Position - vec));
                            var GuidOfScript2 = new Guid("CC62497C-E738-11DF-8390-560BDFD72085");
                            SendScriptCommand(GuidOfScript2, "FTB");
                        }
                        AP[i].ApplyForce(100 * (AP[i].Position - vec));


                    }
                    for (int i = 0; i < AV.Length; i++)
                    {
                        if (!Exists(AV[i])) { continue; }

                        AV[i].ApplyForce(10 * (AV[i].Position - vec));

                    }

                    for (int i = 0; i < AO.Length; i++)
                    {
                        if (!Exists(AO[i])) { continue; }
                        if (AO[i].Position.DistanceTo(vec) < 50.0f)
                        {
                            AO[i].ApplyForce(30 * (AO[i].Position - vec));
                        }
                    }

                    InitFlag = false;
                }
            }


        }

    }


    
}
