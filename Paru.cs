using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using WMPLib;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


namespace TestScriptCS.Scripts
{
    
    //パルプンテMOD
    //基本的な流れは発動時にNumberが乱数により決まり、その値でswtich文を通りその番号の処理が実行される。
    //その効果の処理は100msに1回繰り返し実行され、実行中はパルプンテを発動することはできない。
    //その処理の中で初期化処理が必要な場合はInitFlagを使う。
    //効果を終了させる時にはEndFlagをtrueにする。
    //また強制的に効果を中止させるときはStopFlagを使う。

    //一定時間動作させたければTimerを使う。
    //AV,AO,APはそれぞれ車両、オブジェクト、市民用の配列。


    //ゲーム中に表示される文章の書き方は

    //Game.DisplayText("No.xxx ここに書いた文が左上に", 4000);
    //msgBuff = "ここに書いた文が画面下に表示される";
    //IntervalTimer = 30;   //表示する時間(100ms単位 　『30』で3秒間)

    public class Parupunte : Script
    {
        
        InputChecker inputCheckerMOD = new InputChecker();
        bool ActiveFlag,EndFlag,ChoseFlag,InitFlag;
        bool StopFlag;
        Model PlayerModel;
        GTA.value.PlayerSkin skin;
        int Timer;
        int Number;
        bool FLAG_1;
        int IntervalTimer;
        Random rnd;
        Camera cam;
        Vector3 Pos;
        Ped[] AP;
        Vehicle[] AV;
        GTA.Object[] AO;
        Ped ped;
        string msgBuff,miniMsg;
        GTA.Font screenFont;
        GTA.Font miniFont;
        Vector3 vec;
        GTA.Object OBJ;
        Guid GuidOfScript2;
        int MaxTimer;
        Light light;

        readonly string PlayerName;

        VehicleSeat GetSheat(Vehicle V, Ped P)
        {
            if (V.GetPedOnSeat(VehicleSeat.Driver) == P) { return VehicleSeat.Driver; }
            if (V.GetPedOnSeat(VehicleSeat.RightFront) == P) { return VehicleSeat.RightFront; }
            if (V.GetPedOnSeat(VehicleSeat.RightRear) == P) { return VehicleSeat.RightRear; }
            if (V.GetPedOnSeat(VehicleSeat.LeftRear) == P) { return VehicleSeat.LeftRear; }
            return VehicleSeat.AnyPassengerSeat;
        }

        int GetGoodness()
        {
            int[] G= {0,9,23,24,27,29,30,32,33,37,42,46,51,53,54,55};
            return G[rnd.Next(0,G.Length)];
        }

