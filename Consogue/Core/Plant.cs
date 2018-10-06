using Consogue.Interfaces;
using RLNET;
using RogueSharp;

namespace Consogue.Core
{
    /// <summary>
    /// The Plant class is meant to be inherited and used
    /// by other plants, like Grass, which this was originally
    /// developed for.
    /// </summary>
    public abstract class Plant : IDrawable
    {
        public RLColor Color { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        /// <summary>
        /// Will draw the plant, meant to be defined by the child class.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="map"></param>
        public abstract void Draw(RLConsole console, IMap map);
    }
}
