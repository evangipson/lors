using Consogue.Core;
using Consogue.Systems;
using RLNET;
using RogueSharp.Random;
using System;

namespace Consogue
{
    class Game
    {
        private static bool _renderRequired = true;
        public static CommandSystem CommandSystem { get; private set; }
        public static MessageLog MessageLog { get; private set; }
        public static SchedulingSystem SchedulingSystem { get; private set; }

        // The base object representing the console
        private static RLRootConsole console;

        // Then the subconsoles
        private static RLConsole mapConsole;
        private static RLConsole messageConsole;
        private static RLConsole statConsole;
        private static RLConsole inventoryConsole;

        public static DungeonMap DungeonMap { get; private set; }
        public static Player Player { get; set; }
        public static IRandom Random { get; private set; }

        public static void Main()
        {
            string fontFileName = "terminal8x8.png";
            int fontSize = 8;
            float scale = 1f;
            // Establish the seed for the random number generator from the current time
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);
            // The title will appear at the top of the console window 
            // also include the seed used to generate the level
            string consoleTitle = $"L.O.R.S. - Level 1 - Seed {seed}";
            // Set up our RLRootConsole now that we have defaults
            console = new RLRootConsole(
                fontFileName,
                Dimensions.ScreenWidth,
                Dimensions.ScreenHeight,
                fontSize, fontSize,
                scale,
                consoleTitle
            );
            // Initialize the sub consoles that we will Blit to the root console
            // Note: mapConsole needs to be as big as the world, but we'll only render a section of it
            mapConsole = new RLConsole(Dimensions.WorldWidth, Dimensions.WorldHeight);
            messageConsole = new RLConsole(Dimensions.MessageWidth, Dimensions.MessageHeight);
            statConsole = new RLConsole(Dimensions.StatWidth, Dimensions.StatHeight);
            inventoryConsole = new RLConsole(Dimensions.InventoryWidth, Dimensions.InventoryHeight);
            // Create our command system
            CommandSystem = new CommandSystem();
            // Start our scheduling system
            SchedulingSystem = new SchedulingSystem();
            // Create our map
            int maxRooms = Random.Next(17, 23);
            int maxRoomWidth = Random.Next(13, 22);
            int maxRoomHeight = Random.Next(8, 11);
            DungeonMap = new MapGenerator(
                Dimensions.WorldWidth,
                Dimensions.WorldHeight,
                maxRooms,
                maxRoomWidth,
                maxRoomHeight
            ).CreateMap();
            // Now that we have player AND map, we can update player FOV
            DungeonMap.UpdatePlayerFieldOfView();
            // Now make our console listen to our custom functions
            console.Update += OnConsoleUpdate;
            console.Render += OnConsoleRender;
            // Get the subconsoles background color and text on the screen
            SetSubconsoleColorAndTitle();
            // Create a new MessageLog and print the random seed used to generate the level
            MessageLog = new MessageLog();
            MessageLog.Add("The rogue arrives on level 1");
            MessageLog.Add($"Level created with seed '{seed}'");
            // Now begin the game loop
            console.Run();
        }

        /// <summary>
        /// This function will set background color and text for each subconsole
        /// so that we can verify they are in the correct positions.
        /// </summary>
        private static void SetSubconsoleColorAndTitle()
        {
            inventoryConsole.SetBackColor(0, 0, Dimensions.InventoryWidth, Dimensions.InventoryHeight, Colors.AlternateDarkest);
            inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);
        }

        /// <summary>
        /// The event handler for RLNET's Update event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnConsoleUpdate(object sender, UpdateEventArgs e)
        {
            // Event handler for RLNET's Update event
            bool didPlayerAct = false;
            RLKeyPress keyPress = console.Keyboard.GetKeyPress();
            if (CommandSystem.IsPlayerTurn)
            {
                if (keyPress != null)
                {
                    switch (keyPress.Key)
                    {
                        case RLKey.Keypad1:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.DownLeft);
                                break;
                            }
                        case RLKey.Down:
                        case RLKey.Keypad2:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                                break;
                            }
                        case RLKey.Keypad3:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.DownRight);
                                break;
                            }
                        case RLKey.Keypad4:
                        case RLKey.Left:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                                break;
                            }
                        case RLKey.Keypad6:
                        case RLKey.Right:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                                break;
                            }
                        case RLKey.Keypad7:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.UpLeft);
                                break;
                            }
                        case RLKey.Keypad8:
                        case RLKey.Up:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                                break;
                            }
                        case RLKey.Keypad9:
                            {
                                didPlayerAct = CommandSystem.MovePlayer(Direction.UpRight);
                                break;
                            }
                        case RLKey.Escape:
                            {
                                console.Close();
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                if (didPlayerAct)
                {
                    _renderRequired = true;
                    CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                CommandSystem.ActivateMonsters();
                _renderRequired = true;
            }
        }

        /// <summary>
        /// The event handler for RLNET's render event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnConsoleRender(object sender, UpdateEventArgs e)
        {
            // Don't bother redrawing all of the consoles if nothing has changed.
            if (_renderRequired)
            {
                mapConsole.Clear();
                statConsole.Clear();
                messageConsole.Clear();

                // Draw the map
                DungeonMap.Draw(mapConsole, statConsole);
                // Draw our player on the map subconsole
                Player.Draw(mapConsole, DungeonMap);
                // After we draw our player, draw our player's stats in the stats subconsole
                Player.DrawStats(statConsole);
                // Draw our message log
                MessageLog.Draw(messageConsole);
                // Refocus the camera
                CameraSystem.RefocusCamera();

                // Blit the sub consoles to the root console in the correct locations
                RLConsole.Blit(
                    statConsole,
                    0,
                    0,
                    Dimensions.StatWidth,
                    Dimensions.StatHeight,
                    console,
                    0,
                    0
                );
                RLConsole.Blit(
                    messageConsole,
                    0,
                    0,
                    Dimensions.MessageWidth,
                    Dimensions.MessageHeight,
                    console,
                    0,
                    Dimensions.ScreenHeight - Dimensions.MessageHeight
                );
                RLConsole.Blit(
                    inventoryConsole,
                    0,
                    0,
                    Dimensions.InventoryWidth,
                    Dimensions.InventoryHeight,
                    console,
                    Dimensions.MapWidth,
                    0
                );
                RLConsole.Blit(
                    mapConsole,
                    CameraSystem.ViewportStartX,
                    CameraSystem.ViewportStartY,
                    Dimensions.MapWidth,
                    Dimensions.MapHeight, 
                    console,
                    0,
                    Dimensions.StatHeight
                );
                // Tell RLNET to draw the console that we set
                console.Draw();
            }
        }
    }
}
