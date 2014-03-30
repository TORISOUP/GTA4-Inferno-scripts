using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Drawing;
namespace TestScriptCS.Scripts
{
    public class RPGTarget : Script
    {
        public GTA.Object o;
        public Ped p;
        
        public bool ExistObject()
        {
            if (Exists(o))
            {
                return true;
            }
            return false;
        }

        public void FroceToPed()
        {
            if (Exists(o) && Exists(p))
            {
                Vector3 vec = p.Position - o.Position;
                vec.Normalize();
                
                //o.Velocity = o.Velocity.Length() * vec;
                o.ApplyForce(10 * vec);


            }
        }

        public void Set(GTA.Object O, Ped P)
        {
            o = O;
            p = P;
        }
        public RPGTarget()
        {
            o = null;
            p = null;
        }
    
    }

    public class Horming : Script
    {
        InputChecker inputCheckerMOD2 = new InputChecker();
        RPGTarget[] RT;
        bool AF;
        bool ExternalFlag; //trueで動作停止
        bool IsPlayerTarget;
        GTA.Object[] AO;
        bool Meteo = false;
        public Horming()
        {
            IsPlayerTarget = false;

            GUID = new Guid("EEF9D7C2-0D24-11E0-9289-45BCDFD72085");
            BindScriptCommand("HORMING_DEACTIVATE_FLAG", new ScriptCommandDelegate(FlagChange));
            AF = false;
            ExternalFlag = false;
            Interval = 500;
            RT = new RPGTarget[100];
            for (int i = 0; i < RT.Length; i++)
            {
                RT[i] = new RPGTarget();
            }
            inputCheckerMOD2.AddCheckKeys(new Keys[] { Keys.H, Keys.R, Keys.P, Keys.G });
            inputCheckerMOD2.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O,Keys.N });
            inputCheckerMOD2.AddCheckKeys(new Keys[] { Keys.H, Keys.M,Keys.E,Keys.T,Keys.E});
            KeyDown += new GTA.KeyEventHandler(Meteoat_KeyDown);
            this.Tick += new EventHandler(this.MOD_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
        }
        private void FlagChange(GTA.Script sender, GTA.ObjectCollection Parameter)
        {
            ExternalFlag = Parameter.Convert<bool>(0);
        }

