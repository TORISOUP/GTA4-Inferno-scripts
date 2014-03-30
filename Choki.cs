using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Choki : Script
    {
        //回復＆チョッキ
        InputChecker inputCheckerChoki = new InputChecker();
        bool AllF = false;
        bool provision = false;  //trueなら回復＆チョッキ配布
        bool Missions = false;  //ミッション中かどうか

        public Choki()
        {
            if (GTA.Native.Function.Call<bool>("GET_MISSION_FLAG")) //意味もなくネイティブ関数でミッションフラグ取得
            {
                Missions = true;
            }
            Interval = 100;
            inputCheckerChoki.AddCheckKeys(new Keys[] { Keys.C, Keys.H, Keys.O, Keys.K, Keys.I });
            inputCheckerChoki.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            this.Tick += new EventHandler(this.Choki_Tick);
            KeyDown += new GTA.KeyEventHandler(Choki_KeyDown);
        }
        private void Choki_Tick(object sender, EventArgs e)
        {
            if (AllF == true)
            {
                if (!Player.Character.isAlive) { provision = true; }    //死んだら配布準備
                if (GTA.Native.Function.Call<bool>("GET_MISSION_FLAG")) //意味もなくネイティブ関数でミッションフラグ取得
                {
                    if (Missions == false)
                    {
                        //ミッション開始直後のみに配布準備
                        Missions = true;
                        provision = true;
                    }
                }
                else
                {
                    Missions = false;
                }
                if (provision && Player.Character.isAlive)
                {
                    //プレイヤーが生きていて、かつ配布準備されているなら
                    Game.DisplayText("The armor was supplied. ", 4000);
                    Player.Character.Armor = 100;
                    Player.Character.Health = 200;
                    provision = false;
                }

                
            }


       }
        void Choki_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerChoki.AddInputKey(e.Key);
            if (inputCheckerChoki.Check(1) == true)
            {
                AllF = true;
            }
            if (inputCheckerChoki.Check(0) == true)
            {
                if (AllF)
                {
                    Game.DisplayText("Armor OFF", 4000);
                    AllF = false;
                }
                else
                {
                    Game.DisplayText("armor ON", 4000);
                    AllF = true;
                }
            }
        }
    }
}
