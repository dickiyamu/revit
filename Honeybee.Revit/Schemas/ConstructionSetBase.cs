namespace Honeybee.Revit.Schemas
{
    public abstract class ConstructionSetBase : IBaseObject, ISchema<object, object>
    {
        public abstract string Type { get; }
        public abstract string Identifier { get; set; }
        public abstract string DisplayName { get; set; }
        public abstract object ToDragonfly();
        public abstract object ToHoneybee();
    }
}
