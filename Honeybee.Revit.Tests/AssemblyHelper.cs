using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Honeybee.Revit.Tests
{
    public class AssemblyHelper
    {
        private readonly string _moduleRootFolder;
        private readonly IEnumerable<string> _additionalResolutionPaths;
        private readonly bool _testMode;

        public AssemblyHelper(string moduleRootFolder, IEnumerable<string> additionalResolutionPaths, bool testMode = false)
        {
            if (additionalResolutionPaths == null)
                additionalResolutionPaths = new List<string>();

            _moduleRootFolder = moduleRootFolder;
            _additionalResolutionPaths = additionalResolutionPaths;
            _testMode = testMode;
        }

        public Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            try
            {
                var targetAssemblyName = new AssemblyName(args.Name).Name + ".dll";

                // First check the core path
                var assemblyPath = Path.Combine(_moduleRootFolder, targetAssemblyName);
                if (_testMode)
                {
                    Console.WriteLine("trying to resolve " + targetAssemblyName + " at " + assemblyPath);
                }
                if (File.Exists(assemblyPath))
                {
                    if (_testMode)
                    {
                        Console.WriteLine("loading from " + assemblyPath);

                    }
                    return Assembly.LoadFrom(assemblyPath);
                }

                // Then check all additional resolution paths
                foreach (var resolutionPath in _additionalResolutionPaths)
                {
                    assemblyPath = Path.Combine(resolutionPath, targetAssemblyName);
                    if (_testMode)
                    {
                        Console.WriteLine("trying to resolve " + targetAssemblyName + " at " + assemblyPath);
                    }

                    if (File.Exists(assemblyPath))
                    {
                        if (_testMode)
                        {
                            Console.WriteLine("loading from " + assemblyPath);
                        }
                        return Assembly.LoadFrom(assemblyPath);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "There location of the assembly, " + $"{args.Name} could not be resolved for loading.", ex);
            }
        }
    }
}
