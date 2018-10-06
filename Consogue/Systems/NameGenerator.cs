using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;

namespace Consogue.Systems
{
    public static class NameGenerator
    {
        public readonly static string[] lowLevelEquipmentTypes = new string[] {
            "Leather",
            "Chitin",
            "Bug",
            "Chain",
            "Wool"
        };
        private readonly static string[] consonants = new string[]
        {
            "b",
            "c",
            "d",
            "f",
            "g",
            "h",
            "j",
            "k",
            "l",
            "m",
            "n",
            "p",
            "q",
            "r",
            "s",
            "t",
            "v",
            "w",
            "x",
            "y",
            "z"
        };
        private readonly static string[] vowels = new string[] { "a", "e", "i", "o", "u" };
        // different lexical structures for the syllables
        private readonly static string[] monsterFirstSyllables = new string[]
        {
            Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(vowels).ToString(),
            Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(vowels).ToString(),
            Utilities.getRandomArrayItem(vowels).ToString() + Utilities.getRandomArrayItem(consonants).ToString(),
            Utilities.getRandomArrayItem(vowels).ToString(),
            Utilities.getRandomArrayItem(consonants).ToString(),
        };
        private readonly static string[] monsterSecondSyllables = new string[]
        {
            Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(vowels).ToString(),
            Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(vowels).ToString(),
            Utilities.getRandomArrayItem(vowels).ToString() + Utilities.getRandomArrayItem(consonants).ToString(),
            Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(consonants).ToString(),
            Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(consonants).ToString() + Utilities.getRandomArrayItem(vowels).ToString(),
        };
        /// <summary>
        /// Will generate a single name for a monster.
        /// </summary>
        public static string GenerateMonsterName()
        {
            string name = Utilities.getRandomArrayItem(monsterFirstSyllables).ToString() + Utilities.getRandomArrayItem(monsterSecondSyllables).ToString();
            if (Dice.Roll("1D100") > 50)
            {
                name = Utilities.getRandomArrayItem(monsterFirstSyllables).ToString() + Utilities.getRandomArrayItem(monsterSecondSyllables).ToString() + Utilities.getRandomArrayItem(monsterFirstSyllables).ToString();
            }
            // return the name with the first letter capitalized
            return name.Substring(0,1).ToUpper() + name.Substring(1);
        }
    }
}
