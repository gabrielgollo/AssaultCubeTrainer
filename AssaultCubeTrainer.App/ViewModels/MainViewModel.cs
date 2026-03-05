using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using AssaultCubeTrainer.App.Models;
using AssaultCubeTrainer.App.Services;
using AssaultCubeTrainer.App.Utils;
using AssaultCubeTrainer.Core;
using AssaultCubeTrainer.Game;
using Newtonsoft.Json.Linq;

namespace AssaultCubeTrainer.App.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TrainerService _trainerService;
        private readonly Brush _statusIdleBrush = new SolidColorBrush(Color.FromRgb(42, 58, 78));
        private readonly Brush _statusOkBrush = new SolidColorBrush(Color.FromRgb(36, 112, 84));
        private readonly Brush _statusWarnBrush = new SolidColorBrush(Color.FromRgb(138, 94, 40));
        private readonly Brush _statusErrBrush = new SolidColorBrush(Color.FromRgb(120, 52, 54));

        private ProfileItem? _selectedProfile;
        private string _processName = string.Empty;
        private string _statusText = "Idle";
        private Brush _statusBrush;
        private string _profileDetails = "Select a profile to see details.";
        private string _windowSize = "Window 0x0";
        private string _currentLife = "-";
        private string _currentAmmo = "-";
        private readonly Brush _logInfoBrush = new SolidColorBrush(Color.FromRgb(156, 178, 204));
        private readonly Brush _logWarnBrush = new SolidColorBrush(Color.FromRgb(198, 164, 90));
        private readonly Brush _logErrBrush = new SolidColorBrush(Color.FromRgb(214, 96, 98));
        private int _lifeValue = 999;
        private int _ammoValue = 999;
        private int _grenadesValue = 99;
        private bool _espEnabled;
        private bool _aimbotEnabled;
        private bool _keepAttributes;

        public ObservableCollection<ProfileItem> Profiles { get; } = new();
        public ObservableCollection<LogEntry> Logs { get; } = new();

        public RelayCommand AttachCommand { get; }
        public RelayCommand RefreshProfilesCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            _statusBrush = _statusIdleBrush;
            _trainerService = new TrainerService();
            _trainerService.GameStateUpdated += OnGameStateUpdated;
            _trainerService.Error += message => AddLog($"Error: {message}", _logErrBrush);
            _trainerService.GameDetached += () =>
            {
                UpdateStatus("Detached", _statusWarnBrush);
                AddLog("Game detached", _logWarnBrush);
            };

            AttachCommand = new RelayCommand(AttachGame, () => SelectedProfile != null);
            RefreshProfilesCommand = new RelayCommand(LoadProfiles);

            LoadProfiles();
        }

        public void Shutdown()
        {
            _trainerService.Stop();
        }

        public ProfileItem? SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (SetField(ref _selectedProfile, value))
                {
                    ProcessName = value?.ProcessName ?? string.Empty;
                    ProfileDetails = value == null ? "Select a profile to see details." : BuildProfileSummary(value.ProfilePath);
                    UpdateStatus("Idle", _statusIdleBrush);
                    AttachCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string ProcessName
        {
            get => _processName;
            set => SetField(ref _processName, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetField(ref _statusText, value);
        }

        public Brush StatusBrush
        {
            get => _statusBrush;
            set => SetField(ref _statusBrush, value);
        }

        public string ProfileDetails
        {
            get => _profileDetails;
            set => SetField(ref _profileDetails, value);
        }

        public string WindowSize
        {
            get => _windowSize;
            set => SetField(ref _windowSize, value);
        }

        public string CurrentLife
        {
            get => _currentLife;
            set => SetField(ref _currentLife, value);
        }

        public string CurrentAmmo
        {
            get => _currentAmmo;
            set => SetField(ref _currentAmmo, value);
        }

        public int LifeValue
        {
            get => _lifeValue;
            set
            {
                if (SetField(ref _lifeValue, value))
                {
                    _trainerService.UpdateAttributeValues(_lifeValue, _ammoValue, _grenadesValue);
                }
            }
        }

        public int AmmoValue
        {
            get => _ammoValue;
            set
            {
                if (SetField(ref _ammoValue, value))
                {
                    _trainerService.UpdateAttributeValues(_lifeValue, _ammoValue, _grenadesValue);
                }
            }
        }

        public int GrenadesValue
        {
            get => _grenadesValue;
            set
            {
                if (SetField(ref _grenadesValue, value))
                {
                    _trainerService.UpdateAttributeValues(_lifeValue, _ammoValue, _grenadesValue);
                }
            }
        }

        public bool EspEnabled
        {
            get => _espEnabled;
            set
            {
                if (SetField(ref _espEnabled, value))
                {
                    _trainerService.SetEspEnabled(value);
                }
            }
        }

        public bool AimbotEnabled
        {
            get => _aimbotEnabled;
            set
            {
                if (SetField(ref _aimbotEnabled, value))
                {
                    _trainerService.SetAimbotEnabled(value);
                }
            }
        }

        public bool KeepAttributes
        {
            get => _keepAttributes;
            set
            {
                if (SetField(ref _keepAttributes, value))
                {
                    _trainerService.SetKeepAttributes(value);
                }
            }
        }

        private void AttachGame()
        {
            if (SelectedProfile == null)
            {
                UpdateStatus("Select profile", _statusWarnBrush);
                return;
            }

            bool attached = _trainerService.Start(SelectedProfile.ProfilePath);
            if (attached)
            {
                UpdateStatus("Attached", _statusOkBrush);
                AddLog($"Attached to {SelectedProfile.Name}", _logInfoBrush);
            }
            else
            {
                UpdateStatus("Not found", _statusErrBrush);
                AddLog($"Game not found: {SelectedProfile.ProcessName}", _logErrBrush);
            }
        }

        private void OnGameStateUpdated(GameState state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (state.LocalPlayer != null)
                {
                    CurrentLife = state.LocalPlayer.hp < 0 ? "0" : state.LocalPlayer.hp.ToString();
                    CurrentAmmo = state.LocalPlayer.ammo1 < 0 ? "0" : state.LocalPlayer.ammo1.ToString();
                }

                WindowSize = $"Window {state.GameWindowSize.Width}x{state.GameWindowSize.Height}";
            });
        }

        private void LoadProfiles()
        {
            Profiles.Clear();
            string profilesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Profiles");

            if (!Directory.Exists(profilesDir))
            {
                UpdateStatus("Profiles missing", _statusErrBrush);
                ProfileDetails = "No profiles available.";
                return;
            }

            var files = Directory.GetFiles(profilesDir, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var profile = GameProfileLoader.LoadFromFile(file);
                    Profiles.Add(new ProfileItem
                    {
                        Name = profile.GameName,
                        ProcessName = profile.ProcessName,
                        ProfilePath = file
                    });
                }
                catch (Exception ex)
                {
                    AddLog($"Invalid profile: {Path.GetFileName(file)} ({ex.Message})", _logWarnBrush);
                }
            }

            var sorted = Profiles.OrderBy(p => p.Name).ToList();
            Profiles.Clear();
            foreach (var item in sorted)
            {
                Profiles.Add(item);
            }

            if (Profiles.Count > 0)
            {
                SelectedProfile = Profiles[0];
                UpdateStatus("Idle", _statusIdleBrush);
            }
            else
            {
                UpdateStatus("No profiles", _statusWarnBrush);
                ProfileDetails = "No profiles available.";
            }
        }

        private void UpdateStatus(string text, Brush brush)
        {
            StatusText = text;
            StatusBrush = brush;
        }

        private void AddLog(string message, Brush brush)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            Application.Current.Dispatcher.Invoke(() =>
            {
                Logs.Insert(0, new LogEntry
                {
                    Message = line,
                    LevelBrush = brush
                });
            });
        }

        private string BuildProfileSummary(string profilePath)
        {
            try
            {
                var json = JObject.Parse(File.ReadAllText(profilePath));
                string gameName = json["gameName"]?.ToString() ?? "Unknown";
                string processName = json["processName"]?.ToString() ?? "Unknown";

                string maxPlayers = json.SelectToken("gameSettings.maxPlayers")?.ToString() ?? "-";
                string stride = json.SelectToken("gameSettings.entityListStride")?.ToString() ?? "-";

                var aimbot = json.SelectToken("features.aimbot");
                var esp = json.SelectToken("features.esp");
                var attributeKeeper = json.SelectToken("features.attributeKeeper");

                string aimbotLine = BuildFeatureLine("Aimbot", aimbot, new[]
                {
                    "aimMode",
                    "angleAlgorithm",
                    "writeCulture",
                    "matrixOrder",
                    "requireOnScreen",
                    "smoothing",
                    "fovLimit",
                    "targetBone"
                });

                string diagnosticsLine = BuildFeatureLine("Aimbot Diagnostics", aimbot?["diagnostics"], new[]
                {
                    "logOnly",
                    "intervalMs",
                    "logPath"
                });

                string espLine = BuildFeatureLine("ESP", esp, new[]
                {
                    "boxes",
                    "boxScale",
                    "yOffsetPx",
                    "healthBars",
                    "names",
                    "distance",
                    "axisMode"
                });

                string attrLine = BuildFeatureLine("Attribute Keeper", attributeKeeper, new[]
                {
                    "keepHealth",
                    "keepAmmo",
                    "keepGrenades"
                });

                return string.Join(Environment.NewLine, new[]
                {
                    $"Game: {gameName}",
                    $"Process: {processName}",
                    $"Settings: MaxPlayers {maxPlayers}, EntityStride {stride}",
                    aimbotLine,
                    diagnosticsLine,
                    espLine,
                    attrLine
                });
            }
            catch
            {
                return "Unable to load profile details.";
            }
        }

        private string BuildFeatureLine(string name, JToken? featureToken, string[] fields)
        {
            if (featureToken == null)
            {
                return $"{name}: Not configured";
            }

            string enabled = featureToken["enabled"]?.ToString() ?? "false";
            var parts = new[]
            {
                $"{name}: {(enabled == "true" ? "Enabled" : "Disabled")}"
            }.ToList();

            foreach (var field in fields)
            {
                var value = featureToken[field];
                if (value != null)
                {
                    parts.Add($"{field} {value}");
                }
            }

            return string.Join(", ", parts);
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
