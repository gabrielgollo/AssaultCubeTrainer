using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeTrainer
{
    internal class Aimbot
    {
        public static void CalculateAim(Entity localPlayer, Entity targetPlayer, float tolerance, out float targetPitch, out float targetYaw, out bool changeVision)
        {
            // Vetor direção do jogador alvo em relação ao jogador local
            float deltaX = targetPlayer.x - localPlayer.x;
            float deltaY = targetPlayer.y - localPlayer.y;
            float deltaZ = targetPlayer.z - localPlayer.z;

            // Calcular a distância horizontal e a distância total
            float distanceHorizontal = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            float distanceTotal = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            // Calcular yaw (ângulo no plano XY)
            float desiredYaw = -(float)Math.Atan2(deltaY, deltaX) * (180.0f / (float)Math.PI);

            // Calcular pitch (ângulo vertical)
            float desiredPitch = (float)Math.Atan2(deltaZ, distanceHorizontal) * (180.0f / (float)Math.PI);

            // Ajustar yaw e pitch de acordo com os valores atuais
            float yawDifference = desiredYaw - localPlayer.viewX;
            float pitchDifference = desiredPitch - localPlayer.viewY;

            // Inicializar os alvos com os valores atuais
            targetYaw = desiredYaw;
            targetPitch = desiredPitch;
            changeVision = true;

            Console.WriteLine("Viewing" + targetPlayer.name + " at " + targetYaw + " " + targetPitch);

        }
    }
}
