using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleClaw : BaseTongsItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override int CatchPower => 15;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<IcicleClawProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 12;
            Item.SetWeaponValues(24, 3.5f);
            Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(0, 0, 20));
            Item.autoReuse = true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.IcicleItems)]
    public class IcicleClawProj : BaseTongsProj
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public static ATex IcicleClawChain { get; private set; }
        public static ATex IcicleClawHandle { get; private set; }

        public override Vector2 TongPosOffset => new Vector2(24, -2);

        public override int MaxFlyLength => 16 * 12;

        public override Vector2 HandelOffset => new Vector2(20, -6);

        public override int ItemType => ModContent.ItemType<IcicleClaw>();

        public override Texture2D GetHandleTex() => IcicleClawHandle.Value;
        public override Texture2D GetLineTex() => IcicleClawChain.Value;

        public override Vector2 LineDrawStartPosOffset()
            => -HandleRot.ToRotationVector2() * 4;

        public override void OnHitNPCFlying(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //偷偷借用一下雪花护身符的特效弹幕
            for (int i = -1; i < 1; i++)
            {
                Vector2 velocity = (Projectile.rotation + (i * 0.4f)).ToRotationVector2();
                Projectile.NewProjectileFromThis<SnowflakeSpike>(Projectile.Center
                    , velocity * Main.rand.NextFloat(5, 6f),
                     (int)(Projectile.damage * 0.3f), Projectile.knockBack, ai1: Main.rand.NextFloat(8, 10));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
