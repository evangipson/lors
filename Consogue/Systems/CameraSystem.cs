using Consogue.Core;

namespace Consogue.Systems
{
    public static class CameraSystem
    {
        public static int ViewportStartX = 0;
        public static int ViewportStartY = 0;

        /// <summary>
        /// Will update ViewportStartX and ViewportStartY when run.
        /// Used before blitting the mapConsole to the root console.
        /// </summary>
        public static void RefocusCamera()
        {
            // Center the camera on the player
            ViewportStartX = Game.Player.X - (Dimensions.MapWidth / 2);
            ViewportStartY = Game.Player.Y - (Dimensions.MapHeight / 2);
            // Handle minimum & maximum camera viewport
            if (Game.Player.X <= Dimensions.MapWidth / 2)
            {
                ViewportStartX = 0;
            }
            else if (Game.Player.X > Dimensions.WorldWidth - (Dimensions.MapWidth / 2))
            {
                ViewportStartX = Dimensions.WorldWidth - Dimensions.MapWidth;
            }
            if (Game.Player.Y < Dimensions.MapHeight / 2)
            {
                ViewportStartY = 0;
            }
            else if (Game.Player.Y > Dimensions.WorldHeight - (Dimensions.MapHeight / 2))
            {
                ViewportStartY = Dimensions.WorldHeight - Dimensions.MapHeight;
            }
        }
    }
}