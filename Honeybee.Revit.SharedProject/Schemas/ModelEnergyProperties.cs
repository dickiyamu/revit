using System.Collections.Generic;
using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ModelEnergyProperties : ISchema<DF.ModelEnergyProperties, HB.ModelEnergyProperties>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public List<ConstructionBase> Constructions { get; set; } = new List<ConstructionBase>();
        public List<MaterialBase> Materials { get; set; } = new List<MaterialBase>();
        public List<HB.AnyOf<HB.ConstructionSetAbridged, HB.ConstructionSet>> ConstructionSets { get; set; } = new List<HB.AnyOf<HB.ConstructionSetAbridged, HB.ConstructionSet>>();
        public List<HB.AnyOf<HB.ProgramTypeAbridged, HB.ProgramType>> ProgramTypes { get; set; } = new List<HB.AnyOf<HB.ProgramTypeAbridged, HB.ProgramType>>();
        public List<HB.IdealAirSystemAbridged> Hvacs { get; set; } = null;
        public List<HB.AnyOf<HB.ScheduleRulesetAbridged, HB.ScheduleFixedIntervalAbridged>> Schedules { get; set; } = null;
        public List<HB.ScheduleTypeLimit> ScheduleTypeLimits { get; set; } = null;

        public DF.ModelEnergyProperties ToDragonfly()
        {
            return new DF.ModelEnergyProperties(
                ConstructionSets,
                Constructions.ToDragonfly(),
                Materials.ToHoneybee(),
                null, // hvacs
                ProgramTypes,
                null, // schedules
                null // schedule type limits
            );
        }

        public HB.ModelEnergyProperties ToHoneybee()
        {
            return new HB.ModelEnergyProperties(
                ConstructionSets,
                Constructions.ToHoneybee(),
                Materials.ToHoneybee(),
                null, // hvacs
                ProgramTypes,
                null, // schedules
                null // schedule type limits
            );
        }
    }

    public static class ModelEnergyPropertiesExtensions
    {
        public static List<HB.AnyOf<
            HB.OpaqueConstructionAbridged, 
            HB.WindowConstructionAbridged, 
            HB.WindowConstructionShadeAbridged, 
            HB.AirBoundaryConstructionAbridged, 
            HB.OpaqueConstruction, 
            HB.WindowConstruction, 
            HB.WindowConstructionShade, 
            HB.AirBoundaryConstruction, 
            HB.ShadeConstruction>> ToHoneybee(this List<ConstructionBase> cons)
        {
            var constructions =
                new List<HB.AnyOf<
                    HB.OpaqueConstructionAbridged,
                    HB.WindowConstructionAbridged,
                    HB.WindowConstructionShadeAbridged,
                    HB.AirBoundaryConstructionAbridged,
                    HB.OpaqueConstruction,
                    HB.WindowConstruction,
                    HB.WindowConstructionShade,
                    HB.AirBoundaryConstruction,
                    HB.ShadeConstruction>>();
            foreach (var cb in cons)
            {
                switch (cb)
                {
                    case OpaqueConstructionAbridged unused:
                        constructions.Add(cb.ToDragonfly() as HB.OpaqueConstructionAbridged);
                        break;
                    case WindowConstructionAbridged unused:
                        constructions.Add(cb.ToDragonfly() as HB.WindowConstructionAbridged);
                        break;
                    case ShadeConstruction unused:
                        constructions.Add(cb.ToDragonfly() as HB.ShadeConstruction);
                        break;
                    default:
                        constructions.Add(null);
                        break;
                }
            }

            return constructions;
        }

        public static List<HB.AnyOf<
            HB.OpaqueConstructionAbridged, 
            HB.WindowConstructionAbridged, 
            HB.ShadeConstruction, 
            HB.AirBoundaryConstructionAbridged, 
            HB.OpaqueConstruction, 
            HB.WindowConstruction, 
            HB.AirBoundaryConstruction>> ToDragonfly(this List<ConstructionBase> cons)
        {
            var constructions =
                new List<HB.AnyOf<
                    HB.OpaqueConstructionAbridged,
                    HB.WindowConstructionAbridged,
                    HB.ShadeConstruction,
                    HB.AirBoundaryConstructionAbridged,
                    HB.OpaqueConstruction,
                    HB.WindowConstruction,
                    HB.AirBoundaryConstruction>>();
            foreach (var cb in cons)
            {
                switch (cb)
                {
                    case OpaqueConstructionAbridged unused:
                        constructions.Add(cb.ToDragonfly() as HB.OpaqueConstructionAbridged);
                        break;
                    case WindowConstructionAbridged unused:
                        constructions.Add(cb.ToDragonfly() as HB.WindowConstructionAbridged);
                        break;
                    case ShadeConstruction unused:
                        constructions.Add(cb.ToDragonfly() as HB.ShadeConstruction);
                        break;
                    default:
                        constructions.Add(null);
                        break;
                }
            }

            return constructions;
        }

        public static List<HB.AnyOf<HB.EnergyMaterial, HB.EnergyMaterialNoMass, HB.EnergyWindowMaterialGas,
            HB.EnergyWindowMaterialGasCustom, HB.EnergyWindowMaterialGasMixture, HB.EnergyWindowMaterialSimpleGlazSys,
            HB.EnergyWindowMaterialBlind, HB.EnergyWindowMaterialGlazing, HB.EnergyWindowMaterialShade>> ToHoneybee(
            this List<MaterialBase> mats)
        {
            var materials = new List<HB.AnyOf<HB.EnergyMaterial, HB.EnergyMaterialNoMass, HB.EnergyWindowMaterialGas,
                HB.EnergyWindowMaterialGasCustom, HB.EnergyWindowMaterialGasMixture, HB.EnergyWindowMaterialSimpleGlazSys,
                HB.EnergyWindowMaterialBlind, HB.EnergyWindowMaterialGlazing, HB.EnergyWindowMaterialShade>>();
            foreach (var m in mats)
            {
                switch (m)
                {
                    case EnergyMaterial unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyMaterial);
                        break;
                    case EnergyMaterialNoMass unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyMaterialNoMass);
                        break;
                    case EnergyWindowMaterialBlind unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyWindowMaterialBlind);
                        break;
                    case EnergyWindowMaterialGas unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyWindowMaterialGas);
                        break;
                    case EnergyWindowMaterialGasCustom unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyWindowMaterialGasCustom);
                        break;
                    case EnergyWindowMaterialGasMixture unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyWindowMaterialGasMixture);
                        break;
                    case EnergyWindowMaterialGlazing unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyWindowMaterialGlazing);
                        break;
                    case EnergyWindowMaterialShade unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyWindowMaterialShade);
                        break;
                    case EnergyWindowMaterialSimpleGlazSys unused:
                        materials.Add(m.ToDragonfly() as HB.EnergyWindowMaterialSimpleGlazSys);
                        break;
                    default:
                        materials.Add(null);
                        break;
                }
            }

            return materials;
        }
    }
}