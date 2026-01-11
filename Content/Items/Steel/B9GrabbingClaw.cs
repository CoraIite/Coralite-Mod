using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9GrabbingClaw : BaseTongsItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override int CatchPower => 35;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<B9GrabbingClawProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 14;
            Item.shootSpeed = 16;
            Item.SetWeaponValues(50, 3);
            Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(0, 3));
            Item.autoReuse = true;
        }
    }

    [VaultLoaden(AssetDirectory.SteelItems)]
    public class B9GrabbingClawProj : BaseTongsProj
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public static ATex B9GrabbingClawChain { get; private set; }
        public static ATex B9GrabbingClawHandle { get; private set; }

        public override Vector2 TongPosOffset => new Vector2(18, 0);
        public override Vector2 HandelOffset => new Vector2(16, -6);

        public override int ItemType => ModContent.ItemType<B9GrabbingClaw>();

        public override int MaxFlyLength => 16 * 22;

        /// <summary>
        /// 目标的索引
        /// </summary>
        public int Target { get; set; }

        public override Texture2D GetHandleTex() => B9GrabbingClawHandle.Value;
        public override Texture2D GetLineTex() => B9GrabbingClawChain.Value;

        public override Vector2 LineDrawStartPosOffset()
            => -HandleRot.ToRotationVector2() * 10;

        public override void OnHitNPCFlying(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public override void StartAttack(int @catch, float speed, int damage, float knockack)
        {
            base.StartAttack(@catch, speed, damage, knockack);
            Target = -1;
        }

        public override void Flying()
        {
            base.Flying();


            int alpha = 0;
            float scale = 1.2f;

            if (Target == -1)
            {
                alpha = 150;
                scale = 0.9f;
            }

            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Vector2.Lerp(Projectile.Center, Projectile.oldPosition + Projectile.Size / 2, i / 3f)
                     , DustID.BlueTorch, Vector2.Zero, alpha, Scale: scale);
                d.noGravity = true;
            }

            if (Target == -1)
            {
                if (Timer != 0 && Timer % 10 == 0 &&
                    Helper.TryFindClosestEnemy(Projectile.Center, 500, n => n.CanBeChasedBy(), out NPC target))
                    Target = target.whoAmI;
                return;
            }

            NPC target2 = Main.npc[Target];
            if (!target2.CanBeChasedBy())
            {
                Target = -1;
                return;
            }

            //叶绿弹 追踪
            float chaseSpeed = 42f;
            Vector2 center = Projectile.Center;
            Vector2 targetCenter = target2.Center;
            Vector2 dir = targetCenter - center;
            float length = dir.Length();
            //if (length < 100f)
            //    chaseSpeed = 35f;

            length = chaseSpeed / length;
            dir *= length;
            Projectile.velocity.X = ((Projectile.velocity.X * 14f) + dir.X) / 15f;
            Projectile.velocity.Y = ((Projectile.velocity.Y * 14f) + dir.Y) / 15f;
        }
    }
}
