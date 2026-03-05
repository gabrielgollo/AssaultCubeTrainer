using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AssaultCubeTrainer;
using AssaultCubeTrainer.Core;
using AssaultCubeTrainer.Game;

namespace AssaultCubeTrainer.Features
{
    /// <summary>
    /// Aimbot feature - automatically aims at closest enemy
    /// </summary>
    public class AimbotFeature : ICheatFeature
    {
        private IGameProfile _profile;
        private MemoryManager _memory;

        public string Name => "Aimbot";
        public int Priority => 10; // Run after ESP
        public bool IsEnabled { get; set; }

        // Settings from profile
        private float _smoothing;
        private float _fovLimit;
        private bool _invertPitch;
        private bool _requireVisible;
        private string _targetBone = "head";
        private float _headOffset;
        private string _axisMode = "zup";
        private float _maxPitch = 89.0f;
        private float _maxTargetDistance = 300.0f;
        private float _maxSnapAngle = 85.0f;
        private string _aimMode = "angle";
        private string _angleAlgorithm = "adaptive";
        private string _writeCulture = "current";
        private float _maxScreenDistance = 400.0f;
        private bool _useRowMajorThisFrame;
        private string _matrixOrder = "auto";
        private string _angleUnit = "deg";
        private float _yawOffset = 90.0f;
        private float _yawMin = 0.0f;
        private float _yawMax = 360.0f;
        private float _pitchMin = -90.0f;
        private float _pitchMax = 90.0f;
        private bool _normalizeYaw = true;
        private bool _clampPitch = true;
        private bool _allowOppositeYawOffset;
        private bool _strictPitchLimits;
        private bool _swapYawPitch;
        private bool _simpleAngleDiff;
        private bool _autoPitchSign;
        private bool _swapAxisYZ;
        private float _screenGain = 0.35f;
        private float _screenYawSign = 1.0f;
        private float _screenPitchSign = 1.0f;
        private float _maxYawStep = 8.0f;
        private float _maxPitchStep = 6.0f;
        private float _minTargetDistance = 2.0f;
        private bool _targetLockEnabled = true;
        private bool _requireOnScreen;
        private bool _diagnosticsEnabled;
        private bool _diagnosticsLogOnly;
        private int _diagnosticsIntervalMs = 250;
        private DateTime _nextDiagnosticAt = DateTime.MinValue;
        private string _diagnosticsPath = string.Empty;
        private bool _lastUseRowMajor;
        private string? _lockedName;
        private int _lockedTeam;
        private Vector3 _lockedPos = new Vector3();
        private bool _hasLock;

