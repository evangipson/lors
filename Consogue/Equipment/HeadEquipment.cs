using Consogue.Systems;
using RogueSharp.DiceNotation;

namespace Consogue.Equipment
{
    public class HeadEquipment : Core.Equipment
    {
        private static string[] headEquipmentNames = new string[] {
            "helmet",
            "hat",
            "mask",
        };
        public static HeadEquipment None()
        {
            return new HeadEquipment { Name = "None" };
        }
        public static HeadEquipment GenerateNextLowLevel()
        {
            return new HeadEquipment()
            {
                Defense = Dice.Roll("1D3"),
                DefenseChance = Dice.Roll("10D4"),
                Name = Utilities.getRandomArrayItem(NameGenerator.lowLevelEquipmentTypes) +
                       " " +
                       Utilities.getRandomArrayItem(headEquipmentNames)
            };
        }
    }
}