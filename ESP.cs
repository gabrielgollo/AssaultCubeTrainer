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
    public class ViewMatrix
    {
        public float m11, m12, m13, m14;
        public float m21, m22, m23, m24;
        public float m31, m32, m33, m34;
        public float m41, m42, m43, m44;
    }

    public class Vector3 { public float x, y, z; }
    class ESP
    {
        public static void ShowESP(ViewMatrix viewMatrix, List<Entity> playerList, Size gameProcessWinSize, Process gameProcess) {

            foreach (Entity player in playerList)
                {    
                    // Converter coordenadas de mundo para coordenadas de tela
                    PointF screenPos = ESP.WorldToScreen(viewMatrix, player, gameProcessWinSize);

                    // Verificar se a posição está dentro da tela
                    if (screenPos.X >= 0 && screenPos.X <= gameProcessWinSize.Width && screenPos.Y >= 0 && screenPos.Y <= gameProcessWinSize.Height && player.hp > 0)
                    {
                        try
                        {
                            // Desenhar quadrado ao redor do personagem
                            Rectangle rect = new Rectangle((int)screenPos.X, (int)screenPos.Y, 20, 20); // Tamanho do quadrado 20x20
                            if (player.hp > 70)
                            {
                                Drawing.DrawRect(gameProcess.MainWindowHandle, Color.Green, rect);
                            }
                            else if (player.hp > 30)
                            {
                                Drawing.DrawRect(gameProcess.MainWindowHandle, Color.Orange, rect);
                            }
                            else
                            {
                                Drawing.DrawRect(gameProcess.MainWindowHandle, Color.Red, rect);
                            }
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