        public Parupunte()
        {

         
            screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
            miniFont = new GTA.Font(0.03F, FontScaling.ScreenUnits);
            cam = GTA.Game.CurrentCamera;
            InitFlag = false;
            ChoseFlag = false;
            ActiveFlag = false;
            EndFlag = true;
            StopFlag = false;
            PlayerModel = Player.Model;
            skin = Player.Skin;
            FLAG_1 = false;
            Timer = 0;
            MaxTimer = -1;
            Number = -1;
            rnd = new Random();
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.R, Keys.N, Keys.T });
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.S, Keys.N,Keys.T});
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.A, Keys.S, Keys.N, Keys.T });
            Interval = 100;
            this.Tick += new EventHandler(this.MOD_Tick);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

            switch (Game.CurrentEpisode)
            {
                case GameEpisode.GTAIV:
                    PlayerName = "ニコ";
                    break;

                case GameEpisode.TBOGT:
                    PlayerName = "ルイス";
                    break;
                case GameEpisode.TLAD:
                    PlayerName = "ジョニー";
                    break;
            }
        }

        private void MOD_Tick(object sender, EventArgs e)
        {


            if (Share.Nico_Parupunte && ActiveFlag && EndFlag)
            {
                Share.AddPoint(10);
                IntervalTimer = 15;
                ChoseFlag = true;
                screenFont.Color = Color.White;
                msgBuff = PlayerName + "はパルプンテを唱えた！";
                Share.Nico_Parupunte = false;
                return;
            }


            //
            #region ActiveFlag更新

            if (Share.POINTs - 10 >= 0)
            {
                ActiveFlag = true;
            }
            else
            {
                ActiveFlag = false;
            }

            if (Player.Character.isDead) { StopFlag = true; Timer = -1; }
            /*
            if (!Player.Character.isAlive) { provision = true; StopFlag = true; }    //死んだら配布準備
            if (GTA.Native.Function.Call<bool>("GET_MISSION_FLAG")) //意味もなくネイティブ関数でミッションフラグ取得
            {
                if (Missions == false)
                {
                    
                    Missions = true;
                    provision = true;
                }
            }
            else
            {
                Missions = false;
            }
            if (provision && Player.Character.isAlive && !ActiveFlag)
            {
               
                ActiveFlag = true;
                provision = false;
            }
             * */


            #endregion

            if (Player.Character.isDead && FLAG_1)
            {
                FLAG_1 = false;
                GTA.World.PedDensity = 1.0f;
            }

            if (IntervalTimer > -1)
            {
                IntervalTimer--;
            }



            if (ChoseFlag)
            {
                if (IntervalTimer == -1)
                {
                    MaxTimer = -1;
                    Timer = -1;
                    ChoseFlag = false;
                    EndFlag = false;
                    InitFlag = true;
                    StopFlag = false;
                    if (rnd.Next(0, 100) < 20)
                    {
                        Number = GetGoodness();
                    }
                    else
                    {
                        Number = rnd.Next(0, 69);   //効果を増やしたらここの数値を変えること
                    }
             //       Number = 67;
          //         Number = 65;
           //              Number = rnd.Next(65, 67);

          
                        Share.AddPoint(-10);
               

                    screenFont.Color = Color.White;
                }
            }
            else if (!EndFlag)
            {
                try
                {
                    //ここがメインの処理部
                    switch (Number)
                    {
                        #region case0~14
                        case 0: //主人公全回復
                            Game.DisplayText("No.000 全回復", 4000);
                            msgBuff = "でもRPGで一撃ですけどね";
                            IntervalTimer = 30;
                            Player.Character.Armor = 100;
                            Player.Character.Health = 200;
                            EndFlag = true;
                            break;

                        case 1: //オワタ式
                            Game.DisplayText("No.001 オワタ式", 4000);
                            msgBuff = "オワタ式の可能性";
                            IntervalTimer = 30;
                            Player.Character.Armor = 0;
                            Player.Character.Health = 1;
                            EndFlag = true;
                            break;

                        case 2: //手配度消失
                            if (Player.WantedLevel > 0)
                            {
                                Game.DisplayText("No.002 手配度消失", 4000);
                                msgBuff = "無罪放免";
                                IntervalTimer = 30;
                                Player.WantedLevel = 0;
                                EndFlag = true;
                            }
                            else
                            {
                                ChoseFlag = true;
                            }
                            break;
                        case 3:
                            Game.DisplayText("No.003 手配度＋4", 4000);
                            msgBuff = "日頃の行いが悪い";
                            IntervalTimer = 30;
                            int w = Player.WantedLevel + 4;
                            if (w > 6) { w = 6; }
                            Player.WantedLevel = w;
                            EndFlag = true;
                            break;

                        case 4:
                            Game.DisplayText("No.004 周辺車両一斉発火", 4000);
                            msgBuff = "車は爆発物";
                            IntervalTimer = 30;
                            Vehicle[] v = Cacher.GetVehicles(Player.Character.Position, 100.0f);
                            for (int i = 0; i < v.Length; i++)
                            {
                                if (Exists(v[i])) { v[i].PetrolTankHealth = -1.0f; }
                            }
                            EndFlag = true;
                            break;

                        case 5:
                            Game.DisplayText("No.005 ローマンパラダイス", 4000);
                            msgBuff = "やめローマン";
                            IntervalTimer = 30;
                            for (int i = 0; i < 20; i++)
                            {
                                Ped p = World.CreatePed(new Model(0x89395FC9), Player.Character.Position.Around(rnd.Next(1, 5)));
                                if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; }
                            }
                            EndFlag = true;
                            break;
                        case 6:
                            Game.DisplayText("No.006 マニーパラダイス", 4000);
                            msgBuff = "「「「ヘイメーン！」」」";
                            IntervalTimer = 30;
                            for (int i = 0; i < 20; i++)
                            {
                                Ped p = World.CreatePed(new Model(0x5629F011), Player.Character.Position.Around(rnd.Next(1, 5)));
                                if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; }
                            }
                            EndFlag = true;
                            break;

                        case 7:
                            Game.DisplayText("No.007 LCPDパラダイス", 4000);
                            msgBuff = "「「「仕事しに来ました」」」";
                            IntervalTimer = 30;
                            for (int i = 0; i < 20; i++)
                            {
                                Ped p = World.CreatePed(Model.CurrentCopModel, Player.Character.Position.Around(rnd.Next(5, 10)));
                                if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; }
                            }
                            EndFlag = true;
                            break;

                        case 8:
                            ChoseFlag = true;
                            break;
                        /*
                        Game.DisplayText("No.008 罰金20%", 4000);
                        msgBuff = "罰金おいしいです ^q^";
                        IntervalTimer = 30;
                        int Money = Player.Character.Money;
                        Money = Money/5;
                        Player.Character.Money -= Money;

                        EndFlag = true;
                        break;
                         * */
                        case 9:
                            Game.DisplayText("No.009 +$2000", 4000);
                            msgBuff = "使い道は入院費";
                            IntervalTimer = 30;
                            Player.Money += 2000;
                            EndFlag = true;
                            break;

                        case 10:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.010 火の海", 4000);
                                InitFlag = false;
                                Timer = 100;
                                msgBuff = "今、リバティーシティが熱い！";
                                IntervalTimer = 30;
                                miniMsg = "火の海";
                            }
                            if (Timer > 0)
                            {
                                for (int i = 0; i < 20; i++)
                                {
                                    GTA.World.AddExplosion(Player.Character.Position.Around(rnd.Next(2, 50)), ExplosionType.Molotov, 6.0f, true, false, 0);

                                }
                                Timer--;
                            }
                            else
                            {
                                EndFlag = true;
                            }
                            break;

                        case 11:
                            Game.DisplayText("No.011 全員爆死", 4000);
                            msgBuff = "テンション爆発";
                            IntervalTimer = 30;
                            {
                                Ped[] allped = Cacher.GetPeds(Player.Character.Position, 100.0f);
                                for (int i = 0; i < allped.Length; i++)
                                {
                                    if (allped[i] == Player.Character) { continue; }
                                    if (allped[i].isDead) { continue; }
                                    GTA.World.AddExplosion(allped[i].Position, ExplosionType.Default, 1.0f);
                                }

                                Vehicle[] aV = Cacher.GetVehicles(Player.Character.Position, 100.0f);
                                for (int i = 0; i < aV.Length; i++)
                                {
                                    if (Player.Character.CurrentVehicle == aV[i]) { continue; }
                                    aV[i].Explode();
                                }
                            }

                            EndFlag = true;
                            break;

                        case 12:
                            ChoseFlag = true;
                            break;
                        /*
                        if (InitFlag)
                        {
                            Game.DisplayText("No.012 カメラふにゃふにゃ", 4000);
                            msgBuff = "やってる自分が一番酔う";
                            IntervalTimer = 30;
                            cam.DrunkEffectIntensity = 3.0f;
                            Timer = 140;
                            InitFlag = false;
                        }
                        if (Player.Character.isDead || !Player.CanControlCharacter)
                        {
                            Timer = 0;
                        }
                        else
                        {
                            Timer--;
                        }
                        if (Timer <= 0 || StopFlag)
                        {
                            cam.DrunkEffectIntensity = 0.0f;
                            EndFlag = true;
                        }
                        break;
                        */
                        case 13:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.013 絨毯爆撃", 4000);
                                msgBuff = "「ターゲットを確認した」";
                                IntervalTimer = 20;
                                Timer = 30;
                                Pos = Player.Character.Position;
                                InitFlag = false;
                                miniMsg = "絨毯爆撃";
                            }
                            if (Timer == 30) { GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_green", Pos.X, Pos.Y, Pos.Z, 0, 0, 0, 2.0f); }
                            if (Timer == 20) { GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_yellow", Pos.X, Pos.Y, Pos.Z, 0, 0, 0, 2.0f); }
                            if (Timer == 10) { GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_red", Pos.X, Pos.Y, Pos.Z, 0, 0, 0, 2.0f); }
                            if (Timer < 3)
                            {
                                msgBuff = "発射！";
                                IntervalTimer = 30;
                                Ped Asan;
                                Vector3 AP;
                                for (int i = 0; i < 200; i++)
                                {
                                    AP = Pos.Around(rnd.Next(0, 50)) + new Vector3(0, 0, 50); ;
                                    Asan = GTA.World.CreatePed(AP);
                                    if (Exists(Asan))
                                    {
                                        Asan.Visible = false;
                                        Asan.Weapons.FromType(Weapon.Heavy_RocketLauncher).Ammo = 999;
                                        Asan.Weapons.Select(Weapon.Heavy_RocketLauncher);
                                        GTA.Native.Function.Call("FIRE_PED_WEAPON", Asan, Asan.Position.X, Asan.Position.Y, Asan.Position.Z - 50.0f);
                                        Asan.Delete();
                                    }
                                }
                                if (Timer == 0)
                                {
                                    EndFlag = true;
                                }
                            }
                            Timer--;
                            break;

                        case 14:
                            {
                                if (InitFlag)
                                {
                                    msgBuff = "磯野～！空飛ぼうぜ！";
                                    IntervalTimer = 30;
                                    Game.DisplayText("No.014 空飛ぼうぜ！", 4000);
                                    {

                                        Ped[] allped = Cacher.GetPeds(Player.Character.Position, 200.0f);
                                        for (int i = 0; i < allped.Length; i++)
                                        {

                                            allped[i].Velocity = new Vector3(0, 0, 200);

                                        }

                                        Vehicle[] aV = Cacher.GetVehicles(Player.Character.Position, 200.0f);
                                        for (int i = 0; i < aV.Length; i++)
                                        {

                                            aV[i].Velocity = new Vector3(0, 0, 150);
                                        }
                                    }
                                    InitFlag = false;
                                }
                                else
                                {
                                    if (Player.Character.Position.Z < 150 && Player.Character.Velocity.Z > 0 && !Player.Character.isRagdoll)
                                    {
                                        Player.Character.Velocity = new Vector3(0, 0, 200);
                                    }
                                    else if (Player.Character.Position.Z > 150)
                                    {
                                        Player.Character.Invincible = true;
                                        Player.CanControlRagdoll = true;
                                        Player.Character.isRagdoll = true;
                                    }
                                    else if (Player.Character.Position.Z < 40.0f && Player.Character.Velocity.Length() < 1.0f)
                                    {
                                        Player.Character.Invincible = false;
                                        Player.CanControlRagdoll = false;
                                        Player.Character.isRagdoll = false;
                                        EndFlag = true;
                                    }
                                }
                            }
                            if (StopFlag)
                            {
                                Player.Character.Invincible = false;
                                Player.CanControlRagdoll = false;
                                Player.Character.isRagdoll = false;
                                EndFlag = true;
                            }
                            break;
                        #endregion
                        #region Stealth

                        case 15:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.015 光学迷彩(Player)", 4000);
                                msgBuff = "京レ製全天候型2902（式）熱光学迷彩";
                                IntervalTimer = 30;
                                Timer = 30 * 10;
                                InitFlag = false;
                                Player.IgnoredByEveryone = true;
                                miniMsg = "光学迷彩";
                            }
                            if (--Timer > 0 && !StopFlag)
                            {
                                if ((Timer < 50) && (Timer % 8 > 4) || Timer % 10 == rnd.Next(0, 10))
                                {
                                    Player.Character.Visible = true;
                                    Player.Character.Visible = true;
                                }
                                else
                                {
                                    Player.Character.Visible = false;
                                    Player.IgnoredByEveryone = false;
                                }
                            }
                            else
                            {
                                Player.IgnoredByEveryone = false;
                                Player.Character.Visible = true;
                                EndFlag = true;
                            }
                            break;

                        case 16:
                            Game.DisplayText("No.016 光学迷彩(市民)", 4000);
                            msgBuff = "東セラ製3302（式）熱光学迷彩";
                            IntervalTimer = 30;
                            {
                                Ped[] ap = Cacher.GetPeds(Player.Character.Position, 100.0f);
                                for (int i = 0; i < ap.Length; i++)
                                {
                                    if (!Exists(ap[i])) { continue; }
                                    if (Player.Character == ap[i]) { continue; }
                                    ap[i].Visible = false;
                                }
                            }
                            EndFlag = true;
                            break;

                        case 17:
                            Game.DisplayText("No.017 光学迷彩(車両)", 4000);
                            msgBuff = "再帰性反射材";
                            IntervalTimer = 30;
                            {
                                Vehicle[] av = Cacher.GetVehicles(Player.Character.Position, 200.0f);
                                for (int i = 0; i < av.Length; i++)
                                {
                                    if (!Exists(av[i])) { continue; }
                                    av[i].Visible = false;
                                }
                            }
                            EndFlag = true;
                            break;

                        #endregion
                        #region case18~36
                        case 18:
                            {

                                if (InitFlag)
                                {
                                    InitFlag = false;
                                    Game.DisplayText("No.018 銃弾のアメアラレ…", 4000);
                                    Timer = 200;
                                    msgBuff = "狙撃祭りだヒャッハー！";
                                    IntervalTimer = 30;
                                    AP = Cacher.GetPeds(Player.Character.Position, 100.0f);
                                    miniMsg = "狙撃祭り";
                                }
                                if (--Timer > 0 && !StopFlag)
                                {
                                    if (Timer % 15 == 0) { AP = Cacher.GetPeds(Player.Character.Position, 100.0f); }
                                    if (Exists(ped)) { ped.Delete(); }
                                    for (int i = 0; i < 10; i++)
                                    {
                                        ped = GTA.World.CreatePed(Player.Character.Position.Around(rnd.Next(1, 20)) + new Vector3(0, 0, rnd.Next(20, 40)));
                                        if (Exists(ped)) { break; }
                                    }
                                    if (!Exists(ped)) { break; }

                                    ped.Visible = false;
                                    ped.Weapons.FromType(Weapon.SniperRifle_Basic).Ammo = 999;
                                    ped.Weapons.Select(Weapon.SniperRifle_Basic);
                                    Ped trg = AP[rnd.Next(0, AP.Length)];

                                    if (Exists(trg) && trg.isAlive)
                                    {

                                        GTA.Native.Function.Call("FIRE_PED_WEAPON", ped, trg.Position.X, trg.Position.Y, trg.Position.Z);
                                    }
                                    else
                                    {
                                        trg = Player.Character;
                                        vec = trg.Position.Around(2.0f*(float)rnd.NextDouble());
                                        GTA.Native.Function.Call("FIRE_PED_WEAPON", ped, vec.X, vec.Y, vec.Z);
                                    }


                                }
                                else
                                {
                                    if (Exists(ped)) { ped.Delete(); }
                                    EndFlag = true;
                                }
                            }
                            break;

                        case 19:

                            Game.DisplayText("No.019 天候変化", 4000);

                            switch (rnd.Next(11))
                            {
                                case 0:
                                    msgBuff = "天候：くもり";
                                    World.Weather = Weather.Cloudy;
                                    break;
                                case 1:
                                    msgBuff = "天候：小雨";
                                    World.Weather = Weather.Drizzle;
                                    break;
                                case 2:
                                    msgBuff = "天候：快晴";
                                    World.Weather = Weather.ExtraSunny;
                                    break;
                                case 3:
                                    msgBuff = "天候：快晴２";
                                    World.Weather = Weather.ExtraSunny2;
                                    break;
                                case 4:
                                    msgBuff = "天候：霧";
                                    World.Weather = Weather.Foggy;
                                    break;
                                case 5:
                                    msgBuff = "天候：雨";
                                    World.Weather = Weather.Raining;
                                    break;
                                case 6:
                                    msgBuff = "天候：晴れ";
                                    World.Weather = Weather.Sunny;
                                    break;
                                case 7:
                                    msgBuff = "天候：晴れ（風強め）";
                                    World.Weather = Weather.SunnyAndWindy;
                                    break;
                                case 8:
                                    msgBuff = "天候：晴れ（風強め２）";
                                    World.Weather = Weather.SunnyAndWindy2;
                                    break;
                                case 9:
                                    msgBuff = "天候：嵐";
                                    World.Weather = Weather.ThunderStorm;
                                    break;
                            }

                            IntervalTimer = 30;
                            EndFlag = true;
                            break;

                        case 20:
                            Game.DisplayText("No.020 今何時だっけ？", 4000);
                            int Hour = rnd.Next(0, 24);
                            msgBuff = string.Format("{0}時かな", Hour);
                            IntervalTimer = 30;
                            GTA.Native.Function.Call("SET_TIME_OF_DAY", Hour, 0);
                            EndFlag = true;
                            break;

                        case 21:
                            Game.DisplayText("No.021 マイケルアワー", 4000);
                            msgBuff = string.Format("マイケルが死んじまった！");
                            screenFont.Color = Color.FromArgb(255, 100, 46, 46);
                            IntervalTimer = 30;

                            for (int i = 0; i < 3; i++)
                            {
                                Ped p = World.CreatePed(new Model(0x2BD27039), Player.Character.Position.Around(1.5f));
                                if (Exists(p))
                                {

                                    Player.Group.AddMember(p);
                                    p.Health = 1;
                                    p.Weapons.RocketLauncher.Ammo = 300;
                                    p.Weapons.MP5.Ammo = 3000;
                                    p.Weapons.BaseballBat.Ammo = 2000;
                                }
                            }

                            EndFlag = true;
                            break;

                        case 22:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.022 強風注意報", 4000);
                                msgBuff = string.Format("強風注意");
                                IntervalTimer = 30;
                                InitFlag = false;

                                vec = new Vector3(rnd.Next(0, 20) - 10, rnd.Next(0, 20) - 10, 0);
                                vec.Normalize();
                                Timer = 300;
                                miniMsg = "強風注意";
                            }
                            if (--Timer > 0 && Player.Character.isAlive && !StopFlag)
                            {
                                if (Timer % 80 == 0)
                                {
                                    vec = new Vector3(rnd.Next(0, 20) - 10, rnd.Next(0, 20) - 10, 0);
                                    vec.Normalize();
                                    msgBuff = string.Format("風向きが変わったぞ");
                                    IntervalTimer = 30;
                                }
                                if (Player.Character.isInVehicle())
                                {
                                    Player.Character.CurrentVehicle.ApplyForce(0.7f * vec);
                                }
                                else
                                {

                                    Player.Character.ApplyForce(2.0f * vec);
                                }
                            }
                            else
                            {
                                EndFlag = true;
                                msgBuff = string.Format("強風注意報 解除");
                                IntervalTimer = 30;
                            }
                            break;

                        case 23:
                            {
                                Game.DisplayText("No.023 Faggio進呈", 4000);
                                msgBuff = string.Format("買い物とかに使える");
                                IntervalTimer = 30;
                                Vehicle fg = GTA.World.CreateVehicle("FAGGIO", Player.Character.Position.Around(2.0f));
                                if (Exists(fg))
                                {
                                    fg.NoLongerNeeded();
                                }
                                EndFlag = true;
                            }
                            break;

                        case 24:
                            {
                                Game.DisplayText("No.024 COMETさん", 4000);
                                msgBuff = string.Format("音速が遅いぜ！");
                                IntervalTimer = 30;
                                Vehicle fg = GTA.World.CreateVehicle("COMET", Player.Character.Position.Around(3.0f));
                                if (Exists(fg))
                                {
                                    fg.NoLongerNeeded();
                                }
                                EndFlag = true;
                            }
                            break;

                        case 25:
                            {
                                if (InitFlag)
                                {

                                    ped = GTA.World.CreatePed(Player.Character.Position.Around(rnd.Next(20)) + new Vector3(0, 0, 50));
                                    if (Exists(ped))
                                    {
                                        Game.DisplayText("No.025 ボルガ式解決法", 4000);
                                        msgBuff = string.Format("ボルガ博士！お許し下さい！");
                                        IntervalTimer = 30;
                                        ped.Health = 1000;
                                        Timer = 100;
                                        miniMsg = "ボルガ式解決法";
                                    }
                                    else
                                    {
                                        ChoseFlag = true;
                                    }
                                    InitFlag = false;
                                }
                                if (Exists(ped) && !StopFlag)
                                {
                                    if (ped.HasBeenDamagedBy(Weapon.Misc_Fall) || ped.isDead || --Timer == 0)
                                    {
                                        vec = ped.Position;
                                        ped.NoLongerNeeded();
                                        World.AddExplosion(vec, ExplosionType.Rocket, 20.0f);

                                        AP = Cacher.GetPeds(vec, 50.0f);
                                        AV = Cacher.GetVehicles(vec, 50.0f);
                                        GTA.Object[] AO = World.GetAllObjects();

                                        for (int i = 0; i < AP.Length; i++)
                                        {
                                            if (!Exists(AP[i])) { continue; }
                                            if (AP[i] == Player.Character)
                                            {
                                                GuidOfScript2 = new Guid("CC62497C-E738-11DF-8390-560BDFD72085");
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
                                        Timer = -1;
                                        EndFlag = true;
                                    }
                                }
                                else
                                {
                                    if (Exists(ped)) { ped.NoLongerNeeded(); }
                                    Timer = -1;
                                    EndFlag = true;
                                }
                            }
                            break;

                        case 26:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.026 アステロイドベルト", 4000);
                                msgBuff = string.Format("メテオがやべぇぞ");
                                IntervalTimer = 30;
                                GuidOfScript2 = new Guid("060201CC-E734-11DF-9215-D104DFD72085");
                                SendScriptCommand(GuidOfScript2, "ChangeProbability", 100);
                                InitFlag = false;
                                Timer = 30 * 10;
                                miniMsg = "メテオ100%";
                            }
                            if (--Timer < 0 || StopFlag)
                            {
                                GuidOfScript2 = new Guid("060201CC-E734-11DF-9215-D104DFD72085");
                                SendScriptCommand(GuidOfScript2, "ChangeProbability", -1);
                                EndFlag = true;
                                msgBuff = string.Format("よし！");
                                IntervalTimer = 30;
                            }

                            break;
                        case 27:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.027 ただのカオスモード", 4000);
                                msgBuff = string.Format("メテオが止んだ！チャンスだ！");
                                IntervalTimer = 30;
                                GuidOfScript2 = new Guid("060201CC-E734-11DF-9215-D104DFD72085");
                                SendScriptCommand(GuidOfScript2, "ChangeProbability", 0);
                                InitFlag = false;
                                Timer = 30 * 10;
                                miniMsg = "メテオ0%";
                            }
                            if (--Timer < 0 || StopFlag)
                            {
                                GuidOfScript2 = new Guid("060201CC-E734-11DF-9215-D104DFD72085");
                                SendScriptCommand(GuidOfScript2, "ChangeProbability", -1);
                                EndFlag = true;
                                msgBuff = string.Format("しまった！メテオが降り始めた！");
                                IntervalTimer = 30;
                            }

                            break;

                        case 28:
                            if (InitFlag)
                            {

                                Game.DisplayText("No.028 ドライブ A GO GO!", 4000);
                                msgBuff = string.Format("グランド・セフト・オート");
                                IntervalTimer = 30;
                                InitFlag = false;
                                Timer = 10 * 10;
                                miniMsg = "車両強盗だぜ";
                                AV = Cacher.GetVehicles(Player.Character.Position, 200.0f);
                                AP = new Ped[AV.Length];
                                for (int i = 0; i < AV.Length; i++)
                                {
                                    if (!Exists(AV[i])) { continue; }
                                    if (!AV[i].isAlive || !AV[i].isDriveable) { continue; }

                                   // Ped p = AV[i].GetPedOnSeat(VehicleSeat.Driver);
                                   // if (Exists(p)) { continue; }

                                    AP[i] = World.CreatePed(AV[i].Position.Around(3.0f));
                                    if (!Exists(AP[i])) { continue; }
                                    AP[i].Task.EnterVehicle(AV[i], VehicleSeat.Driver);
                                    AP[i].Money = 500;
                                }
                            }
                            if (--Timer < 0 || StopFlag)
                            {
                                for (int i = 0; i < AP.Length; i++)
                                {
                                    if (Exists(AP[i]))
                                    {
                                        AP[i].Money = rnd.Next(0, 200);
                                        AP[i].NoLongerNeeded();

                                    }

                                }
                                EndFlag = true;
                            }
                            break;


                        case 29:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.029 リジェネ", 4000);
                                msgBuff = string.Format("今日も元気だな！");
                                IntervalTimer = 30;
                                Timer = 10 * 30;
                                InitFlag = false;
                                miniMsg = "リジェネ";
                            }
                            Timer--;

                            if (Timer % 30 == 0)
                            {
                                int HP = Player.Character.Health;
                                HP += 8;
                                Player.Character.Health = HP;
                            }

                            if (Timer == 0 || StopFlag)
                            {
                                msgBuff = string.Format("リジェネの効果が切れた");
                                IntervalTimer = 30;
                                EndFlag = true;
                            }

                            break;

                        case 30:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.030 未来のチョッキ", 4000);
                                msgBuff = string.Format("自動回復チョッキ");
                                IntervalTimer = 30;
                                Timer = 10 * 30;
                                InitFlag = false;
                                miniMsg = "チョッキ自動回復";
                            }
                            Timer--;

                            if (Timer % 30 == 0)
                            {
                                int HP = Player.Character.Armor;
                                HP += 14;

                                Player.Character.Armor = HP;
                            }

                            if (Timer == 0 || StopFlag)
                            {
                                msgBuff = string.Format("チョッキは力尽きた");
                                IntervalTimer = 30;
                                EndFlag = true;
                            }
                            break;


                        case 31:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.031 まきびし", 4000);
                                msgBuff = string.Format("パンクにご注意を");
                                IntervalTimer = 30;
        
                                InitFlag = false;

                                AV = Cacher.GetVehicles(Player.Character.Position, 300.0f);
                            }

                            foreach (Vehicle veh in AV)
                            {
                                veh.CanTiresBurst = true;
                                if (!veh.IsTireBurst(VehicleWheel.FrontLeft)) { veh.BurstTire(VehicleWheel.FrontLeft); }
                                if (!veh.IsTireBurst(VehicleWheel.FrontRight)) { veh.BurstTire(VehicleWheel.FrontRight); }
                                if (!veh.IsTireBurst(VehicleWheel.RearLeft)) { veh.BurstTire(VehicleWheel.RearLeft); }
                                if (!veh.IsTireBurst(VehicleWheel.RearRight)) { veh.BurstTire(VehicleWheel.RearRight); }
                                try
                                {
                                    if (veh.IsTireBurst(VehicleWheel.CenterLeft)) { veh.BurstTire(VehicleWheel.CenterLeft); }
                                    if (veh.IsTireBurst(VehicleWheel.CenterRight)) { veh.BurstTire(VehicleWheel.CenterRight); }
                                }
                                catch
                                {
                                    ;
                                }
                            }

                                EndFlag = true;
                   

                            break;

                        case 32:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.032 無敵", 4000);
                                msgBuff = string.Format("ムテキング");
                                IntervalTimer = 30;
                                Timer = 30 * 10;
                                InitFlag = false;
                                miniMsg = "無敵";
                                Player.Character.Invincible = true;
                            }

                            if (--Timer < 0 || StopFlag)
                            {
                                msgBuff = string.Format("ムテキング　おわり");
                                IntervalTimer = 30;
                                Player.Character.Invincible = false;
                                EndFlag = true;
                            }

                            break;

                        case 33:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.033 RPG反射", 4000);
                                msgBuff = string.Format("フォーチュン");
                                IntervalTimer = 30;
                                Timer = 30 * 10;
                                miniMsg = "RPG反射";
                                InitFlag = false;
                            }
                            if (Timer % 3 == 0)
                            {
                                AO = World.GetAllObjects(new Model(0x5A6525AE));
                            }

                            for (int i = 0; i < AO.Length; i++)
                            {
                                if (Exists(AO[i]))
                                {
                                    if (AO[i].Position.DistanceTo(Player.Character.Position) < 10.0f)
                                    {

                                        vec = AO[i].Position - Player.Character.Position;
                                        vec.Normalize();
                                        AO[i].Velocity = 30 * vec;
                                    }
                                }
                            }
                            if (--Timer < 0 || StopFlag)
                            {
                                msgBuff = string.Format("アンフォーチュン");
                                IntervalTimer = 30;
                                EndFlag = true;
                            }
                            break;

                        case 34:
                            EndFlag = true;
                            ChoseFlag = true;
                            break;
                        case 35:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.035 下痢", 4000);
                                msgBuff = string.Format("おなかいたい");
                                IntervalTimer = 30;
                                Timer = 15 * 10;
                                InitFlag = false;
                                miniMsg = "おなかいたい";
                            }
                            if (Timer % 20 == 0)
                            {
                                Ped ped = World.CreatePed(Player.Character.Position);
                                if (ped.Exists())
                                {
                                    ped.Visible = false;
                                    ped.Weapons.FromType(Weapon.Episodic_22).Ammo = 999;
                                    ped.Weapons.Select(Weapon.Episodic_22);
                                    vec = Player.Character.Position;
                                    GTA.Native.Function.Call("FIRE_PED_WEAPON", ped, vec.X, vec.Y, vec.Z - 0.1f);
                                    ped.Delete();
                                }
                            }
                            if (--Timer < 0 || StopFlag)
                            {
                                msgBuff = string.Format("おなかいたくない");
                                IntervalTimer = 30;
                                EndFlag = true;
                            }

                            break;
                        case 36:
                            Game.DisplayText("No.036 歩道アタック", 4000);
                            msgBuff = string.Format("歩道が広いではないか");
                            IntervalTimer = 30;

                            AV = Cacher.GetVehicles(Player.Character.Position, 200.0f);
                            for (int i = 0; i < AV.Length; i++)
                            {
                                if (Exists(AV[i]))
                                {
                                    if (AV[i] == Player.Character.CurrentVehicle) { continue; }

                                    Vector3 NP = World.GetNextPositionOnPavement(AV[i].Position);
                                    vec = NP - AV[i].Position;
                                    vec.Normalize();
                                    AV[i].Velocity = 30 * vec;
                                    GTA.Native.Function.Call("ADD_EXPLOSION", AV[i].Position.X, AV[i].Position.Y, AV[i].Position.Z, 3, 0.0f, 30, 0, 0.1f);    //ダメージ０の見た目だけの爆風を生成
                                }
                            }
                            EndFlag = true;
                            break;

                        #endregion
                        #region case 37~43
                        case 37:
                            {
                                Game.DisplayText("No.037 ディフェンスに定評がある", 4000);
                                msgBuff = string.Format("上手く使いこなせ");
                                IntervalTimer = 30;
                                Vehicle fg = GTA.World.CreateVehicle("STOCKADE", Player.Character.Position.Around(3.0f));
                                if (Exists(fg))
                                {
                                    fg.NoLongerNeeded();
                                }
                                EndFlag = true;
                            }
                            break;
                        case 38:
                            {
                                Game.DisplayText("No.038 地味にレア", 4000);
                                msgBuff = string.Format("どーすんのこれで");
                                IntervalTimer = 30;
                                Vehicle fg = GTA.World.CreateVehicle("AIRTUG", Player.Character.Position.Around(3.0f));
                                if (Exists(fg))
                                {
                                    fg.NoLongerNeeded();
                                }
                                EndFlag = true;
                            }
                            break;
                        case 39:
                            {
                                Game.DisplayText("No.039 地味に運転しにくい", 4000);
                                msgBuff = string.Format("どーすんのこれで 2");
                                IntervalTimer = 30;
                                Vehicle fg = GTA.World.CreateVehicle("FORKLIFT", Player.Character.Position.Around(3.0f));
                                if (Exists(fg))
                                {
                                    fg.NoLongerNeeded();
                                }
                                EndFlag = true;
                            }
                            break;
                        case 40:
                            {
                                Game.DisplayText("No.040 かなりレア", 4000);
                                msgBuff = string.Format("こんなのあったんだ");
                                IntervalTimer = 30;
                                Vehicle fg = GTA.World.CreateVehicle("RIPLEY", Player.Character.Position.Around(5.0f));
                                if (Exists(fg))
                                {
                                    fg.NoLongerNeeded();
                                }
                                EndFlag = true;
                            }
                            break;

                        case 41:
                            {
                                if(InitFlag)
                                {
                                    Game.DisplayText("No.041 おいもぱわー", 4000);
                                    msgBuff = string.Format("3");
                                    IntervalTimer = 30;
                                    InitFlag = false;
                                    Timer = 30;
                                    miniMsg = "おいもぱわー";
                                }

                                if(Timer==20){
                                                                        msgBuff = string.Format("2");
                                    IntervalTimer = 30;
                                }else if(Timer ==10)
                                {
                                                                        msgBuff = string.Format("1");
                                    IntervalTimer = 30;
                                }
                                else if (Timer == 0)
                                {
                                    msgBuff = string.Format("おぉ　くさいくさい");
                                    IntervalTimer = 30;
                                    GTA.World.AddExplosion(Player.Character.Position, ExplosionType.Default, 0.0f, true, true, 0.5f);


                                    AV = Cacher.GetVehicles(Player.Character.Position, 200.0f);
                                    AP = Cacher.GetPeds(Player.Character.Position, 200.0f);
                                    AO = World.GetAllObjects();

                                    for (int i = 0; i < AV.Length; i++)
                                    {
                                        if (!Exists(AV[i]) || AV[i] == Player.Character.CurrentVehicle) { continue; }
                                        Vector3 TP = AV[i].Position - Player.Character.Position;
                                        TP.Normalize();
                                        if (Math.Abs(GetTheta(Player.Character.Direction, -TP)) < 70)
                                        {
                                            TP = AV[i].Position - Player.Character.Position;
                                            TP.Normalize();
                                            TP = 100 * TP;
                                            AV[i].ApplyForce(TP);
                                        }
                                    }

                                    for (int i = 0; i < AP.Length; i++)
                                    {
                                        if (!Exists(AP[i]) || AP[i] == Player.Character) { continue; }
                                        Vector3 TP = AP[i].Position - Player.Character.Position;
                                        TP.Normalize();
                                        if (Math.Abs(GetTheta(Player.Character.Direction, -TP)) < 70)
                                        {

                                            TP = AP[i].Position - Player.Character.Position;

                                            TP.Normalize();
                                            TP = 150 * TP;
                                            AP[i].ForceRagdoll(5000, false);
                                            AP[i].ApplyForce(TP);

                                        }
                                    }
                                    for (int i = 0; i < AO.Length; i++)
                                    {
                                        if (!Exists(AO[i])) { continue; }
                                        Vector3 TP = AO[i].Position - Player.Character.Position;
                                        TP.Normalize();
                                        if (Math.Abs(GetTheta(Player.Character.Direction, -TP)) < 70)
                                        {
                                            TP = AO[i].Position - Player.Character.Position;

                                            TP.Normalize();
                                            TP = 100 * TP;
                                            AO[i].ApplyForce(TP);
                                        }
                                    }




                                    EndFlag = true;


                                }
                                Timer--;
  
                            }

                            break;
                        case 42:
                            {
                                Game.DisplayText("No.042 大天使の息吹", 4000);
                                msgBuff = string.Format("SS-3 大天使の息吹");
                                IntervalTimer = 30;


                                AV = World.GetAllVehicles();
                                AP = World.GetAllPeds();

                                for (int i = 0; i < AV.Length; i++)
                                {
                                    if (!Exists(AV[i])) { continue; }
                                    AV[i].Repair();
                                    AV[i].DoorLock = DoorLock.None;

                                }
                                for (int i = 0; i < AP.Length; i++)
                                {
                                    if (!Exists(AP[i])) { continue; }

                                    if (AP[i].Health < 100)
                                    {
                                        AP[i].Health = 100;
                                    }
                                    AP[i].Armor = 100;

                                }
                                EndFlag = true;
                            }
                            break;

                        case 43:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.043 ダンボール支援", 4000);
                                msgBuff = string.Format("ダンボール支援要請を確認した");
                                IntervalTimer = 30;
                                InitFlag = false;
                                OBJ = null;
                                while (!Exists(OBJ))
                                {
                                    OBJ = GTA.World.CreateObject(new Model(0xC24943EE), Player.Character.Position.Around(10.0f) + new Vector3(0, 0, 25.0f));
                                }

                                GTA.Native.Function.Call("SET_ACTIVATE_OBJECT_PHYSICS_AS_SOON_AS_IT_IS_UNFROZEN", OBJ, true);
                                OBJ.Detach();
                                OBJ.FreezePosition = false;
                                Blip b = OBJ.AttachBlip();
                                b.Color = BlipColor.LightYellow;

                                OBJ.Collision = true;
                                GTA.Native.Function.Call("SET_OBJECT_RECORDS_COLLISIONS", OBJ, true);
                            }
                            if (Exists(OBJ))
                            {
                                OBJ.ApplyForce(new Vector3(0.1f, 0, 0.0f));
                            }
                            else
                            {
                                EndFlag = true;
                                break;
                            }
                            if (StopFlag || OBJ.Position.DistanceTo2D(Player.Character.Position) > 50.0f)
                            {
                                OBJ.NoLongerNeeded();
                                EndFlag = true;
                                break;
                            }
                            if (GTA.Native.Function.Call<bool>("HAS_OBJECT_COLLIDED_WITH_ANYTHING", OBJ))
                            {
                                vec = OBJ.Position;
                                OBJ.Delete();
                                if (rnd.Next(100) < 70)
                                {
                                    GTA.Native.Function.Call("TRIGGER_PTFX", "qub_sm_explode_yellow", vec.X, vec.Y, vec.Z, 0, 0, 0, 2.5f);
                                    GTA.Pickup.CreateWeaponPickup(vec, Weapon.Misc_Armor, 100);

                                    GTA.Pickup.CreateWeaponPickup(vec.Around(0.8f), GetWeaponFromRandom(), 200);
                                    GTA.Pickup.CreateWeaponPickup(vec.Around(0.8f), GetWeaponFromRandom(), 200);
                                    GTA.Pickup.CreateWeaponPickup(vec.Around(0.8f), GetWeaponFromRandom(), 200);
                                    GTA.Pickup.CreateWeaponPickup(vec.Around(0.8f), GetWeaponFromRandom(), 200);
                                    GTA.Pickup.CreateWeaponPickup(vec.Around(0.8f), GetWeaponFromRandom(), 200);
                                    GTA.Pickup.CreateWeaponPickup(vec.Around(0.8f), GetWeaponFromRandom(), 200);
                                }
                                else
                                {
                                    World.AddExplosion(vec);
                                }

                                EndFlag = true;
                            }

                            break;
                        #endregion
                        #region 44~58
                        case 44:
                            {
                                Game.DisplayText("No.044 チャフ", 4000);
                                msgBuff = string.Format("ジャミング");
                                IntervalTimer = 30;

                                GTA.World.AddExplosion(Player.Character.Position + new Vector3(0, 0, 10.0f), ExplosionType.Rocket, 0.0f, true, true, 1.0f);

                                GuidOfScript2 = new Guid("915B924A-0D24-11E0-A534-02BCDFD72085");
                                SendScriptCommand(GuidOfScript2, "ChangeFlag", true);

                                EndFlag = true;
                            }
                            break;

                        case 45:
                            {
                                Game.DisplayText("No.045 人口密度アップ", 4000);
                                msgBuff = string.Format("激戦区");
                                IntervalTimer = 30;

                                GTA.World.PedDensity = 10.0f;
                                FLAG_1 = true;
                                EndFlag = true;
                            }
                            break;

                        case 46:
                            {
                                Game.DisplayText("No.046 人口密度ダウン", 4000);
                                msgBuff = string.Format("今日はおやすみ");
                                IntervalTimer = 30;

                                GTA.World.PedDensity = 0.1f;
                                FLAG_1 = true;
                                EndFlag = true;
                            }
                            break;

                        case 47:
                            Game.DisplayText("No.047 サイコマンティス", 4000);
                            msgBuff = string.Format("サイコマンティスだ！");
                            IntervalTimer = 30;


                            GuidOfScript2 = new Guid("BE5C26AC-0E9A-11E0-8F04-06D4DFD72085");
                            SendScriptCommand(GuidOfScript2, "Activate");

                            EndFlag = true;
                            break;
                        case 48:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.048 車ふわふわ", 4000);
                                msgBuff = string.Format("もみくちゃ");
                                IntervalTimer = 30;
                                InitFlag = false;
                                miniMsg = "車ふわふわ";
                                Timer = 300;
                            }
                            AV = Cacher.GetVehicles(Player.Character.Position, 100.0f);
                            for (int i = 0; i < AV.Length; i++)
                            {
                                if (!Exists(AV[i])) { continue; }
                                if (AV[i] == Player.Character.CurrentVehicle || !Exists(AV[i].GetPedOnSeat(VehicleSeat.Driver)))
                                {
                                    if (!AV[i].isOnAllWheels)
                                    {
                                        AV[i].ApplyForce(new Vector3(0, 0, 0.9f));
                                    }
                                    continue;
                                }
                                AV[i].ApplyForceRelative(new Vector3(0, 0, 1.1f));


                            }


                            if (--Timer < 0 || StopFlag)
                            {
                                msgBuff = string.Format("もみくちゃおわり");
                                IntervalTimer = 30;
                                EndFlag = true;
                            }
                            break;

                        case 49:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.049 くるくる", 4000);
                                msgBuff = string.Format("いつもより余計に回っております");
                                IntervalTimer = 30;
                                InitFlag = false;
                                GuidOfScript2 = new Guid("C2CFA980-28A7-11E0-B4A2-BD74DFD72085");
                                SendScriptCommand(GuidOfScript2, "Activate", true);
                                Timer = 300;
                                miniMsg = "くるくる";
                            }
                            if (--Timer < 0 || StopFlag)
                            {
                                SendScriptCommand(GuidOfScript2, "Activate", false);
                                msgBuff = string.Format("くるくるおわり");
                                IntervalTimer = 30;
                                EndFlag = true;
                            }
                            break;

                        case 50:
                            if (InitFlag)
                            {

                                Game.DisplayText("No.050 場所チェンジ", 4000);
                                msgBuff = string.Format("あっちこっち");
                                IntervalTimer = 30;
                                InitFlag = false;
                                Timer = 200;
                                miniMsg = "場所チェンジ";
                            }
                            if (Timer % 10 == 0)
                            {
                                AP = Cacher.GetPeds(Player.Character.Position, 200.0f);
                                if (AP.Length < 5) { break; }
                                Ped a = AP[rnd.Next(0, AP.Length)];
                                Ped b = AP[rnd.Next(0, AP.Length)];
                                if (!Exists(a) || !Exists(b) || a == b) { break; }


                                if (a.CurrentVehicle == null && b.CurrentVehicle == null)
                                {
                                    Vector3 tmpa = a.Position;
                                    Vector3 tmpb = b.Position;
                                    b.Position = b.Position + new Vector3(0, 0, 20);
                                    a.Position = tmpb;
                                    b.Position = tmpa;
                                    break;
                                }

                                if (a.CurrentVehicle != null && b.CurrentVehicle != null)
                                {
                                    Vehicle ta, tb;
                                    float vela, velb;
                                    ta = a.CurrentVehicle;
                                    tb = b.CurrentVehicle;
                                    vela = ta.Speed;
                                    velb = ta.Speed;
                                    Vector3 tmpa = ta.Position;
                                    Vector3 ra = ta.Rotation;
                                    Vector3 rb = tb.Rotation;
                                    Vector3 tmpb = tb.Position;
                                    tb.Position += new Vector3(0, 0, 20.0f);
                                    ta.Position = tmpb;
                                    tb.Position = tmpa;
                                    tb.Rotation = ra;
                                    ta.Rotation = rb;
                                    ta.Speed = velb;
                                    tb.Speed = vela;
                                }
                                if ((a.CurrentVehicle == null && b.CurrentVehicle != null) || (a.CurrentVehicle != null && b.CurrentVehicle == null))
                                {
                                    if (b.CurrentVehicle == null)
                                    {
                                        Ped tmp = b;
                                        b = a;
                                        a = tmp;
                                    }

                                    Vehicle tb;
                                    VehicleSeat sb;
                                    Vector3 Pos = a.Position;
                                    tb = b.CurrentVehicle;
                                    sb = GetSheat(tb, b);

                                    GTA.Native.Function.Call("WARP_CHAR_FROM_CAR_TO_COORD", b, b.Position.X, b.Position.Y, b.Position.Z + 20.0f);

                                    a.WarpIntoVehicle(tb, tb.GetFreeSeat());
                                    b.Position = Pos;
                                }

                            }

                            if (--Timer < 0 || StopFlag)
                            {
                                msgBuff = string.Format("おわり");
                                IntervalTimer = 30;
                                EndFlag = true;
                            }
                            break;

                        case 51:
                            {

                                Game.DisplayText("No.051 ポイント進呈", 4000);
                                msgBuff = string.Format("+500 POINTS");
                                IntervalTimer = 30;
                                Share.AddPoint(500);
                                EndFlag = true;
                            }
                            break;

                        case 52:
                            {
                                if (InitFlag)
                                {
                                    Game.DisplayText("No.052 月歩", 4000);
                                    InitFlag = false;
                                }
                                if (!Exists(Game.GetWaypoint()))
                                {

                                    msgBuff = string.Format("行き先を選べ！");
                                    IntervalTimer = 30;

                                }
                                else
                                {
                                    msgBuff = string.Format("いってらっしゃい！");
                                    IntervalTimer = 30;
                                    GuidOfScript2 = new Guid("CF7EB590-3851-11E0-88D2-1396DFD72085");
                                    SendScriptCommand(GuidOfScript2, "Active");
                                    EndFlag = true;
                                }

                            }
                            break;

                        case 53:
                            {

                                Game.DisplayText("No.053 車両強化", 4000);
                                msgBuff = string.Format("車両パワーうｐ");
                                IntervalTimer = 30;

                                EndFlag = true;

                                AV = Cacher.GetVehicles(Player.Character.Position, 100);

                                for (int i = 0; i < AV.Length; i++)
                                {
                                    if (!Exists(AV[i]) || !AV[i].isAlive) { continue; }
                                    AV[i].MakeProofTo(true, true, true, true, true);
                                    AV[i].EngineHealth = 3000;
                                    AV[i].Heading = 3000;
                                    AV[i].PetrolTankHealth = 3000;

                                }
                            }
                            break;

                        case 54:
                            {

                                Game.DisplayText("No.054 車両回復", 4000);
                                msgBuff = string.Format("車回復");
                                IntervalTimer = 30;

                                EndFlag = true;

                                AV = Cacher.GetVehicles(Player.Character.Position, 100);

                                for (int i = 0; i < AV.Length; i++)
                                {
                                    if (!Exists(AV[i]) || !AV[i].isAlive) { continue; }
                                    AV[i].Repair();

                                }
                            }
                            break;

                        case 55:
                            {
                                if (InitFlag)
                                {
                                    Game.DisplayText("No.055 無限ニトロ", 4000);
                                    msgBuff = string.Format("無限ニトロ");
                                    IntervalTimer = 30;
                                    Timer = 30 * 10;
                                    InitFlag = false;
                                    Share.NitroLimit = true;
                                    miniMsg = "無限ニトロ";
                                }
                                if (--Timer < 0 || StopFlag)
                                {
                                    msgBuff = string.Format("おわり");
                                    IntervalTimer = 30;
                                    EndFlag = true;
                                    Share.NitroLimit = false;
                                }

                            }
                            break;
                        case 56:

                            if (InitFlag)
                            {
                                Game.DisplayText("No.056 花火", 4000);
                                msgBuff = string.Format("ひとはなび");
                                IntervalTimer = 30;

                                vec = Player.Character.Position.Around(10.0f) + new Vector3(0, 0, 10);
                                AP = Cacher.GetPeds(Player.Character.Position, 100.0f);
                                InitFlag = false;
                                Timer = 30;
                                miniMsg = "人花火";
                            }

                            if (Timer > 0)
                            {
                                foreach (Ped p in AP)
                                {
                                    if (Exists(p) && Player.Character != p)
                                    {
                                        Vector3 v2 = -(p.Position - vec);
                                        v2.Normalize();
                                        if (p.isInVehicle())
                                        {
                                            p.Task.ClearAllImmediately();
                                        }
                                        p.ApplyForce(v2 * 10.0f);
                                    }
                                }
                            }


                            if (Timer-- < 0 || StopFlag)
                            {
                                foreach (Ped p in AP)
                                {
                                    if (Exists(p) && Player.Character != p)
                                    {
                                        Vector3 v2 = (p.Position - vec);
                                        v2.Normalize();
                                        if (p.isInVehicle())
                                        {
                                            p.Task.ClearAllImmediately();
                                        }
                                        p.ApplyForce(v2 * 10.0f);
                                        p.Health = 0;
                                    }
                                }
                                World.AddExplosion(vec, ExplosionType.Rocket, 10.0f, true, false, 0.1f);


                                EndFlag = true;
                            }
                            break;

                        case 57:
                            Game.DisplayText("No.057 ぶりすた率うｐ", 4000);
                            msgBuff = string.Format("ブリスタ出現率UP");
                            IntervalTimer = 30;
                            EndFlag = true;

                            GTA.Native.Function.Call("REQUEST_MODEL", "BLISTA");


                            break;

                        case 58:
                            Game.DisplayText("No.058　たくさんの運ちゃん", 4000);
                            msgBuff = string.Format("仲間が増えるよ！やったね"+PlayerName+"くん！");
                            IntervalTimer = 30;
                            {
                                Vehicle pV;
                                vec = Player.Character.Position.Around(5.0f);

                                for (int i = 0; i < 10; i++)
                                {
                                    pV = World.CreateVehicle("TAXI", vec + new Vector3(0, 0, i * 0.5f));
                                    if (Exists(pV))
                                    {
                                        pV.NoLongerNeeded();
                                        ped = pV.CreatePedOnSeat(VehicleSeat.Driver);
                                        if (Exists(ped)) { ped.NoLongerNeeded(); }
                                    }
                                }
                            }


                            EndFlag = true;
                            break;

                        #endregion
                        #region 59~63
                        case 59:
                            Game.DisplayText("No.059 全員即死", 4000);
                            msgBuff = string.Format("しぬ");
                            IntervalTimer = 30;

                            AP = World.GetAllPeds();
                            foreach (Ped p in AP)
                            {
                                if (!Exists(p)|| p == Player.Character || p.isRequiredForMission) { continue; }
                                p.Health = -100;
                            }
 
                            EndFlag = true;
                            break;

                        case 60:
                            Game.DisplayText("No.60 デリ焼き", 4000);
                            msgBuff = string.Format("こんがりやけるね");
                            IntervalTimer = 30;
                            for (int i = 0; i < 30; i++)
                            {
                                Ped p = World.CreatePed(new Model(0x45B445F9), Player.Character.Position.Around(rnd.Next(1, 5)));
                                if (Exists(p)) { 
                                    p.NoLongerNeeded();
                                    World.AddExplosion(p.Position, ExplosionType.Molotov, 10);
                                }
                            }
                            EndFlag = true;
                            break;

                        case 61:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.061 強烈な光", 4000);
                                msgBuff = string.Format("輝く"+PlayerName+"さん");
                                IntervalTimer = 30;
                                light = new Light(Color.White, 100.0f, 500.0f, Player.Character.Position+new Vector3(0,0,1.0f));
                                light.Enabled = true;
                                InitFlag = false;
                                Timer = 200;
                                miniMsg = "強烈な光";
                            }
                            

                            if (Timer > 0)
                            {
                                light.Position = Player.Character.Position + new Vector3(0, 0, 1.0f);
                            }


                            if (Timer-- < 0 || StopFlag)
                            {
                                light.Disable();
                                EndFlag = true;
                            }
                            break;

                        case 62:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.062 車消失", 4000);
                                msgBuff = string.Format("車消失");
                                IntervalTimer = 30;

                                InitFlag = false;
                                Timer = 300;
                                miniMsg = "車消失";
                            }

                            if (--Timer < 0 || StopFlag)
                            {

                                EndFlag = true;
                            }

                            if (Timer > 0)
                            {
                                AV = Cacher.GetVehicles(Player.Character.Position, 50.0f);
                                foreach (Vehicle V in AV)
                                {
                                    if (!Exists(V)||V.isRequiredForMission || Player.Character.isInVehicle(V))
                                    {
                                        continue;
                                    }

                                    int N = rnd.Next(1, 5);
                                    for(int i=0;i<N;i++){
                                        Ped p = World.CreatePed(V.Position.Around(0.2f));
                                        if(Exists(p)){
                                            p.NoLongerNeeded();
                                        }
                                    }

                                    V.Delete();
 
                                    
                                }
                            }

                            break;


                            case 63:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.063 斥力", 4000);
                                msgBuff = string.Format("みんな"+PlayerName+"がきらい");
                                IntervalTimer = 30;

                                InitFlag = false;
                                Timer = 300;
                                miniMsg = "斥力発生";
                            }

                            if (--Timer < 0 || StopFlag)
                            {

                                EndFlag = true;
                            }

                            if (Timer > 0)
                            {
                                AV = Cacher.GetVehicles(Player.Character.Position, 10.0f);
                                AP = Cacher.GetPeds(Player.Character.Position,10.0f);
                                
                                foreach(Vehicle veh in AV){
                                    if(!Exists(veh) || veh.isRequiredForMission || Player.Character.isInVehicle(veh)){continue;}
                                    vec = veh.Position - Player.Character.Position;
                                    vec.Normalize();
                                    veh.ApplyForce(3*vec);

                                }
                                

                                foreach(Ped p in AP){
                                    if(!Exists(p) || p.isRequiredForMission || p==Player.Character){continue;}
                                    vec = p.Position - Player.Character.Position;
                                    vec.Normalize();
                                    p.ForceRagdoll(300, false);
                                    p.ApplyForce(3 * vec);
                                }
                            }

                            break;

                        #endregion

                            
                        case 64:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.063 ぽんこつインフェルノ", 4000);
                                msgBuff = string.Format("全車市ニトロ暴走");
                                IntervalTimer = 30;

                                InitFlag = false;
                                Timer = 450;
                                miniMsg = "ぽんこつインフェルノ";

                                Share.PonkotsuInferno = true;
                            }

                            if (--Timer < 0 || StopFlag)
                            {
                                Share.PonkotsuInferno = false;
                                EndFlag = true;
                            }

                            break;

                        case 65:
                            if (InitFlag)
                            {
                                Game.DisplayText("No.065 舞空術", 4000);
                                msgBuff = string.Format("舞空術");
                                IntervalTimer = 30;

                                InitFlag = false;
                                Timer = 300;
                                miniMsg = "舞空術";
                            }

                            if (--Timer < 0 || StopFlag)
                            {

                                EndFlag = true;
                            }

                            if (Timer > 0)
                            {

                                AP = Cacher.GetPeds(Player.Character.Position, 100.0f);



                                foreach (Ped p in AP)
                                {
                                    if (!Exists(p) || p.isRequiredForMission || p == Player.Character || rnd.Next(0,100)<50) { continue; }
                                    p.Task.ClearAllImmediately();
                                    p.MakeProofTo(false, false, false, true, false);
                                    vec = new Vector3().Around(rnd.Next(10,20));
                                    vec.Z = rnd.Next(-20, 20);
                                   // GTA.Native.Function.Call("ADD_EXPLOSION", p.Position.X, p.Position.Y, p.Position.Z, 3, 0.0f, 30, 0, 0.1f);


                                //    World.AddExplosion(p.Position, ExplosionType.Rocket, 0, true, true, 0);
                                    p.ForceRagdoll(300, false);
                                    p.ApplyForce( vec);
                                }
                            }

                            break;

                        case 66:
                                                        if (InitFlag)
                            {
                                Game.DisplayText("No.066 はちゃめちゃシティ", 4000);
                                msgBuff = string.Format("はちゃめちゃシティ＋無敵");
                                IntervalTimer = 30;

                                InitFlag = false;
                                Timer = 300;
                                miniMsg = "はちゃめちゃシティ";
                            }

                            if (--Timer < 0 || StopFlag)
                            {
                                Player.Character.Invincible = false;
                                EndFlag = true;
                            }

                            if (Timer > 0)
                            {
                                Player.Character.Invincible = true;
                                if (Timer % 5 == 0)
                                {
                                    AV = Cacher.GetVehicles(Player.Character.Position, 200.0f);

                                    foreach (Vehicle veh in AV)
                                    {
                                        if (!Exists(veh) || veh.isRequiredForMission || Player.Character.isInVehicle(veh)) { continue; }
                                        if (rnd.Next(0, 100) < 50)
                                        {
                                            veh.Speed = 30;
                                        }
                                        else
                                        {
                                            veh.Speed = -30;
                                        }


                                    }
                                }
                            }
                            break;

                        case 67:
                              if (InitFlag)
                            {
                                Game.DisplayText("No.067 打ち上げ人花火", 4000);
                                msgBuff = string.Format("うちあげひとはなび");
                                IntervalTimer = 30;

                            
                                AP = Cacher.GetPeds(Player.Character.Position, 200.0f);
                                foreach (var p in AP)
                                {
                                    if (Exists(p) && Player.Character != p)
                                    {
                                        World.AddExplosion(p.Position, ExplosionType.Default, 0.0f, true, false, 0.1f);
                                    }
                                }

                                InitFlag = false;
                                Timer = 60;
                                miniMsg = "うちあげひとはなび";
                            }

                            if(Timer<60 && Timer>50 ){
                                foreach (Ped p in AP)
                                {
                                    if (Exists(p) && Player.Character != p)
                                    {
                                       
                                        if (p.isInVehicle())
                                        {
                                            p.Task.ClearAllImmediately();
                                        }
                                        p.Velocity = new Vector3(0,0,14.0f);
                                    }
                                }
                            }

                            if (Timer > 0 && Timer < 50)
                            {
                                foreach (Ped p in AP)
                                {
                                    if (Exists(p) && Player.Character != p && p.Health > 0 && Math.Abs( p.Velocity.Y)<3.0f)
                                    {
                                        World.AddExplosion(p.Position, ExplosionType.Rocket, 10.0f, true, false, 0.1f);
                                        p.Health = 0;
                                    }
                                }
                            }


                            if (Timer-- < 0 || StopFlag)
                            {
                                foreach (Ped p in AP)
                                {
                                    if (Exists(p) && Player.Character != p && p.Health>0)
                                    {
                                        World.AddExplosion(p.Position, ExplosionType.Rocket, 10.0f, true, false, 0.1f);
                                        p.Health = 0;
                                    }
                                }
                                


                                EndFlag = true;
                            }
                            break;


                   

                            default:
                            msgBuff = string.Format("やまびことなって　こだました！");
                            IntervalTimer = 30;
                            EndFlag = true;
                            break;


                    }

                }
                catch
                {
                    Share.ScriptError = true;
                }
            }
        }


        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD.AddInputKey(e.Key);

            if (((inputCheckerMOD.Check(0) || e.Key == Keys.MediaNextTrack ) && ActiveFlag && EndFlag))
            {
                Share.Nico_Parupunte = false;
                ChoseFlag = true;
                screenFont.Color = Color.White;
                msgBuff = PlayerName+"はパルプンテを唱えた！";
                IntervalTimer = 15;


             //   wmp.controls.play();
                
            }

            if (inputCheckerMOD.Check(1))
            {
                StopFlag = true;
                Timer = -1;
                if (FLAG_1)
                {
                    FLAG_1 = false;
                    GTA.World.PedDensity = 1.0f;
                }
            }

            if (inputCheckerMOD.Check(2))
            {
                InitFlag = false;
                ChoseFlag = false;
                ActiveFlag = false;
                EndFlag = true;
                StopFlag = false;
                Game.DisplayText("強制停止", 4000);
            }

        }
        float GetTheta(Vector3 P1, Vector3 P2)
        {
            float th = (P1.X * P2.X + P1.Y * P2.Y + P1.Z * P2.Z) / (P1.Length() * P2.Length());
            return (float)(Math.Acos(th) * 180.0 / Math.PI);
        }

        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;

