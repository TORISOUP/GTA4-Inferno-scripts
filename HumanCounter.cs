using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Xml;
using GTA;
using _InputChecker;
using _ItemListForm;

namespace TestScriptCS.Scripts
{

    public class DeadCounter
    {
        HashSet<Ped> peds = new HashSet<Ped>();
        HashSet<Vehicle> vehs = new HashSet<Vehicle>();
        public int AmountPed = 0;
        public int AmoutCars = 0;
        public DeadCounter()
        {
            ;
        }

        public bool AddPed(Ped p)
        {
            if (peds.Add(p))
            {
                AmountPed++;
                return true;
            }
            return false;
        }

        public void AddVehicle(Vehicle v)
        {
            if (vehs.Add(v))
            {
                AmoutCars++;
            }
        }

        public void Reset()
        {
            peds.Clear();
            vehs.Clear();
        }

    }

    public class DeadPedList
    {
        public string[] str;
        public int[] Timers;
        public bool[] KilledByPlayer;
        public bool[] IsMissionChar;
        public bool isEnable = false;
        int index;
        public int max;
        Player player;
        public DeadPedList(int max,Player p)
        {
            player = p;
            this.max = max;
            str = new string[max];
            Timers = new int[max];
            KilledByPlayer = new bool[max];
            IsMissionChar = new bool[max];
            index = 0;
        }

        public void AddPed(Ped p)
        {

            Timers[index] = 120;
            str[index] = ((Share.eModel)p.Model.Hash).ToString() + "( " + GetCOD(p) + " )";
            KilledByPlayer[index] = p.HasBeenDamagedBy(player.Character);
            if (KilledByPlayer[index])
            {

                Share.AddPoint(1);
            }
            IsMissionChar[index] = p.isRequiredForMission;
            index = (index + 1) % max;
            return;


        }

        public void Check(){
            for (int i = 0; i < max; i++)
            {
                if (Timers[i] > 0)
                {
                    isEnable = true;
                    return;
                }
            }
            isEnable = false;
            return;
        }

        private string GetCOD(Ped ped)
        {
            string S = "OTHER";

            if (ped.HasBeenDamagedBy(Weapon.Melee_PoolCue)) { S = "POOL CUE"; }
            if (ped.HasBeenDamagedBy(Weapon.Misc_AnyMelee)) { S = "ANY MELEE"; }
            if (ped.HasBeenDamagedBy(Weapon.Misc_RammedByCar)) { S = "TRAFFIC ACCIDENT"; }
            if (ped.HasBeenDamagedBy(Weapon.Misc_Fall)) { S = "FALL"; }
            if (ped.HasBeenDamagedBy(Weapon.Misc_AnyWeapon)) { S = "ANY WEAPON"; }
            if (ped.HasBeenDamagedBy(Weapon.Misc_RunOverByCar)) { S = "TRAFFIC ACCIDENT"; }
            if (ped.HasBeenDamagedBy(Weapon.Handgun_DesertEagle)) { S = "D.E."; }
            if (ped.HasBeenDamagedBy(Weapon.Handgun_Glock)) { S = "GLOCK"; }
            if (ped.HasBeenDamagedBy(Weapon.Heavy_FlameThrower)) { S = "FLAME THROWER"; }
            if (ped.HasBeenDamagedBy(Weapon.Heavy_Minigun)) { S = "MINIGUN"; }
            if (ped.HasBeenDamagedBy(Weapon.Heavy_RocketLauncher)) { S = "RPG-7"; }
            if (ped.HasBeenDamagedBy(Weapon.Misc_Rocket)) { S = "RPG-7"; }
            if (ped.HasBeenDamagedBy(Weapon.Melee_BaseballBat)) { S = "BASEBALLBAT"; }
            if (ped.HasBeenDamagedBy(Weapon.Melee_Knife)) { S = "KNIFE"; }
            if (ped.HasBeenDamagedBy(Weapon.Rifle_AK47)) { S = "AK47"; }
            if (ped.HasBeenDamagedBy(Weapon.Rifle_M4)) { S = "M4"; }
            if (ped.HasBeenDamagedBy(Weapon.Shotgun_Baretta)) { S = "SHOTGUN BARETTA"; }
            if (ped.HasBeenDamagedBy(Weapon.Shotgun_Basic)) { S = "SHOTGUN BASIC"; }
            if (ped.HasBeenDamagedBy(Weapon.SMG_MP5)) { S = "MP5"; }
            if (ped.HasBeenDamagedBy(Weapon.SMG_Uzi)) { S = "UZI"; }
            if (ped.HasBeenDamagedBy(Weapon.SniperRifle_Basic)) { S = "PSG-1"; }
            if (ped.HasBeenDamagedBy(Weapon.SniperRifle_M40A1)) { S = "M40A1"; }
            if (ped.HasBeenDamagedBy(Weapon.Thrown_Grenade)) { S = "GRENADE"; }
            if (ped.HasBeenDamagedBy(Weapon.Thrown_Molotov)) { S = "MOLOTOV"; }
            if (ped.HasBeenDamagedBy(Weapon.Unarmed)) { S = "UNARMED"; }
            if (ped.HasBeenDamagedBy(Weapon.Misc_Explosion)) { S = "EXPLOSION"; }
            if (ped.HasBeenDamagedBy(Weapon.Episodic_18)) { S = "ADVANCED RPG-7"; }
            if (ped.HasBeenDamagedBy(Weapon.Episodic_19)) { S = "ADVANCED RPG-7"; }

            if (ped.HasBeenDamagedBy(ped)) { S = S + "/SUICIDE"; }
            return S;

        }
        public int NumberOfQueue(int num)
        {
            return (num + index) % max;
        }
        
    }

