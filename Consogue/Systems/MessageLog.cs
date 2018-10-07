using Consogue.Core;
using RLNET;
using System.Collections.Generic;

namespace Consogue.Systems
{
    public class MessageLog
    {
        // We'll use a Queue to keep track of the lines
        // of text shown in the message log. The first line
        // shown will also be the first removed.
        private readonly Queue<string> lines;
        // How many lines can be shown in the message log at once
        private static readonly int maxLines = 7;

        public MessageLog()
        {
            lines = new Queue<string>();
        }

        /// <summary>
        /// Adds a line to the MessageLog Queue.
        /// </summary>
        /// <param name="message"></param>
        public void Add(string message)
        {
            lines.Enqueue(message);

            // When we exceed the max number of lines, remove the oldest one.
            if(lines.Count > maxLines)
            {
                lines.Dequeue();
            }
        }

        public void AnnounceMonster(Monster monster)
        {
            Game.MessageLog.Add($"{monster.Name} is eager to fight {Game.Player.Name}.");
            if (monster.Head.Name != "None" ||
               monster.Body.Name != "None" ||
               monster.Hand.Name != "None" ||
               monster.Feet.Name != "None")
            {
                Game.MessageLog.Add($"{monster.Name} is wearing:");
                if (monster.Head.Name != "None")
                {
                    Game.MessageLog.Add($"A {monster.Head.Name}, (+{monster.Head.Defense} DEF) on their head.");
                }
                if (monster.Body.Name != "None")
                {
                    Game.MessageLog.Add($"A {monster.Body.Name}, (+{monster.Body.Defense} DEF) on their body.");
                }
                if (monster.Hand.Name != "None")
                {
                    Game.MessageLog.Add($"A {monster.Hand.Name}, (+{monster.Hand.Defense} DEF) on their hands.");
                }
                if (monster.Feet.Name != "None")
                {
                    Game.MessageLog.Add($"A {monster.Feet.Name}, (+{monster.Feet.Defense} DEF) on their feet.");
                }
            }
        }

        /// <summary>
        /// Draws each line of the MessageLog to the console.
        /// </summary>
        /// <param name="console"></param>
        public void Draw(RLConsole console)
        {
            string[] arrayOfLines = lines.ToArray();
            for(int i = 0; i < arrayOfLines.Length; i++)
            {
                console.Print(1, i + 1, arrayOfLines[i], RLColor.White);
            }
        }
    }
}
