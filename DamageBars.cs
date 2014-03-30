using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{


    public class DamageBars : Script
    {
        Vehicle NowV;
        float cE, xE, cP, xP, cB, xB;
        float dE, dP, dB;
        int tE, tP, tB;
        public DamageBars()
        {
            NowV = null;
            Interval = 50;
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }



        private void Bombat_Tick(object sender, EventArgs e)
        {
            if (Player.Character.isInVehicle())
            {
                
                if (NowV == null || NowV != Player.Character.CurrentVehicle)
                {
                    NowV = Player.Character.CurrentVehicle;

                    xP = cP = NowV.PetrolTankHealth;
                    xE = cE = NowV.EngineHealth;
                    xB = cB = NowV.Health;
                }
                if (cP != NowV.PetrolTankHealth && NowV.PetrolTankHealth>=0.0f) { cP = NowV.PetrolTankHealth; dP = 0; }
                if (cB != NowV.Health && NowV.Health >=0.0f) { cB = NowV.Health; dB = 0; }
                if (cE != NowV.EngineHealth && NowV.EngineHealth>0.0f) { cE = NowV.EngineHealth; dE = 0; }
            }
            else
            {
                NowV = null;
            }


        }
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            if (!Player.Character.isInVehicle()) { return; }
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            int alpha=0;
            int max = 40;
            int dTH =20;

            #region Body

            if (xB > cB)
            {
                if (dB == 0)
                {
                    dB = (xB - cB) / 10.0f;
                    tB = max;
                }
                xB -= dB;
            }
            
            if(Math.Abs(xB-cB)<0.1f || xB<cB )
            {
                xB = cB;
                dB = 0;
            }

            if (tB > dTH)
            {
                alpha = 150;
            }else if(tB > 0){
                alpha = 150 * tB / 15;

            }
            if (tB > 0) { 
                tB--;
                e.Graphics.DrawRectangle(new RectangleF(0.5f - 0.15f / 2.0f, 0.65f, 0.15f, 0.03f), Color.FromArgb(alpha, 0, 0, 0));
                e.Graphics.DrawRectangle(new RectangleF(0.5f - 0.15f / 2.0f, 0.65f, 0.15f * xB / 1000.0f, 0.03f), Color.FromArgb(alpha, 64, 64, 255));
            }


            #endregion
            alpha = 0;
            #region Petro

            if (xP > cP)
            {
                if (dP == 0)
                {
                    dP = (xP - cP) / 10.0f;
                    tP = max;
                }
                xP -= dP;
            }

            if (Math.Abs(xP - cP) < 0.1f || xP < cP)
            {
                xP = cP;
                dP = 0;
            }

            if (tP >dTH)
            {
                alpha = 150;
            }
            else if (tP > 0)
            {
                alpha = 150 * tP / 15;

            }
            if (tP > 0)
            {
                tP--;
                e.Graphics.DrawRectangle(new RectangleF(0.5f - 0.15f / 2.0f, 0.58f, 0.15f, 0.03f), Color.FromArgb(alpha, 0, 0, 0));
                e.Graphics.DrawRectangle(new RectangleF(0.5f - 0.15f / 2.0f, 0.58f, 0.15f * xP / 1000.0f, 0.03f), Color.FromArgb(alpha, 255, 64, 64));
            }


            #endregion
            alpha = 0;
            #region Engine

            if (xE > cE)
            {
                if (dE == 0)
                {
                    dE = (xE - cE)/ 10.0f;
                    tE = max;
                }
                xE -= dE;
            }

            if (Math.Abs(xE - cE) < 0.1f || xE < cE)
            {
                xE = cE;
                dE = 0;
            }

            if (tE > dTH)
            {
                alpha = 150;
            }
            else if (tE > 0)
            {
                alpha = 150 * tE / 15;

            }
            if (tE > 0)
            {
                tE--;
                e.Graphics.DrawRectangle(new RectangleF(0.5f - 0.15f / 2.0f, 0.615f, 0.15f, 0.03f), Color.FromArgb(alpha, 0, 0, 0));
                e.Graphics.DrawRectangle(new RectangleF(0.5f - 0.15f / 2.0f, 0.615f, 0.15f * xE / 1000.0f, 0.03f), Color.FromArgb(alpha, 64, 255, 64));
            }


            #endregion


        }
    }
}
