﻿using Consogue.Systems;
using RLNET;

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
            //statConsole.Print(1, 1, $"Name:    {Name}", Colors.Text);
            //statConsole.Print(1, 3, $"Health:  {Health}/{MaxHealth}", Colors.Text);
            //statConsole.Print(1, 5, $"Attack:  {Attack} ({AttackChance}%)", Colors.Text);
            //statConsole.Print(1, 7, $"Defense: {Defense} ({DefenseChance}%)", Colors.Text);
            //statConsole.Print(1, 9, $"Gold:    {Gold}", Colors.Gold);
            statConsole.Print(1, 1, $"Camera X:{CameraSystem.ViewportStartX}", Colors.Text);
            statConsole.Print(1, 3, $"Camera Y:{CameraSystem.ViewportStartY}", Colors.Text);
            statConsole.Print(1, 5, $"Player X:{Game.Player.X}", Colors.Text);
            statConsole.Print(1, 7, $"Player Y:{Game.Player.Y}", Colors.Text);
        }
    }
}
