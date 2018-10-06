using RogueSharp;
using Consogue.Tiles;
using System.Collections.Generic;
using RLNET;
using System.Linq;
using Consogue.Interfaces;

namespace Consogue.Core
{
    /// <summary>
    /// Our DungeonMap class extends the RogueSharp Map class.
    /// </summary>
    public class DungeonMap : Map
    {
        private readonly List<Monster> monsters;
        private readonly List<ITreasurePile> treasurePiles;
        public List<Rectangle> Rooms { get; set; }
        public List<Door> Doors { get; set; }
        public List<Plant> Plants = new List<Plant>();
        public Stairs StairsUp { get; set; }
        public Stairs StairsDown { get; set; }
        /// <summary>
        /// When a DungeonMap is created, a list of Rooms,
        /// Monsters, and Doors comes with it.
        /// </summary>
        public DungeonMap()
        {
            // When we make a new level by going down stairs we want to make sure that
            // all of the monsters from the previous level are removed from the schedule
            // and do not continue to try to act.
            Game.SchedulingSystem.Clear();
            Rooms = new List<Rectangle>();
            monsters = new List<Monster>();
            treasurePiles = new List<ITreasurePile>();
            Doors = new List<Door>();
        }

        /// <summary>
        /// This method will be called each time the map is updated.
        /// It will render all of the symbols/colors for each cell to the map subconsole,
        /// as well as draw all the monsters and doors.
        /// </summary>
        /// <param name="mapConsole"></param>
        /// <param name="statConsole"></param>
        public void Draw(RLConsole mapConsole, RLConsole statConsole)
        {
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
            foreach (Door door in Doors)
            {
                door.Draw(mapConsole, this);
            }
            foreach (Plant plant in Plants)
            {
                plant.Draw(mapConsole, this);
            }
            // Add one stairs up and one stairs down per level.
            if (StairsUp != null) // we don't have stairs in the overworld
            {
                StairsUp.Draw(mapConsole, this);
            }
            StairsDown.Draw(mapConsole, this);

            // Draw our treasure piles
            foreach (ITreasurePile treasurePile in treasurePiles)
            {
                IDrawable drawableTreasure = treasurePile.Treasure as IDrawable;
                drawableTreasure?.Draw(mapConsole, this);
            }

            // Keep an index so we know which position to draw monster stats at
            int i = 0;

            // Iterate through each monster on the map and draw it after drawing the Cells
            foreach (Monster monster in monsters)
            {
                // When the monster is in the field-of-view also draw their stats
                if (IsInFov(monster.X, monster.Y))
                {
                    monster.Draw(mapConsole, this);

                    // Pass in the index to DrawStats and increment it afterwards
                    monster.DrawStats(statConsole, i);
                    i++;
                }
            }
        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell)
        {
            // When we haven't explored a cell yet, we don't want to draw anything
            if (!cell.IsExplored)
            {
                return;
            }
            // When a cell is currently in the field-of-view it should be drawn with ligher colors
            if (IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            // When a cell is outside of the field of view draw it with darker colors
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }
        /// <summary>
        /// This method will be called any time we move the player to update field-of-view.
        /// </summary>
        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            // Compute the field-of-view based on the player's location and awareness
            ComputeFov(player.X, player.Y, player.Awareness, true);
            // Mark all cells in field-of-view as having been explored
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }
        /// <summary>
        /// Returns true when able to place the Actor on the cell or false otherwise
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            // Only allow actor placement if the cell is walkable
            if (GetCell(x, y).IsWalkable)
            {
                // The cell the actor was previously on is now walkable
                SetIsWalkable(actor.X, actor.Y, true);
                // Update the actor's position
                actor.X = x;
                actor.Y = y;
                // The new cell the actor is on is now not walkable
                SetIsWalkable(actor.X, actor.Y, false);
                // Try to open a door if one exists here
                OpenDoor(actor, x, y);
                // Don't forget to update the field of view if we just repositioned the player
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Will place the player at the exit, in the last room. Used when
        /// travelling back upstairs.
        /// </summary>
        public void PlacePlayerNearExit()
        {
            Player player = Game.Player;

            // The player had to take some stairs to get down, right?
            player.X = StairsDown.X;
            player.Y = StairsDown.Y;
            UpdatePlayerFieldOfView();
        }
        /// <summary>
        /// Used to place the player near the entrance when
        /// entering a level you've already been to.
        /// </summary>
        public void PlacePlayerNearEntrance()
        {
            Player player = Game.Player;

            player.X = Rooms[0].Center.X;
            player.Y = Rooms[0].Center.Y;
            UpdatePlayerFieldOfView();
        }
        public void RescheduleExistingActors()
        {
            Game.SchedulingSystem.Clear();
            Game.SchedulingSystem.Add(Game.Player);
            foreach (Monster monster in monsters)
            {
                Game.SchedulingSystem.Add(monster);
            }
            // for some reason, stairs aren't walkable after you do this. oh well.
            if (StairsUp != null) // no stars up in the overworld
            {
                SetIsWalkable(StairsUp.X, StairsUp.Y, true);
            }
            SetIsWalkable(StairsDown.X, StairsDown.Y, true);
        }

        /// <summary>
        /// A helper method for setting the IsWalkable property on a Cell.
        /// Used in the SetActorPosition() method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isWalkable"></param>
        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            SetCellProperties(
                GetCell(x, y).X,
                GetCell(x, y).Y,
                GetCell(x, y).IsTransparent,
                isWalkable,
                GetCell(x, y).IsExplored
            );
        }
        /// <summary>
        /// Adds a treasure to the overworld's treasure piles.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="treasure"></param>
        public void AddTreasure(int x, int y, ITreasure treasure)
        {
            treasurePiles.Add(new ITreasurePile(x, y, treasure));
        }
        /// <summary>
        /// Called by MapGenerator after we generate a new map to add the player to the map
        /// </summary>
        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            Game.SchedulingSystem.Add(player);
        }

