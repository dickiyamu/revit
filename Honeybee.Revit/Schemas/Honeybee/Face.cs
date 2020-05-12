using System;
using System.Collections.Generic;
using System.Linq;
using Honeybee.Core.Extensions;
using Honeybee.Revit.CreateModel.Wrappers;
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
        public HB.Face.FaceTypeEnum FaceType { get; set; }

        [JsonProperty("boundary_condition")]
        public BoundaryConditionBase BoundaryCondition { get; set; }

        [JsonProperty("properties")]
        public HB.FacePropertiesAbridged Properties { get; set; }

        [JsonProperty("apertures")]
        public List<Aperture> Apertures { get; set; } = new List<Aperture>();

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
            BoundaryCondition = new Outdoors();
            Properties = new HB.FacePropertiesAbridged(new HB.FaceEnergyPropertiesAbridged());
        }

        public Face(Autodesk.Revit.DB.Face face)
        {
            Geometry = new Face3D(face);
            BoundaryCondition = new Outdoors();
            Properties = new HB.FacePropertiesAbridged(new HB.FaceEnergyPropertiesAbridged());
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
                FaceType,
                new HB.AnyOf<HB.Ground, HB.Outdoors, HB.Adiabatic, HB.Surface>(BoundaryCondition.ToHoneybee()),
                Properties,
                DisplayName,
                null, // user data
                Apertures?.Select(x => x.ToHoneybee()).ToList(),
                null, // doors
                null, // indoor shades
                null // outdoor shades
            );
        }
    }

    public static class FaceExtensions
    {
        public static bool OverlapsWith(this Face face, Face other)
        {
            if (face.Geometry.Boundary.Count != other.Geometry.Boundary.Count) return false;
            if (face.Geometry.Holes.Count != other.Geometry.Holes.Count) return false;

            if (face.Geometry.Boundary.Any(pt => !other.Geometry.Boundary.Contains(pt))) return false;

            return true;
        }

        public static void AssignBoundaryConditions(this Face face, IEnumerable<SpatialObjectWrapper> objects)
        {
            var adjacentRoomId = string.Empty;
            var adjacentFaceId = string.Empty;

            Face adjacentFace = null;
            foreach (var so in objects)
            {
                if (!string.IsNullOrWhiteSpace(adjacentRoomId) &&
                    !string.IsNullOrWhiteSpace(adjacentFaceId))
                    break;

                var faces = so.Room2D.Faces;
                for (var i = 0; i < faces.Count; i++)
                {
                    var f = faces[i];
                    if (!f.OverlapsWith(face))
                        continue;

                    adjacentFace = faces[i];
                    adjacentFaceId = faces[i].Identifier;
                    adjacentRoomId = so.Room2D.Identifier;
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(adjacentRoomId) &&
                !string.IsNullOrWhiteSpace(adjacentFaceId))
            {
                // (Konrad) We found a matching Surface Boundary Condition.
                // Tuple for HB Surface is always ApertureId, FaceId, RoomId.
                var bConditionObj = new Tuple<string, string, string>(string.Empty, adjacentFaceId, adjacentRoomId);

                face.BoundaryCondition = new Surface(bConditionObj);

                if (face.Apertures == null || !face.Apertures.Any())
                    return;

                var comparer = new ApertureComparer();
                foreach (var aperture in face.Apertures)
                {
                    var adjacentAperture = adjacentFace?.Apertures.FirstOrDefault(comparer, aperture);
                    if (adjacentAperture == null)
                        continue;

                    var bcObject = new Tuple<string, string, string>(adjacentAperture.Identifier, adjacentFaceId, adjacentRoomId);
                    aperture.BoundaryCondition = new Surface(bcObject);
                }

                return;
            }

            face.BoundaryCondition = new Outdoors();
        }
    }
}
