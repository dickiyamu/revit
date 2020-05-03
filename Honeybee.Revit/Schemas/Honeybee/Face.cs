using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas.Honeybee
{
    public class Face : IBaseObject, ISchema<object, HB.Face>
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Face_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("geometry")]
        public Face3D Geometry { get; set; }

        [JsonProperty("face_type")]
        public HB.Face.FaceTypeEnum? FaceType { get; set; }

        [JsonProperty("boundary_condition")]
        public HB.AnyOf<HB.Ground, HB.Outdoors, HB.Adiabatic, HB.Surface> BoundaryCondition { get; set; }

        [JsonProperty("properties")]
        public HB.FacePropertiesAbridged Properties { get; set; }

        [JsonProperty("apertures")]
        public List<HB.Aperture> Apertures { get; set; }

        [JsonProperty("doors")]
        public List<HB.Door> Doors { get; set; }

        [JsonProperty("indoor_shades")]
        public List<HB.Shade> IndoorShades { get; set; }

        [JsonProperty("outdoor_shades")]
        public List<HB.Shade> OutdoorShades { get; set; }

        [JsonProperty("user_data")]
        public object UserData { get; set; } = new Dictionary<string, object>();

        [JsonConstructor]
        public Face()
        {
        }

        public Face(List<Point3D> boundary, List<List<Point3D>> holes)
        {
            Geometry = new Face3D {Boundary = boundary, Holes = holes};
        }

        public Face(Autodesk.Revit.DB.Face face)
        {
            Geometry = new Face3D(face);
        }

        public object ToDragonfly()
        {
            throw new NotImplementedException();
        }

        public HB.Face ToHoneybee()
        {
            return new HB.Face(
                Identifier,
                Geometry.ToHoneybee(),
                null, // face type
                null, // boundary condition
                null, // properties
                null, // apertures
                null, // doors
                null, // indoor shades
                null, // outdoor shades
                DisplayName,
                null // user data
            );
        }
    }
}
