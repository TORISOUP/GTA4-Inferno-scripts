using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //ボンバット改 & 危ナイフ
    public class Bombat : Script
    {
        InputChecker inputCheckerBomb = new InputChecker();
        bool AllF = false;
        bool bombat = true; //trueなら殴られた時に発動
        bool knife = true;  //trueなら刺されたとき発動
        Ped[] ped;
        Random rnd;

        public Bombat()
        {
            rnd = new Random();
            Interval = 200;
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.B, Keys.A, Keys.T, Keys.M, Keys.A, Keys.N });
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            this.Tick += new EventHandler(this.Bombat_Tick);
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);

        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (AllF == true)
            {
                if (!Player.Character.HasBeenDamagedBy(Weapon.Melee_BaseballBat))
                {
                    //バット以外のダメージなら
                    bombat = true;
                }
                else
                {
                    if (bombat == true)
                    {
                        //バットでダメージを受けており、かつ既に爆発していないなら
                        bombat = false;

                        //意味もなくネィティブ関数で爆発を生成
                        GTA.Native.Function.Call("ADD_EXPLOSION", Player.Character.Position.X, Player.Character.Position.Y, Player.Character.Position.Z, 0, 10.0f, 30, 0, 0.5f);

                        //プレイヤーを死亡させる
                        Player.Character.Die();

                        //F:力
                        //R:モーメント（仕様上効果無し？
                        Vector3 F, R;
                        F = new Vector3(rnd.Next(5, 100), rnd.Next(5, 100), rnd.Next(5, 100));
                        R = new Vector3(rnd.Next(5, 100), rnd.Next(5, 100), rnd.Next(5, 100));
                        F.Normalize();
                        R.Normalize();
                        F = F * rnd.Next(600, 1000);
                        R = R * rnd.Next(100, 200);

                        //適当な方向にふっとばす
                        Player.Character.ApplyForceRelative(F, R);

                    }
                }
                if (!Player.Character.HasBeenDamagedBy(Weapon.Melee_Knife))
                {
                    //ナイフ以外のダメージなら
                    knife = true;
                }
                else
                {
                    if (knife == true)
                    {
                        knife = false;
                        //燃やす
                        GTA.Native.Function.Call("ADD_EXPLOSION", Player.Character.Position.X, Player.Character.Position.Y, Player.Character.Position.Z, 1, 10.0f, 30, 0, 0.5f);
                    }
                }

                try
                {
                    ped = World.GetPeds(Player.Character.Position, 50.0f);

                    for (int i = 0, length = ped.Length; i < length; i++)
                    {
                        if (ped[i] == Player.Character) { continue; }
                        if (ped[i].isAlive && ped[i].HasBeenDamagedBy(Weapon.Melee_BaseballBat))
                        {
                            Vector3 F;
                            F = new Vector3(rnd.Next(5, 100), rnd.Next(5, 100), rnd.Next(30, 100));
                            F.Normalize();
                            F = F * rnd.Next(20, 70);
                            ped[i].Velocity = F;
                            ped[i].Die();
                        }
                    }
                }
                catch
                {
                    //市民取得に失敗したとき
                    ;
                }



            }
       }
        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);
            if (inputCheckerBomb.Check(1) == true)
            {
                Game.DisplayText("ALL ON", 4000);
                AllF = true;
            }
            if (inputCheckerBomb.Check(0) == true)
            {
                if (AllF)
                {
                    Game.DisplayText("BOMBAT OFF", 4000);
                    AllF = false;
                }
                else
                {
                    Game.DisplayText("BOMBAT ON", 4000);
                    AllF = true;
                }
            }
        }
    }
}
