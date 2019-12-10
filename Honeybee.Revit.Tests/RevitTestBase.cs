using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NUnit.Framework;

namespace Honeybee.Revit.Tests
{
    public class RevitTestBase
    {
        public Application App { get; set; }
        public UIApplication UiApp { get; set; }
        public Document Doc { get; set; }
        public UIDocument UiDoc { get; set; }
        private AssemblyResolver _assemblyResolver;

        [SetUp]
        public void Setup()
        {
            if (_assemblyResolver == null)
            {
                _assemblyResolver = new AssemblyResolver();
#if Build2020
                const int year = 2020;
#elif Build2019
                const int year = 2019;
#else
                const int year = 2018;
#endif
                _assemblyResolver.Setup($@"C:\Users\{Environment.UserName}\AppData\Roaming\Autodesk\Revit\Addins\{year}\Honeybee.Revit");
            }

            UiApp = RTF.Applications.RevitTestExecutive.CommandData.Application;
            App = RTF.Applications.RevitTestExecutive.CommandData.Application.Application;
            Doc = RTF.Applications.RevitTestExecutive.CommandData.Application.ActiveUIDocument.Document;
            UiDoc = RTF.Applications.RevitTestExecutive.CommandData.Application.ActiveUIDocument;
        }

        [TearDown]
        public virtual void ShutDownTransactionManager()
        {
            if (_assemblyResolver == null) return;

            _assemblyResolver.TearDown();
            _assemblyResolver = null;
        }
    }
}
