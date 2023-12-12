using System;
using System.Collections.Generic;
using System.Text;

namespace Arcemi.Models
{
    /// <summary>
    /// A model for a JBP blueprint file 
    /// </summary>
    public class Blueprint : Model
    {
        public Blueprint(ModelDataAccessor accessor) : base(accessor)
        {
        }

        public string AssetId { get => A.Value<string>("AssetId"); set => A.Value(value); }

        public BlueprintData Data => A.Object<BlueprintData>(factory: BlueprintData.Factory);
    }
}
