using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //市民暴走運転
    public class MAXSPEED : Script
    {
        InputChecker inputCheckerMaxSpeed = new InputChecker();
        Vehicle[] allV;
        bool ActivateFlag = false;
        Random rnd = new Random();

        public MAXSPEED()
        {
            Interval = 3000;
            inputCheckerMaxSpeed.AddCheckKeys(new Keys[] { Keys.R, Keys.U, Keys.N, Keys.A, Keys.W, Keys.A, Keys.Y });
            inputCheckerMaxSpeed.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            this.Tick += new EventHandler(this.MaxSpeed_Tick);
            KeyDown += new GTA.KeyEventHandler(MaxSpeed_KeyDown);
            allV = Cacher.GetVehicles(Player.Character.Position, 100.0f);
        }
        private void MaxSpeed_Tick(object sender, EventArgs e)
        {
            if (ActivateFlag == true)
            {
                try
                {
                    allV = Cacher.GetVehicles(Player.Character.Position, 80.0f);
                    int Length = allV.Length;
                    if (Length <= 0) { return; }
                    for (int i = 0; i < Length; i++)
                    {
                        if (Exists(allV[i]))
                        {
                            if (allV[i] != Player.Character.CurrentVehicle && !allV[i].isRequiredForMission)
                            {
                                Ped a = allV[i].GetPedOnSeat(VehicleSeat.Driver);
                                if (Exists(a) == true)
                                {
                                    //ミッション関係でない車にドライバーがいたら周回速度の上限を300mphにして交通ルールを無視させる
                                    a.Task.CruiseWithVehicle(allV[i], 300.0f, false);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Game.DisplayText("Run away ERROR!", 4000);
                    ActivateFlag = false;
                }
                   
            }
       }
        void MaxSpeed_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMaxSpeed.AddInputKey(e.Key);
            if (inputCheckerMaxSpeed.Check(1) == true)
            {
                ActivateFlag = true;
            }
            if (inputCheckerMaxSpeed.Check(0) == true)
            {
                if (ActivateFlag)
                {
                    Game.DisplayText("Run away OFF", 4000);
                    ActivateFlag = false;
                }
                else
                {
                    Game.DisplayText("Run away ON", 4000);
                    ActivateFlag = true;
                }
            }
        }
    }
}