        public void Initialize(IGameProfile profile, MemoryManager memory)
        {
            _profile = profile;
            _memory = memory;
            _lastUseRowMajor = false;
            _useRowMajorThisFrame = false;
            _hasLock = false;
            _lockedName = null;
            _lockedTeam = 0;
            _lockedPos = new Vector3();
            _nextDiagnosticAt = DateTime.MinValue;

            _smoothing = profile.GetSetting<float>("features.aimbot.smoothing", 0.0f);
            _fovLimit = profile.GetSetting<float>("features.aimbot.fovLimit", 180.0f);
            _invertPitch = profile.GetSetting<bool>("features.aimbot.invertPitch", false);
            _requireVisible = profile.GetSetting<bool>("features.aimbot.requireVisible", true);
            _targetBone = profile.GetSetting<string>("features.aimbot.targetBone", "head") ?? "head";
            _headOffset = profile.GetSetting<float>("features.aimbot.headOffset", 0.0f);
            _axisMode = NormalizeAxisMode(profile.GetSetting<string>("features.aimbot.axisMode", "zup"));
            _maxPitch = profile.GetSetting<float>("features.aimbot.maxPitch", 89.0f);
            _maxTargetDistance = profile.GetSetting<float>("features.aimbot.maxTargetDistance", 300.0f);
            _maxSnapAngle = profile.GetSetting<float>("features.aimbot.maxSnapAngle", 85.0f);
            _aimMode = NormalizeAimMode(profile.GetSetting<string>("features.aimbot.aimMode", "angle"));
            _angleAlgorithm = NormalizeAngleAlgorithm(profile.GetSetting<string>("features.aimbot.angleAlgorithm", "adaptive"));
            _writeCulture = NormalizeWriteCulture(profile.GetSetting<string>("features.aimbot.writeCulture", "current"));
            _maxScreenDistance = profile.GetSetting<float>("features.aimbot.maxScreenDistance", 400.0f);
            _matrixOrder = NormalizeMatrixOrder(
                profile.GetSetting<string>(
                    "features.aimbot.matrixOrder",
                    profile.GetSetting<string>("features.esp.matrixOrder", "auto")));
            _angleUnit = (profile.GetSetting<string>("features.aimbot.angleUnit", "deg") ?? "deg").ToLowerInvariant();
            _yawOffset = profile.GetSetting<float>("features.aimbot.yawOffset", 90.0f);
            _yawMin = profile.GetSetting<float>("features.aimbot.yawRangeMin", 0.0f);
            _yawMax = profile.GetSetting<float>("features.aimbot.yawRangeMax", 360.0f);
            _pitchMin = profile.GetSetting<float>("features.aimbot.pitchRangeMin", -90.0f);
            _pitchMax = profile.GetSetting<float>("features.aimbot.pitchRangeMax", 90.0f);
            _normalizeYaw = profile.GetSetting<bool>("features.aimbot.normalizeYaw", true);
            _clampPitch = profile.GetSetting<bool>("features.aimbot.clampPitch", true);
            _allowOppositeYawOffset = profile.GetSetting<bool>("features.aimbot.allowOppositeYawOffset", false);
            _strictPitchLimits = profile.GetSetting<bool>("features.aimbot.strictPitchLimits", false);
            _swapYawPitch = profile.GetSetting<bool>("features.aimbot.swapYawPitch", false);
            _simpleAngleDiff = profile.GetSetting<bool>("features.aimbot.simpleAngleDiff", false);
            _autoPitchSign = profile.GetSetting<bool>("features.aimbot.autoPitchSign", false);
            _swapAxisYZ = profile.GetSetting<bool>("features.aimbot.swapAxisYZ", false);
            _screenGain = profile.GetSetting<float>("features.aimbot.screenGain", 0.35f);
            _screenYawSign = profile.GetSetting<float>("features.aimbot.screenYawSign", 1.0f);
            _screenPitchSign = profile.GetSetting<float>("features.aimbot.screenPitchSign", 1.0f);
            _maxYawStep = profile.GetSetting<float>("features.aimbot.maxYawStep", 8.0f);
            _maxPitchStep = profile.GetSetting<float>("features.aimbot.maxPitchStep", 6.0f);
            _minTargetDistance = profile.GetSetting<float>("features.aimbot.minTargetDistance", 2.0f);
            _targetLockEnabled = profile.GetSetting<bool>("features.aimbot.targetLock", true);
            _requireOnScreen = profile.GetSetting<bool>("features.aimbot.requireOnScreen", false);
            _diagnosticsEnabled = profile.GetSetting<bool>("features.aimbot.diagnostics.enabled", false);
            _diagnosticsLogOnly = profile.GetSetting<bool>("features.aimbot.diagnostics.logOnly", false);
            _diagnosticsIntervalMs = Math.Max(50, profile.GetSetting<int>("features.aimbot.diagnostics.intervalMs", 250));
            _diagnosticsPath = ResolveDiagnosticsPath(
                profile.GetSetting<string>("features.aimbot.diagnostics.logPath", "Logs/aimbot-diagnostics.log"));
        }

        public void Update(GameState gameState)
        {
            if (!gameState.IsValid || gameState.Enemies.Count == 0 || gameState.LocalPlayer == null)
                return;

            var localPlayer = gameState.LocalPlayer;
            if (localPlayer.hp <= 0)
            {
                return;
            }
            string playerEntityHex = _memory.ReadInt(_profile.LocalPlayerAddress).ToString("X");

            string viewXAddress = Utils.Utils.SumOfHexStrings(playerEntityHex, _profile.GetOffset("player.rotation.yaw"));
            string viewYAddress = Utils.Utils.SumOfHexStrings(playerEntityHex, _profile.GetOffset("player.rotation.pitch"));
            if (_swapYawPitch)
            {
                (viewXAddress, viewYAddress) = (viewYAddress, viewXAddress);
            }

            float currYaw = _memory.ReadFloat(viewXAddress);
            float currPitch = _memory.ReadFloat(viewYAddress);
            _useRowMajorThisFrame = DetermineProjectionMode(gameState, localPlayer);

            if (_aimMode == "screen")
            {
                if (TryGetScreenAim(gameState, localPlayer, currYaw, currPitch, out Vector2 screenAim))
                {
                    float targetYaw = screenAim.x;
                    float targetPitch = screenAim.y;

                    if (_smoothing > 0)
                    {
                        targetYaw = Lerp(currYaw, targetYaw, 1.0f / _smoothing);
                        targetPitch = Lerp(currPitch, targetPitch, 1.0f / _smoothing);
                    }

                    targetYaw = ApplyYawStepLimit(currYaw, targetYaw);
                    targetPitch = ApplyPitchStepLimit(currPitch, targetPitch);

                    WriteAimIfValid(viewXAddress, viewYAddress, targetYaw, targetPitch);
                }

                return;
            }

            if (_angleAlgorithm == "legacy")
            {
                UpdateLegacyAngleAim(gameState, localPlayer, currYaw, currPitch, viewXAddress, viewYAddress);
                return;
            }

            // Find closest target by angle only (independent from view-matrix projection).
            List<TargetCandidate> validTargets = gameState.Enemies
                .Where(e => e.hp > 0)
                .Where(e => e.team != localPlayer.team)
                .Where(e => IsTargetSane(localPlayer, e))
                .Where(e => !_requireOnScreen || IsTargetOnScreen(e, gameState))
                .Where(e => !_requireVisible || IsTargetVisible(e, gameState))
                .Select(e => new TargetCandidate
                {
                    Enemy = e,
                    Angles = CalculateBestAim(localPlayer, e, currYaw, currPitch),
                    Distance = CalculateAngleDifference(currYaw, currPitch, e, localPlayer)
                })
                .Where(t => _fovLimit <= 0 || t.Distance <= _fovLimit)
                .OrderBy(t => t.Distance)
                .ToList();

            MaybeWriteDiagnostics(gameState, localPlayer, currYaw, currPitch, validTargets);

            if (validTargets.Count > 0)
            {
                TargetCandidate target = SelectTarget(validTargets);
                if (target.Distance > _maxSnapAngle)
                {
                    return;
                }

                // Apply smoothing if configured
                float targetYaw = target.Angles.x;
                float targetPitch = target.Angles.y;

                if (_smoothing > 0)
                {
                    targetYaw = Lerp(currYaw, targetYaw, 1.0f / _smoothing);
                    targetPitch = Lerp(currPitch, targetPitch, 1.0f / _smoothing);
                }

                targetYaw = ApplyYawStepLimit(currYaw, targetYaw);
                targetPitch = ApplyPitchStepLimit(currPitch, targetPitch);

                WriteAimIfValid(viewXAddress, viewYAddress, targetYaw, targetPitch);
            }
        }

