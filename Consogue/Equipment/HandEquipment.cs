using Consogue.Systems;
using RogueSharp.DiceNotation;

namespace Consogue.Equipment
{
    public class HandEquipment : Core.Equipment
    {
        private static string[] handEquipmentNames = new string[] {
            "gloves",
            "gauntlets",
            "mittens"
        };
        public static HandEquipment None()
        {
            return new HandEquipment { Name = "None" };
        }
        public static HandEquipment GenerateNextLowLevel()
        {
            return new HandEquipment()
            {
                Defense = Dice.Roll("1D3"),
                DefenseChance = Dice.Roll("10D4"),
                Name = Utilities.getRandomArrayItem(NameGenerator.lowLevelEquipmentTypes) +
                       " " +
                       Utilities.getRandomArrayItem(handEquipmentNames)
            };
        }
    }
}