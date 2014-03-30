using System;
using System.Windows.Forms;
using GTA;
using System.Drawing;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //確かボルガか何かで使ってた気がする…
    public class Futtobi: Script
    {
        GTA.Font screenFont;
        bool AF = false;
        Vector3 StartPoint;
        bool Init = false;
        public Futtobi()
        {
            GUID = new Guid("CF7EB590-3851-11E0-88D2-1396DFD72085");
            BindScriptCommand("Active", new ScriptCommandDelegate(Active));

            screenFont = new GTA.Font(0.05F, FontScaling.ScreenUnits);
            Interval = 500;
            this.Tick += new EventHandler(this.MOD_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.MOD_PerFrameDrawing);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
        }
        private void Active(GTA.Script sender, GTA.ObjectCollection Parameter)
        {
            AF = true;
            Init = true;
        }
        Vector3 GetUpperForce(float x)
        {
            Vector3 tar;
            if (x < 0.2)
            {
                tar = new Vector3(0, 0, 100);
            }
            else
            {
                tar = new Vector3(0, 0, 300.0f * (1 - 4 * (x - 0.5f) * (x - 0.5f)) - Player.Character.Position.Z);
            }

            return tar;
        }

        private void MOD_Tick(object sender, EventArgs e)
        {
            if (AF)
            {
                if (Player.Character.isDead)
                {

                        AF = false;
                        Player.CanControlRagdoll = false;
                        Player.Character.isRagdoll = false;
                        Player.Character.Invincible = false;
                        return;
                    
                }
                if(! Exists(Game.GetWaypoint())){
                    AF = false;
                    Player.CanControlRagdoll = false;
                    Player.Character.isRagdoll = false;
                    Player.Character.Invincible = false;
                    return;
                }
                if (Init)
                {
                    StartPoint = Player.Character.Position;
                    Init = false;
                }

                
                Vector3 Tar = Game.GetWaypoint().Position;
                Vector3 delta = Tar - Player.Character.Position;
                if (delta.Length() < 60.0f) { 
                    AF = false;
                    Player.CanControlRagdoll = false;
                    Player.Character.isRagdoll = false;
                    Player.Character.Invincible = false;
                    return;
                }

                Vector3 X1 = (StartPoint-Player.Character.Position);
                Vector3 X2 = (StartPoint-Game.GetWaypoint().Position);

                Player.Character.ApplyForce(delta * 30 + 50*GetUpperForce(X1.Length()/X2.Length() ));

                World.AddExplosion(Player.Character.Position, ExplosionType.Molotov, 0.0f, true, false, 0.0f);
                Player.CanControlRagdoll = true;
                Player.Character.isRagdoll = true;
                Player.Character.Invincible = true;
                
            }

        }

        unsafe private void MOD_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {


        }

        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            if (e.Key == Keys.U)
            {
                if (AF) { 
                    AF = false;
                    Player.CanControlRagdoll = false;
                    Player.Character.isRagdoll = false;
                    Player.Character.Invincible = false;
                    return;
                }

            }

        }
    }
}
