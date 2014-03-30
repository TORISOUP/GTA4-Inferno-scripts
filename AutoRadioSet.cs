using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    //ラジオをユーザートラックに固定するMOD
    public class RadioSet : Script
    {

        bool RideOn;
        public RadioSet()
        {
            RideOn =false;
            Interval = 1000;
            this.Tick += new EventHandler(this.Radio_Tick);

        }

        private void Radio_Tick(object sender, EventArgs e)
        {
            if (Player.Character.isInVehicle() && !RideOn)
            {
           
                switch(Game.CurrentEpisode)
                {
                    case GTA.GameEpisode.GTAIV:
                        //本編ならユーザートラックに固定
                        GTA.Native.Function.Call("RETUNE_RADIO_TO_STATION_INDEX", 18);
                        break;
                    
                       
                    case GameEpisode.TLAD:
                    case GameEpisode.TBOGT:
                        GTA.Native.Function.Call("RETUNE_RADIO_TO_STATION_INDEX", 12);
                        break;
                }
            }

            RideOn = Player.Character.isInVehicle();
        }

  
    }
}
