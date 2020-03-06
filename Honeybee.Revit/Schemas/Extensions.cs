using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        public static XYZ GetLocationPoint(this SpatialElement se)
        {
            var result = XYZ.Zero;
            if (se.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode())
            {
                if (!(se is Room room)) return result;
                if (!(room.Location is LocationPoint loc)) return result;

                result = loc.Point;
            }
            else
            {
                if (!(se is Space space)) return result;
                if (!(space.Location is LocationPoint loc)) return result;

                result = loc.Point;
            }

            return result;
        }
    }
}
