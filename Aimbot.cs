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
        public static void CalculateAim(Entity player, Entity enemy, out float targetPitch, out float targetYaw)
        {
            float dx = enemy.x - player.x;
            float dy = enemy.y - player.y;
            float dz = enemy.z - player.z;

            float distanceHorizontal = (float)Math.Sqrt(dx * dx + dy * dy);

            float yaw = (float)Math.Atan2(dy, dx); // Em radianos
            float yawDegrees = (float) (yaw * (180.0 / Math.PI)); // Converter para graus e normalizar

            targetYaw = yawDegrees+90;

            float pitch = (float)Math.Atan2(dz, distanceHorizontal); // Em radianos
            float pitchDegrees = (float) ( pitch * (180/Math.PI) ); // Converter para graus

            // Limitar o pitch entre -90 e 90 graus
            pitchDegrees = Math.Max(-90.0f, Math.Min(90.0f, pitchDegrees));
            targetPitch = pitchDegrees;
        }


    }
}
