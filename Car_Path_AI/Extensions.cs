using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Path_AI
{

    public static class Extensions
    {
        public static Vector2 LineIntersectionPoint(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2)
        {
            float a1 = e1.Y - s1.Y;
            float b1 = s1.X - e1.X;
            float c1 = a1 * s1.X + b1 * s1.Y;

            float a2 = e2.Y - s2.Y;
            float b2 = s2.X - e2.X;
            float c2 = a2 * s2.X + b2 * s2.Y;

            float delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be (NaN, NaN).
            return delta == 0 ? new Vector2(float.NaN, float.NaN)
                : new Vector2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }

        public static Vector2 FindIntersection(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, double tolerance = 0.001)
        {
            double x1 = s1.X, y1 = s1.Y;
            double x2 = e1.X, y2 = e1.Y;

            double x3 = s2.X, y3 = s2.Y;
            double x4 = e2.X, y4 = e2.Y;

            //// equations of the form x = c (two vertical lines)
            //if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
            //{
            //    throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
            //}

            ////equations of the form y=c (two horizontal lines)
            //if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
            //{
            //    throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
            //}

            //equations of the form x=c (two vertical lines)
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
            {
                return default(Vector2);
            }

            //equations of the form y=c (two horizontal lines)
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
            {
                return default(Vector2);
            }

            //general equation of line is y = mx + c where m is the slope
            //assume equation of line 1 as y1 = m1x1 + c1 
            //=> -m1x1 + y1 = c1 ----(1)
            //assume equation of line 2 as y2 = m2x2 + c2
            //=> -m2x2 + y2 = c2 -----(2)
            //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
            //so we will get below two equations 
            //-m1x + y = c1 --------(3)
            //-m2x + y = c2 --------(4)

            double x, y;

            //lineA is vertical x1 = x2
            //slope will be infinity
            //so lets derive another solution
            if (Math.Abs(x1 - x2) < tolerance)
            {
                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x1=c1=x
                //subsitute x=x1 in (4) => -m2x1 + y = c2
                // => y = c2 + m2x1 
                x = x1;
                y = c2 + m2 * x1;
            }
            //lineB is vertical x3 = x4
            //slope will be infinity
            //so lets derive another solution
            else if (Math.Abs(x3 - x4) < tolerance)
            {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x3=c3=x
                //subsitute x=x3 in (3) => -m1x3 + y = c1
                // => y = c1 + m1x3 
                x = x3;
                y = c1 + m1 * x3;
            }
            //lineA & lineB are not vertical 
            //(could be horizontal we can handle it with slope = 0)
            else
            {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
                //plugging x value in equation (4) => y = c2 + m2 * x
                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;

                //verify by plugging intersection point (x, y)
                //in orginal equations (1) & (2) to see if they intersect
                //otherwise x,y values will not be finite and will fail this check
                if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                    && Math.Abs(-m2 * x + y - c2) < tolerance))
                {
                    return default(Vector2);
                }
            }

            //x,y can intersect outside the line segment since line is infinitely long
            //so finally check if x, y is within both the line segments
            if (IsInsideLine(s1, e1, x, y) &&
                IsInsideLine(s2, e2, x, y))
            {
                return new Vector2 { X = (float)x, Y = (float)y };
            }

            //return default null (no intersection)
            return default(Vector2);

        }

        public static double FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2)
        {
            Vector2 closest;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Vector2(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Vector2(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Vector2(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Returns true if given point(x,y) is inside the given line segment
        private static bool IsInsideLine(Vector2 s, Vector2 e, double x, double y)
        {
            return (x >= s.X && x <= e.X
                        || x >= e.X && x <= s.X)
                   && (y >= s.Y && y <= e.Y
                        || y >= e.Y && y <= s.Y);
        }

        // Keyboard
        public static bool AreKeysDown(this KeyboardState kbs, params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; ++i)
            {
                if (kbs.IsKeyUp(keys[i]))
                    return false;
            }
            return true;
        }
        public static bool AreKeysUp(this KeyboardState kbs, params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; ++i)
            {
                if (kbs.IsKeyDown(keys[i]))
                    return false;
            }
            return true;
        }

        // Spritebatch
        public static void DrawFilledRectangle(this SpriteBatch sb, Rectangle rec, Color col)
        {
            sb.Draw(Game1.pixel, rec, col);
        }

        public static void DrawLine(this SpriteBatch sb, int x1, int y1, int x2, int y2, Color color)
        {
            DrawLine(sb, new Point(x1, y1), new Point(x2, y2), color, 1.0f);
        }
        public static void DrawLine(this SpriteBatch sb, int x1, int y1, int x2, int y2, Color color, float thickness)
        {
            DrawLine(sb, new Point(x1, y1), new Point(x2, y2), color, thickness);
        }
        public static void DrawLine(this SpriteBatch sb, Point point1, Point point2, Color color)
        {
            DrawLine(sb, point1, point2, color, 1.0f);
        }
        public static void DrawLine(this SpriteBatch sb, Point point1, Point point2, Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1.ToVector2(), point2.ToVector2());

            // calculate the angle between the two vectors
            float angle = (float)System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(sb, point1, distance, angle, color, thickness);
        }
        public static void DrawLine(this SpriteBatch sb, Point point, float length, float angle, Color color)
        {
            DrawLine(sb, point, length, angle, color, 1.0f);
        }
        public static void DrawLine(this SpriteBatch sb, Point point, float length, float angle, Color color, float thickness)
        {
            sb.Draw(Game1.pixel, point.ToVector2(), null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public static void DrawCircle(this BasicEffect be, Vector2 s, float radius)
        {
            be.World = Matrix.CreateScale(radius) * Matrix.CreateTranslation(new Vector3(s, 0));
            be.CurrentTechnique.Passes[0].Apply();
            Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Game1.circle_vertices, 0, Game1.circle_vertices.Length - 1);
        }

        public static void DrawHollowRectangle(this SpriteBatch sb, Rectangle rec, Color col, int strokewidth)
        {
            rec.Size -= new Point(strokewidth, strokewidth);
            sb.DrawLine(rec.Location, rec.Location + new Point(rec.Size.X + strokewidth, 0), col, strokewidth);
            sb.DrawLine(rec.Location + new Point(strokewidth, 0), rec.Location + new Point(strokewidth, rec.Size.Y + strokewidth), col, strokewidth);
            sb.DrawLine(rec.Location + new Point(rec.Size.X + strokewidth, strokewidth), rec.Location + rec.Size + new Point(strokewidth, strokewidth), col, strokewidth);
            sb.DrawLine(rec.Location + new Point(strokewidth, rec.Size.Y), rec.Location + rec.Size + new Point(strokewidth, 0), col, strokewidth);
            //Game1.spriteBatch.Draw(Game1.pixel, pos, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), col, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

        // LINQ
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> block)
        {
            foreach (var item in list)
            {
                block(item);
            }
        }

        // Textures
        private static byte[] pixelbyte = new byte[4];
        public static void SetPixel(this Texture2D tex, byte data, Point pos)
        {
            pixelbyte[3] = data;
            int mulpos = pos.Y * tex.Width + pos.X;
            tex.SetData(0, new Rectangle(pos, new Point(1)), pixelbyte, 0, 4);
        }

        // Arrays
        public static void GetArea<T>(this T[,] arr, T[,] dest, Rectangle source)
        {
            for(int x = 0; x < source.Width; ++x)
            {
                for (int y = 0; y < source.Height; ++y)
                {
                    dest[x, y] = arr[source.X + x, source.Y + y];
                }
            }
        }

        // Reading String from File
        public static string ReadNullTerminated(this System.IO.FileStream rdr)
        {
            var bldr = new System.Text.StringBuilder();
            int nc;
            while ((nc = rdr.ReadByte()) > 0)
                bldr.Append((char)nc);

            return bldr.ToString();
        }

        public static void CMD_Execute(string cmd, string args)
        {
            Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args;
            p.StartInfo.CreateNoWindow = false;

            //required to capture standard output 
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            ////read the command line output 
            //StreamReader sr = p.StandardOutput;
            //return (sr.ReadToEnd());
        }


        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length + 1];
            System.Buffer.BlockCopy(Encoding.ASCII.GetBytes(str), 0, bytes, 0, bytes.Length - 1);
            bytes[bytes.Length - 1] = 0;
            return bytes;
        }

        //Move Item in List
        public static void Move<T>(this List<T> list, int oldindex, int newindex)
        {
            var item = list[oldindex];

            list.RemoveAt(oldindex);

            if (newindex > oldindex) newindex--;

            list.Insert(newindex, item);
        }
    }
}
