using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class YokoNitro : Script
    {
        bool OKFlag = true;
        public YokoNitro()
        {
    
            Interval = 100;
            this.Tick += new EventHandler(this.Bombat_Tick);

        }


        private void Bombat_Tick(object sender, EventArgs e)
        {


            GTA.Native.Pointer LX = typeof(int);
            GTA.Native.Pointer LY = typeof(int);
            GTA.Native.Pointer RX = typeof(int);
            GTA.Native.Pointer RY = typeof(int);
            GTA.Native.Function.Call("GET_POSITION_OF_ANALOGUE_STICKS", 0, LX, LY, RX, RY);

            if (Player.Character.isInVehicle())
            {
                Vehicle pV = Player.Character.CurrentVehicle;



                if (GetGamePad() && OKFlag)
                {
                    if (LX > 100)
                    {
                        if (Share.AddPoint(-1))
                        {
                            pV.ApplyForceRelative(new Vector3(10, 0, -2));
                            GTA.Native.Function.Call("ADD_EXPLOSION", pV.Position.X, pV.Position.Y, pV.Position.Z, 3, 0.0f, 30, 0, 0.1f);
                            OKFlag = false;
                        }
                    }
                    else if (LX < -100)
                    {
                        if (Share.AddPoint(-1))
                        {
                            pV.ApplyForceRelative(new Vector3(-10, 0, -2));

                            GTA.Native.Function.Call("ADD_EXPLOSION", pV.Position.X, pV.Position.Y, pV.Position.Z, 3, 0.0f, 30, 0, 0.1f);

                            OKFlag = false;
                        }
                    }

                }
                if (Math.Abs(LX) < 50)
                {
                    OKFlag = true;
                }



            }
               
        }
     
        bool GetGamePad()
        {

           if (Game.isGameKeyPressed(GameKey.Jump) && Game.isGameKeyPressed(GameKey.Attack))
            {

                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
