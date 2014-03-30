using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using GTA;
using _InputChecker;
using _ItemListForm;

namespace TestScriptCS.Scripts
{
    //メテオストーム
    public class LightsControl : Script
    {
        Light[] l;
        int[] times;
        int MAX;

        public LightsControl()
        {
            ;
        }

        public LightsControl(int n)
        {
            MAX = n;
            Interval = 100;
            l = new Light[MAX];
            times = new int[MAX];

            for (int i = 0; i < MAX; i++)
            {
                times[i] = 0;
            }
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
            this.Tick += new EventHandler(this.lights_Tick);
        }

        public void AddLights(System.Drawing.Color color,float Rsize,float Insinity,Vector3 pos, int t){
            for (int i = 0; i < MAX; i++)
            {
                if (times[i]==0)
                {
                    if (Exists(l[i]))
                    {
                        l[i].Enabled = false;
                        l[i].Intensity = 0;
                        l[i].Range = 0;
                        l[i].Disable();
                        l[i] = null;
                    }
                    l[i] = new Light(color, Rsize, Insinity, pos);
                    l[i].Enabled = true;
                    times[i] = t;
                    return;
                }
            }
        }

        public void CountDown()
        {

            for (int i = 0; i < MAX; i++)
            {
                if (times[i] > 0)
                {
                    times[i]--;
                }
                else
                {
                    if (Exists(l[i]))
                    {
                        l[i].Intensity = 0;
                        l[i].Range = 0;
                        l[i].Enabled = false;
                        l[i].Disable();
                        l[i] = null;
                    }
                }
            }
        }

