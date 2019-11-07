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
                _assemblyResolver.Setup(
                    $@"C:\Users\{Environment.UserName}\AppData\Roaming\Autodesk\Revit\Addins\2019\Honeybee.Revit");
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
