using Autodesk.Revit.DB;

namespace Honeybee.Revit.CreateModel.Wrappers
{
    public class LevelWrapper
    {
        public string Name { get; set; }
        public ElementId Id { get; }
        public double Elevation { get; set; }

        public LevelWrapper(Level l)
        {
            Name = l.Name;
            Id = l.Id;
            Elevation = l.Elevation;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelWrapper item && Id.Equals(item.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