        private void lights_Tick(object sender, EventArgs e)
        {

        }

        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            GTA.Font screenFont = new GTA.Font(0.03F, FontScaling.ScreenUnits);
            for (int i = 0; i < 10; i++)
            {
                e.Graphics.DrawText(string.Format("{0}", times[i]), new RectangleF(0.1f, 0.04f + i * 0.042f, 0.08f, 0.04f), TextAlignment.Left | TextAlignment.VerticalCenter, screenFont);
            }

        }
    }

    public class MeteoStorm : Script
    {

        InputChecker inputCheckerMeteo = new InputChecker();
        bool AllF = false;
        Random rnd = new Random();
        
        Ped A;
        int Probability;       //実際の確率
        int Disp_Probability;  //画面上の確率
        GTA.Font screenFont;
        bool Easy;
        int seed;
        int F_Probability;
        int T;
        RectangleF RF;
        //書き換え

        int MeteoInterval;
        int ProbabilityChangedTime;

  //      LightsControl lc;
        public MeteoStorm()
        {
        //    lc = new LightsControl(5);
            T = 0;
            GUID = new Guid("060201CC-E734-11DF-9215-D104DFD72085");
            BindScriptCommand("ChangeProbability", new ScriptCommandDelegate(ChangeProbability));
            Easy = false;
            Interval = 400;
            seed = rnd.Next();
            screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
            F_Probability = -1;
            Probability = Disp_Probability = 20;

            MeteoInterval = 0;
            RF = new RectangleF(0.0f, 0.2f, 1.0f, 0.2f);
            inputCheckerMeteo.AddCheckKeys(new Keys[] { Keys.M, Keys.E, Keys.T, Keys.E, Keys.O });
            inputCheckerMeteo.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            inputCheckerMeteo.AddCheckKeys(new Keys[] { Keys.E, Keys.A, Keys.S, Keys.Y });
            this.Tick += new EventHandler(this.Meteoat_Tick);
            KeyDown += new GTA.KeyEventHandler(Meteoat_KeyDown);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }
        //
        private void ChangeProbability(GTA.Script sender, GTA.ObjectCollection Parameter)
        {
            F_Probability = Parameter.Convert<int>(0);
        }

        //  メテオが主人公を避ける条件を満たしているか
        private bool IsMeteoOut()
        {
          
            bool Limit = false;
            if (Player.Character.isInVehicle())
            {
                //車に乗っている
                if (Player.Character.CurrentVehicle.Velocity.Length() < 10.0)   //速度が10.0以下なら避ける
                {
                    Limit = true;
                }

            }
            else
            {
                //車に乗っていない
                if (Player.Character.Velocity.Length() < 5.0)    //速度が5.0以下なら避ける
                {
                    Limit = true;
                }
            }

            return Limit;

        }
     Color HSV2RGB(int H, int S, int V)
        {
            if (H < 0) { H += 360; }
            H = H % 360;
            if (H == 360) H = 0;
            int Hi = (int)Math.Floor((double)H / 60) % 6;

            float f = ((float)H / 60) - Hi;
            float p = ((float)V / 255) * (1 - ((float)S / 255));
            float q = ((float)V / 255) * (1 - f * ((float)S / 255));
            float t = ((float)V / 255) * (1 - (1 - f) * ((float)S / 255));

            p *= 255;
            q *= 255;
            t *= 255;

            Color rgb = new Color();

            switch (Hi)
            {
                case 0:
                    rgb = Color.FromArgb(V, (int)t, (int)p);
                    break;
                case 1:
                    rgb = Color.FromArgb((int)q, V, (int)p);
                    break;
                case 2:
                    rgb = Color.FromArgb((int)p, V, (int)t);
                    break;
                case 3:
                    rgb = Color.FromArgb((int)p, (int)q, V);
                    break;
                case 4:
                    rgb = Color.FromArgb((int)t, (int)p, V);
                    break;
                case 5:
                    rgb = Color.FromArgb(V, (int)p, (int)q);
                    break;
            }


            return rgb;
        } 
    
        private void Meteoat_Tick(object sender, EventArgs e)
        {

            if (rnd.Next(0, 100) < 1)
            {
                GC.Collect();
            }

            if (AllF == true && Player.Character.isAlive)
            {
       //         lc.CountDown();
                T = (T + 17) % 360;
                if (Exists(A)) { A.Delete(); }  //一回前の処理で作ったAさんを消す


                if (F_Probability >= 0) { Probability = F_Probability; MeteoInterval = 0; }
                else
                {
                    if (--MeteoInterval < 0)
                    {
                        MeteoInterval = rnd.Next(25, 101);
                        ProbabilityChangedTime = 50;

                        if (Easy)
                        {
                            Probability = rnd.Next(0, 6) * 10;

                        }
                        else
                        {
                            Probability = rnd.Next(0, 11) * 10;
                        }
                    }
                }


                /*
                Random rnd2 = new Random(((int)Player.Character.Position.X / 80 + (int)Player.Character.Position.Y / 80) + seed + (GTA.World.CurrentDayTime.Minutes / 10));


                //rnd2からその場所での降爆確率を求める
                if (Easy)
                {
                    Probability = rnd2.Next(0, 51);

                }
                else
                {
                    Probability = rnd2.Next(0, 101);
                }
                if (F_Probability >= 0) { Probability = F_Probability; }


                */


                //  降爆確率を元にメテオを１つ降らすか決める
                if (rnd.Next(0, 101) > Probability)
                {
                    return;
                }
                //




                Vector3 posit, playpos;  // posit: Aさんの位置（メテオ落下位置）   playpos:主人公の位置
                Vector3 POS;            // 落下エフェクト描画位置

                playpos = Player.Character.Position;
                posit = playpos;
                posit = posit.Around(rnd.Next(150));    //Aさんの出現位置を150mでランダムに決める
                posit.Z = playpos.Z + 50.0f;            //主人公より50m高い位置にAさんを配置する
                A = GTA.World.CreatePed(posit);         //Aさん召喚

                if (Exists(A))
                {
                    //Aさんの召喚に成功したなら

                    if (IsMeteoOut() && A.Position.DistanceTo2D(Player.Character.Position) < 15.0f)
                    {
                        //メテオがそれる条件を満たしている時に近くにメテオが振りそうならAさんの位置をズラす
                        A.Position = new Vector3(playpos.X + rnd.Next(20, 50), playpos.Y + rnd.Next(20, 50), 50);
                    }

                    POS = A.Position;
                    POS.Z = playpos.Z;
                    A.Visible = false;  //Aさんの姿を透明にする

                    float PlayerVelo = 0;
                    if (Player.Character.isInVehicle())
                    {
                        PlayerVelo = Player.Character.CurrentVehicle.Speed * 3;
                    }

                    //メテオ落下予定地点にエフェクトを描画
                    //車で移動している時は描画範囲を広げる
                    if (Player.Character.Position.DistanceTo2D(POS) < 20.0f + PlayerVelo) {
                        if (Player.Character.Position.Z - World.GetGroundZ(Player.Character.Position) > 2)
                        {
                            GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_red", POS.X, POS.Y, POS.Z, 0, 0, 0, 2.5f);
                        }
                        else
                        {
                            GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_red", POS.X, POS.Y, World.GetGroundZ(POS)+1.0f, 0, 0, 0, 2.5f);
                        }
                   //     this.lc.AddLights(Color.Red, 10.0f, 15.0f, new Vector3(POS.X,POS.Y,World.GetGroundZ(POS)+1.0f), 5);
                    }


                    //発射シーケンス
                    if (A.Exists())
                    {
                        Vector3 Apos = A.Position;

                        //RPGを装備させる
                        A.Weapons.FromType(Weapon.Heavy_RocketLauncher).Ammo = 999;
                        A.Weapons.Select(Weapon.Heavy_RocketLauncher);

                        //真下に向かってRPGを発射させる
                        GTA.Native.Function.Call("FIRE_PED_WEAPON", A, Apos.X, Apos.Y, Apos.Z - 50.0f);
                    }

                }
                else
                {

                }

            }
            else
            {
                //メテオを切ったり、主人公が死んだ場合はAさんを削除
                if (Exists(A)) { A.Delete(); }

            }
            if (!Player.Character.isAlive) { seed = rnd.Next(); }   //死んだらrnd2の種をズラす

       }
        void Meteoat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMeteo.AddInputKey(e.Key);


            if (inputCheckerMeteo.Check(2) == true)
            {
                if (Easy)
                {
                    Easy = false;
                    Game.DisplayText("MeteoStorm EasyMode OFF", 4000);
                }
                else
                {
                    Easy = true;
                    Game.DisplayText("MeteoStorm EasyMode ON", 4000);
                }
            }

            if (inputCheckerMeteo.Check(1) == true)
            {
                AllF = true;
            }
            if (inputCheckerMeteo.Check(0) == true)
            {
                if (AllF)
                {
                    Game.DisplayText("MeteoStorm kai OFF", 4000);
                    AllF = false;
                    if (Exists(A)) { A.Delete(); }


                    
                }
                else
                {
                    Game.DisplayText("MeteoStorm kai ON", 4000);
                    AllF = true;
                    seed = rnd.Next();
                    F_Probability = -1;
                }
            }
        }


        //メータ描画処理
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {

            if (Player.Character.isAlive && AllF && Player.CanControlCharacter)
            {
                //実際の確率と描画の確率とに時間差を作ることでアニメーションさせる

                bool changing = false;
                if (Disp_Probability > Probability) {
                    if (Disp_Probability - Probability > 3)
                    {
                        Disp_Probability -= 3;
                    }
                    else
                    {
                        Disp_Probability--;
                    }
                    changing = true;
                }
                if (Disp_Probability < Probability) {
                    if ( Probability - Disp_Probability> 3)
                    {
                        Disp_Probability += 3;
                    }
                    else
                    {
                        Disp_Probability++;
                    }
                    changing = true;
                }

                if (changing == false && ProbabilityChangedTime > 0)
                {
                    ProbabilityChangedTime--;
                }

                
                if (IsMeteoOut())
                {
                    //メテオが避ける条件を満たす時は灰色で描画
                    screenFont.Color = Color.Gray;
                }
                else
                {

                     screenFont.Color = Color.FromArgb(255, 255, (int)(-2.55 * (float)Disp_Probability) + 255, (int)(-2.55 * (float)Disp_Probability) + 255);
                    
                }
                e.Graphics.Scaling = FontScaling.ScreenUnits; // size on screen will always be the same, regardless of resolution
                RectangleF rect = new RectangleF(0.0043F, 0.93F, 0.1F, 0.1F);
                try
                {
                    e.Graphics.DrawRectangle(new RectangleF(0.0f, 0.6f, 0.045f, 0.4f), Color.FromArgb(150, 0, 0, 0));
                    float Ha = -0.4f*(float)Disp_Probability/100.0f;

                    
                        if (IsMeteoOut())
                        {
                            e.Graphics.DrawRectangle(new RectangleF(0.0f, 1.0f, 0.045f, Ha), Color.FromArgb(200, 50, 50,50));
                        }
                        else
                        {
                            if (Easy)
                            {
                                e.Graphics.DrawRectangle(new RectangleF(0.0f, 1.0f, 0.045f, Ha), Color.FromArgb(150, 100, 100, 255));
                            }
                            else
                            {
                                if (F_Probability == 100)
                                {
                                    e.Graphics.DrawRectangle(new RectangleF(0.0f, 1.0f, 0.045f, Ha), HSV2RGB(T,255,255));
                                }
                                else
                                {
                                    e.Graphics.DrawRectangle(new RectangleF(0.0f, 1.0f, 0.045f, Ha), Color.FromArgb(125, 255, (int)(-2.55 * (float)Disp_Probability) + 255, (int)(-2.55 * (float)Disp_Probability) + 255));
                                }
                            }
                        }

                    /*
                        if (ProbabilityChangedTime > 0)
                        {
                            int alpha = 255;
                            
                            if (ProbabilityChangedTime >80)
                            {
                                alpha = (100 - ProbabilityChangedTime) * 255 / 20;
                            }
                           
                            if (ProbabilityChangedTime < 20)
                            {
                                alpha = 255 * (ProbabilityChangedTime  ) / 20;
                            }

                            if (ProbabilityChangedTime > 0)
                            {
                                ProbabilityChangedTime--;
                            }
                            else
                            {
                                alpha = 0;
                            }

                            GTA.Font bigFont = new GTA.Font(0.1F, FontScaling.ScreenUnits);
                            bigFont.Color = Color.FromArgb(alpha, 255,  255,  255);
                            e.Graphics.DrawText(string.Format("{0}%", Disp_Probability), RF, TextAlignment.Center | TextAlignment.VerticalCenter, bigFont);
                            
                        }*/
                        e.Graphics.DrawText(string.Format("{0}%", Disp_Probability), rect, TextAlignment.Left | TextAlignment.Top, screenFont);

                }
                catch
                {
                    Game.DisplayText("Abort MeteoStorm kai!", 4000);
                    AllF = false;
                }
            }
        }


    }
}
