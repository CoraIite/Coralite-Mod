using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Lasso
{
    public class DevilsYoke : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + Name;

        public override int CatchPower => 30;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<DevilsYokeSwing>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 22;
            Item.shootSpeed = 9;
            Item.SetWeaponValues(25, 3);
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 1));
            Item.autoReuse = true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherLasso)]
    public class DevilsYokeSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "DevilsYokeCatcher";

        public static ATex PurpleChain { get; set; }

        public override void SetSwingProperty()
        {
            minDistance = 48;
            shootTime = 18;

            base.SetSwingProperty();

            Projectile.width = Projectile.height = 40;
            Projectile.extraUpdates = 4;
            DrawOriginOffsetX = 8;
        }

        public override void SetMaxDistance()
        {
            MaxDistance = 168;
        }

        public override Texture2D GetStringTex() => PurpleChain.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));
    }
}
