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
