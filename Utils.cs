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
}
