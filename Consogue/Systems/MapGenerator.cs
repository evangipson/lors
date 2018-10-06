using Consogue.Core;
using Consogue.Monsters;
using Consogue.Tiles;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Consogue.Systems
{
    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        private readonly DungeonMap _map;

        /// <summary>
        /// Constructing a new MapGenerator requires the dimensions of the maps it will create
        /// as well as the sizes and maximum number of rooms.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="maxRooms"></param>
        /// <param name="roomMaxSize"></param>
        /// <param name="roomMinSize"></param>
        public MapGenerator(int width, int height,
        int maxRooms, int roomMaxSize, int roomMinSize, int mapLevel)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _map = new DungeonMap();
        }

        /// <summary>
        /// Creates the overworld and places a town in it
        /// </summary>
        /// <returns></returns>
        public DungeonMap CreateWorld()
        {
            // Set the properties of all cells to false
            _map.Initialize(_width, _height);

            // Make every tile a grass tile, walls around the edges
            foreach(Cell cell in _map.GetAllCells())
            {
                // As long as we aren't on the border....
                if(cell.X != 0 && cell.Y != 0 && cell.X != _width - 1 && cell.Y != _height - 1)
                {
                    // High percentage change to plant a tree
                    if (Dice.Roll("1d100") > 85)
                    {
                        _map.SetCellProperties(cell.X, cell.Y, false, false, false);
                        _map.Plants.Add(new Tree(
                            cell.X,
                            cell.Y
                        ));
                    }
                    // Or plant some grass
                    else
                    {
                        _map.SetCellProperties(cell.X, cell.Y, true, true, false);
                        _map.Plants.Add(new Grass(
                            cell.X,
                            cell.Y
                        ));
                    }
                }
            }

            // Now add the stairs- only down- before we add the player
            CreateDownStairs();

            // Now that our rooms and hallways are done, place the player in the middle
            // of the first room
            PlacePlayerInOverworld();

            // Now that the player is placed, place the monsters!
            PlaceMonsters();

            return _map;
        }

        /// <summary>
        /// Generate a new map that places rooms randomly
        /// </summary>
        public DungeonMap CreateMap()
        {
            // Set the properties of all cells to false
            _map.Initialize(_width, _height);

            // Try to place as many rooms as the specified maxRooms
            // Note: Only using decrementing loop because of WordPress formatting
            for (int r = _maxRooms; r > 0; r--)
            {
                // Determine the size and position of the room randomly
                int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomXPosition = Game.Random.Next(0, _width - roomWidth - 1);
                int roomYPosition = Game.Random.Next(0, _height - roomHeight - 1);

                // All of our rooms can be represented as Rectangles
                var newRoom = new Rectangle(roomXPosition, roomYPosition,
                  roomWidth, roomHeight);

                // Check to see if the room rectangle intersects with any other rooms
                bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

                // As long as it doesn't intersect add it to the list of rooms
                if (!newRoomIntersects)
                {
                    _map.Rooms.Add(newRoom);
                }
            }
            // Iterate through each room that we wanted placed
            // and dig out the rooms.
            foreach (Rectangle room in _map.Rooms)
            {
                CreateRoom(room);
            }

            // Iterate through each room that was generated
            // Don't do anything with the first room, so start at r = 1 instead of r = 0
            for (int r = 1; r < _map.Rooms.Count; r++)
            {
                // For all remaing rooms get the center of the room and the previous room
                int previousRoomCenterX = _map.Rooms[r - 1].Center.X;
                int previousRoomCenterY = _map.Rooms[r - 1].Center.Y;
                int currentRoomCenterX = _map.Rooms[r].Center.X;
                int currentRoomCenterY = _map.Rooms[r].Center.Y;

                // Give a 50/50 chance of which 'L' shaped connecting hallway to tunnel out
                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, previousRoomCenterY);
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, currentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCenterY, currentRoomCenterY, previousRoomCenterX);
                    CreateHorizontalTunnel(previousRoomCenterX, currentRoomCenterX, currentRoomCenterY);
                }
            }

            // Now that we've placed the tunnels, we can draw the doors.
            foreach(Rectangle room in _map.Rooms)
            {
                CreateDoors(room);
            }

            // Now add the stairs before placing the Player
            CreateStairs();

            // Now that our rooms and hallways are done, place the player in the middle
            // of the first room
            PlacePlayer();

            // Now that the player is placed, place the monsters!
            PlaceMonsters();

            return _map;
        }
        /// <summary>
        /// Given a rectangular area on the map,
        /// set the cell properties for that area to true.
        /// </summary>
        private void CreateRoom(Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, false);
                }
            }
        }
        /// <summary>
        /// Find the center of the first room that we created and place the Player there
        /// </summary>
        private void PlacePlayer()
        {
            Player player = Game.Player;
            if (player == null)
            {
                player = new Player();
            }

            player.X = _map.Rooms[0].Center.X;
            player.Y = _map.Rooms[0].Center.Y;

            _map.AddPlayer(player);
        }
        /// <summary>
        /// Doesn't rely on Rooms to place the player, but instead
        /// places the player in the middle of the map.
        /// </summary>
        private void PlacePlayerInOverworld()
        {
            Player player = Game.Player;
            if (player == null)
            {
                player = new Player();
            }
            // Find a walkable tile near the middle
            int middleX = (int)_map.Width / 2;
            int middleY = (int)_map.Height / 2;
            // If the tile isn't walkable, try another set up
            while (!_map.GetCell(middleX, middleY).IsWalkable)
            {
                if (Dice.Roll("1d2") > 1) {
                    ++middleX;
                }
                else {
                    ++middleY;
                }
                if (Dice.Roll("1d2") > 1)
                {
                    --middleX;
                }
                else
                {
                    --middleY;
                }
            }
            // Place the player there now that we know it's walkable
            player.X = middleX;
            player.Y = middleY;

            _map.AddPlayer(player);
        }
        /// <summary>
        /// Carve a tunnel out of the map parallel to the x-axis
        /// </summary>
        /// <param name="xStart"></param>
        /// <param name="xEnd"></param>
        /// <param name="yPosition"></param>
        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPosition)
        {
            for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetCellProperties(x, yPosition, true, true);
            }
        }

        /// <summary>
        /// Carve a tunnel out of the map parallel to the y-axis
        /// </summary>
        /// <param name="yStart"></param>
        /// <param name="yEnd"></param>
        /// <param name="xPosition"></param>
        private void CreateVerticalTunnel(int yStart, int yEnd, int xPosition)
        {
            for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPosition, y, true, true);
            }
        }
        /// <summary>
        /// The algorithm will basically go through each room and roll a 10-sided die.
        /// If the result of the roll is 1-6 then we’ll roll 4-sided die and add that many monsters to the room in any open Cells.
        /// </summary>
        private void PlaceMonsters()
        {
            foreach (var room in _map.Rooms)
            {
                // Each room has a 60% chance of having monsters
                if (Dice.Roll("1D10") < 7)
                {
                    // Generate between 1 and 4 monsters
                    var numberOfMonsters = Dice.Roll("1D3");
                    for (int i = 0; i < numberOfMonsters; i++)
                    {
                        // Find a random walkable location in the room to place the monster
                        Point randomRoomLocation = _map.GetRandomWalkableLocationInRoom(room);
                        // It's possible that the room doesn't have space to place a monster
                        // In that case skip creating the monster, and the default point that
                        // GetRandomWalkableLocationInRoom() returns in 0, 0. A wall will be there,
                        // so we know that means it's incorrect.
                        if (randomRoomLocation.X != 0 && randomRoomLocation.Y != 0)
                        {
                            // Temporarily hard code this monster to be created at level 1
                            var monster = Kobold.Create(1);
                            monster.X = randomRoomLocation.X;
                            monster.Y = randomRoomLocation.Y;
                            _map.AddMonster(monster);
                        }
                    }
                }
            }
        }
        private void CreateDoors(Rectangle room)
        {
            // The the boundries of the room
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            // Put the rooms border cells into a list
            var borderCells = (_map.GetCellsAlongLine(xMin, yMin, xMax, yMin)).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            // Go through each of the rooms border cells and look for locations to place doors.
            foreach (var cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    // A door must block field-of-view when it is closed.
                    _map.SetCellProperties(cell.X, cell.Y, false, true);
                    _map.Doors.Add(new Door
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpen = false
                    });
                }
            }
        }
        /// <summary>
        /// Checks to see if a cell is a good candidate for placement of a door
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool IsPotentialDoor(ICell cell)
        {
            // If the cell is not walkable
            // then it is a wall and not a good place for a door
            if (!cell.IsWalkable)
            {
                return false;
            }

            // Store references to all of the neighboring cells 
            ICell right = _map.GetCell(cell.X + 1, cell.Y);
            ICell left = _map.GetCell(cell.X - 1, cell.Y);
            ICell top = _map.GetCell(cell.X, cell.Y - 1);
            ICell bottom = _map.GetCell(cell.X, cell.Y + 1);

            // Make sure there is not already a door here
            if (_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            // This is a good place for a door on the left or right side of the room
            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }

            // This is a good place for a door on the top or bottom of the room
            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// We are creating the stairs up in the center of the first room that was generated.
        /// This is the same room that the player starts in and the player is also in the center of the room,
        /// so we’ll offset the X coordinate by 1 to put the stairs next to the player.
        /// The last room we generated gets stairs going down and again we place them in the center of the room.
        /// </summary>
        private void CreateStairs()
        {
            _map.StairsUp = new Stairs
            {
                X = _map.Rooms.First().Center.X + 1,
                Y = _map.Rooms.First().Center.Y,
                IsUp = true
            };
            _map.StairsDown = new Stairs
            {
                X = _map.Rooms.Last().Center.X,
                Y = _map.Rooms.Last().Center.Y,
                IsUp = false
            };
        }
        /// <summary>
        /// Intended to be used only by CreateWorld(), because dungeons always
        /// have a stairs up AND stairs down, so you should use CreateStairs().
        /// </summary>
        private void CreateDownStairs()
        {
            // roll a dice for how wide the map is
            string mapDiceWidthExpression = "1d" + (_map.Width - 1);
            string mapDiceHeightExpression = "1d" + (_map.Height - 1);
            _map.StairsDown = new Stairs
            {
                // TODO: Make this a random point in the town perhaps?? Center of all rooms?
                X = Dice.Roll(mapDiceWidthExpression),
                Y = Dice.Roll(mapDiceHeightExpression),
                IsUp = false
            };
        }
    }
}
