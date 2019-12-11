using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Car_Path_AI
{
    public class FileHandler
    {
        public FileHandler()
        {

        }
        public static void SaveTrack()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                try
                {
                    string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
                    System.IO.Directory.CreateDirectory(savepath);
                    dialog.InitialDirectory = savepath;
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Error while trying to create Save folder: {0}", exp);
                }
                dialog.CheckPathExists = false;
                dialog.CheckFileExists = false;
                dialog.Title = "Save Track";
                dialog.Filter = "TRC files (*.trc)|*.trc|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;


                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = dialog.FileName;
                    try
                    {
                        FileStream stream = new FileStream(filename, FileMode.Create);
                        List<byte> bytestosave = new List<byte>();
                        int compcount = 0;
                        int wirecount = 0;
                        byte[] bytearray;

                        #region Save Track
                        Track track = Game1.track;
                        stream.Write(BitConverter.GetBytes(track.lines.Count), 0, 4);


                        for (int i = 0; i < track.lines.Count; ++i)
                        {
                            stream.Write(BitConverter.GetBytes(track.lines[i].start.X), 0, 4);
                            stream.Write(BitConverter.GetBytes(track.lines[i].start.Y), 0, 4);
                            stream.Write(BitConverter.GetBytes(track.lines[i].end.X), 0, 4);
                            stream.Write(BitConverter.GetBytes(track.lines[i].end.Y), 0, 4);
                        }

                        stream.Write(BitConverter.GetBytes(track.startpos.X), 0, 4);
                        stream.Write(BitConverter.GetBytes(track.startpos.Y), 0, 4);
                        stream.Write(BitConverter.GetBytes(track.startdir), 0, 4);
                        stream.Write(BitConverter.GetBytes(track.goalpos.X), 0, 4);
                        stream.Write(BitConverter.GetBytes(track.goalpos.Y), 0, 4);

                        
                        #endregion


                        stream.Close();
                        stream.Dispose();
                        Console.WriteLine("Saving suceeded. Filename: {0}", filename);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Saving failed: {0}", exp);
                        System.Windows.Forms.MessageBox.Show("Saving failed", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public static void LoadTrack()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            try
            {
                string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
                System.IO.Directory.CreateDirectory(savepath);
                dialog.InitialDirectory = savepath;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while trying to create Save folder: {0}", exp);
            }

            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Title = "Select Track to Open";
            dialog.Filter = "TRC files (*.trc)|*.trc|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = false;
            DialogResult dd = dialog.ShowDialog();
            if (dd == DialogResult.OK)
            {
                string filename = dialog.FileName;

                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Open);

                    byte[] intbuffer = new byte[4];
                    Game1.track.lines.Clear();
                    stream.Read(intbuffer, 0, 4);
                    int linecount = BitConverter.ToInt32(intbuffer, 0);

                    for(int i = 0; i < linecount; ++i)
                    {
                        stream.Read(intbuffer, 0, 4);
                        int sX = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        int sY = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        int eX = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        int eY = BitConverter.ToInt32(intbuffer, 0);
                        Game1.track.lines.Add(new Line(new Point(sX, sY), new Point(eX, eY)));
                    }
                    stream.Read(intbuffer, 0, 4);
                    Game1.track.startpos.X = BitConverter.ToSingle(intbuffer, 0);
                    stream.Read(intbuffer, 0, 4);
                    Game1.track.startpos.Y = BitConverter.ToSingle(intbuffer, 0);
                    stream.Read(intbuffer, 0, 4);
                    Game1.track.startdir = BitConverter.ToSingle(intbuffer, 0);
                    stream.Read(intbuffer, 0, 4);
                    Game1.track.goalpos.X = BitConverter.ToSingle(intbuffer, 0);
                    stream.Read(intbuffer, 0, 4);
                    Game1.track.goalpos.Y = BitConverter.ToSingle(intbuffer, 0);

                    Game1.track.goal = new Rectangle(new Point((int)Game1.track.goalpos.X - 15, (int)Game1.track.goalpos.Y - 15), new Point(30));


                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Loading failed:\n{0}", exp);
                    System.Windows.Forms.MessageBox.Show("Loading failed:\n" + exp.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            dialog.Dispose();
        }

        public static void SaveNetw()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                try
                {
                    string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
                    System.IO.Directory.CreateDirectory(savepath);
                    dialog.InitialDirectory = savepath;
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Error while trying to create Save folder: {0}", exp);
                }
                dialog.CheckPathExists = false;
                dialog.CheckFileExists = false;
                dialog.Title = "Save Track";
                dialog.Filter = "NTW files (*.ntw)|*.ntw|All files (*.*)|*.*";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;


                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filename = dialog.FileName;
                    try
                    {
                        FileStream stream = new FileStream(filename, FileMode.Create);
                        List<byte> bytestosave = new List<byte>();
                        int compcount = 0;
                        int wirecount = 0;
                        byte[] bytearray;

                        #region Save Network
                        Track track = Game1.track;
                        //stream.Write(BitConverter.GetBytes(track.lines.Count), 0, 4);
                        for (int i = 0; i < track.RecentBeststeer.nodes.Length - 1; i++)
                        {
                            for(int j = 0; j < track.RecentBeststeer.nodes[i].Length; j++)
                            {
                                for(int k = 0; k < track.RecentBeststeer.nodes[i][j].strengths.Length; k++)
                                    stream.Write(BitConverter.GetBytes(track.RecentBeststeer.nodes[i][j].strengths[k]), 0, 4);
                            }
                           
                        }

                        for (int i = 0; i < track.RecentBestspeed.nodes.Length - 1; i++)
                        {
                            for (int j = 0; j < track.RecentBestspeed.nodes[i].Length; j++)
                            {
                                for (int k = 0; k < track.RecentBestspeed.nodes[i][j].strengths.Length; k++)
                                    stream.Write(BitConverter.GetBytes(track.RecentBestspeed.nodes[i][j].strengths[k]), 0, 4);
                            }

                        }


                        //for (int i = 0; i < track.lines.Count; ++i)
                        //{
                        //    stream.Write(BitConverter.GetBytes(track.lines[i].start.X), 0, 4);
                        //    stream.Write(BitConverter.GetBytes(track.lines[i].start.Y), 0, 4);
                        //    stream.Write(BitConverter.GetBytes(track.lines[i].end.X), 0, 4);
                        //    stream.Write(BitConverter.GetBytes(track.lines[i].end.Y), 0, 4);
                        //}

                        //stream.Write(BitConverter.GetBytes(track.startpos.X), 0, 4);
                        //stream.Write(BitConverter.GetBytes(track.startpos.Y), 0, 4);
                        //stream.Write(BitConverter.GetBytes(track.startdir), 0, 4);
                        //stream.Write(BitConverter.GetBytes(track.goalpos.X), 0, 4);
                        //stream.Write(BitConverter.GetBytes(track.goalpos.Y), 0, 4);


                        #endregion


                        stream.Close();
                        stream.Dispose();
                        Console.WriteLine("Saving suceeded. Filename: {0}", filename);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Saving failed: {0}", exp);
                        System.Windows.Forms.MessageBox.Show("Saving failed", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public static void LoadNetw()
        {
            Track track = Game1.track;
            OpenFileDialog dialog = new OpenFileDialog();
            try
            {
                string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
                System.IO.Directory.CreateDirectory(savepath);
                dialog.InitialDirectory = savepath;
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error while trying to create Save folder: {0}", exp);
            }

            dialog.Multiselect = false;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Title = "Select Track to Open";
            dialog.Filter = "NTW files (*.ntw)|*.ntw|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = false;
            DialogResult dd = dialog.ShowDialog();
            if (dd == DialogResult.OK)
            {
                string filename = dialog.FileName;

                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Open);

                    byte[] floatbuffer = new byte[4];
                    
                    

                    for (int i = 0; i < track.RecentBeststeer.nodes.Length - 1; i++)
                    {
                        for (int j = 0; j < track.RecentBeststeer.nodes[i].Length; j++)
                        {
                            for (int k = 0; k < track.RecentBeststeer.nodes[i][j].strengths.Length; k++)
                            {
                                stream.Read(floatbuffer, 0, 4);
                                track.RecentBeststeer.nodes[i][j].strengths[k] = BitConverter.ToSingle(floatbuffer, 0);
                            }
                        }

                    }

                    for (int i = 0; i < track.RecentBestspeed.nodes.Length - 1; i++)
                    {
                        for (int j = 0; j < track.RecentBestspeed.nodes[i].Length; j++)
                        {
                            for (int k = 0; k < track.RecentBestspeed.nodes[i][j].strengths.Length; k++)
                            {
                                stream.Read(floatbuffer, 0, 4);
                                track.RecentBestspeed.nodes[i][j].strengths[k] = BitConverter.ToSingle(floatbuffer, 0);
                            }
                        }

                    }

                    for(int i = 0; i < track.cars.Count; i++)
                    {
                        track.cars[i].Reset(track.startpos, track.startdir);
                        track.RecentBeststeer.CopyTo2(track.cars[i].ste_network);
                        track.RecentBestspeed.CopyTo2(track.cars[i].gas_network);
                        track.cars[i].ste_network.Mutate(Game1.mutation_strength);
                        track.cars[i].gas_network.Mutate(Game1.mutation_strength);
                    }
                    

                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Loading failed:\n{0}", exp);
                    System.Windows.Forms.MessageBox.Show("Loading failed:\n" + exp.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            dialog.Dispose();
        }
    }
}
