using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaycasterEngine
{
    internal class Screen
    {
        public Point Position { get; set; }
        public Point Dimentions { get; set; }
        public int RayCount { get; set; }
        public int RayWidth { get; set; }

        public SpriteBatch spriteBatch { get; set; }

        public Screen(Point Dimentions, SpriteBatch _spritebatch)
        {
            this.spriteBatch = _spritebatch;
            this.Dimentions = Dimentions;

            Position = new Point(0,0);
            RayWidth = 1;
            RayCount = Dimentions.X / RayWidth;
        }
    }
}
