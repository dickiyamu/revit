using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ModelEnergyProperties : ISchema<DF.ModelEnergyProperties, object>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public List<ConstructionBase> Constructions { get; set; } = new List<ConstructionBase>();
        public List<MaterialBase> Materials { get; set; } = new List<MaterialBase>();
        public DF.ModelEnergyProperties.TerrainTypeEnum TerrainType { get; set; } = DF.ModelEnergyProperties.TerrainTypeEnum.City;

        [JsonProperty("global_construction_set")]
        public string GlobalConstructionSet { get; set; }

        [JsonProperty("construction_sets")]
        public List<DF.AnyOf<HB.ConstructionSetAbridged, HB.ConstructionSet>> ConstructionSets { get; set; } = new List<DF.AnyOf<HB.ConstructionSetAbridged, HB.ConstructionSet>>();
        
        [JsonProperty("program_types")]
        public List<DF.AnyOf<HB.ProgramTypeAbridged, HB.ProgramType>> ProgramTypes { get; set; } = new List<DF.AnyOf<HB.ProgramTypeAbridged, HB.ProgramType>>();

        public List<HB.IdealAirSystemAbridged> Hvacs { get; set; } = null;
        public List<DF.AnyOf<HB.ScheduleRulesetAbridged, HB.ScheduleFixedIntervalAbridged>> Schedules { get; set; } = null;
        public List<HB.ScheduleTypeLimit> ScheduleTypeLimits { get; set; } = null;

        public DF.ModelEnergyProperties ToDragonfly()
        {
            return new DF.ModelEnergyProperties(
                TerrainType,
                GlobalConstructionSet,
                ConstructionSets,
                Constructions.ToDragonfly(),
                Materials.ToDragonfly(),
                null, // hvacs
                ProgramTypes,
                null, // schedules
                null // schedule type limits
            );
        }

        public object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }
}