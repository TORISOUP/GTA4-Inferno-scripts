using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class CoD : Script
    {
 
        Ped player;
        RectangleF rect;
        GTA.Font screenFont;
        string COD;
        bool GetFlag;

        public CoD()
        {
            rect = new RectangleF(0.1F, 0.7F, 0.8F, 0.1F);
            screenFont = new GTA.Font(0.07F, FontScaling.ScreenUnits);
            screenFont.Color = Color.White;
            GetFlag = false;
            player = Player.Character;
            Interval = 100;
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (player.isAlive)
            {
                GetFlag = false;
            }
            else
            {
                if (GetFlag == false)
                {
                    COD = GetCOD();
                    GetFlag = true;

                }
            }

       }
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            if (GetFlag)
            {
                try
                {
                    e.Graphics.Scaling = FontScaling.ScreenUnits; // size on screen will always be the same, regardless of resolution

                    e.Graphics.DrawText(COD, rect, TextAlignment.Center | TextAlignment.VerticalCenter, screenFont);
                }
                catch
                {
                    return;
                }
            }


        }

        private string GetCOD()
        {
            string S="OTHER";

            if (player.HasBeenDamagedBy(Weapon.Melee_PoolCue)) { S = "POOL CUE"; }
            if (player.HasBeenDamagedBy(Weapon.Misc_AnyMelee)) { S = "ANY MELEE"; }
            if (player.HasBeenDamagedBy(Weapon.Misc_RammedByCar)) { S = "TRAFFIC ACCIDENT (RAMMED)"; }
            if (player.HasBeenDamagedBy(Weapon.Misc_Fall)) { S = "FALL"; }
            if (player.HasBeenDamagedBy(Weapon.Misc_AnyWeapon)) { S = "ANY WEAPON"; }
            if (player.HasBeenDamagedBy(Weapon.Misc_RunOverByCar)) { S = "TRAFFIC ACCIDENT (CRUSH)"; } 
            if (player.HasBeenDamagedBy(Weapon.Handgun_DesertEagle)){S="D.E.";}
            if (player.HasBeenDamagedBy(Weapon.Handgun_Glock)) { S = "GLOCK"; }
            if (player.HasBeenDamagedBy(Weapon.Heavy_FlameThrower)) { S = "FLAME THROWER"; }
            if (player.HasBeenDamagedBy(Weapon.Heavy_Minigun)) { S = "MINIGUN"; }
            if (player.HasBeenDamagedBy(Weapon.Heavy_RocketLauncher)) { S = "RPG-7"; }
            if (player.HasBeenDamagedBy(Weapon.Misc_Rocket)) { S = "RPG-7"; }
            if (player.HasBeenDamagedBy(Weapon.Melee_BaseballBat)) { S = "BASEBALLBAT"; }
            if (player.HasBeenDamagedBy(Weapon.Melee_Knife)) { S = "KNIFE"; }
            if (player.HasBeenDamagedBy(Weapon.Rifle_AK47)) { S = "AK47"; }
            if (player.HasBeenDamagedBy(Weapon.Rifle_M4)) { S = "M4"; }
            if (player.HasBeenDamagedBy(Weapon.Shotgun_Baretta)) { S = "SHOTGUN BARETTA"; }
            if (player.HasBeenDamagedBy(Weapon.Shotgun_Basic)) { S = "SHOTGUN BASIC"; }
            if (player.HasBeenDamagedBy(Weapon.SMG_MP5)) { S = "MP5"; }
            if (player.HasBeenDamagedBy(Weapon.SMG_Uzi)) { S = "UZI"; }
            if (player.HasBeenDamagedBy(Weapon.SniperRifle_Basic)) { S = "PSG-1"; }
            if (player.HasBeenDamagedBy(Weapon.SniperRifle_M40A1)) { S = "M40A1"; }
            if (player.HasBeenDamagedBy(Weapon.Thrown_Grenade)) { S = "GRENADE"; }
            if (player.HasBeenDamagedBy(Weapon.Thrown_Molotov)) { S = "MOLOTOV"; }
            if (player.HasBeenDamagedBy(Weapon.Unarmed)) { S = "UNARMED"; }
            if (player.HasBeenDamagedBy(Weapon.Misc_Explosion)) { S = "EXPLOSION"; }
            if (player.HasBeenDamagedBy(Weapon.Episodic_18)) { S = "ADVANCED RPG-7"; }
            if (player.HasBeenDamagedBy(Weapon.Episodic_19)) { S = "ADVANCED RPG-7"; }

            if (player.HasBeenDamagedBy(player)) { S = S + "(SUICIDE)"; }

            return S;

        }

    }
}
