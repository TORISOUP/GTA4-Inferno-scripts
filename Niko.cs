using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTA;
using _InputChecker;
using LiveNicoAPIAlert.LiveNicoAPIs;
using PrjNikoNiko;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using remoteGTA;
namespace remoteGTA
{
    public class processListShare : MarshalByRefObject
    {

        public string CommentData;
        // デリゲート宣言
        public delegate void CallEventHandler();
        // 登録用変数
        public CallEventHandler _onMsg;
        /// <summary>
        /// メッセージ(クライアント側で呼ばれる関数)
        /// もちろんpublic関数でないといけない
        /// </summary>
        public void onMsg()
        {
            if (_onMsg != null)
            {
                _onMsg();
            }
        }
        /// <summary>
        /// 有効期間初期化のオーバーライド
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            //      ILease lease = (ILease)base.InitializeLifetimeService();
            //      if (lease.CurrentState == LeaseState.Initial)
            //      {
            //        lease.InitialLeaseTime = TimeSpan.FromMinutes(0);   // リースの初期時間を取得または設定します。既定は5分、0で無限になります
            //        lease.SponsorshipTimeout = TimeSpan.FromMinutes(2); // スポンサがリースの更新時間を返すまで待機する時間を取得または設定します。
            //        lease.RenewOnCallTime = TimeSpan.FromSeconds(2);    // リモート オブジェクトに対する呼び出しによって、CurrentLeaseTime が更新されるのにかかる時間を取得または設定します。
            //      }
            return null; // NULLを返すことによって無限になる。leaseを返すのであれば設定されたとおりの有効期限を反映する
        }
    }


}


