using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaycasterEngine
{
    internal class Grid
    {
        public List<List<GridSlot>> Slots;
        public List<GridSlot> SolidSlots;
        public Point Dimentions { get; set; }


        public Grid(Point Dimentions)
        {
            Random random = new Random();

            this.Dimentions = Dimentions;


            Slots = new List<List<GridSlot>>();
            SolidSlots = new List<GridSlot>();
            for (int y = 0; y < Dimentions.Y; y++)
            {
                Slots.Add(new List<GridSlot>());
                for (int x = 0; x < Dimentions.X; x++)
                {
                    if (random.Next(0, 1000) == 10)
                    {
                        Color Color = Color.Red;
                        Slots.Last().Add(new GridSlot(new Point(x, y), Color));
                        SolidSlots.Add(Slots.Last().Last());
                    }
                    else
                    {
                        Slots.Last().Add(null);
                    }
                }
            }
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                {
                    Slots[50 + y][50 + x] = new GridSlot(new Point(50 + x, 50 + y), Color.Turquoise);
                    SolidSlots.Add(Slots[50 + y][50 + x]);
                }
                    
        }
    }
    internal class GridSlot
    {
        public Point Position { get; set; }
        public Color Color { get; set; }

        public GridSlot(Point Position, Color Color)
        {
            this.Position = Position;
            this.Color = Color;
        }
    }
}
