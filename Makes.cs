using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Jump : Script
    {
        InputChecker inputCheckerMOD = new InputChecker();

        Random rnd;

        public Jump()
        {

            rnd = new Random();
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.A, Keys.L, Keys.L, Keys.O, Keys.N });
            Interval = 80;
            this.Tick += new EventHandler(this.MOD_Tick);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
        }

        private void MOD_Tick(object sender, EventArgs e)
        {
            if (!Player.Character.isInVehicle())
            {
                if (Game.isGameKeyPressed(GameKey.Jump) && Game.isGameKeyPressed(GameKey.Sprint))
                {

                    Player.Character.ApplyForce(new Vector3(0, 0, 0.9f));

                }
                
            }
        }


        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
 
        }
    }
}
