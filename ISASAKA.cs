using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Collections.Generic;
namespace TestScriptCS.Scripts
{
   
    public class ISASAKA : Script
    {

        bool IsonoActive;
        System.Diagnostics.Stopwatch stopWatch;
        

        static event EventHandler IsonoStartHandler;

        public static void ISASAKAStart()
        {
            IsonoStartHandler(new object(), new EventArgs());
        }




        public ISASAKA()
        {
            IsonoStartHandler += ISONO_IsonoStartHandler;

            Interval = 100;
            this.Tick += new EventHandler(this.ISONO_Tick);
            stopWatch = new System.Diagnostics.Stopwatch();


        }



        void ISONO_IsonoStartHandler(object sender, EventArgs e)
        {
            if (!IsonoActive)
            {
                stopWatch.Restart();
                IsonoActive = true;
            }
        }



        private void ISONO_Tick(object sender, EventArgs e)
        {
            if (!IsonoActive) { return; }
            if (stopWatch.ElapsedMilliseconds / 1000.0f > 20 ) {
                var AO1 = World.GetAllObjects();
                foreach (var obj in AO1)
                {
                    if (!Exists(obj)) { continue; }
                    var leng = (obj.Position - Player.Character.Position).Length();
                    if (leng < 100.0f)
                    {
                        obj.Velocity = new Vector3();
                        obj.FreezePosition = false;
                    }

                }

                var AV1 = Cacher.GetVehicles(Player.Character.Position, 100);
                foreach (var vec in AV1)
                {
                    if (!Exists(vec) || Player.Character.CurrentVehicle == vec) { continue; }
                    vec.FreezePosition = false;
                }

                var AP1 = Cacher.GetPeds(Player.Character.Position, 100);
                foreach (var ped in AP1)
                {
                    if (!Exists(ped) || ped == Player.Character) { continue; }
                    ped.FreezePosition = false;
                }

                IsonoActive = false; return; 
            }


            var AO = World.GetAllObjects();
            foreach (var obj in AO)
            {
                if (!Exists(obj)) { continue; }
                var leng = (obj.Position - Player.Character.Position).Length();
                if (leng < 30.0f)
                {

                    try
                    {
                        obj.FreezePosition = true;
                    }
                    catch
                    {
                    }
                }
                
            }

            var AV = Cacher.GetVehicles(Player.Character.Position, 30);
            foreach (var vec in AV)
            {
                if (!Exists(vec) ||  vec.isRequiredForMission) { continue; }
                try
                {
                    if (Player.Character.CurrentVehicle == vec)
                    {
                        vec.FreezePosition = false;
                    }
                    else
                    {
                        vec.FreezePosition = true;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            var AP = Cacher.GetPeds(Player.Character.Position, 30);
            foreach (var ped in AP)
            {
                if (!Exists(ped) || ped == Player.Character || ped.isRequiredForMission) { continue; }
                try
                {
                    ped.PreventRagdoll = false;
                    ped.isRagdoll = true;
                    
                    ped.ApplyForce(new Vector3(0,0,100));
                    ped.ForceRagdoll(6000, false);
                  //  ped.FreezePosition = true;
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }


        }


    }
}
