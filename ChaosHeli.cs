using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Threading;

//カオスヘリ
namespace TestScriptCS.Scripts
{

    public class ChaosHeli : Script
    {

        InputChecker inputCheckerBomb = new InputChecker();
        bool AllF = false;
        Vehicle heli;
        Ped driver;
        Random rnd;
        int Timer;
        Ped A;
       
        bool CameraFlag = false;
        Ped[] p;
        public ChaosHeli()
        {
            rnd = new Random();
            p = new Ped[3];
            Interval = 500;
            inputCheckerBomb.AddCheckKeys(new Keys[] {Keys.C, Keys.H, Keys.E, Keys.L, Keys.I });
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
 
            this.Tick += new EventHandler(this.Bombat_Tick);
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }

        //ヘリと乗ってる市民のメモリを解放する
        private void Reset()
        {
            if (Exists(heli)) { heli.NoLongerNeeded(); }
            if (Exists(driver)) { driver.NoLongerNeeded(); }
            if (Exists(p[0])) { p[0].NoLongerNeeded(); }
            if (Exists(p[1])) { p[1].NoLongerNeeded(); }
            if (Exists(p[2])) { p[2].NoLongerNeeded(); }
            heli = null;
            driver = null;
            p[0] = null;
            p[1] = null;
            p[2] = null;
        }

        //ヘリを生成して、市民を中に乗せる
        private void CreateHeli()
        {
                
                heli = GTA.World.CreateVehicle("MAVERICK",Player.Character.Position + new Vector3(0,0,100));
                if (Exists(heli))
                {
                    //ヘリ生成に成功したら

                    heli.MakeProofTo(false, true, true, true, true);
                    heli.EngineHealth = 3000;
                    heli.PetrolTankHealth = 3000;
                    if (Exists(driver)) { driver.NoLongerNeeded(); } 
                    driver = heli.CreatePedOnSeat(VehicleSeat.Driver);
                    if (Exists(driver))
                    {
                        driver.Money = 500;
                        driver.Invincible = true;
                    }

                    if (Exists(p[0])) { p[0].NoLongerNeeded(); }
                    if (Exists(p[1])) { p[1].NoLongerNeeded(); }
                    if (Exists(p[2])) { p[2].NoLongerNeeded(); }

                    p[0] = heli.CreatePedOnSeat(VehicleSeat.LeftRear);
                    p[1] = heli.CreatePedOnSeat(VehicleSeat.RightFront);
                    p[2] = heli.CreatePedOnSeat(VehicleSeat.RightRear);
                    for (int i = 0; i < 3; i++)
                    {
                        if (Exists(p[i]))
                        {
                            p[i].MaxHealth = 500;
                            p[i].Health = 500;
                            
                        }
                    }
                        Timer = 40 * 2;
                }

      
        }

        //メイン処理
        private void Bombat_Tick(object sender, EventArgs e)
        {


            if (AllF == true)
            {
                //ここからヘリカメラの処理
                if (Game.isGameKeyPressed(GameKey.LookBehind))
                {
                    if (Game.isGameKeyPressed(GameKey.RadarZoom) || CameraFlag)
                    {
                        CameraFlag = true;
                    }
                }

                if (Game.isGameKeyPressed(GameKey.LookBehind) && !Game.isGameKeyPressed(GameKey.RadarZoom))
                {
                    CameraFlag = false;
                }

                if (Player.Character.isDead || !Player.Character.isInVehicle())
                {
                    CameraFlag = false;
                }

                //ここからカオスヘリ自体の処理

                if (Player.Character.isDead)
                {
                    //主人公が死んだらヘリを解放
                    Reset();
                    Timer = 0;
                    return;
                }
                else if (Exists(heli) && Player.Character.isInVehicle(heli))
                {
                    heli.PetrolTankHealth = -1.0f;
                    if (Exists(driver)) { driver.Invincible = false; }
                    Reset();
                    heli = null;
                }

                
                if (Timer < 0)
                {
                    //40秒に1回、ヘリを再生成すべきかチェック
                    if (Exists(heli) && (heli.Speed < 0.2f || heli.Position.DistanceTo2D(Player.Character.Position) > 150.0f))
                    {
                        heli.PetrolTankHealth = -1.0f;
                        Reset();
                    }
                    if (!Exists(heli) || !heli.isAlive)
                    {
                        Reset();
                        CreateHeli();
                    }
                    else
                    {
                        Timer = 40 * 2;
                    }
                }
                else
                {
                    if (!Exists(heli)) { Timer--; return; }

                    if (Exists(driver))
                    {
                        if (driver.isInVehicle())
                        {
                            driver.Task.DriveTo(heli, Player.Character.Position + new Vector3(0, 0, 20), 100, true, true);
                        }
                        else
                        {
                            driver.NoLongerNeeded();
                            driver.Money = 0;
                            driver.Invincible = false;
                            driver = heli.CreatePedOnSeat(VehicleSeat.Driver);
                            if (Exists(driver))
                            {
                                driver.Money = 500;
                                driver.Invincible = true;
                            }

                        }
                    }
                    if (Exists(p[0])) { if (!p[0].isInVehicle()) { p[0].NoLongerNeeded(); p[0] = heli.CreatePedOnSeat(VehicleSeat.LeftRear); } }
                    if (Exists(p[1])) { if (!p[1].isInVehicle()) { p[1].NoLongerNeeded(); p[1] = heli.CreatePedOnSeat(VehicleSeat.RightFront); } }
                    if (Exists(p[2])) { if (!p[2].isInVehicle()) { p[2].NoLongerNeeded(); p[2] = heli.CreatePedOnSeat(VehicleSeat.RightRear); } }
                }

                if (Exists(A)) { A.Delete(); }

                if (Exists(heli))
                {

                    if (Share.Nico_Engo > 0)
                    {
                        Share.Nico_Engo--;

                        A = World.CreatePed(heli.Position + new Vector3(0, 0, -2.5f));
                        Ped[] Tlist=new Ped[0];
                        try
                        {
                            Tlist = World.GetPeds(Player.Character.Position,100.0f);

                        }
                        catch
                        {
                            if (Exists(A)) { A.Delete(); }
                           
                        }

                        if (Exists(A))
                        {

                            //RPGを装備させる
                            A.Weapons.FromType(Weapon.Heavy_RocketLauncher).Ammo = 999;
                            A.Weapons.Select(Weapon.Heavy_RocketLauncher);
                            A.Visible = false;
                            Ped T = Tlist[rnd.Next(0,Tlist.Length)];


                            if (Exists(T))
                            {
                                var Apos = T.Position;
                                //真下に向かってRPGを発射させる
                                GTA.Native.Function.Call("FIRE_PED_WEAPON", A, Apos.X, Apos.Y, Apos.Z);




                            }
                            
                        }

                    }
                }
                else
                {
                    Share.Nico_Engo = 0;
                }




                Timer--;

            }



       }




        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);

            if (inputCheckerBomb.Check(0) == true)
            {
                if (AllF)
                {
                    Game.DisplayText("ChaosHeli OFF", 4000);
                    AllF = false;
                    CameraFlag = false;
                    Reset();

                }
                else
                {
                    Game.DisplayText("ChaosHeli ON", 4000);
                    AllF = true;
                    CameraFlag = false;
                    Reset();
                }
            }
            if (inputCheckerBomb.Check(1))
            {
                AllF = true;
                Reset();
            }
        }

    }
}
