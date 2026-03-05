using AssaultCubeTrainer.Core;
using AssaultCubeTrainer.Game;

namespace AssaultCubeTrainer.Features
{
    /// <summary>
    /// Feature that keeps player attributes at specified values (god mode)
    /// </summary>
    public class AttributeKeeperFeature : ICheatFeature
    {
        private IGameProfile _profile;
        private MemoryManager _memory;

        public string Name => "Attribute Keeper";
        public int Priority => 1; // Run first
        public bool IsEnabled { get; set; }

        // Configurable values
        public int HealthValue { get; set; } = 100;
        public int AmmoValue { get; set; } = 100;
        public int GrenadesValue { get; set; } = 100;

        public void Initialize(IGameProfile profile, MemoryManager memory)
        {
            _profile = profile;
            _memory = memory;
        }

        public void Update(GameState gameState)
        {
            if (!gameState.IsValid)
                return;

            int localPlayerPtr = _memory.ReadInt(_profile.LocalPlayerAddress);
            if (localPlayerPtr <= 0x10000)
            {
                return;
            }

            string playerEntityHex = localPlayerPtr.ToString("X");

            // Set health
            string healthAddress = Utils.Utils.SumOfHexStrings(playerEntityHex, _profile.GetOffset("player.attributes.health"));
            _memory.WriteMemory(healthAddress, "int", HealthValue.ToString());

            // Set ammo
            string ammoAddress = Utils.Utils.SumOfHexStrings(playerEntityHex, _profile.GetOffset("player.attributes.ammo"));
            _memory.WriteMemory(ammoAddress, "int", AmmoValue.ToString());

            // Set grenades
            string grenadesAddress = Utils.Utils.SumOfHexStrings(playerEntityHex, _profile.GetOffset("player.attributes.grenades"));
            _memory.WriteMemory(grenadesAddress, "int", GrenadesValue.ToString());
        }

        public void Cleanup()
        {
            // Nothing to clean up
        }
    }
}
