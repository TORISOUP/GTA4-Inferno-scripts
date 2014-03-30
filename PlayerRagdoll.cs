using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //やるきのないMOD
    public class PlayerRagdoll : Script
    {
        bool RagdollFlag = false;
        Random rnd;
        bool Rt;
        InputChecker inputChecker= new InputChecker();
        public PlayerRagdoll()
        {
            Rt = false;
            rnd = new Random();
            Interval = 100;
            this.Tick += new EventHandler(this.Nikita_Tick);
            inputChecker.AddCheckKeys(new Keys[] { Keys.K, Keys.I, Keys.L, Keys.L, Keys.M,Keys.E });
            KeyDown += new GTA.KeyEventHandler(key_KeyDown);
        }

        private void Nikita_Tick(object sender, EventArgs e)
        {
            if (Game.isGameKeyPressed(GameKey.Crouch) && Game.isGameKeyPressed(GameKey.Jump))
            {
                RagdollFlag = true;
                Player.CanControlRagdoll = true;
                Player.Character.isRagdoll = true;
                Player.CanControlCharacter = true;

            }
            else
            {
                if (RagdollFlag)
                {
                    RagdollFlag = false;
                    Player.CanControlRagdoll = false;
                    Player.Character.isRagdoll = false;
                }
            }

            if (!Player.Character.isInVehicle() && Game.isGameKeyPressed(GameKey.EnterCar))
            {
                Rt = false;
            }

            if (Rt)
            {
                Vehicle pV = Player.Character.CurrentVehicle;
                if (Exists(pV))
                {
                    pV.Rotation = new Vector3(rnd.Next(0, 360) - 180, rnd.Next(0, 360) - 180, rnd.Next(0, 360) - 180);
                }
            }

     
        }
        void key_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            if (e.Key == Keys.NumPad2)
            {
                GTA.Native.Function.Call("DISPLAY_HUD", true);
                Player.CanControlCharacter = true;
            }

            inputChecker.AddInputKey(e.Key);
            if (inputChecker.Check(0) == true)
            {
                Player.Character.Health = -1;
            }

            if (e.Key == Keys.NumPad3 && Player.Character.isInVehicle())
            {
                Rt = true;

            }
            else
            {
                Rt = false;
            }
            if (e.Key == Keys.NumPad1)
            {
                Rt = false;
            }
        }
    }
}
                        