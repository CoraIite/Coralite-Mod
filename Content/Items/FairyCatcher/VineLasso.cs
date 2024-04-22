using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.CursorAIs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher
{
    public class VineLasso : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherItems+Name;

        public override void SetOtherDefaults()
        {
            Item.damage = 20;
            Item.shoot = ModContent.ProjectileType<VineLassoCatcher>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 12;
        }
    }

    public class VineLassoCatcher : BaseFairyCatcherProj
    {
        public override void SetOtherDefaults()
        {
            cursorMovement = new NormalCursor(3, 0.2f, 0.2f, 0.99f);
        }
    }
}
