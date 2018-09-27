using Consogue.Core;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace Consogue.Tiles
{
    public class Tree : Plant
    {
        public Tree(int x, int y)
        {
            Symbol = Dice.Roll("1d2") > 1 ? '[' : ']';
            X = x;
            Y = y;
        }

        /// <summary>
        /// Will draw the tree.
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
