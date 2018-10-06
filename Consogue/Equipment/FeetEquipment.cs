using Consogue.Systems;
using RogueSharp.DiceNotation;

namespace Consogue.Equipment
{
    public class FeetEquipment : Core.Equipment
    {
        private static string[] feetEquipmentNames = new string[] {
            "greaves",
            "boots",
            "shoes",
            "slippers"
        };
        public static FeetEquipment None()
        {
            return new FeetEquipment { Name = "None" };
        }
        public static FeetEquipment GenerateNextLowLevel()
        {
            return new FeetEquipment()
            {
                Defense = Dice.Roll("1D3"),
                DefenseChance = Dice.Roll("10D4"),
                Name = Utilities.getRandomArrayItem(NameGenerator.lowLevelEquipmentTypes) +
                       " " +
                       Utilities.getRandomArrayItem(feetEquipmentNames)
            };
        }
    }
}