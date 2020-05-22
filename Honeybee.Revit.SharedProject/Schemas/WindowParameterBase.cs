using System.Collections.Generic;
using Newtonsoft.Json;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public abstract class WindowParameterBase : ISchema<object, object>
    {
        public abstract string Type { get; }
        public abstract object ToDragonfly();
        public abstract object ToHoneybee();
    }

    public class SingleWindow : WindowParameterBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("width")]
        public double Width { get; set; }

        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonProperty("sill_height")]
        public double SillHeight { get; set; } = 1d;

        public override object ToDragonfly()
        {
            return new DF.SingleWindow(Width, Height, SillHeight);
        }

        public override object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SimpleWindowRatio : WindowParameterBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("window_ratio")]
        public double WindowRatio { get; set; }

        public override object ToDragonfly()
        {
            return new DF.SimpleWindowRatio(WindowRatio);
        }

        public override object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RepeatingWindowRatio : WindowParameterBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("window_ratio")]
        public double WindowRatio { get; set; }

        [JsonProperty("window_height")]
        public double WindowHeight { get; set; }

        [JsonProperty("sill_height")]
        public double SillHeight { get; set; }

        [JsonProperty("horizontal_separation")]
        public double HorizontalSeparation { get; set; }

        [JsonProperty("vertical_separation")]
        public double VerticalSeparation { get; set; }

        public override object ToDragonfly()
        {
            return new DF.RepeatingWindowRatio(WindowRatio, WindowHeight, SillHeight, HorizontalSeparation, VerticalSeparation);
        }

        public override object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RectangularWindows : WindowParameterBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("origins")]
        public List<Point2D> Origins { get; set; } = new List<Point2D>();

        [JsonProperty("widths")]
        public List<double> Widths { get; set; }

        [JsonProperty("heights")]
        public List<double> Heights { get; set; }

        public override object ToDragonfly()
        {
            return new DF.RectangularWindows(Origins.ToDragonfly(), Widths, Heights);
        }

        public override object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }

    public class DetailedWindows : WindowParameterBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("polygons")]
        public List<List<Point2D>> Polygons { get; set; }

        public override object ToDragonfly()
        {
            return new DF.DetailedWindows(Polygons.ToDragonfly());
        }

        public override object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
