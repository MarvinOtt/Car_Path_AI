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
        public List<Car>[] cars;
        public Vector2[] startpos, goalpos;
        public float[] startdir;
        public float goalradius;


        public int curbestID;
        public bool DrawingEnabled = true;
        public bool IsDrawing, IsCancel;
        Point posA, posB;
        public NeuralNetwork RecentBeststeer, RecentBestspeed;
        public List<Line> lines;
        public Rectangle[] goal;
        public Track()
        {
            lines = new List<Line>();
            cars = new List<Car>[Game1.maxcars];
            for(int j = 0; j < Game1.maxcars; ++j)
            {
                cars[j] = new List<Car>();
            }
            startpos = new Vector2[Game1.maxcars];
            startdir = new float[Game1.maxcars];
            goalpos = new Vector2[Game1.maxcars];
            goal = new Rectangle[Game1.maxcars];

            goalradius = 50;
            int[] nodeanz = new int[] { 4, 5, 5, 5, 1 }; //steering
            RecentBeststeer = new NeuralNetwork(nodeanz);
            nodeanz = new int[] { 3, 4, 4, 1 }; //gas
            RecentBestspeed = new NeuralNetwork(nodeanz);
        }

        public void UpdateIO()
        {
            if (!IsDrawing)
            {

                
                if (Game1.mo_states.New.RightButton == ButtonState.Pressed && !IsCancel)
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (FindDistanceToSegment(Game1.mo_states.New.Position.ToVector2(), lines[i].start.ToVector2(), lines[i].end.ToVector2()) < 20)
                        {
                            lines.RemoveAt(i);
                            i--;
                        }
                    }
                   
                }
                
            }

            if (Game1.kb_states.New.IsKeyUp(Keys.LeftControl))
            {
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
               
                if (Game1.mo_states.IsRightButtonToggleOn() && IsDrawing)
                {
                    IsCancel = true;
                    IsDrawing = false;
                }
                if (Game1.mo_states.IsRightButtonToggleOff())
                    IsCancel = false;
            }
            else
            {
                int ID = -1;
                for (int i = 0; i < 8; ++i)
                {
                    if(Game1.kb_states.New.IsKeyDown(Keys.D1 + i))
                    {
                        ID = i;
                        break;
                    }
                }
                if(ID >= 0)
                {
                    if (Game1.mo_states.IsLeftButtonToggleOn())
                    {
                        startpos[ID] = Game1.mo_states.New.Position.ToVector2();
                    }
                    else if (Game1.mo_states.IsLeftButtonToggleOff())
                    {
                        Vector2 dir = Game1.mo_states.New.Position.ToVector2() - startpos[ID];
                        startdir[ID] = Car.RotationFromDir(Vector2.Normalize(dir));
                    }
                }
                
            }
            if (Game1.mo_states.IsMiddleButtonToggleOff())
            {
                int ID = -1;
                for (int i = 0; i < 8; ++i)
                {
                    if (Game1.kb_states.New.IsKeyDown(Keys.D1 + i))
                    {
                        ID = i;
                        break;
                    }
                }
                if (ID >= 0)
                {
                    goal[ID] = new Rectangle(new Point(Game1.mo_states.New.Position.X - 15, Game1.mo_states.New.Position.Y - 15), new Point(30));
                    goalpos[ID] = goal[ID].Location.ToVector2() + new Vector2(15);
                }
            }
        }

		public void Update()
		{
			// Simulate Cars
			for (int j = 0; j < Game1.maxcars; ++j)
				for (int i = 0; i < cars[j].Count; ++i)
					cars[j][i].Update();
        }

        public void GenerateNew(int num)
        {
			for (int j = 0; j < Game1.maxcars; ++j)
				cars[j].Clear();
            for (int i = 0; i < num; ++i)
            {
				for (int j = 0; j < Game1.maxcars; ++j)
				{
					cars[j].Add(new Car(startpos[j], j));
					cars[j][i].rot = startdir[j];
				}
            }
        }

        public void ResetCars(float mutation_strength)
        {
            // Get best Car
            int bestID = 0;
			float bestDist = -9999.0f;
            float bestdrivingtime = 9999999;
            //bool IsFinished = cars.Exists(x => x.State == Car.FINISHED);
            //if(IsFinished)
            //{
            //    Car[] curcars = cars.Where(x => x.State == Car.FINISHED).ToArray();
            //    for (int i = 0; i < curcars.Length; ++i)
            //    {
            //        if (curcars[i].driving_time < bestdrivingtime)
            //        {
            //            bestdrivingtime = curcars[i].driving_time - curcars[i].penalty_points;
            //            bestID = i;
            //        }
            //    }
            //    bestID = cars.IndexOf(curcars[bestID]);
            //}
            if(true)
            {
                for (int i = 0; i < cars[0].Count; ++i)
                {
					float averagedist = 0;
					for(int j = 0; j < Game1.maxcars; ++j)
					{
						averagedist += cars[j][i].total_dist;
					}
					averagedist /= (float)Game1.maxcars;
                    if (averagedist > bestDist)
                    {
                        bestDist = averagedist;
                        bestID = i;
                    }
                }
            }

            cars[0][bestID].ste_network.CopyTo2(RecentBeststeer);
            cars[0][bestID].gas_network.CopyTo2(RecentBestspeed);
            for (int i = 0; i < cars[0].Count; ++i)
            {
                if (i != bestID)
                {
					for (int j = 0; j < Game1.maxcars; ++j)
					{
						cars[j][bestID].ste_network.CopyTo2(cars[j][i].ste_network);
						cars[j][bestID].gas_network.CopyTo2(cars[j][i].gas_network);
						cars[j][i].ste_network.Mutate(mutation_strength);
						cars[j][i].gas_network.Mutate(mutation_strength);
					}
                }
            }
			for (int i = 0; i < cars[0].Count; ++i)
			{
				for (int j = 1; j < Game1.maxcars; ++j)
				{
					cars[0][i].ste_network.CopyTo2(cars[j][i].ste_network);
					cars[0][i].gas_network.CopyTo2(cars[j][i].gas_network);
				}
			}

			for (int j = 0; j < Game1.maxcars; ++j)
			{
				for (int i = 0; i < cars[j].Count; ++i)
				{
					cars[j][i].Reset(startpos[j], startdir[j]);
				}
			}
        }

        
    
        public void Draw(SpriteBatch spritebatch)
        {
           /* int bestID = 0;
            float bestDist = -9999.0f;
            float bestdrivingtime = 9999999;
            bool IsFinished = cars.Exists(x => x.State == Car.FINISHED);
            if (IsFinished)
            {
                Car[] curcars = cars.Where(x => x.State == Car.FINISHED).ToArray();
                for (int i = 0; i < curcars.Length; ++i)
                {
                    if (curcars[i].driving_time < bestdrivingtime)
                    {
                        bestdrivingtime = curcars[i].driving_time - curcars[i].penalty_points;
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
            curbestID = bestID;

            for (int i = 0; i < cars.Count; ++i)
            {
                bool IsBest = false;
                if (i == curbestID)
                    IsBest = true;
                cars[i].Draw(spritebatch, IsBest);
            }*/
            //Zeichnen der Linien
            if (IsDrawing)
            {
                spritebatch.DrawLine(posA.X, posA.Y, Game1.mo_states.New.Position.X, Game1.mo_states.New.Position.Y, Color.Black, 2);
            }
            for (int i = 0; i < lines.Count; i++)
            {
                spritebatch.DrawLine(lines[i].start.X, lines[i].start.Y, lines[i].end.X, lines[i].end.Y, Color.Red, 3);
                spritebatch.DrawFilledRectangle(new Rectangle(lines[i].start, new Point(2)), Color.Red);
                spritebatch.DrawFilledRectangle(new Rectangle(lines[i].end, new Point(2)), Color.Red);
            }
            for(int i = 0; i < goal.Length; i++)
            {
                spritebatch.DrawFilledRectangle(goal[i], Color.Chartreuse);

            }
            for (int i = 0; i < startpos.Length; i++)
            {
                spritebatch.DrawHollowRectangle(new Rectangle(startpos[i].ToPoint() - new Point(4), new Point(8)), Color.Bisque, 2);
                spritebatch.DrawLine(startpos[i].ToPoint(), (Car.DirFromRotation(startdir[i]) * 50).ToPoint() + startpos[i].ToPoint(), Color.Wheat);
            }

        }

    }
}
