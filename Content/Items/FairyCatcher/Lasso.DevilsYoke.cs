using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher
{
    public class DevilsYoke : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override int RightProjType => ModContent.ProjectileType<DevilsYokeSwing>();
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

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherItems)]
    public class DevilsYokeSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + "DevilsYokeCatcher";

        public static ATex PurpleChain { get; set; }

        public override void SetSwingProperty()
        {
            minDistance = 48;
            maxDistance = 128;

            base.SetSwingProperty();
            DrawOriginOffsetX = 8;
        }

        public override Texture2D GetStringTex() => PurpleChain.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));

        public override Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return CursorCenter - new Vector2(cursorTex.Width / 2, 0).RotatedBy(cursorRotation);
        }
    }
}
