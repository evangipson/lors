using Consogue.Core;
using Consogue.Items;
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
                Name = "Kobold",
                Speed = 14,
                Symbol = 'k'
            };
            // Low chance they'll have a healing potion.
            if (Dice.Roll("1D6") >= 5)
            {
                kobold.items.Add(new HealingPotion());
            }
            return kobold;
        }
    }
}