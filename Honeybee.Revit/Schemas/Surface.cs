using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Surface : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        public Tuple<string, string, string> BoundaryConditionObjects { get; set; }

        [JsonConstructor]
        public Surface()
        {
        }

        public Surface(Tuple<string, string, string> bConObj)
        {
            BoundaryConditionObjects = bConObj;
        }

        public override object ToDragonfly()
        {
            return new HB.Surface(BoundaryConditionObjects.ToDragonfly());
        }

        public override object ToHoneybee()
        {
            return new HB.Surface(BoundaryConditionObjects.ToHoneybee());
        }
    }

    public static class SurfaceExtensions
    {
        public static List<string> ToDragonfly(this Tuple<string, string, string> boundaryConditionObj)
        {
            var (unused, adjacentCurveIndex, adjacentRoomId) = boundaryConditionObj;
            var index = int.Parse(adjacentCurveIndex);
            return new List<string>
            {
                $"{adjacentRoomId}..Face{index + 1}", // we +1 because this is not 0-indexed
                adjacentRoomId
            };
        }

        public static List<string> ToHoneybee(this Tuple<string, string, string> boundaryConditionObj)
        {
            var (adjacentApertureId, adjacentFaceId, adjacentRoomId) = boundaryConditionObj;

            var output = new List<string>();
            if (!string.IsNullOrWhiteSpace(adjacentApertureId))
            {
                output.Add(adjacentApertureId);
            }
            output.Add(adjacentFaceId);
            output.Add(adjacentRoomId);
            return output;
        }
    }
}
