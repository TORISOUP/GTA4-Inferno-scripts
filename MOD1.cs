using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

 
    public class MOD2 : Script
    {
        InputChecker inputCheckerMOD = new InputChecker();
        bool ActiveFlag;
        Random rnd;

        Vector3 K_P,K_D,S_P;
        bool KamikazeFlag;
        float Speed;
        Blip KB;
        Ped[] peds;
        Vehicle[] vehs;

        int pedIndex,vehIndex;


        public MOD2()
        {
            Speed = 1.0f;
            KamikazeFlag = false;
            ActiveFlag = false;
            rnd = new Random();
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.K, Keys.A, Keys.M, Keys.I });
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            Interval = 100;
            this.Tick += new EventHandler(this.MOD_Tick);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
        }

        public Vector3 RotationVector(Vector3 src, float theata)
        {
            float X, Y, Z;
            float rad = theata * (float)Math.PI / 180.0f;

            X = (float)Math.Cos(rad) * src.X - (float)Math.Sin(rad) * src.Y;
            Y = (float)Math.Sin(rad) * src.X + (float)Math.Cos(rad) * src.Y;
            Z = src.Z * rnd.Next(10, 40);

            return new Vector3(X, Y, Z);

        }


        private void MOD_Tick(object sender, EventArgs e)
        {
            if (ActiveFlag)
            {
                ///-------------------------------------------------------------
                if (KamikazeFlag)
                {
                    if (K_P.DistanceTo(S_P) > 100.0f)
                    {
                        if (Exists(KB)) { KB.Delete(); }
                        KamikazeFlag = false;
                        Player.Character.Invincible = false;
                    }
                    else
                    {


                        if (Exists(peds[pedIndex]) && peds[pedIndex] != Player.Character)
                        {
                            Vector3 Force = K_P - peds[pedIndex].Position;
                            Force.Normalize();

                            Force = RotationVector(Force, 20.0f);////////////////////

                            Force = 20.0f * Force;
                            peds[pedIndex].ApplyForce(Force);
                        }
                        if (Exists(vehs[vehIndex]) && vehs[vehIndex] != Player.Character.CurrentVehicle)
                        {
                            Vector3 Force = K_P - vehs[vehIndex].Position;
                            Force.Normalize();

                            Force = RotationVector(Force, 20.0f);//////////////

                            Force = 50.0f * Force;
                            vehs[vehIndex].ApplyForce(Force);
                        }

                        pedIndex = (pedIndex + 1) % peds.Length;
                        vehIndex = (vehIndex + 1) % vehs.Length;

                    }


                    K_P += Speed * K_D;
                    if (Exists(KB)) { KB.Delete(); }
                    KB = Blip.AddBlip(K_P);



                }
                else
                {
                    if (Exists(KB)) { KB.Delete(); }
                    KB = Blip.AddBlip(K_P);
                    if (Player.Character.Weapons.Current == Weapon.Melee_BaseballBat || true)
                    {
                        if (Game.isGameKeyPressed(GameKey.Reload))
                        {
                            Game.DisplayText("神風だ！", 4000);

                            K_P = Player.Character.Position;
                            S_P = K_P;
                            float theata = (Player.Character.Heading + 90.0f) * (float)Math.PI / 180.0f;
                            K_D = new Vector3((float)Math.Cos(theata), (float)Math.Sin(theata), 0);
                            K_D.Normalize();

                            peds = World.GetPeds(K_P, 400.0f);
                            vehs = World.GetVehicles(K_P, 400.0f);
                            pedIndex = 0;
                            vehIndex = 0;
                            Player.Character.Invincible = true;
                            KamikazeFlag = true;

                        }
                    }
                }

                ///------------------------------------------------------------------
            }
        }


        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD.AddInputKey(e.Key);

            if (inputCheckerMOD.Check(1))
            {
                //ActiveFlag = true;
            }

            if (inputCheckerMOD.Check(0) == true)
            {
                if (ActiveFlag)
                {
                    ActiveFlag = false;
                    KamikazeFlag = false;
                    if (Exists(KB)) { KB.Delete(); }
                    KB = Blip.AddBlip(K_P);
                    Game.DisplayText("Kamikaze OFF", 4000);
                }
                else
                {
                    ActiveFlag = true;
                    
                    Game.DisplayText("Kamikaze ON", 4000);
                }
            }


        }
    }
}
