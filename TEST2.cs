using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
 
    public class TEST : Script
    {
        Random rnd;

        public TEST()
        {
            rnd = new Random();
            Interval = 5000;
            this.Tick += new EventHandler(this.MOD_Tick);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
        }

        // Interval[ms]間隔で実行される
        private void MOD_Tick(object sender, EventArgs e)
        {


        }

        //キー入力があると実行される
        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            //キーボードのKが押されたら
            //30m以内の車が一斉に発火する

            if (e.Key == Keys.K)
            {
                Vehicle[] V = World.GetVehicles(Player.Character.Position, 100.0f);

                for (int i = 0; i < V.Length; i++)
                {
                    if (!Exists(V[i])) { continue; }
                    if (V[i] == Player.Character.CurrentVehicle) { continue; }

                    Ped ped = V[i].GetPedOnSeat(VehicleSeat.Driver);
                    if (!Exists(ped)) { continue; }

                    float vel = rnd.Next(30, 70);
                    V[i].Speed += vel;

                    World.AddExplosion(V[i].Position, ExplosionType.Default, 0, true, false, 1.0f);
                    


                }


            }
        }
    }
}
