using System;
using System.Collections.Generic;
using System.Linq;
using AssaultCubeTrainer.Game;

namespace AssaultCubeTrainer.Core
{
    /// <summary>
    /// Manages registration and execution of cheat features
    /// </summary>
    public class FeatureManager
    {
        private List<ICheatFeature> _features;

        public IEnumerable<ICheatFeature> Features => _features.AsReadOnly();

        public FeatureManager()
        {
            _features = new List<ICheatFeature>();
        }

        /// <summary>
        /// Register a new feature (automatically sorted by priority)
        /// </summary>
        public void RegisterFeature(ICheatFeature feature)
        {
            if (!_features.Contains(feature))
            {
                _features.Add(feature);
                // Sort by priority after adding
                _features = _features.OrderBy(f => f.Priority).ToList();
            }
        }

        /// <summary>
        /// Unregister a feature
        /// </summary>
        public void UnregisterFeature(ICheatFeature feature)
        {
            _features.Remove(feature);
        }

        /// <summary>
        /// Initialize all registered features
        /// </summary>
        public void InitializeAll(IGameProfile profile, MemoryManager memory)
        {
            foreach (var feature in _features)
            {
                feature.Initialize(profile, memory);
            }
        }

        /// <summary>
        /// Update all enabled features
        /// </summary>
        public void UpdateAll(GameState gameState)
        {
            foreach (var feature in _features.Where(f => f.IsEnabled))
            {
                feature.Update(gameState);
            }
        }

        /// <summary>
        /// Cleanup all features
        /// </summary>
        public void CleanupAll()
        {
            foreach (var feature in _features)
            {
                feature.Cleanup();
            }
        }

        /// <summary>
        /// Get feature by type
        /// </summary>
        public T GetFeature<T>() where T : ICheatFeature
        {
            return (T)_features.FirstOrDefault(f => f is T);
        }
    }
}
