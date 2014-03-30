using System;
using System.Windows.Forms;
using GTA;
using _InputChecker;
using FNF.Utility;
using System.Threading;

namespace TestScriptCS.Scripts
{


    public class Boyomi : Script
    {


        InputChecker inputCheckerBomb = new InputChecker();
        public Boyomi()
        {

            Interval = 100;


            inputCheckerBomb.AddCheckKeys(new Keys[] { Keys.M,Keys.O });
            KeyDown += new GTA.KeyEventHandler(Bombat_KeyDown);
        }

        void Bombat_KeyDown(object sender, GTA.KeyEventArgs e)
        {
            inputCheckerBomb.AddInputKey(e.Key);

            if (inputCheckerBomb.Check(0) == true)
            {
                Thread thread = new Thread(new ThreadStart(StopBoyomi));
                thread.Start();
            }
        }


        void StopBoyomi()
        {
            try
            {
                var BouyomiChan = new BouyomiChanClient();
                int taskId = BouyomiChan.AddTalkTask2("むおん");
                BouyomiChan.Dispose();
            }
            catch
            {
            }
        }
  
    }
}
