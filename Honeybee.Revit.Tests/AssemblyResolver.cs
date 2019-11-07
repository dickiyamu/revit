using System;

namespace Honeybee.Revit.Tests
{
    public class AssemblyResolver
    {
        private AssemblyHelper _assemblyHelper;

        public void Setup(string corePath)
        {
            if (_assemblyHelper != null) return;

            _assemblyHelper = new AssemblyHelper(corePath, null);
            AppDomain.CurrentDomain.AssemblyResolve += _assemblyHelper.ResolveAssembly;
        }

        public void TearDown()
        {
            if (_assemblyHelper == null)
                return;

            AppDomain.CurrentDomain.AssemblyResolve -= _assemblyHelper.ResolveAssembly;
            _assemblyHelper = null;
        }
    }
}
