﻿using System;
using System.Collections.Generic;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class OpaqueConstructionAbridged : ConstructionBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"OpaqueConstructionAbridged_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
        public List<string> Layers { get; set; } = new List<string>();

        public override object ToDragonfly()
        {
            return new DF.OpaqueConstructionAbridged(Identifier, Layers, DisplayName, Type);
        }
    }
}
