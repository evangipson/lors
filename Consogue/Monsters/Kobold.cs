using Consogue.Core;
using Consogue.Equipment;
using Consogue.Items;
using Consogue.Systems;
using RogueSharp.DiceNotation;

namespace Consogue.Monsters
{
    public class Kobold : Monster
    {
        /// <summary>
        /// Will create and return a Kobold of the level passed into the method.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Kobold Create(int level)
        {
            int health = Dice.Roll("2D5");
            Kobold kobold = new Kobold
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = Colors.DbBrightWood,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = NameGenerator.GenerateMonsterName() + ", Kobold",
                Speed = 14,
                Symbol = 'k'
            };
            // Low chance they'll have a healing potion.
            if (Dice.Roll("1D6") >= 5)
            {
                kobold.items.Add(new HealingPotion());
            }
            // Give some Kobolds helmets
            if(Dice.Roll("1D6") >= 5)
            {
                kobold.Head = HeadEquipment.GenerateNextLowLevel();
            }
            // Give some Kobolds body armor
            if (Dice.Roll("1D6") >= 5)
            {
                kobold.Body = BodyEquipment.GenerateNextLowLevel();
            }
            // Give some Kobolds feet and hand armor rarely
            if(Dice.Roll("1D6") > 5)
            {
                kobold.Feet = FeetEquipment.GenerateNextLowLevel();
                kobold.Hand = HandEquipment.GenerateNextLowLevel();
            }
            return kobold;
        }
    }
}