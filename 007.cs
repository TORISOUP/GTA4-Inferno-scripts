using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //ボンドカー
    
    //やってる処理は車の周りにAさんを出してRPGを撃つだけ
    //車の傾き具合でAさんの出現位置、発射方向を決めるのでそこらへんがややこしい。

    public class x007 : Script
    {
        Ped[] ped;
        Vehicle Old;
        int cnt;
        bool Active;
        InputChecker inputCheckerBomb = new InputChecker();
        public x007()
        {
            Active = false;
            ped = new Ped[2];
            Interval = 100;
            cnt = 0;
            this.Tick += new EventHandler(this.Nikita_Tick);
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.B,Keys.O,Keys.N,Keys.D });
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }

        private void Nikita_Tick(object sender, EventArgs e)
        {
            if (!Active) { return; }

            if (Exists(Player.Character.CurrentVehicle))
            {
                if (Player.Character.CurrentVehicle.isRequiredForMission && GTA.Native.Function.Call<bool>("IS_CHAR_ON_ANY_HELI", Player.Character))
                {
                    return;
                }
            }

            if (cnt > 0)
            {
                cnt--;
                if (Player.Money > 0)
                {
                    Old.CanBeDamaged = false;
                }
            }
            else
            {
                if (cnt == 0)
                {
                    cnt--;
                    if (Exists(Old))
                    {
                        Old.CanBeDamaged = true;
                    }
                }
            }
            if (Player.Character.isAlive && Player.Character.isInVehicle() && Player.CanControlCharacter)
            {
                for (int i = 0; i < 2; i++)
                {
                    if(Exists(ped[i])){ped[i].Delete();}
                }
                if (Game.isGameKeyPressed(GameKey.Jump) && Game.isGameKeyPressed(GameKey.RadarZoom) &&  Share.POINTs-2>=0)
                {

                    Share.AddPoint(-2);
                    Vehicle pV = Player.Character.CurrentVehicle;
                    

                    
                    float x,y,z;
                    x=pV.Rotation.X * 0.01745329f;
                    y=pV.Rotation.Y * 0.01745329f;
                    z=pV.Rotation.Z * 0.01745329f;

                    float Rad;

                    if (GTA.Native.Function.Call<bool>("IS_CHAR_ON_ANY_BIKE", Player.Character))
                    {
                        Rad = 0.9f;
                    }
                    else if (GTA.Native.Function.Call<bool>("IS_BIG_VEHICLE", pV))
                    {
                        Rad = 3.0f;
                    }
                    else
                    {
                        Rad = 1.5f;
                    }
                    int S = 0;

                    Vector3[] V = new Vector3[2];

                    //車を正面から縦方向に分断する平面に対する法線ベクトル（Aさんの位置）
                    V[0] = new Vector3((float)(Rad*Math.Cos(z)*Math.Cos(y)) ,(float)(Rad*Math.Sin(z)*Math.Cos(y)),(float)(Rad*Math.Sin(-y)));
                    V[1] = -V[0];

                    ped[0] = GTA.World.CreatePed(pV.Position + V[0]);
                    ped[1] = GTA.World.CreatePed(pV.Position + V[1]);

                    for (int i = 0; i < 2; i++)
                    {
                        if (!Exists(ped[i])) { continue; }
                        ped[i].Money = 9;
                        if (Game.isGameKeyPressed(GameKey.Sprint))
                        {
                            if (!Exists(ped[i])) { continue; }
                            ped[i].Weapons.BarettaShotgun.Ammo = 3000;
                            if (!Exists(ped[i])) { continue; }
                            ped[i].Weapons.Select(Weapon.Shotgun_Baretta);
                            S = 5;
                        }
                        else
                        {
                            if (!Exists(ped[i])) { continue; }
                            ped[i].Weapons.RocketLauncher.Ammo = 3000;
                            if (!Exists(ped[i])) { continue; }
                            if (Game.CurrentEpisode != GameEpisode.TBOGT)
                            {
                                ped[i].Weapons.Select(Weapon.Episodic_18);
                            }
                            else
                            {
                                ped[i].Weapons.Select(Weapon.Episodic_10);
                            }
                            S = 50;
                        }
                        float targetRange = 300.0f;

                        //発射方向を決める（車正面前方300m先）
                        x = (pV.Rotation.X - 2.5f) * 0.01745329f;
                        Vector3 tar = new Vector3(pV.Position.X - targetRange * (float)Math.Cos(z - Math.PI / 2.0f) * (float)Math.Cos(x),
                                                  pV.Position.Y - targetRange * (float)Math.Sin(z - Math.PI / 2.0f) * (float)Math.Cos(x), 
                                                  pV.Position.Z + targetRange * (float)Math.Sin(x));

                        if (!Exists(ped[i])) { continue; }
 
                        GTA.Native.Function.Call("FIRE_PED_WEAPON", ped[i], tar.X,tar.Y,tar.Z);


                      
                    }
                    Player.Money -= S;
                    Old = pV;
                    Old.CanBeDamaged = false;
                    cnt = 20;
                }
            }

        }
        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);

            if (inputCheckerBomb.Check(0) == true)
            {
                if (Active)
                {
                    Game.DisplayText("007 OFF", 4000);
                    Active = false;
      

                }
                else
                {
                    Game.DisplayText("007 ON", 4000);
                    Active = true;
      
                }
            }
            if (inputCheckerBomb.Check(1))
            {
                Active = true;
            }
        }
  
    }
}
