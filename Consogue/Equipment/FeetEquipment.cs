using RogueSharp.DiceNotation;
using System;

namespace Consogue.Equipment
{
    public class FeetEquipment : Core.Equipment
    {
        private static string[] lowLevelFeetItems = new string[] {
            "greaves",
            "boots",
            "shoes",
            "slippers"
        };
        public static FeetEquipment None()
        {
            return new FeetEquipment { Name = "None" };
        }
        private static Random random = new Random();
        public static FeetEquipment GenerateNextLowLevel()
        {
            return new FeetEquipment()
            {
                Defense = Dice.Roll("1D3"),
                DefenseChance = Dice.Roll("2D4"),
                Name = lowLevelBase[random.Next(0, lowLevelBase.Length - 1)] + " " + lowLevelFeetItems[random.Next(0, lowLevelFeetItems.Length - 1)]
            };
        }
    }
}