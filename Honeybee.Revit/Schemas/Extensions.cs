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

        public static List<DF.AnyOf<DF.ConstructionSetAbridged, DF.ConstructionSet>> ToDragonfly(this List<ConstructionSetBase> constructionSets)
        {
            var sets = new List<DF.AnyOf<DF.ConstructionSetAbridged, DF.ConstructionSet>>();
            foreach (var set in constructionSets)
            {
                switch (set)
                {
                    case ConstructionSetAbridged unused:
                        sets.Add(set.ToDragonfly() as DF.ConstructionSetAbridged);
                        break;
                    default:
                        sets.Add(null);
                        break;
                }
            }

            return sets;
        }

        public static List<DF.AnyOf<DF.OpaqueConstructionAbridged, DF.WindowConstructionAbridged, DF.ShadeConstruction, DF.AirBoundaryConstructionAbridged, DF.OpaqueConstruction, DF.WindowConstruction, DF.AirBoundaryConstruction>> ToDragonfly(this List<ConstructionBase> cons)
        {
            var constructions =
                new List<DF.AnyOf<DF.OpaqueConstructionAbridged, DF.WindowConstructionAbridged, DF.ShadeConstruction,
                    DF.AirBoundaryConstructionAbridged, DF.OpaqueConstruction, DF.WindowConstruction,
                    DF.AirBoundaryConstruction>>();
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

        public static List<List<List<double>>> ToDragonfly(this List<List<Point2D>> floorHoles)
        {
            return floorHoles.Select(x => x.Select(y => new List<double> {y.X, y.Y}).ToList()).ToList();
        }

        public static List<string> ToDragonfly(this Tuple<int, string> boundaryConditionObj)
        {
            var (adjacentCurveIndex, adjacentRoomName) = boundaryConditionObj;

            return new List<string>
            {
                $"{adjacentRoomName}..Face{adjacentCurveIndex + 1}",
                adjacentRoomName
            };
        }

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