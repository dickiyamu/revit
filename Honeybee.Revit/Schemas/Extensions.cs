using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using DF = DragonflySchema;

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
        /// <param name="bcs"></param>
        /// <returns></returns>
        public static List<DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface>> ToDragonfly(this List<BoundaryCondition> bcs)
        {
            var boundaryConditions = new List<DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface>>();
            foreach (var bc in bcs)
            {
                switch (bc)
                {
                    case Outdoors o:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Outdoors);
                        break;
                    case Ground g:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Ground);
                        break;
                    case Adiabatic a:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Adiabatic);
                        break;
                    case Surface s:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Surface);
                        break;
                }
            }

            return boundaryConditions;
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
        /// <param name="boundaryConditionObj"></param>
        /// <returns></returns>
        public static List<string> ToDragonfly(this Tuple<int, string> boundaryConditionObj)
        {
            var (adjacentCurveIndex, adjacentRoomName) = boundaryConditionObj;

            return new List<string>
            {
                $"{adjacentRoomName}..Face{adjacentCurveIndex + 1}",
                adjacentRoomName
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        public static XYZ GetLocationPoint(this SpatialElement se)
        {
            XYZ result;
            if (se.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode())
            {
                if (!(se is Room room)) return null;
                if (!(room.Location is LocationPoint loc)) return null;

                result = loc.Point;
            }
            else
            {
                if (!(se is Space space)) return null;
                if (!(space.Location is LocationPoint loc)) return null;

                result = loc.Point;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="se"></param>
        /// <returns></returns>
        public static double GetUnboundHeight(this SpatialElement se)
        {
            var result = 0d;
            if (se.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode())
            {
                if (!(se is Room room)) return result;

                result = room.UnboundedHeight;
            }
            else
            {
                if (!(se is Space space)) return result;

                result = space.UnboundedHeight;
            }

            return result;
        }
    }
}
