using System.Collections.Generic;
using Honeybee.Core;
// ReSharper disable UnusedMember.Global

namespace Honeybee.Revit.Schemas.Enumerations
{
    public class Vintages : Enumeration
    {
        public static readonly Vintages Vintage2013 = new Vintages(0, "2013");
        public static readonly Vintages Vintage2010 = new Vintages(1, "2010");
        public static readonly Vintages Vintage2007 = new Vintages(2, "2007");
        public static readonly Vintages Vintage2004 = new Vintages(3, "2004");
        public static readonly Vintages Vintage1980To2004 = new Vintages(4, "1980_2004");
        public static readonly Vintages VintagePre1980 = new Vintages(5, "pre_1980");

        public Vintages()
        {
        }

        private Vintages(int value, string displayName) : base(value, displayName)
        {
        }

        public IEnumerable<Vintages> GetAll()
        {
            return GetAll<Vintages>();
        }
    }
}
