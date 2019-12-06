using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car_Path_AI
{
    public class Track
    {
        List<Car> cars;
        Vector2 startpos, goalpos;
        float goalradius, startdir;

        public void Update()
        {
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

        }

    }
}
