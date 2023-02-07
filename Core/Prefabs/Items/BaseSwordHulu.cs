using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public class BaseHulu : ModItem
    {
        private readonly string TexturePath;
        private readonly bool PathHasName;
        public readonly int slotCount;

        public List<Item> Yujians;

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public BaseHulu(int slotCount ,string texturePath= AssetDirectory.YujianHulu, bool pathHasName = false)
        {
            TexturePath = texturePath;
            PathHasName = pathHasName;
            this.slotCount = slotCount;
            Yujians = new List<Item>();
        }


    }
}
