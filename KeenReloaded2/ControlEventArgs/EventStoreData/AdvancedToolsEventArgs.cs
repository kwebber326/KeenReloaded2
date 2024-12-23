using KeenReloaded2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.ControlEventArgs.EventStoreData
{
    public class AdvancedToolsEventArgs
    {
        public List<GameObjectMapping> SelectedObjects { get; set; }

        public AdvancedToolsChangeData ChangeData { get; set; }
    }

    public enum AdvancedToolsActions
    {
        EXTEND,
        COPY,
        MOVE,
        DELETE
    }

    public class AdvancedToolsChangeData
    {
        public object ChangedData { get; set; }

        public object ChangeMetaData { get; set; }

        public AdvancedToolsActions Action { get; set; }
    }
}


