using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //フルトン回収
    //なんかうまく動かないけどロジックが意味不明
    public class Flton : Script
    {
        int index,pedindex;
        Model[] PedModels;
        Ped[] F_Ped;
        Ped[] Goei;
        public Random rnd;
        bool OnMission;
        int ActiveTimer;
        public Flton()
        {
            ActiveTimer = 0;
            rnd = new Random();
            pedindex=0;
            index = -1;
            PedModels = new Model[30];
            F_Ped = new Ped[30];
            Goei = new Ped[30];
            Interval = 1000;

            this.Tick += new EventHandler(this.Bakurai_Tick);
            KeyDown += new GTA.KeyEventHandler(Bakurai_KeyDown);
        }

 

        private void Bakurai_Tick(object sender, EventArgs e)
        {

            if (!Player.Character.isInVehicle() && Player.Character.Weapons.Current == Weapon.Unarmed && Game.isGameKeyPressed(GameKey.Reload))
            {
                ActiveTimer = 5;

            }

            Ped[] peds = null;

            if (ActiveTimer > 0)
            {
                ActiveTimer--;
                try
                {
                    peds = Cacher.GetPeds(Player.Character.Position, 2.0f);
                }
                catch
                {
                    Share.ScriptError = true;
                }
            }

            if (peds!=null &&peds.Length >= 0)
            {
                for (int i = 0, length = peds.Length; i < length && i < 10; i++)
                {
                    if (!Exists(peds[i])) { continue; }
                    if (peds[i] == Player.Character) { continue; }
                    if (peds[i].isAlive && !peds[i].isInVehicle() && peds[i].Animation.GetCurrentAnimationTime(new AnimationSet("dam_ko"), "drown") == 0 && peds[i].HasBeenDamagedBy(Player.Character) && peds[i].HasBeenDamagedBy(Weapon.Unarmed))
                    {
                        peds[i].Money = 500;
                        peds[i].Invincible = true;
                        peds[i].Animation.Play(new AnimationSet("dam_ko"), "drown", 20.0f, AnimationFlags.Unknown01 | AnimationFlags.Unknown06);

                        F_Ped[pedindex] = peds[i];
                        pedindex = (pedindex + 1) % 30;
                    }
                }
            }

            if(!Player.Character.isAlive){
                FireG();
            }
            if(!Player.isOnMission && OnMission && !GTA.Native.Function.Call<bool>("IS_MISSION_COMPLETE_PLAYING")){
                FireG();
            }

            OnMission=Player.isOnMission;

                for (int i = 0; i < 30; i++)
                {
                    if (Exists(F_Ped[i]))
                    {
                        float animetaime = (F_Ped[i].Animation.GetCurrentAnimationTime(new AnimationSet("dam_ko"), "drown"));
                        if (animetaime > 0.5f)
                        {
                            // F_Ped[i].Velocity=new Vector3(0,0,30.0f);
                            F_Ped[i].ApplyForce(new Vector3(0, 0, 70.0f));
                        }
                        if (F_Ped[i].Position.DistanceTo(Player.Character.Position) > 10.0f)
                        {
                            if (index < 29)
                            {
                                PedModels[++index] = F_Ped[i].Model;
                            }
                            F_Ped[i].Delete();
                        }
                    }
                    if (Exists(Goei[i]))
                    {
                        if (!Player.Group.isMember(Goei[i]))
                        {
                            Goei[i].Money = 0;
                            Goei[i].isRequiredForMission = false;
                            Goei[i].RelationshipGroup = RelationshipGroup.Civillian_Male;
                            Goei[i].ChangeRelationship(RelationshipGroup.Civillian_Male, Relationship.Hate);
                            Goei[i].CantBeDamagedByRelationshipGroup(RelationshipGroup.Player, false);
                            Goei[i].NoLongerNeeded();
                            Goei[i] = null;
                        }
                    }
                }
                   
      
       }
        void Bakurai_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Insert:
                    if (index > -1)
                    {
                        if (Player.CanControlCharacter)
                        {
                            Ped p = World.CreatePed(PedModels[index], Player.Character.Position.Around(3.0F), RelationshipGroup.Player);
                            if (Exists(p))
                            {

                                if (AddToGroup(p)) { index--; }

                            }
                        }
                    }
                    break;
                case Keys.Delete:
                    FireG();
                    break;
            }
        }

        void AddG(Ped ped)
        {
            for (int i = 0; i < Goei.Length; i++)
            {
                if (!Exists(Goei[i]))
                {
                    Goei[i] = ped;
                    return;
                }
            }
        }

        void EraseG()
        {
            for (int i = 0; i < Goei.Length; i++)
            {
                if (Exists(Goei[i]))
                {
                    Goei[i].Money=0;
                    Goei[i].Weapons.RemoveAll();
                    Goei[i].Delete();
                    Goei[i] = null;
                }
            }
        }
        void FireG()
        {
            for (int i = 0; i < Goei.Length; i++)
            {
                if (Exists(Goei[i]))
                {
                    Goei[i].Money = 0;
                    Goei[i].isRequiredForMission = false;
                    Goei[i].RelationshipGroup = RelationshipGroup.Civillian_Male;
                    Goei[i].ChangeRelationship(RelationshipGroup.Civillian_Male, Relationship.Hate);
                    Goei[i].CantBeDamagedByRelationshipGroup(RelationshipGroup.Player, false);
                    Goei[i].NoLongerNeeded();
                    Player.Group.RemoveMember(Goei[i]);
                    Goei[i] = null;
                }
            }
            
        }

        private bool AddToGroup(Ped p)
        {
            if (!Exists(p)) return false; // check if the ped is valid
            Player.Group.AddMember(p);
            p.CurrentRoom = Player.Character.CurrentRoom; // required, or ped won't be visible when spawned inside a building
            p.WillDoDrivebys = true;
            p.PriorityTargetForEnemies = true;
            p.DuckWhenAimedAtByGroupMember = false;
            p.AlwaysDiesOnLowHealth = true;
            p.SetPathfinding(true, true, true);
            p.CanSwitchWeapons = true;
         //   p.isRequiredForMission = false;
            p.Money = 500;
            p.Health = 300;
            p.Invincible = false;
            p.Weapons.Uzi.Ammo = 30000;
            switch (rnd.Next(6))
            {
                case 0:
                    p.Weapons.MP5.Ammo = 30000;
                    p.Weapons.Select(Weapon.SMG_MP5);
                    break;
                case 1:
                    p.Weapons.AssaultRifle_M4.Ammo = 30000;
                    p.Weapons.Select(Weapon.Rifle_M4);
                    break;
                case 2:
                    p.Weapons.RocketLauncher.Ammo = 30000;
                    p.Weapons.Select(Weapon.Heavy_RocketLauncher);
                    break;
                case 3:
                    p.Weapons.MolotovCocktails.Ammo = 30000;
                    p.Weapons.Select(Weapon.Thrown_Molotov);
                    break;
                case 4:
                    p.Weapons.DesertEagle.Ammo = 30000;
                    p.Weapons.Select(Weapon.Handgun_DesertEagle);
                    break;
                case 5:
                    p.Weapons.RocketLauncher.Ammo = 30000;
                    p.Weapons.Select(Weapon.Heavy_RocketLauncher);
                    break;

                default:

                    p.Weapons.BaseballBat.Ammo = 2000;
                    p.Weapons.Select(Weapon.Melee_BaseballBat);
                    break;


            }
            p.RelationshipGroup = RelationshipGroup.Player;
            p.ChangeRelationship(RelationshipGroup.Player, Relationship.Companion);
            p.CantBeDamagedByRelationshipGroup(RelationshipGroup.Player, true);
            p.DuckWhenAimedAtByGroupMember = true;

            Blip B = p.AttachBlip();
            B.Color = BlipColor.Green;
            AddG(p);
            
            Game.DisplayText(index+"");

            return true;
        }
    }
}
