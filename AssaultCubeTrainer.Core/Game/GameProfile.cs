using System;
using Newtonsoft.Json.Linq;

namespace AssaultCubeTrainer.Game
{
    /// <summary>
    /// Game profile implementation - loads configuration from JSON
    /// </summary>
    public class GameProfile : IGameProfile
    {
        private JObject _jsonData;
        private string _moduleName;

        public string GameName { get; private set; }
        public string ProcessName { get; private set; }
        public string LocalPlayerAddress { get; private set; }
        public string EntityListAddress { get; private set; }
        public string ViewMatrixAddress { get; private set; }

        public GameProfile(JObject jsonData)
        {
            _jsonData = jsonData;

            GameName = jsonData["gameName"]?.ToString();
            ProcessName = jsonData["processName"]?.ToString();
            _moduleName = NormalizeModuleName(ProcessName);

            var offsets = jsonData["offsets"];
            LocalPlayerAddress = ResolveAddress(offsets["localPlayer"]?.ToString());
            EntityListAddress = ResolveAddress(offsets["entityList"]?.ToString());
            ViewMatrixAddress = ResolveAddress(offsets["viewMatrix"]?.ToString());
        }

        /// <summary>
        /// Resolve address - replaces placeholder with module name (e.g. "ac_client.exe")
        /// </summary>
        private string ResolveAddress(string address)
        {
            return address?
                .Replace("processName", _moduleName)
                .Replace("moduleName", _moduleName);
        }

        private static string NormalizeModuleName(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                return string.Empty;
            }

            return processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? processName
                : $"{processName}.exe";
        }

        /// <summary>
        /// Navigate JSON path to get offset (e.g., "player.position.x" -> "0x4")
        /// </summary>
        public string GetOffset(string path)
        {
            var token = _jsonData["offsets"];

            foreach (var part in path.Split('.'))
            {
                token = token?[part];
            }

            return token?.ToString();
        }

        /// <summary>
        /// Navigate JSON path to get setting with default value
        /// </summary>
        public T GetSetting<T>(string path, T defaultValue = default)
        {
            JToken token = _jsonData;

            foreach (var part in path.Split('.'))
            {
                token = token?[part];
            }

            if (token == null)
                return defaultValue;

            try
            {
                return token.ToObject<T>();
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
