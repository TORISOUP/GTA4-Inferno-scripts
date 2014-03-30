using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using GTA;
using _InputChecker;
using _ItemListForm;

namespace TestScriptCS.Scripts
{
    //仲間のHPを出す
    public class FriendsHP : Script
    {
        int OldTeamMember;
        int[] OldHPs;
        GTA.Font screenFont;
        string[] buff;
         
        public FriendsHP()
        {

            screenFont = new GTA.Font(0.03F, FontScaling.ScreenUnits);
            this.Tick += new EventHandler(this.Bombat_Tick);
            this.PerFrameDrawing += new GraphicsEventHandler(this.Kyori_PerFrameDrawing);

        }



        private void Bombat_Tick(object sender, EventArgs e)
        {
            
   
       }
  
        private void Kyori_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;

            Ped[] Friend = Player.Group.ToArray(false);

            if (OldTeamMember != Friend.Length)
            {
                OldTeamMember = Friend.Length;
                OldHPs = new int[OldTeamMember];
                buff = new string[OldTeamMember];
                for (int i = 0, max = Friend.Length,j=0; i < max; i++)
                {
                    OldHPs[i] = Friend[i].Health;
                    if (OldHPs[i] > 100) { OldHPs[i] = 100; }
                    buff[i] = GetPedsNames(Friend[i],j++);
                }
            }

            for(int i=0,max=Friend.Length;i<max;i++){
                if (!Exists(Friend[i])) { continue; }

                int HP = Friend[i].Health;
                if (HP > 100) { HP = 100; }
                if (HP > OldHPs[i]) { OldHPs[i] = HP; }


                e.Graphics.DrawRectangle(new RectangleF(0.0f, 0.04f+0.042f*i, 0.08f,0.04f ), Color.FromArgb(150, 100, 100, 100));


                if (HP >= 0)
                {
                    Color co;
                    if (Friend[i].Money==500)
                    {
                        co = Color.FromArgb(150, 100, 255, 100);
                    }else{
                        co =  Color.FromArgb(150, 255, 100, 100);
                    }

                    e.Graphics.DrawRectangle(new RectangleF(0.0f, 0.04f + 0.042f * i, 0.08f * (float)HP / 100.0f, 0.04f), co);
                    if (OldHPs[i] > HP)
                    {
                        e.Graphics.DrawRectangle(new RectangleF(0.08f * (float)HP / 100.0f, 0.04f + 0.042f * i, 0.08f * (float)(OldHPs[i] - HP) / 100.0f, 0.04f), Color.FromArgb(150, 255, 0, 0));
                        OldHPs[i]--;
                    }
                }


                //string buff = GetPedsNames(Friend[i],i);
                e.Graphics.DrawText(buff[i], new RectangleF(0.0f, 0.04f + 0.042f * i, 0.08f, 0.04f), TextAlignment.Left | TextAlignment.VerticalCenter, screenFont);
            }


        }

        private string GetPedsNames(Ped p, int index)
        {
            string name;

            switch ((uint)p.Model.Hash)
            {
                case 0x6E7BF45F:
                    name ="ANNA";
                    break;

                case 0x9DD666EE:
                    name ="ANTHONY";
                    break;

                case 0x5927A320:
                    name ="BADMAN";
                    break;

                case 0x596FB508:
                    name ="BERNIE";
                    break;

                case 0x6734C2C8:
                    name ="BLEDAR";
                    break;

                case 0x192BDD4A:
                    name ="BRIAN";
                    break;

                case 0x98E29920:
                    name ="BRUCIE";
                    break;

                case 0x0E28247F:
                    name ="BULGARIN";
                    break;

                case 0x0548F609:
                    name ="CHARISE";
                    break;

                case 0xB0D18783:
                    name ="CHARLIEUC";
                    break;

                case 0x500EC110:
                    name ="CLARENCE";
                    break;

                case 0x5786C78F:
                    name ="DARDAN";
                    break;

                case 0x1709B920:
                    name ="DARKO";
                    break;

                case 0x0E27ECC1:
                    name ="DMITRI";
                    break;

                case 0xDB354C19:
                    name ="DWAYNE";
                    break;

                case  0xA09901F1:
                    name ="EDDIELOW";
                    break;

                case  0x03691799:
                    name ="FAUSTIN";
                    break;

                case 0x65F4D88D:
                    name ="FRANCIS";
                    break;

                case 0x54EABEE4:
                    name ="TOM";
                    break;

                case 0x7EED7363:
                    name ="GORDON";
                    break;

                case 0xEAAEA78E:
                    name ="GRACIE";
                    break;

                case 0x3A7556B2:
                    name ="HOSSAN";
                    break;
                case 0xCE3779DA:
                    name = "ILYENA";
                    break;

                case 0xE369F2A6:
                    name = "ISAAC";
                    break;

                case 0x458B61F3:
                    name = "IVAN";
                    break;

                case 0x15BCAD23:
                    name = "JAY";
                    break;

                case 0x0A2D8896:
                    name = "JASON";
                    break;

                case 0x17446345:
                    name = "JEFF";
                    break;

                case 0xEA28DB14:
                    name = "JIMMY";
                    break;

                case 0xC9AB7F1C:
                    name = "JOHNNY";
                    break;

                case 0xD1E17FCA:
                    name = "KATE";
                    break;

                case 0x3B574ABA:
                    name = "KENNY";
                    break;

                case  0x58A1E271: case 0xB4008E4D:
                    name = "L.JACOB";
                    break;

                case 0xD75A60C8:
                    name = "LUCA";
                    break;

                case 0xE2A57E5E:
                    name = "LUIS";
                    break;

                case 0xC1FE7952:
                    name = "MALLORIE";
                    break;

                case 0xECC3FBA7:
                    name = "MAMC";
                    break;

                case 0x5629F011:
                    name = "MANNY";
                    break;

                case 0x188232D0:
                    name = "MARNIE";
                    break;

                case 0xCFE0FB92:
                    name = "MEL";
                    break;

                case 0xBF9672F4:
                    name = "MICHELLE";
                    break;

                case 0xDA0D3182:
                    name = "MICKEY";
                    break;

                case 0xF6237664:
                    name = "PATHOS";
                    break;

                case 0x8BE8B7F2:
                    name = "PETROVIC";
                    break;


                case  0x932272CA:
                    name = "PHIL";
                    break;

                case 0x6AF081E8:
                    name = "PLAYBOY X";
                    break;

                case 0x38E02AB6:
                    name = "RAY";
                    break;

                case 0xDCFE251C:
                    name = "RICKY";
                    break;

                case 0x89395FC9: case 0x2145C7A5:
                    name = "ROMAN";
                    break;

                case 0xFEF00775:
                    name = "SARAH";
                    break;

                case 0x528AE104:
                    name = "TUNA";
                    break;

                case 0xC380AE97:
                    name = "VINNY";
                    break;

                case 0x356E1C42:
                    name = "VLAD";
                    break;

                case 0x45B445F9:
                    name = "DERRICK";
                    break;

		        case 0x64C74D3B:
                    name = "PACKIE";
                    break;

                case 0x2BD27039:
                    name = "MICHAEL";
                    break;
                default:
                    name = string.Format("Ped{0}", index);
                    break;
            
            }

            return name;
        }
    }
}
