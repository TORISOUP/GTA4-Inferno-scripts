using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Drawing;
namespace TestScriptCS.Scripts
{

    public class Invicible : Script
    {
        InputChecker inputCheckerMOD2 = new InputChecker();

        public Invicible()
        {

            inputCheckerMOD2.AddCheckKeys(new Keys[] { Keys.C, Keys.M, Keys.U, Keys.T });
            inputCheckerMOD2.AddCheckKeys(new Keys[] { Keys.P, Keys.U, Keys.K, Keys.Y });

          //  KeyDown += new GTA.KeyEventHandler(Meteoat_KeyDown);
 
        }

        void Meteoat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerMOD2.AddInputKey(e.Key);
            if (inputCheckerMOD2.Check(0) == true)
            {
                
                Player.Character.Invincible = true;
            }


        }

    }
}
