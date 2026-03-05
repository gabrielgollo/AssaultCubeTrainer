using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AssaultCubeTrainer.Game
{
    /// <summary>
    /// Loader for game profiles from JSON files
    /// </summary>
    public static class GameProfileLoader
    {
        /// <summary>
        /// Load game profile from file path
        /// </summary>
        public static IGameProfile LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Game profile not found: {filePath}");
            }

            string json = File.ReadAllText(filePath);
            JObject jsonData = JObject.Parse(json);

            return new GameProfile(jsonData);
        }

        /// <summary>
        /// Load game profile by name (looks in Profiles/ folder)
        /// </summary>
        public static IGameProfile LoadByName(string gameName)
        {
            string profilesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles");
            string profilePath = Path.Combine(profilesDir, $"{gameName.ToLower().Replace(" ", "_")}.json");

            return LoadFromFile(profilePath);
        }
    }
}
