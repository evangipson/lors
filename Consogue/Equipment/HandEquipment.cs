using RogueSharp.DiceNotation;
using System;

namespace Consogue.Equipment
{
    public class HandEquipment : Core.Equipment
    {
        private static string[] lowLevelHandItems = new string[] {
            "gloves",
            "gauntlets",
            "mittens"
        };
        public static HandEquipment None()
        {
            return new HandEquipment { Name = "None" };
        }
        private static Random random = new Random();
        public static HandEquipment GenerateNextLowLevel()
        {
            return new HandEquipment()
            {
                Defense = Dice.Roll("1D3"),
                DefenseChance = Dice.Roll("10D4"),
                Name = lowLevelBase[random.Next(0, lowLevelBase.Length - 1)] + " " + lowLevelHandItems[random.Next(0, lowLevelHandItems.Length - 1)]
            };
        }
    }
}