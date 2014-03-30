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

    public class MeteoStorm : Script
    {

        InputChecker inputCheckerBomb = new InputChecker();
        InputChecker inputCheckerAll = new InputChecker();
        bool AllF = false;
        Random rnd = new Random();
        Ped A;
        Blip target;
        bool Limit;
         
        public MeteoStorm()
        {

            Interval = 1000;
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.M, Keys.E, Keys.T, Keys.E, Keys.O });
            inputCheckerAll.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            this.Tick += new EventHandler(this.Bombat_Tick);
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (AllF == true && Player.Character.isAlive)
            {
                //
                if (Exists(A)) { A.Delete(); }
                if (Exists(target)) { target.Delete(); }

                Vector3 posit,playpos;
                Vector3 POS;
                try
                {
                    GTA.Native.Pointer fx = typeof(float);
                    GTA.Native.Pointer fy = typeof(float);
                    GTA.Native.Pointer fz = typeof(float);
                    GTA.Native.Function.Call("GET_CHAR_COORDINATES", Player.Character, fx, fy, fz);
                    playpos.X=fx;
                    playpos.Y=fy;
                    playpos.Z=fz;
                }
                catch
                {
                    Game.DisplayText("MeteoStorm Error 01", 4000);
                    return;
                }

                    posit = playpos;
                    posit = posit.Around(rnd.Next(150));
                    posit.Z = playpos.Z+50.0f;
                    A = GTA.World.CreatePed(posit);

                    if (Exists(A))
                    {
                       // GTA.Native.Function.Call("DISPLAY_TEXT_WITH_FLOAT", 0.2f, 0.2f, "NUMBER", Player.Character.Velocity.Length(),1);
                        Limit = false;
                        if (Player.Character.isInVehicle())
                        {
                            //車に乗っている
                            if (Player.Character.CurrentVehicle.Velocity.Length() < 10.0)
                            {
                                Limit = true;
                            }
                            
                        }
                        else
                        {
                            //車に乗っていない
                            if (Player.Character.Velocity.Length() < 7.0)
                            {
                                Limit = true;
                            }
                        }
                        if (Limit && A.Position.DistanceTo2D(playpos) < 15.0f)
                        {
                      //      Game.DisplayText("[MeteoStorm] Boming-Poin is changed.", 4000);
                            A.Position = new Vector3(playpos.X + rnd.Next(20, 50), playpos.Y + rnd.Next(20, 50), 50);
                        }
                        POS = A.Position;
                        POS.Z = playpos.Z;
                        A.Visible = false;
                        target = GTA.Blip.AddBlip(POS);

                        if (Player.Character.Position.DistanceTo2D(POS) < 15.0f) { GTA.Native.Function.Call("TRIGGER_PTFX", "qub_lg_explode_orange", POS.X, POS.Y, POS.Z, 0, 0, 0, 2.5f); }

                        if (Exists(target))
                        {
                            target.Color = BlipColor.DarkTurquoise;
                            GTA.Native.Function.Call("CHANGE_BLIP_PRIORITY",target,2);
                        }
                        if (Exists(A))
                        {
                            Vector3 Apos = A.Position;

                            A.Weapons.FromType(Weapon.Heavy_RocketLauncher).Ammo = 999;
                            A.Weapons.Select(Weapon.Heavy_RocketLauncher);


                            GTA.Native.Function.Call("FIRE_PED_WEAPON", A, Apos.X, Apos.Y, Apos.Z - 50.0f);

                        }

                    }
                    else
                    {
                        if (Exists(target)) { target.Delete(); }
                    }

            }
            else
            {
                if (Exists(A)) { A.Delete(); }
                if (Exists(target)) { target.Delete(); }
            }
            

       }
        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);
            inputCheckerAll.AddInputKey(e.Key);
            if (inputCheckerAll.Check(0) == true)
            {
                AllF = true;
            }
            if (inputCheckerBomb.Check(0) == true)
            {
                if (AllF)
                {
                    Game.DisplayText("MeteoStorm OFF", 4000);
                    AllF = false;
                    if (Exists(A)) { A.Delete(); }

                        if (Exists(target)) { target.Delete(); }
                      //  if (Exists(lig.Enabled)) { lig.Disable(); }
                    //    lig.Enabled = false;

                    
                }
                else
                {
                    Game.DisplayText("MeteoStorm ON", 4000);
                    AllF = true;
                }
            }
        }
    }
}
