using System;

namespace Honeybee.Revit.Utilities
{
    public static class DoubleExtensions
    {
        public static bool AlmostEqualTo(this double value1, double value2)
        {
            var tolerance = AppSettings.Instance.StoredSettings.GeometrySettings.Tolerance;
            return Math.Abs(value1 - value2) < tolerance;
        }
    }
}
