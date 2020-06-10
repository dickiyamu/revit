using System.Collections.Generic;
using Newtonsoft.Json;

namespace Honeybee.Revit.CreateModel
{
    public class OpenStudioWorkflow
    {
        [JsonProperty("measure_paths")]
        public List<string> MeasurePaths = new List<string>();

        [JsonProperty("steps")]
        public List<Step> Steps = new List<Step>();
    }

    public class Step
    {
        [JsonProperty("measure_dir_name")]
        public string MeasureDirName;

        public Step(string dirName)
        {
            MeasureDirName = dirName;
        }
    }
}