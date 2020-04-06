namespace Honeybee.Revit.Schemas
{
    public abstract class ConstructionBase : ISchema<object>
    {
        public abstract string Type { get; }
        public abstract string Name { get; set; }
        public abstract object ToDragonfly();
    }
}