        bool CheckLockOn(GTA.Object O)
        {
            try
            {
                for (int i = 0; i < RT.Length; i++)
                {
                    if (RT[i].o == O)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                Game.DisplayText("@HORMING ERROR in CheckLockOn", 4000);
            }
            return false;
        }

        void LockOn(GTA.Object O, Ped P)
        {
            try
            {
                if (Exists(O) && Exists(P))
                {
                    for (int i = 0; i < RT.Length; i++)
                    {
                        if (!RT[i].ExistObject())
                        {
                            RT[i].Set(O, P);
                            if (P == Player.Character)
                            {
                                Blip C = O.AttachBlip();
                                C.Color = BlipColor.LightRed;
                            }
                            return;
                        }
                    }
                }
            }
            catch
            {
                Game.DisplayText("@HORMING ERROR in LockOn", 4000);
            }

        }


        float GetDistanceLineToPoint(Vector3 Po, Vector3 D, Vector3 P)
        {
            Vector3 S = P - Po;
            float Iner = D.X + S.X + D.Y * S.Y + D.Z + S.Z;
            float L = Iner / D.Length();
            L = Math.Abs(L);
            return L;
        }

        float GetTheta(Vector3 P1, Vector3 P2)
        {
            float th = (P1.X * P2.X + P1.Y * P2.Y + P1.Z * P2.Z) / (P1.Length() * P2.Length());
            return (float)(Math.Acos(th) * 180.0 / Math.PI);
        }


        //距離バージョン
        private Ped GetTargetPed(GTA.Object Obj)
        {
            Ped[] Peds;
            Vector3 Po, P, D;
            float MinDist = 30.0f;
            int index = -1;
            D = Obj.Velocity;
            D.Normalize();
            Po = Obj.Position;
            Peds = Cacher.GetPeds(Obj.Position + 20.0f * D, 15.0f);

            for (int i = 0; i < Peds.Length; i++)
            {
                if (!Exists(Peds[i])) { continue; }
                if (Peds[i].isDead) { continue; }
                P = Peds[i].Position;
                float Dist = GetDistanceLineToPoint(Po, D, P);
                if (Dist < MinDist)
                {
                    MinDist = Dist;
                    index = i;
                }

            }
            if (index > -1)
            {
                return Peds[index];
            }
            else
            {
                return null;
            }
        }
        //角度バージョン
        private Ped GetTargetPed2(GTA.Object Obj)
        {
            Ped[] Peds;
            float MinTheta = 33.0f;
            int index = -1;
            Vector3 D = Obj.Velocity;
            D.Normalize();
            Peds = Cacher.GetPeds(Obj.Position , 100.0f);

            for (int i = 0; i < Peds.Length; i++)
            {
                if (!Exists(Peds[i])) { continue; }
                if (Peds[i].isDead) { continue; }
                Vector3 P = Peds[i].Position - Obj.Position;
                P.Normalize();
                float Theta = Math.Abs(GetTheta(P, D));

                if (Theta < MinTheta)
                {
                    MinTheta = Theta;
                    index = i;
                }
            }
            if (index > -1)
            {
                return Peds[index];
            }
            else
            {
                return null;
            }
        }

        private bool isObjectMeteo(GTA.Object O)
        {
            Vector3 Po = O.Velocity;
            Po.Normalize();
            Vector3 G = new Vector3(0, 0, -1.0f);

            if (GetTheta(Po, G) <= 5.0f)
            {
                return true;
            }
            return false;

        }
        private void MOD_Tick(object sender, EventArgs e)
        {
            if (AF == false || ExternalFlag==true) { return; }
            AO = World.GetAllObjects(new Model(0x5A6525AE));
            try
            {
                for (int i = 0; i < AO.Length; i++)
                {
                    if (Exists(AO[i]))
                    {
                        if (!Meteo)
                        {
                            if (isObjectMeteo(AO[i])) { continue; }
                        }
                        if (CheckLockOn(AO[i])) { continue; }

                        Ped P = GetTargetPed2(AO[i]);
                        if (Exists(P))
                        {

                            LockOn(AO[i], P);

                        }
                    }
                }
            }
            catch
            {
                Game.DisplayText("@HORMING ERROR in Main", 4000);
                Share.ScriptError = true;
            }
            bool Pflag = false;
            for (int i = 0; i < RT.Length; i++)
            {
                if (Exists(RT[i]))
                {
                    RT[i].FroceToPed();
                    if (RT[i].ExistObject() && RT[i].p == Player.Character)
                    {
                        Pflag = true;
                    }
                }
            }
            IsPlayerTarget = Pflag;
        }
        void Meteoat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD2.AddInputKey(e.Key);
            if (inputCheckerMOD2.Check(0) == true)
            {
                if (AF)
                {
                    ExternalFlag = false;
                    AF = false;
                    Game.DisplayText("HORMING OFF", 4000);
                }
                else
                {
                    Player.Character.Weapons.FromType(Weapon.Heavy_RocketLauncher).Ammo = 9;
                    AF = true;
                    Game.DisplayText("HORMING ON", 4000);
                }
            }
            if (inputCheckerMOD2.Check(1) == true)
            {
                AF = true;
            }

            if (inputCheckerMOD2.Check(2))
            {
                if (Meteo)
                {
                    Game.DisplayText("METEO HORMING OFF", 4000);
                    Meteo = false;
                }
                else
                {
                    Game.DisplayText("METEO HORMING ON", 4000);
                    Meteo = true;
                }


            }



        }
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            if (!Player.CanControlCharacter) { return; }
            if (!IsPlayerTarget) { return; }
            if (ExternalFlag) { return; }
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            e.Graphics.DrawRectangle(new RectangleF(0, 0, 1.0f, 1.0f), Color.FromArgb(30,255,0,0));


        }
    }
}
