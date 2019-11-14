using System.ComponentModel;
using Autodesk.Revit.DB;

namespace Honeybee.Revit.CreateModel.Wrappers
{
    public class SpatialObjectWrapper : INotifyPropertyChanged
    {
        public string Name { get; }
        public SpatialObjectType ObjectType { get; set; }
        public LevelWrapper Level { get; set; }

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