        private void UpdateLegacyAngleAim(
            GameState gameState,
            Entity localPlayer,
            float currYaw,
            float currPitch,
            string viewXAddress,
            string viewYAddress)
        {
            List<TargetCandidate> candidates = gameState.Enemies
                .Where(e => e.hp > 0)
                .Where(e => e.team != localPlayer.team)
                .Where(e => IsTargetSane(localPlayer, e))
                .Where(e => IsTargetOnScreen(e, gameState))
                .Select(e =>
                {
                    Vector2 angles = CalculateLegacyAim(localPlayer, e);
                    return new TargetCandidate
                    {
                        Enemy = e,
                        Angles = angles,
                        Distance = CalculateLegacyDistance(currYaw, currPitch, angles)
                    };
                })
                .Where(t => _fovLimit <= 0 || t.Distance <= _fovLimit)
                .OrderBy(t => t.Distance)
                .ToList();

            MaybeWriteDiagnostics(gameState, localPlayer, currYaw, currPitch, candidates);

            if (candidates.Count == 0)
            {
                return;
            }

            TargetCandidate target = candidates[0];
            if (target.Distance > _maxSnapAngle)
            {
                return;
            }

            WriteLegacyAim(viewXAddress, viewYAddress, target.Angles.x, target.Angles.y);
        }

        private Vector2 CalculateLegacyAim(Entity player, Entity enemy)
        {
            GetAimCoords(player, enemy, out float px, out float py, out float pz, out float ex, out float ey, out float ez);

            float dx = ex - px;
            float dy = ey - py;
            float dz = (ez + GetTargetZOffset()) - pz;

            float distanceHorizontal = (float)Math.Sqrt(dx * dx + dy * dy);
            if (distanceHorizontal < 0.001f)
            {
                distanceHorizontal = 0.001f;
            }

            float yaw = (float)(Math.Atan2(dy, dx) * (180.0 / Math.PI)) + _yawOffset;
            float pitch = (float)(Math.Atan2(dz, distanceHorizontal) * (180.0 / Math.PI));
            if (_invertPitch)
            {
                pitch = -pitch;
            }

            pitch = Math.Max(-90.0f, Math.Min(90.0f, pitch));
            return new Vector2 { x = yaw, y = pitch };
        }

        private static float CalculateLegacyDistance(float currYaw, float currPitch, Vector2 targetAngles)
        {
            float yawDiff = Math.Abs(currYaw - targetAngles.x);
            float pitchDiff = Math.Abs(currPitch - targetAngles.y);
            return yawDiff + pitchDiff;
        }

        private bool TryGetScreenAim(GameState gameState, Entity localPlayer, float currYaw, float currPitch, out Vector2 aim)
        {
            aim = new Vector2();
            if (gameState.GameWindowSize.Width <= 0 || gameState.GameWindowSize.Height <= 0)
            {
                return false;
            }

            float centerX = gameState.GameWindowSize.Width / 2f;
            float centerY = gameState.GameWindowSize.Height / 2f;
            Entity? best = null;
            float bestScore = float.MaxValue;
            float bestDx = 0;
            float bestDy = 0;

            foreach (Entity enemy in gameState.Enemies.Where(e => e.hp > 0 && IsTargetSane(localPlayer, e)))
            {
                if (_requireVisible && !IsTargetVisible(enemy, gameState))
                {
                    continue;
                }

                PointF p = WorldToScreen(gameState.ViewMatrix, enemy, gameState.GameWindowSize);
                if (p.X < 0 || p.X > gameState.GameWindowSize.Width || p.Y < 0 || p.Y > gameState.GameWindowSize.Height)
                {
                    continue;
                }

                float dx = p.X - centerX;
                float dy = p.Y - centerY;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dist > _maxScreenDistance)
                {
                    continue;
                }

                if (dist < bestScore)
                {
                    bestScore = dist;
                    best = enemy;
                    bestDx = dx;
                    bestDy = dy;
                }
            }

