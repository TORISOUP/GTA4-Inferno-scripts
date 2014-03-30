#region using
using System;
using System.IO;
using System.Windows.Forms;
using GTA;
using System.Drawing;
using _InputChecker;
#endregion

public class OTO : Script
{

    System.Media.SoundPlayer soundPlayer;

    public OTO()
    {
        soundPlayer = new System.Media.SoundPlayer(@"scripts\wave\DQ.wav");
        Interval = 5000;
        Tick += new EventHandler(Puchun_Tick);

    }

    void Puchun_Tick(object sender, EventArgs e)
    {

        soundPlayer.PlaySync();
        
    }

}