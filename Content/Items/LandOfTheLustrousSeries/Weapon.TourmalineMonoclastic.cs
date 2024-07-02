using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class TourmalineMonoclastic : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.StrongRed10, Item.sellPrice(0, 18));
            Item.SetWeaponValues(80, 4);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 8;

            Item.shoot = ModContent.ProjectileType<TourmalineMonoclasticProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as TourmalineMonoclasticProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1.4f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.55f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(TourmalineProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(TourmalineProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(TourmalineProj.darkC.ToVector4());
            }, 0.4f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, AquamarineProj.brightC, AquamarineProj.highlightC);
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<Tourmaline>()
            //    .AddIngredient(ItemID.FragmentNebula, 5)
            //    .AddIngredient(ItemID.Lens)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
        }
    }

    public class TourmalineMonoclasticProj : BaseGemWeaponProj<TourmalineMonoclastic>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public Vector2 scale;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 32);
                Color c = Main.rand.NextFromList(Color.White, TourmalineProj.brightC, TourmalineProj.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(30, 50);
                cs.shineRange = 12;
            }
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center;
            for (int i = 0; i < 16; i++)//检测头顶2个方块并尝试找到没有物块阻挡的那个
            {
                Tile idleTile = Framing.GetTileSafely(idlePos.ToTileCoordinates());
                if (idleTile.HasTile && Main.tileSolid[idleTile.TileType] && !Main.tileSolidTop[idleTile.TileType])
                {
                    idlePos -= new Vector2(0, -4);
                    break;
                }
                else
                    idlePos += new Vector2(0, -4);
            }

            if (AttackTime > 0)
            {
                Vector2 dir = (Main.MouseWorld - Projectile.Center);
                if (dir.Length() < 48)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 48;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Projectile.rotation.AngleLerp(Owner.velocity.X / 20, 0.2f);
            Lighting.AddLight(Projectile.Center, AquamarineProj.brightC.ToVector3());
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {

                if ((int)AttackTime % (Owner.itemTimeMax / 3) == 0 && Owner.CheckMana(Owner.HeldItem.mana, true))
                {
                    Owner.manaRegenDelay = 40;

                    float angle = Main.rand.NextFromList(-1, 1) * 0.35f + Main.rand.NextFloat(-0.5f, 0.5f);
                    Projectile.NewProjectileFromThis<AquamarineProj>(Projectile.Center
                        , Vector2.UnitY.RotatedBy(angle) * 8, Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);

                    for (int i = 0; i < 4; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        AquamarineProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
            }
        }

        public override void StartAttack()
        {
            AttackTime = Owner.itemTimeMax;

            List<NPC> targets = Main.npc.Where(n => n.CanBeChasedBy() && Collision.CanHit(Owner, n)).ToList();

            targets.Sort((t1, t2) => t1.DistanceSQ(Projectile.Center).CompareTo(t2.DistanceSQ(Projectile.Center)));

            Helper.PlayPitched("Crystal/CrystalShoot", 0.4f, 0, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = new Vector2(mainTex.Width / 2, 0);
            Vector2 toCenter = new Vector2(Projectile.width / 2, 0);

            for (int i = 0; i < 4; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    new Color(251, 100, 152) * (0.3f - i * 0.3f / 4), Projectile.oldRot[i] + 0, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Top - Main.screenPosition, null, lightColor, Projectile.rotation,
                origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class TourmalineProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public static Color highlightC = Color.White;
        public static Color brightC = new Color(255, 83, 113);
        public static Color darkC = new Color(75, 7, 28);

        public ref float Owner => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (!Owner.GetProjectileOwner(out Projectile owner, Projectile.Kill)
                || !Target.GetNPCOwner(out NPC target, Projectile.Kill))
                return;

            Projectile.Center = owner.Center;
            Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
            //追踪一小段时间后生成斩击弹幕

            if (Timer < 0)
            {
                //生成斩击弹幕
                Projectile.Kill();
            }

            Timer--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Owner.GetProjectileOwner(out Projectile owner)
                || !Target.GetNPCOwner(out NPC target))
                return false;

            Vector2 dir = target.Center - owner.Center;
            float distance = dir.Length();
            dir = dir.SafeNormalize(Vector2.Zero);

            Texture2D mainTex = TextureAssets.Extra[98].Value;
            var origin = new Vector2(0, mainTex.Height / 2);
            var pos = Projectile.Center - Main.screenPosition;

            distance = Math.Clamp(distance, 0, 350);
            float factor = distance / mainTex.Width;
            Vector2 scale = new Vector2(factor, 0.5f * factor);

            Main.spriteBatch.Draw(mainTex, pos, null, Color.Red, Projectile.rotation, origin, scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, Color.White * 0.3f, Projectile.rotation, origin, scale * 0.8f, 0, 0);

            return false;
        }
    }

    public class TourmalineSlash : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
    }
}