        /// <summary>
        /// Called by MapGenerator after we generate a new map to add monsters to the map
        /// </summary>
        public void AddMonster(Monster monster)
        {
            monsters.Add(monster);
            // After adding the monster to the map make sure to make the cell not walkable
            SetIsWalkable(monster.X, monster.Y, false);
            Game.SchedulingSystem.Add(monster);
        }
        /// <summary>
        /// Called by CommandSystem after a monster dies or we leave the current level
        /// </summary>
        /// <param name="monster"></param>
        public void RemoveMonster(Monster monster)
        {
            monsters.Remove(monster);
            // After removing the monster from the map, make sure the cell is walkable again
            SetIsWalkable(monster.X, monster.Y, true);
            Game.SchedulingSystem.Remove(monster);
        }

        /// <summary>
        /// Gets a monster by an x, y position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Monster GetMonsterAt(int x, int y)
        {
            return monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        /// <summary>
        /// Gets an item by an x, y position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public List<ITreasurePile> GetItemsAt(int x, int y)
        {
            return treasurePiles.Where(m => m.X == x && m.Y == y).ToList();
        }

        /// <summary>
        /// Iterate through each Cell in the room and return true if any are walkable.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Look for a random location in the room that is walkable.
        /// </summary>
        public Point GetRandomWalkableLocationInRoom(Rectangle room)
        {
            Point Point = new Point(0, 0);
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        Point = new Point(x, y);
                    }
                }
            }

            return Point;
        }
        /// <summary>
        /// Return the door at the x,y position or null if one is not found.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }
        /// <summary>
        /// The actor opens the door located at the x,y position
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null && !door.IsOpen)
            {
                door.IsOpen = true;
                var cell = GetCell(x, y);
                // Once the door is opened it should be marked as transparent and no longer block field-of-view
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);

                Game.MessageLog.Add($"{actor.Name} opened a door.");
            }
        }
        /// <summary>
        /// Adds gold to the overworld's treasures.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="amount"></param>
        public void AddGold(int x, int y, int amount)
        {
            if (amount > 0)
            {
                AddTreasure(x, y, new Gold(amount));
            }
        }
        /// <summary>
        /// Allows an actor to pick up a treasure from the overworld.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bool PickUpTreasure(Actor actor, int x, int y)
        {
            bool pickedUpTreasure = false;
            List<ITreasurePile> treasureAtLocation = treasurePiles.Where(g => g.X == x && g.Y == y).ToList();
            foreach (ITreasurePile treasurePile in treasureAtLocation)
            {
                // Treasure.PickUp() handles the MessageLog
                if (treasurePile.Treasure.PickUp(actor))
                {
                    treasurePiles.Remove(treasurePile);
                    pickedUpTreasure = true;
                }
            }
            return pickedUpTreasure;
        }
        /// <summary>
        /// Makes sure the Player is above stairs before moving down them.
        /// </summary>
        /// <returns></returns>
        public bool CanMoveDownToNextLevel()
        {
            Player player = Game.Player;
            if(StairsDown != null)
            {
                return StairsDown.X == player.X && StairsDown.Y == player.Y;
            }
            return false;
        }
        /// <summary>
        /// Makes sure the Player is above upward stairs before moving up them.
        /// </summary>
        /// <returns></returns>
        public bool CanMoveUpToPreviousLevel()
        {
            Player player = Game.Player;
            if (StairsUp != null)
            {
                return StairsUp.X == player.X && StairsUp.Y == player.Y;
            }
            return false;
        }
    }
}