            if (best == null)
            {
                return false;
            }

            float fov = localPlayer.fov;
            if (fov < 20 || fov > 170)
            {
                fov = 90f;
            }

            float gain = Math.Max(0.05f, Math.Min(1.0f, _screenGain));
            float yawDelta = _screenYawSign * (bestDx / (gameState.GameWindowSize.Width / 2f)) * (fov / 2f) * gain;
            float pitchDelta = _screenPitchSign * (bestDy / (gameState.GameWindowSize.Height / 2f)) * (fov / 2f) * gain;
            if (_invertPitch)
            {
                pitchDelta = -pitchDelta;
            }

            aim = new Vector2
            {
                x = NormalizeYaw(currYaw + yawDelta),
                y = ClampPitch(currPitch + pitchDelta)
            };

            return true;
        }

        private struct AimCandidate
        {
            public float Yaw;
            public float Pitch;
            public float Score;
        }

        private Vector2 CalculateAimZUp(Entity player, Entity enemy, float yawOffset)
        {
            GetAimCoords(player, enemy, out float px, out float py, out float pz, out float ex, out float ey, out float ez);
            float dx = ex - px;
            float dy = ey - py;
            float targetZ = ez + GetTargetZOffset();
            float dz = targetZ - pz;

            float distanceHorizontal = (float)Math.Sqrt(dx * dx + dy * dy);
            if (distanceHorizontal < 0.001f)
            {
                distanceHorizontal = 0.001f;
            }

            float yaw = (float)Math.Atan2(dy, dx);
            float targetYaw = NormalizeYaw(ToAngleUnits(yaw) + yawOffset);
            float pitch = (float)Math.Atan2(dz, distanceHorizontal);
            float targetPitch = ClampPitch(ToAngleUnits(pitch));
            return new Vector2 { x = targetYaw, y = targetPitch };
        }

        private Vector2 CalculateAimYUp(Entity player, Entity enemy, float yawOffset)
        {
            GetAimCoords(player, enemy, out float px, out float py, out float pz, out float ex, out float ey, out float ez);
            float dx = ex - px;
            float dz = ez - pz;
            float targetY = ey + GetTargetZOffset();
            float dy = targetY - py;

            float distanceHorizontal = (float)Math.Sqrt(dx * dx + dz * dz);
            if (distanceHorizontal < 0.001f)
            {
                distanceHorizontal = 0.001f;
            }

            float yaw = (float)Math.Atan2(dz, dx);
            float targetYaw = NormalizeYaw(ToAngleUnits(yaw) + yawOffset);
            float pitch = (float)Math.Atan2(dy, distanceHorizontal);
            float targetPitch = ClampPitch(ToAngleUnits(pitch));
            return new Vector2 { x = targetYaw, y = targetPitch };
        }

        private float GetTargetZOffset()
        {
            if (_targetBone.Equals("head", StringComparison.OrdinalIgnoreCase))
            {
                return _headOffset;
            }

            return _headOffset * 0.5f;
        }

        private void GetAimCoords(Entity player, Entity enemy, out float px, out float py, out float pz, out float ex, out float ey, out float ez)
        {
            if (_swapAxisYZ)
            {
                px = player.x;
                py = player.z;
                pz = player.y;
                ex = enemy.x;
                ey = enemy.z;
                ez = enemy.y;
                return;
            }

            px = player.x;
            py = player.y;
            pz = player.z;
            ex = enemy.x;
            ey = enemy.y;
            ez = enemy.z;
        }

        private Vector2 CalculateBestAim(Entity player, Entity enemy, float currentYaw, float currentPitch)
        {
            AimCandidate best = new AimCandidate { Score = float.MaxValue };

            if (_axisMode == "zup" || _axisMode == "auto")
            {
                // Primary: single +90 offset — matches the original working code exactly
                EvaluateCandidate(CalculateAimZUp(player, enemy, _yawOffset), currentYaw, currentPitch, ref best);
                if (_allowOppositeYawOffset)
                {
                    EvaluateCandidate(CalculateAimZUp(player, enemy, -_yawOffset), currentYaw, currentPitch, ref best);
                }
            }
            if (_axisMode == "yup" || _axisMode == "auto")
            {
                EvaluateCandidate(CalculateAimYUp(player, enemy, _yawOffset), currentYaw, currentPitch, ref best);
                if (_allowOppositeYawOffset)
                {
                    EvaluateCandidate(CalculateAimYUp(player, enemy, -_yawOffset), currentYaw, currentPitch, ref best);
                }
            }

            if (best.Score == float.MaxValue)
            {
                Vector2 fallbackAim = CalculateAimZUp(player, enemy, _yawOffset);
                return new Vector2 { x = NormalizeYaw(fallbackAim.x), y = ClampPitch(fallbackAim.y) };
            }

            return new Vector2 { x = best.Yaw, y = best.Pitch };
        }

