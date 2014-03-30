using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Lights : Script
    {
        bool Flag = false;
        Light light;
        InputChecker inputCheckerMOD = new InputChecker();
        public Lights()
        {
            light = new Light(Color.White, 30.0f, 10.0f, Player.Character.Position + new Vector3(0, 0, 5.0f));
            light.Enabled = false;
            inputCheckerMOD.AddCheckKeys(new Keys[] { Keys.L });
            Interval = 30;
            this.Tick += new EventHandler(this.Bombat_Tick);
            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);
        }


        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (Flag)
            {

                light.Enabled = true;
                light.Position = Player.Character.Position + new Vector3(0, 0, 5.0f);
                

            }
            else
            {
                light.Enabled = false;
            }
               
        }
        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD.AddInputKey(e.Key);
            if (inputCheckerMOD.Check(0))
            {
                if (Flag) { Flag = false; } else { Flag = true; }
            }

        }

    }
}
