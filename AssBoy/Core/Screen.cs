using System;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Core
{
    public class Screen
    {
        public static int Width => screenTarget.Width;
        public static int Height => screenTarget.Height;
        public static RenderTarget2D screenTarget { get; private set; }
        
        public static void InitScreen(RenderTarget2D screenTarget)
        {
            if (Screen.screenTarget != null) throw new Exception("Screen :: Already initialized.");
            Screen.screenTarget = screenTarget;
        }
    }
}