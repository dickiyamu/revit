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
        public static List<Curve> GetCurves(this List<Point2D> pts)
        {
            var curves = new List<Curve>();
            for (var i = 0; i < pts.Count; i++)
            {
                var start = pts[i];
                var end = i == pts.Count - 1 ? pts[0] : pts[i + 1];
                curves.Add(Line.CreateBound(new XYZ(start.X, start.Y, 0), new XYZ(end.X, end.Y, 0)));
            }

            return curves;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <returns></returns>
        public static List<Curve> TessellateIntoCurves(this Arc arc)
        {
            var pt1 = arc.Evaluate(0, true);
            var pt2 = arc.Evaluate(0.25, true);
            var pt3 = arc.Evaluate(0.5, true);
            var pt4 = arc.Evaluate(0.75, true);
            var pt5 = arc.Evaluate(1, true);
            return new List<Curve>
            {
                Line.CreateBound(pt1, pt2),
                Line.CreateBound(pt2, pt3),
                Line.CreateBound(pt3, pt4),
                Line.CreateBound(pt4, pt5)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="floorBoundary"></param>
        /// <returns></returns>
        public static List<List<double>> ToDragonfly(this List<Point2D> floorBoundary)
        {
            return floorBoundary.Select(x => new List<double> {x.X, x.Y}).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bcs"></param>
        /// <returns></returns>
        public static List<DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface>> ToDragonfly(
            this List<BoundaryConditionBase> bcs)
        {
            var boundaryConditions = new List<DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface>>();
            foreach (var bc in bcs)
            {
                switch (bc)
                {
                    case Outdoors unused:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Outdoors);
                        break;
                    case Ground unused:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Ground);
                        break;
                    case Adiabatic unused:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Adiabatic);
                        break;
                    case Surface unused:
                        boundaryConditions.Add(bc.ToDragonfly() as DF.Surface);
                        break;
                    default:
                        boundaryConditions.Add(null);
                        break;
                }
            }

            return boundaryConditions;
        }

        public static List<DF.AnyOf<DF.OpaqueConstructionAbridged, DF.WindowConstructionAbridged, DF.ShadeConstruction>>
            ToDragonfly(this List<ConstructionBase> cons)
        {
            var constructions =
                new List<DF.AnyOf<DF.OpaqueConstructionAbridged, DF.WindowConstructionAbridged, DF.ShadeConstruction
                >>();
            foreach (var cb in cons)
            {
                switch (cb)
                {
                    case OpaqueConstructionAbridged unused:
                        constructions.Add(cb.ToDragonfly() as DF.OpaqueConstructionAbridged);
                        break;
                    case WindowConstructionAbridged unused:
                        constructions.Add(cb.ToDragonfly() as DF.WindowConstructionAbridged);
                        break;
                    case ShadeConstruction unused:
                        constructions.Add(cb.ToDragonfly() as DF.ShadeConstruction);
                        break;
                    default:
                        constructions.Add(null);
                        break;
                }
            }

            return constructions;
        }

        public static List<DF.AnyOf<DF.EnergyMaterial, DF.EnergyMaterialNoMass, DF.EnergyWindowMaterialGas,
            DF.EnergyWindowMaterialGasCustom, DF.EnergyWindowMaterialGasMixture, DF.EnergyWindowMaterialSimpleGlazSys,
            DF.EnergyWindowMaterialBlind, DF.EnergyWindowMaterialGlazing, DF.EnergyWindowMaterialShade>> ToDragonfly(
            this List<MaterialBase> mats)
        {
            var materials = new List<DF.AnyOf<DF.EnergyMaterial, DF.EnergyMaterialNoMass, DF.EnergyWindowMaterialGas,
                DF.EnergyWindowMaterialGasCustom, DF.EnergyWindowMaterialGasMixture, DF.EnergyWindowMaterialSimpleGlazSys,
                DF.EnergyWindowMaterialBlind, DF.EnergyWindowMaterialGlazing, DF.EnergyWindowMaterialShade>>();
            foreach (var m in mats)
            {
                switch (m)
                {
                    case EnergyMaterial unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyMaterial);
                        break;
                    case EnergyMaterialNoMass unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyMaterialNoMass);
                        break;
                    case EnergyWindowMaterialBlind unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyWindowMaterialBlind);
                        break;
                    case EnergyWindowMaterialGas unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyWindowMaterialGas);
                        break;
                    case EnergyWindowMaterialGasCustom unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyWindowMaterialGasCustom);
                        break;
                    case EnergyWindowMaterialGasMixture unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyWindowMaterialGasMixture);
                        break;
                    case EnergyWindowMaterialGlazing unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyWindowMaterialGlazing);
                        break;
                    case EnergyWindowMaterialShade unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyWindowMaterialShade);
                        break;
                    case EnergyWindowMaterialSimpleGlazSys unused:
                        materials.Add(m.ToDragonfly() as DF.EnergyWindowMaterialSimpleGlazSys);
                        break;
                    default:
                        materials.Add(null);
                        break;
                }
            }

            return materials;
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