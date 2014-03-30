using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class chaost : Script
    {
        InputChecker inputCheckerMOD = new InputChecker();
        bool ActiveFlag;
        Random rnd;

        public chaost()
        {
            rnd = new Random();
            Interval = 10000;
            this.Tick += new EventHandler(this.MOD_Tick);

        }

        private void MOD_Tick(object sender, EventArgs e)
        {
            Ped[] ped = World.GetPeds(Player.Character.Position, 100.0f);

            for (int i = 0; i < ped.Length; i++)
            {
                if (ped[i] == Player.Character || !Exists(ped[i]) ) { continue; }



                switch (rnd.Next(6))
                {
                    case 0:
                        ped[i].Weapons.MP5.Ammo = 30000;
                        ped[i].Weapons.Select(Weapon.SMG_MP5);
                        break;
                    case 1:
                        ped[i].Weapons.AssaultRifle_M4.Ammo = 30000;
                        ped[i].Weapons.Select(Weapon.Rifle_M4);
                        break;
                    case 2:
                        ped[i].Weapons.RocketLauncher.Ammo = 30000;
                        ped[i].Weapons.Select(Weapon.Heavy_RocketLauncher);
                        break;
                    case 3:
                        ped[i].Weapons.MolotovCocktails.Ammo = 30000;
                        ped[i].Weapons.Select(Weapon.Thrown_Molotov);
                        break;
                    case 4:
                        ped[i].Weapons.DesertEagle.Ammo = 30000;
                        ped[i].Weapons.Select(Weapon.Handgun_DesertEagle);
                        break;
                    case 5:
                        ped[i].Weapons.RocketLauncher.Ammo = 30000;
                        ped[i].Weapons.Select(Weapon.Heavy_RocketLauncher);
                        break;

                    default:

                        ped[i].Weapons.BaseballBat.Ammo = 2000;
                        ped[i].Weapons.Select(Weapon.Melee_BaseballBat);
                        break;


                }
                ped[i].RelationshipGroup = RelationshipGroup.Criminal;
                ped[i].WillDoDrivebys = true;
                ped[i].WillUseCarsInCombat = true;
                ped[i].StartKillingSpree(true);

            }

        }


        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD.AddInputKey(e.Key);

            if (inputCheckerMOD.Check(0) == true || inputCheckerMOD.Check(1) == true)
            {
                if (ActiveFlag)
                {
                    ActiveFlag = false;
                    Game.DisplayText("TamiPato OFF", 4000);
                }
                else
                {
                    ActiveFlag = true;
                    Game.DisplayText("TamiPato ON", 4000);
                }
            }


        }
    }
}
