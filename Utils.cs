using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssaultCubeTrainer
{
    public class Utils
    {
        public static string SumOfHexStrings(string hex1, string hex2)
        {
            int num1 = Convert.ToInt32(hex1, 16);
            int num2 = Convert.ToInt32(hex2, 16);
            return (num1 + num2).ToString("X");
        }
    }

    public class ViewMatrix
    {
        public float m11, m12, m13, m14;
        public float m21, m22, m23, m24;
        public float m31, m32, m33, m34;
        public float m41, m42, m43, m44;
    }

    public class Vector3 { public float x, y, z; }
}
