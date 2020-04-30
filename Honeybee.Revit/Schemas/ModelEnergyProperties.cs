using System.Collections.Generic;
using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ModelEnergyProperties : ISchema<DF.ModelEnergyProperties>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public List<ConstructionBase> Constructions { get; set; } = new List<ConstructionBase>();
        public List<MaterialBase> Materials { get; set; } = new List<MaterialBase>();
        public DF.ModelEnergyProperties.TerrainTypeEnum TerrainType { get; set; } = DF.ModelEnergyProperties.TerrainTypeEnum.City;
        public ConstructionSetAbridged GlobalConstructionSet { get; set; }
        public List<ConstructionSetBase> ConstructionSets { get; set; } = new List<ConstructionSetBase>();
        public List<HB.IdealAirSystemAbridged> Hvacs { get; set; } = null;
        public List<HB.ProgramTypeAbridged> ProgramTypes { get; set; } = null;
        public List<DF.AnyOf<HB.ScheduleRulesetAbridged, HB.ScheduleFixedIntervalAbridged>> Schedules { get; set; } = null;
        public List<HB.ScheduleTypeLimit> ScheduleTypeLimits { get; set; } = null;

        public ModelEnergyProperties()
        {
            GlobalConstructionSet = new ConstructionSetAbridged();
            ConstructionSets.Add(GlobalConstructionSet);
        }

        public DF.ModelEnergyProperties ToDragonfly()
        {
            return new DF.ModelEnergyProperties(
                TerrainType,
                GlobalConstructionSet.Identifier,
                ConstructionSets.ToDragonfly(),
                Constructions.ToDragonfly(),
                Materials.ToDragonfly()
            );
        }
    }
}