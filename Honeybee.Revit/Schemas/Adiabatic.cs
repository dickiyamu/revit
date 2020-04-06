﻿using Newtonsoft.Json;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Adiabatic : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override object ToDragonfly()
        {
            return new DF.Adiabatic(Type);
        }
    }
}
