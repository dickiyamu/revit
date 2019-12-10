using NUnit.Framework;
using RTF.Framework;

namespace Honeybee.Revit.Tests.CreateModel
{
    [TestFixture]
    public class CreateModelCommand : RevitTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void LaunchWindow()
        {
            //TODO: Write unit test.
        }
    }
}
