using System;
using System.Drawing;
using AssaultCubeTrainer.Core;
using AssaultCubeTrainer.Game;
using AssaultCubeTrainer.Rendering;

namespace AssaultCubeTrainer.Features
{
    /// <summary>
    /// ESP (Extra Sensory Perception) feature - draws boxes around enemies
    /// </summary>
    public class ESPFeature : ICheatFeature
    {
        public string Name => "ESP";
        public int Priority => 5; // Run before aimbot
        public bool IsEnabled { get; set; }

        // Settings
        private bool _drawBoxes;
        private float _playerHeight;
        private float _headHeight;
        private bool _debugDraw;
        private float _maxEspDistance;
        private float _boxScale = 1.0f;
        private float _yOffsetPx;
        private bool _useRowMajorThisFrame;
        private string _matrixOrder = "auto";
        private bool _lastUseRowMajor;
        private string _axisMode = "zup";

        public void Initialize(IGameProfile profile, MemoryManager _)
        {
            _drawBoxes = profile.GetSetting<bool>("features.esp.boxes", true);
            _playerHeight = profile.GetSetting<float>("gameSettings.playerHeight", 8.0f);
            _headHeight = profile.GetSetting<float>("gameSettings.headHeight", 2.2f);
            _debugDraw = profile.GetSetting<bool>("features.esp.debugDraw", false);
            _maxEspDistance = profile.GetSetting<float>("features.esp.maxDistance", 220.0f);
            _boxScale = profile.GetSetting<float>("features.esp.boxScale", 1.0f);
            _yOffsetPx = profile.GetSetting<float>("features.esp.yOffsetPx", 0.0f);
            _matrixOrder = (profile.GetSetting<string>("features.esp.matrixOrder", "auto") ?? "auto").ToLowerInvariant();
            _axisMode = (profile.GetSetting<string>("features.esp.axisMode",
                profile.GetSetting<string>("features.aimbot.axisMode", "zup")) ?? "zup").ToLowerInvariant();
        }

        public void Update(GameState gameState)
        {
            if (gameState.GameWindowHandle == IntPtr.Zero || gameState.GameWindowSize.Width <= 0 || gameState.GameWindowSize.Height <= 0)
            {
                return;
            }

            Drawing.BeginOverlayFrame(gameState.GameWindowHandle);
            _useRowMajorThisFrame = DetermineProjectionMode(gameState);

            if (_debugDraw)
            {
                int cx = gameState.GameWindowSize.Width / 2;
                int cy = gameState.GameWindowSize.Height / 2;
                Drawing.DrawRect(gameState.GameWindowHandle, Color.Magenta, new Rectangle(cx - 25, cy - 25, 50, 50));
            }

            if (!gameState.IsValid)
            {
                Drawing.EndOverlayFrame();
                return;
            }

            if (gameState.Enemies.Count == 0)
            {
                Drawing.EndOverlayFrame();
                return;
            }

            if (!_drawBoxes)
            {
                Drawing.EndOverlayFrame();
                return;
            }

            foreach (var enemy in gameState.Enemies)
            {
                if (!IsInFrontByMatrix(gameState.ViewMatrix, enemy))
                {
                    continue;
                }

                var screenPos = WorldToScreen(gameState.ViewMatrix, enemy, gameState.GameWindowSize, _useRowMajorThisFrame);

                if (screenPos.X >= 0 && screenPos.X <= gameState.GameWindowSize.Width &&
                    screenPos.Y >= 0 && screenPos.Y <= gameState.GameWindowSize.Height &&
                    enemy.hp > 0 && IsEnemySane(gameState.LocalPlayer, enemy))
                {
                    DrawESP(enemy, gameState);
                }
            }

            Drawing.EndOverlayFrame();
        }

        /// <summary>
        /// Draw ESP box for enemy
        /// </summary>
        private void DrawESP(Entity enemy, GameState gameState)
        {
            if (gameState.GameWindowHandle == IntPtr.Zero)
            {
                return;
            }

            try
            {
                float distance = CalculateDistance(gameState.LocalPlayer, enemy);
                if (distance <= 0.01f)
                {
                    return;
                }
                if (!TryBuildEspRect(enemy, gameState, out Rectangle rect))
                {
                    return;
                }

                Color espColor = enemy.hp > 70 ? Color.Green :
                                enemy.hp > 30 ? Color.Orange :
                                Color.Red;

                Drawing.DrawRect(gameState.GameWindowHandle, espColor, rect);
            }
            catch
            {
                // Ignore transient window/drawing failures.
            }
        }

