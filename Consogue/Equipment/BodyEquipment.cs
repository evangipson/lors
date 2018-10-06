using Consogue.Systems;
using RogueSharp.DiceNotation;

namespace Consogue.Equipment
{
    public class BodyEquipment : Core.Equipment
    {
        private static string[] bodyEquipmentNames = new string[] {
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
        public static BodyEquipment GenerateNextLowLevel()
        {
            return new BodyEquipment()
            {
                Defense = Dice.Roll("1D4"),
                DefenseChance = Dice.Roll("10D4"),
                Name = Utilities.getRandomArrayItem(NameGenerator.lowLevelEquipmentTypes) +
                       " " +
                       Utilities.getRandomArrayItem(bodyEquipmentNames)
            };
        }
    }
}