using Coralite.Content.Particles;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue((AquamarineProj.brightC * 1.3f).ToVector4());
                effect.Parameters["brightC"].SetValue(AquamarineProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(new Color(50, 50, 160).ToVector4());
            }, 0.4f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, AquamarineProj.brightC, AquamarineProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tourmaline>()
                .AddIngredient(ItemID.FragmentNebula, 5)
                .AddIngredient(ItemID.Lens)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class TourmalineMonoclasticProj : BaseGemWeaponProj<TourmalineMonoclastic>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 32);
                Color c = Main.rand.NextFromList(Color.White, AquamarineProj.brightC, AquamarineProj.highlightC);
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
            Vector2 idlePos = Owner.Center + new Vector2(OwnerDirection * 48, 0);
            for (int i = 0; i < 8; i++)//检测头顶2个方块并尝试找到没有物块阻挡的那个
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
                if (AttackTime % 7 == 0)
                {
                    Color c = Main.rand.NextFromList(AquamarineProj.brightC, AquamarineProj.highlightC, Main.DiscoColor);
                    LightLine ll = LightLine.Spwan(Projectile.Center + (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 10, Vector2.Zero, c,
                       null, Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(0.1f, 0.4f));
                    ll.fadeTime = Main.rand.Next(15, 25);
                    ll.center = () => Projectile.Center + (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 10;
                    ll.scaleY = Main.rand.NextFloat(0.2f, 0.6f);
                }

                Projectile.rotation = MathF.Sin((1 - AttackTime / Owner.itemTimeMax) * MathHelper.TwoPi) * 0.5f;
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

    public class TourmalineProj:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();
        }


    }

    public class TourmalineSlash
    {

    }
}
