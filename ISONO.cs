using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Collections.Generic;
namespace TestScriptCS.Scripts
{
   
    public class ISONO : Script
    {

        bool IsonoActive, InitFlag;
        Ped[] allped;
        Vehicle[] aV;

        

        static event EventHandler IsonoStartHandler;

        public static void IsonoStart()
        {
            IsonoStartHandler(new object(), new EventArgs());
        }




        public ISONO()
        {
            IsonoStartHandler += ISONO_IsonoStartHandler;

            Interval = 100;
            this.Tick += new EventHandler(this.ISONO_Tick);

            KeyDown += new GTA.KeyEventHandler(MOD_KeyDown);

        }

        void MOD_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            if (e.Key == Keys.I)
            {
                IsonoActive = false;
                Player.Character.Invincible = false;
                Player.CanControlRagdoll = false;
                Player.Character.isRagdoll = false;
            }
        }

        void ISONO_IsonoStartHandler(object sender, EventArgs e)
        {

            InitFlag = true;
            IsonoActive = true;
        }



        private void ISONO_Tick(object sender, EventArgs e)
        {
            if (!IsonoActive) { return; }
            if (InitFlag)
            {
                allped = GTA.World.GetPeds(Player.Character.Position, 200.0f);
                for (int i = 0; i < allped.Length; i++)
                {

                    allped[i].Velocity = new Vector3(0, 0, 200);

                }

                aV = GTA.World.GetVehicles(Player.Character.Position, 200.0f);
                for (int i = 0; i < aV.Length; i++)
                {

                    aV[i].Velocity = new Vector3(0, 0, 150);
                }
                InitFlag = false;
            }

            if (Player.Character.Position.Z < 150 && Player.Character.Velocity.Z > 0 && !Player.Character.isRagdoll)
            {
                Player.Character.Velocity = new Vector3(0, 0, 200);
            }
            else if (Player.Character.Position.Z > 150)
            {
                Player.Character.Invincible = true;
                Player.CanControlRagdoll = true;
                Player.Character.isRagdoll = true;
            }
            else if (Player.Character.Position.Z < 40.0f && Player.Character.Velocity.Length() < 1.0f)
            {
                Player.Character.Invincible = false;
                Player.CanControlRagdoll = false;
                Player.Character.isRagdoll = false;
                IsonoActive = false;
            }





        }


    }
}