    public class HumanCounter : Script
    {

        DeadCounter DC;

        InputChecker inputChecker = new InputChecker();

        GTA.Font screenFont;
        GTA.Font miniFont ;
        GTA.Font miniBigFont;
        GTA.Font Font;
        bool PlayerDead = false;


        private DeadPedList DPL;
        public HumanCounter()
        {
            DC = new DeadCounter();

            DPL = new DeadPedList(10,Player);

            DC.Reset();
            screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
            Interval =500;
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);
            KeyDown += new GTA.KeyEventHandler(Meteoat_KeyDown);
            
            inputChecker.AddCheckKeys(new Keys[] { Keys.R, Keys.E, Keys.S, Keys.E, Keys.T });
            inputChecker.AddCheckKeys(new Keys[] { Keys.OemMinus });

            miniFont = new GTA.Font(0.03F, FontScaling.ScreenUnits, false, false);
            miniBigFont = new GTA.Font(0.03F, FontScaling.ScreenUnits, true, true);

        }



        private void Bombat_Tick(object sender, EventArgs e)
        {

            if (Player.Character.isAlive)
            {
                if (PlayerDead)
                {

                }
            }
            else
            {
                PlayerDead = true;
            }



            Ped[] AroundPed = GTA.World.GetPeds(Player.Character.Position, 1000.0f);
            foreach (Ped p in AroundPed)
            {
                if (p.isDead)
                {
                    if (DC.AddPed(p))
                    {
                        DPL.AddPed(p);
                    }


                }
            }


            var AroudnVehicles = GTA.World.GetVehicles(Player.Character.Position, 1000.0f);
            foreach (Vehicle v in AroudnVehicles)
            {
                if (!v.isAlive)
                {
                    DC.AddVehicle(v);
                }
            }



            DPL.Check();
        }
  
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            if (Player.CanControlCharacter && Player.Character.isAlive)
            {

                //   e.Graphics.DrawText(string.Format("死者:{0, 3} / 破壊台数:{1,3}  ",DC.AmountPed,DC.AmoutCars), new RectangleF(0.0f, 0.0f, 1.0f, 1.0f), TextAlignment.Center | TextAlignment.Top, screenFont);



                int j = 0;
                
                for (int i = 0; i < DPL.Timers.Length; i++)
                {
                    if (DPL.Timers[DPL.NumberOfQueue(i)] > 0)
                    {
                        
                        if (DPL.KilledByPlayer[DPL.NumberOfQueue(i)])
                        {
                            Font = miniBigFont;
                        }
                        else {
                            Font = miniFont;
                        }
                        Color cl;
                        if (DPL.IsMissionChar[DPL.NumberOfQueue(i)])
                        {
                            cl = Color.Red;
                        }
                        else
                        {
                            cl = Color.White;
                        }
                       
                        e.Graphics.DrawText(DPL.str[DPL.NumberOfQueue(i)], new RectangleF(0.6f, 0.3f + 0.03f * j, 0.4f, 0.04f), TextAlignment.Right | TextAlignment.VerticalCenter, cl ,Font);
                        DPL.Timers[DPL.NumberOfQueue(i)]--;
                        if (DPL.Timers[DPL.NumberOfQueue(i)] == 0) { DPL.Check(); }
          
                        j++;
                    }
                }


                while (j < DPL.max)
                {
                  
          //          e.Graphics.DrawText(" ", new RectangleF(0.6f, 0.3f + 0.025f * j, 0.4f, 0.04f), TextAlignment.Right | TextAlignment.VerticalCenter, miniFont);
                    j++;
                }
            }
            


            }

        void Meteoat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputChecker.AddInputKey(e.Key);


        }

    }
}
