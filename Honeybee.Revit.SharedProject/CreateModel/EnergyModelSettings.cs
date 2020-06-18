using System.Collections.Generic;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;

namespace Honeybee.Revit.CreateModel
{
    public class EnergyModelSettings
    {
        public List<string> Shades { get; set; } = new List<string>();
        public string BldgConstructionSet { get; set; } = new ConstructionSet().Identifier;
        public string BldgProgramType { get; set; } = new ProgramType().Identifier;
        public List<SpatialObjectWrapper> Rooms { get; set; } = new List<SpatialObjectWrapper>();
    }
}
