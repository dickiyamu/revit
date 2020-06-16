using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;

namespace Honeybee.Core
{
    public static class Json
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public static JsonSerializerSettings Settings { get; set; }

        static Json()
        {
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                CheckAdditionalContent = true,
                Formatting = Formatting.Indented,
                Error = delegate (object sender, ErrorEventArgs args)
                {
                    args.ErrorContext.Handled = true;
#if DEBUG
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
#endif
                }
            };
        }

        public static T Deserialize<T>(string json) where T : new()
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<T>(json, Settings);
                return obj;
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                throw new Exception("Failed to deserialize from JSON: " + e.Message);
            }
        }

        public static string Serialize(object obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj, Settings);
                return json;
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return string.Empty;
            }
        }
    }
}
