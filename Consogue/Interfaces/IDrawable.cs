using RLNET;
using RogueSharp;

namespace Consogue.Interfaces
{
    public interface IDrawable
    {
        RLColor Color { get; set; }
        char Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }
        /// <summary>
        /// This function will be called everytime an IDrawable instantiation
        /// needs to draw to the console.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="map"></param>
        void Draw(RLConsole console, IMap map);
    }
}
