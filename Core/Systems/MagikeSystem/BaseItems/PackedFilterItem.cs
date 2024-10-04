using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class PackedFilterItem(int value, int rare, string texturePath = AssetDirectory.MagikeFilters, bool pathHasName = false) : FilterItem
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = value;
            Item.rare = rare;
        }
    }
}
