using Consogue.Core;
using Consogue.Equipment;
using Consogue.Interfaces;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consogue.Systems
{
    public class CommandSystem
    {
        /// <summary>
        /// Return value is true if the player was able to move, and
        /// false when the player couldn't move, such as trying to move into a wall
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            // Handle vertical movement
            if (direction == Direction.UpLeft || direction == Direction.Up || direction == Direction.UpRight)
            {
                y = Game.Player.Y - 1;
            }
            else if (direction == Direction.DownLeft || direction == Direction.Down || direction == Direction.DownRight)
            {
                y = Game.Player.Y + 1;
            }

            // Handle horizontal movement
            if (direction == Direction.DownLeft || direction == Direction.UpLeft || direction == Direction.Left)
            {
                x = Game.Player.X - 1;
            }
            else if (direction == Direction.DownRight || direction == Direction.UpRight || direction == Direction.Right)
            {
                x = Game.Player.X + 1;
            }

            // First try and place the player on the new position
            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y))
            {
                return true;
            }

            // If that doesn't work, we might've hit a monster.
            Monster monster = Game.DungeonMap.GetMonsterAt(x, y);

            if (monster != null)
            {
                Attack(Game.Player, monster);
                return true;
            }

            // If that doesn't work, we might've hit an item.
            return false;
        }
        /// <summary>
        /// An actor will attack a defending actor. Will update
        /// the message log as well as handle resolving damage.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        public void Attack(Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);

            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            Game.MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                Game.MessageLog.Add(defenseMessage.ToString());
            }

            int damage = hits - blocks;

            ResolveDamage(defender, damage);
        }
        /// <summary>
        /// The attacker rolls based on his stats to see if he gets any hits
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="attackMessage"></param>
        /// <returns></returns>
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            int hits = 0;

            // Roll a number of 100-sided dice equal to the Attack value of the attacking actor
            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            // Look at the face value of each single die that was rolled
            foreach (TermResult termResult in attackResult.Results)
            {
                // Compare the value to 100 minus the attack chance and add a hit if it's greater
                if (termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            // TODO: Different messages based on how many hits were made
            if(hits == attacker.Attack)
            {
                attackMessage.AppendFormat("{0} slams into {1} with full force!", attacker.Name, defender.Name);
            }
            else if(hits > 0)
            {
                attackMessage.AppendFormat("{0} lunges at {1}.", attacker.Name, defender.Name);
            }
            else
            {
                attackMessage.AppendFormat("{0} launches offbalance toward {1}.", attacker.Name, defender.Name);
            }

            return hits;
        }
        /// <summary>
        /// The defender rolls based on his stats to see if he blocks any of the hits from the attacker
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="hits"></param>
        /// <param name="attackMessage"></param>
        /// <param name="defenseMessage"></param>
        /// <returns></returns>
        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if (hits > 0)
            {

                // Roll a number of 100-sided dice equal to the Defense value of the defendering actor
                DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult defenseRoll = defenseDice.Roll();

                // Look at the face value of each single die that was rolled
                foreach (TermResult termResult in defenseRoll.Results)
                {
                    // Compare the value to 100 minus the defense chance and add a block if it's greater
                    if (termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }
                // TODO: Different messages based on how many hits were made
                if (blocks == defender.Defense)
                {
                    defenseMessage.AppendFormat("{0} absorbs the blows with ease.", defender.Name);
                }
                else if (blocks > 0)
                {
                    defenseMessage.AppendFormat("The defense of {0} falls for just a moment.", defender.Name);
                }
            }
            else
            {
                defenseMessage.AppendFormat("{0} was totally unaware!", defender.Name);
            }

            return blocks;
        }

        // Apply any damage that wasn't blocked to the defender
        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0)
            {
                defender.Health = defender.Health - damage;

                Game.MessageLog.Add($"{defender.Name} was hit for {damage} damage.");

                if (defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                Game.MessageLog.Add($"{defender.Name} blocked all damage.");
            }
        }

        // Remove the defender from the map and add some messages upon death.
        private static void ResolveDeath(Actor defender)
        {
            if (defender is Player)
            {
                Game.MessageLog.Add($"{defender.Name} was killed, GAME OVER MAN!");
            }
            else if (defender is Monster)
            {
                Game.MessageLog.Add($"{defender.Name} succubs to their wounds and dies.");
                Game.DungeonMap.RemoveMonster((Monster)defender);
                if(defender.Gold > 0)
                {
                    Game.DungeonMap.AddGold(defender.X, defender.Y, defender.Gold);
                    Game.MessageLog.Add($"{defender.Name} dropped {defender.Gold} gold.");
                }
                if(defender.items.Count > 0)
                {
                    for (int i = 0; i < defender.items.Count; i++)
                    {
                        Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.items[i] as ITreasure);
                        Game.MessageLog.Add($"{defender.Name} dropped {defender.items[i].Name}.");
                    }
                }
                if (defender.Head.Name != "None")
                {
                    Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Head as ITreasure);
                    Game.MessageLog.Add($"{defender.Name} dropped {defender.Head.Name}.");
                }
            }
        }

        public bool IsPlayerTurn { get; set; }

        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        /// <summary>
        /// The ActivateMonsters() method is intended to be called after the Player takes a turn.
        /// This will proceed to get the next scheduled Actor from the SchedulingSystem.
        /// If this happens to be the Player again, we’ll wait for the Player to make a move;
        /// otherwise we’ll have the Monster perform an action and then call ActivateMonsters() again recursively.
        /// This will keep having Monsters perform their actions until it is once again the Player’s turn.
        /// </summary>
        public void ActivateMonsters()
        {
            IScheduleable scheduleable = Game.SchedulingSystem.Get();
            if (scheduleable is Player)
            {
                IsPlayerTurn = true;
                Game.SchedulingSystem.Add(Game.Player);
            }
            else
            {
                Monster monster = scheduleable as Monster;

                if (monster != null)
                {
                    monster.PerformAction(this);
                    Game.SchedulingSystem.Add(monster);
                }

                ActivateMonsters();
            }
        }

        public void MoveMonster(Monster monster, Cell cell)
        {
            if (!Game.DungeonMap.SetActorPosition(monster, cell.X, cell.Y))
            {
                if (Game.Player.X == cell.X && Game.Player.Y == cell.Y)
                {
                    Attack(monster, Game.Player);
                }
            }
        }
        public bool HandleKey(RLKey key)
        {
            bool didUseItem = false;
            if (key == RLKey.Number1)
            {
                if(Game.Player.items.Count > 0)
                {
                    didUseItem = Game.Player.items[0].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number2)
            {
                if (Game.Player.items.Count > 1)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number3)
            {
                if (Game.Player.items.Count > 2)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number4)
            {
                if (Game.Player.items.Count > 3)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number5)
            {
                if (Game.Player.items.Count > 4)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number6)
            {
                if (Game.Player.items.Count > 5)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number7)
            {
                if (Game.Player.items.Count > 6)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number8)
            {
                if (Game.Player.items.Count > 7)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number9)
            {
                if (Game.Player.items.Count > 8)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.Number0)
            {
                if (Game.Player.items.Count > 9)
                {
                    didUseItem = Game.Player.items[1].Use();
                }
                else
                {
                    Game.MessageLog.Add($"{Game.Player.Name} glances in their empty item bag.");
                }
            }
            else if (key == RLKey.P)
            {
                if(Game.DungeonMap.PickUpTreasure(Game.Player, Game.Player.X, Game.Player.Y)) {
                    // PickUpTreasure is the action so meaningfully nothing
                }
                else
                {
                    Game.MessageLog.Add("Nothing to pick up.");
                }
            }
            else if (key == RLKey.L)
            {
                /* We want to make the "Look" message interesting, so even 
                 * if there is nothing, we'll say something. */
                bool sawSomething = false;
                ICell currentPlayerCell = Game.DungeonMap.GetCell(Game.Player.X, Game.Player.Y);
                List<ITreasurePile> currentPlayerCellItems = Game.DungeonMap.GetItemsAt(Game.Player.X, Game.Player.Y);
                if (currentPlayerCellItems != null)
                {
                    for(int i = 0; i < currentPlayerCellItems.Count; i++)
                    {
                        if(currentPlayerCellItems[i].Treasure is HeadEquipment ||
                           currentPlayerCellItems[i].Treasure is HandEquipment ||
                           currentPlayerCellItems[i].Treasure is FeetEquipment ||
                           currentPlayerCellItems[i].Treasure is BodyEquipment)
                        {
                            Game.MessageLog.Add($"{Game.Player.Name} sees the {currentPlayerCellItems[i].Treasure.Name} (+{(currentPlayerCellItems[i].Treasure as IEquipment).Defense} defense).");
                        }
                        else
                        {
                            Game.MessageLog.Add($"{Game.Player.Name} sees {currentPlayerCellItems[i].Treasure.Name}.");
                        }
                    }
                    sawSomething = true;
                }
                for (int i = 0; i < Game.DungeonMap.Plants.Count; i++) {
                    if (Game.DungeonMap.Plants[i].X == Game.Player.X && Game.DungeonMap.Plants[i].Y == Game.Player.Y)
                    {
                        Game.MessageLog.Add($"{Game.Player.Name} sees the {Game.DungeonMap.Plants[i].Name}.");
                        sawSomething = true;
                    }
                }
                for (int i = 0; i < Game.DungeonMap.Doors.Count; i++)
                {
                    if (Game.DungeonMap.Doors[i].X == Game.Player.X && Game.DungeonMap.Doors[i].Y == Game.Player.Y)
                    {
                        // We know the door is open because the tile has to be walkable
                        Game.MessageLog.Add($"{Game.Player.Name} sees an open door.");
                        sawSomething = true;
                    }
                }
                if(Game.DungeonMap.StairsUp != null)
                {
                    if (Game.DungeonMap.StairsUp.X == Game.Player.X && Game.DungeonMap.StairsUp.Y == Game.Player.Y)
                    {
                        // We know the door is open because the tile has to be walkable
                        Game.MessageLog.Add($"{Game.Player.Name} some stairs leading up.");
                        sawSomething = true;
                    }
                }
                if (Game.DungeonMap.StairsDown.X == Game.Player.X && Game.DungeonMap.StairsDown.Y == Game.Player.Y)
                {
                    // We know the door is open because the tile has to be walkable
                    Game.MessageLog.Add($"{Game.Player.Name} some stairs leading down.");
                    sawSomething = true;
                }
                // The last ditch effort to see something interesting: monsters in the tiles around you
                List<ICell> cellsAroundPlayer = Game.DungeonMap.GetBorderCellsInSquare(Game.Player.X, Game.Player.Y, 1).ToList();
                Monster currentMonster = Game.DungeonMap.GetMonsterAt(Game.Player.X, Game.Player.Y);
                foreach(ICell cell in cellsAroundPlayer)
                {
                    currentMonster = Game.DungeonMap.GetMonsterAt(cell.X, cell.Y);
                    if(currentMonster != null)
                    {
                        Game.MessageLog.AnnounceMonster(currentMonster);
                    }
                }
                if(!sawSomething)
                {
                    // TODO: Make this "default" message a bit more varied and interesting
                    Game.MessageLog.Add($"{Game.Player.Name} sees nothing out of the ordinary.");
                }
            }

            if (didUseItem)
            {
                RemoveItemsWithNoRemainingUses();
            }

            return didUseItem;
        }

        private static void RemoveItemsWithNoRemainingUses()
        {
            for(int i = 0; i < Game.Player.items.Count; i++)
            {
                if (Game.Player.items[i].RemainingUses <= 0)
                {
                    Game.Player.items.Remove(Game.Player.items[i]);
                }
            }
        }
    }
}
