using System.Collections.ObjectModel;

namespace Reedy.Core.Models
{
    public class Directory : Item
    {
        public Directory(string name, string path) { Name = name; Path = path; }
        public ObservableCollection<Item> Children { get; } = new ObservableCollection<Item>();
    }
}
