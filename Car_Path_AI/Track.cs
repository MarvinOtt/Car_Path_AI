﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Path_AI
{
    public class Track
    {
        public List<Car> cars;
        Vector2 startpos, goalpos;
        float goalradius, startdir;

        public bool DrawingEnabled = true;
        public bool IsDrawing;
        Point posA, posB;
        public List<Line> lines;
        Rectangle goal;
        public Track()
        {
            lines = new List<Line>();
            cars = new List<Car>();
        }

        public void Update()
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
            if(Game1.mo_states.IsMiddleButtonToggleOff())
            {
               
            }
            // Simulate Cars
            for (int i = 0; i < cars.Count; ++i)
                cars[i].Update();
        }

        public void ResetCars()
        {
            for(int i = 0; i < cars.Count; ++i)
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

        }

    }
}
