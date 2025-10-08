using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeTong : BaseTongsItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override int CatchPower => 5;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<RedJadeTongProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 10;
            Item.SetWeaponValues(17, 7.5f);
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 20));
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(10)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.RedJadeItems)]
    public class RedJadeTongProj : BaseTongsProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public static ATex RedJadeTongChain { get; private set; }
        public static ATex RedJadeTongHandle { get; private set; }

        public ref float HandleHeadRot => ref Projectile.localAI[2];

        public override Vector2 TongPosOffset => new Vector2(18, -2);

        public override int MaxFlyLength => 16 * 9;

        public override Vector2 HandelOffset => new Vector2(16, -6);

        public override int ItemType => ModContent.ItemType<RedJadeTong>();

        public override Texture2D GetHandleTex() => RedJadeTongHandle.Value;
        public override Texture2D GetLineTex() => RedJadeTongChain.Value;

        public override void OnHitNPCFlying(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectileFromThis<RedJadeTongBomb>(Projectile.Center, Vector2.Zero,
                (int)(Projectile.damage * 0.6f), Projectile.knockBack, target.whoAmI);
        }

        public override Vector2 LineDrawStartPosOffset()
            => -HandleRot.ToRotationVector2() * 4;
    }

    [VaultLoaden(AssetDirectory.RedJadeItems)]
    public class RedJadeTongBomb : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeTongProj";

        public ref float TargetNPC => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public static ATex RedJadeTongBombHighLight { get; private set; }


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = true;

            Projectile.DamageType = FairyDamage.Instance;
        }

        public override bool? CanDamage()
        {
            if (State == 0)
                return false;

            return base.CanDamage();
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://黏附在NPC身上
                    {
                        if (!TargetNPC.GetNPCOwner(out NPC npc, () => State = 1))
                            break;

                        if (Timer == 0)
                        {
                            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            Projectile.AttatchToTarget(npc);
                        }

                    }
                    break;
                case 1://自由下落
                    {
                        if (Projectile.velocity.Y < 8)
                            Projectile.velocity.Y += 0.2f;
                    }
                    break;
            }

            Timer++;

            if (Timer > 60 * 4)
                Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == 0)
                return false;

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileFromThis<RedJadeBoom>(Projectile.Center, Vector2.Zero,
                Projectile.damage, 2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer == 0)
                return false;

            float factor = Timer / (60 * 4);

            Color c = Color.Lerp(Color.Transparent, Color.White * 0.5f,
                Math.Abs(Helper.SinEase(Helper.X3Ease(factor) * MathHelper.TwoPi * 2)));

            Vector2 pos = Projectile.Center - Main.screenPosition;

            Projectile.GetTexture().QuickCenteredDraw(Main.spriteBatch, pos, lightColor, Projectile.rotation);
            RedJadeTongBombHighLight.Value.QuickCenteredDraw(Main.spriteBatch, pos, c, Projectile.rotation);

            return false;
        }
    }
}
