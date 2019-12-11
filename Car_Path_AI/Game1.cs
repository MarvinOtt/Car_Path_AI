using Car_Path_AI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Car_Path_AI
{
    public struct Mouse_States
    {
        public MouseState New, Old;

        public Mouse_States(MouseState New, MouseState Old)
        {
            this.New = New;
            this.Old = Old;
        }

        public bool IsMiddleButtonToggleOn()
        {
            return New.MiddleButton == ButtonState.Pressed && Old.MiddleButton == ButtonState.Released;
        }
        public bool IsMiddleButtonToggleOff()
        {
            return New.MiddleButton == ButtonState.Released && Old.MiddleButton == ButtonState.Pressed;
        }

        public bool IsLeftButtonToggleOn()
        {
            return New.LeftButton == ButtonState.Pressed && Old.LeftButton == ButtonState.Released;
        }
        public bool IsRightButtonToggleOn()
        {
            return New.RightButton == ButtonState.Pressed && Old.RightButton == ButtonState.Released;
        }
        public bool IsLeftButtonToggleOff()
        {
            return New.LeftButton == ButtonState.Released && Old.LeftButton == ButtonState.Pressed;
        }
        public bool IsRightButtonToggleOff()
        {
            return New.RightButton == ButtonState.Released && Old.RightButton == ButtonState.Pressed;
        }
    }
    public struct Keyboard_States
    {
        public KeyboardState New, Old;

        public Keyboard_States(KeyboardState New, KeyboardState Old)
        {
            this.New = New;
            this.Old = Old;
        }
        public bool IsKeyToggleDown(Keys key)
        {
            return New.IsKeyDown(key) && Old.IsKeyUp(key);
        }
        public bool IsKeyToggleUp(Keys key)
        {
            return !IsKeyToggleDown(key);
        }
    }

    public struct Line
    {
        public Point start, end;
        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }
       

    }
    public class Game1 : Game
    {   
        public static ContentManager content;
        public static System.Windows.Forms.Form form;
        public static event EventHandler GraphicsChanged;
        public static GraphicsDeviceManager graphics;
        public static SpriteFont font;
        SpriteBatch spriteBatch;
        public static Random r;
        UI_Handler ui_handler;
        public static BasicEffect basiceffect;
        public static VertexPositionColor[] circle_vertices;


        public static Track track;
        public int maxframes, curframe;
        public bool IsPause = true;
        public static float mutation_strength = 0.04f;

        public static Texture2D pixel;

        #region INPUT

        public static Keyboard_States kb_states;
        public static Mouse_States mo_states;

        private bool GraphicsNeedApplyChanges;

        #endregion

        public static int Screenwidth;
        public static int Screenheight;

        public Game1()
        {
            GraphicsChanged += UpdateEverythingOfGraphics;
            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 100,
                PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 100,
                IsFullScreen = false,
                SynchronizeWithVerticalRetrace = true

            };
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(SetToPreserve);
            //TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 6);
            IsFixedTimeStep = false;
            Window.IsBorderless = false;

            Content.RootDirectory = "Content";
        }

        // Gets called everytime the Windows Size gets changed
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width != 0)
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            if (Window.ClientBounds.Height != 0)
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            // Not Applying Graphics here because when resizing happens, ApplyChanges would be called too often which could cause a crash
            // When resizing happens, the Update Method is not going to be called so long until resizing is finished, and therefore Apply Changes gets only called once
            GraphicsNeedApplyChanges = true;
        }

        public void UpdateEverythingOfGraphics(object sender, EventArgs e)
        {
            Screenwidth = graphics.PreferredBackBufferWidth;
            Screenheight = graphics.PreferredBackBufferHeight;
            basiceffect.Projection = Matrix.CreateOrthographicOffCenter
            (0, GraphicsDevice.Viewport.Width,     // left, right
            GraphicsDevice.Viewport.Height, 0,    // bottom, top
            0, 1);
        }
        void SetToPreserve(object sender, PreparingDeviceSettingsEventArgs eventargs) { eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents; }
        void GetsMinimized(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            System.Windows.Forms.CloseReason c = e.CloseReason;
        }

        protected override void Initialize()
        {
            content = Content;
            IsMouseVisible = true;
            form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);
            form.MaximizeBox = true;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            form.Resize += Window_ClientSizeChanged;
            form.FormClosing += GetsMinimized;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            basiceffect = new BasicEffect(GraphicsDevice);
            basiceffect.VertexColorEnabled = true;
            basiceffect.Projection = Matrix.CreateOrthographicOffCenter
            (0, GraphicsDevice.Viewport.Width,     // left, right
            GraphicsDevice.Viewport.Height, 0,    // bottom, top
            0, 1);
            GraphicsChanged(null, EventArgs.Empty);

            
            circle_vertices = new VertexPositionColor[360];
            float fac = (1.0f / (circle_vertices.Length - 1)) * (float)Math.PI * 2;
            for(int i = 0; i < circle_vertices.Length; ++i)
            {
                circle_vertices[i] = new VertexPositionColor(new Vector3((float)Math.Cos(fac * i), (float)Math.Sin(fac * i), 0), Color.Orange);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            r = new Random();
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Bgra32);
            font = Content.Load<SpriteFont>("font");
            Color[] colors = new Color[1];
            colors[0] = Color.White;
            pixel.SetData(colors);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ui_handler = new UI_Handler(Content);
            ui_handler.Initialize(spriteBatch);
            ui_handler.valuebox_maxframes.ValueChanged += MaxFramesChange;
            track = new Track();
            track.GenerateNew(20);
            //for (int i = 0; i < 10; ++i)
            //{
            //    track.cars.Add(new Car(new Vector2(r.Next(0, Screenwidth), r.Next(0, Screenheight))));
            //    //cars[i].rot = r.Next(0, 10000) * 0.003f;
            //}

            maxframes = 600;

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void MaxFramesChange(object sender)
        {
            UI_ValueInput obj = sender as UI_ValueInput;
            maxframes = obj.final_value;
        }

   

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            kb_states.New = Keyboard.GetState();
            mo_states.New = Mouse.GetState();
            if (IsActive)
            {
                ui_handler.Update();

                if (kb_states.New.IsKeyDown(Keys.Down))
                    mutation_strength -= 0.0001f;
                if (kb_states.New.IsKeyDown(Keys.Up))
                    mutation_strength += 0.0001f;
                if (kb_states.IsKeyToggleDown(Keys.Space))
                    IsPause ^= true;
                if (kb_states.IsKeyToggleDown(Keys.R))
                {
                    curframe = 0;
                    track.GenerateNew(60);
                }

                if (UI_Handler.UI_Active_State == 0)
                {
                    track.UpdateIO();
                }
                if (!IsPause)
                {
                    curframe++;
                    track.Update();
                }
                bool allDone = track.cars.TrueForAll(x => x.State > 0);
                if (curframe > maxframes || allDone)
                {
                    curframe = 0;
                    track.ResetCars(mutation_strength);
                }

                if (GraphicsNeedApplyChanges)
                {
                    graphics.ApplyChanges();
                    GraphicsNeedApplyChanges = false;
                    if (Screenwidth != 0 && Screenheight != 0)
                        GraphicsChanged(null, EventArgs.Empty);
                }
                //for (int i = 0; i < cars.Length; ++i)
                //    cars[i].Update();

            }
            kb_states.Old = kb_states.New;
            mo_states.Old = mo_states.New;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            

            track.Draw(spriteBatch);

            spriteBatch.DrawString(font, mutation_strength.ToString(), new Vector2(100, 100), Color.Red);
            spriteBatch.DrawString(font, curframe.ToString() + " / " + maxframes.ToString(), new Vector2(100, 130), Color.Red);

            ui_handler.Draw(spriteBatch);
            spriteBatch.End();
            //basiceffect.DrawCircle(Vector2.Zero, 100);

            base.Draw(gameTime);
        }
        
    }
}