/*
            if (Player.Character.isInVehicle())
            {
                Vehicle vec = Player.Character.CurrentVehicle;
                if (Exists(vec))
                {
                    e.Graphics.DrawText(string.Format("{0}",vec.Model.Hash), new RectangleF(0.0f, 0.85f, 1.0f, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
                }
            }
            */
            if (IntervalTimer > -1 && Player.Character.isAlive)
            {
                e.Graphics.DrawText(msgBuff, new RectangleF(0.0f, 0.85f, 1.0f, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
            }

            if (Timer > 0)
            {
                if (MaxTimer == -1) { MaxTimer = Timer; }
                e.Graphics.DrawRectangle(new RectangleF(0.4f, 0.97f, 0.2f, 0.03f), Color.FromArgb(150, 0, 0, 0));
                e.Graphics.DrawRectangle(new RectangleF(0.4f, 0.97f, 0.2f*((float)Timer/(float)MaxTimer), 0.03f), Color.FromArgb(150, 0, 0, 255));
                e.Graphics.DrawText(miniMsg, new RectangleF(0.4f, 0.938f, 0.2f, 0.4f), TextAlignment.Center | TextAlignment.Top, miniFont);
                
            }
            
        }

        private Weapon GetWeaponFromRandom(){
            switch (rnd.Next(13))
            {
                case 0:
                    return Weapon.Handgun_DesertEagle;
                case 1:
                    return Weapon.Handgun_Glock;
                                    
                case 2:
                    return Weapon.Heavy_RocketLauncher;
                                                        
                case 3:
                    return Weapon.Melee_BaseballBat;                                    
                case 4:
                    return Weapon.Melee_Knife;                                    
                case 5:
                    return Weapon.Rifle_AK47;                                    
                case 6:
                    return Weapon.Rifle_M4;                                    
                case 7:
                    return Weapon.Shotgun_Baretta;                                    
                case 8:
                    return Weapon.SniperRifle_Basic;                                    
                case 9:
                    return Weapon.SniperRifle_M40A1;                                    
                case 10:
                    return Weapon.Episodic_18;                                    
                case 11:
                    return Weapon.Episodic_22;                                    
                case 12:
                    return Weapon.Episodic_23;                                    

            }
            return Weapon.Handgun_DesertEagle;
        }
    }
}