        private void EvaluateCandidate(Vector2 baseAim, float currentYaw, float currentPitch, ref AimCandidate best)
        {
            // Use direct pitch (same as the original simple aimbot logic).
            // Only invert if explicitly configured.
            float pitch = _invertPitch ? -baseAim.y : baseAim.y;
            if (_autoPitchSign)
            {
                float pitchAlt = -pitch;
                float scoreAlt = YawDelta(currentYaw, baseAim.x) + Math.Abs(currentPitch - pitchAlt);
                float scoreBase = YawDelta(currentYaw, baseAim.x) + Math.Abs(currentPitch - pitch);
                if (scoreAlt < scoreBase)
                {
                    pitch = pitchAlt;
                }
            }

            float yawDiff = YawDelta(currentYaw, baseAim.x);
            float pitchDiff = Math.Abs(currentPitch - pitch);
            float score = yawDiff + pitchDiff;

            if (score < best.Score)
            {
                best = new AimCandidate
                {
                    Yaw = NormalizeYaw(baseAim.x),
                    Pitch = ClampPitch(pitch),
                    Score = score
                };
            }
        }

        /// <summary>
        /// Calculate angle difference between current and target
        /// </summary>
        private float CalculateAngleDifference(float currYaw, float currPitch, Entity enemy, Entity player)
        {
            var targetAngles = CalculateBestAim(player, enemy, currYaw, currPitch);
            float yawDiff = _simpleAngleDiff
                ? Math.Abs(currYaw - targetAngles.x)
                : YawDelta(currYaw, targetAngles.x);
            float pitchDiff = Math.Abs(currPitch - targetAngles.y);
            return yawDiff + pitchDiff;
        }

        /// <summary>
        /// Check if target is visible on screen
        /// </summary>
        private bool IsTargetVisible(Entity enemy, GameState gameState)
        {
            return IsTargetOnScreen(enemy, gameState);
        }

        private bool IsTargetOnScreen(Entity enemy, GameState gameState)
        {
            if (gameState.GameWindowSize.Width <= 0 || gameState.GameWindowSize.Height <= 0)
            {
                return false;
            }

            if (!IsInFrontByMatrix(gameState.ViewMatrix, enemy))
            {
                return false;
            }

            var screenPos = WorldToScreen(gameState.ViewMatrix, enemy, gameState.GameWindowSize);
            return screenPos.X >= 0 && screenPos.X <= gameState.GameWindowSize.Width &&
                   screenPos.Y >= 0 && screenPos.Y <= gameState.GameWindowSize.Height;
        }

        /// <summary>
        /// Linear interpolation for smooth aiming
        /// </summary>
        private float Lerp(float from, float to, float t)
        {
            return from + (to - from) * Math.Min(1.0f, Math.Max(0.0f, t));
        }

        private float ToAngleUnits(float radians)
        {
            if (_angleUnit == "rad")
            {
                return radians;
            }

            return (float)(radians * (180.0 / Math.PI));
        }

        private float NormalizeYaw(float yaw)
        {
            if (!_normalizeYaw)
            {
                return yaw;
            }

            float min = _yawMin;
            float max = _yawMax;
            float range = max - min;
            if (range <= 0.001f)
            {
                return yaw;
            }

            float result = (yaw - min) % range;
            if (result < 0f)
            {
                result += range;
            }

            return result + min;
        }

        private float YawDelta(float from, float to)
        {
            float range = Math.Abs(_yawMax - _yawMin);
            if (range <= 0.001f)
            {
                range = 360f;
            }

            float delta = Math.Abs(NormalizeYaw(from) - NormalizeYaw(to));
            return delta > range * 0.5f ? range - delta : delta;
        }

        private float NormalizeSignedYaw(float yaw)
        {
            float normalized = NormalizeYaw(yaw);
            float min = _yawMin;
            float max = _yawMax;
            float range = Math.Abs(max - min);
            if (range <= 0.001f)
            {
                range = 360f;
            }
            float mid = min + (range * 0.5f);
            if (normalized > mid)
            {
                normalized -= range;
            }

            return normalized;
        }

        private float ApplyYawStepLimit(float currentYaw, float targetYaw)
        {
            float maxStep = Math.Max(0.25f, _maxYawStep);
            float delta = NormalizeSignedYaw(targetYaw - currentYaw);
            float clamped = Math.Max(-maxStep, Math.Min(maxStep, delta));
            return NormalizeYaw(currentYaw + clamped);
        }

        private float ApplyPitchStepLimit(float currentPitch, float targetPitch)
        {
            float maxStep = Math.Max(0.25f, _maxPitchStep);
            float delta = targetPitch - currentPitch;
            float clamped = Math.Max(-maxStep, Math.Min(maxStep, delta));
            return ClampPitch(currentPitch + clamped);
        }

