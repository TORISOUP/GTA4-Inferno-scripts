using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using System.Collections.Generic;
namespace TestScriptCS.Scripts
{
   
    public class NAKAJIMA : Script
    {

        bool IsonoActive;
        System.Diagnostics.Stopwatch stopWatch;
        

        static event EventHandler IsonoStartHandler;

        public static void NAKAJIMAStart()
        {
            IsonoStartHandler(new object(), new EventArgs());
        }




        public NAKAJIMA()
        {
            IsonoStartHandler += ISONO_IsonoStartHandler;

            Interval = 100;
            this.Tick += new EventHandler(this.ISONO_Tick);
            stopWatch = new System.Diagnostics.Stopwatch();


        }



        void ISONO_IsonoStartHandler(object sender, EventArgs e)
        {
            if (!IsonoActive)
            {
                stopWatch.Restart();
                IsonoActive = true;
            }
        }



        private void ISONO_Tick(object sender, EventArgs e)
        {
            if (!IsonoActive) { return; }
            if (stopWatch.ElapsedMilliseconds / 1000.0f > 20 ) {

                Player.Character.Invincible = false;
                IsonoActive = false; return; 
            }

            Player.Character.Invincible = true;
            var AO = World.GetAllObjects();
            int i = 0;
            foreach (var obj in AO)
            {
                if (i > 15) { break; }
                if (!Exists(obj)) { continue; }
                var leng = (obj.Position - Player.Character.Position).Length();
                if (leng> 15.0f && leng<27.0f)
                {
                    i++;
                    obj.ApplyForce(30 * (Player.Character.Position - obj.Position));
                }
                
            }
        }


    }
}
