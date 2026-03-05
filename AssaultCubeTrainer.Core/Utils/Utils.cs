using System;

namespace AssaultCubeTrainer.Utils
{
    /// <summary>
    /// Utility functions for hex math and conversions
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Sum two hex strings and return result as hex string
        /// </summary>
        public static string SumOfHexStrings(string hex1, string hex2)
        {
            int num1 = Convert.ToInt32(hex1, 16);
            int num2 = Convert.ToInt32(hex2, 16);
            return (num1 + num2).ToString("X");
        }
    }
}
