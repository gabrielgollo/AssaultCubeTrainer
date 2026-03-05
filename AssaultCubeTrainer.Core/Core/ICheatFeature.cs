using AssaultCubeTrainer.Game;

namespace AssaultCubeTrainer.Core
{
    /// <summary>
    /// Interface for cheat features (plugins)
    /// </summary>
    public interface ICheatFeature
    {
        /// <summary>
        /// Feature name (e.g., "Aimbot", "ESP")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execution priority (lower = executes first)
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Whether this feature is enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Initialize the feature with game profile and memory manager
        /// </summary>
        void Initialize(IGameProfile profile, MemoryManager memory);

        /// <summary>
        /// Update called every frame when feature is enabled
        /// </summary>
        void Update(GameState gameState);

        /// <summary>
        /// Cleanup resources when feature is stopped
        /// </summary>
        void Cleanup();
    }
}
