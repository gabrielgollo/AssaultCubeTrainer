namespace AssaultCubeTrainer.Game
{
    /// <summary>
    /// Interface for game profiles - allows different games to be supported
    /// </summary>
    public interface IGameProfile
    {
        /// <summary>
        /// Game name (e.g., "Assault Cube")
        /// </summary>
        string GameName { get; }

        /// <summary>
        /// Process name (e.g., "ac_client.exe")
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// Local player address
        /// </summary>
        string LocalPlayerAddress { get; }

        /// <summary>
        /// Entity list address
        /// </summary>
        string EntityListAddress { get; }

        /// <summary>
        /// View matrix address
        /// </summary>
        string ViewMatrixAddress { get; }

        /// <summary>
        /// Get offset by path (e.g., "player.position.x" -> "0x4")
        /// </summary>
        string GetOffset(string path);

        /// <summary>
        /// Get setting by path (e.g., "gameSettings.maxPlayers" -> 32)
        /// </summary>
        T GetSetting<T>(string path, T defaultValue = default);
    }
}
