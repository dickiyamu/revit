namespace Honeybee.Revit.Schemas
{
    public abstract class BoundaryCondition
    {
        public abstract string Type { get; }
    }

    public class Outdoors : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }
        
        public bool SunExposure { get; set; }
        public bool WindExposure { get; set; }
        public string ViewFactor { get; set; } = "autocalculate";
    }

    public class Ground : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }
    }

    public class Adiabatic : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }
    }

    public class Surface : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }
    }
}