        private float ClampPitch(float pitch)
        {
            if (!_clampPitch)
            {
                return pitch;
            }

            float min = _pitchMin;
            float max = _pitchMax;
            if (max <= min)
            {
                float limit = Math.Max(10.0f, Math.Min(89.0f, _maxPitch));
                min = -limit;
                max = limit;
            }

            return Math.Max(min, Math.Min(max, pitch));
        }

        private bool IsTargetSane(Entity localPlayer, Entity target)
        {
            if (localPlayer == null || target == null)
            {
                return false;
            }

            if (!IsFiniteCoord(localPlayer) || !IsFiniteCoord(target))
            {
                return false;
            }

            float dx = target.x - localPlayer.x;
            float dy = target.y - localPlayer.y;
            float dz = target.z - localPlayer.z;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (distance < _minTargetDistance || distance > _maxTargetDistance)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(localPlayer.name) &&
                !string.IsNullOrWhiteSpace(target.name) &&
                string.Equals(localPlayer.name.Trim(), target.name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private class TargetCandidate
        {
            public Entity Enemy { get; set; } = null!;
            public Vector2 Angles { get; set; }
            public float Distance { get; set; }
        }

        private TargetCandidate SelectTarget(List<TargetCandidate> candidates)
        {
            if (!_targetLockEnabled)
            {
                _hasLock = false;
                return candidates.First();
            }

            if (_hasLock)
            {
                TargetCandidate? locked = FindLockedTarget(candidates);
                if (locked != null)
                {
                    return locked;
                }
            }

            TargetCandidate best = candidates.First();
            _lockedName = best.Enemy.name?.Trim();
            _lockedTeam = best.Enemy.team;
            _lockedPos = new Vector3 { x = best.Enemy.x, y = best.Enemy.y, z = best.Enemy.z };
            _hasLock = true;
            return best;
        }

        private TargetCandidate? FindLockedTarget(List<TargetCandidate> candidates)
        {
            if (!_hasLock || string.IsNullOrWhiteSpace(_lockedName))
            {
                return null;
            }

            const float maxLockDistance = 20.0f;
            TargetCandidate? best = null;
            float bestDist = float.MaxValue;

            foreach (TargetCandidate candidate in candidates)
            {
                if (!string.Equals(candidate.Enemy.name?.Trim(), _lockedName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (candidate.Enemy.team != _lockedTeam)
                {
                    continue;
                }

                float dx = candidate.Enemy.x - _lockedPos.x;
                float dy = candidate.Enemy.y - _lockedPos.y;
                float dz = candidate.Enemy.z - _lockedPos.z;
                float dist = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = candidate;
                }
            }

            if (best != null && bestDist <= maxLockDistance)
            {
                _lockedPos = new Vector3 { x = best.Enemy.x, y = best.Enemy.y, z = best.Enemy.z };
                return best;
            }

            _hasLock = false;
            return null;
        }

        private void WriteAimIfValid(string yawAddress, string pitchAddress, float targetYaw, float targetPitch)
        {
            if (!IsFinite(targetYaw) || !IsFinite(targetPitch))
            {
                return;
            }

            if (_strictPitchLimits && (targetPitch < _pitchMin || targetPitch > _pitchMax))
            {
                return;
            }

            float yaw = NormalizeYaw(targetYaw);
            float pitch = ClampPitch(targetPitch);
            if (!IsFinite(yaw) || !IsFinite(pitch))
            {
                return;
            }

            if (_diagnosticsEnabled && _diagnosticsLogOnly)
            {
                return;
            }

            _memory.WriteMemory(yawAddress, "float", FormatFloatForMemory(yaw));
            _memory.WriteMemory(pitchAddress, "float", FormatFloatForMemory(pitch));
        }

        private void WriteLegacyAim(string yawAddress, string pitchAddress, float targetYaw, float targetPitch)
        {
            if (!IsFinite(targetYaw) || !IsFinite(targetPitch))
            {
                return;
            }

            if (_diagnosticsEnabled && _diagnosticsLogOnly)
            {
                return;
            }

            _memory.WriteMemory(yawAddress, "float", FormatFloatForMemory(targetYaw));
            _memory.WriteMemory(pitchAddress, "float", FormatFloatForMemory(targetPitch));
        }

        private string FormatFloatForMemory(float value)
        {
            CultureInfo culture = _writeCulture == "invariant"
                ? CultureInfo.InvariantCulture
                : CultureInfo.CurrentCulture;

            return value.ToString("G9", culture);
        }

        private static bool IsFinite(float value)
        {
            return !(float.IsNaN(value) || float.IsInfinity(value));
        }

        private void MaybeWriteDiagnostics(
            GameState gameState,
            Entity localPlayer,
            float currYaw,
            float currPitch,
            List<TargetCandidate> validTargets)
        {
            if (!_diagnosticsEnabled || DateTime.UtcNow < _nextDiagnosticAt)
            {
                return;
            }

            _nextDiagnosticAt = DateTime.UtcNow.AddMilliseconds(_diagnosticsIntervalMs);

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('[')
                  .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture))
                  .Append("] profile=").Append(_profile.GameName)
                  .Append(" mode=").Append(_aimMode)
                  .Append(" algorithm=").Append(_angleAlgorithm)
                  .Append(" axis=").Append(_axisMode)
                  .Append(" unit=").Append(_angleUnit)
                  .Append(" writeCulture=").Append(_writeCulture)
                  .Append(" yawOffset=").Append(_yawOffset.ToString("F3", CultureInfo.InvariantCulture))
                  .Append(" invertPitch=").Append(_invertPitch)
                  .Append(" swapAxisYZ=").Append(_swapAxisYZ)
                  .Append(" swapYawPitch=").Append(_swapYawPitch)
                  .Append(" matrixOrder=").Append(_matrixOrder)
                  .Append(" onScreen=").Append(_requireOnScreen)
                  .Append(" visible=").Append(_requireVisible)
                  .Append(" currYaw=").Append(currYaw.ToString("F3", CultureInfo.InvariantCulture))
                  .Append(" currPitch=").Append(currPitch.ToString("F3", CultureInfo.InvariantCulture))
                  .Append(" local=(")
                  .Append(localPlayer.x.ToString("F3", CultureInfo.InvariantCulture)).Append(',')
                  .Append(localPlayer.y.ToString("F3", CultureInfo.InvariantCulture)).Append(',')
                  .Append(localPlayer.z.ToString("F3", CultureInfo.InvariantCulture)).Append(')')
                  .Append(" targets=").Append(validTargets.Count);

                if (validTargets.Count > 0)
                {
                    TargetCandidate selected = SelectDiagnosticTarget(validTargets);
                    sb.Append(" selected=").Append(BuildTargetSummary(localPlayer, selected));

                    foreach (TargetCandidate candidate in validTargets.Take(3))
                    {
                        sb.Append(" candidate=").Append(BuildTargetSummary(localPlayer, candidate));
                    }
                }
                else
                {
                    sb.Append(" selected=<none>");
                }

                AppendDiagnosticLine(sb.ToString());
            }
            catch
            {
                // Diagnostics must never break the main loop.
            }
        }

