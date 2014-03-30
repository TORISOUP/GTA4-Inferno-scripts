using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Cam : Script
    {
 

        public Cam()
        {
            Interval = 100;

        }
        private void Bombat_Tick(object sender, EventArgs e)
        {



        
       }
        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {

            if (e.Key == Keys.F12)
            {
                if (GTA.Game.CurrentCamera.isActive)
                {
                    GTA.Game.CurrentCamera.Deactivate();
                    Game.DisplayText("Camera disable.", 4000);
                }
                else
                {
                    Game.DisplayText("Camera enable.", 4000);
                    GTA.Game.CurrentCamera.Activate();
                }
            }
        }
    }
}
