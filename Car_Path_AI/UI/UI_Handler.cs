

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

        public UI_ValueInput valuebox_maxframes;

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

            valuebox_maxframes = new UI_ValueInput(new Pos(25, 25), new Point(100, 20), gen_conf, 1);

            

            InitializeUISettings(spriteBatch);
        }

        public void InitializeUISettings(SpriteBatch spritebatch)
        {
            
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

            valuebox_maxframes.UpdateMain();

        }

	    public void Draw(SpriteBatch spritebatch)
	    {
            valuebox_maxframes.Draw(spritebatch);
        }
    }
}
