using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{
    class Dameges : Script
    {
        Random rnd;
        public float[] Damage;
        public int[] Timer;
        public Vector3[] Pos;
        public Color color;
        float a, b;
        int timerscale;
        public Dameges(Color c){
            color = c;
            Damage = new float[30];
            Timer = new int[30];
            Pos = new Vector3[30];
            Reset();
            rnd = new Random();
            float min_x = 0;
            float min_y = 0.5f;
            float max_x = 1000.0f;
            float max_y = 0.1f;
            timerscale = 50;
            if (max_x - min_y == 0)
            {
                a = 0;
                b = 0;
            }
            else
            {
                a = (max_y - min_y) / (max_x - min_x);
                b = max_x - a * max_x;
            }

        }

        public void Reset()
        {
            for (int i = 0; i < 30; i++)
            {
                Timer[i] = -1;
                Damage[i] = 0;
            }
        }

        public void AddDamage(float D)
        {
            for (int i = 0; i < 30; i++)
            {
                if (Timer[i] == -1)
                {
                    Timer[i] = timerscale;
                    Damage[i] = D;
                    Pos[i].X = 0.3f * ((float)rnd.NextDouble() - 0.5f) + 0.5f;
                    Pos[i].Y = 0.3f * ((float)rnd.NextDouble() - 0.5f) + 0.5f;
                    Pos[i].Z = a * D + b;
                    return;
                }
            }
        }

        public void Decrease(){
            for (int i = 0; i < 30; i++)
            {
                if (Timer[i] != -1)
                {
                    Timer[i]--;
                }
            }
        }

        public Color GetColor(int ID)
        {
            return Color.FromArgb(255 * Timer[ID] / timerscale, color.R, color.G, color.B);
           
        }
        public bool IsDamegeActive(int ID)
        {
            if (Timer[ID] != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public class NewCarDamage : Script
    {
        Vehicle pV;
        Dameges E;
        Dameges B;
        Dameges P;
        float OldE, OldB, OldP;
        public NewCarDamage()
        {
            E = new Dameges(Color.FromArgb(0,0,255,0));
            B = new Dameges(Color.FromArgb(0,0,0,255));
            P = new Dameges(Color.FromArgb(0,255,0,0));

            Interval = 50;
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }

        private void Bombat_Tick(object sender, EventArgs e)
        {
            pV = Player.Character.CurrentVehicle;
            if (Exists(pV))
            {
                float dB, dE, dP;
                dB = OldB - pV.Health;
                dE = OldE - pV.EngineHealth;
                dP = OldP - pV.PetrolTankHealth;

                
                if (dB > 0)
                {
                    B.AddDamage(dB);
                }
                if (dE > 0 && pV.EngineHealth>0)
                {
                    E.AddDamage(dE);
                }
                if (dP > 0 && pV.PetrolTankHealth>0)
                {
                    P.AddDamage(dP);
                }
                OldB = pV.Health;
                OldE = pV.EngineHealth;
                OldP = pV.PetrolTankHealth;

                E.Decrease();
                P.Decrease();
                B.Decrease();

            }
            else
            {
                E.Reset();
                P.Reset();
                B.Reset();
                OldB = 0;
                OldE = 0;
                OldP = 0;
            }

       }
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            if (Player.Character.isInVehicle())
            {
                e.Graphics.Scaling = FontScaling.ScreenUnits; // size on screen will always be the same, regardless of resolution

                for (int i = 0; i < 30; i++)
                {
                    Vector3 V;
                    GTA.Font F;

                    //Engine
                    if (E.IsDamegeActive(i))
                    {
                        V = E.Pos[i];
                        F = new GTA.Font(0.04f, FontScaling.ScreenUnits);
                        F.Color = E.GetColor(i);
                        e.Graphics.DrawText(string.Format("-{0:f1}", E.Damage[i]), new RectangleF(V.X, V.Y, 0.1f, 0.5f), TextAlignment.Left | TextAlignment.Top, F);
                    }

                    //Body
                    if (B.IsDamegeActive(i))
                    {
                        V = B.Pos[i];
                        F = new GTA.Font(0.04f, FontScaling.ScreenUnits);
                        F.Color = B.GetColor(i);
                        e.Graphics.DrawText(string.Format("-{0:f1}", B.Damage[i]), new RectangleF(V.X, V.Y, 0.1f, 0.5f), TextAlignment.Left | TextAlignment.Top, F);
                    }

                    //Petrol
                    if (P.IsDamegeActive(i))
                    {
                        V = P.Pos[i];
                        F = new GTA.Font(0.04f, FontScaling.ScreenUnits);
                        F.Color = P.GetColor(i);
                        e.Graphics.DrawText(string.Format("-{0:f1}", P.Damage[i]), new RectangleF(V.X, V.Y, 0.1f, 0.5f), TextAlignment.Left | TextAlignment.Top, F);
                    }
                   
                }

            }

        }
    }
}
 