using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
 
    public class Lights : Script
    {
        Light l;
        int times;
        int Mtimes;
        Vector3 Pos;
        float msize;
        float mi;
        public Lights(System.Drawing.Color color,float lsize, float intensity,Vector3 position,int ms)
        {
            l = new Light(color, 0, 0, position);
            l.Enabled = true;
            Pos = position;
            mi = intensity;
            msize = lsize;
            Interval = 1;
            Mtimes = ms;
            times = 0;
            this.Tick += new EventHandler(this.Nikita_Tick);

        }

        private void Nikita_Tick(object sender, EventArgs e)
        {
            times++;
            if (times >= Mtimes)
            {
                l.Disable();
                Dispose();
            }

            if (times < Mtimes / 2)
            {
                l.Range = msize * (times / (Mtimes / 2));
                l.Intensity = mi * (times / (Mtimes / 2));
            }
            else
            {
                l.Range = msize ;
                l.Intensity = mi;
            }
        }

  
    }
}
