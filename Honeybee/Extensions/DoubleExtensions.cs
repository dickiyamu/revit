using System;

namespace Honeybee.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static bool AlmostEqualTo(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.01;
        }
    }
}