        private bool TryBuildEspRect(Entity enemy, GameState gameState, out Rectangle rect)
        {
            rect = Rectangle.Empty;
            if (!TryProjectBodySegment(enemy, gameState, out PointF top, out PointF bottom))
            {
                return false;
            }

            float boxHeight = Math.Abs(bottom.Y - top.Y);
            if (boxHeight < 6f || float.IsNaN(boxHeight) || float.IsInfinity(boxHeight))
            {
                return false;
            }

            float centerY = (top.Y + bottom.Y) * 0.5f;
            float scale = Math.Max(0.35f, Math.Min(1.8f, _boxScale));
            boxHeight *= scale;

            float boxWidth = boxHeight / 2f;
            float centerX = (top.X + bottom.X) * 0.5f;
            float topY = (centerY - (boxHeight * 0.5f)) + _yOffsetPx;

            boxHeight = Math.Max(6f, Math.Min(boxHeight, 900f));
            boxWidth = Math.Max(3f, Math.Min(boxWidth, 500f));

            rect = new Rectangle(
                (int)Math.Round(centerX - (boxWidth / 2f)),
                (int)Math.Round(topY),
                (int)Math.Round(boxWidth),
                (int)Math.Round(boxHeight));

            return rect.Width > 0 && rect.Height > 0;
        }

        private bool TryProjectBodySegment(Entity enemy, GameState gameState, out PointF top, out PointF bottom)
        {
            top = new PointF(-1, -1);
            bottom = new PointF(-1, -1);

            float topOffset = Math.Max(0.2f, _headHeight);
            float bottomOffset = Math.Max(0.2f, _playerHeight - _headHeight);

            Entity topPoint = new Entity { x = enemy.x, y = enemy.y, z = enemy.z };
            Entity bottomPoint = new Entity { x = enemy.x, y = enemy.y, z = enemy.z };

            if (_axisMode == "yup")
            {
                topPoint.y += topOffset;
                bottomPoint.y -= bottomOffset;
            }
            else
            {
                topPoint.z += topOffset;
                bottomPoint.z -= bottomOffset;
            }

            top = WorldToScreen(gameState.ViewMatrix, topPoint, gameState.GameWindowSize, _useRowMajorThisFrame);
            bottom = WorldToScreen(gameState.ViewMatrix, bottomPoint, gameState.GameWindowSize, _useRowMajorThisFrame);

            return top.X >= 0 && top.Y >= 0 && bottom.X >= 0 && bottom.Y >= 0;
        }

        /// <summary>
        /// Calculate distance between two entities
        /// </summary>
        private float CalculateDistance(Entity from, Entity to)
        {
            return (float)Math.Sqrt(
                Math.Pow(from.x - to.x, 2) +
                Math.Pow(from.y - to.y, 2) +
                Math.Pow(from.z - to.z, 2)
            );
        }

        private bool IsEnemySane(Entity local, Entity enemy)
        {
            if (local == null || enemy == null)
            {
                return false;
            }

            if (!IsFiniteCoord(local) || !IsFiniteCoord(enemy))
            {
                return false;
            }

            float distance = CalculateDistance(local, enemy);
            if (float.IsNaN(distance) || float.IsInfinity(distance))
            {
                return false;
            }

            return distance > 0.5f && distance <= _maxEspDistance;
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

        /// <summary>
        /// Convert 3D world coordinates to 2D screen coordinates
        /// </summary>
        private PointF WorldToScreen(ViewMatrix viewMatrix, Entity target, Size gameScreen, bool useRowMajor)
        {
            return useRowMajor
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

        private bool DetermineProjectionMode(GameState gameState)
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

            foreach (var enemy in gameState.Enemies)
            {
                if (!IsEnemySane(gameState.LocalPlayer, enemy))
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
            Drawing.ShutdownOverlay();
        }
    }
}
