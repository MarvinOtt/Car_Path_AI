

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Car_Path_AI.UI;
using Car_Path_AI.UI.Specific;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static Car_Path_AI.UI.UI_Configs;
using static Car_Path_AI.UI.UI_STRUCTS;

namespace Car_Path_AI.UI
{
    public class UI_Handler
    {
        public const int UI_Active_Main = 1;
        public const int UI_Active_CompDrag = 2;

        public ContentManager Content;
        public static UI_Element ZaWarudo;  //JoJo Reference
        public static bool UI_Element_Pressed, UI_IsWindowHide;
        public static int UI_Active_State;
        public static int buttonheight = 25;
        public static int buttonwidth = 67;
        public static int sqarebuttonwidth = 25;
        public static Color main_BG_Col = new Color(new Vector3(0.15f));
        public static Color main_Hover_Col = Color.White * 0.1f;
        public static Color BackgroundColor = new Color(new Vector3(0.15f));
        public static Color HoverColor = new Color(new Vector3(0.3f));
        public static Color ActivColor = Color.White * 0.2f;
        public static Color ActivHoverColor = Color.Black;
        public static Color BorderColor = new Color(new Vector3(0.45f));
        public static Color[] layer_colors;
        public static Generic_Conf gen_conf;

        public UI_ValueInput valuebox_maxframes, valuebox_goalsandstarts;
        public static UI_StringButton SaveTrack;
        public static UI_StringButton LoadTrack;
        public static UI_StringButton SaveNetw;
        public static UI_StringButton LoadNetw;

        public UI_Handler(ContentManager Content)
	    {
		    this.Content = Content;
	    }

        public void Initialize(SpriteBatch spriteBatch)
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;

            // CONFIGS
            gen_conf = new Generic_Conf(font_color: Color.White, behav: 2, BGColor: BackgroundColor, HoverColor: HoverColor, ActiveColor: ActivColor, ActiveHoverColor: ActivHoverColor, tex_color: Color.White);
            gen_conf.font = Game1.font;

            valuebox_maxframes = new UI_ValueInput(new Pos(10, 50), new Point(100, 20), gen_conf, 1);
			valuebox_maxframes.value = "9999";
			valuebox_goalsandstarts = new UI_ValueInput(new Pos(120, 50), new Point(100, 20), gen_conf, 1);
            valuebox_goalsandstarts.value = "2";
			SaveTrack = new UI_StringButton(new Pos(10, 10), new Point(90, 20), "Save Track", true, gen_conf);
            LoadTrack = new UI_StringButton(new Pos(10, 0, ORIGIN.TR, ORIGIN.DEFAULT, SaveTrack), new Point(90, 20), "Load Track", true, gen_conf);
            SaveNetw = new UI_StringButton(new Pos(10, 0, ORIGIN.TR, ORIGIN.DEFAULT, LoadTrack), new Point(90, 20), "Save Netw", true, gen_conf);
            LoadNetw = new UI_StringButton(new Pos(10, 0, ORIGIN.TR, ORIGIN.DEFAULT, SaveNetw), new Point(90, 20), "Load Netw", true, gen_conf);


            InitializeUISettings(spriteBatch);
        }

        public void InitializeUISettings(SpriteBatch spritebatch)
        {
            SaveTrack.GotActivatedLeft += delegate (object sender)
            {
                FileHandler.SaveTrack();
            };
            LoadTrack.GotActivatedLeft += delegate (object sender)
            {
                FileHandler.LoadTrack();
            };
            SaveNetw.GotActivatedLeft += delegate (object sender)
            {
                FileHandler.SaveNetw();
            };
            LoadNetw.GotActivatedLeft += delegate (object sender)
            {
                FileHandler.LoadNetw();
            };

        }

        // Gets called when something of the Window or Graphics got changed
        public void Window_Graphics_Changed(object sender, EventArgs e)
        {

        }

	    public void Update()
	    {
            
            UI_Element_Pressed = false;
            UI_Active_State = 0;
            if (ZaWarudo != null)
            {
                ZaWarudo.UpdateMain();
                //return;
            }

            SaveTrack.UpdateMain();
            LoadTrack.UpdateMain();
            SaveNetw.UpdateMain();
            LoadNetw.UpdateMain();

            valuebox_maxframes.UpdateMain();
			valuebox_goalsandstarts.UpdateMain();

		}

	    public void Draw(SpriteBatch spritebatch)
	    {
			valuebox_goalsandstarts.Draw(spritebatch);
			valuebox_maxframes.Draw(spritebatch);

            LoadNetw.Draw(spritebatch);
            SaveNetw.Draw(spritebatch);
            LoadTrack.Draw(spritebatch);
            SaveTrack.Draw(spritebatch);
        }
    }
}
