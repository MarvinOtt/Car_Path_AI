using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public Matrix matr;

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

                if (Game1.kb_states.New.IsKeyDown(Keys.W))
                    acl = 0.12f;
                else if (Game1.kb_states.New.IsKeyDown(Keys.S))
                    acl = -0.12f;
                else
                    acl = 0;

                if (Game1.kb_states.New.IsKeyDown(Keys.D))
                    rot += 0.0125f * speed;
                if (Game1.kb_states.New.IsKeyDown(Keys.A))
                    rot -= 0.0125f * speed;

                matr = Matrix.CreateRotationZ(rot);

                speed += acl;
                Vector2 dir = DirFromRotation(rot);
                pos += dir * speed;
            }
            //rot += 0.01f;
        }

        public float RotationFromDir(Vector2 dir)
        {
            float res = (float)(Math.Atan2(dir.Y, dir.X));
            return res;
        }
        public Vector2 DirFromRotation(float rot)
        {
            return new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
        }
        bool onSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        int orientation(Point p, Point q, Point r)
        {
            int val = (q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0;  // colinear 

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }
        bool doIntersect(Line l1, Line l2)
        {
            Point p1 = l1.start;
            Point q1 = l1.end;
            Point p2 = l2.start;
            Point q2 = l2.end;

            // Find the four orientations needed for general and 
            // special cases 
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases 
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(tex, new Rectangle(new Point((int)pos.X, (int)pos.Y), new Point(117, 46)), new Rectangle(0, 0, 117, 46), Color.White, (rot), new Vector2(40, 23), SpriteEffects.None, 0);
            //spritebatch.DrawFilledRectangle(new Rectangle((int)pos.X - 3, (int)pos.Y - 3, 6, 6), Color.Red);

            Vector2 v = new Vector2(-40, -23);
            Vector2 v1 = pos + Vector2.Transform(v, matr);
            Vector2 v2 = pos + Vector2.Transform(v + new Vector2(117, 0), matr);
            Vector2 v3 = pos + Vector2.Transform(v + new Vector2(117, 46), matr);
            Vector2 v4 = pos + Vector2.Transform(v + new Vector2(0, 46), matr);

            Line l1 = new Line(v1.ToPoint(), v2.ToPoint());
            Line l2 = new Line(v2.ToPoint(), v3.ToPoint());
            Line l3 = new Line(v3.ToPoint(), v4.ToPoint());
            Line l4 = new Line(v4.ToPoint(), v1.ToPoint());
            bool IsIntersect = false;
            for(int i = 0; i < Game1.track.lines.Count; ++i)
            {
                Line cur = Game1.track.lines[i];
                if(doIntersect(cur, l1) || doIntersect(cur, l2) || doIntersect(cur, l3) || doIntersect(cur, l4))
                {
                    IsIntersect = true;
                    break;
                }
            }
            Color col = Color.Blue;
            if (IsIntersect)
                col = Color.Green;

            spritebatch.DrawFilledRectangle(new Rectangle((int)v1.X - 3, (int)v1.Y - 3, 6, 6), col);
            spritebatch.DrawFilledRectangle(new Rectangle((int)v2.X - 3, (int)v2.Y - 3, 6, 6), col);
            spritebatch.DrawFilledRectangle(new Rectangle((int)v3.X - 3, (int)v3.Y - 3, 6, 6), col);
            spritebatch.DrawFilledRectangle(new Rectangle((int)v4.X - 3, (int)v4.Y - 3, 6, 6), col);
            spritebatch.DrawLine(l1.start, l1.end, col, 3);
            spritebatch.DrawLine(l2.start, l2.end, col, 3);
            spritebatch.DrawLine(l3.start, l3.end, col, 3);
            spritebatch.DrawLine(l4.start, l4.end, col, 3);


            //v = Vector2.Transform(v, matr);
        }
    }
}