namespace TestScriptCS.Scripts
{
    class Niko:Script
    {
        string comment;
        NikoInfo niko;
        string text;
        InputChecker inputChecker;
        bool IsActive = true;
        private static processListShare _msg;
        Random rnd;
        GTA.Font screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
        string OldComment="";
        Stopwatch stopWatch;
        string message;
        int N = 0;
        List<Ped> ThefList = new List<Ped>();
        Stopwatch IsonoTimer;
        Stopwatch NakajimaTimer;
        Stopwatch ISASAKATimer;
        public Niko()
        {
            niko = new NikoInfo();
            rnd = new Random();
            inputChecker = new InputChecker();
            inputChecker.AddCheckKeys(new Keys[] { Keys.N, Keys.I, Keys.C, Keys.O });
            KeyDown += new GTA.KeyEventHandler(Niko_KeyDown);
           

            IpcServerChannel isc = new IpcServerChannel("processList");
            // リモートオブジェクト登録
            ChannelServices.RegisterChannel(isc, true);
            // イベント登録
            _msg = new processListShare();
            // メッセージを受け取ったら実行する関数を登録する
            _msg._onMsg += new processListShare.CallEventHandler(onMsg);
            RemotingServices.Marshal(_msg, "comment", typeof(processListShare));


            this.Tick += new EventHandler(this.nico_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

            Interval = 10000;
            IsonoTimer = new Stopwatch();
            NakajimaTimer = new Stopwatch();
            ISASAKATimer = new Stopwatch();
            comment = "";
        }

        public  void onMsg()
        {
            if (rnd.Next(0, 100) < 70)
            {
                comment += _msg.CommentData;
            }
         //   if (!Player.Character.isAlive || !IsActive) { return; }



         //   if (comment.Contains("しにとろ"))
         //   {
         ////       Nitro = true;
         //   }

         //   if (comment.Contains("せきりょく"))
         //   {
         // //      Force = true;
         //   }


        }


        void Niko_KeyDown(object sender, GTA.KeyEventArgs e)
        {

            inputChecker.AddInputKey(e.Key);

            if (inputChecker.Check(0) == true)
            {
                if (IsActive)
                {
                    Game.DisplayText("コメントのやつオフ");
                    IsActive = false;
                }
                else
                {
                    Game.DisplayText("コメントのやつオン");
                    IsActive = true;
                }

            }

            
        }


        void Thef()
        {

            var AV = World.GetVehicles(Player.Character.Position, 200.0f);

            foreach (var car in AV)
            {
                if (!Exists(car)|| !car.isAlive || !car.isDriveable) { continue; }

                var ped = World.GetClosestPed(car.Position, 15);
                if (ped == null || !Exists(ped) || !ped.isAlive || ped.isRequiredForMission || ped.isInVehicle()) { continue; }

                ped.Money = 500;
                ped.Task.ClearAllImmediately();
                ped.Task.EnterVehicle(car, VehicleSeat.Driver);
                ThefList.Add(ped);
            }



        }


        void Nitoro()
        {
            var allV = GTA.World.GetVehicles(Player.Character.Position, 100.0f);
            int Length = allV.Length;
            if (Length <= 0) { return; }
            for (int i = 0; i < Length; i++)
            {
                if (Exists(allV[i]))
                {
                    if (allV[i] != Player.Character.CurrentVehicle)
                    {
                        if (!allV[i].isRequiredForMission)
                        {
                            Ped a = allV[i].GetPedOnSeat(VehicleSeat.Driver);

                            if (true)
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

                                if (Speed > 0 && rnd.Next(0, 100) <= 10)
                                {
                                    allV[i].Rotation = new Vector3(allV[i].Rotation.X + (rnd.Next(0, 100) * 60) / 100, allV[i].Rotation.Y, allV[i].Rotation.Z);
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

        }







        void nico_Tick(object sender, EventArgs e)
        {

            if (!Player.Character.isAlive)
            {
                var p = Player.Group;
                foreach (var ped in p)
                {
                    if (Exists(ped))
                    {
                        ped.NoLongerNeeded();
                        ped.isRequiredForMission = false;
                        Player.Group.RemoveMember(ped);
                    }
                }
                foreach (var pe in ThefList)
                {
                    if (!Exists(pe)) { continue; }
                    pe.Money = 0;
                }
            }
            else
            {
                var array = ThefList.ToArray();
                foreach (var pe in array)
                {
                    if (!Exists(pe)) { continue; }
                    if (pe.isInVehicle()) { pe.Money = 0; }
                    ThefList.Remove(pe);
                }
            }

            String Mess = "";

            if (!Player.Character.isAlive || !IsActive) { return; }

            if (OldComment != comment)
            {
                if (comment.Contains("ぱるぷんて"))
                {
                    Share.Nico_Parupunte = true;
                    Mess += "ぱるぷんて ";
                }

                if (comment.Contains("ひとはなび"))
                {
                    HANABI.PedStart();
                    
                    Mess += "ひとはなび ";
                }

                if (comment.Contains("なかじま"))
                {
                    if (NakajimaTimer.ElapsedMilliseconds > 180*1000 || !NakajimaTimer.IsRunning)
                    {
                        NakajimaTimer.Restart();
                        NAKAJIMA.NAKAJIMAStart();
                        Mess +="なかじま(OK) ";
                    }
                    else
                    {
                        Mess += "なかじま("+ (180 - (NakajimaTimer.ElapsedMilliseconds / 1000)) + ")";

                    }
                }


                if (comment.Contains("いささか"))
                {
                    if (ISASAKATimer.ElapsedMilliseconds > 30 * 1000 || !ISASAKATimer.IsRunning)
                    {
                        ISASAKATimer.Restart();
                        ISASAKA.ISASAKAStart();
                        Mess += "いささか(OK) ";
                    }
                    else
                    {
                        Mess += "いささか(" + (30 - (ISASAKATimer.ElapsedMilliseconds / 1000)) + ")";

                    }
                }



                if (comment.Contains("いその"))
                {
                    if (IsonoTimer.ElapsedMilliseconds > 180000 || !IsonoTimer.IsRunning)
                    {
                        IsonoTimer.Restart();
                        ISONO.IsonoStart();
                        Mess += "いその(OK) ";
                    }
                    else
                    {
                        Mess += "いその(" + (180 - (IsonoTimer.ElapsedMilliseconds / 1000)) + ")";

                    }
                }

                if (comment.Contains("ぼるが"))
                {
                    VOLGA.VOLGAStart();
                    Mess += "ぼるが ";
                }


                if (comment.Contains("くるましょうしつ"))
                {
                    var AV = World.GetVehicles(Player.Character.Position, 70.0f);
                    foreach (Vehicle V in AV)
                    {
                        if (!Exists(V) || V.isRequiredForMission || Player.Character.isInVehicle(V))
                        {
                            continue;
                        }

                        int N = rnd.Next(1, 3);
                        for (int i = 0; i < N; i++)
                        {
                            Ped p = World.CreatePed(V.Position.Around(0.2f));
                            if (Exists(p))
                            {
                                p.NoLongerNeeded();
                            }
                        }

                        V.Delete();


                    }


                    Mess += "くるましょうしつ ";
                }


                if (comment.Contains("しにとろ"))
                {
                    Nitoro();
                    Mess += "しにとろ ";
                }


                if (comment.Contains("くるまついか"))
                {
                    
                    var car = World.CreateVehicle(Player.Character.Position + new Vector3(0,0,12));
                    if (Exists(car))
                    {
                        car.NoLongerNeeded();
                        car.FreezePosition = false;
                        car.ApplyForce(new Vector3(0,0,2));
                        Vector3 Pos = Player.Character.Position;
                        GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_blue", Pos.X, Pos.Y, Pos.Z, 0, 0, 0, 2.0f);


                    }

                    Mess += "くるまついか ";
                }



                /*
               if (comment.Contains("くるまはなび"))
                {
                    HANABI.CarStart();
                    SetComment("くるまはなびを受け付けました");
                }*/

                #region ROMAN
                if (comment.Contains("ろーまん"))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(0x89395FC9), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(0x89395FC9));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(0x89395FC9), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        Mess += "ろーまん ";
                    }
                    catch
                    {

                    }
                }

                if (comment.Contains("まにー"))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(0x5629F011), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(0x5629F011));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(0x5629F011), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        Mess += "まにー ";
                    }
                    catch
                    {

                    }
                }

                if (comment.Contains("ぐれいしー"))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(0xEAAEA78E), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(0xEAAEA78E));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(0xEAAEA78E), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        Mess += "ぐれいしー ";
                    }
                    catch
                    {

                    }
                }


