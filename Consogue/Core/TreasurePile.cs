using Consogue.Interfaces;

namespace Consogue.Core
{
    public class ITreasurePile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public ITreasure Treasure { get; set; }


        public ITreasurePile(int x, int y, ITreasure treasure)
        {
            X = x;
            Y = y;
            Treasure = treasure;

            IDrawable drawableTreasure = treasure as IDrawable;
            if (drawableTreasure != null)
            {
                drawableTreasure.X = x;
                drawableTreasure.Y = y;
            }
        }
    }
}
