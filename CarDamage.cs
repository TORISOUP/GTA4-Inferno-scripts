using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class CarDamage : Script
    {
        bool[] D_Flag;
        int[]  D_Timer;
        float[]  D;
        int[]  DC;
        float[] Old;
        int bod, pet, eng;
        Ped player;
        Vehicle v;
        Random rnd;
        Vector2[] Posit;

        GTA.Font[] Nom = new GTA.Font[3];
        GTA.Font[] small = new GTA.Font[3];
       

        public CarDamage()
        {
            bod = 0;
            pet = 1;
            eng = 2;
            for (int i = 0; i < 3; i++)
            {
                Nom[i] = new GTA.Font(0.08F, FontScaling.ScreenUnits);
                small[i] = new GTA.Font(0.05F, FontScaling.ScreenUnits);

                switch (i)
                {
                    case 0:
       
                        Nom[i].Color = Color.Blue;
                        small[i].Color = Color.Blue;
                        break;
                    case 1:
                        Nom[i].Color = Color.Red;
                        small[i].Color = Color.Red;
                        break;
                    case 2:
                        Nom[i].Color = Color.Green;
                        small[i].Color = Color.Green;
                        break;
                }
            }

            D_Flag = new bool[3];
            D_Timer = new int[3];
            D = new float[3];
            DC = new int[3];
            Old = new float[3];

            Posit = new Vector2[3];
            rnd = new Random();
            Reset();
            player = Player.Character;
            Interval = 50;
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }

        private void Reset()
        {
            for (int i = 0; i < 3; i++)
            {
                D_Flag[i] = false;
                D_Timer[i] = 0;
                D[i] = 0;
                DC[i] = 0;
                Old[i] = 0.0f;
            }
        }

        private void SetAlls(int n, float nowHP)
        {
            if (Old[n] - nowHP > 0 && Old[n]>0.0f)
            {
                D[n] += Old[n] - nowHP;
                DC[n]++;
                D_Timer[n] = 30;
                player.Money += 100;
                if (D_Flag[n] == false)
                {
                    D_Flag[n] = true;
                    Posit[n].X = 0.1f*(float)rnd.Next(1, 8);
                    Posit[n].Y = 0.1f * (float)rnd.Next(1, 8);
                    player.Money += 1000;
                }
            }

            if (D_Timer[n] > 0)
            {
                D_Timer[n]--;
            }
            else
            {
                D_Flag[n] = false;
                DC[n] = 0;
                D[n] = 0;
            }
        }

        private void Bombat_Tick(object sender, EventArgs e)
        {
            v = player.CurrentVehicle;
            if (!Exists(v))
            {
                Reset();
                return;
            }
            float Body = v.Health;
            float Petrol = v.PetrolTankHealth;
            float Engine = v.EngineHealth;

            SetAlls(bod,Body);
            SetAlls(pet, Petrol);
            SetAlls(eng, Engine);

            Old[bod] = Body;
            Old[pet] = Petrol;
            Old[eng] = Engine;
            

       }
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            if (D_Flag[bod])
            {

                e.Graphics.Scaling = FontScaling.ScreenUnits;
                RectangleF rect = new RectangleF(Posit[bod].X,Posit[bod].Y, 0.3F, 0.1F);
                e.Graphics.DrawText(string.Format("-{0:f1}",D[bod]), rect, TextAlignment.Center | TextAlignment.VerticalCenter, Nom[bod]);
                if (DC[bod] > 1)
                {

                    rect = new RectangleF(Posit[bod].X+0.04f, Posit[bod].Y+0.05f, 0.3F, 0.1F);
                    e.Graphics.DrawText(string.Format("{0} combo!", DC[bod]), rect, TextAlignment.Center | TextAlignment.VerticalCenter, small[bod]);
                }

            }
            if (D_Flag[pet])
            {

                e.Graphics.Scaling = FontScaling.ScreenUnits;
                RectangleF rect = new RectangleF(Posit[pet].X, Posit[pet].Y, 0.3F, 0.1F);
                e.Graphics.DrawText(string.Format("-{0:f1}", D[pet]), rect, TextAlignment.Center | TextAlignment.VerticalCenter, Nom[pet]);
                if (DC[pet] > 1)
                {

                    rect = new RectangleF(Posit[pet].X + 0.04f, Posit[pet].Y + 0.05f, 0.3F, 0.1F);
                    e.Graphics.DrawText(string.Format("{0} combo!", DC[pet]), rect, TextAlignment.Center | TextAlignment.VerticalCenter, small[pet]);
                }

            }
            if (D_Flag[eng])
            {

                e.Graphics.Scaling = FontScaling.ScreenUnits;
                RectangleF rect = new RectangleF(Posit[eng].X, Posit[eng].Y, 0.3F, 0.1F);
                e.Graphics.DrawText(string.Format("-{0:f1}", D[eng]), rect, TextAlignment.Center | TextAlignment.VerticalCenter, Nom[eng]);
                if (DC[eng] > 1)
                {
                    rect = new RectangleF(Posit[eng].X + 0.04f, Posit[eng].Y + 0.05f, 0.3F, 0.1F);
                    e.Graphics.DrawText(string.Format("{0} combo!", DC[eng]), rect, TextAlignment.Center | TextAlignment.VerticalCenter,small[eng]);
                }

            }



        }
    }
}
