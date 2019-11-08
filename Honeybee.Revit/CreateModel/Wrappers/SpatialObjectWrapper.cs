using Autodesk.Revit.DB;

namespace Honeybee.Revit.CreateModel.Wrappers
{
    public class SpatialObjectWrapper
    {
        public string Name { get; set; }
        public SpatialObjectType ObjectType { get; set; }
        public LevelWrapper Level { get; set; }

        public SpatialObjectWrapper(Element e)
        {
            Name = e.Name;
            ObjectType = e.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode() 
                ? SpatialObjectType.Room 
                : SpatialObjectType.Space;
            Level = new LevelWrapper(e.Document.GetElement(e.LevelId) as Level);
        }
    }

    public enum SpatialObjectType
    {
        Room,
        Space
    }
}
