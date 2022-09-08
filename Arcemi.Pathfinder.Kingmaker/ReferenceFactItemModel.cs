using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class ReferenceFactItemModel : FactItemModel
    {
        public ReferenceFactItemModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string ReferencedId { get => A.Value<string>("$ref"); set => A.Value(value, "$ref"); }

        // To-do: add function to get referenced fact from unit
    }
}
