using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Entities
{
    public class MapMakerObjectProperty
    {
        public string PropertyName { get; set; }

        public string DisplayName { get; set; }

        public object Value { get; set; }

        public bool Readonly { get; set; }

        public bool Hidden { get; set; }

        public Type DataType { get; set; }

        public string[] PossibleValues { get; set; }

        public bool IsSpriteProperty { get; set; }

        public bool IsIgnoredInMapData { get; set; }

        public bool IsDoorSelectionProperty { get; set; }

        public bool IsNodeSelectionProperty { get; set; }

        public bool IsMultiSelect { get; set; }
    }
}
