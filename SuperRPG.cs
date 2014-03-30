using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //吹っ飛び

    public class SuperRPG : Script
    {
        InputChecker inputCheckerAll = new InputChecker();
        bool AllF = false;
        bool Ald;
        bool FlyNow = false;    //吹っ飛んでる間はtrue
        float OldHealth;
        Random rnd;
        public SuperRPG()
        {
            GUID = new Guid("CC62497C-E738-11DF-8390-560BDFD72085");
            BindScriptCommand("FTB", new ScriptCommandDelegate(FTB));
            OldHealth = Player.Character.Health;
            Ald = false;
            rnd = new Random();
            Interval = 50;
            inputCheckerAll.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            inputCheckerAll.AddCheckKeys(new Keys[] { Keys.S, Keys.R, Keys.P, Keys.G});
            this.Tick += new EventHandler(this.SRPG_Tick);
            KeyDown += new GTA.KeyEventHandler(SRPG_KeyDown);

        }
        private void FTB(GTA.Script sender, GTA.ObjectCollection Parameter)
        {
            if (AllF)
            {
                Player.CanControlRagdoll = true;
                Player.Character.isRagdoll = true;
                FlyNow = true;
            }
        }

        //最後に受けたダメージが爆風（＋素手）によるものなのか調べる関数
        private bool isCharDamagedByExplosions(Ped ped)
        {
            if (ped.HasBeenDamagedBy(Weapon.Unarmed) || ped.HasBeenDamagedBy(Weapon.Heavy_RocketLauncher) || ped.HasBeenDamagedBy(Weapon.Misc_Rocket) || ped.HasBeenDamagedBy(Weapon.Misc_Explosion) || ped.HasBeenDamagedBy(Weapon.Thrown_Grenade))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SRPG_Tick(object sender, EventArgs e)
        {
            if (!AllF) { return; }
            if (Player.Character.isAlive)
            {
                Ald = true;

                if (FlyNow)
                {
                    //吹っ飛んだ後
                    if (Player.Character.Velocity.Length() < 2.0f)
                    {
                        //吹っ飛び後に減速したら

                        Player.Character.MakeProofTo(false, false, false, false, false);    //全てのダメージ耐性を元に戻す
                        FlyNow = false;                 //吹っ飛び終了
                        Player.CanControlRagdoll = false;   //やる気を出す
                        Player.Character.isRagdoll = false; //
                    }
                }

                if (OldHealth - Player.Character.Health > 0)
                {
                    if (isCharDamagedByExplosions(Player.Character))
                    {
                        //ダメージを受けたとき、それが爆風によるものなら
                        Player.Character.MakeProofTo(false, false, false, true, true);  //落下ダメージと衝突ダメージ耐性を付ける
                        FlyNow = true;      //吹っ飛び開始
                        Vector3 force = Player.Character.Velocity;
                        force = force * rnd.Next(200, 1000);
                        Player.Character.ApplyForce(force);     //吹っ飛び速度を上げる
                        Player.CanControlRagdoll = true;        //やる気をなくす
                        Player.Character.isRagdoll = true;
                       
                    }
                }
                OldHealth = Player.Character.Health;



            }
            else
            {
                if (isCharDamagedByExplosions(Player.Character))
                {
                    if (Ald)
                    {
                        Ald = false;
                        if (Player.Character.HasBeenDamagedBy(Player.Character))
                        {
                            //自爆でも吹っ飛ぶ
                            if (!Player.Character.isInVehicle())
                            {
                                float x = (float)rnd.Next(0, 800)-400.0f;
                                float y = (float)rnd.Next(0, 800)-400.0f;
                                float z = (float)rnd.Next(0, 800);
                                Player.Character.ApplyForce(new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }


       }
        void SRPG_KeyDown(object sender, GTA.KeyEventArgs e)
        {

            inputCheckerAll.AddInputKey(e.Key);
            if (inputCheckerAll.Check(0))
            {
                AllF = true;
            }
            if (inputCheckerAll.Check(1))
            {
                if (AllF)
                {
                    Game.DisplayText("SRPG OFF", 4000);
                    AllF = false;
                }
                else
                {
                    Game.DisplayText("SRPG ON", 4000);
                    AllF = true;
                }
            }
        }
    }
}
