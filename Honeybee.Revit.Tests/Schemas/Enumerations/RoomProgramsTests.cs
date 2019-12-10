using System.Collections.Generic;
using System.Linq;
using Honeybee.Core;
using Honeybee.Revit.Schemas.Enumerations;
using NUnit.Framework;
using RTF.Framework;

namespace Honeybee.Revit.Tests.Schemas.Enumerations
{
    [TestFixture]
    public class RoomProgramsTests : RevitTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Validate2013()
        {
            var buildingPrograms = Enumeration.GetAll<BuildingPrograms>().ToList();
            Assert.That(buildingPrograms.Count, Is.EqualTo(23));

            foreach (var bp in buildingPrograms)
            {
                var jsonRooms = AppSettings.Instance.Rooms2013[bp.DisplayName];
                var enumRooms = jsonRooms.Select(Enumeration.FromDisplayName<RoomPrograms>);
                Assert.That(jsonRooms.Count, Is.EqualTo(enumRooms.Count()));
            }
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Validate2010()
        {
            var buildingPrograms = Enumeration.GetAll<BuildingPrograms>().ToList();
            Assert.That(buildingPrograms.Count, Is.EqualTo(23));

            foreach (var bp in buildingPrograms)
            {
                var jsonRooms = AppSettings.Instance.Rooms2010[bp.DisplayName];
                var enumRooms = jsonRooms.Select(Enumeration.FromDisplayName<RoomPrograms>);
                Assert.That(jsonRooms.Count, Is.EqualTo(enumRooms.Count()));
            }
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Validate2007()
        {
            var buildingPrograms = Enumeration.GetAll<BuildingPrograms>().ToList();
            Assert.That(buildingPrograms.Count, Is.EqualTo(23));

            foreach (var bp in buildingPrograms)
            {
                var jsonRooms = AppSettings.Instance.Rooms2007[bp.DisplayName];
                var enumRooms = jsonRooms.Select(Enumeration.FromDisplayName<RoomPrograms>);
                Assert.That(jsonRooms.Count, Is.EqualTo(enumRooms.Count()));
            }
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Validate2004()
        {
            var buildingPrograms = Enumeration.GetAll<BuildingPrograms>().ToList();
            Assert.That(buildingPrograms.Count, Is.EqualTo(23));

            foreach (var bp in buildingPrograms)
            {
                var jsonRooms = AppSettings.Instance.Rooms2004[bp.DisplayName];
                var enumRooms = jsonRooms.Select(Enumeration.FromDisplayName<RoomPrograms>);
                Assert.That(jsonRooms.Count, Is.EqualTo(enumRooms.Count()));
            }
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void Validate1980To2004()
        {
            var keys = AppSettings.Instance.Rooms1980To2004.Keys;
            var buildingPrograms = keys.Select(Enumeration.FromDisplayName<BuildingPrograms>).ToList();
            Assert.That(buildingPrograms.Count, Is.EqualTo(keys.Count));

            foreach (var bp in buildingPrograms)
            {
                var jsonRooms = AppSettings.Instance.Rooms1980To2004.ContainsKey(bp.DisplayName)
                    ? AppSettings.Instance.Rooms1980To2004[bp.DisplayName]
                    : new List<string>();
                if (!jsonRooms.Any()) continue;

                var enumRooms = jsonRooms.Select(Enumeration.FromDisplayName<RoomPrograms>);
                Assert.That(jsonRooms.Count, Is.EqualTo(enumRooms.Count()));
            }
        }

        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ValidatePre1980()
        {
            var keys = AppSettings.Instance.RoomsPre1980.Keys;
            var buildingPrograms = keys.Select(Enumeration.FromDisplayName<BuildingPrograms>).ToList();
            Assert.That(buildingPrograms.Count, Is.EqualTo(keys.Count));

            foreach (var bp in buildingPrograms)
            {
                var jsonRooms = AppSettings.Instance.RoomsPre1980.ContainsKey(bp.DisplayName)
                    ? AppSettings.Instance.RoomsPre1980[bp.DisplayName]
                    : new List<string>();
                if (!jsonRooms.Any()) continue;

                var enumRooms = jsonRooms.Select(Enumeration.FromDisplayName<RoomPrograms>);
                Assert.That(jsonRooms.Count, Is.EqualTo(enumRooms.Count()));
            }
        }
    }
}
