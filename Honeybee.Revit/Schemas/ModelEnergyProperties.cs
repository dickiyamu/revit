using System.Collections.Generic;
using DF = DragonflySchema;

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
        public List<DF.IdealAirSystemAbridged> Hvacs { get; set; } = new List<DF.IdealAirSystemAbridged>();
        public List<DF.ProgramTypeAbridged> ProgramTypes { get; set; } = new List<DF.ProgramTypeAbridged>();
        public List<DF.AnyOf<DF.ScheduleRulesetAbridged, DF.ScheduleFixedIntervalAbridged>> Schedules { get; set; } 
            = new List<DF.AnyOf<DF.ScheduleRulesetAbridged, DF.ScheduleFixedIntervalAbridged>>();
        public List<DF.ScheduleTypeLimit> ScheduleTypeLimits { get; set; } = new List<DF.ScheduleTypeLimit>();

        public ModelEnergyProperties()
        {
            GlobalConstructionSet = new ConstructionSetAbridged();
            ConstructionSets.Add(GlobalConstructionSet);
        }

        public DF.ModelEnergyProperties ToDragonfly()
        {
            return new DF.ModelEnergyProperties(
                Type,
                TerrainType,
                GlobalConstructionSet.Identifier,
                ConstructionSets.ToDragonfly(),
                Constructions.ToDragonfly(),
                Materials.ToDragonfly()
            );
        }
    }
}