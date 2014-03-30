using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{


    public class RomanMode : Script
    {


        InputChecker inputCheckerBomb = new InputChecker();
        public RomanMode()
        {

            Interval = 100;


            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.R, Keys.O, Keys.M, Keys.A, Keys.N });
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }

        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);

            if (inputCheckerBomb.Check(0) == true)
            {
                Player.Model = new Model(0x89395FC9);


            }
        }
  
    }
}