        private TargetCandidate SelectDiagnosticTarget(List<TargetCandidate> candidates)
        {
            if (!_targetLockEnabled)
            {
                return candidates.First();
            }

            if (_hasLock)
            {
                TargetCandidate? locked = FindLockedTarget(candidates);
                if (locked != null)
                {
                    return locked;
                }
            }

            return candidates.First();
        }

        private string BuildTargetSummary(Entity localPlayer, TargetCandidate candidate)
        {
            float dx = candidate.Enemy.x - localPlayer.x;
            float dy = candidate.Enemy.y - localPlayer.y;
            float dz = candidate.Enemy.z - localPlayer.z;

            return string.Format(
                CultureInfo.InvariantCulture,
                "{{name={0},team={1},hp={2},enemy=({3:F3},{4:F3},{5:F3}),delta=({6:F3},{7:F3},{8:F3}),targetYaw={9:F3},targetPitch={10:F3},score={11:F3}}}",
                candidate.Enemy.name?.Trim(),
                candidate.Enemy.team,
                candidate.Enemy.hp,
                candidate.Enemy.x,
                candidate.Enemy.y,
                candidate.Enemy.z,
                dx,
                dy,
                dz,
                candidate.Angles.x,
                candidate.Angles.y,
                candidate.Distance);
        }

