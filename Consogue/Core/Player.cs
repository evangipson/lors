using Consogue.Equipment;
using Consogue.Interfaces;
using RLNET;
using System.Collections.Generic;

namespace Consogue.Core
{
    public class Player : Actor
    {
        public Player()
        {
            Attack = 2;
            AttackChance = 50;
            Awareness = 12;
            Color = Colors.Player;
            Defense = 2;
            DefenseChance = 40;
            Gold = 0;
            Health = 100;
            MaxHealth = 100;
            Name = "Rogue";
            Speed = 10;
            Symbol = '@';
        }
        /// <summary>
        /// Draws the Player's stats to the passed in console.
        /// </summary>
        /// <param name="statConsole"></param>
        public void DrawStats(RLConsole statConsole)
        {
            statConsole.Print(1, 1, $"Name:    {Name}", Colors.Text);
            statConsole.Print(1, 3, $"Health:  {Health}/{MaxHealth}", Colors.Text);
            statConsole.Print(1, 5, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text);
            statConsole.Print(1, 7, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text);
            statConsole.Print(1, 9, $"Gold:    {Gold}", Colors.Gold);
        }
        /// <summary>
        /// Will add an item to the player's inventory
        /// </summary>
        /// <param name="item"></param>
        public bool AddItem(IItem item)
        {
            if(items.Count < 10)
            {
                items.Add(item);
                return true;
            }
            else
            {
                Game.MessageLog.Add($"No more space left in {Name}'s inventory.");
            }
            return false;
        }
        public void DrawInventory(RLConsole inventoryConsole)
        {
            inventoryConsole.Print(1, 1, "Inventory", Colors.DbBrightWood);
            for(int i = 0; i < items.Count; i++)
            {
                DrawItem(items[i], inventoryConsole, i);
            }
            inventoryConsole.Print(1, 3 + (items.Count * 2), $"Head: {Head.Name}", Head == HeadEquipment.None() ? Colors.DbOldStone : Colors.DbLight);
            inventoryConsole.Print(1, 3 + (items.Count * 2) + 2, $"Body: {Body.Name}", Body == BodyEquipment.None() ? Colors.DbOldStone : Colors.DbLight);
            inventoryConsole.Print(1, 3 + (items.Count * 2) + 4, $"Hand: {Hand.Name}", Hand == HandEquipment.None() ? Colors.DbOldStone : Colors.DbLight);
            inventoryConsole.Print(1, 3 + (items.Count * 2) + 6, $"Feet: {Feet.Name}", Feet == FeetEquipment.None() ? Colors.DbOldStone : Colors.DbLight);
        }
        private void DrawItem(IItem item, RLConsole inventoryConsole, int position)
        {
            int xPosition = 0;
            int yPosition = 3 + (position * 2);
            string place = (position + 1).ToString();
            if (place == "10")
            {
                // the player will use the "0" key because there is no "10" key
                place = "0";
            }
            inventoryConsole.Print(xPosition, yPosition, $"{place} - {item.Name}", Colors.DbLight);
        }
    }
}
