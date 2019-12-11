﻿using Microsoft.Xna.Framework;
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
    public class Car
    {
        public const int DRIVING = 0;
        public const int CRASHED = 1;
        public const int FINISHED = 2;

        public static float sens_length = 200.0f;

        public NeuralNetwork ste_network, gas_network;
        bool IsIntersect = false;
        public Vector2 pos;
        public float speed, acl, rot, steering, steering_acl, total_dist;
        public double sens_dist_FL, sens_dist_FR, sens_dist_R, sens_dist_L, sens_dist_F, sens_dist_BL, sens_dist_BR;
        public float[] sens_dist;
        public int State;
        public int driving_time;
        public Matrix matr;
        Vector2 v1, v2, v3, v4, v5, v6;
        Line l1, l2, l3, l4;


        public static Texture2D tex;


        public Car(Vector2 pos)
        {
            if (tex == null)
                tex = Game1.content.Load<Texture2D>("CarTexture");
            this.pos = pos;
            int[] nodeanz = new int[] { 2, 4, 4, 1 };
            ste_network = new NeuralNetwork(nodeanz);
            nodeanz = new int[] { 2, 5, 5, 1 };
            gas_network = new NeuralNetwork(nodeanz);
            sens_dist = new float[20];
        }

        public void Reset(Vector2 pos, float rot)
        {
            this.pos = pos;
            this.rot = rot;
            acl = driving_time = 0;
            steering = steering_acl = total_dist = 0;
            speed = 2.0f;
            State = DRIVING;
        }

        public void Update()
        {
            if(State == DRIVING)
            {
                driving_time++;

                // Check for Wall Collisions

                // Check for Finish

                if (Game1.kb_states.New.IsKeyDown(Keys.W))
                    acl = 1;
                else if (Game1.kb_states.New.IsKeyDown(Keys.S))
                    acl = -1;
                else
                    acl = 0;

                if (Game1.kb_states.New.IsKeyDown(Keys.D))
                    steering_acl = 1;
                else if (Game1.kb_states.New.IsKeyDown(Keys.A))
                    steering_acl = -1;
                else
                    steering_acl = 0;





                //ste_network.nodes[0][0].input = speed;
                //ste_network.nodes[0][2].input = 0;
                //network.nodes[0][2].input = 1;
                //network.nodes[0][3].input = -1;
                //ste_network.nodes[0][0].input = 1.01f * (sens_dist_FL - sens_dist_FR) / sens_length;
                //network.nodes[0][3].input = 1.0f - (sens_dist_FR / sens_length);
                //ste_network.nodes[0][3].input = 1.0f / (1.0f + speed);
                //ste_network.nodes[0][0].input = 1.00f * (sens_dist_L - sens_dist_R) / sens_length;
                //network.nodes[0][6].input = 1.0f - (sens_dist_R / sens_length);
                //network.nodes[0][5].input = 1.0f - (sens_dist_F / sens_length);



                //ste_network.nodes[0][0].input = 1.0f - (sens_dist_L / sens_length);
                //ste_network.nodes[0][1].input = steering;
                //for (int i = 0; i < 10; ++i)
                //{
                //    ste_network.nodes[0][2 + i].input = 1.0f - (sens_dist[i] / sens_length);
                //}
                //ste_network.Simulate();
                //float steeringL = ste_network.nodes[3][0].input;

                //ste_network.nodes[0][0].input = 1.0f - (sens_dist_R / sens_length);
                //ste_network.nodes[0][1].input = steering;
                //for (int i = 10; i < 20; ++i)
                //{
                //    ste_network.nodes[0][2 + i - 10].input = 1.0f - (sens_dist[i] / sens_length);// (sens_dist[i] - sens_dist[9 - i]) / sens_length;
                //}
                //ste_network.Simulate();
                //float steeringR = ste_network.nodes[3][0].input;

                //gas_network.nodes[0][0].input = speed;
                //gas_network.nodes[0][1].input = 1.0f - (sens_dist_F / sens_length);
                //gas_network.nodes[0][2].input = 1.0f - (((sens_dist_FL + sens_dist_FR) * 0.5f) / sens_length);

                ste_network.nodes[0][0].input = 1.0f - ((float)sens_dist_FL / sens_length);
                ste_network.nodes[0][1].input = 1.0f - ((float)sens_dist_BL / sens_length);
                //ste_network.nodes[0][2].input = steering;
                ste_network.Simulate();
                float steeringL = ste_network.nodes[3][0].input;

                ste_network.nodes[0][0].input = 1.0f - ((float)sens_dist_FR / sens_length);
                ste_network.nodes[0][1].input = 1.0f - ((float)sens_dist_BR / sens_length);
                //ste_network.nodes[0][2].input = steering;
                ste_network.Simulate();
                float steeringR = ste_network.nodes[3][0].input;

                gas_network.nodes[0][0].input = 1.0f;
                gas_network.nodes[0][1].input = 1.0f - (((float)(sens_dist_FL + sens_dist_FR) * 0.5f) / sens_length);

                gas_network.Simulate();

                acl = gas_network.nodes[3][0].input;
                steering_acl = steeringR - steeringL;
                float newsteering = MathHelper.Clamp(steering_acl, -1, 1);
                float steeringdif = MathHelper.Clamp(newsteering - steering, -0.04f, 0.04f);
                steering += steeringdif;
                //steering += MathHelper.Clamp(steering_acl, -1, 1) * 0.05f;
                steering = MathHelper.Clamp(steering, -1, 1);
                acl = MathHelper.Clamp(acl, -5, 5);
                rot += steering * 0.015f * speed;
                speed = acl;// * 0.01f;
                Vector2 dir = DirFromRotation(rot);
                pos += dir * speed;
                total_dist += (dir * speed).Length();
                speed *= 0.99f;

                matr = Matrix.CreateRotationZ(rot);
                Vector2 v = new Vector2(-40, -23);
                v1 = pos + Vector2.Transform(v, matr);
                v2 = pos + Vector2.Transform(v + new Vector2(117, 0), matr);
                v3 = pos + Vector2.Transform(v + new Vector2(117, 46), matr);
                v4 = pos + Vector2.Transform(v + new Vector2(0, 46), matr);
                v5 = pos + Vector2.Transform(v + new Vector2(80, 0), matr);
                v6 = pos + Vector2.Transform(v + new Vector2(80, 46), matr);

                sens_dist_FL = CalculateShortestDist(v2, sens_length);
                sens_dist_FR = CalculateShortestDist(v3, sens_length);
                sens_dist_BR = CalculateShortestDist(v4, sens_length);
                sens_dist_BL = CalculateShortestDist(v1, sens_length);

                //for (int i = 0; i < 20; ++i)
                //{
                //    float fac = (float)(Math.PI * 2.0f) / 10.0f;
                //    Vector2 curdir = DirFromRotation(rot - 1.0f + i * 0.1f);
                //    sens_dist[i] = CalculateDist(pos, pos + curdir * sens_length, sens_length);
                //}

                //Vector2 dir_main = DirFromRotation(rot);
                //Vector2 dir_FL = DirFromRotation(rot - 0.5f);
                //Vector2 dir_FR = DirFromRotation(rot + 0.5f);
                //Vector2 dir_L = DirFromRotation(rot - (float)Math.PI / 2);
                //Vector2 dir_R = DirFromRotation(rot + (float)Math.PI / 2);
                //sens_dist_FL = CalculateDist(v2, v2 + dir_FL * sens_length, sens_length);
                //sens_dist_FR = CalculateDist(v3, v3 + dir_FR * sens_length, sens_length);
                //sens_dist_L = CalculateDist(v5, v5 + dir_L * sens_length, sens_length);
                //sens_dist_R = CalculateDist(v6, v6 + dir_R * sens_length, sens_length);
                //sens_dist_F = CalculateDist(pos + dir_main * 77.0f, (pos + dir_main * 77.0f) + dir_main * sens_length, sens_length);


                //Check Hitbox

                l1 = new Line(v1.ToPoint(), v2.ToPoint());
                l2 = new Line(v2.ToPoint(), v3.ToPoint());
                l3 = new Line(v3.ToPoint(), v4.ToPoint());
                l4 = new Line(v4.ToPoint(), v1.ToPoint());
                IsIntersect = false;
                for (int i = 0; i < Game1.track.lines.Count; ++i)
                {
                    Line cur = Game1.track.lines[i];
                    if (doIntersect(cur, l1) || doIntersect(cur, l2) || doIntersect(cur, l3) || doIntersect(cur, l4))
                    {
                        IsIntersect = true;
                        break;
                    }
                }
                if(IsIntersect)
                {
                    State = CRASHED;
                }
                if((pos - Game1.track.goalpos).Length() < Game1.track.goalradius)
                {
                    State = FINISHED;
                }
                
            }
            //rot += 0.01f;
        }

        public float CalculateDist(Vector2 s1, Vector2 e1, float maxlength)
        {
            float length = maxlength;
            for (int i = 0; i < Game1.track.lines.Count; ++i)
            {
                Vector2 intersecpoint = FindIntersection(s1, e1, Game1.track.lines[i].start.ToVector2(), Game1.track.lines[i].end.ToVector2());
                float dist = (intersecpoint - s1).Length();
                if (dist < length)
                {
                    length = dist;
                }
            }
            return length;
        }
        public double CalculateShortestDist(Vector2 s, double maxlength)
        {
            double length = maxlength;
            for (int i = 0; i < Game1.track.lines.Count; ++i)
            {
                double dist = FindDistanceToSegment(s, Game1.track.lines[i].start.ToVector2(), Game1.track.lines[i].end.ToVector2());
                if (dist < length)
                {
                    length = dist;
                }
            }
            return length;
        }

        public static float RotationFromDir(Vector2 dir)
        {
            float res = (float)(Math.Atan2(dir.Y, dir.X));
            return res;
        }
        public static Vector2 DirFromRotation(float rot)
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
            float trans = 1.0f;
            if (State != 0)
                trans = 0.2f;

            spritebatch.Draw(tex, new Rectangle(new Point((int)pos.X, (int)pos.Y), new Point(117, 46)), new Rectangle(0, 0, 117, 46), Color.White * trans, (rot), new Vector2(40, 23), SpriteEffects.None, 0);
            //spritebatch.DrawFilledRectangle(new Rectangle((int)pos.X - 3, (int)pos.Y - 3, 6, 6), Color.Red);

            Color col = Color.Blue;
            if (IsIntersect)
                col = Color.Green;

            //spritebatch.DrawFilledRectangle(new Rectangle((int)v1.X - 3, (int)v1.Y - 3, 6, 6), col);
            //spritebatch.DrawFilledRectangle(new Rectangle((int)v2.X - 3, (int)v2.Y - 3, 6, 6), col);
            //spritebatch.DrawFilledRectangle(new Rectangle((int)v3.X - 3, (int)v3.Y - 3, 6, 6), col);
            //spritebatch.DrawFilledRectangle(new Rectangle((int)v4.X - 3, (int)v4.Y - 3, 6, 6), col);
            //spritebatch.DrawLine(l1.start, l1.end, col, 3);
            //spritebatch.DrawLine(l2.start, l2.end, col, 3);
            //spritebatch.DrawLine(l3.start, l3.end, col, 3);
            //spritebatch.DrawLine(l4.start, l4.end, col, 3);
            if (State == 0)
            {
                //for (int i = 0; i < 10; ++i)
                //{
                //    float fac = (float)(Math.PI * 2.0f) / 10.0f;
                //    Vector2 curdir = DirFromRotation(rot + i * fac);
                //    sens_dist[i] = CalculateDist(pos, pos + curdir * sens_length, sens_length);
                //    spritebatch.DrawLine(pos.ToPoint(), (pos + curdir * sens_dist[i]).ToPoint(), Color.Orange, 1);
                //}

                Game1.basiceffect.DrawCircle(v2, (float)sens_dist_FL);
                Game1.basiceffect.DrawCircle(v3, (float)sens_dist_FR);
                Game1.basiceffect.DrawCircle(v4, (float)sens_dist_BR);
                Game1.basiceffect.DrawCircle(v1, (float)sens_dist_BL);

                //for (int i = 0; i < 20; ++i)
                //{
                //    float fac = (float)(Math.PI * 2.0f) / 10.0f;
                //    Vector2 curdir = DirFromRotation(rot - 1.0f + i * 0.1f);
                //    spritebatch.DrawLine(pos.ToPoint(), (pos + curdir * sens_dist[i]).ToPoint(), Color.Orange, 1);
                //}

                //Vector2 dir_main = DirFromRotation(rot);
                //spritebatch.DrawLine(v2.ToPoint(), (v2 + DirFromRotation(rot - 0.5f) * sens_dist_FL).ToPoint(), Color.Orange, 1);
                //spritebatch.DrawLine(v3.ToPoint(), (v3 + DirFromRotation(rot + 0.5f) * sens_dist_FR).ToPoint(), Color.Orange, 1);
                //spritebatch.DrawLine(v5.ToPoint(), (v5 + DirFromRotation(rot - (float)Math.PI / 2) * sens_dist_L).ToPoint(), Color.Orange, 1);
                //spritebatch.DrawLine(v6.ToPoint(), (v6 + DirFromRotation(rot + (float)Math.PI / 2) * sens_dist_R).ToPoint(), Color.Orange, 1);
                //spritebatch.DrawLine((pos + dir_main * 77.0f).ToPoint(), ((pos + dir_main * 77.0f) + dir_main * sens_dist_F).ToPoint(), Color.Orange, 1);
            }

            //v = Vector2.Transform(v, matr);
        }
    }
}
