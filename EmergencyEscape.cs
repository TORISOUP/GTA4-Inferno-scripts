using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class EmergencyEscape : Script
    {

        public EmergencyEscape()
        {

            Interval = 100;
            this.Tick += new EventHandler(this.Radio_Tick);

        }

        private void Radio_Tick(object sender, EventArgs e)
        {
            if (Player.Character.isInVehicle())
            {

            }
            else
            {
                if (Game.isGameKeyPressed(GameKey.Jump))
                {
                    Player.Character.GravityMultiplier = 0.1f;
                }
                else
                {
                    Player.Character.GravityMultiplier = 1.0f;
                }
            }

        }

  
    }
}