        private void AppendDiagnosticLine(string line)
        {
            if (string.IsNullOrWhiteSpace(_diagnosticsPath))
            {
                return;
            }

            string? directory = Path.GetDirectoryName(_diagnosticsPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.AppendAllText(_diagnosticsPath, line + Environment.NewLine, Encoding.UTF8);
        }

        private static string ResolveDiagnosticsPath(string? configuredPath)
        {
            string path = string.IsNullOrWhiteSpace(configuredPath)
                ? "Logs/aimbot-diagnostics.log"
                : configuredPath;

            return Path.IsPathRooted(path)
                ? path
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }


        private bool IsInFrontByMatrix(ViewMatrix viewMatrix, Entity enemy)
        {
            if (enemy == null)
            {
                return false;
            }

            float wColumn = (viewMatrix.m14 * enemy.x) + (viewMatrix.m24 * enemy.y) + (viewMatrix.m34 * enemy.z) + viewMatrix.m44;
            float wRow = (viewMatrix.m41 * enemy.x) + (viewMatrix.m42 * enemy.y) + (viewMatrix.m43 * enemy.z) + viewMatrix.m44;
            return wColumn > 0.01f || wRow > 0.01f;
        }

        private PointF WorldToScreen(ViewMatrix viewMatrix, Entity target, Size gameScreen)
        {
            return _useRowMajorThisFrame
                ? ProjectRowMajor(viewMatrix, target, gameScreen)
                : ProjectColumnMajor(viewMatrix, target, gameScreen);
        }

        private static PointF ProjectColumnMajor(ViewMatrix viewMatrix, Entity target, Size gameScreen)
        {
            float w = (viewMatrix.m14 * target.x) + (viewMatrix.m24 * target.y) + (viewMatrix.m34 * target.z) + viewMatrix.m44;
            if (w < 0.01f) return new PointF(-1, -1);

            float screenX = (viewMatrix.m11 * target.x) + (viewMatrix.m21 * target.y) + (viewMatrix.m31 * target.z) + viewMatrix.m41;
            float screenY = (viewMatrix.m12 * target.x) + (viewMatrix.m22 * target.y) + (viewMatrix.m32 * target.z) + viewMatrix.m42;
            float camX = gameScreen.Width / 2f;
            float camY = gameScreen.Height / 2f;

            return new PointF(camX + screenX * camX / w, camY - screenY * camY / w);
        }

        private static PointF ProjectRowMajor(ViewMatrix viewMatrix, Entity target, Size gameScreen)
        {
            float w = (viewMatrix.m41 * target.x) + (viewMatrix.m42 * target.y) + (viewMatrix.m43 * target.z) + viewMatrix.m44;
            if (w < 0.01f) return new PointF(-1, -1);

            float screenX = (viewMatrix.m11 * target.x) + (viewMatrix.m12 * target.y) + (viewMatrix.m13 * target.z) + viewMatrix.m14;
            float screenY = (viewMatrix.m21 * target.x) + (viewMatrix.m22 * target.y) + (viewMatrix.m23 * target.z) + viewMatrix.m24;
            float camX = gameScreen.Width / 2f;
            float camY = gameScreen.Height / 2f;

            return new PointF(camX + screenX * camX / w, camY - screenY * camY / w);
        }

        private bool DetermineProjectionMode(GameState gameState, Entity localPlayer)
        {
            if (_matrixOrder == "row")
            {
                return true;
            }

            if (_matrixOrder == "column")
            {
                return false;
            }

            int rowHits = 0;
            int colHits = 0;

            foreach (Entity enemy in gameState.Enemies.Where(e => e.hp > 0))
            {
                if (!IsTargetSane(localPlayer, enemy))
                {
                    continue;
                }

                PointF col = ProjectColumnMajor(gameState.ViewMatrix, enemy, gameState.GameWindowSize);
                if (IsPointOnScreen(col, gameState.GameWindowSize))
                {
                    colHits++;
                }

                PointF row = ProjectRowMajor(gameState.ViewMatrix, enemy, gameState.GameWindowSize);
                if (IsPointOnScreen(row, gameState.GameWindowSize))
                {
                    rowHits++;
                }
            }

            if (rowHits == 0 && colHits == 0)
            {
                return _lastUseRowMajor;
            }

            bool useRow = rowHits > colHits;
            _lastUseRowMajor = useRow;
            return useRow;
        }

        private static bool IsPointOnScreen(PointF p, Size size)
        {
            return p.X >= 0 && p.X <= size.Width && p.Y >= 0 && p.Y <= size.Height;
        }

        private static bool IsFiniteCoord(Entity e)
        {
            return !(float.IsNaN(e.x) || float.IsNaN(e.y) || float.IsNaN(e.z) ||
                     float.IsInfinity(e.x) || float.IsInfinity(e.y) || float.IsInfinity(e.z));
        }

        public void Cleanup()
        {
            _hasLock = false;
            _lockedName = null;
            _lockedTeam = 0;
            _lockedPos = new Vector3();
            _lastUseRowMajor = false;
            _useRowMajorThisFrame = false;
        }

        private static string NormalizeAxisMode(string? axisMode)
        {
            return (axisMode ?? "zup").ToLowerInvariant() switch
            {
                "zup" => "zup",
                "yup" => "yup",
                "auto" => "auto",
                _ => "auto"
            };
        }

        private static string NormalizeAimMode(string? aimMode)
        {
            return (aimMode ?? "angle").ToLowerInvariant() switch
            {
                "screen" => "screen",
                _ => "angle"
            };
        }

        private static string NormalizeMatrixOrder(string? matrixOrder)
        {
            return (matrixOrder ?? "auto").ToLowerInvariant() switch
            {
                "row" => "row",
                "rowmajor" => "row",
                "column" => "column",
                "columnmajor" => "column",
                _ => "auto"
            };
        }

        private static string NormalizeAngleAlgorithm(string? algorithm)
        {
            return (algorithm ?? "adaptive").ToLowerInvariant() switch
            {
                "legacy" => "legacy",
                _ => "adaptive"
            };
        }

        private static string NormalizeWriteCulture(string? writeCulture)
        {
            return (writeCulture ?? "current").ToLowerInvariant() switch
            {
                "invariant" => "invariant",
                _ => "current"
            };
        }
    }
}
