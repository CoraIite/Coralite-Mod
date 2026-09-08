using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.RedJade;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeStaff : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<RedJadeKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<RedJadeItemPage>();

        public int shootCount;
        /// <summary> 使用多少次后进行强化大爆炸的射击 </summary>
        public int useBigBoom = 8;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(20, 5f);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 0, 50, 0));

            Item.DamageType = DamageClass.Magic;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<RedJadeStaffHeldProj>();
            Item.shootSpeed = 9.5f;
            Item.mana = 9;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (shootCount >= useBigBoom) //射出大爆炸弹幕
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 1);

                shootCount = 0;
                useBigBoom = Main.rand.Next(6, 9);
            }
            else
            {
                Helper.PlayPitched("RedJade/RedJadeBeam", 0.16f, 0f, player.Center);
                Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 11f, ProjectileType<RedJadeBeam>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }

            shootCount++;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<RedJade>(14)
            .AddTile<Tiles.RedJades.MagicCraftStation>()
            .Register();
        }
    }

    /// <summary>
    /// ai[0]用于控制状态，为0时为默认发射
    /// 为1时为蓄力攻击
    /// </summary>
    public class RedJadeStaffHeldProj : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeStaff";

        public ref float State => ref Projectile.ai[0];
        protected ref float TargetRot => ref Projectile.ai[1];

        protected Player Owner => Main.player[Projectile.owner];

        private bool initialized;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Owner.itemAnimation = Owner.itemTime = 2;
            Owner.heldProj = Projectile.whoAmI;

            switch (State)
            {
                default:
                case 0: //默认
                    if (!initialized)
                        Initialize_Normal();

                    Projectile.Center = Owner.Center + (Owner.direction * Projectile.rotation.ToRotationVector2() * 20);

                    break;
                case 1: //强化
                    if (!initialized)
                        Initialize_Special();

                    if (Projectile.alpha < 255)
                    {
                        Projectile.alpha += 50;
                        if (Projectile.alpha > 255)
                            Projectile.alpha = 255;
                    }

                    if (Projectile.IsOwnedByLocalPlayer() && Projectile.timeLeft == 24)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center,
                            (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX) * 12f, ProjectileType<RedJadeBeam>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner, 1);

                        Helper.PlayPitched("RedJade/RedJadeBeam", 0.16f, 0f, Projectile.Center);
                        float r = (Main.MouseWorld - Owner.Center).ToRotation();
                        Vector2 targetDir = r.ToRotationVector2();
                        Vector2 center = Projectile.Center + Projectile.velocity;
                        for (int i = 0; i < 20; i++)
                        {
                            r += 0.314f;
                            Vector2 dir = r.ToRotationVector2() * Helper.EllipticalEase(1.85f + (0.314f * i), 1f, 3f);

                            Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, (dir * 1.4f) + (targetDir * 6), Scale: 1.8f);
                            dust.noGravity = true;
                            Dust dust2 = Dust.NewDustPerfect(center, DustID.GemRuby, (dir * 0.8f) + (targetDir * 3), Scale: 1.5f);
                            dust2.noGravity = true;
                        }
                    }

                    if (Projectile.IsOwnedByLocalPlayer())
                        TargetRot = (Main.MouseWorld - Owner.Center).ToRotation();

                    Projectile.rotation = TargetRot + (Owner.direction > 0 ? 0f : MathHelper.Pi);
                    Projectile.velocity = TargetRot.ToRotationVector2() * 32;
                    Projectile.Center = Owner.Center + (Owner.direction * Projectile.rotation.ToRotationVector2() * 20);
                    Owner.itemRotation = Projectile.rotation + (Owner.direction * 0.3f);
                    Projectile.netUpdate = true;
                    break;
            }
        }

        public void Initialize_Normal()
        {
            Projectile.timeLeft = 24;
            if (Projectile.IsOwnedByLocalPlayer())
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (Owner.direction > 0 ? 0f : 3.141f);

            Projectile.rotation = TargetRot;
            Owner.itemRotation = Projectile.rotation + (Owner.direction * 0.3f);
            Projectile.netUpdate = true;
            initialized = true;
        }

        public void Initialize_Special()
        {
            Projectile.timeLeft = 36;
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);

            initialized = true;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            Vector2 center = Projectile.Center - Main.screenPosition;

            SpriteEffects effects = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation + (Owner.direction * 0.785f), mainTex.Size() / 2, Projectile.scale, effects, 0f);

            if (Projectile.ai[0] == 1)
            {
                if (Projectile.timeLeft > 22)
                {
                    float factor = 1 - ((36 - Projectile.timeLeft) / 14f);
                    Helper.DrawPrettyStarSparkle(1, SpriteEffects.None, center + Projectile.velocity, new Color(255, 255, 255, 0) * 0.8f,
                        Coralite.RedJadeRed, factor, 0f, 0.4f, 0.6f, 1f, 0f, new Vector2(6, 3f) * factor, Vector2.One);
                }
            }

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (Projectile.ai[0] == 1)
            {
                if (Projectile.timeLeft > 22)
                {
                    Texture2D mainTex = Request<Texture2D>(AssetDirectory.Rediancie + "RedShield").Value;
                    float factor = 1 - ((36 - Projectile.timeLeft) / 14f);
                    spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition + Projectile.velocity, null, Coralite.RedJadeRed * (Projectile.alpha / 255f), Projectile.timeLeft * 0.25f, mainTex.Size() / 2, MathF.Sin(factor * 3.141f) * 0.3f, SpriteEffects.None, 0f);
                }
            }
        }
    }

    /// <summary>
    /// 使用ai[0]来控制是否能产生大爆炸，为1能大爆炸
    /// </summary>
    public class RedJadeBeam : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Vector2 center = Projectile.Center;
                if (Projectile.ai[0] == 1)
                {
                    Projectile.scale = 1.6f;
                    Projectile.localAI[1] = 588;
                }
                else
                {
                    Projectile.localAI[1] = 596;
                }

                Projectile.Center = center;
                Projectile.localAI[0] = 1;
            }
            if (Projectile.timeLeft < Projectile.localAI[1])
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, Projectile.scale);
                    dust.noGravity = true;
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                if (Projectile.ai[0] == 1)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<RedJadeBigBoom>(), Projectile.damage * 2, 0, Projectile.owner);
                else
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<RedJadeBoom>(), Projectile.damage / 2, 0, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
