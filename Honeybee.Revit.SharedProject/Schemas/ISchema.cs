// ReSharper disable UnusedMemberInSuper.Global

namespace Honeybee.Revit.Schemas
{
    public interface ISchema<out T, out T1> where T : class where T1 : class
    {
        T ToDragonfly();
        T1 ToHoneybee();
    }
}