                if (comment.Contains("まいける"))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(0x2BD27039), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(0x2BD27039));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(0x2BD27039), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 10; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        Mess += "まいける ";
                    }
                    catch
                    {

                    }
                }

               
                if (comment.Contains("ぱっきー"))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(0x64C74D3B), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(0x64C74D3B));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(0x64C74D3B), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        Mess += "ぱっきー ";
                    }
                    catch
                    {

                    }
                }


                #endregion
                string strchekc = "";
                uint modelID;

                #region その他

                strchekc = "ぶるーしー";
                modelID = 0x98E29920;
                if (comment.Contains(strchekc))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(modelID));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                         Mess += strchekc+" ";
                    }
                    catch
                    {

                    }
                }

                #endregion
                #region その他

                strchekc = "やるお";
                modelID = 0x6AF081E8;
                if (comment.Contains(strchekc))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(modelID));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        Mess += "やるお ";
                    }
                    catch
                    {

                    }
                }

                #endregion
                #region その他

                strchekc = "でりっく";
                modelID = 0x45B445F9;
                if (comment.Contains(strchekc))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(modelID));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                         Mess += strchekc + " ";
                    }
                    catch
                    {

                    }
                }

                #endregion
                #region その他

                strchekc = "ぺごりーの";
                modelID = 0xEA28DB14;
                if (comment.Contains(strchekc))
                {
                    try
                    {
                        Ped p;
                        if (!Player.Character.isInVehicle())
                        {
                            p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5)));
                        }
                        else
                        {

                            var car = Player.Character.CurrentVehicle;
                            p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat, new Model(modelID));


                        }
                        if (!Exists(p)) { p = World.CreatePed(new Model(modelID), Player.Character.Position.Around(rnd.Next(1, 5))); }
                        if (Exists(p)) { p.Health = 500; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        Mess += strchekc + " ";
                    }
                    catch
                    {

                    }
                }

                #endregion



                if (comment.Contains("しみんついか"))
                {
                    try
                    {
                        for (int MAX = rnd.Next(1, 5), i = 0; i < MAX; i++)
                        {
                            Ped p;
                            if (!Player.Character.isInVehicle())
                            {
                                p = World.CreatePed(Player.Character.Position.Around(rnd.Next(1, 5)));
                            }
                            else
                            {

                                var car = Player.Character.CurrentVehicle;
                                p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat);


                            }
                            if (!Exists(p)) { Player.Character.Position.Around(rnd.Next(1, 5)); }
                            if (Exists(p)) { p.Health = 100; p.isRequiredForMission = false; p.NoLongerNeeded(); }
                        }
                        Mess += "しみんついか "; 
                    }
                    catch
                    {

                    }
                }
                if (comment.Contains("なかまついか"))
                {
                    try
                    {
                   
                            Ped p;
                            if (!Player.Character.isInVehicle())
                            {
                                p = World.CreatePed(Player.Character.Position.Around(rnd.Next(1, 5)));
                            }
                            else
                            {

                                var car = Player.Character.CurrentVehicle;
                                p = car.CreatePedOnSeat(VehicleSeat.AnyPassengerSeat);


                            }
                            if (!Exists(p)) { Player.Character.Position.Around(rnd.Next(1, 5)); }
                            if (Exists(p)) { p.Health = 100;
                            Player.Group.AddMember(p);
                            p.CurrentRoom = Player.Character.CurrentRoom; // required, or ped won't be visible when spawned inside a building
                            p.WillDoDrivebys = true;
                            p.PriorityTargetForEnemies = true;
                            p.DuckWhenAimedAtByGroupMember = false;
                            p.AlwaysDiesOnLowHealth = true;
                            p.SetPathfinding(true, true, true);
                            p.CanSwitchWeapons = true;
                            //   p.isRequiredForMission = false;
                            p.Money = 500;
                            p.Health = 300;
                            p.Invincible = false;
                            p.Weapons.Uzi.Ammo = 30000;
                            switch (rnd.Next(6))
                            {
                                case 0:
                                    p.Weapons.MP5.Ammo = 30000;
                                    p.Weapons.Select(Weapon.SMG_MP5);
                                    break;
                                case 1:
                                    p.Weapons.AssaultRifle_M4.Ammo = 30000;
                                    p.Weapons.Select(Weapon.Rifle_M4);
                                    break;
                                case 2:
                                    p.Weapons.RocketLauncher.Ammo = 30000;
                                    p.Weapons.Select(Weapon.Heavy_RocketLauncher);
                                    break;
                                case 3:
                                    p.Weapons.MolotovCocktails.Ammo = 30000;
                                    p.Weapons.Select(Weapon.Thrown_Molotov);
                                    break;
                                case 4:
                                    p.Weapons.DesertEagle.Ammo = 30000;
                                    p.Weapons.Select(Weapon.Handgun_DesertEagle);
                                    break;
                                case 5:
                                    p.Weapons.RocketLauncher.Ammo = 30000;
                                    p.Weapons.Select(Weapon.Heavy_RocketLauncher);
                                    break;

                                default:

                                    p.Weapons.BaseballBat.Ammo = 2000;
                                    p.Weapons.Select(Weapon.Melee_BaseballBat);
                                    break;


                            }
                            p.RelationshipGroup = RelationshipGroup.Player;
                            p.ChangeRelationship(RelationshipGroup.Player, Relationship.Companion);
                            p.CantBeDamagedByRelationshipGroup(RelationshipGroup.Player, true);
                            p.DuckWhenAimedAtByGroupMember = true;
                            
                            
                            }

                            Mess += "なかまついか "; 
                    }
                    catch
                    {

                    }
                }



                if (comment.Contains("えんごしゃげき"))
                {
                    Share.Nico_Engo += rnd.Next(1, 10);
                    Mess += "えんごしゃげき"+ Share.Nico_Engo+"発";
                }

                if (comment.Contains("ごうとう"))
                {
                    Thef();
                    Mess += "ごうとう "; 
                }



            }



            if (Mess != "") { SetComment(Mess); }

            OldComment = comment;
            comment = "";
        }


        private void SetComment(String mess)
        {
            message = mess;
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            if (stopWatch != null && stopWatch.IsRunning)
            {
                if (stopWatch.ElapsedMilliseconds < 4000)
                {
                    e.Graphics.DrawText(message, new RectangleF(0.0f  , 0.01f, 1.0f, 0.1f), TextAlignment.Center | TextAlignment.Top, screenFont);
                }
                else
                {
                    stopWatch.Stop();
                }
            }
        }

    }
}
