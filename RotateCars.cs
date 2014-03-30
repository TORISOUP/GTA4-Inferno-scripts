using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    public class RC : Script
    {
        bool AF;
        Random rnd;
        public RC()
        {
            rnd = new Random();
            AF = false;
            Interval = 1;
            GUID = new Guid("C2CFA980-28A7-11E0-B4A2-BD74DFD72085");
            BindScriptCommand("Activate", new ScriptCommandDelegate(Activate));
            this.Tick += new EventHandler(this.MOD_Tick);
        }

        private void Activate(GTA.Script sender, GTA.ObjectCollection Parameter)
        {
            AF = Parameter.Convert<bool>(0);
        }

        private void MOD_Tick(object sender, EventArgs e)
        {
       

            if (!AF) { return; }
            Vehicle[] AV = Cacher.GetVehicles(Player.Character.Position, 50.0f);

            for (int i = 0; i < AV.Length; i++)
            {
                if (!Exists(AV[i])) { continue; }
                if (!Exists(AV[i].GetPedOnSeat(VehicleSeat.Driver)) || Player.Character.CurrentVehicle == AV[i]) { continue; }

                if (i % 5 == 0)
                {
                    AV[i].Rotation = new Vector3(rnd.Next(0, 360) - 180, rnd.Next(0, 360) - 180, rnd.Next(0, 360) - 180);
                }
                else if (i % 3 == 0)
                {
                    AV[i].Rotation = AV[i].Rotation + new Vector3(0, 0, 33.9f);

                }
                else if (i % 3 == 1)
                {
                    AV[i].Rotation = AV[i].Rotation + new Vector3(33.9f, 0, 0);
                }
                else
                {
                    AV[i].Rotation = AV[i].Rotation + new Vector3(0, 33.9f, 0);
                }

            }
 
        }


    }
}
