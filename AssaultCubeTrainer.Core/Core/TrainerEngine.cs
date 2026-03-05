using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using AssaultCubeTrainer.Game;
using AssaultCubeTrainer.Rendering;

namespace AssaultCubeTrainer.Core
{
    /// <summary>
    /// Main trainer engine with optimized loop and FPS control
    /// </summary>
    public class TrainerEngine
    {
        private IGameProfile _gameProfile;
        private MemoryManager _memoryManager;
        private FeatureManager _featureManager;
        private GameState _gameState;
        private Thread _mainThread;
        private bool _isRunning;

        // Performance settings
        public int TargetFPS { get; set; } = 60;
        private int _frameTimeMs;
        private IntPtr _stableWindowHandle;
        private Size _stableWindowSize;

        // Events for UI integration
        public event Action<GameState> OnGameStateUpdated;
        public event Action<string> OnError;
        public event Action OnGameDetached;

        public bool IsAttached => _memoryManager?.IsAttached ?? false;
        public GameState CurrentState => _gameState;

        public TrainerEngine()
        {
            _memoryManager = new MemoryManager();
            _featureManager = new FeatureManager();
            _gameState = new GameState();
            UpdateFrameTime();
        }

        private void UpdateFrameTime()
        {
            int fps = Math.Max(1, TargetFPS);
            _frameTimeMs = Math.Max(1, 1000 / fps);
        }

        /// <summary>
        /// Initialize engine with game profile
        /// </summary>
        public bool Initialize(IGameProfile gameProfile)
        {
            _gameProfile = gameProfile;

            if (!_memoryManager.AttachToProcess(gameProfile))
            {
                return false;
            }

            // Initialize all features
            _featureManager.InitializeAll(gameProfile, _memoryManager);

            return true;
        }

        /// <summary>
        /// Register a cheat feature
        /// </summary>
        public void RegisterFeature(ICheatFeature feature)
        {
            _featureManager.RegisterFeature(feature);
        }

        /// <summary>
        /// Start the main loop
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _mainThread = new Thread(MainLoop)
            {
                IsBackground = true
            };
            _mainThread.Start();
        }

