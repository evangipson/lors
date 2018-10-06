using Consogue.Equipment;
using Consogue.Interfaces;
using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consogue.Core
{
    public class Equipment : IEquipment, ITreasure, IDrawable
    {
        public static string[] lowLevelBase = new string[] {
            "Leather",
            "Chitin",
            "Bug",
            "Chain",
            "Wool"
        };
        public int Value { get; set; }
        public int Attack { get; set; }
        public int AttackChance { get; set; }
        public int Defense { get; set; }
        public int DefenseChance { get; set; }
        public int Speed { get; set; }
        public string Name { get; set; }
        public Equipment()
        {
            Symbol = ']';
            Color = RLColor.Yellow;
        }

        public bool PickUp(IActor actor)
        {
            if (this is HeadEquipment)
            {
                actor.Head = this as HeadEquipment;
                Game.MessageLog.Add($"{actor.Name} picked up a {Name} helmet");
                return true;
            }

            if (this is BodyEquipment)
            {
                actor.Body = this as BodyEquipment;
                Game.MessageLog.Add($"{actor.Name} picked up {Name} body armor");
                return true;
            }

            if (this is HandEquipment)
            {
                actor.Hand = this as HandEquipment;
                Game.MessageLog.Add($"{actor.Name} picked up a {Name}");
                return true;
            }

            if (this is FeetEquipment)
            {
                actor.Feet = this as FeetEquipment;
                Game.MessageLog.Add($"{actor.Name} picked up {Name} boots");
                return true;
            }

            return false;
        }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public void Draw(RLConsole console, IMap map)
        {
            if (!map.IsExplored(X, Y))
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, RLColor.Blend(Color, RLColor.Gray, 0.5f), Colors.FloorBackground, Symbol);
            }
        }
    }
}
