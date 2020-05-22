using Autodesk.Revit.DB;

namespace Honeybee.Core
{
    public static class FamilyUtils
    {
        /// <summary>
        /// A standard implementation for Revit's family loading interface.
        /// </summary>
        public class FamilyLoadProcessor : IFamilyLoadOptions
        {
            private readonly bool _overwriteParameters;
            private readonly bool _useNewShared;

            public FamilyLoadProcessor(bool overwriteParams = false, bool useNewShared = true)
            {
                _overwriteParameters = overwriteParams;
                _useNewShared = useNewShared;
            }

            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = _overwriteParameters;

                return true;
            }

            public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                var sourceToUse = _useNewShared ? FamilySource.Family : FamilySource.Project;
                source = sourceToUse;
                overwriteParameterValues = _overwriteParameters;

                return true;
            }
        }
    }
}
