using NetworkService.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class EntityTypeGroup : BindableBase
    {
        public string type { get; set; }
        public BindingList<Entity> entities { get; set; }

        public EntityTypeGroup(string type)
        {
            Type = type;
            Entities = new BindingList<Entity>();
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
        public BindingList<Entity> Entities
        {
            get { return entities; }
            set
            {
                entities = value;
                OnPropertyChanged(nameof(Entities));
            }
        }
    }
}