        /// <summary>
        /// Stop the main loop
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _mainThread?.Join(1000);
            _featureManager.CleanupAll();
        }

        /// <summary>
        /// Main loop - optimized with FPS control
        /// </summary>
        private void MainLoop()
        {
            Stopwatch loopTimer = new Stopwatch();

            while (_isRunning)
            {
                loopTimer.Restart();

                try
                {
                    if (!_memoryManager.IsAttached)
                    {
                        OnGameDetached?.Invoke();
                        Thread.Sleep(1000); // Wait before retry
                        continue;
                    }

                    // Update game state (single pass)
                    UpdateGameState();

                    // Execute all enabled features in priority order
                    _featureManager.UpdateAll(_gameState);

                    // Notify UI
                    OnGameStateUpdated?.Invoke(_gameState);

                    _gameState.FrameCount++;
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex.Message);

                    if (ex.Message.Contains("Missed the win handler"))
                    {
                        _memoryManager.AttachToProcess(_gameProfile);
                    }
                }

                // FPS limiting
                loopTimer.Stop();
                int elapsed = (int)loopTimer.ElapsedMilliseconds;
                int sleepTime = Math.Max(0, _frameTimeMs - elapsed);

                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }

                _gameState.DeltaTime = (float)loopTimer.Elapsed.TotalSeconds;
            }
        }

        /// <summary>
        /// Update game state - reads all data once per frame
        /// </summary>
        private void UpdateGameState()
        {
            IntPtr previousWindowHandle = _gameState.GameWindowHandle;
            Size previousWindowSize = _gameState.GameWindowSize;

            // Read view matrix once per frame with caching
            _gameState.ViewMatrix = _memoryManager.ReadViewMatrix(
                _gameProfile.ViewMatrixAddress,
                TimeSpan.FromMilliseconds(16) // Cache for ~1 frame at 60fps
            );

            // Read local player
            _gameState.LocalPlayer = ReadLocalPlayer();

            // Read all entities
            _gameState.AllEntities = ReadAllEntities();

            // Filter enemies
            if (_gameState.LocalPlayer != null)
            {
                _gameState.Enemies = _gameState.AllEntities
                    .Where(e => e.team >= 0 && e.team <= 100 &&
                                e.hp > 0 && e.hp <= 100 &&
                                e.team != _gameState.LocalPlayer.team)
                    .ToList();
            }

            // Read window size/handle
            IntPtr resolvedHandle;
            if (_memoryManager.GameProcess != null)
            {
                _memoryManager.GameProcess.Refresh();
                resolvedHandle = Drawing.GetWindowHandle(_memoryManager.GameProcess.Id);
                if (resolvedHandle == IntPtr.Zero)
                {
                    resolvedHandle = _memoryManager.GameProcess.MainWindowHandle;
                }
            }
            else
            {
                resolvedHandle = IntPtr.Zero;
            }

            if (resolvedHandle == IntPtr.Zero)
            {
                resolvedHandle = Drawing.GetWindowHandle(_gameProfile.ProcessName);
            }

            Size resolvedSize = resolvedHandle == IntPtr.Zero
                ? new Size()
                : Drawing.GetSize(resolvedHandle);

            if (resolvedHandle != IntPtr.Zero && resolvedSize.Width > 0 && resolvedSize.Height > 0)
            {
                _stableWindowHandle = resolvedHandle;
                _stableWindowSize = resolvedSize;
            }

            _gameState.GameWindowHandle = _stableWindowHandle;
            _gameState.GameWindowSize = _stableWindowSize;

            if (_gameState.GameWindowHandle == IntPtr.Zero && previousWindowHandle != IntPtr.Zero)
            {
                _gameState.GameWindowHandle = previousWindowHandle;
            }

            if ((_gameState.GameWindowSize.Width <= 0 || _gameState.GameWindowSize.Height <= 0) &&
                previousWindowSize.Width > 0 && previousWindowSize.Height > 0)
            {
                _gameState.GameWindowSize = previousWindowSize;
            }

            _gameState.IsValid = true;
            _gameState.LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Read local player from memory
        /// </summary>
        private Entity ReadLocalPlayer()
        {
            string localPlayerHex = _memoryManager.ReadInt(_gameProfile.LocalPlayerAddress).ToString("X");

            return new Entity
            {
                name = _memoryManager.ReadString(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.info.name"))),
                x = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.position.x"))),
                y = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.position.y"))),
                z = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.position.z"))),
                viewX = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.rotation.yaw"))),
                viewY = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.rotation.pitch"))),
                fov = _memoryManager.ReadInt(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.info.fov"))),
                hp = _memoryManager.ReadInt(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.attributes.health"))),
                ammo1 = _memoryManager.ReadInt(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.attributes.ammo"))),
                team = _memoryManager.ReadInt(Utils.Utils.SumOfHexStrings(localPlayerHex, _gameProfile.GetOffset("player.info.team")))
            };
        }

        /// <summary>
        /// Read all entities from memory
        /// </summary>
        private List<Entity> ReadAllEntities()
        {
            var entities = new List<Entity>();
            string entityList = _memoryManager.ReadInt(_gameProfile.EntityListAddress).ToString("x");

            int maxPlayers = _gameProfile.GetSetting<int>("gameSettings.maxPlayers", 32);
            int stride = _gameProfile.GetSetting<int>("gameSettings.entityListStride", 4);
            int startOffset = _gameProfile.GetSetting<int>("gameSettings.entityListStartOffset", 4);

            for (int i = startOffset; i <= stride * maxPlayers; i += stride)
            {
                string playerEntityHex = _memoryManager.ReadInt(Utils.Utils.SumOfHexStrings(entityList, i.ToString("x"))).ToString("x");

                string name = _memoryManager.ReadString(Utils.Utils.SumOfHexStrings(playerEntityHex, _gameProfile.GetOffset("player.info.name")));

                if (string.IsNullOrEmpty(name))
                    continue;

                entities.Add(new Entity
                {
                    name = name,
                    x = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(playerEntityHex, _gameProfile.GetOffset("player.position.x"))),
                    y = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(playerEntityHex, _gameProfile.GetOffset("player.position.y"))),
                    z = _memoryManager.ReadFloat(Utils.Utils.SumOfHexStrings(playerEntityHex, _gameProfile.GetOffset("player.position.z"))),
                    hp = _memoryManager.ReadInt(Utils.Utils.SumOfHexStrings(playerEntityHex, _gameProfile.GetOffset("player.attributes.health"))),
                    team = _memoryManager.ReadInt(Utils.Utils.SumOfHexStrings(playerEntityHex, _gameProfile.GetOffset("player.info.team")))
                });
            }

            return entities;
        }
    }
}
