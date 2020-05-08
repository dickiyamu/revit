﻿using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public static class Extensions
    {
        public static List<Curve> GetCurves(this List<Point2D> pts, double z)
        {
            var curves = new List<Curve>();
            for (var i = 0; i < pts.Count; i++)
            {
                var start = pts[i];
                var end = i == pts.Count - 1 ? pts[0] : pts[i + 1];
                curves.Add(Line.CreateBound(new XYZ(start.X, start.Y, z), new XYZ(end.X, end.Y, z)));
            }

            return curves;
        }

        public static List<List<double>> ToDragonfly(this List<Point2D> floorBoundary)
        {
            return floorBoundary.Select(x => new List<double> {x.X, x.Y}).ToList();
        }

        public static List<DF.AnyOf<HB.Ground, HB.Outdoors, HB.Adiabatic, HB.Surface>> ToDragonfly(
            this List<BoundaryConditionBase> bcs)
        {
            var boundaryConditions = new List<DF.AnyOf<HB.Ground, HB.Outdoors, HB.Adiabatic, HB.Surface>>();
            foreach (var bc in bcs)
            {
                switch (bc)
                {
                    case Outdoors unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Outdoors);
                        break;
                    case Ground unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Ground);
                        break;
                    case Adiabatic unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Adiabatic);
                        break;
                    case Surface unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Surface);
                        break;
                    default:
                        boundaryConditions.Add(null);
                        break;
                }
            }

            return boundaryConditions;
        }

        public static List<DF.AnyOf<DF.SingleWindow, DF.SimpleWindowRatio, DF.RepeatingWindowRatio, DF.RectangularWindows, DF.DetailedWindows>> ToDragonfly(
            this List<WindowParameterBase> bcs)
        {
            var windowParameters = new List<DF.AnyOf<DF.SingleWindow, DF.SimpleWindowRatio, DF.RepeatingWindowRatio, DF.RectangularWindows, DF.DetailedWindows>>();
            foreach (var bc in bcs)
            {
                switch (bc)
                {
                    case SingleWindow unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.SingleWindow);
                        break;
                    case SimpleWindowRatio unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.SimpleWindowRatio);
                        break;
                    case RepeatingWindowRatio unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.RepeatingWindowRatio);
                        break;
                    case RectangularWindows unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.RectangularWindows);
                        break;
                    case DetailedWindows unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.DetailedWindows);
                        break;
                    default:
                        windowParameters.Add(null);
                        break;
                }
            }

            return windowParameters;
        }

        //public static List<DF.AnyOf<HB.ConstructionSetAbridged, HB.ConstructionSet>> ToDragonfly(this List<ConstructionSetBase> constructionSets)
        //{
        //    var sets = new List<DF.AnyOf<HB.ConstructionSetAbridged, HB.ConstructionSet>>();
        //    foreach (var set in constructionSets)
        //    {
        //        switch (set)
        //        {
        //            case ConstructionSetAbridged unused:
        //                sets.Add(set.ToDragonfly() as HB.ConstructionSetAbridged);
        //                break;
        //            default:
        //                sets.Add(null);
        //                break;
        //        }
        //    }

        //    return sets;
        //}

        public static List<List<List<double>>> ToDragonfly(this List<List<Point2D>> floorHoles)
        {
            return floorHoles.Select(x => x.Select(y => new List<double> {y.X, y.Y}).ToList()).ToList();
        }

        //public static List<string> ToDragonfly(this Tuple<int, string> boundaryConditionObj)
        //{
        //    var (adjacentCurveIndex, adjacentRoomName) = boundaryConditionObj;

        //    return new List<string>
        //    {
        //        $"{adjacentRoomName}..Face{adjacentCurveIndex + 1}",
        //        adjacentRoomName
        //    };
        //}

        //public static XYZ GetLocationPoint(this SpatialElement se)
        //{
        //    XYZ result;
        //    if (se.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode())
        //    {
        //        if (!(se is Room room)) return null;
        //        if (!(room.Location is LocationPoint loc)) return null;

        //        result = loc.Point;
        //    }
        //    else
        //    {
        //        if (!(se is Space space)) return null;
        //        if (!(space.Location is LocationPoint loc)) return null;

        //        result = loc.Point;
        //    }

        //    return result;
        //}

        //public static double GetUnboundHeight(this SpatialElement se)
        //{
        //    var result = 0d;
        //    if (se.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode())
        //    {
        //        if (!(se is Room room)) return result;

        //        result = room.UnboundedHeight;
        //    }
        //    else
        //    {
        //        if (!(se is Space space)) return result;

        //        result = space.UnboundedHeight;
        //    }

        //    return result;
        //}
    }
}