using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Path_AI
{
    public struct Node
    {
        public float input;
        public float[] strengths;

        public Node(int strength_count)
        {
            input = 0;
            strengths = new float[strength_count];
            for (int i = 0; i < strength_count; ++i)
                strengths[i] = Game1.r.Next(-10000, 10000) * 0.0001f;
        }
    }

    public class NeuralNetwork
    {

        public int layer_count;
        public Node[][] nodes;


        public NeuralNetwork(int[] alllayer_count)
        {
            layer_count = alllayer_count.Length;
            nodes = new Node[layer_count][];
            for(int i = 0; i < layer_count; ++i)
            {
                nodes[i] = new Node[alllayer_count[i]];
            }
            for (int i = 0; i < layer_count - 1; ++i)
            {
                for(int j = 0; j < nodes[i].Length; ++j)
                {
                    nodes[i][j] = new Node(nodes[i + 1].Length);
                }
            }

        }

        public void Simulate()
        {
            for (int i = 1; i < layer_count; ++i)
            {
                for (int j = 0; j < nodes[i].Length; ++j)
                {
                    nodes[i][j].input = 0;
                }
            }
            for (int i = 0; i < layer_count - 1; ++i)
            {
                for (int j = 0; j < nodes[i].Length; ++j)
                {
                    for (int k = 0; k < nodes[i + 1].Length; ++k)
                    {
                        nodes[i + 1][k].input += nodes[i][j].input * nodes[i][j].strengths[k];
                    }
                }
            }
        }

        public void Mutate(float strength)
        {
            for (int i = 0; i < layer_count - 1; ++i)
            {
                for (int j = 0; j < nodes[i].Length; ++j)
                {
                    for (int k = 0; k < nodes[i + 1].Length; ++k)
                    {
                        nodes[i][j].strengths[k] += (Game1.r.Next(-10000, 10000) * 0.0001f) * strength;
                    }
                }
            }
        }

        public void CopyTo(NeuralNetwork dest)
        {
            for(int i = 0; i < layer_count; ++i)
            {
                Array.Copy(nodes[i], dest.nodes[i], nodes[i].Length);
            }
        }
    }
}
