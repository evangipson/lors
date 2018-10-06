using Consogue.Behaviors;
using Consogue.Interfaces;
using Consogue.Systems;
using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consogue.Core
{
    public class Monster : Actor
    {
        public int? TurnsAlerted { get; set; }

        public virtual void PerformAction(CommandSystem commandSystem)
        {
            var behavior = new StandardMoveAndAttack();
            behavior.Act(this, commandSystem);
        }
        public void DrawStats(RLConsole statConsole, int position)
        {
            int healthBarWidth = 16;
            int monstersPerRow = 3;
            int playerStatWidth = 18;

            int yPosition = (position + monstersPerRow) / monstersPerRow;
            // Multiply the position by 2 to leave a space between each stat
            int xPosition = playerStatWidth + ((position % monstersPerRow) * healthBarWidth);
            // Add some padding if this is the second monster
            if(position % monstersPerRow != 0)
            {
                xPosition += position % monstersPerRow;
            }
            if(yPosition > 1)
            {
                yPosition += 1;
            }

            // Begin the line by printing the symbol of the monster in the appropriate color
            statConsole.Print(xPosition, yPosition, Symbol.ToString(), Color);

            // Figure out the width of the health bar by dividing current health by max health
            int width = Convert.ToInt32(((double)Health / (double)MaxHealth) * healthBarWidth);
            int remainingWidth = healthBarWidth - width;

            // Set the background colors of the health bar to show how damaged the monster is
            statConsole.SetBackColor(xPosition + 2, yPosition, width - 2, 1, Colors.Primary);
            statConsole.SetBackColor(xPosition + 2 + width, yPosition, remainingWidth - 2, 1, Colors.PrimaryDarkest);

            // Print the monsters name over top of the health bar
            statConsole.Print(xPosition + 2, yPosition, $": {Name}", Colors.DbLight);
        }
    }
}
