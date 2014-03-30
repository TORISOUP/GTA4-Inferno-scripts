using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Grab : Script
    {
        Vehicle car;
        public Grab()
        {
            Interval = 100;

        }
        private void Bakurai_Tick(object sender, EventArgs e)
        {
            if(Game.isGameKeyPressed(GameKey.Attack) && !Player.Character.isInVehicle()){
                car = GTA.World.GetClosestVehicle(Player.Character.Position, 10.0f);
                if (Exists(car))
                {
                    
                }
            }
            
       }

    }
}
