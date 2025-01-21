using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeTrainer
{
    internal class Aimbot
    {
        public static void CalculateAim(Entity localPlayer, Entity targetPlayer, out float targetPitch, out float targetYaw, out bool changeVision)
        {
            // Calcular as diferenças entre as posições do alvo e do jogador local
            float dx = targetPlayer.x - localPlayer.x;
            float dy = targetPlayer.y - localPlayer.y;
            float dz = targetPlayer.z + 0.3f - localPlayer.z;

            // Calcular a distância horizontal entre o jogador e o alvo
            float distanceHorizontal = (float)Math.Sqrt(dx * dx + dy * dy);

            // Calcular o yaw (horizontal)
            float yaw = (float)Math.Atan2(dy, dx); // Em radianos
            float yawDegrees =  yaw * 180.0f / (float) Math.PI; // Converter para graus e normalizar

            // Calcular o pitch (vertical)
            float pitch = (float)Math.Atan2(dz, distanceHorizontal); // Em radianos
            float pitchDegrees = pitch * 180.0f / (float)Math.PI; // Converter para graus

            // Limitar o pitch entre -90 e 90 graus
            pitchDegrees = Math.Max(-90.0f, Math.Min(90.0f, pitchDegrees));

            // Atribuir os valores calculados para os parâmetros de saída
            targetYaw = yawDegrees;
            targetPitch = pitchDegrees;

            // Verificar se a visão precisa ser alterada (sempre ajusta a visão neste ponto)
            changeVision = true;

            // Depuração - Mostrar resultados
            Console.WriteLine($"Targeting {targetPlayer.name} at Pitch: {targetPitch:F6}, Yaw: {targetYaw:F6}, ChangeVision: {changeVision}");
        }


    }
}
