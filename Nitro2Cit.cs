using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Drawing.Imaging;
using System.Drawing;

namespace TestScriptCS.Scripts
{
    //市ニトロ

    public class Nitor2 : Script
    {
        InputChecker inputCheckerN2C = new InputChecker();
        Vehicle[] allV;
        bool ActivateFlag = false;
        Random rnd = new Random();
        bool TrendNitoro;
        int Time;
        bool RemoveMissions;


        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            GTA.Font screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);

            if (TrendNitoro && Player.CanControlCharacter && Player.Character.isAlive)
            {
                if ((Time / 2) % 2 == 1)
                {
                    e.Graphics.DrawText("※市ニトロ注意※", new RectangleF(0.0f, 0.15f, 1.0f, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
                }
           //     e.Graphics.DrawRectangle(new RectangleF(0, 0, 1.0f, 0.05f), Color.FromArgb(30, 0, 0, 255));
            }
        }

        public Nitor2()
        {
            RemoveMissions = true;  //ミッション関係車両を除外するか
            TrendNitoro = false;
            Interval = 3000;
            inputCheckerN2C.AddCheckKeys(new Keys[] { Keys.D, Keys.A, Keys.N, Keys.G});
            inputCheckerN2C.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            this.Tick += new EventHandler(this.N2C_Tick);
            KeyDown += new GTA.KeyEventHandler(N2C_KeyDown);
            allV = Cacher.GetVehicles(Player.Character.Position, 100.0f);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }

        private bool IsTaxi(Vehicle vec)
        {
            Model model = vec.Model;

            if ((long)(model.Hash) == 3338918751 || model.Hash == 0x480DAF95 ||model.Hash == -956048545 || model.Hash == 1884962369 ||  model == Model.TaxiCarModel)
            {
                return true;
            }
            return false;
        }

        private void N2C_Tick(object sender, EventArgs e)
        {
            if (ActivateFlag == true)
            {
                if (rnd.Next(0, 300) < 2 && TrendNitoro==false)
                {
                    //たまにTrend_Nitroを発生させる
   
                    TrendNitoro = true;
                    Time = rnd.Next(30*2, 60*2);
                    Game.DisplayText("Trend_Nitro", 5000);
                    Interval = 500; //普段の6倍実行する（readmeと違う
                }

                allV = Cacher.GetVehicles(Player.Character.Position, 100.0f);
                int Length = allV.Length;
                if (Length <= 0) { return; }
                for (int i = 0; i < Length; i++)
                {
                    if (Exists(allV[i]))
                    {
                        if (allV[i] != Player.Character.CurrentVehicle)
                        {
                            if (!allV[i].isRequiredForMission || !RemoveMissions)
                            {
                                Ped a = allV[i].GetPedOnSeat(VehicleSeat.Driver);

                                if ( (Exists(a) == true && (rnd.Next(0, 100) < 3 )) || (Share.PonkotsuInferno )  )
                                {
                                    //3%の確率で市ニトロ発動

                                    if (rnd.Next(0, 101) < 50)
                                    {
                                        // 1/2の確率で緊急脱出させる
                                        try
                                        {
                                            Vector3 Po = allV[i].Position;
                                            if (Exists(a))
                                            {
                                                a.Task.ClearAllImmediately();
                                                a.ApplyForce(new Vector3(0, 0, 5));
                                            }
                                        }
                                        catch
                                        {
                                            Game.DisplayText("NITRO2 ERROR", 4000);
                                            return;
                                        }

                                    }

                                    float Speed = rnd.Next(50, 100);    //速度
                                    if (rnd.Next(0, 100) <= 30) { Speed = -Speed; } // 30%の確率でバックニトロ

                                    if (Speed > 0 && rnd.Next(0,100)<=10)
                                    {
                                        allV[i].Rotation = new Vector3(allV[i].Rotation.X + (rnd.Next(0,100) * 60) / 100, allV[i].Rotation.Y, allV[i].Rotation.Z);
                                    }

                                    allV[i].Speed = Speed;  //速度を強制的に変更
                                    GTA.Native.Function.Call("ADD_EXPLOSION", allV[i].Position.X, allV[i].Position.Y, allV[i].Position.Z, 3, 0.0f, 30, 0, 0.1f);    //ダメージ０の見た目だけの爆風を生成

                                    if (rnd.Next(0, 100) < 40 && allV[i].PetrolTankHealth > 0)
                                    {
                                        //40%の確率で発火させる
                                        allV[i].PetrolTankHealth = -rnd.Next(700, 990);
                                    }

                                }
                            }
                        }
                    }
                }
                if (--Time == 0)
                {
                    TrendNitoro = false;
                    Game.DisplayText("Trend_Nitro End", 5000);
                    Interval = 3000;
                }
                   
            }
       }
        void N2C_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerN2C.AddInputKey(e.Key);

            if (e.Key == Keys.NumPad6)
            {
                if (RemoveMissions)
                {
                    Game.DisplayText("[Nitro2Cit] No Exclude Mission Vehicles.", 5000);
                }
                else
                {
                    Game.DisplayText("[Nitro2Cit] Exclude Mission Vehicles.", 5000);
                }
                RemoveMissions = !RemoveMissions;
            }

            if (inputCheckerN2C.Check(1) == true)
            {
                ActivateFlag = true;
            }
            if (inputCheckerN2C.Check(0) == true)
            {
                if (ActivateFlag)
                {
                    Game.DisplayText("Nitro2 OFF", 4000);
                    ActivateFlag = false;
                }
                else
                {
                    Game.DisplayText("Nitro2 ON", 4000);
                    ActivateFlag = true;
                }
            }
        }
    }
}
