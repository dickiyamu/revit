namespace Honeybee.Revit.Schemas
{
    public interface ISchema<out T> where T : class
    {
        T ToDragonfly();
    }
}
