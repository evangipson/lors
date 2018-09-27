using Consogue.Core;
using Consogue.Interfaces;
using RLNET;
using RogueSharp;

namespace Consogue.Tiles
{
    public class Grass : Plant
    {
        public Grass(int x, int y)
        {
            Symbol = '~';
            X = x;
            Y = y;
        }

        /// <summary>
        /// Will draw grass.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="map"></param>
        public override void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }
            
            if (map.IsInFov(X, Y))
            {
                Color = Colors.DbGrass;
            }
            else
            {
                Color = Colors.DbOldStone;
            }

            console.Set(X, Y, Color, null, Symbol);
        }
    }
}
