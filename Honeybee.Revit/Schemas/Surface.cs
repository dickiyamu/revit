using System;
using System.Collections.Generic;
using Honeybee.Revit.Schemas.Converters;
using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class HoneybeeSurface : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return "Surface"; }
        }

        public Tuple<string, string, string> BoundaryConditionObjects { get; set; }

        [JsonConstructor]
        public HoneybeeSurface()
        {
        }

        public HoneybeeSurface(Tuple<string, string, string> bConObj)
        {
            BoundaryConditionObjects = bConObj;
        }

        public override object ToDragonfly()
        {
            throw new NotImplementedException();
        }

        public override object ToHoneybee()
        {
            return new HB.Surface(BoundaryConditionObjects.ToHoneybee());
        }
    }

    public class DragonflySurface : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return "Surface"; }
        }

        //[JsonProperty("boundary_condition_objects")]
        //[JsonConverter(typeof(BoundaryConditionObjectsConverter))]
        public Tuple<int, string> BoundaryConditionObjects { get; set; }

        [JsonConstructor]
        public DragonflySurface()
        {
        }

        public DragonflySurface(Tuple<int, string> bConObj)
        {
            BoundaryConditionObjects = bConObj;
        }

        public override object ToDragonfly()
        {
            return new HB.Surface(BoundaryConditionObjects.ToDragonfly());
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }

    public static class SurfaceExtensions
    {
        public static List<string> ToDragonfly(this Tuple<int, string> boundaryConditionObj)
        {
            var (adjacentCurveIndex, adjacentRoomName) = boundaryConditionObj;

            return new List<string>
            {
                $"{adjacentRoomName}..Face{adjacentCurveIndex + 1}",
                adjacentRoomName
            };
        }

        public static List<string> ToHoneybee(this Tuple<string, string, string> boundaryConditionObj)
        {
            var (adjacentAperture, adjacentFace, adjacentRoom) = boundaryConditionObj;

            var output = new List<string>();
            if (!string.IsNullOrWhiteSpace(adjacentAperture))
            {
                output.Add(adjacentAperture);
            }
            output.Add(adjacentFace);
            output.Add(adjacentRoom);
            return output;
        }
    }
}
