using RogueSharp.DiceNotation;
using System;

namespace Consogue.Equipment
{
    public class HeadEquipment : Core.Equipment
    {
        private static string[] lowLevelHeadItems = new string[] {
            "helmet",
            "hat",
            "mask",
        };
        public static HeadEquipment None()
        {
            return new HeadEquipment { Name = "None" };
        }
        private static Random random = new Random();
        public static HeadEquipment GenerateNextLowLevel()
        {
            return new HeadEquipment()
            {
                Defense = Dice.Roll("1D3"),
                DefenseChance = Dice.Roll("10D4"),
                Name = lowLevelBase[random.Next(0, lowLevelBase.Length - 1)] + " " + lowLevelHeadItems[random.Next(0, lowLevelHeadItems.Length - 1)]
            };
        }
    }
}