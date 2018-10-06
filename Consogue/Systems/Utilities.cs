using RLNET;
using RogueSharp;
using System;

namespace Consogue.Systems
{
    /// <summary>
    /// Usually meant for pure static functions.
    /// </summary>
    public static class Utilities
    {
        private static Random random = new Random();
        public static Object getRandomArrayItem(Object[] array)
        {
            return array[random.Next(0, array.Length - 1)];
        }
    }
}
