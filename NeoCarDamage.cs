using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using _InputChecker;

namespace TestScriptCS.Scripts
{

    public class Damaged : Script
    {
        public RectangleF rect;
        public float Damage;
        public int Timer;
        public Vector2 Pos;
        public GTA.Font sFont;
        Random rnd;

        public Damaged(int S)
        {
            rnd = new Random(S);
            Damage = 0.0f;
            Timer = 0;
        }

        public Damaged()
        {
        }

        public void Set(float damage,int Type){

            Damage = damage;
            Timer = 30;

            sFont = new GTA.Font(0.08F, FontScaling.ScreenUnits);
            switch (Type)
            {
                case 0://Engine
                    sFont.Color = Color.FromArgb(0, 255, 0);
                    break;
                case 1://Petrol
                    sFont.Color = Color.FromArgb(255,0, 0);
                    break;
                case 2://Body
                    sFont.Color = Color.FromArgb(0,0, 255);
                    break;
            }
            Pos.X = (float)rnd.NextDouble();
            Pos.Y = (float)rnd.NextDouble();

            Pos.X = Pos.X * 0.4f + 0.2f;
            Pos.Y = Pos.Y * 0.4f + 0.2f;
            rect = new RectangleF(Pos.X,Pos.Y,0.2f,0.2f);

        }
        public void Decriment()
        {
            Timer--;
            Pos.Y -= 0.005f;
            rect.Y -= 0.005f;
        }

    }

     public class NewCarDamage : Script
    {
        Vehicle NowV, OldV;
        float OldE, OldP, OldB;
        int Engine, Petrol, Body;
        Damaged[] DAM;
        Random rnd;


        public NewCarDamage()
        {
            DAM = new Damaged[50];
            Engine = 0;
            Petrol = 1;
            Body = 2;
            rnd = new Random();
 
            for (int i = 0; i < 50; i++)
            {
                DAM[i] = new Damaged(rnd.Next());
            }

                Interval = 50;
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }

        private void SetDamegeArry(float Dam,int type){
            for (int i = 0; i < 50; i++)
            {
                if (DAM[i].Timer == 0)
                {
                    DAM[i].Set(Dam, type);
                    return;
                }
            }
        
        }

        private void Reset()
        {
            for (int i = 0; i < 50; i++)
            {
                DAM[i].Timer = 0;
            }
        }
       unsafe private void Bombat_Tick(object sender, EventArgs e)
        {

            if (Player.Character.isAlive)
            {
                //Player is alive.
                if (Player.Character.isInVehicle())
                {
                    //Player is in vehicle.
                    NowV = Player.Character.CurrentVehicle;

                    if (OldV != NowV) { Reset(); }
                    float nE = NowV.EngineHealth;
                    float nP = NowV.PetrolTankHealth;
                    float nB = NowV.Health;

                    if (nP > 0 && nE > 0)
                    {

                        if (nE < OldE)
                        {
                            SetDamegeArry(OldE - nE, Engine);
                        }
                        if (nP < OldP)
                        {
                            SetDamegeArry(OldP - nP, Petrol);
                        }
                        if (nB < OldB)
                        {
                            SetDamegeArry(OldB - nB, Body);
                        }
                    }

                    OldB = nB;
                    OldE = nE;
                    OldP = nP;
                    OldV = NowV;
                }
                else
                {
                    //Plyaer is not in vehicle.
                    OldP = -100;
                    OldE = -100;
                    OldB = -100;

                }

            }else
            {
                //Player is dead.
            }


        }
        unsafe private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            
            if (!Player.Character.isInVehicle()) { return; }

            e.Graphics.Scaling = FontScaling.ScreenUnits;
            for (int i = 0; i < 50; i++)
            {
                if (DAM[i].Timer == 0) { continue; }
                e.Graphics.DrawText(string.Format("-{0:f1}", DAM[i].Damage), DAM[i].rect, TextAlignment.Center | TextAlignment.Top, DAM[i].sFont);
                DAM[i].Decriment();
            }
            
            
        }
    }
}
