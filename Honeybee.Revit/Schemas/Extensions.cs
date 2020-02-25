using System.Collections.Generic;
using System.Linq;

namespace Honeybee.Revit.Schemas
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="floorBoundary"></param>
        /// <returns></returns>
        public static List<List<double>> ToDragonfly(this List<Point2D> floorBoundary)
        {
            return floorBoundary.Select(x => new List<double> { x.X, x.Y }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="floorHoles"></param>
        /// <returns></returns>
        public static List<List<List<double>>> ToDragonfly(this List<List<Point2D>> floorHoles)
        {
            return floorHoles.Select(x => x.Select(y => new List<double> {y.X, y.Y}).ToList()).ToList();
        }
    }
}
