using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //緊急脱出
    public class EmergencyExit : Script
    {
        Ped[] ped;
        bool ExitNow = false;

        public EmergencyExit()
        {
            ped = new Ped[3];
            this.Tick += new EventHandler(this.Nikita_Tick);

        }

        private void Nikita_Tick(object sender, EventArgs e)
        {
            if (Player.Character.isInVehicle())
            {
                if (ExitNow == true)    //ここの処理が実行されることはほぼ無い
                {
                    Player.CanControlRagdoll = false;
                    Player.Character.isRagdoll = false;
                    ExitNow = false;
                }

                if (Game.isGameKeyPressed(GameKey.Crouch))  //左スティックが押し込まれたら
                {
                    if (Game.isGameKeyPressed(GameKey.EnterCar)) //Yボタンが押されたら
                    {
                        Vehicle pV = Player.Character.CurrentVehicle;
                        Vector3 Po = Player.Character.Position;
                        Vector3 vel = Player.Character.CurrentVehicle.Velocity;


                        ped = Player.Group.ToArray(false);  //主人公の仲間を取得

                     //   GTA.Native.Function.Call("WARP_CHAR_FROM_CAR_TO_COORD", Player.Character, Po.X, Po.Y, Po.Z + 0.5f);

                        Player.Character.Task.ClearAllImmediately();    //主人公を強制的に車外に出す

                        if (GTA.Native.Function.Call<bool>("IS_BIG_VEHICLE", pV))
                        {
                            if (Player.Character.Position.Z - World.GetGroundZ(Player.Character.Position) > 30)
                            {
                                Player.Character.Position += new Vector3(0, 0, -1.0f);
                            }
                            else
                            {
                                Player.Character.Position += new Vector3(0, 0, 1.0f);
                            }
                        }

                        Player.Character.Velocity = 0.3f*vel;
                        Player.CanControlRagdoll = true;
                        Player.Character.isRagdoll = true;
                        Player.Character.ApplyForce(new Vector3(0, 0, 8.0f));   
                      //  Player.Character.Velocity += new Vector3(0, 0, 10);

                        //仲間が同じ車両に乗ってるなら一緒に緊急脱出させる
                        for (int i = 0; i < ped.Length; i++)
                        {
                            if (Exists(ped[i]))
                            {
                                if (ped[i].CurrentVehicle == pV)
                                {
                                    ped[i].Task.ClearAllImmediately();
                                    ped[i].Velocity = 0.3f * vel;
                                    ped[i].ApplyForce(new Vector3(0, 0, 18));
                                    ped[i].ForceHelmet(true);
                                }
                            }
                        }

                        Game.DisplayText("緊急脱出", 8000);
                        Player.Character.MakeProofTo(false, true, false, true, false);
                        ExitNow = true;
                        Player.Character.ForceHelmet(true);
                        
                    }
                }

            }
            else
            {
                if (ExitNow == true)
                {
                    if (Game.isGameKeyPressed(GameKey.Sprint))
                    {
                        Player.CanControlRagdoll = false;
                        Player.Character.isRagdoll = false;
                        ExitNow = false;
                        Player.Character.MakeProofTo(false, false, false, false, false);
                    }
                }
            }

        }
  
    }
}
