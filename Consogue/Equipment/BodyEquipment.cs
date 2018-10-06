using RogueSharp.DiceNotation;
using System;

namespace Consogue.Equipment
{
    public class BodyEquipment : Core.Equipment
    {
        private static string[] lowLevelBodyItems = new string[] {
            "breastplate",
            "cuirass",
            "armor",
            "scarf",
            "shirt"
        };
        public static BodyEquipment None()
        {
            return new BodyEquipment { Name = "None" };
        }
        private static Random random = new Random();
        public static BodyEquipment GenerateNextLowLevel()
        {
            return new BodyEquipment()
            {
                Defense = Dice.Roll("1D4"),
                DefenseChance = Dice.Roll("10D4"),
                Name = lowLevelBase[random.Next(0, lowLevelBase.Length - 1)] + " " + lowLevelBodyItems[random.Next(0, lowLevelBodyItems.Length - 1)]
            };
        }
    }
}