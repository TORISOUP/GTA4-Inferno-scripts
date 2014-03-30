using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //市民車両強盗

    //基本的には確率で強盗を行わせていない
    //条件を満たせば必ず強盗行為を行わせている
    //処理の周期を遅くしてあるからたまに強盗するように見える

    //かなり複雑な方のMODです

    public class Goutou : Script
    {
        InputChecker inputCheckerGou = new InputChecker();
        Ped[] allPed;
        Ped[] actPed;
        bool AllF;
        int check;
        int index;
        Random rnd;
        public Goutou()
        {
            rnd = new Random();
            AllF = false;
            Interval = 2000;
            actPed = new Ped[20];   //強盗中の市民
            index = 0;              //indexナンバー(2秒に一回インクリメント
            check = -1;
            inputCheckerGou.AddCheckKeys(new Keys[] { Keys.G, Keys.O, Keys.U, Keys.T, Keys.O, Keys.U });
            inputCheckerGou.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N});
            this.Tick += new EventHandler(this.Gouat_Tick);
            KeyDown += new GTA.KeyEventHandler(Gouat_KeyDown);

        }
        private void Gouat_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Player.CanControlCharacter == false)
                {
                    check = -1;
                    return;
                }
                if (AllF == true)
                {
                    //強盗中の市民を2秒に一人ずつチェックする
                    if (Exists(actPed[index]))
                    {
                        if (actPed[index].isInVehicle() && actPed[index].isAlive)
                        {
                            //無事車を奪えたなら
                            actPed[index].Money = 0;
                            actPed[index].WantedByPolice = true;
                        }
                        else
                        {
                            //まだ車を奪えてなければインクリメント
                            index = (index + 1) % 20;

                        }
                    }

                    if (check == -1)
                    {
                        //市民取得
                        allPed = World.GetPeds(Player.Character.Position, 50.0f);
                    }

                    //取得した周辺市民を2秒に1回ずつ調べる
                    check++;
                    if (check == allPed.Length)
                    {
                        check = -1;
                        return;
                    }


                    if (Exists(allPed[check]))
                    {
                        if (allPed[check].Money == 500 || allPed[check].isRequiredForMission) { return; }   //Moneyが500(なんか行動中)だったりミッション関係のキャラは除外
                        if (allPed[check].isInVehicle() && rnd.Next(0, 10) < 8) { return; } //既に乗車中の市民は20%の確率で強盗を始める
                        
                        //以下強盗処理
                        Ped A = allPed[check];  //対象市民
                        Vehicle V = GTA.World.GetClosestVehicle(A.Position, 15.0f); //対象市民に最も近い車を取得

                        if (Exists(V))
                        {
                            //市民の近くに車があるなら
                            if (Exists(actPed[index]))
                            {
                                //既に強盗行為中市民をフリーにして新しい市民で上書き
                                if (actPed[index].Money == 500)
                                {
                                    actPed[index].Money = 0;
                                }
                            }
                            actPed[index] = A;
                            A.Money = 500;      //Moneyを500にする（ChaosModeMODのreadme参照
                            int r = rnd.Next() % 5;
                            switch (r)
                            {
                                    // 1/5の確率で運転席以外の座席に乗り込む
                                case 0:
                                    A.Task.EnterVehicle(V, VehicleSeat.AnyPassengerSeat);
                                    break;

                                default:
                                    A.Task.EnterVehicle(V, VehicleSeat.Driver);
                                    break;
                            }

                        }
                        else
                        {
                            //市民の近くに車が無かったとき
                            if (rnd.Next() % 10 == 1 && Player.Character.isInVehicle()) // 1/10の確率で主人公の車を奪う
                            {
                                actPed[index] = A;
                                A.Money = 500;
                                switch (rnd.Next() % 4)
                                {
                                        // 1/4の確率で運転席以外の座席に乗り込む
                                    default:
                                        A.Task.EnterVehicle(Player.Character.CurrentVehicle, VehicleSeat.Driver);
                                        break;
                                    case 1:
                                        A.Task.EnterVehicle(Player.Character.CurrentVehicle, VehicleSeat.AnyPassengerSeat);
                                        break;
                                }
                            }
                        }

                    }

                    index = (index + 1) % 20;

                }
            }
            catch
            {
                //どこかでエラーが起きたらMODを停止させる
                Game.DisplayText("THIEVES error!", 4000);
                Share.ScriptError = true;
                AllF = false;
                return;
            }

       }


        void Gouat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerGou.AddInputKey(e.Key);
            if (inputCheckerGou.Check(1) == true)
            {
                AllF = true;
            }
            if (inputCheckerGou.Check(0) == true)
            {
                if (AllF)
                {
                    Game.DisplayText("THIEVES OFF", 4000);
                    AllF = false;
                }
                else
                {
                    Game.DisplayText("THIEVES ON", 4000);
                    AllF = true;
                }
            }
        }
    }
}
