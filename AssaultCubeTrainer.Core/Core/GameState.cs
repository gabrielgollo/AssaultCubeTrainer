using System;
using System.Collections.Generic;
using System.Drawing;

namespace AssaultCubeTrainer.Core
{
    /// <summary>
    /// Cached game state data - updated once per frame and shared across all features
    /// </summary>
    public class GameState
    {
        // Cached data - updated once per frame
        public Entity LocalPlayer { get; set; }
        public List<Entity> AllEntities { get; set; }
        public List<Entity> Enemies { get; set; }
        public ViewMatrix ViewMatrix { get; set; }
        public Size GameWindowSize { get; set; }
        public IntPtr GameWindowHandle { get; set; }

        // Frame timing
        public float DeltaTime { get; set; }
        public int FrameCount { get; set; }

        // Cache validity
        public bool IsValid { get; set; }
        public DateTime LastUpdate { get; set; }

        public GameState()
        {
            AllEntities = new List<Entity>();
            Enemies = new List<Entity>();
            IsValid = false;
        }

        public void Invalidate()
        {
            IsValid = false;
        }
    }
}
