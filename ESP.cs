using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeTrainer
{
    class ESP
    {
        public static void ShowESP(ViewMatrix viewMatrix, List<Entity> enemies, Entity player, Size gameProcessWinSize, Process gameProcess) {

            foreach (Entity enemy in enemies)
                {    
                // Converter coordenadas de mundo para coordenadas de tela
                PointF screenPos = ESP.WorldToScreen(viewMatrix, enemy, gameProcessWinSize);

                
                // Verificar se a posição está dentro da tela
                if (screenPos.X >= 0 && screenPos.X <= gameProcessWinSize.Width &&
                    screenPos.Y >= 0 && screenPos.Y <= gameProcessWinSize.Height &&
                    enemy.hp > 0)
                {
                    try
                        {
                        // Distância do jogador ao inimigo
                        float distance = (float)Math.Sqrt(
                            Math.Pow(player.x - enemy.x, 2) +
                            Math.Pow(player.y - enemy.y, 2) +
                            Math.Pow(player.z - enemy.z, 2)
                        );

                        // Altura aproximada de um jogador no Assault Cube
                        float playerHeight = 8f;

                        // Calcula o tamanho em pixels com base na projeção da matriz de visão
                        float screenHeight = gameProcessWinSize.Height;
                        float fov = player.fov;
                        float scale = (screenHeight / (2.0f * (float)Math.Tan(fov * Math.PI / 360.0))) / distance;

                        // Tamanho do quadrado baseado na altura do jogador
                        float squareHeight = playerHeight * scale;
                        float squareWidth = squareHeight / 2; // Ajuste para a largura proporcional

                        // Criar o retângulo para desenhar ao redor do inimigo
                        Rectangle rect = new Rectangle(
                            (int)(screenPos.X - squareWidth / 2),
                            (int)(screenPos.Y - playerHeight*2.2), // Ajuste para ficar centralizado
                            (int)squareWidth,
                            (int)squareHeight
                        );

                        // Escolher a cor do quadrado baseado na vida do inimigo
                        Color espColor = enemy.hp > 70 ? Color.Green :
                                        enemy.hp > 30 ? Color.Orange :
                                        Color.Red;

                        // Desenhar o quadrado
                        Drawing.DrawRect(gameProcess.MainWindowHandle, espColor, rect);

                    }
                        catch
                        {
                            Console.WriteLine("Missed the win handler");
                            throw new Exception("Missed the win handler");
                        }
                    }
            }
        }
        public static PointF WorldToScreen(ViewMatrix viewMatrix, Entity target, Size gameScreen) { 
            
            float w = (viewMatrix.m14 * target.x) + (viewMatrix.m24 * target.y) + (viewMatrix.m34 * target.z) + viewMatrix.m44;
            if (w < 0.01f) return new PointF(-1, -1);

            float screenX = (viewMatrix.m11 * target.x) + (viewMatrix.m21 * target.y) + (viewMatrix.m31 * target.z) + viewMatrix.m41;
            float screenY = (viewMatrix.m12 * target.x) + (viewMatrix.m22 * target.y) + (viewMatrix.m32 * target.z) + viewMatrix.m42;

            float camX = gameScreen.Width / 2f;
            float camY = gameScreen.Height / 2f;

            float x = camX + screenX * camX / w;
            float y = camY - screenY * camY / w;

            return new PointF(x, y);
        }

        public static ViewMatrix GetViewMatrix(Mem m)
        {
            var matrix = new ViewMatrix();

            byte[] buffer = new byte[16 * 4];
            var bytes = m.ReadBytes(Offsets.ViewMatrix, (long) buffer.Length);

            matrix.m11 = BitConverter.ToSingle(bytes, 0);
            matrix.m12 = BitConverter.ToSingle(bytes, 4);
            matrix.m13 = BitConverter.ToSingle(bytes, 8);
            matrix.m14 = BitConverter.ToSingle(bytes, 12);

            matrix.m21 = BitConverter.ToSingle(bytes, 16);
            matrix.m22 = BitConverter.ToSingle(bytes, 20);
            matrix.m23 = BitConverter.ToSingle(bytes, 24);
            matrix.m24 = BitConverter.ToSingle(bytes, 28);

            matrix.m31 = BitConverter.ToSingle(bytes, 32);
            matrix.m32 = BitConverter.ToSingle(bytes, 36);
            matrix.m33 = BitConverter.ToSingle(bytes, 40);
            matrix.m34 = BitConverter.ToSingle(bytes, 44);

            matrix.m41 = BitConverter.ToSingle(bytes, 48);
            matrix.m42 = BitConverter.ToSingle(bytes, 52);
            matrix.m43 = BitConverter.ToSingle(bytes, 56);
            matrix.m44 = BitConverter.ToSingle(bytes, 60);

            return matrix;  
        }
    }
}
