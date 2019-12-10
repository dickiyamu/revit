using System.ComponentModel;
using Autodesk.Revit.DB;
using Honeybee.Revit.Schemas;

namespace Honeybee.Revit.CreateModel.Wrappers
{
    public class SpatialObjectWrapper : INotifyPropertyChanged
    {
        public string Name { get; }
        public SpatialObjectType ObjectType { get; set; }
        public LevelWrapper Level { get; set; }

        private ConstructionSet _constructionSet = new ConstructionSet();
        public ConstructionSet ConstructionSet
        {
            get { return _constructionSet; }
            set { _constructionSet = value; RaisePropertyChanged(nameof(ConstructionSet)); }
        }

        private bool _isConstructionSetOverriden;
        public bool IsConstructionSetOverriden
        {
            get { return _isConstructionSetOverriden; }
            set { _isConstructionSetOverriden = value; RaisePropertyChanged(nameof(IsConstructionSetOverriden)); }
        }

        private ProgramType _programType = new ProgramType();
        public ProgramType ProgramType
        {
            get { return _programType; }
            set { _programType = value; RaisePropertyChanged(nameof(ProgramType)); }
        }

        private bool _isProgramTypeOverriden;
        public bool IsProgramTypeOverriden
        {
            get { return _isProgramTypeOverriden; }
            set { _isProgramTypeOverriden = value; RaisePropertyChanged(nameof(IsProgramTypeOverriden)); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; RaisePropertyChanged(nameof(IsSelected)); }
        }

        public SpatialObjectWrapper(Element e)
        {
            Name = e.Name;
            ObjectType = e.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode() 
                ? SpatialObjectType.Room 
                : SpatialObjectType.Space;
            Level = new LevelWrapper(e.Document.GetElement(e.LevelId) as Level);
        }

        public override bool Equals(object obj)
        {
            return obj is SpatialObjectWrapper item && Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum SpatialObjectType
    {
        Room,
        Space
    }
}
