using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Car_Path_AI.Extensions;

namespace Car_Path_AI
{
    public class Track
    {
        public List<Car> cars;
        public Vector2 startpos, goalpos;
        public float goalradius, startdir;

        public bool DrawingEnabled = true;
        public bool IsDrawing;
        Point posA, posB;
        public List<Line> lines;
        Rectangle goal;
        public Track()
        {
            lines = new List<Line>();
            cars = new List<Car>();
            goalradius = 45;
        }

        public void UpdateIO()
        {
            if(Game1.mo_states.New.RightButton == ButtonState.Pressed )
            {
                for(int i = 0; i < lines.Count; i++)
                {
                    if (FindDistanceToSegment(Game1.mo_states.New.Position.ToVector2(), lines[i].start.ToVector2(), lines[i].end.ToVector2()) < 20)
                    {
                        lines.RemoveAt(i);
                        i--;
                    }
                }
               
            }

            if (Game1.kb_states.New.IsKeyUp(Keys.LeftControl))
            {
                if (Game1.mo_states.New.LeftButton == ButtonState.Pressed)
                {

                    if (IsDrawing)
                    {

                        Point dif = posA - Game1.mo_states.New.Position;

                        if (dif.ToVector2().Length() > 20)
                        {
                            posB = Game1.mo_states.New.Position;
                            lines.Add(new Line(posA, posB));
                            posA = Game1.mo_states.New.Position;
                        }

                    }

                }
                if (Game1.mo_states.IsLeftButtonToggleOn() && DrawingEnabled)
                {
                    if (IsDrawing)
                    {
                        posB = Game1.mo_states.New.Position;
                        lines.Add(new Line(posA, posB));
                    }
                    posA = Game1.mo_states.New.Position;
                    IsDrawing = true;
                }
                if (Game1.mo_states.IsRightButtonToggleOn() && IsDrawing)
                {
                    IsDrawing = false;
                }
            }
            else
            {
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    startpos = Game1.mo_states.New.Position.ToVector2();
                }
                else if (Game1.mo_states.IsLeftButtonToggleOff())
                {
                    Vector2 dir = Game1.mo_states.New.Position.ToVector2() - startpos;
                    startdir = Car.RotationFromDir(Vector2.Normalize(dir));
                }
            }
            if (Game1.mo_states.IsMiddleButtonToggleOff())
            {
                goal = new Rectangle(new Point(Game1.mo_states.New.Position.X - 15, Game1.mo_states.New.Position.Y - 15), new Point(30));
                goalpos = goal.Location.ToVector2() + new Vector2(15);
            }
        }

        public void Update()
        {
            // Simulate Cars
            for (int i = 0; i < cars.Count; ++i)
                cars[i].Update();
        }

        public void GenerateNew(int num)
        {
            cars.Clear();
            for (int i = 0; i < num; ++i)
            {
                cars.Add(new Car(startpos));
                cars[i].rot = startdir;
                //cars[i].rot = r.Next(0, 10000) * 0.003f;
            }
        }

        public void ResetCars(float mutation_strength)
        {
            // Get best Car
            int bestID = 0;
            float bestDist = -9999.0f;
            int bestdrivingtime = 9999999;
            bool IsFinished = cars.Exists(x => x.State == Car.FINISHED);
            if(IsFinished)
            {
                Car[] curcars = cars.Where(x => x.State == Car.FINISHED).ToArray();
                for (int i = 0; i < curcars.Length; ++i)
                {
                    if (curcars[i].driving_time < bestdrivingtime)
                    {
                        bestdrivingtime = curcars[i].driving_time;
                        bestID = i;
                    }
                }
                bestID = cars.IndexOf(curcars[bestID]);
            }
            else
            {
                for (int i = 0; i < cars.Count; ++i)
                {
                    if (cars[i].total_dist > bestDist)
                    {
                        bestDist = cars[i].total_dist;
                        bestID = i;
                    }
                }
            }


            for(int i = 0; i < cars.Count; ++i)
            {
                if (i != bestID)
                {
                    cars[bestID].ste_network.CopyTo2(cars[i].ste_network);
                    cars[bestID].gas_network.CopyTo2(cars[i].gas_network);
                    cars[i].ste_network.Mutate(mutation_strength);
                    cars[i].gas_network.Mutate(mutation_strength);
                }
            }

            for (int i = 0; i < cars.Count; ++i)
            {
                cars[i].Reset(startpos, startdir);
            }
        }

        

        public void Draw(SpriteBatch spritebatch)
        {
            for (int i = 0; i < cars.Count; ++i)
                cars[i].Draw(spritebatch);

            if (IsDrawing)
            {
                spritebatch.DrawLine(posA.X, posA.Y, Game1.mo_states.New.Position.X, Game1.mo_states.New.Position.Y, Color.Black, 2);
            }
            for (int i = 0; i < lines.Count; i++)
            {
                spritebatch.DrawLine(lines[i].start.X, lines[i].start.Y, lines[i].end.X, lines[i].end.Y, Color.Red, 3);
            }
            spritebatch.DrawFilledRectangle(goal, Color.Chartreuse);
            spritebatch.DrawHollowRectangle(new Rectangle(startpos.ToPoint() - new Point(4), new Point(8)), Color.Bisque, 2);
            spritebatch.DrawLine(startpos.ToPoint(), (Car.DirFromRotation(startdir) * 50).ToPoint() + startpos.ToPoint(), Color.Wheat);




        }

    }
}
