using System;
using AssaultCubeTrainer.Core;
using AssaultCubeTrainer.Features;
using AssaultCubeTrainer.Game;

namespace AssaultCubeTrainer.App.Services
{
    public class TrainerService
    {
        private readonly TrainerEngine _engine;
        private readonly AimbotFeature _aimbotFeature;
        private readonly ESPFeature _espFeature;
        private readonly AttributeKeeperFeature _attributeKeeperFeature;

        public event Action<GameState>? GameStateUpdated;
        public event Action<string>? Error;
        public event Action? GameDetached;

        public bool IsAttached => _engine.IsAttached;

        public TrainerService()
        {
            _engine = new TrainerEngine();
            _engine.TargetFPS = 240;

            _aimbotFeature = new AimbotFeature();
            _espFeature = new ESPFeature();
            _attributeKeeperFeature = new AttributeKeeperFeature();

            _engine.RegisterFeature(_attributeKeeperFeature);
            _engine.RegisterFeature(_espFeature);
            _engine.RegisterFeature(_aimbotFeature);

            _engine.OnGameStateUpdated += state => GameStateUpdated?.Invoke(state);
            _engine.OnError += message => Error?.Invoke(message);
            _engine.OnGameDetached += () => GameDetached?.Invoke();
        }

        public bool Start(string profilePath)
        {
            try
            {
                _engine.Stop();
                var profile = GameProfileLoader.LoadFromFile(profilePath);
                if (_engine.Initialize(profile))
                {
                    _engine.Start();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(ex.Message);
            }

            return false;
        }

        public void Stop()
        {
            _engine.Stop();
        }

        public void SetEspEnabled(bool enabled)
        {
            _espFeature.IsEnabled = enabled;
            if (!enabled)
            {
                _espFeature.Cleanup();
            }
        }

        public void SetAimbotEnabled(bool enabled) => _aimbotFeature.IsEnabled = enabled;

        public void SetKeepAttributes(bool enabled) => _attributeKeeperFeature.IsEnabled = enabled;

        public void UpdateAttributeValues(int life, int ammo, int grenades)
        {
            _attributeKeeperFeature.HealthValue = life;
            _attributeKeeperFeature.AmmoValue = ammo;
            _attributeKeeperFeature.GrenadesValue = grenades;
        }
    }
}
