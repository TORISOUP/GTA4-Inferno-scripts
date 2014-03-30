using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Aleret : Script
    {
        System.IO.Stream strm;
        System.Media.SoundPlayer wmp = null;
        bool OnPlay = false;
        bool OnAleret = false;
        public Aleret()
        {

            strm = TestScriptCS.Properties.Resources.piri;
            wmp = new System.Media.SoundPlayer(strm);
            Interval = 500;
            this.Tick += new EventHandler(this.Bombat_Tick);

        }
        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (Player.Character.Health < 25 && Player.CanControlCharacter && Player.Character.isAlive && Share.ScriptError==false)
            {
                if (!OnPlay)
                {
                    wmp.PlayLooping();
                    OnPlay = true;
                }
            }
            else
            {
                if (!Share.ScriptError)
                {
                    OnPlay = false;
                    wmp.Stop();
                }
            }

            if (Share.ScriptError)
            {
                if (OnAleret == false)
                {
                 //   wmp.Stop();
                  //  strm = TestScriptCS.Properties.Resources.Reload;
                //    wmp = new System.Media.SoundPlayer(strm);
                 //   wmp.PlayLooping();
                    OnAleret = true;
                }
            }

       }



    }
}
