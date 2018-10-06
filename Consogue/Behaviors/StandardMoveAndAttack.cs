using Consogue.Core;
using Consogue.Interfaces;
using Consogue.Systems;
using RogueSharp;

namespace Consogue.Behaviors
{
    /// <summary>
    /// Briefly lets go over what the StandardMoveAndAttack behavior will do.
    ///   * A monster should perform a standard melee attack on the player
    ///     if the player is adjacent to the monster.
    ///   * If the player is not within attack range, the monster should move
    ///     closer to the player via the shortest available path.
    ///   * Only monsters that are aware of the player should chase him.
    ///   * Once the monster is alerted and begins the chase, if the player evades
    ///     him and remains out of visual range for a period of time the monster should stop pursuit.
    ///   * If the monster can see the player but does not have a valid path
    ///     (possibly because of being blocked by other monsters) it should wait a turn.
    /// </summary>
    public class StandardMoveAndAttack : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Player player = Game.Player;
            FieldOfView monsterFov = new FieldOfView(dungeonMap);

            // If the monster has not been alerted, compute a field-of-view 
            // Use the monster's Awareness value for the distance in the FoV check
            // If the player is in the monster's FoV then alert it
            // Add a message to the MessageLog regarding this alerted status
            if (!monster.TurnsAlerted.HasValue)
            {
                monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
                if (monsterFov.IsInFov(player.X, player.Y))
                {
                    Game.MessageLog.Add($"{monster.Name} is eager to fight {player.Name}.");
                    monster.TurnsAlerted = 1;
                }
            }

            if (monster.TurnsAlerted.HasValue)
            {
                // Before we find a path, make sure to make the monster and player Cells walkable
                dungeonMap.SetIsWalkable(monster.X, monster.Y, true);
                dungeonMap.SetIsWalkable(player.X, player.Y, true);

                PathFinder pathFinder = new PathFinder(dungeonMap);
                Path path = null;

                try
                {
                    path = pathFinder.ShortestPath(
                    dungeonMap.GetCell(monster.X, monster.Y),
                    dungeonMap.GetCell(player.X, player.Y));
                }
                catch (PathNotFoundException)
                {
                    // The monster can see the player, but cannot find a path to him
                    // This could be due to other monsters blocking the way
                    // Add a message to the message log that the monster is waiting
                    Game.MessageLog.Add($"{monster.Name} gets distracted by something.");
                }

                // Don't forget to set the walkable status back to false
                dungeonMap.SetIsWalkable(monster.X, monster.Y, false);
                dungeonMap.SetIsWalkable(player.X, player.Y, false);

                // In the case that there was a path, tell the CommandSystem to move the monster
                if (path != null)
                {
                    try
                    {
                        Cell destinationCell = path.StepForward() as Cell;
                        commandSystem.MoveMonster(monster, destinationCell);
                    }
                    catch (NoMoreStepsException)
                    {
                        Game.MessageLog.Add($"{monster.Name} growls in frustration.");
                    }
                }

                monster.TurnsAlerted++;

                // Lose alerted status every 15 turns. 
                // As long as the player is still in FoV the monster will stay alert
                // Otherwise the monster will quit chasing the player.
                if (monster.TurnsAlerted > 15)
                {
                    monster.TurnsAlerted = null;
                }
            }
            return true;
        }
    }
}
