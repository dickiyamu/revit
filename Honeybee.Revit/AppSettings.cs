﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Honeybee.Core;
using Newtonsoft.Json;
using NLog;

namespace Honeybee.Revit
{
    public sealed class AppSettings
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly Lazy<AppSettings> _lazy = new Lazy<AppSettings>(() => new AppSettings());

        public static AppSettings Instance
        {
            get { return _lazy.Value; }
        }

        public Dictionary<string, List<string>> Rooms2013 { get; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Rooms2010 { get; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Rooms2007 { get; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Rooms2004 { get; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Rooms1980To2004 { get; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> RoomsPre1980 { get; } = new Dictionary<string, List<string>>();

        private AppSettings()
        {
            try
            {
                Rooms2013 = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    Resources.StreamEmbeddedResource(Assembly.GetExecutingAssembly(),
                        "Honeybee.Revit.Resources.Schema.2013_registry.json"));
                Rooms2010 = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    Resources.StreamEmbeddedResource(Assembly.GetExecutingAssembly(),
                        "Honeybee.Revit.Resources.Schema.2010_registry.json"));
                Rooms2007 = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    Resources.StreamEmbeddedResource(Assembly.GetExecutingAssembly(),
                        "Honeybee.Revit.Resources.Schema.2007_registry.json"));
                Rooms2004 = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    Resources.StreamEmbeddedResource(Assembly.GetExecutingAssembly(),
                        "Honeybee.Revit.Resources.Schema.2004_registry.json"));
                Rooms1980To2004 = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    Resources.StreamEmbeddedResource(Assembly.GetExecutingAssembly(),
                        "Honeybee.Revit.Resources.Schema.1980_2004_registry.json"));
                RoomsPre1980 = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    Resources.StreamEmbeddedResource(Assembly.GetExecutingAssembly(),
                        "Honeybee.Revit.Resources.Schema.pre_1980_registry.json"));
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
        }
    }
}
