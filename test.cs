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

    public class test : Script
    {

        InputChecker inputCheckerBomb = new InputChecker();
        bool AF;
        int N;
        public test()
        {
            N = 0;
            AF = false;
            Interval = 100;
            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.T, Keys.E, Keys.S, Keys.T});

            this.Tick += new EventHandler(this.Bombat_Tick);
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }

        private void Req(Model m)
        {
            GTA.Native.Function.Call("REQUEST_MODEL", m);
            while(!GTA.Native.Function.Call<bool>("HAS_MODEL_LOADED", m)){
                Wait(0);
            }
            return;

        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (AF == true)
            {
                /*
                Ped[] ped = World.GetPeds(Player.Character.Position, 50.0f);
                if (Exists(ped[1]))
                {
                    Req(ped[1].Skin.Model);
                    GTA.Native.Function.Call("CHANGE_PLAYER_MODEL",Player,ped[1].Skin.Model);
                }
                */
                AF = false;
                Ped player = Player.Character;
                switch (N)
                {
                    case 0:
                        try
                        {
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_01, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_02, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_03, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_04, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_05, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_06, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_07, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_08, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_09, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_10 , 3000);
   

                        }
                        catch
                        {
                            Game.DisplayText("Mistake !", 4000);
                        }
                        break;
                    case 1:
                        try
                        {
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_11, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_12, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_13, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_14, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_15, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_16, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_17, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_18, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_19, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_20, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_21, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_22, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_23, 3000);
                            GTA.Pickup.CreateWeaponPickup(player.Position.Around(1.0f), Weapon.Episodic_24, 3000);


                        }
                        catch
                        {
                            Game.DisplayText("Mistake !", 4000);
                        }
                        break;
                }
                N = (N + 1) % 2;
            }

       }
        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);

            if (inputCheckerBomb.Check(0) == true)
            {

                 Game.DisplayText("Test Active", 4000);
                 AF = true;

            }
        }
    }
}
