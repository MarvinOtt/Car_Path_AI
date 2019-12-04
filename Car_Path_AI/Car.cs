using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Path_AI
{
    public class Car
    {
        public const int DRIVING = 0;
        public const int CRASHED = 1;
        public const int FINISHED = 2;

        public Vector2 pos;
        public float speed, acl, rot;
        public int State;
        public int driving_time;

        public static Texture2D tex;

        public Car(Vector2 pos)
        {
            if (tex == null)
                tex = Game1.content.Load<Texture2D>("CarTexture");
            this.pos = pos;
        }

        public void Update()
        {
            if(State == DRIVING)
            {
                driving_time++;

                // Check for Wall Collisions

                // Check for Finish

                
                
            }
            rot += 0.1f;
        }

        public float RotationFromDir(Vector2 dir)
        {
            float res = (float)(Math.Atan2(dir.Y, dir.X));
            return res;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(tex, new Rectangle(new Point((int)pos.X, (int)pos.Y), new Point(117, 46)), new Rectangle(0, 0, 117, 46), Color.White, (rot), new Vector2(40, 23), SpriteEffects.None, 0);
            spritebatch.DrawFilledRectangle(new Rectangle((int)pos.X - 3, (int)pos.Y - 3, 6, 6), Color.Red);
        }
    }
}
